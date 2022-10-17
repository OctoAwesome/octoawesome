using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Collections;
/// <summary>
/// Enumerable to enumerate through an enumerator and including the item index.
/// </summary>
/// <typeparam name="T">The type of the items to enumerate through.</typeparam>
/// <typeparam name="TEnumerator">The type of the enumerator to enumerate through.</typeparam>
public readonly struct IndexableEnumerable<T, TEnumerator>
    : IEnumerable<(int, T)> where TEnumerator : IEnumerator<T>
{
    private readonly Func<TEnumerator> instantiator;

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexableEnumerable{T,TEnumerator}"/> struct.
    /// </summary>
    /// <param name="instantiator">Function that creates a new enumerator that is to be enumerated throuhg.</param>
    public IndexableEnumerable(Func<TEnumerator> instantiator)
    {
        this.instantiator = instantiator;
    }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    public Enumerator GetEnumerator() => new(this);
    IEnumerator<(int, T)> IEnumerable<(int, T)>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Enumerator containing index and item of the enumeration.
    /// </summary>
    public struct Enumerator : IEnumerator<(int, T)>
    {
        private readonly TEnumerator parent;
        private int index;
        /// <summary>
        /// Initializes a new instance of the <see cref="Enumerator"/> struct.
        /// </summary>
        /// <param name="parent">The parent enumerable to enumerate through.</param>
        public Enumerator(IndexableEnumerable<T, TEnumerator> parent)
        {
            this.parent = parent.instantiator();
            Current = default;
            index = 0;
        }

        /// <inheritdoc />
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
