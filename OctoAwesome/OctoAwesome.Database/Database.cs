using OctoAwesome.Database.Checks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

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
        public IEnumerable<TTag> Keys => keyStore.Tags;

        public bool IsOpen { get; private set; }

        /// <summary>
        /// This Threshold handels the auto defragmenation. 
        /// If the Database have more Empty Values than this Threshold the <see cref="Defragmentation"/> is executed.
        /// Use -1 to deactivate the deframentation for this Database.
        /// Default Value is 1000.
        /// </summary>
        public int Threshold { get; set; }

        private readonly KeyStore<TTag> keyStore;
        private readonly ValueStore valueStore;
        private readonly Defragmentation<TTag> defragmentation;
        private readonly ValueFileCheck<TTag> fileCheck;
        private readonly FileInfo keyFile;
        private readonly FileInfo valueFile;

        public Database(FileInfo keyFile, FileInfo valueFile, bool fixedValueLength) : base(typeof(TTag))
        {

            keyStore = new KeyStore<TTag>(new Writer(keyFile), new Reader(keyFile));
            valueStore = new ValueStore(new Writer(valueFile), new Reader(valueFile), fixedValueLength);
            defragmentation = new Defragmentation<TTag>(keyFile, valueFile);
            fileCheck = new ValueFileCheck<TTag>(valueFile);
            this.keyFile = keyFile;
            this.valueFile = valueFile;
            Threshold = 1000;
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
                when(ex is KeyInvalidException || ex is ArgumentException)
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
            => ExecuteOperationOnKeyValueStore(fileCheck.Check);

        public void Defragmentation()
            => ExecuteOperationOnKeyValueStore(defragmentation.StartDefragmentation);

        public Value GetValue(TTag tag)
        {
            var key = keyStore.GetKey(tag);
            return valueStore.GetValue(key);
        }

        public void AddOrUpdate(TTag tag, Value value)
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

        public bool ContainsKey(TTag tag)
            => keyStore.Contains(tag);

        public void Remove(TTag tag)
        {
            keyStore.Remove(tag, out var key);
            valueStore.Remove(key);
        }

        public override DatabaseLock Lock(Operation mode)
        {
            //1 counted für Read (1 komplett block)
            //1 counted für Write (1 komplett block)

            if (mode.HasFlag(Operation.Read))
            {
                //Read -> Blocks Write && Other read is ok
                //Exclusive -> Blocks every other operation
            }

            if (mode.HasFlag(Operation.Write))
            {
                //Write -> Blocks Read && Other write is ok
                //Exclusive -> Blocks every other operation
            }

            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            keyStore.Dispose();
            valueStore.Dispose();
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
