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

[SerializationId(1, uint.MaxValue-1)]
[Nooson]
public partial class OfficialCommandDTO : IPoolElement, INoosonSerializable<OfficialCommandDTO>
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

}