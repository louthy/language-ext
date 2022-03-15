using System;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class Optional
    {
        internal static readonly Action noneIgnore = () => { };
        internal static readonly Func<Unit> noneIgnoreF = () => unit;

        /// <summary>
        /// Invokes the f action if Option is in the Some state, otherwise nothing happens.
        /// </summary>
        public static Unit ifSome<OPT, OA, A>(OA opt, Action<A> f)
            where OPT : struct, Optional<OA, A> =>
            default(OPT).Match(opt, f, noneIgnore);

        /// <summary>
        /// Invokes the f function if Option is in the Some state, otherwise nothing
        /// happens.
        /// </summary>
        public static Unit ifSome<OPT, OA, A>(OA opt, Func<A, Unit> f)
            where OPT : struct, Optional<OA, A> =>
            default(OPT).Match(opt, f, noneIgnoreF);

        /// <summary>
        /// Returns the result of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will not accept a null return value from the None operation</remarks>
        /// <param name="None">Operation to invoke if the structure is in a None state</param>
        /// <returns>Tesult of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.</returns>
        [Pure]
        public static A ifNone<OPT, OA, A>(OA opt, Func<A> None)
            where OPT : struct, Optional<OA, A> =>
            default(OPT).Match(opt, identity, None);

        /// <summary>
        /// Returns the noneValue if the optional is in a None state, otherwise
        /// the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will not accept a null noneValue</remarks>
        /// <param name="noneValue">Value to return if in a None state</param>
        /// <returns>noneValue if the optional is in a None state, otherwise
        /// the bound Some(x) value is returned</returns>
        [Pure]
        public static A ifNone<OPT, OA, A>(OA opt, A noneValue)
            where OPT : struct, Optional<OA, A> =>
            default(OPT).Match(opt, identity, () => noneValue);

        /// <summary>
        /// Match operation with an untyped value for Some. This can be
        /// useful for serialisation and dealing with the IOptional interface
        /// </summary>
        /// <typeparam name="R">The return type</typeparam>
        /// <param name="Some">Operation to perform if the option is in a Some state</param>
        /// <param name="None">Operation to perform if the option is in a None state</param>
        /// <returns>The result of the match operation</returns>
        [Pure]
        public static R matchUntyped<OPT, OA, A, R>(OA ma, Func<object, R> Some, Func<R> None)
            where OPT : struct, Optional<OA, A> =>
            default(OPT).Match(ma,
                Some: x => Some(x),
                None: () => None());

        /// <summary>
        /// Match operation with an untyped value for Some. This can be
        /// useful for serialisation and dealing with the IOptional interface
        /// </summary>
        /// <typeparam name="R">The return type</typeparam>
        /// <param name="Some">Operation to perform if the option is in a Some state</param>
        /// <param name="None">Operation to perform if the option is in a None state</param>
        /// <returns>The result of the match operation</returns>
        [Pure]
        public static R matchUntypedUnsafe<OPT, OA, A, R>(OA ma, Func<object, R> Some, Func<R> None)
            where OPT : struct, OptionalUnsafe<OA, A> =>
            default(OPT).MatchUnsafe(ma,
                Some: x => Some(x),
                None: () => None());

        /// <summary>
        /// Convert the Option to an enumerable of zero or one items
        /// </summary>
        /// <param name="ma">Option</param>
        /// <returns>An enumerable of zero or one items</returns>
        [Pure]
        public static Arr<A> toArray<OPT, OA, A>(OA ma)
            where OPT : struct, Optional<OA, A> =>
            default(OPT).Match(ma,
                Some: x => new A[1] { x },
                None: () => System.Array.Empty<A>());

        /// <summary>
        /// Convert the Option to an immutable list of zero or one items
        /// </summary>
        /// <param name="ma">Option</param>
        /// <returns>An immutable list of zero or one items</returns>
        [Pure]
        public static Lst<A> toList<OPT, OA, A>(OA ma)
            where OPT : struct, Optional<OA, A> =>
            toList<A>(toArray<OPT, OA, A>(ma));

        /// <summary>
        /// Convert the Option to an enumerable of zero or one items
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ma">Option</param>
        /// <returns>An enumerable of zero or one items</returns>
        [Pure]
        public static Seq<A> asEnumerable<OPT, OA, A>(OA ma)
            where OPT : struct, Optional<OA, A> =>
            toSeq(toArray<OPT, OA, A>(ma));

        /// <summary>
        /// Convert the structure to an Either
        /// </summary>
        [Pure]
        public static Either<L, A> toEither<OPT, OA, L, A>(OA ma, L defaultLeftValue)
            where OPT : struct, Optional<OA, A> =>
            default(OPT).Match(ma,
                Some: Right<L, A>,
                None: () => Left<L, A>(defaultLeftValue));

        /// <summary>
        /// Convert the structure to an Either
        /// </summary>
        [Pure]
        public static Either<L, A> toEither<OPT, OA, L, A>(OA ma, Func<L> Left)
            where OPT : struct, Optional<OA, A> =>
            default(OPT).Match(ma,
                Some: x => Right<L, A>(x),
                None: () => Left<L, A>(Left()));

        /// <summary>
        /// Convert the structure to an EitherUnsafe
        /// </summary>
        [Pure]
        public static EitherUnsafe<L, A> toEitherUnsafe<OPT, OA, L, A>(OA ma, L defaultLeftValue)
            where OPT : struct, Optional<OA, A> =>
            default(OPT).Match(ma,
                Some: x => RightUnsafe<L, A>(x),
                None: () => LeftUnsafe<L, A>(defaultLeftValue));

        /// <summary>
        /// Convert the structure to an EitherUnsafe
        /// </summary>
        [Pure]
        public static EitherUnsafe<L, A> toEitherUnsafe<OPT, OA, L, A>(OA ma, Func<L> Left)
            where OPT : struct, Optional<OA, A> =>
            default(OPT).Match(ma,
                Some: x => RightUnsafe<L, A>(x),
                None: () => LeftUnsafe<L, A>(Left()));

        /// <summary>
        /// Convert the structure to a Option
        /// </summary>
        [Pure]
        public static Option<A> toOption<OPT, OA, A>(OA ma)
            where OPT : struct, Optional<OA, A> =>
            default(OPT).Match(ma,
                Some: x => Optional(x),
                None: () => Option<A>.None);

        /// <summary>
        /// Convert the structure to a OptionUnsafe
        /// </summary>
        [Pure]
        public static OptionUnsafe<A> toOptionUnsafe<OPT, OA, A>(OA ma)
            where OPT : struct, Optional<OA, A> =>
            default(OPT).Match(ma,
                Some: x => SomeUnsafe(x),
                None: () => OptionUnsafe<A>.None);

        /// <summary>
        /// Convert the structure to a TryOption
        /// </summary>
        [Pure]
        public static TryOption<A> toTryOption<OPT, OA, A>(OA ma)
            where OPT : struct, Optional<OA, A> => () =>
                default(OPT).Match(ma,
                    Some: x => Option<A>.Some(x),
                    None: () => Option<A>.None);
    }
}
