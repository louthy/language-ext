using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt.TypeClass
{
    public struct TLst<A> : Semigroup<Lst<A>>, Difference<Lst<A>>
    {
        [Pure]
        public Lst<A> Append(Lst<A> x, Lst<A> y) =>
            x.Append(y);

        [Pure]
        public Lst<A> Difference(Lst<A> x, Lst<A> y) =>
            x.Difference(y);

        [Pure]
        public Functor<B> Map<B>(Functor<A> fa, Func<A, B> f) =>
            fa.Map((Lst<A>)fa, f);
    }
}
