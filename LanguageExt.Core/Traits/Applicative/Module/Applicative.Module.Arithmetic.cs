using System.Diagnostics.Contracts;
using System.Numerics;

namespace LanguageExt.Traits;

public static partial class Applicative
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
    public static K<F, A> add<NumA, F, A>(K<F, A> fa, K<F, A> fb)
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
    public static K<F, A> add<F, A>(K<F, A> fa, K<F, A> fb)
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
    public static K<F, A> subtract<NumA, F, A>(K<F, A> fa, K<F, A> fb)
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
    public static K<F, A> subtract<F, A>(K<F, A> fa, K<F, A> fb)
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
    public static K<F, A> multiply<NumA, F, A>(K<F, A> fa, K<F, A> fb)
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
    public static K<F, A> multiply<F, A>(K<F, A> fa, K<F, A> fb)
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
    public static K<F, A> divide<NumA, F, A>(K<F, A> fa, K<F, A> fb)
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
    public static K<F, A> divide<F, A>(K<F, A> fa, K<F, A> fb)
        where F : Applicative<F>
        where A : IDivisionOperators<A, A, A> =>
        (fa, fb).Apply((x, y) => x / y);
}
