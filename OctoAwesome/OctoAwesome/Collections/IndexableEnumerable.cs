using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Collections;
public struct IndexableEnumerable<T, TEnumerator>
    : IEnumerable<(int, T)> where TEnumerator : IEnumerator<T>
{
    private readonly Func<TEnumerator> instantiator;

    public IndexableEnumerable(Func<TEnumerator> instantiator)
    {
        this.instantiator = instantiator;
    }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    public Enumerator GetEnumerator() => new(this);
    IEnumerator<(int, T)> IEnumerable<(int, T)>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<(int, T)>
    {
        private readonly TEnumerator parent;
        private int index;
        public Enumerator(IndexableEnumerable<T, TEnumerator> parent)
        {
            this.parent = parent.instantiator();
            Current = default;
            index = 0;
        }
        public (int, T) Current { get; private set; }
        object IEnumerator.Current => Current;

        /// <inheritdoc/>
        public void Dispose()
        {
            parent.Dispose();
        }
        /// <inheritdoc/>
        public bool MoveNext()
        {
            var ret = parent.MoveNext();
            Current = (index++, parent.Current);
            return ret;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            parent.Reset();
        }
    }
}
