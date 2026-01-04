using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using LanguageExt.Common;

namespace LanguageExt.Traits;

/// <summary>
/// Monad module
/// </summary>
public static partial class Monad
{
    [Pure]
    public static K<M, A> pure<M, A>(A value) 
        where M : Monad<M> =>
        M.Pure(value);

    [Pure]
    public static K<M, A> flatten<M, A>(K<M, K<M, A>> mma)
        where M : Monad<M> =>
        M.Flatten(mma);

    [Pure]
    public static K<M, B> bind<M, A, B>(K<M, A> ma, Func<A, K<M, B>> f)
        where M : Monad<M> =>
        M.Bind(ma, f);
    
    [Pure]
    public static MB bind<M, MB, A, B>(K<M, A> ma, Func<A, MB> f)
        where MB : K<M, B>
        where M : Monad<M> =>
        (MB)bind(ma, x => f(x));

    /// <summary>
    /// This is equivalent to the infinite loop below without the stack-overflow issues:
    ///
    ///     K〈M, A〉go =>
    ///         ma.Bind(_ => go);
    /// 
    /// </summary>
    /// <param name="ma">Computation to recursively run</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <typeparam name="B">'Result' type, there will never be a result of `B`, but the monad rules may exit the loop
    /// with an alternative value; and so `B` is still valid</typeparam>
    /// <returns>A looped computation</returns>
    [Pure]
    public static K<M, A> forever<M, A>(K<M, A> ma)
        where M : Monad<M> =>
        forever<M, A, A>(ma);
    
    /// <summary>
    /// This is equivalent to the infinite loop below without the stack-overflow issues:
    ///
    ///     K〈M, A〉go =>
    ///         ma.Bind(_ => go);
    /// 
    /// </summary>
    /// <param name="ma">Monadic computation to recursively run</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <typeparam name="B">'Result' type, there will never be a result of `B`, but the monad rules may exit the loop
    /// with an alternative value; and so `B` is still valid</typeparam>
    /// <returns>A looped computation</returns>
    [Pure]
    public static K<M, B> forever<M, A, B>(K<M, A> ma) 
        where M : Monad<M> => 
        recur<M, A, B>(default!, _ => ma.Map(_ => Next<A, B>.UnsafeDefault));

    /// <summary>
    /// Running the monadic computation `ma` a fixed number of times (`count`) collecting the results
    /// </summary>
    /// <param name="ma">Monadic computation to repeatedly run</param>
    /// <param name="count">Number of times to replicate monadic computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>A lifted iterable of values collected</returns>
    [Pure]
    public static K<M, Iterable<A>> replicate<M, A>(K<M, A> ma, int count)
        where M : Monad<M> =>
        recur<M, (Iterable<A> Items, int Remain), Iterable<A>>(
            ([], count),
            acc => ma * (a => acc.Remain > 0 
                          ? Next.Loop<(Iterable<A> Items, int Remain), Iterable<A>>((acc.Items.Add(a), acc.Remain - 1))
                          : Next.Done<(Iterable<A> Items, int Remain), Iterable<A>>(acc.Items.Add(a))));

    /// <summary>
    /// Keep running the monadic computation `ma` collecting the result values until a result value
    /// yielded triggers a `true` value when passed to the `f` predicate
    /// </summary>
    /// <param name="ma">Monadic computation to repeatedly run</param>
    /// <param name="f">Predicate</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>A lifted iterable of values collected</returns>
    [Pure]
    public static K<M, Iterable<A>> accumUntil<M, A>(K<M, A> ma, Func<A, bool> f)
        where M : Monad<M> =>
        recur<M, Iterable<A>, Iterable<A>>(
            [],
            acc => ma * (a => f(a) ? Next.Done<Iterable<A>, Iterable<A>>(acc)
                                   : Next.Loop<Iterable<A>, Iterable<A>>(acc.Add(a))));

    /// <summary>
    /// Keep running the monadic computation `ma` collecting the result values until a result value
    /// yielded triggers a `true` value when passed to the `f` predicate
    /// </summary>
    /// <param name="ma">Monadic computation to repeatedly run</param>
    /// <param name="f">Predicate</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>A lifted iterable of values collected</returns>
    [Pure]
    public static K<M, Iterable<A>> accumUntilM<M, A>(K<M, A> ma, Func<A, K<M, bool>> f)
        where M : Monad<M> =>
        recur<M, Iterable<A>, Iterable<A>>(
            [],
            acc => ma >> (a => f(a) * (r => r ? Next.Done<Iterable<A>, Iterable<A>>(acc)
                                              : Next.Loop<Iterable<A>, Iterable<A>>(acc.Add(a)))));

