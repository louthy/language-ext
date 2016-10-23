using System;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt.Instances
{
    public struct TLst<A> : Monoid<Lst<A>>
    {
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
