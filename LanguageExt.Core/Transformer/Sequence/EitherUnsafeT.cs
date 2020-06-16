using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class EitherUnsafeT
    {
        public static EitherUnsafe<L, Arr<B>> Sequence<L, A, B>(this Arr<A> ta, Func<A, EitherUnsafe<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static EitherUnsafe<L, Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, EitherUnsafe<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static EitherUnsafe<L, EitherUnsafe<L, B>> Sequence<L, A, B>(this EitherUnsafe<L, A> ta, Func<A, EitherUnsafe<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static EitherUnsafe<L, Identity<B>> Sequence<L, A, B>(this Identity<A> ta, Func<A, EitherUnsafe<L, B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);

        public static EitherUnsafe<L, IEnumerable<B>> Sequence<L, A, B>(this IEnumerable<A> ta, Func<A, EitherUnsafe<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static EitherUnsafe<L, Lst<B>> Sequence<L, A, B>(this Lst<A> ta, Func<A, EitherUnsafe<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static EitherUnsafe<L, Option<B>> Sequence<L, A, B>(this Option<A> ta, Func<A, EitherUnsafe<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static EitherUnsafe<L, OptionUnsafe<B>> Sequence<L, A, B>(this OptionUnsafe<A> ta, Func<A, EitherUnsafe<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static EitherUnsafe<L, Seq<B>> Sequence<L, A, B>(this Seq<A> ta, Func<A, EitherUnsafe<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static EitherUnsafe<L, Set<B>> Sequence<L, A, B>(this Set<A> ta, Func<A, EitherUnsafe<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static EitherUnsafe<L, HashSet<B>> Sequence<L, A, B>(this HashSet<A> ta, Func<A, EitherUnsafe<L, B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static EitherUnsafe<L, Try<B>> Sequence<L, A, B>(this Try<A> ta, Func<A, EitherUnsafe<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static EitherUnsafe<L, TryOption<B>> Sequence<L, A, B>(this TryOption<A> ta, Func<A, EitherUnsafe<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static EitherUnsafe<L, Validation<FAIL, B>> Sequence<FAIL, L, A, B>(this Validation<FAIL, A> ta, Func<A, EitherUnsafe<L, B>> f) => 
            ta.Map(f).Sequence();
    }
}
