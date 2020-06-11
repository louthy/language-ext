using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class ValidationT
    {
        public static Validation<Fail, Arr<B>> Sequence<Fail, A, B>(this Arr<A> ta, Func<A, Validation<Fail, B>> f) =>
            ta.Map(f).Sequence();
        
        public static Validation<Fail, Either<Fail, B>> Sequence<Fail, A, B>(this Either<Fail, A> ta, Func<A, Validation<Fail, B>> f) =>
            ta.Map(f).Sequence();
        
        public static Validation<Fail, EitherUnsafe<Fail, B>> Sequence<Fail, A, B>(this EitherUnsafe<Fail, A> ta, Func<A, Validation<Fail, B>> f) =>
            ta.Map(f).Sequence();
        
        public static Validation<Fail, Identity<B>> Sequence<Fail, A, B>(this Identity<A> ta, Func<A, Validation<Fail, B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);

        public static Validation<Fail, IEnumerable<B>> Sequence<Fail, A, B>(this IEnumerable<A> ta, Func<A, Validation<Fail, B>> f) =>
            ta.Map(f).Sequence();
        
        public static Validation<Fail, Lst<B>> Sequence<Fail, A, B>(this Lst<A> ta, Func<A, Validation<Fail, B>> f) =>
            ta.Map(f).Sequence();
        
        public static Validation<Fail, Option<B>> Sequence<Fail, A, B>(this Option<A> ta, Func<A, Validation<Fail, B>> f) =>
            ta.Map(f).Sequence();
        
        public static Validation<Fail, OptionUnsafe<B>> Sequence<Fail, A, B>(this OptionUnsafe<A> ta, Func<A, Validation<Fail, B>> f) =>
            ta.Map(f).Sequence();
        
        public static Validation<Fail, Seq<B>> Sequence<Fail, A, B>(this Seq<A> ta, Func<A, Validation<Fail, B>> f) =>
            ta.Map(f).Sequence();
        
        public static Validation<Fail, Set<B>> Sequence<Fail, A, B>(this Set<A> ta, Func<A, Validation<Fail, B>> f) =>
            ta.Map(f).Sequence();
        
        public static Validation<Fail, HashSet<B>> Sequence<Fail, A, B>(this HashSet<A> ta, Func<A, Validation<Fail, B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Validation<Fail, Try<B>> Sequence<Fail, A, B>(this Try<A> ta, Func<A, Validation<Fail, B>> f) =>
            ta.Map(f).Sequence();
        
        public static Validation<Fail, TryOption<B>> Sequence<Fail, A, B>(this TryOption<A> ta, Func<A, Validation<Fail, B>> f) =>
            ta.Map(f).Sequence();
        
        public static Validation<Fail, Validation<Fail, B>> Sequence<Fail, A, B>(this Validation<Fail, A> ta, Func<A, Validation<Fail, B>> f) => 
            ta.Map(f).Sequence();
        
        public static Validation<Fail, Validation<MonoidFail, Fail, B>> Sequence<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, A> ta, Func<A, Validation<Fail, B>> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ta.Map(f).Traverse(Prelude.identity);
    }
}
