/*
using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Effects.Traits;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Transducer based IO monad
/// </summary>
/// <typeparam name="RT">Runtime struct</typeparam>
/// <typeparam name="E">Error value type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public readonly struct IO<RT, E, A> : K<IO<E>.Runtime<RT>, A>
    where RT : HasIO<RT, E>
{
    /// <summary>
    /// Underlying transducer that captures all of the IO behaviour 
    /// </summary>
    readonly Transducer<RT, Sum<E, A>> thunk;
    
    public K<IO<E>.Runtime<RT>, A>  this;

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Constructors
    //
    
    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    internal IO(Transducer<RT, Sum<E, A>> thunk) =>
        this.thunk = thunk;

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    IO(Func<RT, Sum<E, A>> thunk) =>
        this.thunk = lift(thunk);

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    IO(Func<RT, A> thunk) 
        : this(rt => Sum<E, A>.Right(thunk(rt)))
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    internal IO(Transducer<RT, A> thunk) 
        : this(Transducer.compose(thunk, Transducer.mkRight<E, A>()))
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    IO(Func<RT, Either<E, A>> thunk) 
        : this(rt => thunk(rt).ToSum())
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    IO(Transducer<RT, Either<E, A>> thunk) 
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
    public Transducer<RT, Sum<E, A>> Morphism =>
        thunk ?? Transducer.fail<RT, Sum<E, A>>(Errors.Bottom);
    
    /// <summary>
    /// Reduction of the underlying transducer
    /// </summary>
    /// <param name="reduce">Reducer </param>
    /// <typeparam name="S"></typeparam>
    /// <returns></returns>
    [Pure, MethodImpl(Opt.Default)]
    public Reducer<RT, S> Transform<S>(Reducer<Sum<E, A>, S> reduce) => 
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
    public IO<RT, E, A> Try() =>
        new(Transducer.@try(Morphism, _ => true, Transducer.mkLeft<E, A>()));

    /// <summary>
    /// Invoke the effect
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Either<E, A> Run(RT env) =>
        Morphism.Run1(env, env.CancellationToken, env.SynchronizationContext)
                .ToEither(RT.FromError);

    /// <summary>
    /// Invoke the effect (which could produce multiple values).  Run a reducer for
    /// each value yielded.
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Fin<S> RunMany<S>(RT env, S initialState, Func<S, Either<E, A>, TResult<S>> reducer) =>
        Morphism.Run(
                    env,
                    initialState,
                    Reducer.from<Sum<E, A>, S>(
                        (_, s, sv) => sv switch
                        {
                            SumRight<E, A> r => reducer(s, Either<E, A>.Right(r.Value)),
                            SumLeft<E, A> l => reducer(s, Either<E, A>.Left(l.Value)),
                            _ => TResult.Complete(s)
                        }),
                    env.CancellationToken, 
                    env.SynchronizationContext)
                .ToFin();

    /// <summary>
    /// Invoke the effect (which could produce multiple values).  Collect those results in a `Seq`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Either<E, Seq<A>> RunMany(RT env) =>
        RunMany(env,
                Either<E, Seq<A>>.Right(Seq<A>()),
                (s, v) =>
                    (s.IsRight, v.IsRight) switch
                    {
                        (true, true) => TResult.Continue(Either<E, Seq<A>>.Right(((Seq<A>)s).Add((A)v))),
                        (true, false) => TResult.Complete(Either<E, Seq<A>>.Left((E)v)),
                        _ => TResult.Complete(s)
                    })
            .Match(
                Succ: v => v,
                Fail: RT.FromError);

    /// <summary>
    /// Invoke the effect asynchronously
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Task<Either<E, A>> RunAsync(RT env) =>
        Morphism.Run1Async(env, null, env.CancellationToken, env.SynchronizationContext)
                .Map(r => r.ToEither(RT.FromError));

    /// <summary>
    /// Invoke the effect (which could produce multiple values).  Run a reducer for
    /// each value yielded.
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Task<Fin<S>> RunManyAsync<S>(RT env, S initialState, Func<S, Either<E, A>, TResult<S>> reducer) =>
        Morphism.RunAsync(
                    env,
                    initialState,
                    Reducer.from<Sum<E, A>, S>(
                        (_, s, sv) => sv switch
                        {
                            SumRight<E, A> r => reducer(s, Either<E, A>.Right(r.Value)),
                            SumLeft<E, A> l => reducer(s, Either<E, A>.Left(l.Value)),
                            _ => TResult.Complete(s)
                        }),
                    null,
                    env.CancellationToken,
                    env.SynchronizationContext)
                .Map(r => r.ToFin());

    /// <summary>
    /// Invoke the effect (which could produce multiple values).  Collect those results in a `Seq`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Task<Either<E, Seq<A>>> RunManyAsync(RT env) =>
        RunManyAsync(env,
                Either<E, Seq<A>>.Right(Seq<A>()),
                (s, v) =>
                    (s.IsRight, v.IsRight) switch
                    {
                        (true, true) => TResult.Continue(Either<E, Seq<A>>.Right(((Seq<A>)s).Add((A)v))),
                        (true, false) => TResult.Complete(Either<E, Seq<A>>.Left((E)v)),
                        _ => TResult.Complete(s)
                    })
            .Map(r => r.Match(
                Succ: v => v,
                Fail: RT.FromError));
    
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
    public IO<RT, E, A> Timeout(TimeSpan timeoutDelay)
    {
        var self = this;
        return LiftIO(async rt =>
        {
            using var delayTokSrc = new CancellationTokenSource();
            var lenv       = rt.LocalCancel;
            var delay      = Task.Delay(timeoutDelay, delayTokSrc.Token);
            var task       = self.RunAsync(lenv);
            var completed  = await Task.WhenAny(delay, task).ConfigureAwait(false);

            if (completed == delay)
            {
                try
                {
                    await lenv.CancellationTokenSource.CancelAsync().ConfigureAwait(false);
                }
                catch
                {
                    // The token-source might have already been disposed, so let's ignore that error
                }
                return Left<E, A>(RT.FromError(Errors.TimedOut));
            }
            else
            {
                await delayTokSrc.CancelAsync().ConfigureAwait(false);
                return await task.ConfigureAwait(false);
            }
        });
    }
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Lifting
    //
    
    /// <summary>
    /// Lift a value into the IO monad 
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> Pure(A value) =>
        new (_ => Sum<E, A>.Right(value));

    /// <summary>
    /// Lift a failure into the IO monad 
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> Fail(E error) =>
        new (_ => Sum<E, A>.Left(error));

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> Lift(Func<RT, Either<E, A>> f) =>
        new (f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> Lift(Transducer<RT, Either<E, A>> f) =>
        new (f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> Lift(Func<RT, Sum<E, A>> f) =>
        new (f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> Lift(Transducer<RT, Sum<E, A>> f) =>
        new (f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> Lift(Func<RT, A> f) =>
        new (f);

    /// <summary>
    /// Lift a synchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> Lift(Transducer<RT, A> f) =>
        new (f);

    /// <summary>
    /// Lift a asynchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> LiftIO(Func<RT, Task<A>> f) =>
        new (liftAsync<RT, Sum<E, A>>(
            async (_, rt) => Sum<E, A>.Right(await f(rt).ConfigureAwait(false))));

    /// <summary>
    /// Lift a asynchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> LiftIO(Func<RT, Task<Sum<E, A>>> f) =>
        new(liftAsync<RT, Sum<E, A>>(
            async (_, rt) => await f(rt).ConfigureAwait(false)));

    /// <summary>
    /// Lift a asynchronous effect into the IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> LiftIO(Func<RT, Task<Either<E, A>>> f) =>
        new (liftAsync<RT, Sum<E, A>>(
            async (_, rt) => (await f(rt).ConfigureAwait(false)).ToSum()));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Memoisation and tail calls
    //
    
    /// <summary>
    /// Memoise the result, so subsequent calls don't invoke the side-effect
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, A> Memo() =>
        new(Transducer.memo(Morphism));

    /// <summary>
    /// Wrap this around the final `from` call in a `IO` LINQ expression to void a recursion induced space-leak.
    /// </summary>
    /// <example>
    /// 
    ///     IO<RT, E, A> recursive(int x) =>
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
    public IO<RT, E, A> Tail() =>
        new(tail(Morphism));
    
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
    public IO<RT, E, ForkIO<RT, E, A>> Fork(Option<TimeSpan> timeout = default) =>
        new(Transducer.fork(Morphism, timeout).Map(TFork.ToIO<RT, E, A>));
    
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
    public IO<RT, E, B> Map<B>(Func<A, B> f) =>
        new(Transducer.mapRight(Morphism, f));

    /// <summary>
    /// Maps the IO monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, B> Select<B>(Func<A, B> f) =>
        new(Transducer.mapRight(Morphism, f));

    /// <summary>
    /// Maps the IO monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, B> Map<B>(Transducer<A, B> f) =>
        new(Transducer.mapRight(Morphism, f));

    /// <summary>
    /// Maps the IO monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public  IO<RT, E, A> MapFail(Func<E, E> f) =>
        new(Transducer.mapLeft(Morphism, f));

    /// <summary>
    /// Maps the IO monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public  IO<RT, E, A> MapFail(Transducer<E, E> f) =>
        new(Transducer.mapLeft(Morphism, f));

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
    public IO<RT, E, B> BiMap<B>(Func<A, B> Succ, Func<E, E> Fail) =>
        new(Transducer.bimap(Morphism, Fail, Succ));

    /// <summary>
    /// Mapping of either the Success state or the Failure state depending on what
    /// state this IO monad is in.  
    /// </summary>
    /// <param name="Succ">Mapping to use if the IO monad is in a success state</param>
    /// <param name="Fail">Mapping to use if the IO monad is in a failure state</param>
    /// <returns>Mapped IO monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, B> BiMap<B>(Transducer<A, B> Succ, Transducer<E, E> Fail) =>
        new(Transducer.bimap(Morphism, Fail, Succ));

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
    public IO<RT, E, B> Match<B>(Func<A, B> Succ, Func<E, B> Fail) =>
        Match(lift(Succ), lift(Fail));

    /// <summary>
    /// Pattern match the success or failure values and collapse them down to a success value
    /// </summary>
    /// <param name="Succ">Success value mapping</param>
    /// <param name="Fail">Failure value mapping</param>
    /// <returns>IO in a success state</returns>
    [Pure]
    public IO<RT, E, B> Match<B>(Transducer<A, B> Succ, Transducer<E, B> Fail) =>
        new(Transducer.compose(
                Transducer.bimap(Morphism, Fail, Succ),
                lift<Sum<B, B>, Sum<E, B>>(s => s switch
                {
                    SumRight<B, B> r => Sum<E, B>.Right(r.Value),
                    SumLeft<B, B> l => Sum<E, B>.Right(l.Value),
                    _ => throw new BottomException()
                })));

    /// <summary>
    /// Map the failure to a success value
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO in a success state</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, A> IfFail(Func<E, A> Fail) =>
        IfFail(lift(Fail));

    /// <summary>
    /// Map the failure to a success value
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO in a success state</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, A> IfFail(Transducer<E, A> Fail) =>
        Match(Transducer.identity<A>(), Fail);

    /// <summary>
    /// Map the failure to a success value
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO in a success state</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, A> IfFail(A Fail) =>
        Match(Transducer.identity<A>(), Transducer.constant<E, A>(Fail));

    /// <summary>
    /// Map the failure to a new IO effect
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO that encapsulates that IfFail</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, A> IfFailIO(Func<E, IO<RT, E, A>> Fail) =>
        new(Transducer.bimap(
                Morphism,
                e => Fail(e).Morphism,
                x => Transducer.constant<RT, Sum<E, A>>(Sum<E, A>.Right(x)))
            .Flatten());

    /// <summary>
    /// Map the failure to a new IO effect
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO that encapsulates that IfFail</returns>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, A> IfFailIO(IO<RT, E, A> Fail) =>
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
    public IO<RT, E, A> Filter(Func<A, bool> predicate) =>
        Filter(lift(predicate));

    /// <summary>
    /// Only allow values through the effect if the predicate returns `true` for the bound value
    /// </summary>
    /// <param name="predicate">Predicate to apply to the bound value></param>
    /// <returns>Filtered IO</returns>
    public IO<RT, E, A> Filter(Transducer<A, bool> predicate) =>
        new(Transducer.filter(Morphism, predicate));

    /// <summary>
    /// Only allow values through the effect if the predicate returns `true` for the bound value
    /// </summary>
    /// <param name="predicate">Predicate to apply to the bound value></param>
    /// <returns>Filtered IO</returns>
    public IO<RT, E, A> Where(Func<A, bool> predicate) =>
        Filter(lift(predicate));

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
    public IO<RT, E, B> Bind<B>(Func<A, IO<RT, E, B>> f) =>
        new(Transducer.bind(Morphism, x => f(x).Morphism));

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    public IO<RT, E, B> Bind<B>(Func<A, K<IO<E>.Runtime<RT>, B>> f) =>
        new(Transducer.bind(Morphism, x => f(x).As().Morphism));

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// transducer provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the transducer provided</returns>
    public IO<RT, E, B> Bind<B>(Transducer<A, IO<RT, E, B>> f) =>
        Map(f).Flatten();

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    public IO<RT, E, B> Bind<B>(Func<A, Pure<B>> f) =>
        Bind(x => f(x).ToIO<RT, E>());

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    public IO<RT, E, A> Bind(Func<A, Fail<E>> f) =>
        Bind(x => f(x).ToIO<RT, A>());
    
    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    public IO<RT, E, B> Bind<B>(Func<A, Transducer<Unit, B>> f) =>
        new(Transducer.mapRight(Morphism, lift(f).Flatten()));
    
    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    public IO<RT, E, B> Bind<B>(Func<A, Transducer<RT, B>> f) =>
        Bind(x => IO<RT, E, B>.Lift(f(x)));
    
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
    public IO<RT, E, C> SelectMany<B, C>(Func<A, IO<RT, E, B>> bind, Func<A, B, C> project) =>
        new(Transducer.selectMany(Morphism, x => bind(x).Morphism, project));

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    public IO<RT, E, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).ToIO<RT, E>(), project);

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    public IO<RT, E, C> SelectMany<B, C>(Func<A, Fail<E>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).ToIO<RT, B>(), project);

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    public IO<RT, E, C> SelectMany<C>(Func<A, Guard<E, Unit>> bind, Func<A, Unit, C> project) =>
        Bind(x => bind(x) switch
        {
            { Flag: true } => IO<RT, E, C>.Pure(project(x, default)),
            var g => IO<RT, E, C>.Fail(g.OnFalse())
        });

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    public IO<RT, E, C> SelectMany<C>(Func<A, Guard<Fail<E>, Unit>> bind, Func<A, Unit, C> project) =>
        Bind(x => bind(x) switch
        {
            { Flag: true } => IO<RT, E, C>.Pure(project(x, default)),
            var g => IO<RT, E, C>.Fail(g.OnFalse().Value)
        });

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    public IO<RT, E, C> SelectMany<B, C>(Func<A, Transducer<Unit, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monadic bind operation.  This runs the current IO monad and feeds its result to the
    /// function provided; which in turn returns a new IO monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    public IO<RT, E, C> SelectMany<B, C>(Func<A, Transducer<RT, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Folding
    //

    /// <summary>
    /// Fold the effect forever or until the schedule expires
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, S> Fold<S>(
        Schedule schedule, 
        S initialState,
        Func<S, A, S> folder) =>
        new(Transducer.compose(
            Morphism, 
            Transducer.foldSum<S, E, A>(
                schedule,
                initialState, 
                folder, 
                _ => TResult.Continue(unit))));
    
    /// <summary>
    /// Fold the effect forever
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, S> Fold<S>(
        S initialState,
        Func<S, A, S> folder) =>
        Fold(Schedule.Forever, initialState, folder);

    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, S> FoldUntil<S>(
        Schedule schedule, 
        S initialState,
        Func<S, A, S> folder, 
        Func<(S State, A Value), bool> predicate) =>
        new(Transducer.compose(
            Morphism, 
            Transducer.foldSum<S, E, A>(
                schedule,
                initialState, 
                folder, 
                last => predicate(last) ? TResult.Complete(unit) : TResult.Continue(unit))));
    
    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, S> FoldUntil<S>(
        Schedule schedule, 
        S initialState,
        Func<S, A, S> folder, 
        Func<S, bool> stateIs) =>
        FoldUntil(schedule, initialState, folder, last => stateIs(last.State));

    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, S> FoldUntil<S>(
        Schedule schedule, 
        S initialState,
        Func<S, A, S> folder, 
        Func<A, bool> valueIs) =>
        FoldUntil(schedule, initialState, folder, last => valueIs(last.Value));

    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, S> FoldUntil<S>(
        S initialState,
        Func<S, A, S> folder, 
        Func<(S State, A Value), bool> predicate) =>
        new(Transducer.compose(
            Morphism, 
            Transducer.foldSum<S, E, A>(
                Schedule.Forever,
                initialState, 
                folder, 
                last => predicate(last) ? TResult.Continue(unit) : TResult.Complete(unit))));
    
    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, S> FoldUntil<S>(
        S initialState,
        Func<S, A, S> folder, 
        Func<S, bool> stateIs) =>
        FoldUntil(Schedule.Forever, initialState, folder, last => stateIs(last.State));

    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, S> FoldUntil<S>(
        S initialState,
        Func<S, A, S> folder, 
        Func<A, bool> valueIs) =>
        FoldUntil(Schedule.Forever, initialState, folder, last => valueIs(last.Value));

    /// <summary>
    /// Fold the effect while the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, S> FoldWhile<S>(
        Schedule schedule, 
        S initialState,
        Func<S, A, S> folder, 
        Func<(S State, A Value), bool> predicate) =>
        new(Transducer.compose(
            Morphism, 
            Transducer.foldSum<S, E, A>(
                schedule,
                initialState, 
                folder, 
                last => predicate(last) ? TResult.Complete(unit) : TResult.Continue(unit))));
    
    /// <summary>
    /// Fold the effect while the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, S> FoldWhile<S>(
        Schedule schedule, 
        S initialState,
        Func<S, A, S> folder, 
        Func<S, bool> stateIs) =>
        FoldWhile(schedule, initialState, folder, last => stateIs(last.State));

    /// <summary>
    /// Fold the effect while the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, S> FoldWhile<S>(
        Schedule schedule, 
        S initialState,
        Func<S, A, S> folder, 
        Func<A, bool> valueIs) =>
        FoldWhile(schedule, initialState, folder, last => valueIs(last.Value));

    /// <summary>
    /// Fold the effect while the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, S> FoldWhile<S>(
        S initialState,
        Func<S, A, S> folder, 
        Func<(S State, A Value), bool> predicate) =>
        FoldWhile(Schedule.Forever, initialState, folder, predicate);
    
    /// <summary>
    /// Fold the effect while the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, S> FoldWhile<S>(
        S initialState,
        Func<S, A, S> folder, 
        Func<S, bool> stateIs) =>
        FoldWhile(Schedule.Forever, initialState, folder, last => stateIs(last.State));

    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, S> FoldWhile<S>(
        S initialState,
        Func<S, A, S> folder, 
        Func<A, bool> valueIs) =>
        FoldWhile(Schedule.Forever, initialState, folder, last => valueIs(last.Value));        

    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Folding, where each fold is monadic
    //

    /// <summary>
    /// Fold the effect forever or until the schedule expires
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, S> FoldM<S>(
        Schedule schedule, 
        S initialState,
        Func<S, A, IO<RT, E, S>> folder) =>
        Fold(schedule, IO<RT, E, S>.Pure(initialState), (ms, a) => ms.Bind(s => folder(s, a)))
            .Flatten();

    /// <summary>
    /// Fold the effect forever
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, S> FoldM<S>(
        S initialState,
        Func<S, A, IO<RT, E, S>> folder) =>
        FoldM(Schedule.Forever, initialState, folder);

    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, S> FoldUntilM<S>(
        Schedule schedule, 
        S initialState,
        Func<S, A, IO<RT, E, S>> folder, 
        Func<A, bool> valueIs) =>
        FoldUntil(schedule, IO<RT, E, S>.Pure(initialState), (ms, a) => ms.Bind(s => folder(s, a)), valueIs)
            .Flatten();

    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, S> FoldUntilM<S>(
        S initialState,
        Func<S, A, IO<RT, E, S>> folder, 
        Func<A, bool> valueIs) =>
        FoldUntilM(Schedule.Forever, initialState, folder, valueIs);

    /// <summary>
    /// Fold the effect while the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, S> FoldWhileM<S>(
        Schedule schedule, 
        S initialState,
        Func<S, A, IO<RT, E, S>> folder, 
        Func<A, bool> valueIs) =>
        FoldWhile(schedule, IO<RT, E, S>.Pure(initialState), (ms, a) => ms.Bind(s => folder(s, a)), valueIs)
            .Flatten();

    /// <summary>
    /// Fold the effect while the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public IO<RT, E, S> FoldWhileM<S>(
        S initialState,
        Func<S, A, IO<RT, E, S>> folder, 
        Func<A, bool> valueIs) =>
        FoldWhileM(Schedule.Forever, initialState, folder, valueIs);

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
    public IO<RT, E, A> Post() =>
        new(Transducer.post(Morphism));        
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Operators
    //

    /// <summary>
    /// Convert to an IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator IO<RT, E, A>(Pure<A> ma) =>
        ma.ToIO<RT, E>();

    /// <summary>
    /// Convert to an IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator IO<RT, E, A>(Fail<E> ma) =>
        ma.ToIO<RT, A>();

    /// <summary>
    /// Convert to an `IO` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator IO<RT, E, A>(in Either<E, A> ma) =>
        ma.Match(Right: Pure, Left: Fail);

    /// <summary>
    /// Convert to an IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator IO<RT, E, A>(Transducer<RT, Sum<E, A>> ma) =>
        Lift(ma);

    /// <summary>
    /// Convert to an IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator IO<RT, E, A>(Transducer<RT, Either<E, A>> ma) =>
        Lift(ma);

    /// <summary>
    /// Convert to an IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator IO<RT, E, A>(Transducer<Unit, Sum<E, A>> ma) =>
        Lift(Transducer.compose(Transducer.constant<RT, Unit>(default), ma));

    /// <summary>
    /// Convert to an IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator IO<RT, E, A>(Transducer<Unit, Either<E, A>> ma) =>
        Lift(Transducer.compose(Transducer.constant<RT, Unit>(default), ma));

    /// <summary>
    /// Convert to an IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator IO<RT, E, A>(Transducer<RT, A> ma) =>
        Lift(ma);

    /// <summary>
    /// Convert to an IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator IO<RT, E, A>(Transducer<Unit, A> ma) =>
        Lift(Transducer.compose(Transducer.constant<RT, Unit>(default), ma));

    /// <summary>
    /// Convert to an IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator IO<RT, E, A>(IO<E, A> ma) =>
        ma.WithRuntime<RT>();
    
    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> operator |(IO<RT, E, A> ma, IO<RT, E, A> mb) =>
        new(Transducer.choice(ma.Try().Morphism, mb.Morphism));

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> operator |(IO<RT, E, A> ma, CatchError<E> mb) =>
        new(Transducer.@try(
            ma.Morphism,
            mb.Match,
            Transducer.compose(lift(mb.Value), Transducer.mkLeft<E, A>())));

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> operator |(IO<RT, E, A> ma, CatchValue<E, A> mb) =>
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
    public static IO<RT, E, A> operator |(IO<RT, E, A> ma, IOCatch<RT, E, A> mb) =>
        ma.Try().Match(Succ: Pure, Fail: mb.Run).Flatten();        

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> operator |(IO<RT, E, A> ma, Transducer<RT, A> mb) =>
        ma.Try() | new IO<RT, E, A>(Transducer.compose(mb, Transducer.mkRight<E, A>()));

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> operator |(IO<RT, E, A> ma, Transducer<RT, E> mb) =>
        ma.Try() | new IO<RT, E, A>(Transducer.compose(mb, Transducer.mkLeft<E, A>()));

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> operator |(IO<RT, E, A> ma, Transducer<Unit, A> mb) =>
        ma.Try() | new IO<RT, E, A>(
            Transducer.compose(
                Transducer.constant<RT, Unit>(default), 
                mb, 
                Transducer.mkRight<E, A>()));

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> operator |(IO<RT, E, A> ma, Transducer<Unit, E> mb) =>
        ma.Try() | new IO<RT, E, A>(
            Transducer.compose(
                Transducer.constant<RT, Unit>(default), 
                mb, 
                Transducer.mkLeft<E, A>()));

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> operator |(IO<RT, E, A> ma, Transducer<RT, Sum<E, A>> mb) =>
        ma.Try() | new IO<RT, E, A>(mb);

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> operator |(IO<RT, E, A> ma, Transducer<Unit, Sum<E, A>> mb) =>
        ma.Try() | new IO<RT, E, A>(Transducer.compose(Transducer.constant<RT, Unit>(default), mb));

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> operator |(IO<RT, E, A> ma, Transducer<RT, Sum<Error, A>> mb) =>
        ma.Try() | new IO<RT, E, A>(Transducer.mapLeft(mb, lift<Error, E>(e => RT.FromError(e))));

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> operator |(IO<RT, E, A> ma, Transducer<Unit, Sum<Error, A>> mb) =>
        ma.Try() | new IO<RT, E, A>(
            Transducer.compose(
                Transducer.constant<RT, Unit>(default), 
                Transducer.mapLeft(mb, lift<Error, E>(e => RT.FromError(e)))));

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> operator |(Transducer<RT, A> ma, IO<RT, E, A> mb) =>
        new IO<RT, E, A>(Transducer.compose(ma, Transducer.mkRight<E, A>())) | mb;

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> operator |(Transducer<RT, E> ma, IO<RT, E, A> mb) =>
        new IO<RT, E, A>(Transducer.compose(ma, Transducer.mkLeft<E, A>())).Try() | mb;

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> operator |(Transducer<Unit, A> ma, IO<RT, E, A> mb) =>
        new IO<RT, E, A>(
            Transducer.compose(
                Transducer.constant<RT, Unit>(default),
                ma,
                Transducer.mkRight<E, A>())).Try() | mb;

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> operator |(Transducer<Unit, E> ma, IO<RT, E, A> mb) =>
        new IO<RT, E, A>(
            Transducer.compose(
                Transducer.constant<RT, Unit>(default),
                ma,
                Transducer.mkLeft<E, A>())).Try() | mb;

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> operator |(Transducer<RT, Sum<E, A>> ma, IO<RT, E, A>  mb) =>
        new IO<RT, E, A>(ma).Try() | mb;

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> operator |(Transducer<Unit, Sum<E, A>> ma, IO<RT, E, A> mb) =>
        new IO<RT, E, A>(Transducer.compose(Transducer.constant<RT, Unit>(default), ma)).Try() | mb;

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> operator |(Transducer<RT, Sum<Error, A>> ma, IO<RT, E, A> mb) =>
        new IO<RT, E, A>(Transducer.mapLeft(ma, lift<Error, E>(e => RT.FromError(e)))).Try() | mb;

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<RT, E, A> operator |(Transducer<Unit, Sum<Error, A>> ma, IO<RT, E, A> mb) =>
        new IO<RT, E, A>(
            Transducer.compose(
                Transducer.constant<RT, Unit>(default),
                Transducer.mapLeft(ma, lift<Error, E>(e => RT.FromError(e))))).Try() | mb;
}
*/
