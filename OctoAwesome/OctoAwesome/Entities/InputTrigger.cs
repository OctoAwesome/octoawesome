using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Entities
{
    public class InputTrigger<T>
    {
        public T Value { get; private set; }
        public void Set(T value)
        {
            Value = value;
        }
        public void Validate()
        {
            Value = default;
        }
    }
}
