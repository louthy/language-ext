using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// This monad is used to encapsulate side effects, exception capture, and dependency-injection via the `RT` runtime. 
/// </summary>
/// <typeparam name="RT">Runtime type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record Eff<RT, A>(ReaderT<RT, IO, A> effect) : 
    K<Eff<RT>, A>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Constructors
    //

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    internal Eff(Func<RT, Task<A>> effect)
        : this(Eff<RT>.getsIO(effect))
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    Eff(A value) 
        : this(Eff<RT>.pure(value))
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    Eff(Error value) 
        : this(Eff<RT>.fail<A>(value))
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    Eff(Func<RT, A> effect) 
        : this(Eff<RT>.gets(effect))
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    Eff(Func<RT, Fin<A>> effect)
        : this(Eff<RT>.gets(effect))
    {
    }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    Eff(Func<RT, Either<Error, A>> effect) 
        : this(Eff<RT>.gets(effect))
    { }

    /// <summary>
    /// Constructor
    /// </summary>
    [MethodImpl(Opt.Default)]
    Eff(IO<A> effect) 
        : this(ReaderT.liftIO<RT, IO, A>(effect))
    { }
    
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
    public static Eff<RT, A> LiftIO(Func<RT, Task<A>> f) =>
        new (f);

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> LiftIO(Func<RT, Task<Fin<A>>> f) =>
        new (rt => f(rt).Map(r => r.ThrowIfFail()));

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> LiftIO(Func<RT, IO<A>> f) =>
        new(Eff<RT>.getsM(f));

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> Lift(Func<Either<Error, A>> f) =>
        new (_ => f());    

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> Lift(Func<Fin<A>> f) =>
        new (_ => f());    

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> Lift(Func<A> f) =>
        new (_ => f());    

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> LiftIO(Func<Task<A>> f) =>
        new (_ => f());

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> LiftIO(Func<Task<Fin<A>>> f) =>
        new(_ => f().Map(r => r.ThrowIfFail()));    

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<RT, A> LiftIO(IO<A> ma) =>
        new(ma);
    
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
        new (effect.Map(f));

    /// <summary>
    /// Maps the `Eff` monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped `Eff` monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, B> Select<B>(Func<A, B> f) =>
        new (effect.Map(f));

    /// <summary>
    /// Maps the `Eff` monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped `Eff` monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, A> MapFail(Func<Error, Error> f) =>
        this.Catch(f).As();

    /// <summary>
    /// Maps the inner IO monad
    /// </summary>
    /// <param name="f">Function to map with</param>
    /// <returns>Mapped `Eff` monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, B> MapIO<B>(Func<IO<A>, IO<B>> f) =>
        mapIO(this, f).As();

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
        Map(Succ).Catch(Fail).As();

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
        Map(Succ).Catch(Fail).As();

    /// <summary>
    /// Map the failure to a success value
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO in a success state</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, A> IfFail(Func<Error, A> Fail) =>
        this.Catch(Fail).As();

    /// <summary>
    /// Map the failure to a new IO effect
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO that encapsulates that IfFail</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, A> IfFailEff(Func<Error, K<Eff<RT>, A>> Fail) =>
        this.Catch(Fail).As();

    /// <summary>
    /// Map the failure to a new IO effect
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO that encapsulates that IfFail</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, A> IfFailEff(Func<Error, K<Eff, A>> Fail) =>
        IfFailEff(e => Fail(e).As().WithRuntime<RT>());
    
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

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, B> Bind<B>(Func<A, K<Eff<RT>, B>> f) =>
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

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, B> Bind<B>(Func<A, Eff<B>> f) =>
        Bind(x => f(x).WithRuntime<RT>());

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
    public Eff<RT, C> SelectMany<B, C>(Func<A, K<Eff<RT>, B>> bind, Func<A, B, C> project) =>
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

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, C> SelectMany<B, C>(Func<A, Eff<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, C> SelectMany<B, C>(Func<A, K<Eff, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).As(), project);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Conversion operators
    //

    /// <summary>
    /// Convert to an `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<RT, A>(Pure<A> ma) =>
        ma.ToEff<RT>();

    /// <summary>
    /// Convert to an `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<RT, A>(Fail<Error> ma) =>
        ma.ToEff<RT, A>();

    /// <summary>
    /// Convert to an `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<RT, A>(in Lift<A> ma) =>
        Lift(ma.Function);

    /// <summary>
    /// Convert to an `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<RT, A>(in Lift<Fin<A>> ma) =>
        Lift(ma.Function);

    /// <summary>
    /// Convert to an `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<RT, A>(in Lift<RT, A> ma) =>
        Lift(ma.Function);

    /// <summary>
    /// Convert to an `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<RT, A>(in Lift<RT, Fin<A>> ma) =>
        Lift(ma.Function);    
    
    /// <summary>
    /// Convert to an `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<RT, A>(in Either<Error, A> ma) =>
        ma.Match(Left: Fail, Right: Pure);

    /// <summary>
    /// Convert to an `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<RT, A>(in Fin<A> ma) =>
        ma.Match(Succ: Pure, Fail: Fail);

    /// <summary>
    /// Convert to an `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<RT, A>(in Eff<A> ma) =>
        ma.WithRuntime<RT>();

    /// <summary>
    /// Convert to an `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<RT, A>(in IO<A> ma) =>
        LiftIO(ma);

    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<RT, A>(Error fail) =>
        Fail(fail);

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

    public override string ToString() => 
        "Eff";
}
