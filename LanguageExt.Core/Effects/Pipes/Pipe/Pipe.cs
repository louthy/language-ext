using System;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using static LanguageExt.Pipes.Proxy;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

/// <summary>
/// Pipes both can both be `await` and can `yield`
/// </summary>
/// <remarks>
///       Upstream | Downstream
///           +---------+
///           |         |
///     Unit <==       <== Unit
///           |         |
///      IN  ==>       ==> OUT
///           |    |    |
///           +----|----+
///                |
///                A
/// </remarks>
public static class Pipe
{
    /// <summary>
    /// Monad return / pure
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<A, B, M, R> Pure<A, B, M, R>(R value) where M : Monad<M> =>
        new Pure<Unit, A, Unit, B, M, R>(value).ToPipe();
        
    /// <summary>
    /// Wait for a value from upstream (whilst in a pipe)
    /// </summary>
    /// <remarks>
    /// This is the version of `await` that works for pipes.  In consumers, use `Consumer.await`
    /// </remarks>
    [Pure, MethodImpl(mops)]
    public static Pipe<A, Y, M, A> awaiting<M, A, Y>() 
        where M : Monad<M> =>
        request<Unit, A, Unit, Y, M>(unit).ToPipe();
        
    /// <summary>
    /// Send a value downstream (whilst in a pipe)
    /// </summary>
    /// <remarks>
    /// This is the version of `yield` that works for pipes.  In producers, use `Producer.yield`
    /// </remarks>
    [Pure, MethodImpl(mops)]
    public static Pipe<IN, OUT, M, Unit> yield<IN, OUT, M>(OUT value) 
        where M : Monad<M> =>
        respond<Unit, IN, Unit, OUT, M>(value).ToPipe();

    /// <summary>
    /// Only forwards values that satisfy the predicate.
    /// </summary>
    public static Pipe<A, A, M, Unit> filter<M, A>(Func<A, bool> f) 
        where M : Monad<M> =>
        cat<A, M, Unit>().For(a => f(a)
                                       ? yield<A, A, M>(a)
                                       : Pure<A, A, M, Unit>(default))
                         .ToPipe();

    /// <summary>
    /// Map the output of the pipe (not the bound value as is usual with Map)
    /// </summary>
    public static Pipe<A, B, M, R> map<A, B, M, R>(Func<A, B> f) 
        where M : Monad<M> =>
        cat<A, M, R>().For(a => yield<A, B, M>(f(a))).ToPipe();

    /// <summary>
    /// Map the output of the pipe (not the bound value as is usual with Map)
    /// </summary>
    public static Pipe<A, B, M, Unit> map<A, M, B>(Func<A, B> f) 
        where M : Monad<M> =>
        cat<A, M, Unit>().For(a => yield<A, B, M>(f(a))).ToPipe();

    /// <summary>
    /// Map the output of the pipe (not the bound value as is usual with Map)
    /// </summary>
    public static Pipe<A, B, Unit> map<A, B>(Func<A, B> f) =>
        new Pipe<A, B, Unit>.Await(x => new Pipe<A, B, Unit>.Yield(f(x), PureProxy.PipePure<A, B, Unit>));
    
    /// <summary>
    /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<A, B, M, R> mapM<A, B, M, R>(Func<A, K<M, B>> f) where M : Monad<M> =>
        cat<A, M, R>()
           .ForEach(a => lift<A, B, M, B>(f(a)).Bind(x => yield<A, B, M>(x)).ToPipe());
    
    /// <summary>
    /// Lift the IO monad into the Pipe monad transformer (a specialism of the Proxy monad transformer)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<A, B, M, R> lift<A, B, M, R>(K<M, R> ma) 
        where M : Monad<M> =>
        lift<Unit, A, Unit, B, M, R>(ma).ToPipe(); 
 
    /// <summary>
    /// Lift the IO monad into the Pipe monad transformer (a specialism of the Proxy monad transformer)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<A, B, M, R> liftIO<A, B, M, R>(IO<R> ma) 
        where M : Monad<M> =>
        liftIO<Unit, A, Unit, B, M, R>(ma).ToPipe(); 

