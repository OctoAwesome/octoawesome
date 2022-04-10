using System;
using System.Linq.Expressions;

namespace OctoAwesome.Caching
{
    /// <summary>
    /// Generic caster that can cast one generic type to another.
    /// </summary>
    /// <typeparam name="TFrom">The generic type to cast.</typeparam>
    /// <typeparam name="TTo">The generic type to cast to.</typeparam>
    public static class GenericCaster<TFrom, TTo>
    {
        /// <summary>
        /// Casts type of <typeparamref name="TFrom"/> to type of <typeparamref name="TTo"/>.
        /// </summary>
        public static Func<TFrom, TTo> Cast { get; }
        static GenericCaster()
        {
            var param = Expression.Parameter(typeof(TFrom), "tFrom");
            Cast = Expression.Lambda<Func<TFrom, TTo>>(Expression.Convert(param, typeof(TTo)), param).Compile();
        }
    }
}
