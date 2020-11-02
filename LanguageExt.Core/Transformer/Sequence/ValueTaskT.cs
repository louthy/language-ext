using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    public partial class ValueTaskT
    {
        public static ValueTask<Arr<B>> Sequence<A, B>(this Arr<A> ta, Func<A, ValueTask<B>> f) =>
            ta.Map(f).Sequence();
        
        public static ValueTask<Either<L, B>> Sequence<L, A, B>(this Either<L, A> ta, Func<A, ValueTask<B>> f) =>
            ta.Map(f).Sequence();
        
        public static ValueTask<EitherUnsafe<L, B>> Sequence<L, A, B>(this EitherUnsafe<L, A> ta, Func<A, ValueTask<B>> f) =>
            ta.Map(f).Sequence();
        
        public static ValueTask<Identity<B>> Sequence<A, B>(this Identity<A> ta, Func<A, ValueTask<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);

        public static ValueTask<IEnumerable<B>> SequenceSerial<A, B>(this IEnumerable<A> ta, Func<A, ValueTask<B>> f) =>
            ta.Map(f).SequenceSerial();

        public static ValueTask<IEnumerable<B>> SequenceParallel<A, B>(this IEnumerable<A> ta, Func<A, ValueTask<B>> f) =>
            ta.Map(f).SequenceParallel();

        public static ValueTask<IEnumerable<B>> SequenceParallel<A, B>(this IEnumerable<A> ta, Func<A, ValueTask<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);
 
        public static ValueTask<ValueTask<B>> Sequence<A, B>(this ValueTask<A> ta, Func<A, ValueTask<B>> f) =>
            ta.Map(f).Sequence();
        
        public static ValueTask<Fin<B>> Sequence<A, B>(this Fin<A> ta, Func<A, ValueTask<B>> f) =>
            ta.Map(f).Sequence();
        
        public static ValueTask<Option<B>> Sequence<A, B>(this Option<A> ta, Func<A, ValueTask<B>> f) =>
            ta.Map(f).Sequence();
        
        public static ValueTask<OptionUnsafe<B>> Sequence<A, B>(this OptionUnsafe<A> ta, Func<A, ValueTask<B>> f) =>
            ta.Map(f).Sequence();
        
        public static ValueTask<Seq<B>> SequenceSerial<A, B>(this Seq<A> ta, Func<A, ValueTask<B>> f) =>
            ta.Map(f).SequenceSerial();
        
        public static ValueTask<Seq<B>> SequenceParallel<A, B>(this Seq<A> ta, Func<A, ValueTask<B>> f) =>
            ta.Map(f).SequenceParallel();
        
        public static ValueTask<Seq<B>> SequenceParallel<A, B>(this Seq<A> ta, Func<A, ValueTask<B>> f, int windowSize) =>
            ta.Map(f).SequenceParallel(windowSize);
        
        public static ValueTask<Set<B>> Sequence<A, B>(this Set<A> ta, Func<A, ValueTask<B>> f) =>
            ta.Map(f).Sequence();
        
        public static ValueTask<Try<B>> Sequence<A, B>(this Try<A> ta, Func<A, ValueTask<B>> f) =>
            ta.Map(f).Sequence();
        
        public static ValueTask<TryOption<B>> Sequence<A, B>(this TryOption<A> ta, Func<A, ValueTask<B>> f) =>
            ta.Map(f).Sequence();
        
        public static ValueTask<Validation<FAIL, B>> Sequence<FAIL, A, B>(this Validation<FAIL, A> ta, Func<A, ValueTask<B>> f) => 
            ta.Map(f).Sequence();
        
        public static ValueTask<Validation<MonoidFail, FAIL, B>> Sequence<MonoidFail, FAIL, A, B>(this Validation<MonoidFail, FAIL, A> ta, Func<A, ValueTask<B>> f)
            where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL> =>
            ta.Map(f).Traverse(Prelude.identity);
        
        public static ValueTask<TryAsync<B>> Sequence<A, B>(this TryAsync<A> ta, Func<A, ValueTask<B>> f) =>
            ta.Map(f).Sequence();
        
        public static ValueTask<OptionAsync<B>> Sequence<A, B>(this OptionAsync<A> ta, Func<A, ValueTask<B>> f) =>
            ta.Map(f).Sequence();
        
        public static ValueTask<TryOptionAsync<B>> Sequence<A, B>(this TryOptionAsync<A> ta, Func<A, ValueTask<B>> f) =>
            ta.Map(f).Sequence();
        
        public static ValueTask<EitherAsync<L, B>> Sequence<L, A, B>(this EitherAsync<L, A> ta, Func<A, ValueTask<B>> f) =>
            ta.Map(f).Sequence();    

        public static ValueTask<Aff<B>> Sequence<A, B>(this Aff<A> ta, Func<A, ValueTask<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);

        public static ValueTask<Eff<B>> Sequence<A, B>(this Eff<A> ta, Func<A, ValueTask<B>> f) =>
            ta.Map(f).Traverse(Prelude.identity);
    }
}
