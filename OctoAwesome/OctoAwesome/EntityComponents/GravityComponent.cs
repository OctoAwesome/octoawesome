using System.IO;
using OctoAwesome.Ecs;

namespace OctoAwesome.EntityComponents
{
    [ComponentConfig(2000)]
    public class AffectedByGravity : Component<AffectedByGravity>
    {
        private static void Deserialize(Entity target, AffectedByGravity component, BinaryReader reader) { }

        public override void Serialize(Entity e, BinaryWriter writer) {}
    }
}
