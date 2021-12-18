using System;
using System.Reflection;

namespace OctoAwesome.Serialization
{
    /// <summary>
    /// Extension methods for the type <see cref="Type"/>.
    /// </summary>
    public static class TypeExtension
    {
        /// <summary>
        /// Gets the <see cref="SerializationIdAttribute.CombinedId"/> from this <see cref="Type"/>
        /// or <c>0</c> if no <see cref="SerializationIdAttribute"/> is associated with this <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to get the id for.</param>
        /// <returns>
        /// The read <see cref="SerializationIdAttribute.CombinedId"/>;
        /// or <c>0</c> if no <see cref="SerializationIdAttribute"/> is associated with this <see cref="Type"/>.
        /// </returns>
        public static ulong SerializationId(this Type type)
        {
            var attr = type.GetCustomAttribute<SerializationIdAttribute>();
            if (attr is null)
                return 0;

            return attr.CombinedId;
        }
    }
}
