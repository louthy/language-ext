using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class TryOptionT
    {
        public static TryOption<Arr<B>> Sequence<A, B>(this Arr<A> ta, Func<A, TryOption<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryOption<Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, TryOption<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryOption<EitherUnsafe<L, B>> Sequence<L, A, B>(this EitherUnsafe<L, A> ta, Func<A, TryOption<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryOption<Identity<B>> Sequence<A, B>(this Identity<A> ta, Func<A, TryOption<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);

        public static TryOption<IEnumerable<B>> Sequence<A, B>(this IEnumerable<A> ta, Func<A, TryOption<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryOption<Lst<B>> Sequence<A, B>(this Lst<A> ta, Func<A, TryOption<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryOption<OptionUnsafe<B>> Sequence<A, B>(this OptionUnsafe<A> ta, Func<A, TryOption<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryOption<Seq<B>> Sequence<A, B>(this Seq<A> ta, Func<A, TryOption<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryOption<Set<B>> Sequence<A, B>(this Set<A> ta, Func<A, TryOption<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryOption<HashSet<B>> Sequence<A, B>(this HashSet<A> ta, Func<A, TryOption<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static TryOption<Try<B>> Sequence<A, B>(this Try<A> ta, Func<A, TryOption<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryOption<TryOption<B>> Sequence<A, B>(this TryOption<A> ta, Func<A, TryOption<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryOption<Validation<FAIL, B>> Sequence<FAIL, A, B>(this Validation<FAIL, A> ta, Func<A, TryOption<B>> f) => 
            ta.Map(f).Sequence();
        
        public static TryOption<Validation<MonoidFail, FAIL, B>> Sequence<MonoidFail, FAIL, A, B>(this Validation<MonoidFail, FAIL, A> ta, Func<A, TryOption<B>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            ta.Map(f).Traverse(Prelude.identity);
    }
}
