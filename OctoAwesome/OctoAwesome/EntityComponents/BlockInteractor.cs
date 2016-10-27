using System.IO;
using OctoAwesome.Ecs;

namespace OctoAwesome.EntityComponents
{
    public class BlockInteractor : Component<BlockInteractor>
    {
        public Index3 Target;
        public bool Interact;
        public bool Mode;

        // ReSharper disable once UnusedMember.Local (Reflection)
        private static void Deserialize(Entity target, BlockInteractor component, BinaryReader reader) { }
        public override void Serialize(Entity e, BinaryWriter writer) {}
    }
}