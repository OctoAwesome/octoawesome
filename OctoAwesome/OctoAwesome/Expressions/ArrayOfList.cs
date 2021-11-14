using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Expressions
{
    public static class ArrayOfList<T>
    {
        public readonly static Func<List<T>, T[]> GetArray;
        static ArrayOfList()
        {
            ParameterExpression input = Expression.Parameter(typeof(List<T>), "list");
            //ParameterExpression result = Expression.Parameter(typeof(T[]), "list");

            var body = Expression.Field(input, "_items");

            GetArray = Expression.Lambda<Func<List<T>, T[]>>(body, input).Compile();
        }
    }
}
