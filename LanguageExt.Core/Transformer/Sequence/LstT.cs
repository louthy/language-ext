using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class LstT
    {
        public static Lst<Arr<B>> Sequence<A, B>(this Arr<A> ta, Func<A, Lst<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Lst<Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, Lst<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Lst<EitherUnsafe<L, B>> Sequence<L, A, B>(this EitherUnsafe<L, A> ta, Func<A, Lst<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Lst<Identity<B>> Sequence<A, B>(this Identity<A> ta, Func<A, Lst<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);

        public static Lst<IEnumerable<B>> Sequence<A, B>(this IEnumerable<A> ta, Func<A, Lst<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Lst<Lst<B>> Sequence<A, B>(this Lst<A> ta, Func<A, Lst<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Lst<Option<B>> Sequence<A, B>(this Option<A> ta, Func<A, Lst<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Lst<OptionUnsafe<B>> Sequence<A, B>(this OptionUnsafe<A> ta, Func<A, Lst<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Lst<Seq<B>> Sequence<A, B>(this Seq<A> ta, Func<A, Lst<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Lst<Set<B>> Sequence<A, B>(this Set<A> ta, Func<A, Lst<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Lst<Try<B>> Sequence<A, B>(this Try<A> ta, Func<A, Lst<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Lst<TryOption<B>> Sequence<A, B>(this TryOption<A> ta, Func<A, Lst<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Lst<Validation<FAIL, B>> Sequence<FAIL, A, B>(this Validation<FAIL, A> ta, Func<A, Lst<B>> f) => 
            ta.Map(f).Sequence();
        
        public static Lst<Validation<MonoidFail, FAIL, B>> Sequence<MonoidFail, FAIL, A, B>(this Validation<MonoidFail, FAIL, A> ta, Func<A, Lst<B>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            ta.Map(f).Traverse(Prelude.identity);
    }
}
