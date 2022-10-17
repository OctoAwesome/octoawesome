using OctoAwesome.Pooling;
using OctoAwesome.Threading;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Collections;

/// <summary>
/// List that can be modified during enumeration, but is not threadsafe
/// </summary>
/// <typeparam name="T">Type to hold in the list</typeparam>
public class EnumerationmodifiableList<T> : IList<T>, IReadOnlyCollection<T>
{
    T IList<T>.this[int index]
    {
        get
        {
            return list[index];
        }
        set
        {
            list[index] = value;
        }
    }

    /// <inheritdoc/>
    public int Count => list.Count;
    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<T>)list).IsReadOnly;


    private readonly List<T> list;
    private readonly EnumeratorPool pool;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumerationmodifiableList{T}"/> class that is empty and has the default initial capacity.
    /// </summary>
    public EnumerationmodifiableList()
    {
        list = new List<T>();
        pool = new(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumerationmodifiableList{T}"/> class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new list.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="collection" /> is <see langword="null" />.</exception>
    public EnumerationmodifiableList(IEnumerable<T> collection)
    {
        list = new List<T>(collection);
        pool = new(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumerationmodifiableList{T}"/> class that is empty and has the specified initial capacity.
    /// </summary>
    /// <param name="capacity">The number of elements that the new list can initially store.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="capacity" /> is less than 0.</exception>
    public EnumerationmodifiableList(int capacity)
    {
        list = new List<T>(capacity);
        pool = new(this);
    }

    /// <inheritdoc/>
    public void Add(T item)
    {
        var index = list.Count;
        list.Add(item);
        pool.InsertAt(index);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        list.Clear();
        pool.Clear();
    }
    /// <inheritdoc/>
    public bool Contains(T item)
    {
        return list.Contains(item);
    }
    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        list.CopyTo(array, arrayIndex);
    }
    /// <inheritdoc/>
    public int IndexOf(T item)
    {
        return list.IndexOf(item);
    }
    /// <inheritdoc/>
    public void Insert(int index, T item)
    {
        list.Insert(index, item);
        pool.InsertAt(index);
    }
    /// <inheritdoc/>
    public bool Remove(T item)
    {
        var index = IndexOf(item);
        if (index == -1)
            return false;

        RemoveAt(index);
        return true;
    }
    /// <inheritdoc/>
    public void RemoveAt(int index)
    {
        list.RemoveAt(index);
        pool.RemoveAt(index);
    }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    public Enumerator GetEnumerator() => pool.Rent();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Enumerates the elements of a <see cref="EnumerationmodifiableList{T}"/>.
    /// </summary>
    public sealed class Enumerator : IEnumerator<T>, IPoolElement
    {
        /// <inheritdoc/>
        public T Current { get; private set; }
        object IEnumerator.Current => Current;

        private int currentIndex = -1;
        private readonly EnumerationmodifiableList<T> parent;
        private readonly EnumeratorPool pool;

        internal Enumerator(EnumerationmodifiableList<T> list)
        {
            parent = list;
            pool = list.pool;
        }

        internal void RemoveAt(int index)
        {
            if (index > currentIndex)
                return;

            if (index == currentIndex)
                Current = default;
            currentIndex--;
        }

        internal void InsertAt(int index)
        {
            if (currentIndex < index)
                return;


            currentIndex++;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Release();
        }

        /// <inheritdoc/>
        public bool MoveNext()
        {
            currentIndex++;
            if (currentIndex >= parent.Count)
                return false;

            Current = parent.list[currentIndex];
            return true;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            currentIndex = -1;
            Current = default;
        }

        /// <inheritdoc/>
        public void Init(IPool pool)
        {
            Reset();
        }

        /// <inheritdoc/>
        public void Release()
        {
            pool.Return(this);
        }
    }

    private sealed class EnumeratorPool : IPool<Enumerator>
    {
        private readonly Stack<Enumerator> pool = new();
        private readonly HashSet<Enumerator> gottem = new();

        private readonly EnumerationmodifiableList<T> list;

        public EnumeratorPool(EnumerationmodifiableList<T> list)
        {
            this.list = list;
        }


        internal void RemoveAt(int index)
        {
            foreach (var item in gottem)
                item.RemoveAt(index);
        }

        internal void InsertAt(int index)
        {
            foreach (var item in gottem)
                item.InsertAt(index);
        }
        internal void Clear()
        {
            foreach (var item in gottem)
                item.Reset();
        }


        public Enumerator Rent()
        {
            if (pool.Count > 0)
            {
                Enumerator item;
                item = pool.Pop();
                item.Reset();
                gottem.Add(item);
                return item;
            }

            var e = new Enumerator(list);
            e.Init(this);
            gottem.Add(e);
            return e;
        }

        public void Return(Enumerator obj)
        {
            gottem.Remove(obj);
            pool.Push(obj);
        }

        public void Return(IPoolElement obj)
        {
            if (obj is Enumerator e)
                pool.Push(e);
            else
                throw new ArgumentException(nameof(obj));
        }
    }
}

