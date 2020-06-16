using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class StckT
    {
        public static Stck<Arr<B>> Sequence<A, B>(this Arr<A> ta, Func<A, Stck<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Stck<Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, Stck<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Stck<EitherUnsafe<L, B>> Sequence<L, A, B>(this EitherUnsafe<L, A> ta, Func<A, Stck<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Stck<Identity<B>> Sequence<A, B>(this Identity<A> ta, Func<A, Stck<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);

        public static Stck<IEnumerable<B>> Sequence<A, B>(this IEnumerable<A> ta, Func<A, Stck<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Stck<Option<B>> Sequence<A, B>(this Option<A> ta, Func<A, Stck<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Stck<OptionUnsafe<B>> Sequence<A, B>(this OptionUnsafe<A> ta, Func<A, Stck<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Stck<Seq<B>> Sequence<A, B>(this Seq<A> ta, Func<A, Stck<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Stck<Set<B>> Sequence<A, B>(this Set<A> ta, Func<A, Stck<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Stck<Try<B>> Sequence<A, B>(this Try<A> ta, Func<A, Stck<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Stck<TryOption<B>> Sequence<A, B>(this TryOption<A> ta, Func<A, Stck<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Stck<Validation<FAIL, B>> Sequence<FAIL, A, B>(this Validation<FAIL, A> ta, Func<A, Stck<B>> f) => 
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Stck<Validation<MonoidFail, FAIL, B>> Sequence<MonoidFail, FAIL, A, B>(this Validation<MonoidFail, FAIL, A> ta, Func<A, Stck<B>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            ta.Map(f).Traverse(Prelude.identity);
    }
}
