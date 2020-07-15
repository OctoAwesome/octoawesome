using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Serialization
{
    public interface ISerializableEnumerable<T> : IEnumerable<T>, ISerializable where T : ISerializable
    {
    }
}
