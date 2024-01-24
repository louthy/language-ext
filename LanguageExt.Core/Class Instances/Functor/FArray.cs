using System;
using System.Linq;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances;

public struct FArray<A, B> :
    Functor<A[], B[], A, B>
{
    [Pure]
    public static B[] Map(A[] ma, Func<A, B> f)
    {
        var bs = new B[ma.Length];
        using var iter = ma.AsEnumerable().GetEnumerator();
        for (int i = 0; iter.MoveNext(); i++)
        {
            bs[i] = f(iter.Current);
        }
        return bs;
    }
}
