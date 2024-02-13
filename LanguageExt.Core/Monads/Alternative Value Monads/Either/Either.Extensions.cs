using System;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.TypeClasses;
using LanguageExt.Common;
using LanguageExt.HKT;

namespace LanguageExt;

/// <summary>
/// Extension methods for Either
/// </summary>
public static class EitherExtensions
{
    public static Either<L, R> As<L, R>(this K<Either<L>, R> ma) =>
        (Either<L, R>)ma;
 
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Either<L, R> Flatten<L, R>(this Either<L, Either<L, R>> ma) =>
        ma.Bind(x => x);

    /// <summary>
    /// Add the bound values of x and y, uses an Add trait to provide the add
    /// operation for type A.  For example x.Add<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="NUM">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option with y added to x</returns>
    [Pure]
    public static Either<L, R> Plus<NUM, L, R>(this Either<L, R> x, Either<L, R> y) where NUM : Num<R> =>
        from a in x
        from b in y
        select NUM.Plus(a, b);

    /// <summary>
    /// Find the subtract between the two bound values of x and y, uses a Subtract trait 
    /// to provide the subtract operation for type A.  For example x.Subtract<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="NUM">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option with the subtract between x and y</returns>
    [Pure]
    public static Either<L, R> Subtract<NUM, L, R>(this Either<L, R> x, Either<L, R> y) where NUM : Num<R> =>
        from a in x
        from b in y
        select NUM.Subtract(a, b);

    /// <summary>
    /// Find the product between the two bound values of x and y, uses a Product trait 
    /// to provide the product operation for type A.  For example x.Product<TInteger,int>(y)
    /// </summary>
    /// <typeparam name="NUM">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option with the product of x and y</returns>
    [Pure]
    public static Either<L, R> Product<NUM, L, R>(this Either<L, R> x, Either<L, R> y) where NUM : Num<R> =>
        from a in x
        from b in y
        select NUM.Product(a, b);

    /// <summary>
    /// Divide the two bound values of x and y, uses a Divide trait to provide the divide
    /// operation for type A.  For example x.Divide<TDouble,double>(y)
    /// </summary>
    /// <typeparam name="NUM">Num of A</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <param name="x">Left hand side of the operation</param>
    /// <param name="y">Right hand side of the operation</param>
    /// <returns>An option x / y</returns>
    [Pure]
    public static Either<L, R> Divide<NUM, L, R>(this Either<L, R> x, Either<L, R> y) where NUM : Num<R> =>
        from a in x
        from b in y
        select NUM.Divide(a, b);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type FB derived from Applicative of B</returns>
    [Pure]
    public static Either<L, B> Apply<L, A, B>(this Either<L, Func<A, B>> fab, Either<L, A> fa) =>
        Applicative.apply(fab, fa).As();

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative a to apply</param>
    /// <param name="fb">Applicative b to apply</param>
    /// <returns>Applicative of type FC derived from Applicative of C</returns>
    [Pure]
    public static Either<L, C> Apply<L, A, B, C>(this Either<L, Func<A, B, C>> fabc, Either<L, A> fa, Either<L, B> fb) =>
        Applicative.apply(fabc.Map(curry), fa).As().Apply(fb);

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static Either<L, Func<B, C>> Apply<L, A, B, C>(this Either<L, Func<A, B, C>> fabc, Either<L, A> fa) =>
        Applicative.apply(fabc.Map(curry), fa).As();

    /// <summary>
    /// Apply
    /// </summary>
    /// <param name="fab">Function to apply the applicative to</param>
    /// <param name="fa">Applicative to apply</param>
    /// <returns>Applicative of type f(b -> c) derived from Applicative of Func<B, C></returns>
    [Pure]
    public static Either<L, Func<B, C>> Apply<L, A, B, C>(this Either<L, Func<A, Func<B, C>>> fabc, Either<L, A> fa) =>
        Applicative.apply(fabc, fa).As();

