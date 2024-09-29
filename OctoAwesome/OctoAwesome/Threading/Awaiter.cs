using OctoAwesome.Pooling;
using OctoAwesome.Serialization;

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using OctoAwesome.Extension;
using OctoAwesome.Caching;

namespace OctoAwesome.Threading
{
    /// <summary>
    /// For awaiting a result from an asynchronous task.
    /// </summary>
    public class Awaiter : IPoolElement, IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether awaiting has timed out.
        /// </summary>
        public bool TimedOut { get; private set; }
        public bool Network { get; set; }

        private ISerializable? knownResult;
        private byte[]? result;
        private readonly ManualResetEventSlim manualReset;
        private readonly LockSemaphore semaphore;
        private bool alreadyDeserialized;
        private IPool? pool;
        private Func<byte[], object>? deserializeFunc;
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
            Debug.WriteLine("Waiting for result");
            if (!alreadyDeserialized)
                TimedOut = !manualReset.Wait(-1); //10000

            return !TimedOut;
        }

        /// <summary>
        /// Deserializes the local bytes based on the set local deserialization function
        /// </summary>
        /// <typeparam name="T">The type to deserialize into</typeparam>
        /// <returns>The deserialized object or null, if waiting for result failed</returns>
        public T? DeserializeFuncAndRelease<T>()
        {
            Debug.Assert(!isPooled, "Is released into pool!");
            var res = WaitOn();
            if (!res)
            {
                Release();
                return default;
            }

            T? ret = default;
            if (deserializeFunc is not null)
            {
                ret = GenericCaster<object, T>.Cast(deserializeFunc(result));
            }

            Release();
            return ret;
        }

        /// <summary>
        /// Deserializes the local bytes based on the <paramref name="deserializeFunc"/>
        /// </summary>
        /// <typeparam name="T">The type to deserialize into</typeparam>
        /// <param name="deserializeFunc">The method used for deserialization</param>
        /// <returns>The deserialized object or null, if waiting for result failed</returns>
        public T? DeserializeFuncAndRelease<T>(Func<byte[], T> deserializeFunc)
        {

            Debug.Assert(!isPooled, "Is released into pool!");
            var res = WaitOn();
            if (!res)
            {
                Release();
                return default;
            }

            T ret = deserializeFunc(result);

            Release();
            return ret;
        }

        /// <summary>
        /// Waits on the result or time outs(10000s) and releases the awaiter.
        /// </summary>
        /// <returns>The result; or <c>null</c> if there is no result yet.</returns>
        public T? WaitOnAndRelease<T>() where T : IConstructionSerializable<T>, new()
        {
            Debug.Assert(!isPooled, "Is released into pool!");
            var res = WaitOn();
            if (!res)
            {
                Release();
                return default;
            }

            T? ret;
            if (deserializeFunc is not null)
            {
                ret = GenericCaster<object, T>.Cast(deserializeFunc(result));
            }
            else if (knownResult != default && result is null)
            {
                ret = GenericCaster<ISerializable, T>.Cast(knownResult);
            }
            else if (knownResult is T instance && result is not null)
            {
                ret = Network 
                    ? Serializer.DeserializeNetwork(instance, result) 
                    : Serializer.Deserialize(instance, result);
            }
            else if (result is not null)
            {
                ret = Network 
                    ? Serializer.DeserializeSpecialCtorNetwork<T>(result) 
                    : Serializer.DeserializeSpecialCtor<T>(result);
            }
            else
            {
                ret = default;
            }


            Release();
            return ret;
        }

        /// <summary>
        /// Waits on the result or time outs(10000s) and releases the awaiter.
        /// </summary>
        /// <returns>The result; or <c>null</c> if there is no result yet.</returns>
        public T? WaitOnAndRelease<T>(T? instance) where T : class, ISerializable
        {
            Debug.Assert(!isPooled, "Is released into pool!");
            var res = WaitOn();
            if (!res)
            {
                Release();
                return null;
            }

            T? ret;
            if (deserializeFunc is not null)
                ret = GenericCaster<object, T>.Cast(deserializeFunc(result));
            else if (knownResult is not null && result is null)
                ret = GenericCaster<ISerializable, T>.Cast(knownResult);
            else if (result is not null && instance is not null)
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
                alreadyDeserialized = true;
                manualReset.Set();
            }
        }

        /// <summary>
        /// Try to set the awaiter result from a byte array.
        /// </summary>
        /// <param name="bytes">The byte array to try to deserialize the result from.</param>
        /// <returns>Whether the result could be set from the byte array.</returns>
        public bool TrySetResult(byte[] bytes)
        {
            Debug.WriteLine("Setting result");
            Debug.Assert(!isPooled, "Is released into pool!");
            try
            {
                using (semaphore.Wait())
                {
                    if (TimedOut)
                        return false;

                    result = bytes;
                    alreadyDeserialized = true;
                    manualReset.Set();
                    return true;
                }

            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SetDesializeFunc(Func<byte[], object> func)
        {
            deserializeFunc = func;
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
            deserializeFunc = null;
            Network = false;
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
