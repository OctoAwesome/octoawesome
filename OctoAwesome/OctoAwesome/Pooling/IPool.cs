using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Pooling
{
    public interface IPool
    {
        void Push(IPoolElement obj);
    }
    public interface IPool<T> : IPool where T : IPoolElement, new() 
    {
        T Get();

        void Push(T obj);
    }
}
