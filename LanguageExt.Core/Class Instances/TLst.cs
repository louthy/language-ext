using System;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct TLst<A> : Monoid<Lst<A>>
    {
        public static readonly TLst<A> Inst = default(TLst<A>);

        [Pure]
        public Lst<A> Append(Lst<A> x, Lst<A> y) =>
            x.Append(y);

        [Pure]
        public Lst<A> Subtract(Lst<A> x, Lst<A> y) =>
            x.Subtract(y);

        [Pure]
        public Lst<A> Empty() =>
            List.empty<A>();
    }
}
