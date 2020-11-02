using System;
using LanguageExt;
using System.Linq;
using System.Collections.Generic;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class IEnumerableT
    {
        public static IEnumerable<Arr<B>> Sequence<A, B>(this Arr<A> ma, Func<A, IEnumerable<B>> f) =>
            ma.Map(f).Sequence();

        public static IEnumerable<Either<L, B>> Sequence<L, A, B>(this Either<L, A> ma, Func<A, IEnumerable<B>> f) =>
            ma.Map(f).Sequence();

        public static IEnumerable<EitherUnsafe<L, B>> Sequence<L, A, B>(this EitherUnsafe<L, A> ma, Func<A, IEnumerable<B>> f) =>
            ma.Map(f).Sequence();

        public static IEnumerable<HashSet<B>> Sequence<A, B>(this HashSet<A> ma, Func<A, IEnumerable<B>> f) =>
            ma.Map(f).Sequence();

        public static IEnumerable<IEnumerable<B>> Sequence<A, B>(this IEnumerable<A> ma, Func<A, IEnumerable<B>> f) =>
            ma.Map(f).Sequence();
        
        public static IEnumerable<Lst<B>> Sequence<A, B>(this Lst<A> ma, Func<A, IEnumerable<B>> f) =>
            ma.Map(f).Sequence();
        
        public static IEnumerable<Fin<B>> Sequence<A, B>(this Fin<A> ma, Func<A, IEnumerable<B>> f) =>
            ma.Map(f).Sequence();
        
        public static IEnumerable<Option<B>> Sequence<A, B>(this Option<A> ma, Func<A, IEnumerable<B>> f) =>
            ma.Map(f).Sequence();
        
        public static IEnumerable<OptionUnsafe<B>> Sequence<A, B>(this OptionUnsafe<A> ma, Func<A, IEnumerable<B>> f) =>
            ma.Map(f).Sequence();
        
        public static IEnumerable<Que<B>> Sequence<A, B>(this Que<A> ma, Func<A, IEnumerable<B>> f) =>
            ma.Map(f).Traverse(Prelude.identity);
        
        public static IEnumerable<Seq<B>> Sequence<A, B>(this Seq<A> ma, Func<A, IEnumerable<B>> f) =>
            ma.Map(f).Sequence();
        
        public static IEnumerable<Set<B>> Sequence<A, B>(this Set<A> ma, Func<A, IEnumerable<B>> f) =>
            ma.Map(f).Sequence();
        
        public static IEnumerable<Stck<B>> Sequence<A, B>(this Stck<A> ma, Func<A, IEnumerable<B>> f) =>
            ma.Map(f).Traverse(Prelude.identity);
        
        public static IEnumerable<Try<B>> Sequence<A, B>(this Try<A> ma, Func<A, IEnumerable<B>> f) =>
            ma.Map(f).Sequence();
        
        public static IEnumerable<TryOption<B>> Sequence<A, B>(this TryOption<A> ma, Func<A, IEnumerable<B>> f) =>
            ma.Map(f).Sequence();
        
        public static IEnumerable<Validation<Fail, B>> Sequence<Fail, A, B>(this Validation<Fail, A> ma, Func<A, IEnumerable<B>> f) =>
            ma.Map(f).Sequence();
        
        public static IEnumerable<Validation<MonoidFail, Fail, B>> Sequence<MonoidFail, Fail, A, B>(this Validation<MonoidFail, Fail, A> ma, Func<A, IEnumerable<B>> f)
            where MonoidFail : struct, Monoid<Fail>, Eq<Fail> =>
            ma.Map(f).Traverse(Prelude.identity);
         
        public static IEnumerable<Eff<B>> Sequence<A, B>(this Eff<A> ta, Func<A, IEnumerable<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
    }
}
