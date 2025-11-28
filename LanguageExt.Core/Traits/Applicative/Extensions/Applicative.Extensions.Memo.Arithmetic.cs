using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Numerics;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class ApplicativeExtensions
{
    /// <summary>
    /// Sum the bound values of the applicative structures provided
    /// </summary>
    /// <typeparam name="NumA">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fx">Left hand side of the operation</param>
    /// <param name="fy">Right hand side of the operation</param>
    /// <returns>An applicative structure with the arithmetic operation applied to the bound values.</returns>
    [Pure]
    public static K<F, A> Add<NumA, F, A>(this Memo<F, A> fa, Memo<F, A> fb)
        where F : Applicative<F>
        where NumA : Num<A> =>
        (fa, fb).Apply(NumA.Add);

    /// <summary>
    /// Sum the bound values of the applicative structures provided
    /// </summary>
    /// <typeparam name="NumA">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fx">Left hand side of the operation</param>
    /// <param name="fy">Right hand side of the operation</param>
    /// <returns>An applicative structure with the arithmetic operation applied to the bound values.</returns>
    [Pure]
    public static K<F, A> Add<F, A>(this Memo<F, A> fa, Memo<F, A> fb)
        where F : Applicative<F>
        where A : IAdditionOperators<A, A, A> =>
        (fa, fb).Apply((x, y) => x + y);

    /// <summary>
    /// Subtract the bound values of the applicative structures provided
    /// </summary>
    /// <typeparam name="NumA">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fx">Left hand side of the operation</param>
    /// <param name="fy">Right hand side of the operation</param>
    /// <returns>An applicative structure with the arithmetic operation applied to the bound values.</returns>
    [Pure]
    public static K<F, A> Subtract<NumA, F, A>(this Memo<F, A> fa, Memo<F, A> fb)
        where F : Applicative<F>
        where NumA : Arithmetic<A> =>
        (fa, fb).Apply(NumA.Subtract);

    /// <summary>
    /// Subtract the bound values of the applicative structures provided
    /// </summary>
    /// <typeparam name="NumA">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fx">Left hand side of the operation</param>
    /// <param name="fy">Right hand side of the operation</param>
    /// <returns>An applicative structure with the arithmetic operation applied to the bound values.</returns>
    [Pure]
    public static K<F, A> Subtract<F, A>(this Memo<F, A> fa, Memo<F, A> fb)
        where F : Applicative<F>
        where A : ISubtractionOperators<A, A, A> =>
        (fa, fb).Apply((x, y) => x - y);

    /// <summary>
    /// Multiply the bound values of the applicative structures provided
    /// </summary>
    /// <typeparam name="NumA">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fx">Left hand side of the operation</param>
    /// <param name="fy">Right hand side of the operation</param>
    /// <returns>An applicative structure with the arithmetic operation applied to the bound values.</returns>
    [Pure]
    public static K<F, A> Multiply<NumA, F, A>(this Memo<F, A> fa, Memo<F, A> fb)
        where F : Applicative<F>
        where NumA : Arithmetic<A> =>
        (fa, fb).Apply(NumA.Multiply);

    /// <summary>
    /// Multiply the bound values of the applicative structures provided
    /// </summary>
    /// <typeparam name="NumA">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fx">Left hand side of the operation</param>
    /// <param name="fy">Right hand side of the operation</param>
    /// <returns>An applicative structure with the arithmetic operation applied to the bound values.</returns>
    [Pure]
    public static K<F, A> Multiply<F, A>(this Memo<F, A> fa, Memo<F, A> fb)
        where F : Applicative<F>
        where A : IMultiplyOperators<A, A, A> =>
        (fa, fb).Apply((x, y) => x * y);

    /// <summary>
    /// Multiply the bound values of the applicative structures provided
    /// </summary>
    /// <typeparam name="NumA">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fx">Left hand side of the operation</param>
    /// <param name="fy">Right hand side of the operation</param>
    /// <returns>An applicative structure with the arithmetic operation applied to the bound values.</returns>
    [Pure]
    public static K<F, A> Divide<NumA, F, A>(this Memo<F, A> fa, Memo<F, A> fb)
        where F : Applicative<F>
        where NumA : Num<A> =>
        (fa, fb).Apply(NumA.Divide);

    /// <summary>
    /// Multiply the bound values of the applicative structures provided
    /// </summary>
    /// <typeparam name="NumA">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="fx">Left hand side of the operation</param>
    /// <param name="fy">Right hand side of the operation</param>
    /// <returns>An applicative structure with the arithmetic operation applied to the bound values.</returns>
    [Pure]
    public static K<F, A> Divide<F, A>(this Memo<F, A> fa, Memo<F, A> fb)
        where F : Applicative<F>
        where A : IDivisionOperators<A, A, A> =>
        (fa, fb).Apply((x, y) => x / y);
}
