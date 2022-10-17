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
/// List that is thread safe and can be modified during enumeration
/// </summary>
/// <typeparam name="T">Type to hold in the list</typeparam>
public class EnumerationmodifiableConcurrentList<T> : IList<T>, IReadOnlyCollection<T>
{
    T IList<T>.this[int index]
    {
        get
        {
            using (scopeSemaphore.EnterCountScope())
                return list[index];
        }
        set
        {
            using (scopeSemaphore.EnterExclusiveScope())
                list[index] = value;
        }
    }

    /// <inheritdoc/>
    public int Count => list.Count;
    /// <inheritdoc/>
    public bool IsReadOnly => ((ICollection<T>)list).IsReadOnly;


    private readonly List<T> list;
    private readonly EnumeratorPool pool;
    private readonly CountedScopeSemaphore scopeSemaphore = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumerationmodifiableConcurrentList{T}"/> class that is empty and has the default initial capacity.
    /// </summary>
    public EnumerationmodifiableConcurrentList()
    {
        list = new List<T>();
        pool = new(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumerationmodifiableConcurrentList{T}"/> class that contains elements copied from the specified collection and has sufficient capacity to accommodate the number of elements copied.
    /// </summary>
    /// <param name="collection">The collection whose elements are copied to the new list.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="collection" /> is <see langword="null" />.</exception>
    public EnumerationmodifiableConcurrentList(IEnumerable<T> collection)
    {
        list = new List<T>(collection);
        pool = new(this);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnumerationmodifiableConcurrentList{T}"/> class that is empty and has the specified initial capacity.
    /// </summary>
    /// <param name="capacity">The number of elements that the new list can initially store.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="capacity" /> is less than 0.</exception>
    public EnumerationmodifiableConcurrentList(int capacity)
    {
        list = new List<T>(capacity);
        pool = new(this);
    }

    /// <inheritdoc/>
    public void Add(T item)
    {
        var index = list.Count;
        using var _ = scopeSemaphore.EnterExclusiveScope();
        list.Add(item);
        pool.InsertAt(index);
    }

    /// <inheritdoc/>
    public void Clear()
    {
        using var _ = scopeSemaphore.EnterExclusiveScope();
        list.Clear();
        pool.Clear();
    }
    /// <inheritdoc/>
    public bool Contains(T item)
    {
        using var _ = scopeSemaphore.EnterCountScope();
        return list.Contains(item);
    }
    /// <inheritdoc/>
    public void CopyTo(T[] array, int arrayIndex)
    {
        using var _ = scopeSemaphore.EnterExclusiveScope();
        list.CopyTo(array, arrayIndex);
    }
    /// <inheritdoc/>
    public int IndexOf(T item)
    {
        using var _ = scopeSemaphore.EnterCountScope();
        return list.IndexOf(item);
    }
    /// <inheritdoc/>
    public void Insert(int index, T item)
    {
        using var _ = scopeSemaphore.EnterExclusiveScope();
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
        using var _ = scopeSemaphore.EnterExclusiveScope();
        list.RemoveAt(index);
        pool.RemoveAt(index);
    }

    /// <inheritdoc cref="IEnumerable{T}.GetEnumerator"/>
    public Enumerator GetEnumerator() => pool.Rent();

    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>
    /// Enumerates the elements of a <see cref="EnumerationmodifiableConcurrentList{T}"/>.
    /// </summary>
    public sealed class Enumerator : IEnumerator<T>, IPoolElement
    {
        /// <inheritdoc/>
        public T Current { get; private set; }
        object IEnumerator.Current => Current!;

        private int currentIndex = -1;
        private readonly EnumerationmodifiableConcurrentList<T> parent;
        private readonly EnumeratorPool pool;
        private readonly CountedScopeSemaphore scopeSemaphore = new();

        internal Enumerator(EnumerationmodifiableConcurrentList<T> list)
        {
            parent = list;
            pool = list.pool;
            Current = default!;
        }

        internal void RemoveAt(int index)
        {
            using (scopeSemaphore.EnterCountScope())
                if (index > currentIndex)
                    return;

            using (scopeSemaphore.EnterExclusiveScope())
            {
                if (index == currentIndex)
                    Current = default!;
                currentIndex--;
            }
        }

        internal void InsertAt(int index)
        {
            using (scopeSemaphore.EnterCountScope())
                if (currentIndex < index)
                    return;


            using (scopeSemaphore.EnterExclusiveScope())
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
            using var _ = scopeSemaphore.EnterExclusiveScope();

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
            Current = default!;
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

        private readonly EnumerationmodifiableConcurrentList<T> list;
        private readonly CountedScopeSemaphore scopeSemaphore = new();

        public EnumeratorPool(EnumerationmodifiableConcurrentList<T> list)
        {
            this.list = list;
        }


        internal void RemoveAt(int index)
        {
            using var _ = scopeSemaphore.EnterCountScope();
            foreach (var item in gottem)
                item.RemoveAt(index);
        }

        internal void InsertAt(int index)
        {
            using var _ = scopeSemaphore.EnterCountScope();
            foreach (var item in gottem)
                item.InsertAt(index);
        }
        internal void Clear()
        {
            using var _ = scopeSemaphore.EnterCountScope();
            foreach (var item in gottem)
                item.Reset();
        }


        public Enumerator Rent()
        {
            if (pool.Count > 0)
            {
                Enumerator item;
                using (var __ = scopeSemaphore.EnterExclusiveScope())
                {
                    item = pool.Pop();
                    item.Reset();
                    gottem.Add(item);
                }
                return item;
            }

            var e = new Enumerator(list);
            e.Init(this);
            using var _ = scopeSemaphore.EnterExclusiveScope();
            gottem.Add(e);
            return e;
        }

        public void Return(Enumerator obj)
        {
            using var _ = scopeSemaphore.EnterExclusiveScope();
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

