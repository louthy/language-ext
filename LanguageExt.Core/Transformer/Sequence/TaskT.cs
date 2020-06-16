using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class TaskT
    {
        public static Task<Arr<B>> Sequence<A, B>(this Arr<A> ta, Func<A, Task<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Task<Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, Task<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Task<EitherUnsafe<L, B>> Sequence<L, A, B>(this EitherUnsafe<L, A> ta, Func<A, Task<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Task<Identity<B>> Sequence<A, B>(this Identity<A> ta, Func<A, Task<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);

        public static Task<IEnumerable<B>> SequenceSerial<A, B>(this IEnumerable<A> ta, Func<A, Task<B>> f) =>
            ta.Map(f).SequenceSerial();

        public static Task<IEnumerable<B>> SequenceParallel<A, B>(this IEnumerable<A> ta, Func<A, Task<B>> f) =>
            ta.Map(f).SequenceParallel();

        public static Task<IEnumerable<B>> SequenceParallel<A, B>(this IEnumerable<A> ta, Func<A, Task<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);
 
        public static Task<Task<B>> Sequence<A, B>(this Task<A> ta, Func<A, Task<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Task<Option<B>> Sequence<A, B>(this Option<A> ta, Func<A, Task<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Task<OptionUnsafe<B>> Sequence<A, B>(this OptionUnsafe<A> ta, Func<A, Task<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Task<Seq<B>> SequenceSerial<A, B>(this Seq<A> ta, Func<A, Task<B>> f) =>
            ta.Map(f).SequenceSerial();
        
        public static Task<Seq<B>> SequenceParallel<A, B>(this Seq<A> ta, Func<A, Task<B>> f) =>
            ta.Map(f).SequenceParallel();
        
        public static Task<Seq<B>> SequenceParallel<A, B>(this Seq<A> ta, Func<A, Task<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);
        
        public static Task<Set<B>> Sequence<A, B>(this Set<A> ta, Func<A, Task<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Task<Try<B>> Sequence<A, B>(this Try<A> ta, Func<A, Task<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Task<TryOption<B>> Sequence<A, B>(this TryOption<A> ta, Func<A, Task<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Task<Validation<FAIL, B>> Sequence<FAIL, A, B>(this Validation<FAIL, A> ta, Func<A, Task<B>> f) => 
            ta.Map(f).Sequence();
        
        public static Task<Validation<MonoidFail, FAIL, B>> Sequence<MonoidFail, FAIL, A, B>(this Validation<MonoidFail, FAIL, A> ta, Func<A, Task<B>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static Task<TryAsync<B>> Sequence<A, B>(this TryAsync<A> ta, Func<A, Task<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Task<OptionAsync<B>> Sequence<A, B>(this OptionAsync<A> ta, Func<A, Task<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Task<TryOptionAsync<B>> Sequence<A, B>(this TryOptionAsync<A> ta, Func<A, Task<B>> f) =>
            ta.Map(f).Sequence();
        
        public static Task<EitherAsync<L, B>> Sequence<L, A, B>(this EitherAsync<L, A> ta, Func<A, Task<B>> f) =>
            ta.Map(f).Sequence();    }
}
