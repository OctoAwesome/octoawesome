using OctoAwesome.Pooling;
using OctoAwesome.Serialization;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using OctoAwesome.Extension;

namespace OctoAwesome.Threading
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
        private IPool? pool;
        private IPool Pool
        {
            get => NullabilityHelper.NotNullAssert(pool, $"{nameof(IPoolElement)} was not initialized!");
            set => pool = NullabilityHelper.NotNullAssert(value, $"{nameof(Pool)} cannot be initialized with null!");
        }

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
            Debug.Assert(!isPooled, "Is released into pool!");
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
            Debug.Assert(!isPooled, "Is released into pool!");
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
            Debug.Assert(!isPooled, "Is released into pool!");
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
            Debug.Assert(!isPooled, "Is released into pool!");
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
            Pool = pool;
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

                Pool.Return(this);
                pool = null;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            manualReset.Dispose();
        }
    }
}
