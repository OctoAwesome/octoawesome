using System.IO;
using engenious;
using OctoAwesome.Ecs;

namespace OctoAwesome.EntityComponents
{
    [ComponentConfig(2000)]
    public sealed class PositionComponent : Component<PositionComponent>
    {
        public Coordinate Coordinate;
        public Vector3 Dimensions;
        public float Radius;
        public float Height;
        public IPlanet Planet;
        public bool OnGround;

        public override void CopyTo(PositionComponent other)
        {
            other.Coordinate = Coordinate;
            other.Dimensions = Dimensions;
            other.Radius = Radius;
            other.Height = Height;
            other.Planet = Planet;
            other.OnGround = OnGround;
        }

        private static void Deserialize(Entity target, PositionComponent component, BinaryReader reader)
        {
            var planet = reader.ReadInt32();
            component.Coordinate = new Coordinate(planet, 
                new Index3(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()),
                new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()) 
            );
            component.Dimensions = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
            component.Radius = reader.ReadSingle();
            component.Height = reader.ReadSingle();
            component.OnGround = reader.ReadBoolean();
        }

        public override void Serialize(Entity e, BinaryWriter writer)
        {
            writer.Write(Coordinate.Planet);
            writer.Write(Coordinate.Block.X);
            writer.Write(Coordinate.Block.Y);
            writer.Write(Coordinate.Block.Z);
            writer.Write(Coordinate.Position.X);
            writer.Write(Coordinate.Position.Y);
            writer.Write(Coordinate.Position.Z);
            writer.Write(Dimensions.X);
            writer.Write(Dimensions.Y);
            writer.Write(Dimensions.Z);
            writer.Write(Radius);
            writer.Write(Height);
            writer.Write(OnGround);
        }
    }
}