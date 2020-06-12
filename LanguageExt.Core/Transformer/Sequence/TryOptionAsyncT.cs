using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class TryOptionAsyncT
    {
        public static TryOptionAsync<Arr<B>> Sequence<A, B>(this Arr<A> ta, Func<A, TryOptionAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryOptionAsync<Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, TryOptionAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryOptionAsync<EitherUnsafe<L, B>> Sequence<L, A, B>(this EitherUnsafe<L, A> ta, Func<A, TryOptionAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryOptionAsync<Identity<B>> Sequence<A, B>(this Identity<A> ta, Func<A, TryOptionAsync<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);

        public static TryOptionAsync<IEnumerable<B>> SequenceSerial<A, B>(this IEnumerable<A> ta, Func<A, TryOptionAsync<B>> f) =>
            ta.Map(f).SequenceSerial();

        public static TryOptionAsync<IEnumerable<B>> SequenceParallel<A, B>(this IEnumerable<A> ta, Func<A, TryOptionAsync<B>> f) =>
            ta.Map(f).SequenceParallel();

        public static TryOptionAsync<IEnumerable<B>> SequenceParallel<A, B>(this IEnumerable<A> ta, Func<A, TryOptionAsync<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);
        
        public static TryOptionAsync<Lst<B>> Sequence<A, B>(this Lst<A> ta, Func<A, TryOptionAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryOptionAsync<TryAsync<B>> Sequence<A, B>(this TryAsync<A> ta, Func<A, TryOptionAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryOptionAsync<OptionUnsafe<B>> Sequence<A, B>(this OptionUnsafe<A> ta, Func<A, TryOptionAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryOptionAsync<Seq<B>> SequenceSerial<A, B>(this Seq<A> ta, Func<A, TryOptionAsync<B>> f) =>
            ta.Map(f).SequenceSerial();
        
        public static TryOptionAsync<Seq<B>> SequenceParallel<A, B>(this Seq<A> ta, Func<A, TryOptionAsync<B>> f) =>
            ta.Map(f).SequenceParallel();
        
        public static TryOptionAsync<Seq<B>> SequenceParallel<A, B>(this Seq<A> ta, Func<A, TryOptionAsync<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);
        
        public static TryOptionAsync<Set<B>> Sequence<A, B>(this Set<A> ta, Func<A, TryOptionAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryOptionAsync<HashSet<B>> Sequence<A, B>(this HashSet<A> ta, Func<A, TryOptionAsync<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static TryOptionAsync<Try<B>> Sequence<A, B>(this Try<A> ta, Func<A, TryOptionAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryOptionAsync<TryOption<B>> Sequence<A, B>(this TryOption<A> ta, Func<A, TryOptionAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryOptionAsync<Validation<FAIL, B>> Sequence<FAIL, A, B>(this Validation<FAIL, A> ta, Func<A, TryOptionAsync<B>> f) => 
            ta.Map(f).Sequence();
        
        public static TryOptionAsync<Validation<MonoidFail, FAIL, B>> Sequence<MonoidFail, FAIL, A, B>(this Validation<MonoidFail, FAIL, A> ta, Func<A, TryOptionAsync<B>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static TryOptionAsync<Task<B>> Sequence<A, B>(this Task<A> ta, Func<A, TryOptionAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryOptionAsync<OptionAsync<B>> Sequence<A, B>(this OptionAsync<A> ta, Func<A, TryOptionAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryAsync<Task<B>> Sequence<A, B>(this Task<A> ta, Func<A, TryAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryOptionAsync<TryOptionAsync<B>> Sequence<A, B>(this TryOptionAsync<A> ta, Func<A, TryOptionAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static TryOptionAsync<EitherAsync<L, B>> Sequence<L, A, B>(this EitherAsync<L, A> ta, Func<A, TryOptionAsync<B>> f) =>
            ta.Map(f).Sequence();
    }
}
