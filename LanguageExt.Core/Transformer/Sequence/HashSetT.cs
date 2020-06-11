using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class HashSetT
    {
        public static HashSet<Arr<B>> Sequence<A, B>(this Arr<A> ta, Func<A, HashSet<B>> f) =>
            ta.Map(f).Sequence();
        
        public static HashSet<Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, HashSet<B>> f) =>
            ta.Map(f).Sequence();
        
        public static HashSet<EitherUnsafe<L, B>> Sequence<L, A, B>(this EitherUnsafe<L, A> ta, Func<A, HashSet<B>> f) =>
            ta.Map(f).Sequence();
        
        public static HashSet<Identity<B>> Sequence<A, B>(this Identity<A> ta, Func<A, HashSet<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);

        public static HashSet<IEnumerable<B>> Sequence<A, B>(this IEnumerable<A> ta, Func<A, HashSet<B>> f) =>
            ta.Map(f).Sequence();
        
        public static HashSet<Lst<B>> Sequence<A, B>(this Lst<A> ta, Func<A, HashSet<B>> f) =>
            ta.Map(f).Sequence();
        
        public static HashSet<Option<B>> Sequence<A, B>(this Option<A> ta, Func<A, HashSet<B>> f) =>
            ta.Map(f).Sequence();
        
        public static HashSet<OptionUnsafe<B>> Sequence<A, B>(this OptionUnsafe<A> ta, Func<A, HashSet<B>> f) =>
            ta.Map(f).Sequence();
        
        public static HashSet<Seq<B>> Sequence<A, B>(this Seq<A> ta, Func<A, HashSet<B>> f) =>
            ta.Map(f).Sequence();
        
        public static HashSet<Set<B>> Sequence<A, B>(this Set<A> ta, Func<A, HashSet<B>> f) =>
            ta.Map(f).Sequence();
        
        public static HashSet<HashSet<B>> Sequence<A, B>(this HashSet<A> ta, Func<A, HashSet<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static HashSet<Try<B>> Sequence<A, B>(this Try<A> ta, Func<A, HashSet<B>> f) =>
            ta.Map(f).Sequence();
        
        public static HashSet<TryOption<B>> Sequence<A, B>(this TryOption<A> ta, Func<A, HashSet<B>> f) =>
            ta.Map(f).Sequence();
        
        public static HashSet<Validation<FAIL, B>> Sequence<FAIL, A, B>(this Validation<FAIL, A> ta, Func<A, HashSet<B>> f) => 
            ta.Map(f).Sequence();
        
        public static HashSet<Validation<MonoidFail, FAIL, B>> Sequence<MonoidFail, FAIL, A, B>(this Validation<MonoidFail, FAIL, A> ta, Func<A, HashSet<B>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            ta.Map(f).Traverse(Prelude.identity);
    }
}
