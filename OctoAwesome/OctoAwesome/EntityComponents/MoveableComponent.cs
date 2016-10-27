using System.IO;
using engenious;
using OctoAwesome.Ecs;

namespace OctoAwesome.EntityComponents
{
    [ComponentConfig(1000)]
    public sealed class MoveableComponent : Component<MoveableComponent>
    {
        public Vector3 Velocity;
        public Vector2 Move;
        public float Mass;
        public Vector3 Force;
        public Vector3 Power;

        public override void CopyTo(MoveableComponent other)
        {
            other.Velocity = Velocity;
            other.Move = Move;
            other.Mass = Mass;
            other.Force = Force;
            other.Power = Power;
        }

        // ReSharper disable once UnusedMember.Local (Reflection)
        private static void Deserialize(Entity target, MoveableComponent component, BinaryReader reader)
        {
            component.Velocity = new Vector3(reader.ReadSingle(), reader.ReadSingle(),reader.ReadSingle());
            component.Move = new Vector2(reader.ReadSingle(), reader.ReadSingle());
            component.Mass = reader.ReadSingle();
            component.Force = new Vector3(reader.ReadSingle(), reader.ReadSingle(),reader.ReadSingle());
            component.Power = new Vector3(reader.ReadSingle(), reader.ReadSingle(),reader.ReadSingle());
        }

        public override void Serialize(Entity e, BinaryWriter writer)
        {
            writer.Write(Velocity.X);
            writer.Write(Velocity.Y);
            writer.Write(Velocity.Z);
           
            writer.Write(Move.X);
            writer.Write(Move.Y);

            writer.Write(Mass);

            writer.Write(Force.X);
            writer.Write(Force.Y);
            writer.Write(Force.Z);

            writer.Write(Power.X);
            writer.Write(Power.Y);
            writer.Write(Power.Z);
        }
    }
}
