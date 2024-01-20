#nullable enable

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class Prelude
{
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Recursion
    //
    
    /// <summary>
    /// Wrap this around a tail recursive call to mark it as the end of a recursive expression.
    /// </summary>
    public static Transducer<A, B> tail<A, B>(Transducer<A, B> recursive) =>
        new TailTransducer<A, B>(recursive);
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Lifting
    //
    
    /// <summary>
    /// Lift a function into a `Transducer`
    /// </summary>
    /// <param name="action0">Function</param>
    /// <returns>Transducer that wraps the lifted function</returns>
    public static Transducer<Unit, Unit> lift(Action action0) =>
        lift<Unit, Unit>(x =>
        {
            action0();
            return unit;
        });

    /// <summary>
    /// Lift a function into a `Transducer`
    /// </summary>
    /// <param name="action1">Function</param>
    /// <returns>Transducer that wraps the lifted function</returns>
    public static Transducer<A, Unit> lift<A>(Action<A> action1) =>
        lift<A, Unit>(x =>
        {
            action1(x);
            return unit;
        });
    
    /// <summary>
    /// Lift a function into a `Transducer`
    /// </summary>
    /// <param name="function0">Function</param>
    /// <returns>Transducer that wraps the lifted function</returns>
    public static Transducer<Unit, A> lift<A>(Func<A> function0) =>
        new LiftTransducer4<A>(function0);
    
    /// <summary>
    /// Lift a function into a `Transducer`
    /// </summary>
    /// <param name="tfunction0">Function</param>
    /// <returns>Transducer that wraps the lifted function</returns>
    public static Transducer<Unit, A> lift<A>(Func<TResult<A>> tfunction0) =>
        new LiftTransducer2<A>(tfunction0);
    
    /// <summary>
    /// Lift a function into a `Transducer`
    /// </summary>
    /// <param name="function1">Function</param>
    /// <returns>Transducer that wraps the lifted function</returns>
    public static Transducer<A, B> lift<A, B>(Func<A, B> function1) =>
        new LiftTransducer3<A, B>(function1);

    /// <summary>
    /// Lift a function into a `Transducer`
    /// </summary>
    /// <param name="tfunction1">Function</param>
    /// <returns>Transducer that wraps the lifted function</returns>
    public static Transducer<A, B> lift<A, B>(Func<A, TResult<B>> tfunction1) =>
        new LiftTransducer1<A, B>(tfunction1);

    /// <summary>
    /// Lift a function into a `Transducer`
    /// </summary>
    /// <param name="function2">Function</param>
    /// <returns>Transducer that wraps the lifted function</returns>
    public static Transducer<A, Transducer<B, C>> lift<A, B, C>(
        Func<A, B, C> function2) =>
        lift(curry(function2)).Map(lift);

    /// <summary>
    /// Lift a function into a `Transducer`
    /// </summary>
    /// <param name="tfunction2">Function</param>
    /// <returns>Transducer that wraps the lifted function</returns>
    public static Transducer<A, Transducer<B, C>> lift<A, B, C>(
        Func<A, B, TResult<C>> tfunction2) =>
        lift(curry(tfunction2)).Map(lift);

    /// <summary>
    /// Lift a function into a `Transducer`
    /// </summary>
    /// <param name="function3">Function</param>
    /// <returns>Transducer that wraps the lifted function</returns>
    public static Transducer<A, Transducer<B, Transducer<C, D>>> lift<A, B, C, D>(
        Func<A, B, C, D> function3) =>
        lift(curry(function3)).Map(t => lift(t).Map(lift));

    /// <summary>
    /// Lift a function into a `Transducer`
    /// </summary>
    /// <param name="tfunction3">Function</param>
    /// <returns>Transducer that wraps the lifted function</returns>
    public static Transducer<A, Transducer<B, Transducer<C, D>>> lift<A, B, C, D>(
        Func<A, B, C, TResult<D>> tfunction3) =>
        lift(curry(tfunction3)).Map(t => lift(t).Map(lift));

    /// <summary>
    /// Lift a function into a `Transducer`
    /// </summary>
    /// <param name="function4">Function</param>
    /// <returns>Transducer that wraps the lifted function</returns>
    public static Transducer<A, Transducer<B, Transducer<C, Transducer<D, E>>>> lift<A, B, C, D, E>(
        Func<A, B, C, D, E> function4) =>
        lift(curry(function4)).Map(t1 => lift(t1).Map(t2 => lift(t2).Map(lift)));

    /// <summary>
    /// Lift a function into a `Transducer`
    /// </summary>
    /// <param name="tfunction4">Function</param>
    /// <returns>Transducer that wraps the lifted function</returns>
    public static Transducer<A, Transducer<B, Transducer<C, Transducer<D, E>>>> lift<A, B, C, D, E>(
        Func<A, B, C, D, TResult<E>> tfunction4) =>
        lift(curry(tfunction4)).Map(t1 => lift(t1).Map(t2 => lift(t2).Map(lift)));

    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Asynchronous IO lifting
    //
    
    /// <summary>
    /// Lift a asynchronous IO function into a `Transducer`
    /// </summary>
    /// <param name="function0">Function</param>
    /// <returns>Transducer that wraps the lifted function</returns>
    public static Transducer<Unit, Unit> liftIO(Func<Task> action0) =>
        liftIO(async _ =>
        {
            await action0().ConfigureAwait(false);
            return unit;
        });

    /// <summary>
    /// Lift a asynchronous IO function into a `Transducer`
    /// </summary>
    /// <param name="function0">Function</param>
    /// <returns>Transducer that wraps the lifted function</returns>
    public static Transducer<A, Unit> liftIO<A>(Func<A, Task> action1) =>
        liftIO<A, Unit>(async (_, v) =>
        {
            await action1(v).ConfigureAwait(false);
            return unit;
        });
    
    /// <summary>
    /// Lift a asynchronous IO function into a `Transducer`
    /// </summary>
    /// <param name="function0">Function</param>
    /// <returns>Transducer that wraps the lifted function</returns>
    public static Transducer<Unit, A> liftIO<A>(Func<Task<A>> function0) =>
        new LiftIOTransducer4<A>(_ => function0());
    
    /// <summary>
    /// Lift a asynchronous IO function into a `Transducer`
    /// </summary>
    /// <param name="tfunction0">Function</param>
    /// <returns>Transducer that wraps the lifted function</returns>
    public static Transducer<Unit, A> liftIO<A>(Func<Task<TResult<A>>> tfunction0) =>
        new LiftIOTransducer2<A>(_ => tfunction0());
    
    /// <summary>
    /// Lift a asynchronous IO function into a `Transducer`
    /// </summary>
    /// <param name="function1">Function</param>
    /// <returns>Transducer that wraps the lifted function</returns>
    public static Transducer<A, B> liftIO<A, B>(Func<A, Task<B>> function1) =>
        new LiftIOTransducer3<A, B>((_, x) => function1(x));
    
    /// <summary>
    /// Lift a asynchronous IO function into a `Transducer`
    /// </summary>
    /// <param name="tfunction1">Function</param>
    /// <returns>Transducer that wraps the lifted function</returns>
    public static Transducer<A, B> liftIO<A, B>(Func<A, Task<TResult<B>>> tfunction1) =>
        new LiftIOTransducer1<A, B>((_, x) => tfunction1(x));    
    
    /// <summary>
    /// Lift a asynchronous IO function into a `Transducer`
    /// </summary>
    /// <param name="function0">Function</param>
    /// <returns>Transducer that wraps the lifted function</returns>
    public static Transducer<Unit, Unit> liftIO(Func<CancellationToken, Task> action0) =>
        liftIO(async t =>
        {
            await action0(t).ConfigureAwait(false);
            return unit;
        });

    /// <summary>
    /// Lift a asynchronous IO function into a `Transducer`
    /// </summary>
    /// <param name="function0">Function</param>
    /// <returns>Transducer that wraps the lifted function</returns>
    public static Transducer<A, Unit> liftIO<A>(Func<CancellationToken, A, Task> action1) =>
        liftIO<A, Unit>(async (t, v) =>
        {
            await action1(t, v).ConfigureAwait(false);
            return unit;
        });
    
    /// <summary>
    /// Lift a asynchronous IO function into a `Transducer`
    /// </summary>
    /// <param name="function0">Function</param>
    /// <returns>Transducer that wraps the lifted function</returns>
    public static Transducer<Unit, A> liftIO<A>(Func<CancellationToken, Task<A>> function0) =>
        new LiftIOTransducer4<A>(function0);
    
    /// <summary>
    /// Lift a asynchronous IO function into a `Transducer`
    /// </summary>
    /// <param name="tfunction0">Function</param>
    /// <returns>Transducer that wraps the lifted function</returns>
    public static Transducer<Unit, A> liftIO<A>(Func<CancellationToken, Task<TResult<A>>> tfunction0) =>
        new LiftIOTransducer2<A>(tfunction0);
    
    /// <summary>
    /// Lift a asynchronous IO function into a `Transducer`
    /// </summary>
    /// <param name="function1">Function</param>
    /// <returns>Transducer that wraps the lifted function</returns>
    public static Transducer<A, B> liftIO<A, B>(Func<CancellationToken, A, Task<B>> function1) =>
        new LiftIOTransducer3<A, B>(function1);
    
    /// <summary>
    /// Lift a asynchronous IO function into a `Transducer`
    /// </summary>
    /// <param name="tfunction1">Function</param>
    /// <returns>Transducer that wraps the lifted function</returns>
    public static Transducer<A, B> liftIO<A, B>(Func<CancellationToken, A, Task<TResult<B>>> tfunction1) =>
        new LiftIOTransducer1<A, B>(tfunction1);
}
