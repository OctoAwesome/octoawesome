using OctoAwesome.Database.Checks;
using OctoAwesome.Database.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace OctoAwesome.Database
{
    public abstract class Database : IDisposable
    {
        public Type TagType { get; }

        protected Database(Type tagType)
        {
            TagType = tagType;
        }

        public abstract void Open();
        public abstract void Close();
        public abstract void Dispose();
        /// <summary>
        /// Locks this Database for the specific operation
        /// </summary>
        /// <param name="mode">Indicates witch operation is currently performed</param>
        /// <returns>A new database lock</returns>
        public abstract DatabaseLock Lock(Operation mode);
    }

    public sealed class Database<TTag> : Database where TTag : ITag, new()
    {
        public bool FixedValueLength => valueStore.FixedValueLength;
        public IReadOnlyList<TTag> Keys
        {
            get
            {
                using (databaseLockMonitor.StartOperation(Operation.Read))
                    return keyStore.Tags;
            }
        }

        public bool IsOpen { get; private set; }

        /// <summary>
        /// This Threshold handels the auto defragmenation. 
        /// If the Database have more Empty Values than this Threshold the <see cref="Defragmentation"/> is executed.
        /// Use -1 to deactivate the deframentation for this Database.
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

        public Database(FileInfo keyFile, FileInfo valueFile, bool fixedValueLength) : base(typeof(TTag))
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
        public Database(FileInfo keyFile, FileInfo valueFile) : this(keyFile, valueFile, false)
        {

        }

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
                when (ex is KeyInvalidException || ex is ArgumentException)
            {
                keyStore.Close();
                defragmentation.RecreateKeyFile();
                keyStore.Open();
            }

            valueStore.Open();

            if (Threshold >= 0 && keyStore.EmptyKeys >= Threshold)
                Defragmentation();
        }

        public override void Close()
        {
            IsOpen = false;
            keyStore.Close();
            valueStore.Close();
        }

        public void Validate()
            => ExecuteOperationOnKeyValueStore(checkFunc);

        public void Defragmentation()
            => ExecuteOperationOnKeyValueStore(startDefragFunc);

        public Value GetValue(TTag tag)
        {
            using (databaseLockMonitor.StartOperation(Operation.Read))
            {
                var key = keyStore.GetKey(tag);
                return valueStore.GetValue(key);
            }
        }

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

        public bool ContainsKey(TTag tag)
        {
            using (databaseLockMonitor.StartOperation(Operation.Read))
                return keyStore.Contains(tag);
        }

        public void Remove(TTag tag)
        {
            using (databaseLockMonitor.StartOperation(Operation.Write))
            {
                keyStore.Remove(tag, out var key);
                valueStore.Remove(key);
            }
        }

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
