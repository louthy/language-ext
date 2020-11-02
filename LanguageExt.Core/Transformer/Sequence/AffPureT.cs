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
        public static Aff<Arr<A>> SequenceParallel<A>(this Arr<Aff<A>> ma) =>
            TraverseParallel<A, A>(ma, identity, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static Aff<Arr<A>> SequenceParallel<A>(this Arr<Aff<A>> ma, int windowSize) =>
            TraverseParallel<A, A>(ma, identity, windowSize);
        [Pure]
        public static Aff<Arr<B>> SequenceParallel<A, B>(this Arr<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).SequenceParallel();
        
        [Pure]
        public static Aff<Arr<B>> SequenceParallel<A, B>(this Arr<A> ta, Func<A, Aff<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);
        
        [Pure]
        public static Aff<Arr<A>> SequenceSerial<A>(this Arr<Aff<A>> ma) =>
            TraverseSerial<A, A>(ma, identity);
 
        [Pure]
        public static Aff<Arr<B>> SequenceSerial<A, B>(this Arr<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).SequenceSerial();
        
        [Pure]
        public static Aff<Either<L, A>> Sequence<L, A>(this Either<L, Aff<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<Fin<A>> Sequence<A>(this Fin<Aff<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<Fin<B>> Sequence<A, B>(this Fin<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<EitherUnsafe<L, A>> Sequence<L, A>(this EitherUnsafe<L, Aff<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<EitherUnsafe<L, B>> Sequence<L, A, B>(this EitherUnsafe<L, A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<Identity<A>> Sequence<A>(this Identity<Aff<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<Identity<B>> Sequence<A, B>(this Identity<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);

        [Pure]
        public static Aff<IEnumerable<B>> SequenceSerial<A, B>(this IEnumerable<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).SequenceSerial();

        [Pure]
        public static Aff<IEnumerable<B>> SequenceParallel<A, B>(this IEnumerable<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).SequenceParallel();

        [Pure]
        public static Aff<IEnumerable<B>> SequenceParallel<A, B>(this IEnumerable<A> ta, Func<A, Aff<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);
        
        [Pure]
        public static Aff<IEnumerable<A>> SequenceParallel<A>(this IEnumerable<Aff<A>> ma) =>
            TraverseParallel<A, A>(ma, identity, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static Aff<IEnumerable<A>> SequenceParallel<A>(this IEnumerable<Aff<A>> ma, int windowSize) =>
            TraverseParallel<A, A>(ma, identity, windowSize);
 
        [Pure]
        public static Aff<IEnumerable<B>> TraverseParallel<A, B>(this IEnumerable<Aff<A>> ma, Func<A, B> f) =>
            TraverseParallel<A, B>(ma, f, Sys.DefaultAsyncSequenceConcurrency);
        
        [Pure]
        public static Aff<IEnumerable<A>> SequenceSerial<A>(this IEnumerable<Aff<A>> ma) =>
            TraverseSerial<A, A>(ma, identity);

        [Pure]
        public static Aff<Lst<A>> SequenceParallel<A>(this Lst<Aff<A>> ma) =>
            TraverseParallel<A, A>(ma, identity, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static Aff<Lst<A>> SequenceParallel<A>(this Lst<Aff<A>> ma, int windowSize) =>
            TraverseParallel<A, A>(ma, identity, windowSize);

        [Pure]
        public static Aff<Lst<A>> SequenceSerial<A>(this Lst<Aff<A>> ma) =>
            TraverseSerial<A, A>(ma, identity);
        
        [Pure]
        public static Aff<Lst<B>> SequenceParallel<A, B>(this Lst<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).SequenceParallel();
        
        [Pure]
        public static Aff<Lst<B>> SequenceParallel<A, B>(this Lst<A> ta, Func<A, Aff<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);
        
        [Pure]
        public static Aff<Lst<B>> SequenceSerial<A, B>(this Lst<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).SequenceSerial();
        
        [Pure]
        public static Aff<TryAsync<A>> Sequence<A>(this TryAsync<Aff<A>> mma) =>
            mma.Traverse(identity);
 
        [Pure]
        public static Aff<TryAsync<B>> Sequence<A, B>(this TryAsync<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<Option<A>> Sequence<A>(this Option<Aff<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<Option<B>> Sequence<A, B>(this Option<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<OptionUnsafe<A>> Sequence<A>(this OptionUnsafe<Aff<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<OptionUnsafe<B>> Sequence<A, B>(this OptionUnsafe<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).Sequence();

        [Pure]
        public static Aff<Que<A>> SequenceParallel<A>(this Que<Aff<A>> ma) =>
            TraverseParallel<A, A>(ma, identity, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static Aff<Que<A>> SequenceParallel<A>(this Que<Aff<A>> ma, int windowSize) =>
            TraverseParallel<A, A>(ma, identity, windowSize);
 
        [Pure]
        public static Aff<Que<A>> SequenceSerial<A>(this Que<Aff<A>> ma) =>
            TraverseSerial<A, A>(ma, identity);

        [Pure]
        public static Aff<Que<B>> SequenceParallel<A, B>(this Que<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).SequenceParallel();
 
        [Pure]
        public static Aff<Que<B>> SequenceParallel<A, B>(this Que<A> ta, Func<A, Aff<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);
 
        [Pure]
        public static Aff<Que<B>> SequenceSerial<A, B>(this Que<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).SequenceSerial();
        
        [Pure]
        public static Aff<Seq<B>> SequenceSerial<A, B>(this Seq<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).SequenceSerial();
        
        [Pure]
        public static Aff<Seq<B>> SequenceParallel<A, B>(this Seq<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).SequenceParallel();
        
        [Pure]
        public static Aff<Seq<B>> SequenceParallel<A, B>(this Seq<A> ta, Func<A, Aff<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);

        [Pure]
        public static Aff<Seq<A>> SequenceParallel<A>(this Seq<Aff<A>> ma) =>
            TraverseParallel<A, A>(ma, identity, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static Aff<Seq<A>> SequenceParallel<A>(this Seq<Aff<A>> ma, int windowSize) =>
            TraverseParallel<A, A>(ma, identity, windowSize);
 
        [Pure]
        public static Aff<Seq<A>> SequenceSerial<A>(this Seq<Aff<A>> ma) =>
            TraverseSerial<A, A>(ma, identity);
        
        [Pure]
        public static Aff<Set<B>> SequenceSerial<A, B>(this Set<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).SequenceSerial();
        
        [Pure]
        public static Aff<Set<B>> SequenceParallel<A, B>(this Set<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).SequenceParallel();
        
        [Pure]
        public static Aff<Set<B>> SequenceParallel<A, B>(this Set<A> ta, Func<A, Aff<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);
         
        [Pure]
        public static Aff<Set<A>> SequenceParallel<A>(this Set<Aff<A>> ma) =>
            TraverseParallel<A, A>(ma, identity, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static Aff<Set<A>> SequenceParallel<A>(this Set<Aff<A>> ma, int windowSize) =>
            TraverseParallel<A, A>(ma, identity, windowSize);
 
        [Pure]
        public static Aff<Set<A>> SequenceSerial<A>(this Set<Aff<A>> ma) =>
            TraverseSerial<A, A>(ma, identity);

        [Pure]
        public static Aff<HashSet<A>> SequenceParallel<A>(this HashSet<Aff<A>> ma) =>
            TraverseParallel<A, A>(ma, identity, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static Aff<HashSet<A>> SequenceParallel<A>(this HashSet<Aff<A>> ma, int windowSize) =>
            TraverseParallel<A, A>(ma, identity, windowSize);
 
        [Pure]
        public static Aff<HashSet<A>> SequenceSerial<A>(this HashSet<Aff<A>> ma) =>
            TraverseSerial<A, A>(ma, identity);
        
        [Pure]
        public static Aff<HashSet<B>> SequenceSerial<A, B>(this HashSet<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).SequenceSerial();
        
        [Pure]
        public static Aff<HashSet<B>> SequenceParallel<A, B>(this HashSet<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).SequenceParallel();
        
        [Pure]
        public static Aff<HashSet<B>> SequenceParallel<A, B>(this HashSet<A> ta, Func<A, Aff<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);

        [Pure]
        public static Aff<Stck<A>> SequenceParallel<A>(this Stck<Aff<A>> ma) =>
            TraverseParallel<A, A>(ma, identity, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static Aff<Stck<A>> SequenceParallel<A>(this Stck<Aff<A>> ma, int windowSize) =>
            TraverseParallel<A, A>(ma, identity, windowSize);
 
        [Pure]
        public static Aff<Stck<A>> SequenceSerial<A>(this Stck<Aff<A>> ma) =>
            TraverseSerial<A, A>(ma, identity);
        [Pure]
        public static Aff<Stck<B>> SequenceSerial<A, B>(this Stck<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).SequenceSerial();
        
        [Pure]
        public static Aff<Stck<B>> SequenceParallel<A, B>(this Stck<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).SequenceParallel();
        
        [Pure]
        public static Aff<Stck<B>> SequenceParallel<A, B>(this Stck<A> ta, Func<A, Aff<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);
        
        [Pure]
        public static Aff<Try<A>> Sequence<A>(this Try<Aff<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<Try<B>> Sequence<A, B>(this Try<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<TryOption<A>> Sequence<A>(this TryOption<Aff<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<TryOption<B>> Sequence<A, B>(this TryOption<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<Validation<FAIL, A>> Sequence<FAIL, A>(this Validation<FAIL, Aff<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<Validation<FAIL, B>> Sequence<FAIL, A, B>(this Validation<FAIL, A> ta, Func<A, Aff<B>> f) => 
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<Validation<MonoidFail, FAIL, A>> Sequence<MonoidFail, FAIL, A>(this Validation<MonoidFail, FAIL, Aff<A>> mma)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<Validation<MonoidFail, FAIL, B>> Sequence<MonoidFail, FAIL, A, B>(this Validation<MonoidFail, FAIL, A> ta, Func<A, Aff<B>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<Task<A>> Sequence<A>(this Task<Aff<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<Task<B>> Sequence<A, B>(this Task<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<OptionAsync<A>> Sequence<A>(this OptionAsync<Aff<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<OptionAsync<B>> Sequence<A, B>(this OptionAsync<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<TryOptionAsync<A>> Sequence<A>(this TryOptionAsync<Aff<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<TryOptionAsync<B>> Sequence<A, B>(this TryOptionAsync<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<EitherAsync<L, A>> Sequence<L, A>(this EitherAsync<L, Aff<A>> mma) =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<EitherAsync<L, B>> Sequence<L, A, B>(this EitherAsync<L, A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<Eff<A>> Sequence<A>(this Eff<Aff<A>> mma) =>
            mma.Traverse(identity);

        [Pure]
        public static Aff<Eff<B>> Sequence<A, B>(this Eff<A> ta, Func<A, Aff<B>> f) =>
            ta.Map(f).Sequence();
    }
}
