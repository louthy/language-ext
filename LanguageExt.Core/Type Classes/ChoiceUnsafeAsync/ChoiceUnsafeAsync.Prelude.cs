using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static partial class TypeClass
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
        public static Task<R> matchUntypedUnsafeAsync<CHOICE, CH, A, B, R>(CH ma, Func<object, R> Left, Func<object, R> Right, Func<R> Bottom = null)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: x => Left(x),
                Right: y => Right(y),
                Bottom: Bottom);

        /// <summary>
        /// Convert the Option to an enumerable of zero or one items
        /// </summary>
        /// <param name="ma">Option</param>
        /// <returns>An enumerable of zero or one items</returns>
        [Pure]
        public static Task<Arr<B>> toArrayUnsafeAsync<CHOICE, CH, A, B>(CH ma)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: x => Arr<B>.Empty,
                Right: y => Array(y),
                Bottom: () => Arr<B>.Empty);

        /// <summary>
        /// Convert the Option to an immutable list of zero or one items
        /// </summary>
        /// <param name="ma">Option</param>
        /// <returns>An immutable list of zero or one items</returns>
        [Pure]
        public static Task<Lst<B>> toListUnsafeAsync<CHOICE, CH, A, B>(CH ma)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: x => Lst<B>.Empty,
                Right: y => List(y),
                Bottom: () => Lst<B>.Empty);

        /// <summary>
        /// Convert the Option to an enumerable sequence of zero or one items
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ma">Option</param>
        /// <returns>An enumerable sequence of zero or one items</returns>
        [Pure]
        public static Task<IEnumerable<B>> toSeqUnsafeAsync<CHOICE, CH, A, B>(CH ma)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: x => Enumerable.Empty<B>(),
                Right: y => new [] {y},
                Bottom: () => Enumerable.Empty<B>());


        [Pure]
        public static Task<R> matchUnsafeAsync<CHOICE, CH, A, B, R>(CH ma, Func<A, Task<R>> LeftAsync, Func<B, R> Right)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B> =>
            default(CHOICE).MatchUnsafeAsync(ma, LeftAsync: LeftAsync, Right: Right);

        [Pure]
        public static Task<R> matchUnsafeAsync<CHOICE, CH, A, B, R>(CH ma, Func<A, R> Left, Func<B, Task<R>> RightAsync)
            where CHOICE : struct, ChoiceAsync<CH, A, B> =>
            default(CHOICE).MatchUnsafeAsync(ma, Left: Left, RightAsync: RightAsync);

        [Pure]
        public static Task<R> matchUnsafeAsync<CHOICE, CH, A, B, R>(CH ma, Func<A, Task<R>> LeftAsync, Func<B, Task<R>> RightAsync)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B> =>
            default(CHOICE).MatchUnsafeAsync(ma, LeftAsync: LeftAsync, RightAsync: RightAsync);

        [Pure]
        public static Task<B> ifLeftUnsafeAsync<CHOICE, CH, A, B>(CH ma, Func<B> Left)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: _ => Left(),
                Right: identity);

        [Pure]
        public static Task<B> ifLeftUnsafeAsync<CHOICE, CH, A, B>(CH ma, Func<A, B> Left)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: Left,
                Right: identity);

        [Pure]
        public static Task<B> ifLeftUnsafeAsync<CHOICE, CH, A, B>(CH ma, B Right)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: _ => Right,
                Right: identity);

        public static Task<Unit> ifLeftUnsafeAsync<CHOICE, CH, A, B>(CH ma, Action<A> Left)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: a => { Left(a); return unit; },
                Right: a => { return unit; },
                Bottom: () => { return unit; });

        public static Task<Unit> ifRightUnsafeAsync<CHOICE, CH, A, B>(CH ma, Action<B> Right)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: a => { return unit; },
                Right: b => { Right(b); return unit; },
                Bottom: () => { return unit; });

        /// <summary>
        /// Returns the leftValue if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="Left">Value to return if in the Left state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public static Task<A> ifRightUnsafeAsync<CHOICE, CH, A, B>(CH ma, A Left)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: identity,
                Right: _ => Left);

        /// <summary>
        /// Returns the result of Right() if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="Right">Function to generate a Left value if in the Right state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public static Task<A> ifRightUnsafeAsync<CHOICE, CH, A, B>(CH ma, Func<A> Right)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: identity,
                Right: _ => Right());

        /// <summary>
        /// Returns the result of rightMap if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="Right">Function to generate a Left value if in the Right state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public static Task<A> ifRightUnsafeAsync<CHOICE, CH, A, B>(CH ma, Func<B, A> Right)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: identity,
                Right: Right);

        /// <summary>
        /// Project the Either into a Lst R
        /// </summary>
        /// <returns>If the Either is in a Right state, a Lst of R with one item.  A zero length Lst R otherwise</returns>
        [Pure]
        public static Task<Lst<B>> rightToListUnsafeAsync<CHOICE, CH, A, B>(CH ma)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: _ => Lst<B>.Empty,
                Right: b => List(b),
                Bottom: () => Lst<B>.Empty);

        /// <summary>
        /// Project the Either into an Arr R
        /// </summary>
        /// <returns>If the Either is in a Right state, a ImmutableArray of R with one item.  A zero length ImmutableArray of R otherwise</returns>
        [Pure]
        public static Task<Arr<B>> rightToArrayUnsafeAsync<CHOICE, CH, A, B>(CH ma)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: _ => Arr<B>.Empty,
                Right: b => Array(b),
                Bottom: () => Arr<B>.Empty);

        /// <summary>
        /// Project the Either into a Lst R
        /// </summary>
        /// <returns>If the Either is in a Right state, a Lst of R with one item.  A zero length Lst R otherwise</returns>
        [Pure]
        public static Task<Lst<A>> leftToListUnsafeAsync<CHOICE, CH, A, B>(CH ma)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: a => List(a),
                Right: _ => Lst<A>.Empty,
                Bottom: () => Lst<A>.Empty);

        /// <summary>
        /// Project the Either into an ImmutableArray R
        /// </summary>
        /// <returns>If the Either is in a Right state, a ImmutableArray of R with one item.  A zero length ImmutableArray of R otherwise</returns>
        [Pure]
        public static Task<Arr<A>> leftToArrayUnsafeAsync<CHOICE, CH, A, B>(CH ma)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: a => Array(a),
                Right: _ => Arr<A>.Empty,
                Bottom: () => Arr<A>.Empty);

        /// <summary>
        /// Project the Either into a IEnumerable R
        /// </summary>
        /// <returns>If the Either is in a Right state, a IEnumerable of R with one item.  A zero length IEnumerable R otherwise</returns>
        [Pure]
        public static Task<Seq<B>> rightAsEnumerableUnsafeAsync<CHOICE, CH, A, B>(CH ma)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma, 
                Left: _ => Seq<B>.Empty,
                Right: b => b.Cons(Seq<B>.Empty),
                Bottom: () => Seq<B>.Empty);

        /// <summary>
        /// Project the Either into a IEnumerable L
        /// </summary>
        /// <returns>If the Either is in a Left state, a IEnumerable of L with one item.  A zero length IEnumerable L otherwise</returns>
        [Pure]
        public static Task<Seq<A>> leftAsEnumerableUnsafeAsync<CHOICE, CH, A, B>(CH ma)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: a => a.Cons(Seq<A>.Empty),
                Right: _ => Seq<A>.Empty,
                Bottom: () => Seq<A>.Empty);

        [Pure]
        public static Task<int> hashCodeUnsafeAsync<CHOICE, CH, A, B>(CH ma)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: a => a?.GetHashCode() ?? 0,
                Right: b => b?.GetHashCode() ?? 0,
                Bottom: () => -1);

        /// <summary>
        /// Extracts from a list of 'Either' all the 'Left' elements.
        /// All the 'Left' elements are extracted in order.
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="ma">Either list</param>
        /// <returns>An enumerable of L</returns>
        [Pure]
        public static async Task<IEnumerable<A>> leftsUnsafeAsync<CHOICE, CH, A, B>(IEnumerable<CH> ma)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B>
        {
            var res = await Task.WhenAll(ma.Map(item =>
                default(CHOICE).MatchUnsafe(
                    item,
                    Left: x => (true, x),
                    Right: y => (false, default(A)),
                    Bottom: () => (false, default(A))))).ConfigureAwait(false);

            return res.Filter(x => x.Item1).Map(x => x.Item2);
        }

        /// <summary>
        /// Extracts from a list of 'Either' all the 'Left' elements.
        /// All the 'Left' elements are extracted in order.
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="ma">Either list</param>
        /// <returns>An enumerable of L</returns>
        [Pure]
        public static async Task<Seq<A>> leftsUnsafeAsync<CHOICE, CH, A, B>(Seq<CH> ma)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B> =>
            Prelude.toSeq(await leftsUnsafeAsync<CHOICE, CH, A, B>(ma.AsEnumerable()).ConfigureAwait(false));

        /// <summary>
        /// Extracts from a list of 'Either' all the 'Right' elements.
        /// All the 'Right' elements are extracted in order.
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="ma">Choice  list</param>
        /// <returns>An enumerable of L</returns>
        [Pure]
        public static async Task<IEnumerable<B>> rightsUnsafeAsync<CHOICE, CH, A, B>(IEnumerable<CH> ma)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B>
        {
            var res = await Task.WhenAll(ma.Map(item =>
                default(CHOICE).MatchUnsafe(
                    item,
                    Left: x => (false, default(B)),
                    Right: y => (true, y),
                    Bottom: () => (false, default(B))))).ConfigureAwait(false);

            return res.Filter(x => x.Item1).Map(x => x.Item2);
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
        public static async Task<Seq<B>> rightsUnsafeAsync<CHOICE, CH, A, B>(Seq<CH> ma)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B> =>
            Prelude.toSeq(await rightsUnsafeAsync<CHOICE, CH, A, B>(ma.AsEnumerable()).ConfigureAwait(false));

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
        public static async Task<(IEnumerable<A> Lefts, IEnumerable<B> Rights)> partitionUnsafeAsync<CHOICE, CH, A, B>(IEnumerable<CH> ma)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B>
        {
            var res = await Task.WhenAll(ma.Map(item =>
                default(CHOICE).MatchUnsafe(
                    item,
                    Left: x => (1, x, default(B)),
                    Right: y => (2, default(A), y),
                    Bottom: () => (0, default(A), default(B))))).ConfigureAwait(false);

            return (res.Filter(x => x.Item1 == 1).Map(x => x.Item2), res.Filter(x => x.Item1 == 2).Map(x => x.Item3));
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
        public static Task<(Seq<A> Lefts, Seq<B> Rights)> partitionUnsafeAsync<CHOICE, CH, A, B>(Seq<CH> ma)
            where CHOICE : struct, ChoiceUnsafeAsync<CH, A, B> =>
            partitionUnsafeAsync<CHOICE, CH, A, B>(ma.AsEnumerable())
                .Map(pair => (Prelude.toSeq(pair.Lefts), Prelude.toSeq(pair.Rights)));
    }
}
