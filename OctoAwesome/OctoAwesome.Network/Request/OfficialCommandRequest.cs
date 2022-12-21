using OctoAwesome.Pooling;
using OctoAwesome.Serialization;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Network.Request;


public class OfficialCommandRequest : IPoolElement, ISerializable
{
    public OfficialCommand Command { get; set; }
    public byte[] Data { get; set; }

    private IPool pool;

    public void Init(IPool pool)
    {
        this.pool = pool;
    }

    public void Release()
    {
        pool.Return(this);
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write((ushort)Command);
        writer.Write(Data.Length);
        writer.Write(Data);
    }

    public void Deserialize(BinaryReader reader)
    {
        Command = (OfficialCommand)reader.ReadUInt16();
        var len = reader.ReadInt32();
        Data = reader.ReadBytes(len);
    }
}
