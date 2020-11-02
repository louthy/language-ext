using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class FinT
    {
        public static Fin<Arr<B>> Sequence<A, B>(this Arr<A> ta, Func<A, Fin<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Fin<Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, Fin<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Fin<EitherUnsafe<L, B>> Sequence<L, A, B>(this EitherUnsafe<L, A> ta, Func<A, Fin<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Fin<Identity<B>> Sequence<A, B>(this Identity<A> ta, Func<A, Fin<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);

        public static Fin<IEnumerable<B>> Sequence<A, B>(this IEnumerable<A> ta, Func<A, Fin<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Fin<Lst<B>> Sequence<A, B>(this Lst<A> ta, Func<A, Fin<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Fin<Fin<B>> Sequence<A, B>(this Fin<A> ta, Func<A, Fin<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Fin<Option<B>> Sequence<A, B>(this Option<A> ta, Func<A, Fin<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Fin<OptionUnsafe<B>> Sequence<A, B>(this OptionUnsafe<A> ta, Func<A, Fin<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Fin<Seq<B>> Sequence<A, B>(this Seq<A> ta, Func<A, Fin<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Fin<Set<B>> Sequence<A, B>(this Set<A> ta, Func<A, Fin<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Fin<HashSet<B>> Sequence<A, B>(this HashSet<A> ta, Func<A, Fin<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Fin<Try<B>> Sequence<A, B>(this Try<A> ta, Func<A, Fin<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Fin<TryOption<B>> Sequence<A, B>(this TryOption<A> ta, Func<A, Fin<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Fin<Validation<FAIL, B>> Sequence<FAIL, A, B>(this Validation<FAIL, A> ta, Func<A, Fin<B>> f) => 
            ta.Map(f).Sequence();
        
        public static Fin<Validation<MonoidFail, FAIL, B>> Sequence<MonoidFail, FAIL, A, B>(this Validation<MonoidFail, FAIL, A> ta, Func<A, Fin<B>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            ta.Map(f).Traverse(Prelude.identity);

        public static Fin<Eff<B>> Sequence<A, B>(this Eff<A> ta, Func<A, Fin<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
    }
}
