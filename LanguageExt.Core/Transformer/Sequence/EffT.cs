using System;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude; 
using System.Diagnostics.Contracts;
using LanguageExt.Effects.Traits;

namespace LanguageExt
{
    public partial class AffT
    {
        [Pure]
        public static Eff<RT, Arr<B>> Sequence<RT, A, B>(this Arr<A> ta, Func<A, Eff<RT, B>> f) => 
            ta.Map(f).Sequence();

        [Pure]
        public static Eff<RT, Arr<A>> Sequence<RT, A>(this Arr<Eff<RT, A>> ma) =>
            ma.Traverse(identity);
        
        [Pure]
        public static Eff<RT, Either<L, A>> Sequence<RT, L, A>(this Either<L, Eff<RT, A>> mma) => 
            mma.Traverse(identity);
        
        [Pure]
        public static Eff<RT, Either<L, B>> Sequence<RT, L, A, B>(this Either<L, A> ta, Func<A, Eff<RT, B>> f) => 
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<RT, EitherUnsafe<L, A>> Sequence<RT, L, A>(this EitherUnsafe<L, Eff<RT, A>> mma)  =>
            mma.Traverse(identity);
        
        [Pure]
        public static Eff<RT, EitherUnsafe<L, B>> Sequence<RT, L, A, B>(this EitherUnsafe<L, A> ta, Func<A, Eff<RT, B>> f) => 
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<RT, Identity<A>> Sequence<RT, A>(this Identity<Eff<RT, A>> mma)  =>
            mma.Traverse(identity);
        
        [Pure]
        public static Eff<RT, Identity<B>> Sequence<RT, A, B>(this Identity<A> ta, Func<A, Eff<RT, B>> f) => 
            ta.Map(f).Traverse(Prelude.identity);

        [Pure]
        public static Eff<RT, IEnumerable<B>> Sequence<RT, A, B>(this IEnumerable<A> ta, Func<A, Eff<RT, B>> f) => 
            ta.Map(f).Sequence();

        [Pure]
        public static Eff<RT, IEnumerable<A>> Sequence<RT, A>(this IEnumerable<Eff<RT, A>> ma) =>
            ma.Traverse(identity);
        
        [Pure]
        public static Eff<RT, Lst<B>> Sequence<RT, A, B>(this Lst<A> ta, Func<A, Eff<RT, B>> f)  =>
            ta.Map(f).Sequence();

        [Pure]
        public static Eff<RT, Lst<A>> Sequence<RT, A>(this Lst<Eff<RT, A>> ma) =>
            ma.Traverse(identity);
        
        [Pure]
        public static Eff<RT, Fin<A>> Sequence<RT, A>(this Fin<Eff<RT, A>> mma)  =>
            mma.Traverse(identity);
        
        [Pure]
        public static Eff<RT, Fin<B>> Sequence<RT, A, B>(this Fin<A> ta, Func<A, Eff<RT, B>> f)  =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<RT, Option<A>> Sequence<RT, A>(this Option<Eff<RT, A>> mma)  =>
            mma.Traverse(identity);
        
        [Pure]
        public static Eff<RT, Option<B>> Sequence<RT, A, B>(this Option<A> ta, Func<A, Eff<RT, B>> f)  =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<RT, OptionUnsafe<A>> Sequence<RT, A>(this OptionUnsafe<Eff<RT, A>> mma)  =>
            mma.Traverse(identity);
        
        [Pure]
        public static Eff<RT, OptionUnsafe<B>> Sequence<RT, A, B>(this OptionUnsafe<A> ta, Func<A, Eff<RT, B>> f)  =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<RT, Seq<B>> Sequence<RT, A, B>(this Seq<A> ta, Func<A, Eff<RT, B>> f)  =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<RT, Seq<A>> Sequence<RT, A>(this Seq<Eff<RT, A>> ma) =>
            ma.Traverse(identity);
        
        [Pure]
        public static Eff<RT, Set<B>> Sequence<RT, A, B>(this Set<A> ta, Func<A, Eff<RT, B>> f)  =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<RT, Set<A>> Sequence<RT, A>(this Set<Eff<RT, A>> ma) =>
            ma.Traverse(identity);
        
        [Pure]
        public static Eff<RT, HashSet<B>> Sequence<RT, A, B>(this HashSet<A> ta, Func<A, Eff<RT, B>> f)  =>
            ta.Map(f).Sequence();
 
        [Pure]
        public static Eff<RT, HashSet<A>> Sequence<RT, A>(this HashSet<Eff<RT, A>> ma) =>
            ma.Traverse(identity);
        
        [Pure]
        public static Eff<RT, Que<B>> Sequence<RT, A, B>(this Que<A> ta, Func<A, Eff<RT, B>> f)  =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<RT, Que<A>> Sequence<RT, A>(this Que<Eff<RT, A>> ma) =>
            ma.Traverse(identity);
        
        [Pure]
        public static Eff<RT, Stck<B>> Sequence<RT, A, B>(this Stck<A> ta, Func<A, Eff<RT, B>> f)  =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<RT, Stck<A>> Sequence<RT, A>(this Stck<Eff<RT, A>> ma) =>
            ma.Traverse(identity);
        
        [Pure]
        public static Eff<RT, Try<A>> Sequence<RT, A>(this Try<Eff<RT, A>> mma)  =>
            mma.Traverse(identity);
        
        [Pure]
        public static Eff<RT, Try<B>> Sequence<RT, A, B>(this Try<A> ta, Func<A, Eff<RT, B>> f)  =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<RT, TryOption<A>> Sequence<RT, A>(this TryOption<Eff<RT, A>> mma)  =>
            mma.Traverse(identity);
        
        [Pure]
        public static Eff<RT, TryOption<B>> Sequence<RT, A, B>(this TryOption<A> ta, Func<A, Eff<RT, B>> f)  =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<RT, Validation<FAIL, A>> Sequence<RT, FAIL, A>(this Validation<FAIL, Eff<RT, A>> mma)  =>
            mma.Traverse(identity);
        
        [Pure]
        public static Eff<RT, Validation<FAIL, B>> Sequence<RT, FAIL, A, B>(this Validation<FAIL, A> ta, Func<A, Eff<RT, B>> f)  =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<RT, Validation<MonoidFail, FAIL, A>> Sequence<RT, MonoidFail, FAIL, A>(this Validation<MonoidFail, FAIL, Eff<RT, A>> mma)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> => 
            mma.Traverse(identity);
        
        [Pure]
        public static Eff<RT, Validation<MonoidFail, FAIL, B>> Sequence<RT, MonoidFail, FAIL, A, B>(this Validation<MonoidFail, FAIL, A> ta, Func<A, Eff<RT, B>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> => 
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<RT, Eff<A>> Sequence<RT, A>(this Eff<Eff<RT, A>> mma)  =>
            mma.Traverse(identity);

        [Pure]
        public static Eff<RT, Eff<B>> Sequence<RT, A, B>(this Eff<A> ta, Func<A, Eff<RT, B>> f)  =>
            ta.Map(f).Sequence();
    }
}