    /// <summary>
    /// Evaluate fa, then fb, ignoring the result of fa
    /// </summary>
    /// <param name="fa">Applicative to evaluate first</param>
    /// <param name="fb">Applicative to evaluate second and then return</param>
    /// <returns>Applicative of type Option<B></returns>
    [Pure]
    public static Either<L, B> Action<L, A, B>(this Either<L, A> fa, Either<L, B> fb) =>
        Applicative.action(fa, fb).As();

    /// <summary>
    /// Extracts from a list of 'Either' all the 'Left' elements.
    /// All the 'Left' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static IEnumerable<L> Lefts<L, R>(this IEnumerable<Either<L, R>> self)
    {
        foreach (var item in self)
        {
            if (item.IsLeft)
            {
                yield return item.LeftValue;
            }
        }
    }

    /// <summary>
    /// Extracts from a list of 'Either' all the 'Left' elements.
    /// All the 'Left' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static Seq<L> Lefts<L, R>(this Seq<Either<L, R>> self) =>
        Lefts(self.AsEnumerable()).ToSeq();

    /// <summary>
    /// Extracts from a list of 'Either' all the 'Right' elements.
    /// All the 'Right' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static IEnumerable<R> Rights<L, R>(this IEnumerable<Either<L, R>> self)
    {
        foreach (var item in self)
        {
            if (item.IsRight)
            {
                yield return item.RightValue;
            }
        }
    }

    /// <summary>
    /// Extracts from a list of 'Either' all the 'Right' elements.
    /// All the 'Right' elements are extracted in order.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>An enumerable of L</returns>
    [Pure]
    public static Seq<R> Rights<L, R>(this Seq<Either<L, R>> self) =>
        Rights(self.AsEnumerable()).ToSeq();

    /// <summary>
    /// Partitions a list of 'Either' into two lists.
    /// All the 'Left' elements are extracted, in order, to the first
    /// component of the output.  Similarly the 'Right' elements are extracted
    /// to the second component of the output.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>A tuple containing the an enumerable of L and an enumerable of R</returns>
    [Pure]
    public static (IEnumerable<L> Lefts, IEnumerable<R> Rights) Partition<L, R>(this IEnumerable<Either<L, R>> self)
    {
        var ls = new List<L>();
        var rs = new List<R>();
        foreach (var item in self)
        {
            if (item.IsRight) rs.Add(item.RightValue);
            if (item.IsLeft) ls.Add(item.LeftValue);
        }
        return (ls, rs);
    }

    /// <summary>
    /// Partitions a list of 'Either' into two lists.
    /// All the 'Left' elements are extracted, in order, to the first
    /// component of the output.  Similarly the 'Right' elements are extracted
    /// to the second component of the output.
    /// </summary>
    /// <typeparam name="L">Left</typeparam>
    /// <typeparam name="R">Right</typeparam>
    /// <param name="self">Either list</param>
    /// <returns>A tuple containing the an enumerable of L and an enumerable of R</returns>
    [Pure]
    public static (Seq<L> Lefts, Seq<R> Rights) Partition<L, R>(this Seq<Either<L, R>> self)
    {
        var (l, r) =self.AsEnumerable().Partition();
        return (l.ToSeq(), r.ToSeq());
    }

    /// <summary>
    /// Match the two states of the Either and return a promise of a non-null R2.
    /// </summary>
    public static async Task<R2> MatchAsync<L, R, R2>(this Either<L, Task<R>> self, Func<R, R2> Right, Func<L, R2> Left) =>
        Check.NullReturn(self.IsRight
            ? Right(await self.RightValue.ConfigureAwait(false))
            : Left(self.LeftValue));

    public static async Task<Either<L, R2>> MapAsync<L, R, R2>(this Either<L, R> self, Func<R, Task<R2>> map) =>
        self.IsRight
            ? await map(self.RightValue).ConfigureAwait(false)
            : Left<L, R2>(self.LeftValue);

    public static async Task<Either<L, R2>> MapAsync<L, R, R2>(this Task<Either<L, R>> self, Func<R, Task<R2>> map)
    {
        var val = await self.ConfigureAwait(false);
        return val.IsRight
            ? await map(val.RightValue).ConfigureAwait(false)
            : Left<L, R2>(val.LeftValue);
    }

