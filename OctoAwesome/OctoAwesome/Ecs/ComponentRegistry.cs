using System.Collections.Generic;

namespace OctoAwesome.Ecs
{
    public class ComponentRegistry<T> where T : Component, new()
    {
        public static Dictionary<Entity, T> Map;

        private static Stack<T> _freeList;

        // ReSharper disable once StaticMemberInGenericType
        public static int Index;


        // ReSharper disable once UnusedMember.Local
        private static void Initialize(int index, int prefill, int expectedEntityCount) // Called via reflection
        {
            Index = index;
            _freeList = new Stack<T>(prefill);

            for (int i = 0; i < prefill; i++)
            {
                _freeList.Push(new T());
            }
            Map = new Dictionary<Entity, T>(expectedEntityCount);
        }

        public static T Get(Entity entity)
        {
            T c;
            return Map.TryGetValue(entity, out c) ? c : null;
        }

        public static void Remove(Entity entity)
        {
            T c;
            if (!Map.TryGetValue(entity, out c))
                return;

            c.Reset();
            _freeList.Push(c);
            Map.Remove(entity);
        }

        public static T Add(Entity entity)
        {
            var c = _freeList.Count > 0 ? _freeList.Pop() : new T();
            Map[entity] = c;
            return c;
        }
    }
}