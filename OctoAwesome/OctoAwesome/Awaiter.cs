using OctoAwesome.Pooling;
using OctoAwesome.Serialization;
using OctoAwesome.Threading;
using System;
using System.IO;
using System.Threading;

namespace OctoAwesome
{
    /// <summary>
    /// For awaiting a result from an asynchronous task.
    /// </summary>
    public class Awaiter : IPoolElement, IDisposable
    {
        /// <summary>
        /// Gets or sets the result for the awaiter.
        /// </summary>
        public ISerializable? Result { get; set; }

        /// <summary>
        /// Gets a value indicating whether awaiting has timed out.
        /// </summary>
        public bool TimedOut { get; private set; }
        private readonly ManualResetEventSlim manualReset;
        private readonly LockSemaphore semaphore;
        private bool alreadyDeserialized;
        private IPool pool;
        private bool isPooled;

        /// <summary>
        /// Initializes a new instance of the <see cref="Awaiter"/> class.
        /// </summary>
        public Awaiter()
        {
            manualReset = new ManualResetEventSlim(false);
            semaphore = new LockSemaphore(1, 1);
        }

        /// <summary>
        /// Waits on the result or time outs(10000s).
        /// </summary>
        /// <returns>The result; or <c>null</c> if there is no result yet.</returns>
        public ISerializable? WaitOn()
        {
            if (!alreadyDeserialized)
                TimedOut = !manualReset.Wait(10000);

            return Result;
        }

        /// <summary>
        /// Waits on the result or time outs(10000s) and releases the awaiter.
        /// </summary>
        /// <returns>The result; or <c>null</c> if there is no result yet.</returns>
        public ISerializable? WaitOnAndRelease()
        {
            var res = WaitOn();
            Release();
            return res;
        }

        /// <summary>
        /// Sets the awaiter result to the given value.
        /// </summary>
        /// <param name="result"></param>
        public void SetResult(ISerializable result)
        {
            using (semaphore.Wait())
            {
                Result = result;
                manualReset.Set();
                alreadyDeserialized = true;
            }
        }

        /// <summary>
        /// Try to set the awaiter result from a byte array.
        /// </summary>
        /// <param name="bytes">The byte array to try to deserialize the result from.</param>
        /// <returns>Whether the result could be set from the byte array.</returns>
        /// <exception cref="ArgumentNullException">Throws when <see cref="Result"/> is <c>null</c>.</exception>
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

        /// <inheritdoc />
        public void Init(IPool pool)
        {
            this.pool = pool;
            TimedOut = false;
            isPooled = false;
            alreadyDeserialized = false;
            Result = null;
            manualReset.Reset();
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void Dispose()
        {
            manualReset.Dispose();
        }
    }
}
