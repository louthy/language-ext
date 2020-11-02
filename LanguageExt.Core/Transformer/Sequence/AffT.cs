using System;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using static LanguageExt.Prelude; 
using System.Diagnostics.Contracts;
using LanguageExt.Interfaces;

namespace LanguageExt
{
    public partial class AffT
    {
        [Pure]
        public static Aff<RT, Arr<B>> SequenceParallel<RT, A, B>(this Arr<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).SequenceParallel();
        
        [Pure]
        public static Aff<RT, Arr<B>> SequenceParallel<RT, A, B>(this Arr<A> ta, Func<A, Aff<RT, B>> f, int windowSize) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).SequenceParallel(windowSize);
        
        [Pure]
        public static Aff<RT, Arr<B>> SequenceSerial<RT, A, B>(this Arr<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).SequenceSerial();

        [Pure]
        public static Aff<RT, Arr<A>> SequenceParallel<RT, A>(this Arr<Aff<RT, A>> ma)
            where RT : struct, HasCancel<RT> =>
            TraverseParallel<RT, A, A>(ma, identity, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static Aff<RT, Arr<A>> SequenceParallel<RT, A>(this Arr<Aff<RT, A>> ma, int windowSize)
            where RT : struct, HasCancel<RT> =>
            TraverseParallel<RT, A, A>(ma, identity, windowSize);
 
        [Pure]
        public static Aff<RT, Arr<A>> SequenceSerial<RT, A>(this Arr<Aff<RT, A>> ma)
            where RT : struct, HasCancel<RT> =>
            TraverseSerial<RT, A, A>(ma, identity);
        
        [Pure]
        public static Aff<RT, Either<L, A>> Sequence<RT, L, A>(this Either<L, Aff<RT, A>> mma) 
            where RT : struct, HasCancel<RT> =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<RT, Either<L, B>> Sequence<RT, L, A, B>(this Either<L, A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<RT, EitherUnsafe<L, A>> Sequence<RT, L, A>(this EitherUnsafe<L, Aff<RT, A>> mma) 
            where RT : struct, HasCancel<RT> =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<RT, EitherUnsafe<L, B>> Sequence<RT, L, A, B>(this EitherUnsafe<L, A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<RT, Identity<A>> Sequence<RT, A>(this Identity<Aff<RT, A>> mma) 
            where RT : struct, HasCancel<RT> =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<RT, Identity<B>> Sequence<RT, A, B>(this Identity<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).Traverse(Prelude.identity);

        [Pure]
        public static Aff<RT, IEnumerable<B>> SequenceSerial<RT, A, B>(this IEnumerable<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).SequenceSerial();

        [Pure]
        public static Aff<RT, IEnumerable<B>> SequenceParallel<RT, A, B>(this IEnumerable<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).SequenceParallel();

        [Pure]
        public static Aff<RT, IEnumerable<B>> SequenceParallel<RT, A, B>(this IEnumerable<A> ta, Func<A, Aff<RT, B>> f, int windowSize) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).SequenceParallel(windowSize);

        [Pure]
        public static Aff<RT, IEnumerable<A>> SequenceParallel<RT, A>(this IEnumerable<Aff<RT, A>> ma)
            where RT : struct, HasCancel<RT> =>
            TraverseParallel<RT, A, A>(ma, identity, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static Aff<RT, IEnumerable<A>> SequenceParallel<RT, A>(this IEnumerable<Aff<RT, A>> ma, int windowSize)
            where RT : struct, HasCancel<RT> =>
            TraverseParallel<RT, A, A>(ma, identity, windowSize);
 
        [Pure]
        public static Aff<RT, IEnumerable<A>> SequenceSerial<RT, A>(this IEnumerable<Aff<RT, A>> ma)
            where RT : struct, HasCancel<RT> =>
            TraverseSerial<RT, A, A>(ma, identity);
        
        [Pure]
        public static Aff<RT, Lst<B>> SequenceParallel<RT, A, B>(this Lst<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).SequenceParallel();
        
        [Pure]
        public static Aff<RT, Lst<B>> SequenceParallel<RT, A, B>(this Lst<A> ta, Func<A, Aff<RT, B>> f, int windowSize) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).SequenceParallel(windowSize);
        
        [Pure]
        public static Aff<RT, Lst<B>> SequenceSerial<RT, A, B>(this Lst<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).SequenceSerial();

        [Pure]
        public static Aff<RT, Lst<A>> SequenceParallel<RT, A>(this Lst<Aff<RT, A>> ma)
            where RT : struct, HasCancel<RT> =>
            TraverseParallel<RT, A, A>(ma, identity, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static Aff<RT, Lst<A>> SequenceParallel<RT, A>(this Lst<Aff<RT, A>> ma, int windowSize)
            where RT : struct, HasCancel<RT> =>
            TraverseParallel<RT, A, A>(ma, identity, windowSize);
 
        [Pure]
        public static Aff<RT, Lst<A>> SequenceSerial<RT, A>(this Lst<Aff<RT, A>> ma)
            where RT : struct, HasCancel<RT> =>
            TraverseSerial<RT, A, A>(ma, identity);
        
        [Pure]
        public static Aff<RT, TryAsync<A>> Sequence<RT, A>(this TryAsync<Aff<RT, A>> mma) 
            where RT : struct, HasCancel<RT> =>
            mma.Traverse(identity);
 
        [Pure]
        public static Aff<RT, TryAsync<B>> Sequence<RT, A, B>(this TryAsync<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<RT, Fin<A>> Sequence<RT, A>(this Fin<Aff<RT, A>> mma) 
            where RT : struct, HasCancel<RT> =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<RT, Fin<B>> Sequence<RT, A, B>(this Fin<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<RT, Option<A>> Sequence<RT, A>(this Option<Aff<RT, A>> mma) 
            where RT : struct, HasCancel<RT> =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<RT, Option<B>> Sequence<RT, A, B>(this Option<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<RT, OptionUnsafe<A>> Sequence<RT, A>(this OptionUnsafe<Aff<RT, A>> mma) 
            where RT : struct, HasCancel<RT> =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<RT, OptionUnsafe<B>> Sequence<RT, A, B>(this OptionUnsafe<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<RT, Seq<B>> SequenceSerial<RT, A, B>(this Seq<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).SequenceSerial();
        
        [Pure]
        public static Aff<RT, Seq<B>> SequenceParallel<RT, A, B>(this Seq<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).SequenceParallel();
        
        [Pure]
        public static Aff<RT, Seq<A>> SequenceParallel<RT, A>(this Seq<Aff<RT, A>> ma)
            where RT : struct, HasCancel<RT> =>
            TraverseParallel<RT, A, A>(ma, identity, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static Aff<RT, Seq<A>> SequenceParallel<RT, A>(this Seq<Aff<RT, A>> ma, int windowSize)
            where RT : struct, HasCancel<RT> =>
            TraverseParallel<RT, A, A>(ma, identity, windowSize);
 
        [Pure]
        public static Aff<RT, Seq<A>> SequenceSerial<RT, A>(this Seq<Aff<RT, A>> ma)
            where RT : struct, HasCancel<RT> =>
            TraverseSerial<RT, A, A>(ma, identity);
        
        [Pure]
        public static Aff<RT, Seq<B>> SequenceParallel<RT, A, B>(this Seq<A> ta, Func<A, Aff<RT, B>> f, int windowSize) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).SequenceParallel(windowSize);
        
        [Pure]
        public static Aff<RT, Set<B>> SequenceSerial<RT, A, B>(this Set<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).SequenceSerial();
        
        [Pure]
        public static Aff<RT, Set<B>> SequenceParallel<RT, A, B>(this Set<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).SequenceParallel();
        
        [Pure]
        public static Aff<RT, Set<B>> SequenceParallel<RT, A, B>(this Set<A> ta, Func<A, Aff<RT, B>> f, int windowSize) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).SequenceParallel(windowSize);
         
        [Pure]
        public static Aff<RT, Set<A>> SequenceParallel<RT, A>(this Set<Aff<RT, A>> ma)
            where RT : struct, HasCancel<RT> =>
            TraverseParallel<RT, A, A>(ma, identity, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static Aff<RT, Set<A>> SequenceParallel<RT, A>(this Set<Aff<RT, A>> ma, int windowSize)
            where RT : struct, HasCancel<RT> =>
            TraverseParallel<RT, A, A>(ma, identity, windowSize);
        
        [Pure]
        public static Aff<RT, Set<A>> SequenceSerial<RT, A>(this Set<Aff<RT, A>> ma)
            where RT : struct, HasCancel<RT> =>
            TraverseSerial<RT, A, A>(ma, identity);
        
        [Pure]
        public static Aff<RT, HashSet<B>> SequenceSerial<RT, A, B>(this HashSet<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).SequenceSerial();
        
        [Pure]
        public static Aff<RT, HashSet<B>> SequenceParallel<RT, A, B>(this HashSet<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).SequenceParallel();
        
        [Pure]
        public static Aff<RT, HashSet<B>> SequenceParallel<RT, A, B>(this HashSet<A> ta, Func<A, Aff<RT, B>> f, int windowSize) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).SequenceParallel(windowSize);

        [Pure]
        public static Aff<RT, HashSet<A>> SequenceParallel<RT, A>(this HashSet<Aff<RT, A>> ma)
            where RT : struct, HasCancel<RT> =>
            TraverseParallel<RT, A, A>(ma, identity, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static Aff<RT, HashSet<A>> SequenceParallel<RT, A>(this HashSet<Aff<RT, A>> ma, int windowSize)
            where RT : struct, HasCancel<RT> =>
            TraverseParallel<RT, A, A>(ma, identity, windowSize);
 
        [Pure]
        public static Aff<RT, HashSet<A>> SequenceSerial<RT, A>(this HashSet<Aff<RT, A>> ma)
            where RT : struct, HasCancel<RT> =>
            TraverseSerial<RT, A, A>(ma, identity);
        
        [Pure]
        public static Aff<RT, Que<B>> SequenceSerial<RT, A, B>(this Que<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).SequenceSerial();
        
        [Pure]
        public static Aff<RT, Que<B>> SequenceParallel<RT, A, B>(this Que<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).SequenceParallel();
        
        [Pure]
        public static Aff<RT, Que<B>> SequenceParallel<RT, A, B>(this Que<A> ta, Func<A, Aff<RT, B>> f, int windowSize) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).SequenceParallel(windowSize);
        
        [Pure]
        public static Aff<RT, Que<A>> SequenceParallel<RT, A>(this Que<Aff<RT, A>> ma)
            where RT : struct, HasCancel<RT> =>
            TraverseParallel<RT, A, A>(ma, identity, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static Aff<RT, Que<A>> SequenceParallel<RT, A>(this Que<Aff<RT, A>> ma, int windowSize)
            where RT : struct, HasCancel<RT> =>
            TraverseParallel<RT, A, A>(ma, identity, windowSize);
        
        [Pure]
        public static Aff<RT, Que<A>> SequenceSerial<RT, A>(this Que<Aff<RT, A>> ma)
            where RT : struct, HasCancel<RT> =>
            TraverseSerial<RT, A, A>(ma, identity);
        
        [Pure]
        public static Aff<RT, Stck<B>> SequenceSerial<RT, A, B>(this Stck<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).SequenceSerial();
        
        [Pure]
        public static Aff<RT, Stck<B>> SequenceParallel<RT, A, B>(this Stck<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).SequenceParallel();
        
        [Pure]
        public static Aff<RT, Stck<B>> SequenceParallel<RT, A, B>(this Stck<A> ta, Func<A, Aff<RT, B>> f, int windowSize) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).SequenceParallel(windowSize);
        
        [Pure]
        public static Aff<RT, Stck<A>> SequenceSerial<RT, A>(this Stck<Aff<RT, A>> ma)
            where RT : struct, HasCancel<RT> =>
            TraverseSerial<RT, A, A>(ma, identity);
        
        [Pure]
        public static Aff<RT, Stck<A>> SequenceParallel<RT, A>(this Stck<Aff<RT, A>> ma)
            where RT : struct, HasCancel<RT> =>
            TraverseParallel<RT, A, A>(ma, identity, Sys.DefaultAsyncSequenceConcurrency);
 
        [Pure]
        public static Aff<RT, Stck<A>> SequenceParallel<RT, A>(this Stck<Aff<RT, A>> ma, int windowSize)
            where RT : struct, HasCancel<RT> =>
            TraverseParallel<RT, A, A>(ma, identity, windowSize);
        
        [Pure]
        public static Aff<RT, Try<A>> Sequence<RT, A>(this Try<Aff<RT, A>> mma) 
            where RT : struct, HasCancel<RT> =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<RT, Try<B>> Sequence<RT, A, B>(this Try<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<RT, TryOption<A>> Sequence<RT, A>(this TryOption<Aff<RT, A>> mma) 
            where RT : struct, HasCancel<RT> =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<RT, TryOption<B>> Sequence<RT, A, B>(this TryOption<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<RT, Validation<FAIL, A>> Sequence<RT, FAIL, A>(this Validation<FAIL, Aff<RT, A>> mma) 
            where RT : struct, HasCancel<RT> =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<RT, Validation<FAIL, B>> Sequence<RT, FAIL, A, B>(this Validation<FAIL, A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> => 
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<RT, Validation<MonoidFail, FAIL, A>> Sequence<RT, MonoidFail, FAIL, A>(this Validation<MonoidFail, FAIL, Aff<RT, A>> mma)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> 
            where RT : struct, HasCancel<RT> =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<RT, Validation<MonoidFail, FAIL, B>> Sequence<RT, MonoidFail, FAIL, A, B>(this Validation<MonoidFail, FAIL, A> ta, Func<A, Aff<RT, B>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<RT, Task<A>> Sequence<RT, A>(this Task<Aff<RT, A>> mma) 
            where RT : struct, HasCancel<RT> =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<RT, Task<B>> Sequence<RT, A, B>(this Task<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<RT, OptionAsync<A>> Sequence<RT, A>(this OptionAsync<Aff<RT, A>> mma) 
            where RT : struct, HasCancel<RT> =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<RT, OptionAsync<B>> Sequence<RT, A, B>(this OptionAsync<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<RT, TryOptionAsync<A>> Sequence<RT, A>(this TryOptionAsync<Aff<RT, A>> mma) 
            where RT : struct, HasCancel<RT> =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<RT, TryOptionAsync<B>> Sequence<RT, A, B>(this TryOptionAsync<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<RT, EitherAsync<L, A>> Sequence<RT, L, A>(this EitherAsync<L, Aff<RT, A>> mma) 
            where RT : struct, HasCancel<RT> =>
            mma.Traverse(identity);
        
        [Pure]
        public static Aff<RT, EitherAsync<L, B>> Sequence<RT, L, A, B>(this EitherAsync<L, A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).Sequence();

        [Pure]
        public static Aff<RT, Aff<A>> Sequence<RT, A>(this Aff<Aff<RT, A>> mma)
            where RT : struct, HasCancel<RT> =>
            mma.Traverse(identity);

        [Pure]
        public static Aff<RT, Aff<B>> Sequence<RT, A, B>(this Aff<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).Sequence();
        
        [Pure]
        public static Aff<RT, Eff<A>> Sequence<RT, A>(this Eff<Aff<RT, A>> mma) 
            where RT : struct, HasCancel<RT> =>
            mma.Traverse(identity);

        [Pure]
        public static Aff<RT, Eff<B>> Sequence<RT, A, B>(this Eff<A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).Sequence();
            
        [Pure]
        public static Aff<RT, Eff<RT, A>> Sequence<RT, A>(this Eff<RT, Aff<RT, A>> mma) 
            where RT : struct, HasCancel<RT> =>
            mma.Traverse(identity);

        [Pure]
        public static Aff<RT, Eff<RT, B>> Sequence<RT, A, B>(this Eff<RT, A> ta, Func<A, Aff<RT, B>> f) 
            where RT : struct, HasCancel<RT> =>
            ta.Map(f).Sequence();
    }
}
