using System;
using System.Collections.Generic;

namespace OctoAwesome.Ecs
{
    public class ComponentRegistry<T> where T : Component, new()
    {
        private static Stack<T> _freeList;

        // ReSharper disable once StaticMemberInGenericType
        public static int Index;

        // ReSharper disable once UnusedMember.Local
        private static void Initialize(int index, int prefill) // Called via reflection
        {
            Index = index;
            _freeList = new Stack<T>(prefill);

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
            return _freeList.Count > 0 ? _freeList.Pop() : new T();
        }
    }
}