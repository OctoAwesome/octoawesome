using OctoAwesome.Pooling;
using OctoAwesome.Serialization;
using OctoAwesome.Threading;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using OctoAwesome.Extension;
using OctoAwesome.Caching;

namespace OctoAwesome
{
    /// <summary>
    /// For awaiting a result from an asynchronous task.
    /// </summary>
    public class Awaiter : IPoolElement, IDisposable
    {

        private ISerializable? knownResult;
        private byte[]? result;
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
        public bool WaitOn()
        {
            Debug.Assert(!isPooled, "Is released into pool!");
            if (!alreadyDeserialized)
                TimedOut = !manualReset.Wait(10000);

            return !TimedOut;
        }

        /// <summary>
        /// Waits on the result or time outs(10000s) and releases the awaiter.
        /// </summary>
        /// <returns>The result; or <c>null</c> if there is no result yet.</returns>
        public T? WaitOnAndRelease<T>() where T : ISerializable, new()
        {
            Debug.Assert(!isPooled, "Is released into pool!");
            var res = WaitOn();
            if (!res)
            {
                Release();
                return default;
            }

            T? ret;
            if (knownResult != default && result is null)
                ret = GenericCaster<ISerializable, T>.Cast(knownResult);
            else if(knownResult != default && result is not null)
                ret = GenericCaster<ISerializable, T>.Cast(Serializer.Deserialize(knownResult, result));
            else if (result is not null)
                ret = Serializer.Deserialize<T>(result);
            else
                ret = default;


            Release();
            return ret;
        }

        /// <summary>
        /// Waits on the result or time outs(10000s) and releases the awaiter.
        /// </summary>
        /// <returns>The result; or <c>null</c> if there is no result yet.</returns>
        public T? WaitOnAndRelease<T>(T instance) where T : class, ISerializable
        {
            Debug.Assert(!isPooled, "Is released into pool!");
            var res = WaitOn();
            if (!res)
            {
                Release();
                return null;
            }

            T? ret;
            if (knownResult is not null && result is null)
                ret = GenericCaster<ISerializable, T>.Cast(knownResult);
            else if (result is not null)
                ret = Serializer.Deserialize(instance, result);
            else
                ret = null;

            Release();
            return ret;
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
                knownResult = result;
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

                    result = bytes;
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
            result = null;
            knownResult = null;
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
            semaphore.Dispose();
        }
    }
}
