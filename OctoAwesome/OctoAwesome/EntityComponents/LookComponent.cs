using System.IO;
using engenious;
using OctoAwesome.Ecs;

namespace OctoAwesome.EntityComponents
{
    public class LookComponent : Component<LookComponent>
    {
        public Vector2 Head;
        public float Angle;
        public float Tilt;

        // ReSharper disable once UnusedMember.Local (Reflection)
        private static void Deserialize(Entity target, LookComponent component, BinaryReader reader)
        {
            component.Head = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            component.Angle = reader.ReadSingle();
            component.Tilt = reader.ReadSingle();
        }

        public override void Serialize(Entity e, BinaryWriter writer)
        {
            writer.Write(Head.X);
            writer.Write(Head.Y);
            writer.Write(Angle);
            writer.Write(Tilt);
        }
    }
}