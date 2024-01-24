#nullable enable
using System;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class OptionalUnsafe
{
    static readonly Action noneIgnoreUnsafe = () => { };
    static readonly Func<Unit> noneIgnoreUnsafeF = () => unit;

    /// <summary>
    /// Invokes the f action if Option is in the Some state, otherwise nothing happens.
    /// </summary>
    public static Unit ifSomeUnsafe<OPT, OA, A>(OA opt, Action<A?> f)
        where OPT : OptionalUnsafe<OA, A> =>
        OPT.Match(opt, f, noneIgnoreUnsafe);

    /// <summary>
    /// Invokes the f function if Option is in the Some state, otherwise nothing
    /// happens.
    /// </summary>
    public static Unit ifSomeUnsafe<OPT, OA, A>(OA opt, Func<A?, Unit> f)
        where OPT : OptionalUnsafe<OA, A> =>
        OPT.MatchUnsafe(opt, f, noneIgnoreUnsafeF);

    /// <summary>
    /// Returns the result of invoking the None() operation if the optional 
    /// is in a None state, otherwise the bound Some(x) value is returned.
    /// </summary>
    /// <remarks>Will not accept a null return value from the None operation</remarks>
    /// <param name="None">Operation to invoke if the structure is in a None state</param>
    /// <returns>Tesult of invoking the None() operation if the optional 
    /// is in a None state, otherwise the bound Some(x) value is returned.</returns>
    [Pure]
    public static A? ifNoneUnsafe<OPT, OA, A>(OA opt, Func<A?> None)
        where OPT : OptionalUnsafe<OA, A> =>
        OPT.MatchUnsafe(opt, identity, None);

    /// <summary>
    /// Returns the noneValue if the optional is in a None state, otherwise
    /// the bound Some(x) value is returned.
    /// </summary>
    /// <remarks>Will not accept a null noneValue</remarks>
    /// <param name="noneValue">Value to return if in a None state</param>
    /// <returns>noneValue if the optional is in a None state, otherwise
    /// the bound Some(x) value is returned</returns>
    [Pure]
    public static A? ifNoneUnsafe<OPT, OA, A>(OA opt, A? noneValue)
        where OPT : OptionalUnsafe<OA, A> =>
        OPT.MatchUnsafe(opt, identity, () => noneValue);

    /// <summary>
    /// Match operation with an untyped value for Some. This can be
    /// useful for serialisation and dealing with the IOptional interface
    /// </summary>
    /// <typeparam name="R">The return type</typeparam>
    /// <param name="Some">Operation to perform if the option is in a Some state</param>
    /// <param name="None">Operation to perform if the option is in a None state</param>
    /// <returns>The result of the match operation</returns>
    [Pure]
    public static R? matchUntypedUnsafe<OPT, OA, A, R>(OA ma, Func<object?, R?> Some, Func<R?> None)
        where OPT : OptionalUnsafe<OA, A> =>
        OPT.MatchUnsafe(ma,
                        Some: x => Some(x),
                        None: None);

    /// <summary>
    /// Match operation with an untyped value for Some. This can be
    /// useful for serialisation and dealing with the IOptional interface
    /// </summary>
    /// <typeparam name="R">The return type</typeparam>
    /// <param name="Some">Operation to perform if the option is in a Some state</param>
    /// <param name="None">Operation to perform if the option is in a None state</param>
    /// <returns>The result of the match operation</returns>
    [Pure]
    public static R? matchUntyped<OPT, OA, A, R>(OA ma, Func<object?, R?> Some, Func<R?> None)
        where OPT : Optional<OA, A> =>
        OPT.Match(ma,
                  Some: x => Some(x),
                  None: None);

    /// <summary>
    /// Convert the Option to an enumerable of zero or one items
    /// </summary>
    /// <param name="ma">Option</param>
    /// <returns>An enumerable of zero or one items</returns>
    [Pure]
    public static Arr<A?> toArray<OPT, OA, A>(OA ma)
        where OPT : OptionalUnsafe<OA, A> =>
        OPT.MatchUnsafe(ma,
                        Some: x => [x],
                        None: System.Array.Empty<A?>);

    /// <summary>
    /// Convert the Option to an immutable list of zero or one items
    /// </summary>
    /// <param name="ma">Option</param>
    /// <returns>An immutable list of zero or one items</returns>
    [Pure]
    public static Lst<A?> toList<OPT, OA, A>(OA ma)
        where OPT : OptionalUnsafe<OA, A> =>
        toList<A?>(toArray<OPT, OA, A>(ma));

    /// <summary>
    /// Convert the Option to an enumerable of zero or one items
    /// </summary>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="ma">Option</param>
    /// <returns>An enumerable of zero or one items</returns>
    [Pure]
    public static Seq<A?> asEnumerable<OPT, OA, A>(OA ma)
        where OPT : OptionalUnsafe<OA, A> =>
        toSeq(toArray<OPT, OA, A>(ma));

    /// <summary>
    /// Convert the structure to an EitherUnsafe
    /// </summary>
    [Pure]
    public static EitherUnsafe<L, A> toEitherUnsafe<OPT, OA, L, A>(OA ma, L? defaultLeftValue)
        where OPT : OptionalUnsafe<OA, A> =>
        OPT.MatchUnsafe(ma,
                        Some: RightUnsafe<L, A>,
                        None: () => LeftUnsafe<L, A>(defaultLeftValue));

    /// <summary>
    /// Convert the structure to an EitherUnsafe
    /// </summary>
    [Pure]
    public static EitherUnsafe<L, A> toEitherUnsafe<OPT, OA, L, A>(OA ma, Func<L?> Left)
        where OPT : OptionalUnsafe<OA, A> =>
        OPT.MatchUnsafe(ma,
                        Some: RightUnsafe<L, A>,
                        None: () => LeftUnsafe<L, A>(Left()));

    /// <summary>
    /// Convert the structure to a OptionUnsafe
    /// </summary>
    [Pure]
    public static OptionUnsafe<A> toOptionUnsafe<OPT, OA, A>(OA ma)
        where OPT : OptionalUnsafe<OA, A> =>
        OPT.MatchUnsafe(ma,
                        Some: SomeUnsafe,
                        None: () => OptionUnsafe<A>.None);
}
