using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class OptionAsyncT
    {
        public static OptionAsync<Arr<B>> Sequence<A, B>(this Arr<A> ta, Func<A, OptionAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static OptionAsync<Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, OptionAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static OptionAsync<EitherUnsafe<L, B>> Sequence<L, A, B>(this EitherUnsafe<L, A> ta, Func<A, OptionAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static OptionAsync<Identity<B>> Sequence<A, B>(this Identity<A> ta, Func<A, OptionAsync<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);

        public static OptionAsync<IEnumerable<B>> SequenceSerial<A, B>(this IEnumerable<A> ta, Func<A, OptionAsync<B>> f) =>
            ta.Map(f).SequenceSerial();

        public static OptionAsync<IEnumerable<B>> SequenceParallel<A, B>(this IEnumerable<A> ta, Func<A, OptionAsync<B>> f) =>
            ta.Map(f).SequenceParallel();

        public static OptionAsync<IEnumerable<B>> SequenceParallel<A, B>(this IEnumerable<A> ta, Func<A, OptionAsync<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);
        
        public static OptionAsync<Lst<B>> Sequence<A, B>(this Lst<A> ta, Func<A, OptionAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static OptionAsync<OptionAsync<B>> Sequence<A, B>(this OptionAsync<A> ta, Func<A, OptionAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static OptionAsync<OptionUnsafe<B>> Sequence<A, B>(this OptionUnsafe<A> ta, Func<A, OptionAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static OptionAsync<Seq<B>> SequenceSerial<A, B>(this Seq<A> ta, Func<A, OptionAsync<B>> f) =>
            ta.Map(f).SequenceSerial();
        
        public static OptionAsync<Seq<B>> SequenceParallel<A, B>(this Seq<A> ta, Func<A, OptionAsync<B>> f) =>
            ta.Map(f).SequenceParallel();
        
        public static OptionAsync<Seq<B>> SequenceParallel<A, B>(this Seq<A> ta, Func<A, OptionAsync<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);
        
        public static OptionAsync<Set<B>> Sequence<A, B>(this Set<A> ta, Func<A, OptionAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static OptionAsync<HashSet<B>> Sequence<A, B>(this HashSet<A> ta, Func<A, OptionAsync<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static OptionAsync<Try<B>> Sequence<A, B>(this Try<A> ta, Func<A, OptionAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static OptionAsync<Validation<FAIL, B>> Sequence<FAIL, A, B>(this Validation<FAIL, A> ta, Func<A, OptionAsync<B>> f) => 
            ta.Map(f).Sequence();
        
        public static OptionAsync<Validation<MonoidFail, FAIL, B>> Sequence<MonoidFail, FAIL, A, B>(this Validation<MonoidFail, FAIL, A> ta, Func<A, OptionAsync<B>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static OptionAsync<TryAsync<B>> Sequence<A, B>(this TryAsync<A> ta, Func<A, OptionAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static OptionAsync<Task<B>> Sequence<A, B>(this Task<A> ta, Func<A, OptionAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static OptionAsync<TryOptionAsync<B>> Sequence<A, B>(this TryOptionAsync<A> ta, Func<A, OptionAsync<B>> f) =>
            ta.Map(f).Sequence();
        
        public static OptionAsync<EitherAsync<L, B>> Sequence<L, A, B>(this EitherAsync<L, A> ta, Func<A, OptionAsync<B>> f) =>
            ta.Map(f).Sequence();    
    }
}
