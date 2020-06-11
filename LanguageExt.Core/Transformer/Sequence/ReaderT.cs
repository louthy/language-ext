using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class ReaderT
    {
        public static Reader<Env, Arr<B>> Sequence<Env, A, B>(this Arr<A> ta, Func<A, Reader<Env, B>> f) =>
            ta.Map(f).Sequence();

        public static Reader<Env, IEnumerable<B>> Sequence<Env, A, B>(this IEnumerable<A> ta, Func<A, Reader<Env, B>> f) =>
            ta.Map(f).Sequence();
        
        public static Reader<Env, Set<B>> Sequence<Env, A, B>(this Set<A> ta, Func<A, Reader<Env, B>> f) =>
            ta.Map(f).Sequence();
    }
}