    /// <summary>
    /// Folds values coming down-stream, when the predicate returns false the folded value is yielded 
    /// </summary>
    /// <param name="Initial">Initial state</param>
    /// <param name="Fold">Fold operation</param>
    /// <param name="WhileState">Predicate</param>
    /// <returns>A pipe that folds</returns>
    [Pure, MethodImpl(mops)]
    public static Pipe<IN, OUT, M, Unit> foldWhile<IN, OUT, M>(
        OUT Initial, 
        Func<OUT, IN, OUT> Fold, 
        Func<OUT, bool> WhileState) 
        where M : Monad<M> =>
        foldUntil<IN, OUT, M>(Initial, Fold, x => !WhileState(x));
 
    /// <summary>
    /// Folds values coming down-stream, when the predicate returns true the folded value is yielded 
    /// </summary>
    /// <param name="Initial">Initial state</param>
    /// <param name="Fold">Fold operation</param>
    /// <param name="UntilState">Predicate</param>
    /// <returns>A pipe that folds</returns>
    public static Pipe<IN, OUT, M, Unit> foldUntil<IN, OUT, M>(
        OUT Initial, 
        Func<OUT, IN, OUT> Fold, 
        Func<OUT, bool> UntilState)
        where M : Monad<M>
    {
        var state = Initial;
        return awaiting<M, IN, OUT>()
                   .Bind(x =>
                         {
                             state = Fold(state, x);
                             if (UntilState(state))
                             {
                                 var nstate = state;
                                 state = Initial;
                                 return yield<IN, OUT, M>(nstate);
                             }
                             else
                             {
                                 return Pure<IN, OUT, M, Unit>(unit);
                             }
                         })
                   .ToPipe();
    }

    /// <summary>
    /// Folds values coming down-stream, when the predicate returns false the folded value is yielded 
    /// </summary>
    /// <param name="Initial">Initial state</param>
    /// <param name="Fold">Fold operation</param>
    /// <param name="WhileValue">Predicate</param>
    /// <returns>A pipe that folds</returns>
    [Pure, MethodImpl(mops)]
    public static Pipe<IN, OUT, M, Unit> foldWhile<IN, OUT, M>(
        OUT Initial
      , Func<OUT, IN, OUT> Fold, 
        Func<IN, bool> WhileValue) 
        where M : Monad<M> =>
        foldUntil<IN, OUT, M>(Initial, Fold, x => !WhileValue(x));
 
    /// <summary>
    /// Folds values coming down-stream, when the predicate returns true the folded value is yielded 
    /// </summary>
    /// <param name="Initial">Initial state</param>
    /// <param name="Fold">Fold operation</param>
    /// <param name="UntilValue">Predicate</param>
    /// <returns>A pipe that folds</returns>
    public static Pipe<IN, OUT, M, Unit> foldUntil<IN, OUT, M>(
        OUT Initial, 
        Func<OUT, IN, OUT> Fold, 
        Func<IN, bool> UntilValue)
        where M : Monad<M>
    {
        var state = Initial;
        return awaiting<M, IN, OUT>()
                   .Bind(x =>
                         {
                             if (UntilValue(x))
                             {
                                 var nstate = state;
                                 state = Initial;
                                 return yield<IN, OUT, M>(nstate);
                             }
                             else
                             {
                                 state = Fold(state, x);
                                 return Pure<IN, OUT, M, Unit>(unit);
                             }
                         })
                   .ToPipe();
    }

    /// <summary>
    /// Strict left scan
    /// </summary>
    public static Pipe<IN, OUT, M, Unit> scan<IN, OUT, M, S>(Func<S, IN, S> Step, S Begin, Func<S, OUT> Done)
        where M : Monad<M>
    {
        return go(Begin);

        Pipe<IN, OUT, M, Unit> go(S x) =>
            from _ in yield<IN, OUT, M>(Done(x))
            from a in awaiting<M, IN, OUT>()
            let x1 = Step(x, a)
            from r in go(x1)
            select r;
    }
}
