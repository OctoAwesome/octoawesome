using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace OctoAwesome.Database.Expressions
{
    public static class InstanceCreator<T> where T : new()
    {
        public static Func<T> CreateInstance { get; }

        static InstanceCreator()
        {
            var body = Expression.New(typeof(T));
            CreateInstance = Expression.Lambda<Func<T>>(body).Compile();
        }
    }
}
