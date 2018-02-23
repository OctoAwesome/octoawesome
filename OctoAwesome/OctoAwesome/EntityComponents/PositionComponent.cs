using engenious;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.EntityComponents
{
    //public sealed class PositionComponent : EntityComponent
    //{
    //    public Coordinate Position { get; set; }

    //    public float Azimuth { get; set; }

    //    public override void Serialize(BinaryWriter writer, IDefinitionManager definitionManager)
    //    {
    //        base.Serialize(writer, definitionManager);
    //        // Position
    //        writer.Write(Position.Planet);
    //        writer.Write(Position.GlobalBlockIndex.X);
    //        writer.Write(Position.GlobalBlockIndex.Y);
    //        writer.Write(Position.GlobalBlockIndex.Z);
    //        writer.Write(Position.BlockPosition.X);
    //        writer.Write(Position.BlockPosition.Y);
    //        writer.Write(Position.BlockPosition.Z);
    //        writer.Write(Position.ChunkIndex.X);
    //        writer.Write(Position.ChunkIndex.Y);
    //        writer.Write(Position.ChunkIndex.Z);
    //    }

    //    public override void Deserialize(BinaryReader reader, IDefinitionManager definitionManager)
    //    {
    //        base.Deserialize(reader, definitionManager);
    //        Position = new Coordinate(reader.ReadInt32(), 
    //            new Index3(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()), 
    //            new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle()));
    //    }
    //}
}
