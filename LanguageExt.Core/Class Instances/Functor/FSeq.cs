using System;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt.ClassInstances
{
    public struct FSeq<A, B> :
        Functor<Seq<A>, Seq<B>, A, B>
    {
        public static readonly FSeq<A, B> Inst = default(FSeq<A, B>);

        [Pure]
        public Seq<B> Map(Seq<A> ma, Func<A, B> f) =>
            ma.Map(f);
    }
}
