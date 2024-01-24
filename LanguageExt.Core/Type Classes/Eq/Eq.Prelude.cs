using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class TypeClass
{
    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool equals<EQ, A>(A x, A y) where EQ : Eq<A> =>
        EQ.Equals(x, y);

    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool equals<EQ, A>(Option<A> x, Option<A> y) where EQ : Eq<A> =>
        EqOptional<EQ, MOption<A>, Option<A>, A>.Equals(x, y);

    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool equals<EQ, A>(OptionUnsafe<A> x, OptionUnsafe<A> y) where EQ : Eq<A> =>
        EqOptionalUnsafe<EQ, MOptionUnsafe<A>, OptionUnsafe<A>, A>.Equals(x, y);

    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool equals<EQ, L, R>(Either<L, R> x, Either<L, R> y) where EQ : Eq<R> =>
        EqChoice<EQ, MEither<L, R>, Either<L, R>, L, R>.Equals(x, y);

    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool equals<EQ, L, R>(EitherUnsafe<L, R> x, EitherUnsafe<L, R> y) where EQ : Eq<R> =>
        EqChoiceUnsafe<EQ, MEitherUnsafe<L, R>, EitherUnsafe<L, R>, L, R>.Equals(x, y);

    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool equals<EQA, EQB, A, B>(Either<A, B> x, Either<A, B> y)
        where EQA : Eq<A>
        where EQB : Eq<B> =>
        EqChoice<EQA, EQB, MEither<A, B>, Either<A, B>, A, B>.Equals(x, y);

    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool equals<EQA, EQB, A, B>(EitherUnsafe<A, B> x, EitherUnsafe<A, B> y)
        where EQA : Eq<A>
        where EQB : Eq<B> =>
        EqChoiceUnsafe<EQA, EQB, MEitherUnsafe<A, B>, EitherUnsafe<A, B>, A, B>.Equals(x, y);


    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool equals<EQ, A>(A? x, A? y)
        where EQ : Eq<A>
        where A : struct =>
        EqOptional<EQ, MNullable<A>, A?, A>.Equals(x, y);

    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool equals<EQ, A>(Try<A> x, Try<A> y) where EQ : Eq<A> =>
        EqTry<EQ, A>.Equals(x, y);

    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool equals<EQ, A>(TryOption<A> x, TryOption<A> y) where EQ : Eq<A> =>
        EqTryOption<EQ, A>.Equals(x, y);

    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool equals<EQ, A>(Lst<A> x, Lst<A> y) where EQ : Eq<A> =>
        EqLst<EQ, A>.Equals(x, y);

    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool equals<EQ, A>(HashSet<A> x, HashSet<A> y) where EQ : Eq<A> =>
        EqHashSet<EQ, A>.Equals(x, y);

    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool equals<EQ, A>(Que<A> x, Que<A> y) where EQ : Eq<A> =>
        EqQue<EQ, A>.Equals(x, y);

    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool equals<EQ, A>(Set<A> x, Set<A> y) where EQ : Eq<A> =>
        EqSet<EQ, A>.Equals(x, y);

    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool equals<EQ, A>(Arr<A> x, Arr<A> y) where EQ : Eq<A> =>
        EqArr<EQ, A>.Equals(x, y);

    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool equals<EQ, A>(A[] x, A[] y) where EQ : Eq<A> =>
        EqArray<EQ, A>.Equals(x, y);

    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool equals<EQ, A>(IEnumerable<A> x, IEnumerable<A> y) where EQ : Eq<A> =>
        EqEnumerable<EQ, A>.Equals(x, y);

    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool equals<EQ, A>(Seq<A> x, Seq<A> y) where EQ : Eq<A> =>
        EqSeq<EQ, A>.Equals(x, y);

    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool equals<NEWTYPE, EQ, A, PRED>(NewType<NEWTYPE, A, PRED> x, NewType<NEWTYPE, A, PRED> y) 
        where EQ      : Eq<A>
        where PRED    : Pred<A>
        where NEWTYPE : NewType<NEWTYPE, A, PRED> =>
        !ReferenceEquals(x, y) || ReferenceEquals(x, null) || ReferenceEquals(y, null) 
            ? false 
            : EQ.Equals((A)x, (A)y);
}
