using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace LanguageExt
{
    public static partial class TypeClassExtensions
    {
        /// <summary>
        /// Match operation with an untyped value for Some. This can be
        /// useful for serialisation and dealing with the IOptional interface
        /// </summary>
        /// <typeparam name="R">The return type</typeparam>
        /// <param name="Some">Operation to perform if the option is in a Some state</param>
        /// <param name="None">Operation to perform if the option is in a None state</param>
        /// <returns>The result of the match operation</returns>
        [Pure]
        public static R MatchUntyped<CHOICE, CH, A, B, R>(this CH ma, Func<object, R> Choice1, Func<object, R> Choice2, Func<R> Bottom = null)
            where CHOICE : struct, Choice<CH, A, B> =>
            default(CHOICE).Match(ma,
                Choice1: x => Choice1(x),
                Choice2: y => Choice2(y),
                Bottom: Bottom);

        /// <summary>
        /// Convert the Option to an enumerable of zero or one items
        /// </summary>
        /// <param name="ma">Option</param>
        /// <returns>An enumerable of zero or one items</returns>
        [Pure]
        public static B[] ToArray<CHOICE, CH, A, B>(this CH ma)
            where CHOICE : struct, Choice<CH, A, B> =>
            default(CHOICE).Match(ma,
                Choice1: x => new B[0],
                Choice2: y => new B[1] { y },
                Bottom: () => new B[0]);

        /// <summary>
        /// Convert the Option to an immutable list of zero or one items
        /// </summary>
        /// <param name="ma">Option</param>
        /// <returns>An immutable list of zero or one items</returns>
        [Pure]
        public static Lst<B> ToList<CHOICE, CH, A, B>(this CH ma)
            where CHOICE : struct, Choice<CH, A, B> =>
            List(ToArray<CHOICE, CH, A, B>(ma));

        /// <summary>
        /// Convert the Option to an enumerable sequence of zero or one items
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ma">Option</param>
        /// <returns>An enumerable sequence of zero or one items</returns>
        [Pure]
        public static IEnumerable<B> ToSeq<CHOICE, CH, A, B>(this CH ma)
            where CHOICE : struct, Choice<CH, A, B> =>
            ToArray<CHOICE, CH, A, B>(ma).AsEnumerable();

        /// <summary>
        /// Convert the Option to an enumerable of zero or one items
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ma">Option</param>
        /// <returns>An enumerable of zero or one items</returns>
        [Pure]
        public static IEnumerable<B> AsEnumerable<CHOICE, CH, A, B>(this CH ma)
            where CHOICE : struct, Choice<CH, A, B> =>
            ToArray<CHOICE, CH, A, B>(ma).AsEnumerable();

        /// <summary>
        /// Convert the structure to an Either
        /// </summary>
        [Pure]
        public static Either<A, B> ToEither<CHOICE, CH, A, B>(this CH ma)
            where CHOICE : struct, Choice<CH, A, B> =>
            default(CHOICE).Match(ma,
                Choice1: Left<A, B>,
                Choice2: Right<A, B>);

        /// <summary>
        /// Convert the structure to an EitherUnsafe
        /// </summary>
        [Pure]
        public static EitherUnsafe<A, B> ToEitherUnsafe<CHOICE, CH, A, B>(this CH ma)
            where CHOICE : struct, Choice<CH, A, B> =>
            default(CHOICE).Match(ma,
                Choice1: LeftUnsafe<A, B>,
                Choice2: RightUnsafe<A, B>);

        /// <summary>
        /// Convert the structure to a Option
        /// </summary>
        [Pure]
        public static Option<B> ToOption<CHOICE, CH, A, B>(this CH ma)
            where CHOICE : struct, Choice<CH, A, B> =>
            default(CHOICE).Match(ma,
                Choice1: _ => Option<B>.None,
                Choice2:      Option<B>.Some,
                Bottom: () => Option<B>.None);

        /// <summary>
        /// Convert the structure to a OptionUnsafe
        /// </summary>
        [Pure]
        public static OptionUnsafe<B> ToOptionUnsafe<CHOICE, CH, A, B>(this CH ma)
            where CHOICE : struct, Choice<CH, A, B> =>
            default(CHOICE).Match(ma,
                Choice1: _ => OptionUnsafe<B>.None,
                Choice2:      OptionUnsafe<B>.Some,
                Bottom: () => OptionUnsafe<B>.None);

        /// <summary>
        /// Convert the structure to a TryOption
        /// </summary>
        [Pure]
        public static TryOption<B> ToTryOption<CHOICE, CH, A, B>(this CH ma)
            where CHOICE : struct, Choice<CH, A, B> =>
            TryOption(() =>
                default(CHOICE).Match(ma,
                    Choice1: _ => Option<B>.None,
                    Choice2:      Option<B>.Some,
                    Bottom: () => Option<B>.None));

        public static Task<R> MatchAsync<CHOICE, CH, A, B, R>(this CH ma, Func<A, R> Choice1, Func<B, Task<R>> Choice2)
            where CHOICE : struct, Choice<CH, A, B> =>
            default(CHOICE).Match(ma,
                Choice1: a => Task.FromResult(Choice1(a)),
                Choice2: b => Choice2(b));

        public static Task<R> MatchAsync<CHOICE, CH, A, B, R>(this CH ma, Func<A, Task<R>> Choice1, Func<B, Task<R>> Choice2)
            where CHOICE : struct, Choice<CH, A, B> =>
            default(CHOICE).Match(ma, 
                Choice1: a => Choice1(a),
                Choice2: b => Choice2(b));

        [Pure]
        public static IObservable<R> MatchObservable<CHOICE, CH, A, B, R>(this CH ma, Func<A, R> Choice1, Func<B, IObservable<R>> Choice2)
            where CHOICE : struct, Choice<CH, A, B> =>
            default(CHOICE).Match(ma,
                Choice1: a => Observable.Return(Choice1(a)),
                Choice2: b => Choice2(b));

        [Pure]
        public static IObservable<R> MatchObservable<CHOICE, CH, A, B, R>(this CH ma, Func<A, IObservable<R>> Choice1, Func<B, IObservable<R>> Choice2)
            where CHOICE : struct, Choice<CH, A, B> =>
            default(CHOICE).Match(ma,
                Choice1: a => Choice1(a),
                Choice2: b => Choice2(b));

        [Pure]
        public static B IfChoice1<CHOICE, CH, A, B>(this CH ma, Func<B> Left)
            where CHOICE : struct, Choice<CH, A, B> =>
            default(CHOICE).Match(ma,
                Choice1: _ => Left(),
                Choice2: identity);

        [Pure]
        public static B IfChoice1<CHOICE, CH, A, B>(this CH ma, Func<A, B> leftMap)
            where CHOICE : struct, Choice<CH, A, B> =>
            default(CHOICE).Match(ma,
                Choice1: leftMap,
                Choice2: identity);

        [Pure]
        public static B IfChoice1<CHOICE, CH, A, B>(this CH ma, B rightValue)
            where CHOICE : struct, Choice<CH, A, B> =>
            default(CHOICE).Match(ma,
                Choice1: _ => rightValue,
                Choice2: identity);

        public static Unit IfChoice1<CHOICE, CH, A, B>(this CH ma, Action<A> Left)
            where CHOICE : struct, Choice<CH, A, B> =>
            default(CHOICE).Match(ma,
                Choice1: a => { Left(a); return unit; },
                Choice2: a => { return unit; },
                Bottom: () => { return unit; });

        public static Unit IfChoice2<CHOICE, CH, A, B>(this CH ma, Action<B> Right)
            where CHOICE : struct, Choice<CH, A, B> =>
            default(CHOICE).Match(ma,
                Choice1: a => { return unit; },
                Choice2: b => { Right(b); return unit; },
                Bottom: () => { return unit; });

        /// <summary>
        /// Returns the leftValue if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="leftValue">Value to return if in the Left state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public static A IfChoice2<CHOICE, CH, A, B>(this CH ma, A leftValue)
            where CHOICE : struct, Choice<CH, A, B> =>
            default(CHOICE).Match(ma,
                Choice1: identity,
                Choice2: _ => leftValue);

        /// <summary>
        /// Returns the result of Right() if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="Right">Function to generate a Left value if in the Right state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public static A IfChoice2<CHOICE, CH, A, B>(this CH ma, Func<A> Right)
            where CHOICE : struct, Choice<CH, A, B> =>
            default(CHOICE).Match(ma,
                Choice1: identity,
                Choice2: _ => Right());

        /// <summary>
        /// Returns the result of rightMap if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="rightMap">Function to generate a Left value if in the Right state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public static A IfChoice2<CHOICE, CH, A, B>(this CH ma, Func<B, A> rightMap)
            where CHOICE : struct, Choice<CH, A, B> =>
            default(CHOICE).Match(ma,
                Choice1: identity,
                Choice2: rightMap);

        /// <summary>
        /// Project the Either into a Lst R
        /// </summary>
        /// <returns>If the Either is in a Right state, a Lst of R with one item.  A zero length Lst R otherwise</returns>
        [Pure]
        public static Lst<B> Choice2ToList<CHOICE, CH, A, B>(this CH ma)
            where CHOICE : struct, Choice<CH, A, B> =>
            toList(Choice2AsEnumerable<CHOICE, CH, A, B>(ma));

        /// <summary>
        /// Project the Either into an ImmutableArray R
        /// </summary>
        /// <returns>If the Either is in a Right state, a ImmutableArray of R with one item.  A zero length ImmutableArray of R otherwise</returns>
        [Pure]
        public static B[] Choice2ToArray<CHOICE, CH, A, B>(this CH ma)
            where CHOICE : struct, Choice<CH, A, B> =>
            toArray(Choice2AsEnumerable<CHOICE, CH, A, B>(ma));

        /// <summary>
        /// Project the Either into a Lst R
        /// </summary>
        /// <returns>If the Either is in a Right state, a Lst of R with one item.  A zero length Lst R otherwise</returns>
        [Pure]
        public static Lst<A> Choice1ToList<CHOICE, CH, A, B>(this CH ma)
            where CHOICE : struct, Choice<CH, A, B> =>
            toList(Choice1AsEnumerable<CHOICE, CH, A, B>(ma));

        /// <summary>
        /// Project the Either into an ImmutableArray R
        /// </summary>
        /// <returns>If the Either is in a Right state, a ImmutableArray of R with one item.  A zero length ImmutableArray of R otherwise</returns>
        [Pure]
        public static A[] Choice1ToArray<CHOICE, CH, A, B>(this CH ma)
            where CHOICE : struct, Choice<CH, A, B> =>
            toArray(Choice1AsEnumerable<CHOICE, CH, A, B>(ma));

        /// <summary>
        /// Project the Either into a IEnumerable R
        /// </summary>
        /// <returns>If the Either is in a Right state, a IEnumerable of R with one item.  A zero length IEnumerable R otherwise</returns>
        [Pure]
        public static IEnumerable<B> Choice2AsEnumerable<CHOICE, CH, A, B>(this CH ma)
            where CHOICE : struct, Choice<CH, A, B> =>
            default(CHOICE).Match(ma, 
                Choice1: _ => new B[0],
                Choice2: b => new B[1] { b },
                Bottom: () => new B[0]);

        /// <summary>
        /// Project the Either into a IEnumerable L
        /// </summary>
        /// <returns>If the Either is in a Left state, a IEnumerable of L with one item.  A zero length IEnumerable L otherwise</returns>
        [Pure]
        public static IEnumerable<A> Choice1AsEnumerable<CHOICE, CH, A, B>(this CH ma)
            where CHOICE : struct, Choice<CH, A, B> =>
            default(CHOICE).Match(ma,
                Choice1: a => new A[1] { a },
                Choice2: _ => new A[0],
                Bottom: () => new A[0]);

        [Pure]
        public static int GetHashCode<CHOICE, CH, A, B>(this CH ma)
            where CHOICE : struct, Choice<CH, A, B> =>
            default(CHOICE).Match(ma,
                Choice1: a => a?.GetHashCode() ?? 0,
                Choice2: b => b?.GetHashCode() ?? 0,
                Bottom: () => -1
                );

        /// <summary>
        /// Extracts from a list of 'Either' all the 'Left' elements.
        /// All the 'Left' elements are extracted in order.
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="ma">Either list</param>
        /// <returns>An enumerable of L</returns>
        [Pure]
        public static IEnumerable<A> Choice1s<CHOICE, CH, A, B>(this IEnumerable<CH> ma)
            where CHOICE : struct, Choice<CH, A, B>
        {
            foreach (var item in ma)
            {
                if (default(CHOICE).IsChoice1(item))
                {
                    yield return default(CHOICE).Match(
                        item,
                        Choice1: x => x,
                        Choice2: y => default(A),
                        Bottom: () => default(A));
                }
            }
        }

        /// <summary>
        /// Extracts from a list of 'Either' all the 'Right' elements.
        /// All the 'Right' elements are extracted in order.
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="ma">Choice  list</param>
        /// <returns>An enumerable of L</returns>
        [Pure]
        public static IEnumerable<B> Choice2s<CHOICE, CH, A, B>(this IEnumerable<CH> ma)
            where CHOICE : struct, Choice<CH, A, B>
        {
            foreach (var item in ma)
            {
                if (default(CHOICE).IsChoice1(item))
                {
                    yield return default(CHOICE).Match(
                        item,
                        Choice1: x => default(B),
                        Choice2: y => y,
                        Bottom: () => default(B));
                }
            }
        }

        /// <summary>
        /// Partitions a list of 'Either' into two lists.
        /// All the 'Left' elements are extracted, in order, to the first
        /// component of the output.  Similarly the 'Right' elements are extracted
        /// to the second component of the output.
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="ma">Choice list</param>
        /// <returns>A tuple containing the an enumerable of L and an enumerable of R</returns>
        [Pure]
        public static Tuple<IEnumerable<A>, IEnumerable<B>> Partition<CHOICE, CH, A, B>(this IEnumerable<CH> ma)
            where CHOICE : struct, Choice<CH, A, B> =>
            Tuple(Choice1s<CHOICE, CH, A, B>(ma), Choice2s<CHOICE, CH, A, B>(ma));

    }
}
