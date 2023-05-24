using OctoAwesome.Threading;

using System;
using System.Collections.Generic;

namespace OctoAwesome.Pooling
{
    /// <summary>
    /// Memory pool for <see cref="Chunk"/>.
    /// </summary>
    public sealed class ChunkPool : IPool<Chunk>
    {
        private readonly Stack<Chunk> internalStack;
        private readonly LockSemaphore semaphoreExtended;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChunkPool"/> class.
        /// </summary>
        public ChunkPool()
        {
            internalStack = new Stack<Chunk>();
            semaphoreExtended = new LockSemaphore(1, 1);
        }

        /// <inheritdoc />
        [Obsolete("Can not be used. Use Get(Index3, IPlanet) instead.", true)]
        public Chunk Rent()
        {
            throw new NotSupportedException($"Use Get(Index3, IPlanet) instead.");
        }

        /// <summary>
        /// Retrieves an element from the memory pool.
        /// </summary>
        /// <param name="position">The position to initialize the pooled element with.</param>
        /// <param name="planetId">The planet to initialize the pooled element with.</param>
        /// <returns>The pooled element that can be used thereon.</returns>
        /// <remarks>Use <see cref="Return(Chunk)"/> to return the object back into the memory pool.</remarks>
        public Chunk Rent(Index3 position, int planetId)
        {
            Chunk obj;

            using (semaphoreExtended.Wait())
            {
                if (internalStack.Count > 0)
                    obj = internalStack.Pop();
                else
                    obj = new Chunk(position, planetId);
            }

            obj.Init(position, planetId);
            return obj;
        }

        /// <inheritdoc/>
        public IPoolElement RentElement()
        {
            throw new NotSupportedException($"Use Get(Index3, IPlanet) instead.");
        }


        /// <inheritdoc />
        public void Return(Chunk obj)
        {
            using (semaphoreExtended.Wait())
                internalStack.Push(obj);
        }

        /// <inheritdoc />
        public void Return(IPoolElement obj)
        {
            if (obj is Chunk chunk)
            {
                Return(chunk);
            }
            else
            {
                throw new InvalidCastException("Can not push object of type: " + obj.GetType());
            }
        }

    }
}
