using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static partial class ChoiceUnsafe
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
        public static R matchUntypedUnsafe<CHOICE, CH, A, B, R>(CH ma, Func<object, R> Left, Func<object, R> Right, Func<R> Bottom = null)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
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
        public static Arr<B> toArray<CHOICE, CH, A, B>(CH ma)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: x => System.Array.Empty<B>(),
                Right: y => new B[1] { y },
                Bottom: () => System.Array.Empty<B>());

        /// <summary>
        /// Convert the Option to an immutable list of zero or one items
        /// </summary>
        /// <param name="ma">Option</param>
        /// <returns>An immutable list of zero or one items</returns>
        [Pure]
        public static Lst<B> toList<CHOICE, CH, A, B>(CH ma)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
            toList<B>(toArray<CHOICE, CH, A, B>(ma));

        /// <summary>
        /// Convert the Option to an enumerable sequence of zero or one items
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ma">Option</param>
        /// <returns>An enumerable sequence of zero or one items</returns>
        [Pure]
        public static IEnumerable<B> toSeq<CHOICE, CH, A, B>(CH ma)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
            toArray<CHOICE, CH, A, B>(ma).AsEnumerable();

        /// <summary>
        /// Convert the structure to an EitherUnsafe
        /// </summary>
        [Pure]
        public static EitherUnsafe<A, B> toEitherUnsafe<CHOICE, CH, A, B>(CH ma)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: LeftUnsafe<A, B>,
                Right: RightUnsafe<A, B>);

        /// <summary>
        /// Convert the structure to a OptionUnsafe
        /// </summary>
        [Pure]
        public static OptionUnsafe<B> toOptionUnsafe<CHOICE, CH, A, B>(CH ma)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: _ => OptionUnsafe<B>.None,
                Right:      OptionUnsafe<B>.Some,
                Bottom: () => OptionUnsafe<B>.None);

        [Pure]
        public static B ifLeftUnsafe<CHOICE, CH, A, B>(CH ma, Func<B> Left)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: _ => Left(),
                Right: identity);

        [Pure]
        public static B ifLeftUnsafe<CHOICE, CH, A, B>(CH ma, Func<A, B> leftMap)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: leftMap,
                Right: identity);

        [Pure]
        public static B ifLeftUnsafe<CHOICE, CH, A, B>(CH ma, B rightValue)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: _ => rightValue,
                Right: identity);

        public static Unit ifLeftUnsafe<CHOICE, CH, A, B>(CH ma, Action<A> Left)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: a => { Left(a); return unit; },
                Right: a => { return unit; },
                Bottom: () => { return unit; });

        public static Unit ifRightUnsafe<CHOICE, CH, A, B>(CH ma, Action<B> Right)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: a => { return unit; },
                Right: b => { Right(b); return unit; },
                Bottom: () => { return unit; });

        /// <summary>
        /// Returns the leftValue if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="leftValue">Value to return if in the Left state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public static A ifRightUnsafe<CHOICE, CH, A, B>(CH ma, A leftValue)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: identity,
                Right: _ => leftValue);

        /// <summary>
        /// Returns the result of Right() if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="Right">Function to generate a Left value if in the Right state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public static A ifRightUnsafe<CHOICE, CH, A, B>(CH ma, Func<A> Right)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: identity,
                Right: _ => Right());

        /// <summary>
        /// Returns the result of rightMap if the Either is in a Right state.
        /// Returns the Left value if the Either is in a Left state.
        /// </summary>
        /// <param name="rightMap">Function to generate a Left value if in the Right state</param>
        /// <returns>Returns an unwrapped Left value</returns>
        [Pure]
        public static A ifRightUnsafe<CHOICE, CH, A, B>(CH ma, Func<B, A> rightMap)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: identity,
                Right: rightMap);

        /// <summary>
        /// Project the Either into a Lst R
        /// </summary>
        /// <returns>If the Either is in a Right state, a Lst of R with one item.  A zero length Lst R otherwise</returns>
        [Pure]
        public static Lst<B> rightToList<CHOICE, CH, A, B>(CH ma)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
            rightAsEnumerable<CHOICE, CH, A, B>(ma).Freeze();

        /// <summary>
        /// Project the Either into an ImmutableArray R
        /// </summary>
        /// <returns>If the Either is in a Right state, a ImmutableArray of R with one item.  A zero length ImmutableArray of R otherwise</returns>
        [Pure]
        public static Arr<B> rightToArray<CHOICE, CH, A, B>(CH ma)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
            toArray<B>(rightAsEnumerable<CHOICE, CH, A, B>(ma));

        /// <summary>
        /// Project the Either into a Lst R
        /// </summary>
        /// <returns>If the Either is in a Right state, a Lst of R with one item.  A zero length Lst R otherwise</returns>
        [Pure]
        public static Lst<A> leftToList<CHOICE, CH, A, B>(CH ma)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
            leftAsEnumerable<CHOICE, CH, A, B>(ma).Freeze();

        /// <summary>
        /// Project the Either into an ImmutableArray R
        /// </summary>
        /// <returns>If the Either is in a Right state, a ImmutableArray of R with one item.  A zero length ImmutableArray of R otherwise</returns>
        [Pure]
        public static Arr<A> leftToArray<CHOICE, CH, A, B>(CH ma)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
            toArray<A>(leftAsEnumerable<CHOICE, CH, A, B>(ma));

        /// <summary>
        /// Project the Either into a IEnumerable R
        /// </summary>
        /// <returns>If the Either is in a Right state, a IEnumerable of R with one item.  A zero length IEnumerable R otherwise</returns>
        [Pure]
        public static Seq<B> rightAsEnumerable<CHOICE, CH, A, B>(CH ma)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma, 
                Left: _ => Empty,
                Right: b => b.Cons(Empty),
                Bottom: () => Empty);

        /// <summary>
        /// Project the Either into a IEnumerable L
        /// </summary>
        /// <returns>If the Either is in a Left state, a IEnumerable of L with one item.  A zero length IEnumerable L otherwise</returns>
        [Pure]
        public static Seq<A> leftAsEnumerable<CHOICE, CH, A, B>(CH ma)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: a => a.Cons(Empty),
                Right: _ => Empty,
                Bottom: () => Empty);

        [Pure]
        public static int hashCode<CHOICE, CH, A, B>(CH ma)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
            default(CHOICE).MatchUnsafe(ma,
                Left: a => a?.GetHashCode() ?? 0,
                Right: b => b?.GetHashCode() ?? 0,
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
        public static IEnumerable<A> lefts<CHOICE, CH, A, B>(IEnumerable<CH> ma)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B>
        {
            foreach (var item in ma)
            {
                if (default(CHOICE).IsLeft(item))
                {
                    yield return default(CHOICE).MatchUnsafe(
                        item,
                        Left: x => x,
                        Right: y => default(A),
                        Bottom: () => default(A));
                }
            }
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
        public static Seq<A> lefts<CHOICE, CH, A, B>(Seq<CH> ma)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
            Prelude.toSeq(lefts<CHOICE, CH, A, B>(ma.AsEnumerable()));

        /// <summary>
        /// Extracts from a list of 'Either' all the 'Right' elements.
        /// All the 'Right' elements are extracted in order.
        /// </summary>
        /// <typeparam name="L">Left</typeparam>
        /// <typeparam name="R">Right</typeparam>
        /// <param name="ma">Choice  list</param>
        /// <returns>An enumerable of L</returns>
        [Pure]
        public static IEnumerable<B> rights<CHOICE, CH, A, B>(IEnumerable<CH> ma)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B>
        {
            foreach (var item in ma)
            {
                if (default(CHOICE).IsRight(item))
                {
                    yield return default(CHOICE).MatchUnsafe(
                        item,
                        Left: x => default(B),
                        Right: y => y,
                        Bottom: () => default(B));
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
        public static Seq<B> rights<CHOICE, CH, A, B>(Seq<CH> ma)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
            Prelude.toSeq(rights<CHOICE, CH, A, B>(ma.AsEnumerable()));

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
        public static (IEnumerable<A> Lefts, IEnumerable<B> Rights) partition<CHOICE, CH, A, B>(IEnumerable<CH> ma)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
            (lefts<CHOICE, CH, A, B>(ma), rights<CHOICE, CH, A, B>(ma));

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
        public static (Seq<A> Lefts, Seq<B> Rights) partition<CHOICE, CH, A, B>(Seq<CH> ma)
            where CHOICE : struct, ChoiceUnsafe<CH, A, B> =>
            (lefts<CHOICE, CH, A, B>(ma), rights<CHOICE, CH, A, B>(ma));
    }
}
