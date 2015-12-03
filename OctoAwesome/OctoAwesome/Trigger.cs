using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public class Trigger<T>
    {
        /// <summary>
        /// Den jetzigen Zustand des Triggers
        /// </summary>
        private T state;
        /// <summary>
        /// Der Zustand beim letzten Setzen des Triggers
        /// </summary>
        private T lastState;

        /// <summary>
        /// Der Wert des Triggers. Wird bei einem Aufruf auf den Standartwert gesetzt. Wird nur gesetzt, falls beim letzten setzen
        /// einen anderen Wert gesetzt wurde
        /// </summary>
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
