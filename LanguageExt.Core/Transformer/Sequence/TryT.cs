using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class TryT
    {
        public static Try<Arr<B>> Sequence<A, B>(this Arr<A> ta, Func<A, Try<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Try<Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, Try<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Try<EitherUnsafe<L, B>> Sequence<L, A, B>(this EitherUnsafe<L, A> ta, Func<A, Try<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Try<Identity<B>> Sequence<A, B>(this Identity<A> ta, Func<A, Try<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);

        public static Try<IEnumerable<B>> Sequence<A, B>(this IEnumerable<A> ta, Func<A, Try<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Try<Lst<B>> Sequence<A, B>(this Lst<A> ta, Func<A, Try<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Try<OptionUnsafe<B>> Sequence<A, B>(this OptionUnsafe<A> ta, Func<A, Try<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Try<Seq<B>> Sequence<A, B>(this Seq<A> ta, Func<A, Try<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Try<Set<B>> Sequence<A, B>(this Set<A> ta, Func<A, Try<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Try<HashSet<B>> Sequence<A, B>(this HashSet<A> ta, Func<A, Try<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Try<Try<B>> Sequence<A, B>(this Try<A> ta, Func<A, Try<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Try<TryOption<B>> Sequence<A, B>(this TryOption<A> ta, Func<A, Try<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Try<Validation<FAIL, B>> Sequence<FAIL, A, B>(this Validation<FAIL, A> ta, Func<A, Try<B>> f) => 
            ta.Map(f).Sequence();
        
        public static Try<Validation<MonoidFail, FAIL, B>> Sequence<MonoidFail, FAIL, A, B>(this Validation<MonoidFail, FAIL, A> ta, Func<A, Try<B>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            ta.Map(f).Traverse(Prelude.identity);
    }
}
