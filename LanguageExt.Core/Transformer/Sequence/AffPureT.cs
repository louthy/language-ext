using System;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude; 
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    public partial class AffPureT
    {
        [Pure]
        public static AffPure<Arr<A>> SequenceParallel<A>(this Arr<AffPure<A>> ma) =>
            TraverseParallel<A, A>(ma, identity, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static AffPure<Arr<A>> SequenceParallel<A>(this Arr<AffPure<A>> ma, int windowSize) =>
            TraverseParallel<A, A>(ma, identity, windowSize);
        [Pure]
        public static AffPure<Arr<B>> SequenceParallel<A, B>(this Arr<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).SequenceParallel();
        
        [Pure]
        public static AffPure<Arr<B>> SequenceParallel<A, B>(this Arr<A> ta, Func<A, AffPure<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);
        
        [Pure]
        public static AffPure<Arr<A>> SequenceSerial<A>(this Arr<AffPure<A>> ma) =>
            TraverseSerial<A, A>(ma, identity);
 
        [Pure]
        public static AffPure<Arr<B>> SequenceSerial<A, B>(this Arr<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).SequenceSerial();
        
        [Pure]
        public static AffPure<Either<L, A>> Sequence<L, A>(this Either<L, AffPure<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static AffPure<Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static AffPure<EitherUnsafe<L, A>> Sequence<L, A>(this EitherUnsafe<L, AffPure<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static AffPure<EitherUnsafe<L, B>> Sequence<L, A, B>(this EitherUnsafe<L, A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static AffPure<Identity<A>> Sequence<A>(this Identity<AffPure<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static AffPure<Identity<B>> Sequence<A, B>(this Identity<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);

        [Pure]
        public static AffPure<IEnumerable<B>> SequenceSerial<A, B>(this IEnumerable<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).SequenceSerial();

        [Pure]
        public static AffPure<IEnumerable<B>> SequenceParallel<A, B>(this IEnumerable<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).SequenceParallel();

        [Pure]
        public static AffPure<IEnumerable<B>> SequenceParallel<A, B>(this IEnumerable<A> ta, Func<A, AffPure<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);
        
        [Pure]
        public static AffPure<IEnumerable<A>> SequenceParallel<A>(this IEnumerable<AffPure<A>> ma) =>
            TraverseParallel<A, A>(ma, identity, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static AffPure<IEnumerable<A>> SequenceParallel<A>(this IEnumerable<AffPure<A>> ma, int windowSize) =>
            TraverseParallel<A, A>(ma, identity, windowSize);
 
        [Pure]
        public static AffPure<IEnumerable<B>> TraverseParallel<A, B>(this IEnumerable<AffPure<A>> ma, Func<A, B> f) =>
            TraverseParallel<A, B>(ma, f, Sys.DefaultAsyncSequenceConcurrency);
        
        [Pure]
        public static AffPure<IEnumerable<A>> SequenceSerial<A>(this IEnumerable<AffPure<A>> ma) =>
            TraverseSerial<A, A>(ma, identity);

        [Pure]
        public static AffPure<Lst<A>> SequenceParallel<A>(this Lst<AffPure<A>> ma) =>
            TraverseParallel<A, A>(ma, identity, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static AffPure<Lst<A>> SequenceParallel<A>(this Lst<AffPure<A>> ma, int windowSize) =>
            TraverseParallel<A, A>(ma, identity, windowSize);

        [Pure]
        public static AffPure<Lst<A>> SequenceSerial<A>(this Lst<AffPure<A>> ma) =>
            TraverseSerial<A, A>(ma, identity);
        
        [Pure]
        public static AffPure<Lst<B>> SequenceParallel<A, B>(this Lst<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).SequenceParallel();
        
        [Pure]
        public static AffPure<Lst<B>> SequenceParallel<A, B>(this Lst<A> ta, Func<A, AffPure<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);
        
        [Pure]
        public static AffPure<Lst<B>> SequenceSerial<A, B>(this Lst<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).SequenceSerial();
        
        [Pure]
        public static AffPure<TryAsync<A>> Sequence<A>(this TryAsync<AffPure<A>> mma) =>
            mma.Traverse(identity);
 
        [Pure]
        public static AffPure<TryAsync<B>> Sequence<A, B>(this TryAsync<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static AffPure<Option<A>> Sequence<A>(this Option<AffPure<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static AffPure<Option<B>> Sequence<A, B>(this Option<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static AffPure<OptionUnsafe<A>> Sequence<A>(this OptionUnsafe<AffPure<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static AffPure<OptionUnsafe<B>> Sequence<A, B>(this OptionUnsafe<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).Sequence();

        [Pure]
        public static AffPure<Que<A>> SequenceParallel<A>(this Que<AffPure<A>> ma) =>
            TraverseParallel<A, A>(ma, identity, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static AffPure<Que<A>> SequenceParallel<A>(this Que<AffPure<A>> ma, int windowSize) =>
            TraverseParallel<A, A>(ma, identity, windowSize);
 
        [Pure]
        public static AffPure<Que<A>> SequenceSerial<A>(this Que<AffPure<A>> ma) =>
            TraverseSerial<A, A>(ma, identity);

        [Pure]
        public static AffPure<Que<B>> SequenceParallel<A, B>(this Que<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).SequenceParallel();
 
        [Pure]
        public static AffPure<Que<B>> SequenceParallel<A, B>(this Que<A> ta, Func<A, AffPure<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);
 
        [Pure]
        public static AffPure<Que<B>> SequenceSerial<A, B>(this Que<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).SequenceSerial();
        
        [Pure]
        public static AffPure<Seq<B>> SequenceSerial<A, B>(this Seq<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).SequenceSerial();
        
        [Pure]
        public static AffPure<Seq<B>> SequenceParallel<A, B>(this Seq<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).SequenceParallel();
        
        [Pure]
        public static AffPure<Seq<B>> SequenceParallel<A, B>(this Seq<A> ta, Func<A, AffPure<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);

        [Pure]
        public static AffPure<Seq<A>> SequenceParallel<A>(this Seq<AffPure<A>> ma) =>
            TraverseParallel<A, A>(ma, identity, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static AffPure<Seq<A>> SequenceParallel<A>(this Seq<AffPure<A>> ma, int windowSize) =>
            TraverseParallel<A, A>(ma, identity, windowSize);
 
        [Pure]
        public static AffPure<Seq<A>> SequenceSerial<A>(this Seq<AffPure<A>> ma) =>
            TraverseSerial<A, A>(ma, identity);
        
        [Pure]
        public static AffPure<Set<B>> SequenceSerial<A, B>(this Set<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).SequenceSerial();
        
        [Pure]
        public static AffPure<Set<B>> SequenceParallel<A, B>(this Set<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).SequenceParallel();
        
        [Pure]
        public static AffPure<Set<B>> SequenceParallel<A, B>(this Set<A> ta, Func<A, AffPure<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);
         
        [Pure]
        public static AffPure<Set<A>> SequenceParallel<A>(this Set<AffPure<A>> ma) =>
            TraverseParallel<A, A>(ma, identity, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static AffPure<Set<A>> SequenceParallel<A>(this Set<AffPure<A>> ma, int windowSize) =>
            TraverseParallel<A, A>(ma, identity, windowSize);
 
        [Pure]
        public static AffPure<Set<A>> SequenceSerial<A>(this Set<AffPure<A>> ma) =>
            TraverseSerial<A, A>(ma, identity);

        [Pure]
        public static AffPure<HashSet<A>> SequenceParallel<A>(this HashSet<AffPure<A>> ma) =>
            TraverseParallel<A, A>(ma, identity, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static AffPure<HashSet<A>> SequenceParallel<A>(this HashSet<AffPure<A>> ma, int windowSize) =>
            TraverseParallel<A, A>(ma, identity, windowSize);
 
        [Pure]
        public static AffPure<HashSet<A>> SequenceSerial<A>(this HashSet<AffPure<A>> ma) =>
            TraverseSerial<A, A>(ma, identity);
        
        [Pure]
        public static AffPure<HashSet<B>> SequenceSerial<A, B>(this HashSet<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).SequenceSerial();
        
        [Pure]
        public static AffPure<HashSet<B>> SequenceParallel<A, B>(this HashSet<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).SequenceParallel();
        
        [Pure]
        public static AffPure<HashSet<B>> SequenceParallel<A, B>(this HashSet<A> ta, Func<A, AffPure<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);

        [Pure]
        public static AffPure<Stck<A>> SequenceParallel<A>(this Stck<AffPure<A>> ma) =>
            TraverseParallel<A, A>(ma, identity, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static AffPure<Stck<A>> SequenceParallel<A>(this Stck<AffPure<A>> ma, int windowSize) =>
            TraverseParallel<A, A>(ma, identity, windowSize);
 
        [Pure]
        public static AffPure<Stck<A>> SequenceSerial<A>(this Stck<AffPure<A>> ma) =>
            TraverseSerial<A, A>(ma, identity);
        [Pure]
        public static AffPure<Stck<B>> SequenceSerial<A, B>(this Stck<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).SequenceSerial();
        
        [Pure]
        public static AffPure<Stck<B>> SequenceParallel<A, B>(this Stck<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).SequenceParallel();
        
        [Pure]
        public static AffPure<Stck<B>> SequenceParallel<A, B>(this Stck<A> ta, Func<A, AffPure<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);
        
        [Pure]
        public static AffPure<Try<A>> Sequence<A>(this Try<AffPure<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static AffPure<Try<B>> Sequence<A, B>(this Try<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static AffPure<TryOption<A>> Sequence<A>(this TryOption<AffPure<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static AffPure<TryOption<B>> Sequence<A, B>(this TryOption<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static AffPure<Validation<FAIL, A>> Sequence<FAIL, A>(this Validation<FAIL, AffPure<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static AffPure<Validation<FAIL, B>> Sequence<FAIL, A, B>(this Validation<FAIL, A> ta, Func<A, AffPure<B>> f) => 
            ta.Map(f).Sequence();
        
        [Pure]
        public static AffPure<Validation<MonoidFail, FAIL, A>> Sequence<MonoidFail, FAIL, A>(this Validation<MonoidFail, FAIL, AffPure<A>> mma)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            mma.Traverse(identity);
        
        [Pure]
        public static AffPure<Validation<MonoidFail, FAIL, B>> Sequence<MonoidFail, FAIL, A, B>(this Validation<MonoidFail, FAIL, A> ta, Func<A, AffPure<B>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static AffPure<Task<A>> Sequence<A>(this Task<AffPure<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static AffPure<Task<B>> Sequence<A, B>(this Task<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static AffPure<OptionAsync<A>> Sequence<A>(this OptionAsync<AffPure<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static AffPure<OptionAsync<B>> Sequence<A, B>(this OptionAsync<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static AffPure<TryOptionAsync<A>> Sequence<A>(this TryOptionAsync<AffPure<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static AffPure<TryOptionAsync<B>> Sequence<A, B>(this TryOptionAsync<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static AffPure<EitherAsync<L, A>> Sequence<L, A>(this EitherAsync<L, AffPure<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static AffPure<EitherAsync<L, B>> Sequence<L, A, B>(this EitherAsync<L, A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static AffPure<EffPure<A>> Sequence<A>(this EffPure<AffPure<A>> mma) =>
            mma.Traverse(identity);

        [Pure]
        public static AffPure<EffPure<B>> Sequence<A, B>(this EffPure<A> ta, Func<A, AffPure<B>> f) =>
            ta.Map(f).Sequence();
    }
}