    /// <summary>
    /// Keep running the monadic computation `ma` collecting the result values until a result value
    /// yielded triggers a `true` value when passed to the `f` predicate
    /// </summary>
    /// <param name="ma">Monadic computation to repeatedly run</param>
    /// <param name="f">Predicate</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>A lifted iterable of values collected</returns>
    [Pure]
    public static K<M, Iterable<A>> accumWhile<M, A>(K<M, A> ma, Func<A, bool> f)
        where M : Monad<M> =>
        recur<M, Iterable<A>, Iterable<A>>(
            [],
            acc => ma * (a => f(a) ? Next.Loop<Iterable<A>, Iterable<A>>(acc.Add(a))
                                   : Next.Done<Iterable<A>, Iterable<A>>(acc)));

    /// <summary>
    /// Keep running the monadic computation `ma` collecting the result values until a result value
    /// yielded triggers a `true` value when passed to the `f` predicate
    /// </summary>
    /// <param name="ma">Monadic computation to repeatedly run</param>
    /// <param name="f">Predicate</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>A lifted iterable of values collected</returns>
    [Pure]
    public static K<M, Iterable<A>> accumWhileM<M, A>(K<M, A> ma, Func<A, K<M, bool>> f)
        where M : Monad<M> =>
        recur<M, Iterable<A>, Iterable<A>>(
            [],
            acc => ma >> (a => f(a) * (r => r ? Next.Loop<Iterable<A>, Iterable<A>>(acc.Add(a))
                                              : Next.Done<Iterable<A>, Iterable<A>>(acc))));
    
    /// <summary>
    /// Lift a Next.Done value into the monad
    /// </summary>
    [Pure]
    public static K<M, Next<A, B>> done<M, A, B>(B value) 
        where M : Monad<M> =>
        M.Pure(Next.Done<A, B>(value));
    
    /// <summary>
    /// Lift a Next.Done value into the monad
    /// </summary>
    [Pure]
    public static K<M, Next<A, B>> loop<M, A, B>(A value) 
        where M : Monad<M> =>
        M.Pure(Next.Loop<A, B>(value));
    
    /// <summary>
    /// Allow for tail-recursion by using a trampoline function that returns a monad with the bound value
    /// wrapped by `Next`, which enables decision-making about whether to keep the computation going or not.  
    /// </summary>
    /// <remarks>
    /// It is expected that the implementor of the `Monad` trait has made a 'stack-neutral' implementation
    /// of `Monad.Recur` 
    /// </remarks>
    /// <param name="value">Initial value to start the recursive process</param>
    /// <param name="f">Bind function that returns a monad with the bound value wrapped by `Next`, which
    /// enables decision-making about whether to recur, or not.</param>
    /// <typeparam name="M">Monad type</typeparam>
    /// <typeparam name="A">Loop value</typeparam>
    /// <typeparam name="B">Done value</typeparam>
    /// <returns>Monad structure</returns>
    [Pure]
    public static K<M, B> recur<M, A, B>(A value, Func<A, K<M, Next<A, B>>> f) 
        where M : Monad<M> =>
        M.Recur(value, f);

    /// <summary>
    /// This is a default implementation of `Monad.Recur` that doesn't use the trampoline.
    /// It's here to use as a placeholder implementation for the trampoline version and for types
    /// where it's unlikely that recursion will be a problem.  NOTE: Using this isn't declarative,
    /// and the users of `Monad.recur` would rightly be miffed if an implementation yielded a
    /// stack-overflow, so use this function with caution.
    /// </summary>
    /// <param name="value">Initial value to start the recursive process</param>
    /// <param name="f">Bind function that returns a monad with the bound value wrapped by `Next`, which
    /// enables decision-making about whether to recur, or not.</param>
    /// <typeparam name="M">Monad type</typeparam>
    /// <typeparam name="A">Loop value</typeparam>
    /// <typeparam name="B">Done value</typeparam>
    /// <returns>Monad structure</returns>
    /// <exception cref="BottomException"></exception>
    [Pure]
    public static K<M, B> unsafeRecur<M, A, B>(A value, Func<A, K<M, Next<A, B>>> f)
        where M : Monad<M> =>
        f(value).Bind(n => n switch
                           {
                               { IsLoop: true, Loop: var v } => unsafeRecur(v, f),
                               { IsDone: true, Done: var v } => M.Pure(v),
                               _                                  => throw new BottomException()
                           });

