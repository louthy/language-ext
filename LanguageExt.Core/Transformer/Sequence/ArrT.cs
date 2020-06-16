using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class ArrT
    {
        public static Arr<Arr<B>> Sequence<A, B>(this Arr<A> ta, Func<A, Arr<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Arr<Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, Arr<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Arr<EitherUnsafe<L, B>> Sequence<L, A, B>(this EitherUnsafe<L, A> ta, Func<A, Arr<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Arr<Identity<B>> Sequence<A, B>(this Identity<A> ta, Func<A, Arr<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);

        public static Arr<IEnumerable<B>> Sequence<A, B>(this IEnumerable<A> ta, Func<A, Arr<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Arr<Lst<B>> Sequence<A, B>(this Lst<A> ta, Func<A, Arr<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Arr<Option<B>> Sequence<A, B>(this Option<A> ta, Func<A, Arr<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Arr<OptionUnsafe<B>> Sequence<A, B>(this OptionUnsafe<A> ta, Func<A, Arr<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Arr<Seq<B>> Sequence<A, B>(this Seq<A> ta, Func<A, Arr<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Arr<Set<B>> Sequence<A, B>(this Set<A> ta, Func<A, Arr<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Arr<HashSet<B>> Sequence<A, B>(this HashSet<A> ta, Func<A, Arr<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Arr<Try<B>> Sequence<A, B>(this Try<A> ta, Func<A, Arr<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Arr<TryOption<B>> Sequence<A, B>(this TryOption<A> ta, Func<A, Arr<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Arr<Validation<FAIL, B>> Sequence<FAIL, A, B>(this Validation<FAIL, A> ta, Func<A, Arr<B>> f) => 
            ta.Map(f).Sequence();
        
        public static Arr<Validation<MonoidFail, FAIL, B>> Sequence<MonoidFail, FAIL, A, B>(this Validation<MonoidFail, FAIL, A> ta, Func<A, Arr<B>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            ta.Map(f).Traverse(Prelude.identity);
    }
}
