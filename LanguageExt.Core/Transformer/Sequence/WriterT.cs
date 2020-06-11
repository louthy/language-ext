using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class WriterT
    {
        public static Writer<MonoidW, W, Arr<B>> Sequence<MonoidW, W, A, B>(this Arr<A> ta, Func<A, Writer<MonoidW, W, B>> f) 
            where MonoidW : struct, Monoid<W> =>
            ta.Map(f).Sequence();

        public static Writer<MonoidW, W, IEnumerable<B>> Sequence<MonoidW, W, A, B>(this IEnumerable<A> ta, Func<A, Writer<MonoidW, W, B>> f)
            where MonoidW : struct, Monoid<W> =>
            ta.Map(f).Sequence();
        
        public static Writer<MonoidW, W, Set<B>> Sequence<MonoidW, W, A, B>(this Set<A> ta, Func<A, Writer<MonoidW, W, B>> f)
            where MonoidW : struct, Monoid<W> =>
            ta.Map(f).Sequence();
    }
}
