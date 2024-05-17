using LanguageExt.ClassInstances;
using LanguageExt.Traits;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class Prelude
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
        x.Equals<EQ>(y);

    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="x">The left hand side of the equality operation</param>
    /// <param name="y">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool equals<EQ, L, R>(Either<L, R> x, Either<L, R> y) where EQ : Eq<R> =>
        x.Equals<EQ>(y);

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
        x.Equals<EQA, EQB>(y);

    /// <summary>
    /// Structural equality test
    /// </summary>
    /// <param name="mx">The left hand side of the equality operation</param>
    /// <param name="my">The right hand side of the equality operation</param>
    /// <returns>True if x and y are equal</returns>
    [Pure]
    public static bool equals<EQ, A>(A? mx, A? my)
        where EQ : Eq<A>
        where A : struct =>
        (mx, my) switch
        {
            (null, null) => true,
            (_, null)    => false,
            (null, _)    => false,
            var (x, y)   => EQ.Equals(x.Value, y.Value)
        };

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
        ReferenceEquals(x, y) && !ReferenceEquals(x, null) && !ReferenceEquals(y, null) && EQ.Equals((A)x, (A)y);
}
