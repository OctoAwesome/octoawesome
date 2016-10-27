using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace OctoAwesome.Ecs
{
    public static class ComponentRegistry
    {
        public static Action<Component>[] Release;
    }

    public class ComponentRegistry<T> where T : Component, new()
    {
        private static ConcurrentStack<T> _freeList;

        // ReSharper disable once StaticMemberInGenericType
        public static int Index;

        // ReSharper disable once UnusedMember.Local
        private static void Initialize(int index, int prefill) // Called via reflection
        {
            Index = index;
            _freeList = new ConcurrentStack<T>();

            for (int i = 0; i < prefill; i++)
            {
                _freeList.Push(new T());
            }
        }
        
        public static void Release(T item)
        {
            item.Reset();
            _freeList.Push(item);
        }

        public static T Take()
        {
            T item;

            if (!_freeList.TryPop(out item))
                item = new T();

            return item;
        }
    }
}