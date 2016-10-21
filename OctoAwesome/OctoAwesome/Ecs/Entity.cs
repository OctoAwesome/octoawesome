using System.Collections;

namespace OctoAwesome.Ecs
{
    public class Entity
    {
        public BitArray Flags;
        public bool Complete;
        public EntityManager Manager;

        public T Get<T>() where T : Component, new()
        {
            return ComponentRegistry<T>.Get(this);
        }
    }
}