    public static async Task<Either<L, R2>> MapAsync<L, R, R2>(this Task<Either<L, R>> self, Func<R, R2> map)
    {
        var val = await self.ConfigureAwait(false);
        return val.IsRight
            ? map(val.RightValue)
            : Left<L, R2>(val.LeftValue);
    }

    public static async Task<Either<L, R2>> MapAsync<L, R, R2>(this Either<L, Task<R>> self, Func<R, R2> map) =>
        self.IsRight
            ? map(await self.RightValue.ConfigureAwait(false))
            : Left<L, R2>(self.LeftValue);

    public static async Task<Either<L, R2>> MapAsync<L, R, R2>(this Either<L, Task<R>> self, Func<R, Task<R2>> map) =>
        self.IsRight
            ? await map(await self.RightValue).ConfigureAwait(false)
            : Left<L, R2>(self.LeftValue);


    public static async Task<Either<L, R2>> BindAsync<L, R, R2>(this Either<L, R> self, Func<R, Task<Either<L, R2>>> bind) =>
        self.IsRight
            ? await bind(self.RightValue).ConfigureAwait(false)
            : Left<L, R2>(self.LeftValue);

    public static async Task<Either<L, R2>> BindAsync<L, R, R2>(this Task<Either<L, R>> self, Func<R, Task<Either<L, R2>>> bind)
    {
        var val = await self.ConfigureAwait(false);
        return val.IsRight
            ? await bind(val.RightValue).ConfigureAwait(false)
            : Left<L, R2>(val.LeftValue);
    }

    public static async Task<Either<L, R2>> BindAsync<L, R, R2>(this Task<Either<L, R>> self, Func<R, Either<L, R2>> bind)
    {
        var val = await self.ConfigureAwait(false);
        return val.IsRight
            ? bind(val.RightValue)
            : Left<L, R2>(val.LeftValue);
    }

    public static async Task<Either<L, R2>> BindAsync<L, R, R2>(this Either<L, Task<R>> self, Func<R, Either<L, R2>> bind) =>
        self.IsRight
            ? bind(await self.RightValue.ConfigureAwait(false))
            : Left<L, R2>(self.LeftValue);

    public static async Task<Either<L, R2>> BindAsync<L, R, R2>(this Either<L, Task<R>> self, Func<R, Task<Either<L, R2>>> bind) =>
        self.IsRight
            ? await bind(await self.RightValue.ConfigureAwait(false)).ConfigureAwait(false)
            : Left<L, R2>(self.LeftValue);

    public static async Task<Unit> IterAsync<L, R>(this Task<Either<L, R>> self, Action<R> action)
    {
        var val = await self.ConfigureAwait(false);
        if (val.IsRight) action(val.RightValue);
        return unit;
    }

    public static async Task<Unit> IterAsync<L, R>(this Either<L, Task<R>> self, Action<R> action)
    {
        if (self.IsRight) action(await self.RightValue.ConfigureAwait(false));
        return unit;
    }

    /// <summary>
    /// Convert to an Eff
    /// </summary>
    /// <returns>Eff monad</returns>
    [Pure]
    public static Eff<R> ToEff<R>(this Either<Error, R> ma) =>
        ma.State switch
        {
            EitherStatus.IsRight => Pure(ma.RightValue),
            EitherStatus.IsLeft  => Fail(ma.LeftValue),
            _                    => default // bottom
        };

    /// <summary>
    /// Convert to an Eff
    /// </summary>
    /// <returns>Eff monad</returns>
    [Pure]
    public static Eff<R> ToEff<R>(this Either<Exception, R> ma) =>
        ma.State switch
        {
            EitherStatus.IsRight => Pure(ma.RightValue),
            EitherStatus.IsLeft  => Fail<Error>(ma.LeftValue),
            _                    => default // bottom
        };

    /// <summary>
    /// Convert to an Eff
    /// </summary>
    /// <returns>Eff monad</returns>
    [Pure]
    public static Eff<R> ToEff<R>(this Either<string, R> ma) =>
        ma.State switch
        {
            EitherStatus.IsRight => Pure(ma.RightValue),
            EitherStatus.IsLeft  => Fail(Error.New(ma.LeftValue)),
            _                    => default // bottom
        };
}
