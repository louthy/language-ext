using System;
using static LanguageExt.Prelude;
using LanguageExt.Effects.Traits;
using System.Diagnostics.Contracts;
using static LanguageExt.Pipes.Proxy;
using System.Runtime.CompilerServices;
using LanguageExt.Common;

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
    public static Pipe<RT, A, B, R> Pure<RT, A, B, R>(R value) where RT : HasIO<RT, Error> =>
        new Pure<RT, Unit, A, Unit, B, R>(value).ToPipe();
        
    /// <summary>
    /// Wait for a value from upstream (whilst in a pipe)
    /// </summary>
    /// <remarks>
    /// This is the version of `await` that works for pipes.  In consumers, use `Consumer.await`
    /// </remarks>
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, A, Y, A> awaiting<RT, A, Y>() where RT : HasIO<RT, Error> =>
        request<RT, Unit, A, Unit, Y>(unit).ToPipe();
        
    /// <summary>
    /// Send a value downstream (whilst in a pipe)
    /// </summary>
    /// <remarks>
    /// This is the version of `yield` that works for pipes.  In producers, use `Producer.yield`
    /// </remarks>
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, IN, OUT, Unit> yield<RT, IN, OUT>(OUT value) where RT : HasIO<RT, Error> =>
        respond<RT, Unit, IN, Unit, OUT>(value).ToPipe();

    /// <summary>
    /// Only forwards values that satisfy the predicate.
    /// </summary>
    public static Pipe<RT, A, A, Unit> filter<RT, A>(Func<A, bool> f)  where RT : HasIO<RT, Error> =>
        cat<RT, A, Unit>().For(a => f(a)
                                        ? yield<RT, A, A>(a)
                                        : Pure<RT, A, A, Unit>(default))
                          .ToPipe();

    /// <summary>
    /// Map the output of the pipe (not the bound value as is usual with Map)
    /// </summary>
    public static Pipe<RT, A, B, R> map<RT, A, B, R>(Func<A, B> f) where RT : HasIO<RT, Error> =>
        cat<RT, A, R>().For(a => yield<RT, A, B>(f(a))).ToPipe();

    /// <summary>
    /// Map the output of the pipe (not the bound value as is usual with Map)
    /// </summary>
    public static Pipe<RT, A, B, Unit> map<RT, A, B>(Func<A, B> f) where RT : HasIO<RT, Error> =>
        cat<RT, A, Unit>().For(a => yield<RT, A, B>(f(a))).ToPipe();

    /// <summary>
    /// Map the output of the pipe (not the bound value as is usual with Map)
    /// </summary>
    public static Pipe<A, B, Unit> map<A, B>(Func<A, B> f) =>
        new Pipe<A, B, Unit>.Await(x => new Pipe<A, B, Unit>.Yield(f(x), PureProxy.PipePure<A, B, Unit>));
    
    /// <summary>
    /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, A, B, R> mapM<RT, A, B, R>(Func<A, Eff<RT, B>> f) where RT : HasIO<RT, Error> =>
        cat<RT, A, R>()
           .ForEach(a => lift<RT, A, B, B>(f(a)).Bind(yield<RT, A, B>).ToPipe());
    
    /// <summary>
    /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, A, B, R> mapM<RT, A, B, R>(Func<A, Transducer<RT, B>> f) where RT : HasIO<RT, Error> =>
        cat<RT, A, R>()
           .ForEach(a => lift<RT, A, B, B>(f(a)).Bind(yield<RT, A, B>).ToPipe());
    
    /// <summary>
    /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, A, B, R> mapM<RT, A, B, R>(Func<A, Transducer<Unit, B>> f) where RT : HasIO<RT, Error> =>
        cat<RT, A, R>()
           .ForEach(a => lift<RT, A, B, B>(f(a)).Bind(yield<RT, A, B>).ToPipe());
    
    /// <summary>
    /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, A, B, R> mapM<RT, A, B, R>(Func<A, Transducer<RT, Sum<Error, B>>> f) where RT : HasIO<RT, Error> =>
        cat<RT, A, R>()
           .ForEach(a => lift<RT, A, B, B>(f(a)).Bind(yield<RT, A, B>).ToPipe());
    
    /// <summary>
    /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, A, B, R> mapM<RT, A, B, R>(Func<A, Transducer<Unit, Sum<Error, B>>> f) where RT : HasIO<RT, Error> =>
        cat<RT, A, R>()
           .ForEach(a => lift<RT, A, B, B>(f(a)).Bind(yield<RT, A, B>).ToPipe());
        
    /// <summary>
    /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, A, A, Unit> mapM<RT, A>(Func<A, Eff<RT, A>> f) where RT : HasIO<RT, Error> =>
        cat<RT, A, Unit>()
           .ForEach(a => lift<RT, A, A, A>(f(a)).Bind(yield<RT, A, A>).ToPipe());
        
    /// <summary>
    /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, A, A, Unit> mapM<RT, A>(Func<A, Transducer<RT, A>> f) where RT : HasIO<RT, Error> =>
        cat<RT, A, Unit>()
           .ForEach(a => lift<RT, A, A, A>(f(a)).Bind(yield<RT, A, A>).ToPipe());
        
    /// <summary>
    /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, A, A, Unit> mapM<RT, A>(Func<A, Transducer<Unit, A>> f) where RT : HasIO<RT, Error> =>
        cat<RT, A, Unit>()
           .ForEach(a => lift<RT, A, A, A>(f(a)).Bind(yield<RT, A, A>).ToPipe());
        
    /// <summary>
    /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, A, A, Unit> mapM<RT, A>(Func<A, Transducer<RT, Sum<Error, A>>> f) where RT : HasIO<RT, Error> =>
        cat<RT, A, Unit>()
           .ForEach(a => lift<RT, A, A, A>(f(a)).Bind(yield<RT, A, A>).ToPipe());
        
    /// <summary>
    /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, A, A, Unit> mapM<RT, A>(Func<A, Transducer<Unit, Sum<Error, A>>> f) where RT : HasIO<RT, Error> =>
        cat<RT, A, Unit>()
           .ForEach(a => lift<RT, A, A, A>(f(a)).Bind(yield<RT, A, A>).ToPipe());

    /// <summary>
    /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, A, B, R> mapM<RT, A, B, R>(Func<A, Eff<B>> f) where RT : HasIO<RT, Error> =>
        cat<RT, A, R>()
           .ForEach(a => lift<RT, A, B, B>(f(a)).Bind(yield<RT, A, B>).ToPipe());
 
    /// <summary>
    /// Lift the IO monad into the Pipe monad transformer (a specialism of the Proxy monad transformer)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, A, B, R> lift<RT, A, B, R>(Eff<R> ma) where RT : HasIO<RT, Error> =>
        lift<RT, Unit, A, Unit, B, R>(ma).ToPipe(); 
 
    /// <summary>
    /// Lift the IO monad into the Pipe monad transformer (a specialism of the Proxy monad transformer)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, A, B, R> lift<RT, A, B, R>(Transducer<RT, R> ma) where RT : HasIO<RT, Error> =>
        lift<RT, Unit, A, Unit, B, R>(ma).ToPipe(); 
 
    /// <summary>
    /// Lift the IO monad into the Pipe monad transformer (a specialism of the Proxy monad transformer)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, A, B, R> lift<RT, A, B, R>(Transducer<RT, Sum<Error, R>> ma) where RT : HasIO<RT, Error> =>
        lift<RT, Unit, A, Unit, B, R>(ma).ToPipe(); 
 
    /// <summary>
    /// Lift the IO monad into the Pipe monad transformer (a specialism of the Proxy monad transformer)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, A, B, R> lift<RT, A, B, R>(Transducer<Unit, R> ma) where RT : HasIO<RT, Error> =>
        lift<RT, Unit, A, Unit, B, R>(ma).ToPipe(); 
 
    /// <summary>
    /// Lift the IO monad into the Pipe monad transformer (a specialism of the Proxy monad transformer)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, A, B, R> lift<RT, A, B, R>(Transducer<Unit, Sum<Error, R>> ma) where RT : HasIO<RT, Error> =>
        lift<RT, Unit, A, Unit, B, R>(ma).ToPipe(); 
 
    /// <summary>
    /// Lift the IO monad into the Pipe monad transformer (a specialism of the Proxy monad transformer)
    /// </summary>
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, A, B, R> lift<RT, A, B, R>(Eff<RT, R> ma) where RT : HasIO<RT, Error> =>
        lift<RT, Unit, A, Unit, B, R>(ma).ToPipe(); 

    /// <summary>
    /// Folds values coming down-stream, when the predicate returns false the folded value is yielded 
    /// </summary>
    /// <param name="Initial">Initial state</param>
    /// <param name="Fold">Fold operation</param>
    /// <param name="WhileState">Predicate</param>
    /// <returns>A pipe that folds</returns>
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, IN, OUT, Unit> foldWhile<RT, IN, OUT>(OUT Initial, Func<OUT, IN, OUT> Fold, Func<OUT, bool> WhileState) 
        where RT : HasIO<RT, Error> =>
        foldUntil<RT, IN, OUT>(Initial, Fold, x => !WhileState(x));
 
    /// <summary>
    /// Folds values coming down-stream, when the predicate returns true the folded value is yielded 
    /// </summary>
    /// <param name="Initial">Initial state</param>
    /// <param name="Fold">Fold operation</param>
    /// <param name="UntilState">Predicate</param>
    /// <returns>A pipe that folds</returns>
    public static Pipe<RT, IN, OUT, Unit> foldUntil<RT, IN, OUT>(OUT Initial, Func<OUT, IN, OUT> Fold, Func<OUT, bool> UntilState)
        where RT : HasIO<RT, Error>
    {
        var state = Initial;
        return awaiting<RT, IN, OUT>()
                   .Bind(x =>
                         {
                             state = Fold(state, x);
                             if (UntilState(state))
                             {
                                 var nstate = state;
                                 state = Initial;
                                 return yield<RT, IN, OUT>(nstate);
                             }
                             else
                             {
                                 return Pure<RT, IN, OUT, Unit>(unit);
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
    public static Pipe<RT, IN, OUT, Unit> foldWhile<RT, IN, OUT>(OUT Initial, Func<OUT, IN, OUT> Fold, Func<IN, bool> WhileValue) 
        where RT : HasIO<RT, Error> =>
        foldUntil<RT, IN, OUT>(Initial, Fold, x => !WhileValue(x));
 
    /// <summary>
    /// Folds values coming down-stream, when the predicate returns true the folded value is yielded 
    /// </summary>
    /// <param name="Initial">Initial state</param>
    /// <param name="Fold">Fold operation</param>
    /// <param name="UntilValue">Predicate</param>
    /// <returns>A pipe that folds</returns>
    public static Pipe<RT, IN, OUT, Unit> foldUntil<RT, IN, OUT>(OUT Initial, Func<OUT, IN, OUT> Fold, Func<IN, bool> UntilValue)
        where RT : HasIO<RT, Error>
    {
        var state = Initial;
        return awaiting<RT, IN, OUT>()
                   .Bind(x =>
                         {
                             if (UntilValue(x))
                             {
                                 var nstate = state;
                                 state = Initial;
                                 return yield<RT, IN, OUT>(nstate);
                             }
                             else
                             {
                                 state = Fold(state, x);
                                 return Pure<RT, IN, OUT, Unit>(unit);
                             }
                         })
                   .ToPipe();
    }

    /// <summary>
    /// Strict left scan
    /// </summary>
    public static Pipe<RT, IN, OUT, Unit> scan<RT, IN, OUT, S>(Func<S, IN, S> Step, S Begin, Func<S, OUT> Done)
        where RT : HasIO<RT, Error>
    {
        return go(Begin);

        Pipe<RT, IN, OUT, Unit> go(S x) =>
            from _ in yield<RT, IN, OUT>(Done(x))
            from a in awaiting<RT, IN, OUT>()
            let x1 = Step(x, a)
            from r in go(x1)
            select r;
    }
     
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Obsolete
    //

    /// <summary>
    /// Lift the IO monad into the Pipe monad transformer (a specialism of the Proxy monad transformer)
    /// </summary>
    [Obsolete(Change.UseEffMonadInsteadOfAff)]
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, A, B, R> lift<RT, A, B, R>(Aff<R> ma) where RT : HasIO<RT, Error> =>
        lift<RT, Unit, A, Unit, B, R>(ma).ToPipe(); 

    /// <summary>
    /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
    /// </summary>
    [Obsolete(Change.UseEffMonadInsteadOfAff)]
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, A, B, R> mapM<RT, A, B, R>(Func<A, Aff<RT, B>> f) where RT : HasIO<RT, Error> =>
        cat<RT, A, R>()
           .ForEach(a => lift<RT, A, B, B>(f(a)).Bind(yield<RT, A, B>)
                                                .ToPipe());

    /// <summary>
    /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
    /// </summary>
    [Obsolete(Change.UseEffMonadInsteadOfAff)]
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, A, A, Unit> mapM<RT, A>(Func<A, Aff<RT, A>> f) where RT : HasIO<RT, Error> =>
        cat<RT, A, Unit>()
           .ForEach(a => lift<RT, A, A, A>(f(a)).Bind(yield<RT, A, A>).ToPipe());

    /// <summary>
    /// Apply a monadic function to all values flowing downstream (not the bound value as is usual with Map)
    /// </summary>
    [Obsolete(Change.UseEffMonadInsteadOfAff)]
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, A, B, R> mapM<RT, A, B, R>(Func<A, Aff<B>> f) where RT : HasIO<RT, Error> =>
        cat<RT, A, R>()
           .ForEach(a => lift<RT, A, B, B>(f(a)).Bind(yield<RT, A, B>)
                                                .ToPipe());

    /// <summary>
    /// Lift the IO monad into the Pipe monad transformer (a specialism of the Proxy monad transformer)
    /// </summary>
    [Obsolete(Change.UseEffMonadInsteadOfAff)]
    [Pure, MethodImpl(mops)]
    public static Pipe<RT, A, B, R> lift<RT, A, B, R>(Aff<RT, R> ma) where RT : HasIO<RT, Error> =>
        lift<RT, Unit, A, Unit, B, R>(ma.ToTransducer()).ToPipe(); 

}
