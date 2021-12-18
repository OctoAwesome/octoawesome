using OctoAwesome.Database.Checks;
using OctoAwesome.Database.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace OctoAwesome.Database
{
    /// <summary>
    /// Base class for OctoAwesome database implementations.
    /// </summary>
    public abstract class Database : IDisposable
    {
        /// <summary>
        /// Gets the type for tags that are held in this database.
        /// </summary>
        public Type TagType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class.
        /// </summary>
        /// <param name="tagType">The type for tags that are held in this database.</param>
        protected Database(Type tagType)
        {
            TagType = tagType;
        }

        /// <summary>
        /// Opens the database.
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// Closes the database.
        /// </summary>
        public abstract void Close();

        /// <inheritdoc />
        public abstract void Dispose();
        /// <summary>
        /// Locks this Database for the specific operation
        /// </summary>
        /// <param name="mode">Indicates witch operation is currently performed</param>
        /// <returns>A new database lock</returns>
        public abstract DatabaseLock Lock(Operation mode);
    }

    /// <summary>
    /// Database using a generic type of tags for identification.
    /// </summary>
    /// <typeparam name="TTag">The type of the identifying tags.</typeparam>
    public sealed class Database<TTag> : Database where TTag : ITag, new()
    {
        /// <summary>
        /// Gets a value indicating whether the stored value data has a fixed length.
        /// </summary>
        public bool FixedValueLength => valueStore.FixedValueLength;

        /// <summary>
        /// Gets a list of keys associated to the database.
        /// </summary>
        public IReadOnlyList<TTag> Keys
        {
            get
            {
                using (databaseLockMonitor.StartOperation(Operation.Read))
                    return keyStore.Tags;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the database is open.
        /// </summary>
        public bool IsOpen { get; private set; }

        /// <summary>
        /// This Threshold handles the auto defragmentation.
        /// If the Database have more Empty Values than this Threshold the <see cref="Defragment"/> is executed.
        /// Use -1 to deactivate the defragmentation for this Database.
        /// Default Value is 1000.
        /// </summary>
        public int Threshold { get; set; }

        private Action startDefragFunc;
        private Action checkFunc;
        private readonly KeyStore<TTag> keyStore;
        private readonly ValueStore valueStore;
        private readonly Defragmentation<TTag> defragmentation;
        private readonly ValueFileCheck<TTag> fileCheck;
        private readonly FileInfo keyFile;
        private readonly FileInfo valueFile;
        private readonly DatabaseLockMonitor databaseLockMonitor;
        private readonly SemaphoreSlim dbLockSemaphore;

        /// <summary>
        /// Initializes a new instance of the <see cref="Database{TTag}"/> class.
        /// </summary>
        /// <param name="keyFile">The <see cref="FileInfo"/> of the file that contains the database tag keys.</param>
        /// <param name="valueFile">The <see cref="FileInfo"/> of the file that contains the database values.</param>
        /// <param name="fixedValueLength">A value indicating whether the stored value data has a fixed length.</param>
        public Database(FileInfo keyFile, FileInfo valueFile, bool fixedValueLength = false) : base(typeof(TTag))
        {
            dbLockSemaphore = new SemaphoreSlim(1, 1);
            databaseLockMonitor = new DatabaseLockMonitor();
            keyStore = new KeyStore<TTag>(new Writer(keyFile), new Reader(keyFile));
            valueStore = new ValueStore(new Writer(valueFile), new Reader(valueFile), fixedValueLength);
            defragmentation = new Defragmentation<TTag>(keyFile, valueFile);
            fileCheck = new ValueFileCheck<TTag>(valueFile);
            this.keyFile = keyFile;
            this.valueFile = valueFile;
            Threshold = 1000;
            startDefragFunc = defragmentation.StartDefragmentation;
            checkFunc = fileCheck.Check;
        }

        /// <inheritdoc />
        public override void Open()
        {
            IsOpen = true;

            if (valueFile.Exists && valueFile.Length > 0 && (!keyFile.Exists || keyFile.Length == 0))
                defragmentation.RecreateKeyFile();

            try
            {
                keyStore.Open();
            }
            catch (Exception ex)
                when (ex is InvalidKeyException || ex is ArgumentException)
            {
                keyStore.Close();
                defragmentation.RecreateKeyFile();
                keyStore.Open();
            }

            valueStore.Open();

            if (Threshold >= 0 && keyStore.EmptyKeys >= Threshold)
                Defragment();
        }

        /// <inheritdoc />
        public override void Close()
        {
            IsOpen = false;
            keyStore.Close();
            valueStore.Close();
        }

        /// <summary>
        /// Validates the database value file.
        /// </summary>
        public void Validate()
            => ExecuteOperationOnKeyValueStore(checkFunc);

        /// <summary>
        /// Defragments the database for more optimized storage and access.
        /// </summary>
        public void Defragment()
            => ExecuteOperationOnKeyValueStore(startDefragFunc);

        /// <summary>
        /// Gets the value associated to a specified tag.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public Value GetValue(TTag tag)
        {
            using (databaseLockMonitor.StartOperation(Operation.Read))
            {
                var key = keyStore.GetKey(tag);
                return valueStore.GetValue(key);
            }
        }

        /// <summary>
        /// Add or update a value associated to a specified tag key.
        /// </summary>
        /// <param name="tag">The tag key to associate the value with.</param>
        /// <param name="value">The new or updated value.</param>
        public void AddOrUpdate(TTag tag, Value value)
        {
            using (databaseLockMonitor.StartOperation(Operation.Write))
            {
                var contains = keyStore.Contains(tag);
                if (contains)
                {
                    var key = keyStore.GetKey(tag);

                    if (FixedValueLength)
                    {
                        valueStore.Update(key, value);
                    }
                    else
                    {
                        valueStore.Remove(key);
                    }
                }

                var newKey = valueStore.AddValue(tag, value);

                if (contains)
                    keyStore.Update(newKey);
                else
                    keyStore.Add(newKey);
            }
        }

        /// <summary>
        /// Checks whether the database contains a value for an identifying tag.
        /// </summary>
        /// <param name="tag">The tag to check for.</param>
        /// <returns>A value indicating whether the database contains a value for an identifying tag.</returns>
        public bool ContainsKey(TTag tag)
        {
            using (databaseLockMonitor.StartOperation(Operation.Read))
                return keyStore.Contains(tag);
        }

        /// <summary>
        /// Removes a value from the database identified by a tag.
        /// </summary>
        /// <param name="tag">The identifying tag to remove the value of.</param>
        public void Remove(TTag tag)
        {
            using (databaseLockMonitor.StartOperation(Operation.Write))
            {
                keyStore.Remove(tag, out var key);
                valueStore.Remove(key);
            }
        }

        /// <inheritdoc />
        public override DatabaseLock Lock(Operation mode)
        {
            //Read -> Blocks Write && Other read is ok
            //Exclusive -> Blocks every other operation

            //Write -> Blocks Read && Other write is ok
            //Exclusive -> Blocks every other operation
            dbLockSemaphore.Wait();
            try
            {
                if (!databaseLockMonitor.CheckLock(mode))
                {
                    databaseLockMonitor.Wait(mode);
                }

                var dbLock = new DatabaseLock(databaseLockMonitor, mode);
                dbLock.Enter();
                return dbLock;
            }
            finally
            {
                dbLockSemaphore.Release();
            }
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            keyStore.Dispose();
            valueStore.Dispose();

            databaseLockMonitor.Dispose();
            dbLockSemaphore.Dispose();
        }

        private void ExecuteOperationOnKeyValueStore(Action action)
        {
            if (IsOpen)
            {
                keyStore.Close();
                valueStore.Close();
            }

            action();

            if (IsOpen)
            {
                keyStore.Open();
                valueStore.Open();
            }
        }
    }
}
