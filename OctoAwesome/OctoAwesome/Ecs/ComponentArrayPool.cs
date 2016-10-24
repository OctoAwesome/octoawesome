using System.Collections.Generic;

namespace OctoAwesome.Ecs
{
    public static class ComponentArrayPool
    {
        private static Stack<Component[]> _freeList;
        
        public static void Initialize(int componentCount)
        {
            const int prefill = 2000;
            
            _freeList = new Stack<Component[]>(prefill);

            for (int i = 0; i < prefill; i++)
            {
                _freeList.Push(new Component[componentCount]);
            }
        }

        public static Component[] Take()
        {
            return _freeList.Count > 0 ? _freeList.Pop() : new Component[EntityManager.ComponentCount];
        }

        public static void Release(Component[] arr, bool clear = true)
        {
            if (clear)
            {
                for (int i = 0; i < EntityManager.ComponentCount; i++)
                {
                    if (arr[i] != null)
                    {
                        // TODO: implement release method
                       // ComponentRegistry.Release(i, arr[i]);
                    }
                    arr[i] = null;
                }
            }

            _freeList.Push(arr);
        }
    }
}