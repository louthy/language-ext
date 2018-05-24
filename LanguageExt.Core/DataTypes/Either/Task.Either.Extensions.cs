using System;
using System.Collections.Generic;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.TypeClasses;
using LanguageExt.DataTypes.Serialisation;

public static partial class TaskEitherAsyncExtensions
{
    public static EitherAsync<L, R> ToAsync<L, R>(this Task<Either<L, R>> ma) =>
        new EitherAsync<L, R>(
            ma.Map(a => 
                a.Match(r => new EitherData<L, R>(EitherStatus.IsRight, r,default(L)), 
                        l => new EitherData<L, R>(EitherStatus.IsLeft, default(R),l),
                        () => new EitherData<L, R>(EitherStatus.IsBottom, default(R), default(L)))));

    /// <summary>
    /// Add the bound values of x and y, uses an Add type-class to provide the add
    /// operation for type A.  For example x.Add<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="NUM">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option with y added to x</returns>
    [Pure]
    public static EitherAsync<L, R> Plus<NUM, L, R>(this Task<Either<L, R>> x, Task<Either<L, R>> y) where NUM : struct, Num<R> =>
        from a in x.ToAsync()
        from b in y.ToAsync()
        select default(NUM).Plus(a, b);

    /// <summary>
    /// Find the subtract between the two bound values of x and y, uses a Subtract type-class 
    /// to provide the subtract operation for type A.  For example x.Subtract<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="NUM">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option with the subtract between x and y</returns>
    [Pure]
    public static EitherAsync<L, R> Subtract<NUM, L, R>(this Task<Either<L, R>> x, Task<Either<L, R>> y) where NUM : struct, Num<R> =>
        from a in x.ToAsync()
        from b in y.ToAsync()
        select default(NUM).Subtract(a, b);

    /// <summary>
    /// Find the product between the two bound values of x and y, uses a Product type-class 
    /// to provide the product operation for type A.  For example x.Product<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="NUM">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option with the product of x and y</returns>
    [Pure]
    public static EitherAsync<L, R> Product<NUM, L, R>(this Task<Either<L, R>> x, Task<Either<L, R>> y) where NUM : struct, Num<R> =>
        from a in x.ToAsync()
        from b in y.ToAsync()
        select default(NUM).Product(a, b);

    /// <summary>
    /// Divide the two bound values of x and y, uses a Divide type-class to provide the divide
    /// operation for type A.  For example x.Divide<TDouble,double>(y)
    /// </summary>
    /// <typeparam name="NUM">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option x / y</returns>
    [Pure]
    public static EitherAsync<L, R> Divide<NUM, L, R>(this Task<Either<L, R>> x, Task<Either<L, R>> y) where NUM : struct, Num<R> =>
        from a in x.ToAsync()
        from b in y.ToAsync()
        select default(NUM).Divide(a, b);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
    public static EitherAsync<L, B> Apply<L, A, B>(this Task<Either<L, Func<A, B>>> fab, Task<Either<L, A>> fa) =>
        default(ApplEitherAsync<L, A, B>).Apply(fab.ToAsync(), fa.ToAsync());

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
    public static EitherAsync<L, B> Apply<L, A, B>(this Func<A, B> fab, Task<Either<L, A>> fa) =>
        default(ApplEitherAsync<L, A, B>).Apply(fab, fa.ToAsync());

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative a to apply</param>
    /// <param name="fb">Applicative b to apply</param>
    /// <returns>Applicative of type FC derived from Applicative of C</returns>
    [Pure]
    public static EitherAsync<L, C> Apply<L, A, B, C>(this Task<Either<L, Func<A, B, C>>> fabc, Task<Either<L, A>> fa, Task<Either<L, B>> fb) =>
        from x in fabc.ToAsync()
        from y in default(ApplEitherAsync<L, A, B, C>).Apply(curry(x), fa.ToAsync(), fb.ToAsync())
        select y;

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative a to apply</param>
    /// <param name="fb">Applicative b to apply</param>
    /// <returns>Applicative of type FC derived from Applicative of C</returns>
    [Pure]
    public static EitherAsync<L, C> Apply<L, A, B, C>(this Func<A, B, C> fabc, Task<Either<L, A>> fa, Task<Either<L, B>> fb) =>
        default(ApplEitherAsync<L, A, B, C>).Apply(curry(fabc), fa.ToAsync(), fb.ToAsync());

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static EitherAsync<L, Func<B, C>> Apply<L, A, B, C>(this Task<Either<L, Func<A, B, C>>> fabc, Task<Either<L, A>> fa) =>
        from x in fabc.ToAsync()
        from y in default(ApplEitherAsync<L, A, B, C>).Apply(curry(x), fa.ToAsync())
        select y;

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static EitherAsync<L, Func<B, C>> Apply<L, A, B, C>(this Func<A, B, C> fabc, Task<Either<L, A>> fa) =>
        default(ApplEitherAsync<L, A, B, C>).Apply(curry(fabc), fa.ToAsync());

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static EitherAsync<L, Func<B, C>> Apply<L, A, B, C>(this Task<Either<L, Func<A, Func<B, C>>>> fabc, Task<Either<L, A>> fa) =>
        default(ApplEitherAsync<L, A, B, C>).Apply(fabc.ToAsync(), fa.ToAsync());

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static EitherAsync<L, Func<B, C>> Apply<L, A, B, C>(this Func<A, Func<B, C>> fabc, Task<Either<L, A>> fa) =>
        default(ApplEitherAsync<L, A, B, C>).Apply(fabc, fa.ToAsync());

    /// <summary>
    /// Evaluate fa, then fb, ignoring the result of fa
    /// </summary>
    /// <param name="fa">Applicative to evaluate first</param>
    /// <param name="fb">Applicative to evaluate second and then return</param>
    /// <returns>Applicative of type Option<B></returns>
    [Pure]
    public static EitherAsync<L, B> Action<L, A, B>(this Task<Either<L, A>> fa, Task<Either<L, B>> fb) =>
        default(ApplEitherAsync<L, A, B>).Action(fa.ToAsync(), fb.ToAsync());
}
