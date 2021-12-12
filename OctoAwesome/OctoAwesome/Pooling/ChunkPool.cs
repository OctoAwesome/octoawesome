using OctoAwesome.Threading;

using System;
using System.Collections.Generic;

namespace OctoAwesome.Pooling
{

    public sealed class ChunkPool : IPool<Chunk>
    {
        private readonly Stack<Chunk> internalStack;
        private readonly LockSemaphore semaphoreExtended;
        public ChunkPool()
        {
            internalStack = new Stack<Chunk>();
            semaphoreExtended = new LockSemaphore(1, 1);
        }
        [Obsolete("Can not be used. Use Get(Index3, IPlanet) instead.", true)]
        public Chunk Rent()
        {
            throw new NotSupportedException($"Use Get(Index3, IPlanet) instead.");
        }
        public Chunk Rent(Index3 position, IPlanet planet)
        {
            Chunk obj;

            using (semaphoreExtended.Wait())
            {
                if (internalStack.Count > 0)
                    obj = internalStack.Pop();
                else
                    obj = new Chunk(position, planet);
            }

            obj.Init(position, planet);
            return obj;
        }

        public void Return(Chunk obj)
        {
            using (semaphoreExtended.Wait())
                internalStack.Push(obj);
        }
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
