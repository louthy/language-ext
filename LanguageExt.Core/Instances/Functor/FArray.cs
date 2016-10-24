using System;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Collections.Generic;

namespace LanguageExt.Instances
{
    public struct FArray<A, B> : 
        Functor<A[], B[], A, B>
    {
        public static readonly FArray<A, B> Inst = default(FArray<A, B>);

        public B[] Map(A[] ma, Func<A, B> f) =>
            default(FSeq<A, B>).Map(ma, f).ToArray();
    }
}
