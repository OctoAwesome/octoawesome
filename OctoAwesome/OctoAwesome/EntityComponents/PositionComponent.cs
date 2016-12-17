using engenious;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.EntityComponents
{
    public sealed class PositionComponent : EntityComponent
    {

        public Coordinate Position { get; set; }

        public override void Serialize(BinaryWriter writer, IDefinitionManager definitionManager)
        {
            base.Serialize(writer, definitionManager);
            // Position
            writer.Write(Position.Planet);
            writer.Write(Position.GlobalBlockIndex.X);
            writer.Write(Position.GlobalBlockIndex.Y);
            writer.Write(Position.GlobalBlockIndex.Z);
            writer.Write(Position.BlockPosition.X);
            writer.Write(Position.BlockPosition.Y);
            writer.Write(Position.BlockPosition.Z);
        }

        public override void Deserialize(BinaryReader reader, IDefinitionManager definitionManager)
        {
            base.Deserialize(reader, definitionManager);

            // Position
            int planet = reader.ReadInt32();
            int blockX = reader.ReadInt32();
            int blockY = reader.ReadInt32();
            int blockZ = reader.ReadInt32();
            float posX = reader.ReadSingle();
            float posY = reader.ReadSingle();
            float posZ = reader.ReadSingle();
            Position = new Coordinate(planet, new Index3(blockX, blockY, blockZ), new Vector3(posX, posY, posZ));
        }
    }
}
