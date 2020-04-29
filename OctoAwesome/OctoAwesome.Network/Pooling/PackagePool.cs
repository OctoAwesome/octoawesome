using OctoAwesome.Pooling;
using OctoAwesome.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Network.Pooling
{
    public sealed class PackagePool : IPool<Package>
    {
        private readonly Stack<Package> internalStack;
        private readonly LockSemaphore semaphoreExtended;

        public PackagePool()
        {
            internalStack = new Stack<Package>();
            semaphoreExtended = new LockSemaphore(1, 1);
        }

        public Package Get()
        {
            Package obj;

            using (semaphoreExtended.Wait())
            {
                if (internalStack.Count > 0)
                    obj = internalStack.Pop();
                else
                    obj = new Package();
            }

            obj.Init(this);
            obj.UId = Package.NextUId;
            return obj;
        }
        public Package GetBlank()
        {
            Package obj;

            using (semaphoreExtended.Wait())
            {
                if (internalStack.Count > 0)
                    obj = internalStack.Pop();
                else
                    obj = new Package(false);
            }

            obj.Init(this);
            return obj;
        }

        public void Push(Package obj)
        {
            using (semaphoreExtended.Wait())
                internalStack.Push(obj);
        }

        public void Push(IPoolElement obj)
        {
            if (obj is Package package)
            {
                Push(package);
            }
            else
            {
                throw new InvalidCastException("Can not push object from type: " + obj.GetType());
            }
        }       
    }
}
