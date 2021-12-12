using OctoAwesome.Pooling;
using OctoAwesome.Threading;
using System;
using System.Collections.Generic;

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
        
        public Package Rent()
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
        public void Return(Package obj)
        {
            using (semaphoreExtended.Wait())
                internalStack.Push(obj);
        }
        public void Return(IPoolElement obj)
        {
            if (obj is Package package)
            {
                Return(package);
            }
            else
            {
                throw new InvalidCastException("Can not push object from type: " + obj.GetType());
            }
        }       
    }
}
