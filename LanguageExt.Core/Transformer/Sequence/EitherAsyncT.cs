using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class EitherAsyncT
    {
        public static EitherAsync<L, Arr<B>> Sequence<L, A, B>(this Arr<A> ta, Func<A, EitherAsync<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static EitherAsync<L, Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, EitherAsync<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static EitherAsync<L, EitherAsync<L, B>> Sequence<L, A, B>(this EitherAsync<L, A> ta, Func<A, EitherAsync<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static EitherAsync<L, Identity<B>> Sequence<L, A, B>(this Identity<A> ta, Func<A, EitherAsync<L, B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);

        public static EitherAsync<L, IEnumerable<B>> SequenceSerial<L, A, B>(this IEnumerable<A> ta, Func<A, EitherAsync<L, B>> f) =>
            ta.Map(f).SequenceSerial();

        public static EitherAsync<L, IEnumerable<B>> SequenceParallel<L, A, B>(this IEnumerable<A> ta, Func<A, EitherAsync<L, B>> f) =>
            ta.Map(f).SequenceParallel();

        public static EitherAsync<L, IEnumerable<B>> SequenceParallel<L, A, B>(this IEnumerable<A> ta, Func<A, EitherAsync<L, B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);
        
        public static EitherAsync<L, Lst<B>> Sequence<L, A, B>(this Lst<A> ta, Func<A, EitherAsync<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static EitherAsync<L, Option<B>> Sequence<L, A, B>(this Option<A> ta, Func<A, EitherAsync<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static EitherAsync<L, OptionUnsafe<B>> Sequence<L, A, B>(this OptionUnsafe<A> ta, Func<A, EitherAsync<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static EitherAsync<L, Seq<B>> SequenceSerial<L, A, B>(this Seq<A> ta, Func<A, EitherAsync<L, B>> f) =>
            ta.Map(f).SequenceSerial();
        
        public static EitherAsync<L, Seq<B>> SequenceParallel<L, A, B>(this Seq<A> ta, Func<A, EitherAsync<L, B>> f) =>
            ta.Map(f).SequenceParallel();
        
        public static EitherAsync<L, Seq<B>> SequenceParallel<L, A, B>(this Seq<A> ta, Func<A, EitherAsync<L, B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);
        
        public static EitherAsync<L, Set<B>> Sequence<L, A, B>(this Set<A> ta, Func<A, EitherAsync<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static EitherAsync<L, HashSet<B>> Sequence<L, A, B>(this HashSet<A> ta, Func<A, EitherAsync<L, B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static EitherAsync<L, Try<B>> Sequence<L, A, B>(this Try<A> ta, Func<A, EitherAsync<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static EitherAsync<L, TryOption<B>> Sequence<L, A, B>(this TryOption<A> ta, Func<A, EitherAsync<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static EitherAsync<L, Validation<FAIL, B>> Sequence<FAIL, L, A, B>(this Validation<FAIL, A> ta, Func<A, EitherAsync<L, B>> f) => 
            ta.Map(f).Sequence();
        
        public static EitherAsync<L, TryAsync<B>> Sequence<L, A, B>(this TryAsync<A> ta, Func<A, EitherAsync<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static EitherAsync<L, Task<B>> Sequence<L, A, B>(this Task<A> ta, Func<A, EitherAsync<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static EitherAsync<L, TryOptionAsync<B>> Sequence<L, A, B>(this TryOptionAsync<A> ta, Func<A, EitherAsync<L, B>> f) =>
            ta.Map(f).Sequence();
        
        public static EitherAsync<L, OptionAsync<B>> Sequence<L, A, B>(this OptionAsync<A> ta, Func<A, EitherAsync<L, B>> f) =>
            ta.Map(f).Sequence();    
    }
}
