using System.IO;
using OctoAwesome.Ecs;

namespace OctoAwesome.EntityComponents
{
    public class JumpComponent : Component<JumpComponent>
    {
        public bool Jump;
        public float JumpPower;

        // ReSharper disable once UnusedMember.Local (Reflection)
        private static void Deserialize(Entity target, JumpComponent component, BinaryReader reader)
        {
            component.JumpPower = reader.ReadSingle();
        }

        public override void Serialize(Entity e, BinaryWriter writer)
        {
            writer.Write(JumpPower);
        }
    }
}