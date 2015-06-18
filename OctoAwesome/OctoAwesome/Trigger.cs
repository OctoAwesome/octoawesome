using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public class Trigger<T>
    {
        private T state;
        private T lastState;

        public T Value
        {
            get
            {
                T result = state;
                state = default(T);
                return result;
            }
            set
            {
                if (!value.Equals(default(T)) && !value.Equals(lastState))
                {
                    state = value;
                }
                lastState = value;
            }
        }

        public static implicit operator T(Trigger<T> d)
        {
            return d.Value;
        }
    }
}
