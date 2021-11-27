using System;
using System.Linq.Expressions;

namespace OctoAwesome.Caching
{
    public static class GenericCaster<TTo, TFrom>
    {
        public static Func<TFrom, TTo> Cast { get; }
        static GenericCaster()
        {
            var param = Expression.Parameter(typeof(TFrom), "tFrom");
            Cast = Expression.Lambda<Func<TFrom, TTo>>(Expression.Convert(param, typeof(TTo)), param).Compile();
        }
    }
}
