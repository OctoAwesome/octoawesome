using System.IO;
using OctoAwesome.Ecs;

namespace OctoAwesome.EntityComponents
{
    [ComponentConfig(10000)]
    public class CollisionComponent : Component<CollisionComponent>
    {
        private static void Deserialize(Entity target, CollisionComponent component, BinaryReader reader) {}
        public override void Serialize(Entity e, BinaryWriter writer) { }
    }
}
