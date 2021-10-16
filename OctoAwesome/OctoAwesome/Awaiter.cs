using OctoAwesome.Pooling;
using OctoAwesome.Serialization;
using OctoAwesome.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome
{
    public class Awaiter : IPoolElement, IDisposable
    {
        public ISerializable Serializable { get; set; }
        public bool Timeouted { get; private set; }
        private readonly ManualResetEventSlim manualReset;
        private readonly LockSemaphore semaphore;
        private bool alreadyDeserialized;
        private IPool pool;
        private bool isPooled;

        public Awaiter()
        {
            manualReset = new ManualResetEventSlim(false);
            semaphore = new LockSemaphore(1, 1);
        }

        public ISerializable WaitOn()
        {
            if (!alreadyDeserialized)
                Timeouted = !manualReset.Wait(10000);

            return Serializable;
        }

        public void WaitOnAndRelease()
        {
            WaitOn();
            Release();
        }

        public void SetResult(ISerializable serializable)
        {
            using (semaphore.Wait())
            {
                Serializable = serializable;
                manualReset.Set();
                alreadyDeserialized = true;
            }
        }

        public bool TrySetResult(byte[] bytes)
        {
            try
            {
                using (semaphore.Wait())
                {
                    if (Timeouted)
                        return false;

                    if (Serializable == null)
                        throw new ArgumentNullException(nameof(Serializable));

                    using (var stream = new MemoryStream(bytes))
                    using (var reader = new BinaryReader(stream))
                    {
                        Serializable.Deserialize(reader);
                    }
                    manualReset.Set();
                    return alreadyDeserialized = true;
                }

            }
            catch (Exception ex)
            {
                ;
                return false;
            }
        }

        public void Init(IPool pool)
        {
            this.pool = pool;
            Timeouted = false;
            isPooled = false;
            alreadyDeserialized = false;
            Serializable = null;
            manualReset.Reset();
        }

        public void Release()
        {
            using (semaphore.Wait())
            {
                if (!manualReset.IsSet)
                    manualReset.Set();

                isPooled = true;

                pool.Push(this);
            }
        }

        public void Dispose()
        {
            manualReset.Dispose();
        }
    }
}
