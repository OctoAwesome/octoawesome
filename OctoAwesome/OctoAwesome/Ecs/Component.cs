using System.IO;

namespace OctoAwesome.Ecs
{
    public abstract class Component
    {
        public virtual void Reset() { }
        public abstract void CopyTo(Component other);
        public abstract void Serialize(Entity e, BinaryWriter writer);
    }

    public abstract class Component<T> : Component where T : Component<T>
    {
        public virtual void CopyTo(T other) { }
        public override void CopyTo(Component other)
        {
            CopyTo((T)other);
        }
    }
}