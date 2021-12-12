using System;
using System.Reflection;

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
