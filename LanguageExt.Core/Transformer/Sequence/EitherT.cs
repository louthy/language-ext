using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class EitherT
    {
        public static Either<L, Arr<B>> Sequence<L, A, B>(this Arr<A> ta, Func<A, Either<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static Either<L, Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, Either<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static Either<L, EitherUnsafe<L, B>> Sequence<L, A, B>(this EitherUnsafe<L, A> ta, Func<A, Either<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static Either<L, Identity<B>> Sequence<L, A, B>(this Identity<A> ta, Func<A, Either<L, B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);

        public static Either<L, IEnumerable<B>> Sequence<L, A, B>(this IEnumerable<A> ta, Func<A, Either<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static Either<L, Lst<B>> Sequence<L, A, B>(this Lst<A> ta, Func<A, Either<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static Either<L, Option<B>> Sequence<L, A, B>(this Option<A> ta, Func<A, Either<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static Either<L, OptionUnsafe<B>> Sequence<L, A, B>(this OptionUnsafe<A> ta, Func<A, Either<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static Either<L, Seq<B>> Sequence<L, A, B>(this Seq<A> ta, Func<A, Either<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static Either<L, Set<B>> Sequence<L, A, B>(this Set<A> ta, Func<A, Either<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static Either<L, HashSet<B>> Sequence<L, A, B>(this HashSet<A> ta, Func<A, Either<L, B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Either<L, Try<B>> Sequence<L, A, B>(this Try<A> ta, Func<A, Either<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static Either<L, TryOption<B>> Sequence<L, A, B>(this TryOption<A> ta, Func<A, Either<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static Either<L, Validation<FAIL, B>> Sequence<FAIL, L, A, B>(this Validation<FAIL, A> ta, Func<A, Either<L, B>> f) => 
            ta.Map(f).Sequence();
        
        public static Either<FAIL, Validation<MonoidFail, FAIL, B>> Sequence<MonoidFail, FAIL, A, B>(this Validation<MonoidFail, FAIL, A> ta, Func<A, Either<FAIL, B>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            ta.Map(f).Traverse(Prelude.identity);
    }
}
