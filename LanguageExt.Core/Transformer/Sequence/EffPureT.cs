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
        public static EffPure<Arr<A>> Sequence<A>(this Arr<EffPure<A>> ma) =>
            Traverse<A, A>(ma, identity);
 
        [Pure]
        public static EffPure<Arr<B>> Sequence<A, B>(this Arr<A> ta, Func<A, EffPure<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static EffPure<Either<L, A>> Sequence<L, A>(this Either<L, EffPure<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static EffPure<Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, EffPure<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static EffPure<EitherUnsafe<L, A>> Sequence<L, A>(this EitherUnsafe<L, EffPure<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static EffPure<EitherUnsafe<L, B>> Sequence<L, A, B>(this EitherUnsafe<L, A> ta, Func<A, EffPure<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static EffPure<Identity<A>> Sequence<A>(this Identity<EffPure<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static EffPure<Identity<B>> Sequence<A, B>(this Identity<A> ta, Func<A, EffPure<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);

        [Pure]
        public static EffPure<IEnumerable<A>> Sequence<A>(this IEnumerable<EffPure<A>> mma) =>
            mma.Traverse(identity);

        [Pure]
        public static EffPure<IEnumerable<B>> Sequence<A, B>(this IEnumerable<A> ta, Func<A, EffPure<B>> f) =>
            ta.Map(f).Sequence();
       
        [Pure]
        public static EffPure<Lst<A>> Sequence<A>(this Lst<EffPure<A>> mma) =>
            mma.Traverse<A, A>(identity);
        
        [Pure]
        public static EffPure<Lst<B>> Sequence<A, B>(this Lst<A> ta, Func<A, EffPure<B>> f) =>
            ta.Map(f).Sequence();
 
        [Pure]
        public static EffPure<OptionUnsafe<A>> Sequence<A>(this OptionUnsafe<EffPure<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static EffPure<OptionUnsafe<B>> Sequence<A, B>(this OptionUnsafe<A> ta, Func<A, EffPure<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static EffPure<Seq<A>> Sequence<A>(this Seq<EffPure<A>> ma) =>
            Traverse<A, A>(ma, identity);
        
        [Pure]
        public static EffPure<Seq<B>> Sequence<A, B>(this Seq<A> ta, Func<A, EffPure<B>> f) =>
            ta.Map(f).Sequence();

        [Pure]
        public static EffPure<Set<A>> Sequence<A>(this Set<EffPure<A>> ma) =>
            ma.Traverse(identity);
        
        [Pure]
        public static EffPure<Set<B>> Sequence<A, B>(this Set<A> ta, Func<A, EffPure<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static EffPure<HashSet<A>> Sequence<A>(this HashSet<EffPure<A>> ma) =>
            ma.Traverse(identity);
 
        [Pure]
        public static EffPure<HashSet<B>> Sequence<A, B>(this HashSet<A> ta, Func<A, EffPure<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static EffPure<Que<A>> Sequence<A>(this Que<EffPure<A>> ma) =>
            ma.Traverse(identity);
 
        [Pure]
        public static EffPure<Que<B>> Sequence<A, B>(this Que<A> ta, Func<A, EffPure<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static EffPure<Stck<A>> Sequence<A>(this Stck<EffPure<A>> ma) =>
            Traverse<A, A>(ma, identity);
 
        [Pure]
        public static EffPure<Stck<B>> Sequence<A, B>(this Stck<A> ta, Func<A, EffPure<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static EffPure<Try<A>> Sequence<A>(this Try<EffPure<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static EffPure<Try<B>> Sequence<A, B>(this Try<A> ta, Func<A, EffPure<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static EffPure<TryOption<A>> Sequence<A>(this TryOption<EffPure<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static EffPure<TryOption<B>> Sequence<A, B>(this TryOption<A> ta, Func<A, EffPure<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static EffPure<Validation<FAIL, A>> Sequence<FAIL, A>(this Validation<FAIL, EffPure<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static EffPure<Validation<FAIL, B>> Sequence<FAIL, A, B>(this Validation<FAIL, A> ta, Func<A, EffPure<B>> f) => 
            ta.Map(f).Sequence();
        
        [Pure]
        public static EffPure<Validation<MonoidFail, FAIL, A>> Sequence<MonoidFail, FAIL, A>(this Validation<MonoidFail, FAIL, EffPure<A>> mma)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            mma.Traverse(identity);
        
        [Pure]
        public static EffPure<Validation<MonoidFail, FAIL, B>> Sequence<MonoidFail, FAIL, A, B>(this Validation<MonoidFail, FAIL, A> ta, Func<A, EffPure<B>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            ta.Map(f).Sequence();
    }
}
