using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class StateT
    {
        public static State<S, Arr<B>> Sequence<S, A, B>(this Arr<A> ta, Func<A, State<S, B>> f) =>
            ta.Map(f).Sequence();

        public static State<S, IEnumerable<B>> Sequence<S, A, B>(this IEnumerable<A> ta, Func<A, State<S, B>> f) =>
            ta.Map(f).Sequence();
        
        public static State<S, Set<B>> Sequence<S, A, B>(this Set<A> ta, Func<A, State<S, B>> f) =>
            ta.Map(f).Sequence();
    }
}
