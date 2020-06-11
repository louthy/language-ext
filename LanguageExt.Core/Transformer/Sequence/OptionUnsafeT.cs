using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class OptionUnsafeT
    {
        public static OptionUnsafe<Arr<B>> Sequence<A, B>(this Arr<A> ta, Func<A, OptionUnsafe<B>> f) =>
            ta.Map(f).Sequence();
        
        public static OptionUnsafe<Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, OptionUnsafe<B>> f) =>
            ta.Map(f).Sequence();
        
        public static OptionUnsafe<EitherUnsafe<L, B>> Sequence<L, A, B>(this EitherUnsafe<L, A> ta, Func<A, OptionUnsafe<B>> f) =>
            ta.Map(f).Sequence();
        
        public static OptionUnsafe<Identity<B>> Sequence<A, B>(this Identity<A> ta, Func<A, OptionUnsafe<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);

        public static OptionUnsafe<IEnumerable<B>> Sequence<A, B>(this IEnumerable<A> ta, Func<A, OptionUnsafe<B>> f) =>
            ta.Map(f).Sequence();
        
        public static OptionUnsafe<Lst<B>> Sequence<A, B>(this Lst<A> ta, Func<A, OptionUnsafe<B>> f) =>
            ta.Map(f).Sequence();
        
        public static OptionUnsafe<Option<B>> Sequence<A, B>(this Option<A> ta, Func<A, OptionUnsafe<B>> f) =>
            ta.Map(f).Sequence();
        
        public static OptionUnsafe<OptionUnsafe<B>> Sequence<A, B>(this OptionUnsafe<A> ta, Func<A, OptionUnsafe<B>> f) =>
            ta.Map(f).Sequence();
        
        public static OptionUnsafe<Seq<B>> Sequence<A, B>(this Seq<A> ta, Func<A, OptionUnsafe<B>> f) =>
            ta.Map(f).Sequence();
        
        public static OptionUnsafe<Set<B>> Sequence<A, B>(this Set<A> ta, Func<A, OptionUnsafe<B>> f) =>
            ta.Map(f).Sequence();
        
        public static OptionUnsafe<HashSet<B>> Sequence<A, B>(this HashSet<A> ta, Func<A, OptionUnsafe<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static OptionUnsafe<Try<B>> Sequence<A, B>(this Try<A> ta, Func<A, OptionUnsafe<B>> f) =>
            ta.Map(f).Sequence();
        
        public static OptionUnsafe<TryOption<B>> Sequence<A, B>(this TryOption<A> ta, Func<A, OptionUnsafe<B>> f) =>
            ta.Map(f).Sequence();
        
        public static OptionUnsafe<Validation<FAIL, B>> Sequence<FAIL, A, B>(this Validation<FAIL, A> ta, Func<A, OptionUnsafe<B>> f) => 
            ta.Map(f).Sequence();
        
        public static OptionUnsafe<Validation<MonoidFail, FAIL, B>> Sequence<MonoidFail, FAIL, A, B>(this Validation<MonoidFail, FAIL, A> ta, Func<A, OptionUnsafe<B>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            ta.Map(f).Traverse(Prelude.identity);
    }
}
