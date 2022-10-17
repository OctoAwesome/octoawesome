using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Collections;
public struct CombineEnumerator<T, TEnumerator1, TEnumerator2> : IEnumerator<T>
    where TEnumerator1 : IEnumerator<T>
    where TEnumerator2 : IEnumerator<T>
{
    private readonly TEnumerator1 enum1;
    private readonly TEnumerator2 enum2;

    public T Current { get; private set; }
    object IEnumerator.Current => Current;

    public CombineEnumerator(TEnumerator1 enum1, TEnumerator2 enum2)
    {
        this.enum1 = enum1;
        this.enum2 = enum2;
        Current = default;
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
