using OctoAwesome.Pooling;
using OctoAwesome.Serialization;
using OctoAwesome.Threading;
using System;
using System.IO;
using System.Threading;

namespace OctoAwesome
{

    public class Awaiter : IPoolElement, IDisposable
    {

        public ISerializable? Result { get; set; }
        public bool TimedOut { get; private set; }
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

        public ISerializable? WaitOn()
        {
            if (!alreadyDeserialized)
                TimedOut = !manualReset.Wait(10000);

            return Result;
        }

        public ISerializable? WaitOnAndRelease()
        {
            var res = WaitOn();
            Release();
            return res;
        }

        public void SetResult(ISerializable result)
        {
            using (semaphore.Wait())
            {
                Result = result;
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
                    if (TimedOut)
                        return false;

                    if (Result == null)
                        throw new ArgumentNullException(nameof(Result));

                    using (var stream = new MemoryStream(bytes))
                    using (var reader = new BinaryReader(stream))
                    {
                        Result.Deserialize(reader);
                    }
                    manualReset.Set();
                    return alreadyDeserialized = true;
                }

            }
            catch (Exception)
            {
                return false;
            }
        }
        public void Init(IPool pool)
        {
            this.pool = pool;
            TimedOut = false;
            isPooled = false;
            alreadyDeserialized = false;
            Result = null;
            manualReset.Reset();
        }
        public void Release()
        {
            using (semaphore.Wait())
            {
                if (!manualReset.IsSet)
                    manualReset.Set();

                isPooled = true;

                pool.Return(this);
            }
        }
        public void Dispose()
        {
            manualReset.Dispose();
        }
    }
}
