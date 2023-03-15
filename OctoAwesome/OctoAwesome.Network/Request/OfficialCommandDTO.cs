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

[SerializationId(1, uint.MaxValue - 1)]
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

    private static ulong ReadTypeId(BinaryReader reader) => 0;
    private void WriteTypeId(BinaryWriter writer)
    {
        if (writer.BaseStream.Position == 0)
            writer.Write(SerializationId);
    }

}