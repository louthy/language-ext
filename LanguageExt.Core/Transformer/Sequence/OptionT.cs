using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class OptionT
    {
        public static Option<Arr<B>> Sequence<A, B>(this Arr<A> ta, Func<A, Option<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Option<Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, Option<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Option<EitherUnsafe<L, B>> Sequence<L, A, B>(this EitherUnsafe<L, A> ta, Func<A, Option<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Option<Identity<B>> Sequence<A, B>(this Identity<A> ta, Func<A, Option<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);

        public static Option<IEnumerable<B>> Sequence<A, B>(this IEnumerable<A> ta, Func<A, Option<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Option<Lst<B>> Sequence<A, B>(this Lst<A> ta, Func<A, Option<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Option<Option<B>> Sequence<A, B>(this Option<A> ta, Func<A, Option<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Option<OptionUnsafe<B>> Sequence<A, B>(this OptionUnsafe<A> ta, Func<A, Option<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Option<Seq<B>> Sequence<A, B>(this Seq<A> ta, Func<A, Option<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Option<Set<B>> Sequence<A, B>(this Set<A> ta, Func<A, Option<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Option<HashSet<B>> Sequence<A, B>(this HashSet<A> ta, Func<A, Option<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Option<Try<B>> Sequence<A, B>(this Try<A> ta, Func<A, Option<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Option<TryOption<B>> Sequence<A, B>(this TryOption<A> ta, Func<A, Option<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Option<Validation<FAIL, B>> Sequence<FAIL, A, B>(this Validation<FAIL, A> ta, Func<A, Option<B>> f) => 
            ta.Map(f).Sequence();
        
        public static Option<Validation<MonoidFail, FAIL, B>> Sequence<MonoidFail, FAIL, A, B>(this Validation<MonoidFail, FAIL, A> ta, Func<A, Option<B>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            ta.Map(f).Traverse(Prelude.identity);
    }
}
