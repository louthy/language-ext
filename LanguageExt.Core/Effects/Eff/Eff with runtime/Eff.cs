using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.Effects.Traits;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// Transducer based effect/`Eff` monad
/// </summary>
/// <typeparam name="RT">Runtime struct</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public readonly struct Eff<RT, A> : K<Eff.Runtime<RT>, A>
    where RT : HasIO<RT, Error>
{
    /// <summary>
    /// Underlying monad transformer stack that captures all of the IO behaviour 
    /// </summary>
    readonly ReaderT<RT, ResourceT<IO>, A> effect;
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Transformer helpers
    //
    
    static ReaderT<RT, ResourceT<IO>, X> asksM<X>(Func<RT, K<ResourceT<IO>, X>> f) =>
        ReaderT.asksM(f);

    static ReaderT<RT, ResourceT<IO>, X> asksIO<X>(Func<EnvIO, RT, ValueTask<X>> f) =>
        asksM(rt => ResourceT<IO>.liftIO(IO.asksIO(eio => f(eio, rt))));

    static ReaderT<RT, ResourceT<IO>, X> asksIO<X>(Func<RT, ValueTask<X>> f) =>
        asksIO<X>((_, rt) => f(rt));

    static ReaderT<RT, ResourceT<IO>, X> asks<X>(Func<RT, X> f) =>
        ReaderT.asks<RT, ResourceT<IO>, X>(f);

    static ReaderT<RT, ResourceT<IO>, X> asks<X>(Func<RT, Fin<X>> f) =>
        asksM<X>(rt => f(rt).Match(Succ: ResourceT.Pure<IO, X>,
                                   Fail: e => ResourceT.liftIO<IO, X>(IO<X>.Lift(e.Throw<X>))));

    static ReaderT<RT, ResourceT<IO>, X> asks<X>(Func<RT, Either<Error, X>> f) =>
        asksM<X>(rt => f(rt).Match(Right: ResourceT.Pure<IO, X>,
                                   Left: e => ResourceT.liftIO<IO, X>(IO<X>.Lift(e.Throw<X>))));

    static ReaderT<RT, ResourceT<IO>, X> liftIO<X>(IO<X> ma) =>
        ReaderT.liftIO<RT, ResourceT<IO>, X>(ma);

    static ReaderT<RT, ResourceT<IO>, X> fail<X>(Error value) =>
        asksM(_ => ResourceT.lift(IO<X>.Lift(value.Throw<X>)));

    static ReaderT<RT, ResourceT<IO>, X> pure<X>(X value) =>
        ReaderT<RT, ResourceT<IO>, X>.Pure(value);

    Eff<RT, B> MapReader<B>(Func<ReaderT<RT, ResourceT<IO>, A>, ReaderT<RT, ResourceT<IO>, B>> f) =>
        new(f(effect));

    Eff<RT, B> MapResource<B>(Func<ResourceT<IO, A>, ResourceT<IO, B>> f) =>
        MapReader(reader => reader.MapT(resource => f(resource.As())));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Constructors
    //

    [MethodImpl(Opt.Default)]
    internal Eff(ReaderT<RT, ResourceT<IO>, A> effect) =>
        this.effect = effect;

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    internal Eff(Func<RT, ValueTask<A>> effect)
        : this(asksIO(effect))
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    Eff(A value) 
        : this(pure(value))
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    Eff(Error value) 
        : this(fail<A>(value))
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    Eff(Func<RT, A> effect) 
        : this(asks(effect))
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    Eff(Func<RT, Fin<A>> effect)
        : this(asks(effect))
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    Eff(Func<RT, Either<Error, A>> effect) 
        : this(asks(effect))
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    Eff(IO<A> effect) 
        : this(liftIO(effect))
    { }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Invoking
    //

    /// <summary>
    /// Invoke the effect
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Fin<A> Run(RT env) =>
        effect.Run(env).As()
              .Run().As()
              .Run(EnvIO.New(
                       env.CancellationToken,
                       env.CancellationTokenSource,
                       env.SynchronizationContext));
    
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
    public Eff<RT, A> Timeout(TimeSpan timeoutDelay) =>
        MapIO(io => io.Timeout(timeoutDelay));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Lifting
    //
    
    /// <summary>
    /// Lift a value into the `Eff` monad 
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> Pure(A value) =>
        new(value);

    /// <summary>
    /// Lift a failure into the `Eff` monad 
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> Fail(Error error) =>
        new(error);

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> Lift(Func<RT, Either<Error, A>> f) =>
        new (f);

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> Lift(Func<RT, Fin<A>> f) =>
        new (f);

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> Lift(Func<RT, A> f) =>
        new (f);

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> LiftIO(Func<RT, ValueTask<A>> f) =>
        new (f);

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> LiftIO(IO<A> ma) =>
        new(ma);
    
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
    public Eff<RT, ForkIO<A>> Fork(Option<TimeSpan> timeout = default) =>
        MapIO(io => io.Fork(timeout));
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Map and map-left
    //

    /// <summary>
    /// Maps the `Eff` monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped `Eff` monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, B> Map<B>(Func<A, B> f) =>
        new(effect.Map(f));

    /// <summary>
    /// Maps the `Eff` monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped `Eff` monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, B> Select<B>(Func<A, B> f) =>
        new(effect.Select(f));

    /// <summary>
    /// Maps the `Eff` monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped `Eff` monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, A> MapFail(Func<Error, Error> f) =>
        MapIO(io => io.MapFail(f));

    /// <summary>
    /// Maps the inner IO monad
    /// </summary>
    /// <param name="f">Function to map with</param>
    /// <returns>Mapped `Eff` monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, B> MapIO<B>(Func<IO<A>, IO<B>> f) =>
        MapResource(resource => resource.MapT(io => f(io.As())));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Bi-map
    //

    /// <summary>
    /// Mapping of either the Success state or the Failure state depending on what
    /// state this `Eff` monad is in.  
    /// </summary>
    /// <param name="Succ">Mapping to use if the `Eff` monad if in a success state</param>
    /// <param name="Fail">Mapping to use if the `Eff` monad if in a failure state</param>
    /// <returns>Mapped `Eff` monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, B> BiMap<B>(Func<A, B> Succ, Func<Error, Error> Fail) =>
        MapIO(io => io.BiMap(Succ, Fail));

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
    public Eff<RT, B> Match<B>(Func<A, B> Succ, Func<Error, B> Fail) =>
        MapIO(io => io.Match(Succ, Fail));

    /// <summary>
    /// Map the failure to a success value
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO in a success state</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, A> IfFail(Func<Error, A> Fail) =>
        MapIO(io => io.IfFail(Fail));

    /// <summary>
    /// Map the failure to a new IO effect
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO that encapsulates that IfFail</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, A> IfFailEff(Func<Error, Eff<RT, A>> Fail) =>
        Match(Succ: Pure, Fail: Fail).Flatten();
    
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
    public Eff<RT, A> Filter(Func<A, bool> predicate) =>
        Bind(x => predicate(x) ? Pure(x) : Fail(Errors.None));

    /// <summary>
    /// Only allow values through the effect if the predicate returns `true` for the bound value
    /// </summary>
    /// <param name="predicate">Predicate to apply to the bound value></param>
    /// <returns>Filtered IO</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, A> Where(Func<A, bool> predicate) =>
        Bind(x => predicate(x) ? Pure(x) : Fail(Errors.None));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monadic binding
    //

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, B> Bind<B>(Func<A, Eff<RT, B>> f) =>
        new(effect.Bind(x => f(x).effect));

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, B> Bind<B>(Func<A, IO<B>> f) =>
        new(effect.Bind(f));

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, B> Bind<B>(Func<A, Ask<RT, B>> f) =>
        new(effect.Bind(f));
    
    /*
     TODO
     
    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, B> Bind<B>(Func<A, Eff<B>> f) =>
        new(effect.Bind(x => f(x).effect));
        */

    /*
     TODO

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, B> Bind<B>(Func<A, K<Eff, B>> f) =>
        Bind(a => f(a).As());
        */

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, B> Bind<B>(Func<A, K<Eff.Runtime<RT>, B>> f) =>
        Bind(a => f(a).As());

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, B> Bind<B>(Func<A, Pure<B>> f) =>
        Map(x => f(x).Value);

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, A> Bind(Func<A, Fail<Error>> f) =>
        Bind(x => Fail(f(x).Value));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monadic binding and projection
    //

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, C> SelectMany<B, C>(Func<A, Eff<RT, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, C> SelectMany<B, C>(Func<A, K<Eff.Runtime<RT>, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).As(), project);

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        new(effect.SelectMany(bind, project));

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        new(effect.SelectMany(bind, project));

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, C> SelectMany<B, C>(Func<A, Ask<RT, B>> bind, Func<A, B, C> project) =>
        new(effect.SelectMany(bind, project));

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, C> SelectMany<B, C>(Func<A, Fail<Error>> bind, Func<A, B, C> project) =>
        SelectMany(x => Eff<RT, B>.Fail(bind(x).Value), project);

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, C> SelectMany<C>(Func<A, Guard<Error, Unit>> bind, Func<A, Unit, C> project) =>
        from x in this
        from r in bind(x) switch
                  {
                      { Flag: true } => Eff<RT, Unit>.Pure(unit),
                      var g          => Eff<RT, Unit>.Fail(g.OnFalse())
                  }
        select project(x, unit);

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, C> SelectMany<C>(Func<A, Guard<Fail<Error>, Unit>> bind, Func<A, Unit, C> project) =>
        from x in this
        from r in bind(x) switch
                  {
                      { Flag: true } => Eff<RT, Unit>.Pure(unit),
                      var g          => Eff<RT, Unit>.Fail(g.OnFalse().Value)
                  }
        select project(x, unit);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Folding
    //

    /// <summary>
    /// Fold the effect forever or until the schedule expires
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, S> Fold<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder) =>
        MapIO(io => io.Fold(schedule, initialState, folder));

    /// <summary>
    /// Fold the effect forever
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, S> Fold<S>(
        S initialState,
        Func<S, A, S> folder) =>
        MapIO(io => io.Fold(initialState, folder));

    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, S> FoldUntil<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        MapIO(io => io.FoldUntil(schedule, initialState, folder, predicate));

    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, S> FoldUntil<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        MapIO(io => io.FoldUntil(schedule, initialState, folder, stateIs));

    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, S> FoldUntil<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        MapIO(io => io.FoldUntil(schedule, initialState, folder, valueIs));

    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, S> FoldUntil<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        MapIO(io => io.FoldUntil(initialState, folder, predicate));

    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, S> FoldUntil<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        MapIO(io => io.FoldUntil(initialState, folder, stateIs));

    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, S> FoldUntil<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        MapIO(io => io.FoldUntil(initialState, folder, valueIs));

    /// <summary>
    /// Fold the effect while the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, S> FoldWhile<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        MapIO(io => io.FoldWhile(schedule, initialState, folder, predicate));

    /// <summary>
    /// Fold the effect while the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, S> FoldWhile<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        MapIO(io => io.FoldWhile(schedule, initialState, folder, stateIs));

    /// <summary>
    /// Fold the effect while the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, S> FoldWhile<S>(
        Schedule schedule,
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        MapIO(io => io.FoldWhile(schedule, initialState, folder, valueIs));

    /// <summary>
    /// Fold the effect while the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, S> FoldWhile<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<(S State, A Value), bool> predicate) =>
        MapIO(io => io.FoldWhile(initialState, folder, predicate));

    /// <summary>
    /// Fold the effect while the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, S> FoldWhile<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<S, bool> stateIs) =>
        MapIO(io => io.FoldWhile(initialState, folder, stateIs));

    /// <summary>
    /// Fold the effect until the predicate returns `true`
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, S> FoldWhile<S>(
        S initialState,
        Func<S, A, S> folder,
        Func<A, bool> valueIs) =>
        MapIO(io => io.FoldWhile(initialState, folder, valueIs));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Synchronisation between contexts
    //

    /// <summary>
    /// Make the effect run on the `SynchronizationContext` that was captured at the start
    /// of an `Run` call.
    /// </summary>
    /// <remarks>
    /// The effect receives its input value from the currently running sync-context and
    /// then proceeds to run its operation in the captured `SynchronizationContext`:
    /// typically a UI context, but could be any captured context.  The result of the
    /// effect is the received back on the currently running sync-context.
    /// </remarks>
    public Eff<RT, A> Post() =>
        MapIO(io => io.Post());

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Operators
    //

    /// <summary>
    /// Convert to an `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<RT, A>(in Pure<A> ma) =>
        ma.ToEff<RT>();

    /// <summary>
    /// Convert to an `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<RT, A>(in Fail<Error> ma) =>
        ma.ToEff<RT, A>();

    /// <summary>
    /// Convert to an `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<RT, A>(in Either<Error, A> ma) =>
        ma.Match(Right: Pure, Left: Fail, Bottom: () => Fail(Errors.Bottom));

    /// <summary>
    /// Convert to an `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<RT, A>(in Fin<A> ma) =>
        ma.Match(Succ: Pure, Fail: Fail);

    /*
     TODO
     
    /// <summary>
    /// Convert to an `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<RT, A>(Eff<A> ma) =>
        Lift(Transducer.compose(MinRT.convert<RT>(), ma.Morphism));
        */

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> operator |(Eff<RT, A> ma, Eff<RT, A> mb) =>
        Lift(rt =>
             {
                 var ra = ma.Run(rt);
                 if (ra.IsSucc) return ra;
                 return mb.Run(rt);
             });

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative value if the IO operation fails</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> operator |(Eff<RT, A> ma, Pure<A> mb) =>
        Lift(rt =>
            {
                 var ra = ma.Run(rt);
                 if (ra.IsSucc) return ra;
                 return mb.Value;
             });

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="error">Alternative value if the IO operation fails</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> operator |(Eff<RT, A> ma, Fail<Error> error) =>
        Lift(rt =>
             {
                 var ra = ma.Run(rt);
                 if (ra.IsSucc) return ra;
                 return error.Value;
             });

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="error">Error if the IO operation fails</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> operator |(Eff<RT, A> ma, Error error) =>
        Lift(rt =>
             {
                 var ra = ma.Run(rt);
                 if (ra.IsSucc) return ra;
                 return error;
             });

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="value">Alternative value if the IO operation fails</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> operator |(Eff<RT, A> ma, A value) =>
        Lift(rt =>
             {
                 var ra = ma.Run(rt);
                 if (ra.IsSucc) return ra;
                 return value;
             });

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> operator |(Eff<RT, A> ma, CatchError<Error> mb) =>
        Lift(rt =>
             {
                 var ra = ma.Run(rt);
                 if (ra.IsSucc) return ra;
                 return mb.Match(ra.Error) ? mb.Value(ra.Error) : ra;
             });

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> operator |(Eff<RT, A> ma, CatchError mb) =>
        Lift(rt =>
             {
                 var ra = ma.Run(rt);
                 if (ra.IsSucc) return ra;
                 return mb.Match(ra.Error) ? mb.Value(ra.Error) : ra;
             });

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> operator |(Eff<RT, A> ma, CatchValue<Error, A> mb) =>
        Lift(rt =>
             {
                 var ra = ma.Run(rt);
                 if (ra.IsSucc) return ra;
                 return mb.Match(ra.Error) ? mb.Value(ra.Error) : ra;
             });

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> operator |(Eff<RT, A> ma, CatchValue<A> mb) =>
        Lift(rt =>
             {
                 var ra = ma.Run(rt);
                 if (ra.IsSucc) return ra;
                 return mb.Match(ra.Error) ? mb.Value(ra.Error) : ra;
             });

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> operator |(Eff<RT, A> ma, EffCatch<RT, A> mb) =>
        Lift(rt =>
             {
                 var ra = ma.Run(rt);
                 if (ra.IsSucc) return ra;
                 return mb.Run(ra.Error).Run(rt);
             });

    /// <summary>
    /// Run the first IO operation; if it fails, run the second.  Otherwise return the
    /// result of the first without running the second.
    /// </summary>
    /// <param name="ma">First IO operation</param>
    /// <param name="mb">Alternative IO operation</param>
    /// <returns>Result of either the first or second operation</returns>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> operator |(Eff<RT, A> ma, EffCatch<A> mb) =>
        Lift(rt =>
             {
                 var ra = ma.Run(rt);
                 if (ra.IsSucc) return ra;
                 return mb.Run(ra.Error).Run();
             });

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Obsolete
    //

    /// <summary>
    /// Lift a value into the `Eff` monad
    /// </summary>
    [Obsolete("Use either: `Eff<RT, A>.Lift`, `Prelude.liftEff`, or `lift`")]
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> Success(A value) =>
        Pure(value);

    /// <summary>
    /// Lift a synchronous effect into the `Eff` monad
    /// </summary>
    [Obsolete("Use either: `Eff<RT, A>.Lift`, `Prelude.liftEff`, or `lift`")]
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> Effect(Func<RT, A> f) =>
        Lift(f);

    /// <summary>
    /// Lift a synchronous effect into the `Eff` monad
    /// </summary>
    [Obsolete("Use either: `Eff<RT, A>.Lift`, `Prelude.liftEff`, or `lift`")]
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> EffectMaybe(Func<RT, Fin<A>> f) =>
        Lift(f);
}
