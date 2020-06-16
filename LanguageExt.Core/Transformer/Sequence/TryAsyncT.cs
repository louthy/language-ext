using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class TryAsyncT
    {
        public static TryAsync<Arr<B>> Sequence<A, B>(this Arr<A> ta, Func<A, TryAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryAsync<Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, TryAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryAsync<EitherUnsafe<L, B>> Sequence<L, A, B>(this EitherUnsafe<L, A> ta, Func<A, TryAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryAsync<Identity<B>> Sequence<A, B>(this Identity<A> ta, Func<A, TryAsync<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);

        public static TryAsync<IEnumerable<B>> SequenceSerial<A, B>(this IEnumerable<A> ta, Func<A, TryAsync<B>> f) =>
            ta.Map(f).SequenceSerial();

        public static TryAsync<IEnumerable<B>> SequenceParallel<A, B>(this IEnumerable<A> ta, Func<A, TryAsync<B>> f) =>
            ta.Map(f).SequenceParallel();

        public static TryAsync<IEnumerable<B>> SequenceParallel<A, B>(this IEnumerable<A> ta, Func<A, TryAsync<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);
        
        public static TryAsync<Lst<B>> Sequence<A, B>(this Lst<A> ta, Func<A, TryAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryAsync<TryAsync<B>> Sequence<A, B>(this TryAsync<A> ta, Func<A, TryAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryAsync<OptionUnsafe<B>> Sequence<A, B>(this OptionUnsafe<A> ta, Func<A, TryAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryAsync<Seq<B>> SequenceSerial<A, B>(this Seq<A> ta, Func<A, TryAsync<B>> f) =>
            ta.Map(f).SequenceSerial();
        
        public static TryAsync<Seq<B>> SequenceParallel<A, B>(this Seq<A> ta, Func<A, TryAsync<B>> f) =>
            ta.Map(f).SequenceParallel();
        
        public static TryAsync<Seq<B>> SequenceParallel<A, B>(this Seq<A> ta, Func<A, TryAsync<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);
        
        public static TryAsync<Set<B>> Sequence<A, B>(this Set<A> ta, Func<A, TryAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryAsync<HashSet<B>> Sequence<A, B>(this HashSet<A> ta, Func<A, TryAsync<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static TryAsync<Try<B>> Sequence<A, B>(this Try<A> ta, Func<A, TryAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryAsync<TryOption<B>> Sequence<A, B>(this TryOption<A> ta, Func<A, TryAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryAsync<Validation<FAIL, B>> Sequence<FAIL, A, B>(this Validation<FAIL, A> ta, Func<A, TryAsync<B>> f) => 
            ta.Map(f).Sequence();
        
        public static TryAsync<Validation<MonoidFail, FAIL, B>> Sequence<MonoidFail, FAIL, A, B>(this Validation<MonoidFail, FAIL, A> ta, Func<A, TryAsync<B>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static TryAsync<Task<B>> Sequence<A, B>(this Task<A> ta, Func<A, TryAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryAsync<OptionAsync<B>> Sequence<A, B>(this OptionAsync<A> ta, Func<A, TryAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryAsync<TryOptionAsync<B>> Sequence<A, B>(this TryOptionAsync<A> ta, Func<A, TryAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryAsync<EitherAsync<L, B>> Sequence<L, A, B>(this EitherAsync<L, A> ta, Func<A, TryAsync<B>> f) =>
            ta.Map(f).Sequence();
    }
}
