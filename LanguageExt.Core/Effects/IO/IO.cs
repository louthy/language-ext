using System;
using LanguageExt.Common;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.Effects;
using LanguageExt.Effects.Traits;
using LanguageExt.HKT;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// Transducer based IO monad
/// </summary>
/// <typeparam name="RT">Runtime struct</typeparam>
/// <typeparam name="E">Error value type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public readonly struct IO<E, A> : KArr<Any, MinRT<E>, Sum<E, A>>
{
    /// <summary>
    /// Underlying transducer that captures all of the IO behaviour 
    /// </summary>
    readonly IO<MinRT<E>, E, A> effect;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Constructors
    //
    
    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    internal IO(IO<MinRT<E>, E, A> effect) =>
        this.effect = effect;
    
    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    internal IO(Transducer<MinRT<E>, Sum<E, A>> thunk) =>
        effect = new IO<MinRT<E>, E, A>(thunk);

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    IO(Func<MinRT<E>, Sum<E, A>> thunk)
        : this(lift(thunk))
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    IO(Func<MinRT<E>, A> thunk) 
        : this(rt => Sum<E, A>.Right(thunk(rt)))
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    IO(Transducer<MinRT<E>, A> thunk) 
        : this(Transducer.compose(thunk, Transducer.mkRight<E, A>()))
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    IO(Func<MinRT<E>, Either<E, A>> thunk) 
        : this(rt => thunk(rt).ToSum())
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    IO(Transducer<MinRT<E>, Either<E, A>> thunk) 
        : this(Transducer.compose(thunk, lift<Either<E, A>, Sum<E, A>>(x => x.ToSum())))
    { }
    
    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    internal IO(Transducer<Unit, Sum<E, A>> thunk) 
        : this (Transducer.compose(Transducer.constant<MinRT<E>, Unit>(default), thunk))
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    IO(Func<Sum<E, A>> thunk)
        : this (lift(thunk))
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    IO(Func<A> thunk) 
        : this(_ => Sum<E, A>.Right(thunk()))
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    IO(Transducer<Unit, A> thunk) 
        : this(Transducer.compose(thunk, Transducer.mkRight<E, A>()))
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    IO(Func<Either<E, A>> thunk) 
        : this(_ => thunk().ToSum())
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    IO(Transducer<Unit, Either<E, A>> thunk) 
        : this(Transducer.compose(thunk, lift<Either<E, A>, Sum<E, A>>(x => x.ToSum())))
    { }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Transducer
    //

    /// <summary>
    /// Access to the underlying transducer
    /// </summary>
    [Pure]
    public Transducer<MinRT<E>, Sum<E, A>> Morphism =>
        effect.Morphism;
    
    /// <summary>
    /// Reduction of the underlying transducer
    /// </summary>
    /// <param name="reduce">Reducer </param>
    /// <typeparam name="S"></typeparam>
    /// <returns></returns>
    [Pure, MethodImpl(Opt.Default)]
    public Reducer<MinRT<E>, S> Transform<S>(Reducer<Sum<E, A>, S> reduce) => 
        Morphism.Transform(reduce);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Invoking
    //
    
    /// <summary>
    /// All IO monads will catch exceptions at their point of invocation (i.e. in the `Run*` methods).
    /// But they do not catch exceptions elsewhere without explicitly using this `Try()` method.
    ///
    /// This wraps the `IO` monad in a try/catch that converts exceptional errors to an `E` and 
    /// therefore puts the `IO` in a `Fail` state the expression that can then be matched upon.
    /// </summary>
    /// <remarks>
    /// Useful when you want exceptions to be dealt with through matching/bi-mapping/etc.
    /// and not merely be caught be the exception handler in `Run*`.
    /// </remarks>
    /// <remarks>
    /// This is used automatically for the first argument in the coalescing `|` operator so that any
    /// exceptional failures, in the first argument, allow the second argument to be invoked. 
    /// </remarks>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, A> Try() =>
        new(effect.Try());

    /// <summary>
    /// Invoke the effect
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Either<E, A> Run(MinRT<E> env) =>
        new(effect.Run(env));

    /// <summary>
    /// Invoke the effect
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Either<E, A> Run() =>
        Run(new MinRT<E>());

    /// <summary>
    /// Invoke the effect (which could produce multiple values).  Run a reducer for
    /// each value yielded.
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Fin<S> RunMany<S>(MinRT<E> env, S initialState, Func<S, Either<E, A>, TResult<S>> reducer) =>
        effect.RunMany(env, initialState, reducer);

    /// <summary>
    /// Invoke the effect (which could produce multiple values).  Run a reducer for
    /// each value yielded.
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Fin<S> RunMany<S>(S initialState, Func<S, Either<E, A>, TResult<S>> reducer) =>
        effect.RunMany(new MinRT<E>(), initialState, reducer);

    /// <summary>
    /// Invoke the effect (which could produce multiple values).  Collect those results in a `Seq`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Either<E, Seq<A>> RunMany(MinRT<E> env) =>
        effect.RunMany(env);

    /// <summary>
    /// Invoke the effect (which could produce multiple values).  Collect those results in a `Seq`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Either<E, Seq<A>> RunMany() =>
        effect.RunMany(new MinRT<E>());

    /// <summary>
    /// Invoke the effect asynchronously
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Task<Either<E, A>> RunAsync(MinRT<E> env) =>
        effect.RunAsync(env);

    /// <summary>
    /// Invoke the effect (which could produce multiple values).  Run a reducer for
    /// each value yielded.
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Task<Fin<S>> RunManyAsync<S>(MinRT<E> env, S initialState, Func<S, Either<E, A>, TResult<S>> reducer) =>
        effect.RunManyAsync(env, initialState, reducer);

    /// <summary>
    /// Invoke the effect (which could produce multiple values).  Run a reducer for
    /// each value yielded.
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Task<Fin<S>> RunManyAsync<S>(S initialState, Func<S, Either<E, A>, TResult<S>> reducer) =>
        effect.RunManyAsync(new MinRT<E>(), initialState, reducer);

    /// <summary>
    /// Invoke the effect (which could produce multiple values).  Collect those results in a `Seq`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Task<Either<E, Seq<A>>> RunManyAsync(MinRT<E> env) =>
        effect.RunManyAsync(env);

    /// <summary>
    /// Invoke the effect (which could produce multiple values).  Collect those results in a `Seq`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Task<Either<E, Seq<A>>> RunManyAsync() =>
        effect.RunManyAsync(new MinRT<E>());
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Timeout
    //

    /// <summary>
    /// Cancel the operation if it takes too long
    /// </summary>
    /// <param name="timeoutDelay">Timeout period</param>
    /// <returns>An IO operation that will timeout if it takes too long</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, A> Timeout(TimeSpan timeoutDelay) =>
        new(effect.Timeout(timeoutDelay));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Lifting
    //
    
    /// <summary>
    /// Lift a value into the IO monad 
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> Pure(A value) =>
        new (_ => Sum<E, A>.Right(value));

    /// <summary>
    /// Lift a failure into the IO monad 
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> Fail(E error) =>
        new (_ => Sum<E, A>.Left(error));

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> Lift(Func<MinRT<E>, Either<E, A>> f) =>
        new (f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> Lift(Func<Either<E, A>> f) =>
        new (f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> Lift(Transducer<MinRT<E>, Either<E, A>> f) =>
        new (f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> Lift(Transducer<Unit, Either<E, A>> f) =>
        new (f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> Lift(Func<MinRT<E>, Sum<E, A>> f) =>
        new (f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> Lift(Func<Sum<E, A>> f) =>
        new (f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> Lift(Transducer<MinRT<E>, Sum<E, A>> f) =>
        new (f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> Lift(Transducer<Unit, Sum<E, A>> f) =>
        new (f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> Lift(Func<MinRT<E>, A> f) =>
        new (f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> Lift(Func<A> f) =>
        new (f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> Lift(Transducer<MinRT<E>, A> f) =>
        new (f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> Lift(Transducer<Unit, A> f) =>
        new (f);

    /// <summary>
    /// Lift a asynchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> LiftIO(Func<MinRT<E>, Task<A>> f) =>
        new(IO<MinRT<E>, E, A>.LiftIO(f));

    /// <summary>
    /// Lift a asynchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> LiftIO(Func<MinRT<E>, Task<Sum<E, A>>> f) =>
        new(IO<MinRT<E>, E, A>.LiftIO(f));

    /// <summary>
    /// Lift a asynchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> LiftIO(Func<MinRT<E>, Task<Either<E, A>>> f) =>
        new(IO<MinRT<E>, E, A>.LiftIO(f));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Memoisation and tail calls
    //
    
    /// <summary>
    /// Memoise the result, so subsequent calls don't invoke the side-effect
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, A> Memo() =>
        new(effect.Memo());

    /// <summary>
    /// Wrap this around the final `from` call in a `IO` LINQ expression to void a recursion induced space-leak.
    /// </summary>
    /// <example>
    /// 
    ///     IO<MinRT, E, A> recursive(int x) =>
    ///         from x in writeLine(x)
    ///         from r in tail(recursive(x + 1))
    ///         select r;      <--- this never runs
    /// 
    /// </example>
    /// <remarks>
    /// This means the result of the LINQ expression comes from the final `from`, _not_ the `select.  If the
    /// type of the `final` from differs from the type of the `select` then this has no effect.
    /// </remarks>
    /// <remarks>
    /// Background: When making recursive LINQ expressions, the final `select` is problematic because it means
    /// there's code to run _after_ the final `from` expression.  This means there's you're guaranteed to have a
    /// space-leak due to the need to hold thunks to the final `select` on every recursive step.
    ///
    /// This function ignores the `select` altogether and says that the final `from` is where we get our return
    /// result from and therefore there's no need to hold the thunk. 
    /// </remarks>
    /// <returns>IO operation that's marked ready for tail recursion</returns>        
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, A> Tail() =>
        new(effect.Tail());
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // 
    // Forking
    //

    /// <summary>
    /// Queue this IO operation to run on the thread-pool. 
    /// </summary>
    /// <param name="timeout">Maximum time that the forked IO operation can run for. `None` for no timeout.</param>
    /// <returns>Returns a `ForkIO` data-structure that contains two IO effects that can be used to either cancel
    /// the forked IO operation or to await the result of it.
    /// </returns>
    [MethodImpl(Opt.Default)]
    public IO<E, ForkIO<E, A>> Fork(Option<TimeSpan> timeout = default) =>
        new(Transducer.fork(Morphism, timeout).Map(TFork.ToIO));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Map and map-left
    //

    /// <summary>
    /// Maps the IO monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, B> Map<B>(Func<A, B> f) =>
        new(effect.Map(f));

    /// <summary>
    /// Maps the IO monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, B> Select<B>(Func<A, B> f) =>
        new(effect.Map(f));

    /// <summary>
    /// Maps the IO monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, B> Map<B>(Transducer<A, B> f) =>
        new(effect.Map(f));

    /// <summary>
    /// Maps the IO monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public  IO<E, A> MapFail(Func<E, E> f) =>
        new(effect.MapFail(f));

    /// <summary>
    /// Maps the IO monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public  IO<E, A> MapFail(Transducer<E, E> f) =>
        new(effect.MapFail(f));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Bi-map
    //

    /// <summary>
    /// Mapping of either the Success state or the Failure state depending on what
    /// state this IO monad is in.  
    /// </summary>
    /// <param name="Succ">Mapping to use if the IO monad is in a success state</param>
    /// <param name="Fail">Mapping to use if the IO monad is in a failure state</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, B> BiMap<B>(Func<A, B> Succ, Func<E, E> Fail) =>
        new(effect.BiMap(Succ, Fail));

    /// <summary>
    /// Mapping of either the Success state or the Failure state depending on what
    /// state this IO monad is in.  
    /// </summary>
    /// <param name="Succ">Mapping to use if the IO monad is in a success state</param>
    /// <param name="Fail">Mapping to use if the IO monad is in a failure state</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, B> BiMap<B>(Transducer<A, B> Succ, Transducer<E, E> Fail) =>
        new(effect.BiMap(Succ, Fail));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Matching
    //

    /// <summary>
    /// Pattern match the success or failure values and collapse them down to a success value
    /// </summary>
    /// <param name="Succ">Success value mapping</param>
    /// <param name="Fail">Failure value mapping</param>
    /// <returns>IO in a success state</returns>
    [Pure]
    public IO<E, B> Match<B>(Func<A, B> Succ, Func<E, B> Fail) =>
        new(effect.Match(Succ, Fail));

    /// <summary>
    /// Pattern match the success or failure values and collapse them down to a success value
    /// </summary>
    /// <param name="Succ">Success value mapping</param>
    /// <param name="Fail">Failure value mapping</param>
    /// <returns>IO in a success state</returns>
    [Pure]
    public IO<E, B> Match<B>(Transducer<A, B> Succ, Transducer<E, B> Fail) =>
        new(effect.Match(Succ, Fail));

    /// <summary>
    /// Map the failure to a success value
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO in a success state</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, A> IfFail(Func<E, A> Fail) =>
        new(effect.IfFail(Fail));

    /// <summary>
    /// Map the failure to a success value
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO in a success state</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, A> IfFail(Transducer<E, A> Fail) =>
        new(effect.IfFail(Fail));

    /// <summary>
    /// Map the failure to a success value
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO in a success state</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, A> IfFail(A Fail) =>
        new(effect.IfFail(Fail));

    /// <summary>
    /// Map the failure to a new IO effect
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO that encapsulates that IfFail</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, A> IfFailIO(Func<E, IO<E, A>> Fail) =>
        new(Transducer.bimap(
                Morphism,
                e => Fail(e).Morphism,
                x => Transducer.constant<MinRT<E>, Sum<E, A>>(Sum<E, A>.Right(x)))
            .Flatten());

    /// <summary>
    /// Map the failure to a new IO effect
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO that encapsulates that IfFail</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, A> IfFailIO(IO<E, A> Fail) =>
        IfFailIO(_ => Fail);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Filter
    //

    /// <summary>
    /// Only allow values through the effect if the predicate returns `true` for the bound value
    /// </summary>
    /// <param name="predicate">Predicate to apply to the bound value></param>
    /// <returns>Filtered IO</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, A> Filter(Func<A, bool> predicate) =>
        new(effect.Filter(predicate));

    /// <summary>
    /// Only allow values through the effect if the predicate returns `true` for the bound value
    /// </summary>
    /// <param name="predicate">Predicate to apply to the bound value></param>
    /// <returns>Filtered IO</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, A> Filter(Transducer<A, bool> predicate) =>
        new(effect.Filter(predicate));

    /// <summary>
    /// Only allow values through the effect if the predicate returns `true` for the bound value
    /// </summary>
    /// <param name="predicate">Predicate to apply to the bound value></param>
    /// <returns>Filtered IO</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, A> Where(Func<A, bool> predicate) =>
        new(effect.Filter(predicate));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monadic binding
    //

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, B> Bind<B>(Func<A, IO<E, B>> f) =>
        new(Transducer.bind(Morphism, x => f(x).Morphism));

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// transducer provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the transducer provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, B> Bind<B>(Transducer<A, IO<E, B>> f) =>
        Map(f).Flatten();

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, B> Bind<B>(Func<A, Pure<B>> f) =>
        new(effect.Bind(f));

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, A> Bind(Func<A, Fail<E>> f) =>
        new(effect.Bind(f));
    
    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, B> Bind<B>(Func<A, Transducer<Unit, B>> f) =>
        new(effect.Bind(f));
    
    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, B> Bind<B>(Func<A, Transducer<MinRT<E>, B>> f) =>
        new(effect.Bind(f));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monadic binding and projection
    //

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, C> SelectMany<B, C>(Func<A, IO<E, B>> bind, Func<A, B, C> project) =>
        new(Transducer.selectMany(Morphism, x => bind(x).Morphism, project));

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        new(effect.SelectMany(bind, project));

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, C> SelectMany<B, C>(Func<A, Fail<E>> bind, Func<A, B, C> project) =>
        new(effect.SelectMany(bind, project));

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, C> SelectMany<C>(Func<A, Guard<E, Unit>> bind, Func<A, Unit, C> project) =>
        new(effect.SelectMany(bind, project));

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, C> SelectMany<C>(Func<A, Guard<Fail<E>, Unit>> bind, Func<A, Unit, C> project) =>
        new(effect.SelectMany(bind, project));

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, C> SelectMany<B, C>(Func<A, Transducer<Unit, B>> bind, Func<A, B, C> project) =>
        new(effect.SelectMany(bind, project));

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, C> SelectMany<B, C>(Func<A, Transducer<MinRT<E>, B>> bind, Func<A, B, C> project) =>
        new(effect.SelectMany(bind, project));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Folding
    //

    /// <summary>
    /// Fold the effect forever or until the schedule expires
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, S> Fold<S>(
        Schedule schedule, 
        S initialState,
        Func<S, A, S> folder) =>
        new(effect.Fold(schedule, initialState, folder));
    
    /// <summary>
    /// Fold the effect forever
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, S> Fold<S>(
        S initialState,
        Func<S, A, S> folder) =>
        new(effect.Fold(initialState, folder));

    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, S> FoldUntil<S>(
        Schedule schedule, 
        S initialState,
        Func<S, A, S> folder, 
        Func<(S State, A Value), bool> predicate) =>
        new(effect.FoldUntil(schedule, initialState, folder, predicate));
    
    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, S> FoldUntil<S>(
        Schedule schedule, 
        S initialState,
        Func<S, A, S> folder, 
        Func<S, bool> stateIs) =>
        new(effect.FoldUntil(schedule, initialState, folder, stateIs));

    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, S> FoldUntil<S>(
        Schedule schedule, 
        S initialState,
        Func<S, A, S> folder, 
        Func<A, bool> valueIs) =>
        new(effect.FoldUntil(schedule, initialState, folder, valueIs));

    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, S> FoldUntil<S>(
        S initialState,
        Func<S, A, S> folder, 
        Func<(S State, A Value), bool> predicate) =>
        new(effect.FoldUntil(initialState, folder, predicate));
    
    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, S> FoldUntil<S>(
        S initialState,
        Func<S, A, S> folder, 
        Func<S, bool> stateIs) =>
        new(effect.FoldUntil(initialState, folder, stateIs));

    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, S> FoldUntil<S>(
        S initialState,
        Func<S, A, S> folder, 
        Func<A, bool> valueIs) =>
        new(effect.FoldUntil(initialState, folder, valueIs));

    /// <summary>
    /// Fold the effect while the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, S> FoldWhile<S>(
        Schedule schedule, 
        S initialState,
        Func<S, A, S> folder, 
        Func<(S State, A Value), bool> predicate) =>
        new(effect.FoldWhile(schedule, initialState, folder, predicate));
    
    /// <summary>
    /// Fold the effect while the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, S> FoldWhile<S>(
        Schedule schedule, 
        S initialState,
        Func<S, A, S> folder, 
        Func<S, bool> stateIs) =>
        new(effect.FoldWhile(schedule, initialState, folder, stateIs));

    /// <summary>
    /// Fold the effect while the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, S> FoldWhile<S>(
        Schedule schedule, 
        S initialState,
        Func<S, A, S> folder, 
        Func<A, bool> valueIs) =>
        new(effect.FoldWhile(schedule, initialState, folder, valueIs));

    /// <summary>
    /// Fold the effect while the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, S> FoldWhile<S>(
        S initialState,
        Func<S, A, S> folder, 
        Func<(S State, A Value), bool> predicate) =>
        new(effect.FoldWhile(initialState, folder, predicate));
    
    /// <summary>
    /// Fold the effect while the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, S> FoldWhile<S>(
        S initialState,
        Func<S, A, S> folder, 
        Func<S, bool> stateIs) =>
        new(effect.FoldWhile(initialState, folder, stateIs));

    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, S> FoldWhile<S>(
        S initialState,
        Func<S, A, S> folder, 
        Func<A, bool> valueIs) =>
        new(effect.FoldWhile(initialState, folder, valueIs));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Folding, where each fold is monadic
    //

    /// <summary>
    /// Fold the effect forever or until the schedule expires
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, S> FoldM<S>(
        Schedule schedule, 
        S initialState,
        Func<S, A, IO<E, S>> folder) =>
        new(effect.FoldM(schedule, initialState, (s, x) => folder(s, x).Morphism));

    /// <summary>
    /// Fold the effect forever
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, S> FoldM<S>(
        S initialState,
        Func<S, A, IO<E, S>> folder) =>
        new(effect.FoldM(initialState, (s, x) => folder(s, x).Morphism));

    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, S> FoldUntilM<S>(
        Schedule schedule, 
        S initialState,
        Func<S, A, IO<E, S>> folder, 
        Func<A, bool> valueIs) =>
        new(effect.FoldUntilM(schedule, initialState, (s, x) => folder(s, x).Morphism, valueIs));

    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, S> FoldUntilM<S>(
        S initialState,
        Func<S, A, IO<E, S>> folder, 
        Func<A, bool> valueIs) =>
        new(effect.FoldUntilM(initialState, (s, x) => folder(s, x).Morphism, valueIs));

    /// <summary>
    /// Fold the effect while the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, S> FoldWhileM<S>(
        Schedule schedule, 
        S initialState,
        Func<S, A, IO<E, S>> folder, 
        Func<A, bool> valueIs) =>
        new(effect.FoldWhileM(schedule, initialState, (s, x) => folder(s, x).Morphism, valueIs));

    /// <summary>
    /// Fold the effect while the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, S> FoldWhileM<S>(
        S initialState,
        Func<S, A, IO<E, S>> folder, 
        Func<A, bool> valueIs) =>
        new(effect.FoldWhileM(initialState, (s, x) => folder(s, x).Morphism, valueIs));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Synchronisation between contexts
    //
    
    /// <summary>
    /// Make a transducer run on the `SynchronizationContext` that was captured at the start
    /// of an `Invoke` call.
    /// </summary>
    /// <remarks>
    /// The transducer receives its input value from the currently running sync-context and
    /// then proceeds to run its operation in the captured `SynchronizationContext`:
    /// typically a UI context, but could be any captured context.  The result of the
    /// transducer is the received back on the currently running sync-context. 
    /// </remarks>
    /// <param name="f">Transducer</param>
    /// <returns></returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<E, A> Post() =>
        new(effect.Post());        
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Conversion
    //
    
    /// <summary>
    /// Natural transformation to an `IO` monad
    /// </summary>
    [Pure]
    public IO<MinRT<E>, E, A> As =>
        effect;

    /// <summary>
    /// Convert to an `IO` monad that has a runtime
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, A> WithRuntime<RT>() where RT : HasIO<RT, E> =>
        IO<RT, E, A>.Lift(Transducer.compose(MinRT<E>.convert<RT>(), Morphism));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Operators
    //

    /// <summary>
    /// Convert to an IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator IO<E, A>(Pure<A> ma) =>
        ma.ToIO<E>();

    /// <summary>
    /// Convert to an IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator IO<E, A>(Fail<E> ma) =>
        ma.ToIO<A>();

    /// <summary>
    /// Convert to an `IO` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator IO<E, A>(in Either<E, A> ma) =>
        ma.Match(Right: Pure, Left: Fail);

    /// <summary>
    /// Convert to an IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator IO<E, A>(Transducer<MinRT<E>, Sum<E, A>> ma) =>
        Lift(ma);

    /// <summary>
    /// Convert to an IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator IO<E, A>(Transducer<MinRT<E>, Either<E, A>> ma) =>
        Lift(ma);

    /// <summary>
    /// Convert to an IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator IO<E, A>(Transducer<Unit, Sum<E, A>> ma) =>
        Lift(ma);

    /// <summary>
    /// Convert to an IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator IO<E, A>(Transducer<Unit, Either<E, A>> ma) =>
        Lift(ma);

    /// <summary>
    /// Convert to an IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator IO<E, A>(Transducer<MinRT<E>, A> ma) =>
        Lift(ma);

    /// <summary>
    /// Convert to an IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator IO<E, A>(Transducer<Unit, A> ma) =>
        Lift(ma);
    
    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> operator |(IO<E, A> ma, IO<E, A> mb) =>
        new(ma.effect | mb.effect);

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> operator |(IO<E, A> ma, CatchError<E> mb) =>
        new(ma.effect | mb);

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> operator |(IO<E, A> ma, CatchValue<E, A> mb) =>
        new(Transducer.@try(
            ma.Morphism,
            mb.Match,
            Transducer.compose(lift(mb.Value), Transducer.mkRight<E, A>())));
    
    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> operator |(IO<E, A> ma, IOCatch<MinRT<E>, E, A> mb) =>
        new(ma.effect | mb);
    
    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> operator |(IO<E, A> ma, IOCatch<E, A> mb) =>
        new(ma.effect | mb.As);

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> operator |(IO<E, A> ma, Transducer<MinRT<E>, A> mb) =>
        new(ma.effect | mb);

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> operator |(IO<E, A> ma, Transducer<MinRT<E>, E> mb) =>
        new(ma.effect | mb);

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> operator |(IO<E, A> ma, Transducer<Unit, A> mb) =>
        new(ma.effect | mb);

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> operator |(IO<E, A> ma, Transducer<Unit, E> mb) =>
        new(ma.effect | mb);

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> operator |(IO<E, A> ma, Transducer<MinRT<E>, Sum<E, A>> mb) =>
        new(ma.effect | mb);

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> operator |(IO<E, A> ma, Transducer<Unit, Sum<E, A>> mb) =>
        new(ma.effect | mb);

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> operator |(IO<E, A> ma, Transducer<MinRT<E>, Sum<Error, A>> mb) =>
        new(ma.effect | mb);

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> operator |(IO<E, A> ma, Transducer<Unit, Sum<Error, A>> mb) =>
        new(ma.effect | mb);

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> operator |(Transducer<MinRT<E>, A> ma, IO<E, A> mb) =>
        new(ma | mb.effect);

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> operator |(Transducer<MinRT<E>, E> ma, IO<E, A> mb) =>
        new(ma | mb.effect);

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> operator |(Transducer<Unit, A> ma, IO<E, A> mb) =>
        new(ma | mb.effect);

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> operator |(Transducer<Unit, E> ma, IO<E, A> mb) =>
        new(ma | mb.effect);

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> operator |(Transducer<MinRT<E>, Sum<E, A>> ma, IO<E, A>  mb) =>
        new(ma | mb.effect);

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> operator |(Transducer<Unit, Sum<E, A>> ma, IO<E, A> mb) =>
        new(ma | mb.effect);

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> operator |(Transducer<MinRT<E>, Sum<Error, A>> ma, IO<E, A> mb) =>
        new(ma | mb.effect);

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<E, A> operator |(Transducer<Unit, Sum<Error, A>> ma, IO<E, A> mb) =>
        new(ma | mb.effect);
}
