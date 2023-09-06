using OctoAwesome.Pooling;
using OctoAwesome.Serialization;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NonSucking.Framework.Serialization;

namespace OctoAwesome.Network.Request;

[SerializationId()]
[Nooson]
public partial class OfficialCommandDTO : IPoolElement, IConstructionSerializable<OfficialCommandDTO>
{
    [NoosonCustom(SerializeMethodName = nameof(WriteTypeId), DeserializeMethodName = nameof(ReadTypeId))]
    [NoosonOrder(0)]
    [NoosonInclude]
    private static readonly ulong SerializationId = typeof(OfficialCommandDTO).SerializationId();

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

    private static ulong ReadTypeId(BinaryReader _) => 0;
    private void WriteTypeId(BinaryWriter writer)
    {
        if (writer.BaseStream.Position == 0) //required because we sometimes have it already written on the outside
            writer.Write(SerializationId);
    }
}