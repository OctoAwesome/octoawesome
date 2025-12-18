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
        /// Gets the <see cref="SerializationIdAttribute{Type}.CombinedId"/> from this <see cref="Type"/>
        /// or <c>0</c> if no <see cref="SerializationIdAttribute"/> is associated with this <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to get the id for.</param>
        /// <returns>
        /// The read <see cref="SerializationIdAttribute{Type}.CombinedId"/>;
        /// or <c>0</c> if no <see cref="SerializationIdAttribute"/> is associated with this <see cref="Type"/>.
        /// </returns>
        public static ulong SerializationId(this Type type)
        {
            if (!SerializationIdTypeProvider.TryGet(type, out var serId))
                return 0;
            return serId;
        }
    }
}
