using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Collections;
/// <summary>
/// An enumerator to enumerate through two enumerators in succession, without unnecessary allocations.
/// </summary>
/// <typeparam name="T">The type of the items contained in the enumerators.</typeparam>
/// <typeparam name="TEnumerator1">The type of the first enumerator to enumerate through.</typeparam>
/// <typeparam name="TEnumerator2">The type of the second enumerator to enumerate through.</typeparam>
public struct CombineEnumerator<T, TEnumerator1, TEnumerator2> : IEnumerator<T>
    where TEnumerator1 : IEnumerator<T>
    where TEnumerator2 : IEnumerator<T>
{
    private readonly TEnumerator1 enum1;
    private readonly TEnumerator2 enum2;

    /// <inheritdoc />
    public T Current { get; private set; }
    object IEnumerator.Current => Current!;

    /// <summary>
    /// Initializes a new instance of the <see cref="CombineEnumerator{T,TEnumerator1,TEnumerator2}"/> struct.
    /// </summary>
    /// <param name="enum1">The enumerator to enumerate through first.</param>
    /// <param name="enum2">The enumerator to enumerate through second, after the first one has been enumerated.</param>
    public CombineEnumerator(TEnumerator1 enum1, TEnumerator2 enum2)
    {
        this.enum1 = enum1;
        this.enum2 = enum2;
        Current = default!;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        enum1.Dispose();
        enum2.Dispose();
    }

    /// <inheritdoc/>
    public bool MoveNext()
    {
        var res = enum1.MoveNext();
        if (res)
        {
            Current = enum1.Current;
            return res;
        }
        res = enum2.MoveNext();
        if (res)
            Current = enum2.Current;
        return res;
    }

    /// <inheritdoc/>
    public void Reset()
    {
        enum1.Reset();
        enum2.Reset();
    }
}
