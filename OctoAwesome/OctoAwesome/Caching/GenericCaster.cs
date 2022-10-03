using System;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
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
        /// Delegate for casting from one generic type to another.
        /// </summary>
        /// <param name="toCast">The instance to cast.</param>
        [return: NotNullIfNotNull(nameof(toCast))]
        public delegate TTo? CastFromTo(TFrom? toCast);

        /// <summary>
        /// Casts type of <typeparamref name="TFrom"/> to type of <typeparamref name="TTo"/>.
        /// </summary>
        public static CastFromTo Cast { get; }
    
        static GenericCaster()
        {
            var param = Expression.Parameter(typeof(TFrom), "tFrom");
            Cast = Expression.Lambda<CastFromTo>(Expression.Convert(param, typeof(TTo)), param).Compile();
        }

        /// <summary>
        /// Casts a generic list to another generic typed list.
        /// </summary>
        /// <typeparam name="TListFrom">The generic enumerable type to cast from.</typeparam>
        /// <typeparam name="TListTo">The generic collection type to cast to.</typeparam>
        /// <param name="list">The list to convert.</param>
        /// <returns>The converted list.</returns>
        public static TListTo CastList<TListFrom, TListTo>(TListFrom list) 
            where TListFrom : IEnumerable<TFrom>
            where TListTo : ICollection<TTo>, new()
        {
            var to = new TListTo();
            foreach (var item in list)
                to.Add(Cast(item)!);
            
            return to;
        }
    }
}
