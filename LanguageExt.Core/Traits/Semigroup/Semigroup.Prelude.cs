using LanguageExt.Traits;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// An associative binary operation
    /// </summary>
    /// <param name="x">The left hand side of the operation</param>
    /// <param name="y">The right hand side of the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static A combine<A>(A x, A y) where A : Semigroup<A> =>
        x + y;

    /// <summary>
    /// An associative binary operation
    /// </summary>
    /// <param name="lhs">Left-hand side of the operation</param>
    /// <param name="rhs">Right-hand side of the operation</param>
    /// <returns>lhs + rhs</returns>
    [Pure]
    public static Either<L, R> combine<L, R>(Either<L, R> lhs, Either<L, R> rhs) 
        where R : Semigroup<R> =>
        from x in lhs
        from y in rhs
        select x + y;

    /// An associative binary operation
    /// </summary>
    /// <param name="x">The left hand side of the operation</param>
    /// <param name="y">The right hand side of the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static NEWTYPE combine<NEWTYPE, A>(NewType<NEWTYPE, A> x, NewType<NEWTYPE, A> y)
        where NEWTYPE : NewType<NEWTYPE, A>
        where A : Semigroup<A> =>
        from a in x
        from b in y
        select a + b;

    /// <summary>
    /// An associative binary operation
    /// </summary>
    /// <param name="x">The left hand side of the operation</param>
    /// <param name="y">The right hand side of the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static NUMTYPE combine<NUMTYPE, NUM, A>(NumType<NUMTYPE, NUM, A> x, NumType<NUMTYPE, NUM, A> y)
        where NUMTYPE : NumType<NUMTYPE, NUM, A>
        where NUM : Num<A>
        where A : Semigroup<A> =>
        from a in x
        from b in y
        select a + b;

    /// <summary>
    /// An associative binary operation
    /// </summary>
    /// <param name="x">The left hand side of the operation</param>
    /// <param name="y">The right hand side of the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static NEWTYPE combine<NEWTYPE, A, PRED>(NewType<NEWTYPE, A, PRED> x, NewType<NEWTYPE, A, PRED> y)
        where NEWTYPE : NewType<NEWTYPE, A, PRED>
        where PRED : Pred<A>
        where A : Semigroup<A> =>
        from a in x
        from b in y
        select a + b;

    /// <summary>
    /// An associative binary operation
    /// </summary>
    /// <param name="x">The left hand side of the operation</param>
    /// <param name="y">The right hand side of the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static NUMTYPE combine<NUMTYPE, NUM, A, PRED>(NumType<NUMTYPE, NUM, A, PRED> x, NumType<NUMTYPE, NUM, A, PRED> y)
        where NUMTYPE : NumType<NUMTYPE, NUM, A, PRED>
        where PRED    : Pred<A>
        where NUM     : Num<A> 
        where A       : Semigroup<A> =>
        from a in x
        from b in y
        select a + b;

    /// <summary>
    /// An associative binary operation
    /// </summary>
    /// <param name="x">The left hand side of the operation</param>
    /// <param name="y">The right hand side of the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static Option<A> combine<A>(Option<A> x, Option<A> y) 
        where A : Semigroup<A> =>
        from a in x
        from b in y
        select a + b;

    /// <summary>
    /// An associative binary operation
    /// </summary>
    /// <param name="x">The left hand side of the operation</param>
    /// <param name="y">The right hand side of the operation</param>
    /// <returns>The result of the operation</returns>
    [Pure]
    public static IEnumerable<A> combine<A>(IEnumerable<A> x, IEnumerable<A> y) 
        where A : Semigroup<A>
    {
        foreach (var a in x)
        foreach (var b in y)
            yield return a + b;
    }
}
