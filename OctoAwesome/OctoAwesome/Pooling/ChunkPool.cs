using OctoAwesome.Chunking;
using OctoAwesome.Location;
using OctoAwesome.Threading;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Chunk Get()
        {
            throw new NotSupportedException($"Use Get(Index3, IPlanet) instead.");
        }
        public Chunk Get(Index3 position, IPlanet planet)
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
   

        public void Push(Chunk obj)
        {
            using (semaphoreExtended.Wait())
                internalStack.Push(obj);
        }

        public void Push(IPoolElement obj)
        {
            if (obj is Chunk chunk)
            {
                Push(chunk);
            }
            else
            {
                throw new InvalidCastException("Can not push object from type: " + obj.GetType());
            }
        }
     
    }
}
