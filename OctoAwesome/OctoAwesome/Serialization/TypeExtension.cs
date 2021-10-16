using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Serialization
{
    public static class TypeExtension
    {
        public static ulong SerializationId(this Type type)
        {
            var attr = type.GetCustomAttribute<SerializationIdAttribute>();
            if (attr is null)
                return 0;
            
            return attr.CombinedId;
        }
    }
}