    /// <summary>
    /// Allow for tail-recursion by using a trampoline function that returns a monad with the bound value
    /// wrapped by `Next`, which enables decision-making about whether to keep the computation going or not.  
    /// </summary>
    /// <remarks>
    /// This is a handy pre-built version of `Monad.Recur` that works with `Iterable` (a lazy stream that supports
    /// both synchronicity and asynchronicity).  The `Natural` and `CoNatural` constraints allow any type that can
    /// convert to and from `Iterable` to gain this prebuilt stack-protecting recursion.  
    /// </remarks>
    /// <param name="value">Initial value to start the recursive process</param>
    /// <param name="f">Bind function that returns a monad with the bound value wrapped by `Next`, which
    /// enables decision-making about whether to recur, or not.</param>
    /// <typeparam name="M">Monad type</typeparam>
    /// <typeparam name="A">Loop value</typeparam>
    /// <typeparam name="B">Done value</typeparam>
    /// <returns>Monad structure</returns>
    [Pure]
    public static K<M, B> iterableRecur<M, A, B>(A value, Func<A, K<M, Next<A, B>>> f)
        where M : Natural<M, Iterable>, CoNatural<M, Iterable>
    {
        var iterable = Iterable.createRange(IO.lift(e => go(e.Token)));
        return CoNatural.transform<M, Iterable, B>(iterable);
        
        async IAsyncEnumerable<B> go([EnumeratorCancellation] CancellationToken token)
        {
            List<A> values = [value];
            List<A> next   = [];

            while (true)
            {
                foreach (var x in values)
                {
                    var iterable1 = Natural.transform<M, Iterable, Next<A, B>>(f(x)).As().AsAsyncEnumerable(token);
                    await foreach (var mb in iterable1)
                    {
                        if (mb.IsDone)
                        {
                            yield return mb.Done;
                        }
                        else
                        {
                            next.Add(mb.Loop);
                        }
                    }
                }

                if (next.Count == 0)
                {
                    break;
                }
                else
                {
                    (values, next) = (next, values);
                    next.Clear();
                }
            }
        }
    }

    /// <summary>
    /// Allow for tail-recursion by using a trampoline function that returns an enumerable monad with the bound value
    /// wrapped by `Next`, which enables decision-making about whether to keep the computation going or not.  
    /// </summary>
    /// <remarks>
    /// This is a handy pre-built version of `Monad.Recur` that works with `IEnumerable` (a lazy stream that supports
    /// synchronicity only)  
    /// </remarks>
    /// <param name="value">Initial value to start the recursive process</param>
    /// <param name="f">Bind function that returns a monad with the bound value wrapped by `Next`, which
    /// enables decision-making about whether to recur, or not.</param>
    /// <typeparam name="M">Monad type</typeparam>
    /// <typeparam name="A">Loop value</typeparam>
    /// <typeparam name="B">Done value</typeparam>
    /// <returns>Monad structure</returns>
    [Pure]
    public static IEnumerable<B> enumerableRecur<A, B>(A value, Func<A, IEnumerable<Next<A, B>>> f)
    {
        List<A> values = [value];
        List<A> next   = [];

        while (true)
        {
            foreach (var x in values)
            {
                var iterable1 = f(x);
                foreach (var mb in iterable1)
                {
                    if (mb.IsDone)
                    {
                        yield return mb.Done;
                    }
                    else
                    {
                        next.Add(mb.Loop);
                    }
                }
            }

            if (next.Count == 0)
            {
                break;
            }
            else
            {
                (values, next) = (next, values);
                next.Clear();
            }
        }
    }
    
    /// <summary>
    /// When the predicate evaluates to `true`, compute `Then`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, Unit> when<M>(K<M, bool> Pred, K<M, Unit> Then)
        where M : Monad<M> =>
        Pred.Bind(f => Applicative.when(f, Then));

