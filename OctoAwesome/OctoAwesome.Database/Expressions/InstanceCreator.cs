using System;
using System.Linq.Expressions;

namespace OctoAwesome.Database.Expressions
{
    /// <summary>
    /// Helper class for cached instance creation with an empty constructor.
    /// </summary>
    /// <typeparam name="T">The type to create the instance of.</typeparam>
    public static class InstanceCreator<T> where T : new()
    {
        /// <summary>
        /// Creates an instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>The created instance.</returns>
        public static Func<T> CreateInstance { get; }

        static InstanceCreator()
        {
            var body = Expression.New(typeof(T));
            CreateInstance = Expression.Lambda<Func<T>>(body).Compile();
        }
    }
}
