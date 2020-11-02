using System;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude; 
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public partial class EffPureT
    {
        [Pure]
        public static Eff<Arr<A>> Sequence<A>(this Arr<Eff<A>> ma) =>
            Traverse<A, A>(ma, identity);
 
        [Pure]
        public static Eff<Arr<B>> Sequence<A, B>(this Arr<A> ta, Func<A, Eff<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<Either<L, A>> Sequence<L, A>(this Either<L, Eff<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static Eff<Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, Eff<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<EitherUnsafe<L, A>> Sequence<L, A>(this EitherUnsafe<L, Eff<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static Eff<EitherUnsafe<L, B>> Sequence<L, A, B>(this EitherUnsafe<L, A> ta, Func<A, Eff<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<Identity<A>> Sequence<A>(this Identity<Eff<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static Eff<Identity<B>> Sequence<A, B>(this Identity<A> ta, Func<A, Eff<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);

        [Pure]
        public static Eff<IEnumerable<A>> Sequence<A>(this IEnumerable<Eff<A>> mma) =>
            mma.Traverse(identity);

        [Pure]
        public static Eff<IEnumerable<B>> Sequence<A, B>(this IEnumerable<A> ta, Func<A, Eff<B>> f) =>
            ta.Map(f).Sequence();
       
        [Pure]
        public static Eff<Lst<A>> Sequence<A>(this Lst<Eff<A>> mma) =>
            mma.Traverse<A, A>(identity);
        
        [Pure]
        public static Eff<Lst<B>> Sequence<A, B>(this Lst<A> ta, Func<A, Eff<B>> f) =>
            ta.Map(f).Sequence();
 
        [Pure]
        public static Eff<Fin<A>> Sequence<A>(this Fin<Eff<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static Eff<Fin<B>> Sequence<A, B>(this Fin<A> ta, Func<A, Eff<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<Option<A>> Sequence<A>(this Option<Eff<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static Eff<Option<B>> Sequence<A, B>(this Option<A> ta, Func<A, Eff<B>> f) =>
            ta.Map(f).Sequence();
 
        [Pure]
        public static Eff<OptionUnsafe<A>> Sequence<A>(this OptionUnsafe<Eff<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static Eff<OptionUnsafe<B>> Sequence<A, B>(this OptionUnsafe<A> ta, Func<A, Eff<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<Seq<A>> Sequence<A>(this Seq<Eff<A>> ma) =>
            Traverse<A, A>(ma, identity);
        
        [Pure]
        public static Eff<Seq<B>> Sequence<A, B>(this Seq<A> ta, Func<A, Eff<B>> f) =>
            ta.Map(f).Sequence();

        [Pure]
        public static Eff<Set<A>> Sequence<A>(this Set<Eff<A>> ma) =>
            ma.Traverse(identity);
        
        [Pure]
        public static Eff<Set<B>> Sequence<A, B>(this Set<A> ta, Func<A, Eff<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<HashSet<A>> Sequence<A>(this HashSet<Eff<A>> ma) =>
            ma.Traverse(identity);
 
        [Pure]
        public static Eff<HashSet<B>> Sequence<A, B>(this HashSet<A> ta, Func<A, Eff<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<Que<A>> Sequence<A>(this Que<Eff<A>> ma) =>
            ma.Traverse(identity);
 
        [Pure]
        public static Eff<Que<B>> Sequence<A, B>(this Que<A> ta, Func<A, Eff<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<Stck<A>> Sequence<A>(this Stck<Eff<A>> ma) =>
            Traverse<A, A>(ma, identity);
 
        [Pure]
        public static Eff<Stck<B>> Sequence<A, B>(this Stck<A> ta, Func<A, Eff<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<Try<A>> Sequence<A>(this Try<Eff<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static Eff<Try<B>> Sequence<A, B>(this Try<A> ta, Func<A, Eff<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<TryOption<A>> Sequence<A>(this TryOption<Eff<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static Eff<TryOption<B>> Sequence<A, B>(this TryOption<A> ta, Func<A, Eff<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<Validation<FAIL, A>> Sequence<FAIL, A>(this Validation<FAIL, Eff<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static Eff<Validation<FAIL, B>> Sequence<FAIL, A, B>(this Validation<FAIL, A> ta, Func<A, Eff<B>> f) => 
            ta.Map(f).Sequence();
        
        [Pure]
        public static Eff<Validation<MonoidFail, FAIL, A>> Sequence<MonoidFail, FAIL, A>(this Validation<MonoidFail, FAIL, Eff<A>> mma)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            mma.Traverse(identity);
        
        [Pure]
        public static Eff<Validation<MonoidFail, FAIL, B>> Sequence<MonoidFail, FAIL, A, B>(this Validation<MonoidFail, FAIL, A> ta, Func<A, Eff<B>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            ta.Map(f).Sequence();
    }
}