    /// <summary>
    /// When the predicate evaluates to `true`, compute `Then`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, Unit> when<M>(K<M, bool> Pred, Pure<Unit> Then)
        where M : Monad<M> =>
        Pred.Bind(f => Applicative.when(f, M.Pure(Prelude.unit)));

    /// <summary>
    /// When the predicate evaluates to `false`, compute `Then`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, Unit> unless<M>(K<M, bool> Pred, K<M, Unit> Then)
        where M : Monad<M> =>
        Pred.Bind(f => Applicative.unless(f, Then));

    /// <summary>
    /// When the predicate evaluates to `false`, compute `Then`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, Unit> unless<M>(K<M, bool> Pred, Pure<Unit> Then)
        where M : Monad<M> =>
        Pred.Bind(f => Applicative.unless(f, M.Pure(Prelude.unit)));

    /// <summary>
    /// Compute the predicate and depending on its state compute `Then` or `Else`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <param name="Else">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, A> iff<M, A>(K<M, bool> Pred, K<M, A> Then, K<M, A> Else)
        where M : Monad<M> =>
        Pred.Bind(f => f ? Then : Else);

    /// <summary>
    /// Compute the predicate and depending on its state compute `Then` or `Else`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <param name="Else">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, A> iff<M, A>(K<M, bool> Pred, K<M, A> Then, K<IO, A> Else)
        where M : Monad<M> =>
        Pred.Bind(f => f ? Then : M.LiftIOMaybe(Else));

    /// <summary>
    /// Compute the predicate and depending on its state compute `Then` or `Else`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <param name="Else">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, A> iff<M, A>(K<M, bool> Pred, K<IO, A> Then, K<M, A> Else)
        where M : Monad<M> =>
        Pred.Bind(f => f ? M.LiftIOMaybe(Then) : Else);

    /// <summary>
    /// Compute the predicate and depending on its state compute `Then` or `Else`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <param name="Else">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, A> iff<M, A>(K<M, bool> Pred, K<IO, A> Then, K<IO, A> Else)
        where M : Monad<M> =>
        Pred.Bind(f => f ? M.LiftIOMaybe(Then) : M.LiftIOMaybe(Else));

    /// <summary>
    /// Compute the predicate and depending on its state compute `Then` or `Else`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <param name="Else">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, A> iff<M, A>(K<M, bool> Pred, K<M, A> Then, Pure<A> Else)
        where M : Monad<M> =>
        Pred.Bind(f => f ? Then : M.Pure(Else.Value));

    /// <summary>
    /// Compute the predicate and depending on its state compute `Then` or `Else`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <param name="Else">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, A> iff<M, A>(K<M, bool> Pred, Pure<A> Then, K<M, A> Else)
        where M : Monad<M> =>
        Pred.Bind(f => f ? M.Pure(Then.Value) : Else);

    /// <summary>
    /// Compute the predicate and depending on its state compute `Then` or `Else`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <param name="Else">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, A> iff<M, A>(K<M, bool> Pred, Pure<A> Then, Pure<A> Else)
        where M : Monad<M> =>
        Pred.Bind(f => f ? M.Pure(Then.Value) : M.Pure(Else.Value));

    /// <summary>
    /// Compute the predicate and depending on its state compute `Then` or `Else`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <param name="Else">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, A> iff<M, A>(K<M, bool> Pred, Pure<A> Then, K<IO, A> Else)
        where M : Monad<M> =>
        Pred.Bind(f => f ? M.Pure(Then.Value) : M.LiftIOMaybe(Else));

    /// <summary>
    /// Compute the predicate and depending on its state compute `Then` or `Else`
    /// </summary>
    /// <param name="Pred">Predicate</param>
    /// <param name="Then">Computation</param>
    /// <param name="Else">Computation</param>
    /// <typeparam name="M">Monad</typeparam>
    /// <returns>Unit monad</returns>
    [Pure]
    public static K<M, A> iff<M, A>(K<M, bool> Pred, K<IO, A> Then, Pure<A> Else)
        where M : Monad<M> =>
        Pred.Bind(f => f ? M.LiftIOMaybe(Then) : M.Pure(Else.Value));
}
