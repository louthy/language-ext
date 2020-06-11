using System;
using System.Collections.Generic;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class ValidationT
    {
        public static Validation<MonoidFail, Fail, Arr<B>> Sequence<MonoidFail, Fail, A, B>(this Arr<A> ta, Func<A, Validation<MonoidFail, Fail, B>> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Validation<MonoidFail, Fail, Either<Fail, B>> Sequence<MonoidFail, Fail, A, B>(this Either<Fail, A> ta, Func<A, Validation<MonoidFail, Fail, B>> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Validation<MonoidFail, Fail, EitherUnsafe<Fail, B>> Sequence<MonoidFail, Fail, A, B>(this EitherUnsafe<Fail, A> ta, Func<A, Validation<MonoidFail, Fail, B>> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Validation<MonoidFail, Fail, Identity<B>> Sequence<MonoidFail, Fail, A, B>(this Identity<A> ta, Func<A, Validation<MonoidFail, Fail, B>> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ta.Map(f).Traverse(Prelude.identity);

        public static Validation<MonoidFail, Fail, IEnumerable<B>> Sequence<MonoidFail, Fail, A, B>(this IEnumerable<A> ta, Func<A, Validation<MonoidFail, Fail, B>> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Validation<MonoidFail, Fail, Lst<B>> Sequence<MonoidFail, Fail, A, B>(this Lst<A> ta, Func<A, Validation<MonoidFail, Fail, B>> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Validation<MonoidFail, Fail, Option<B>> Sequence<MonoidFail, Fail, A, B>(this Option<A> ta, Func<A, Validation<MonoidFail, Fail, B>> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Validation<MonoidFail, Fail, OptionUnsafe<B>> Sequence<MonoidFail, Fail, A, B>(this OptionUnsafe<A> ta, Func<A, Validation<MonoidFail, Fail, B>> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Validation<MonoidFail, Fail, Seq<B>> Sequence<MonoidFail, Fail, A, B>(this Seq<A> ta, Func<A, Validation<MonoidFail, Fail, B>> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Validation<MonoidFail, Fail, Set<B>> Sequence<MonoidFail, Fail, A, B>(this Set<A> ta, Func<A, Validation<MonoidFail, Fail, B>> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Validation<MonoidFail, Fail, HashSet<B>> Sequence<MonoidFail, Fail, A, B>(this HashSet<A> ta, Func<A, Validation<MonoidFail, Fail, B>> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Validation<MonoidFail, Fail, Try<B>> Sequence<MonoidFail, Fail, A, B>(this Try<A> ta, Func<A, Validation<MonoidFail, Fail, B>> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Validation<MonoidFail, Fail, TryOption<B>> Sequence<MonoidFail, Fail, A, B>(this TryOption<A> ta, Func<A, Validation<MonoidFail, Fail, B>> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Validation<MonoidFail, Fail, Validation<Fail, B>> Sequence<MonoidFail, Fail, A, B>(this Validation<Fail, A> ta, Func<A, Validation<MonoidFail, Fail, B>> f) 
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Validation<MonoidFail, Fail, Validation<MonoidFail, Fail, B>> Sequence<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, A> ta, Func<A, Validation<MonoidFail, Fail, B>> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ta.Map(f).Traverse(Prelude.identity);
    }
}
