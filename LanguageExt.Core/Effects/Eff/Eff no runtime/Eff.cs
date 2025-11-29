using System;
using System.Collections.Generic;
using LanguageExt.Common;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.Async.Linq;
using LanguageExt.Effects;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// This monad is used to encapsulate side effects and exception capture 
/// </summary>
/// <typeparam name="RT">Runtime type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public record Eff<A>(Eff<MinRT, A> effect) :
    Fallible<Eff<A>>,
    K<Eff, A>,
    Alternative<Eff<A>>,
    MonoidK<Eff<A>>,
    Final<Eff<A>>,
    Deriving.Readable<Eff<A>, A, ReaderT<A, IO>>,
    Deriving.MonadUnliftIO<Eff<A>, ReaderT<A, IO>>,
    Natural<Eff<MinRT>, Eff>,
    CoNatural<Eff<MinRT>, Eff>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Lifting
    //

    /// <summary>
    /// Lift a value into the `Eff` monad 
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> Pure(A value) =>
        new(Eff<MinRT, A>.Pure(value));

    /// <summary>
    /// Lift a failure into the `Eff` monad 
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> Fail(Error error) =>
        new(Eff<MinRT, A>.Fail(error));

    /// <summary>
    /// Convert to an `Eff` monad with a runtime
    /// </summary>
    public Eff<RT, A> WithRuntime<RT>() =>
        MonadIO.liftIO<Eff<RT>, A>(effect.RunIO(new MinRT())).As();

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> Lift(Func<MinRT, Either<Error, A>> f) =>
        new(Eff<MinRT, A>.Lift(f));

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> Lift(Func<MinRT, Fin<A>> f) =>
        new(Eff<MinRT, A>.Lift(f));

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> Lift(Func<MinRT, A> f) =>
        new(Eff<MinRT, A>.Lift(f));

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> LiftIO(Func<MinRT, Task<A>> f) =>
        new(Eff<MinRT, A>.LiftIO(f));

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> LiftIO(Func<MinRT, Task<Fin<A>>> f) =>
        new(Eff<MinRT, A>.LiftIO(rt => f(rt).Map(r => r.ThrowIfFail())));

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> LiftIO(Func<MinRT, IO<A>> f) =>
        new(Eff<MinRT, A>.LiftIO(f));

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> LiftIO(IO<A> ma) =>
        new(Eff<MinRT, A>.LiftIO(ma));

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> Lift(Func<Either<Error, A>> f) =>
        new(Eff<MinRT, A>.Lift(_ => f()));

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> Lift(Func<Fin<A>> f) =>
        new(Eff<MinRT, A>.Lift(_ => f()));

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> Lift(Func<A> f) =>
        new(Eff<MinRT, A>.Lift(_ => f()));

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> LiftIO(Func<Task<A>> f) =>
        new(Eff<MinRT, A>.LiftIO(_ => f()));

    /// <summary>
    /// Lift an effect into the `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> LiftIO(Func<Task<Fin<A>>> f) =>
        new(Eff<MinRT, A>.LiftIO(_ => f().Map(r => r.ThrowIfFail())));

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
    public Eff<B> Map<B>(Func<A, B> f) =>
        new(effect.Map(f));

    /// <summary>
    /// Maps the `Eff` monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped `Eff` monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<B> Select<B>(Func<A, B> f) =>
        new(effect.Map(f));

    /// <summary>
    /// Maps the `Eff` monad if it's in a success state
    /// </summary>
    /// <param name="f">Function to map the success value with</param>
    /// <returns>Mapped `Eff` monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<A> MapFail(Func<Error, Error> f) =>
        this.Catch(f).As();

    /// <summary>
    /// Maps the inner IO monad
    /// </summary>
    /// <param name="f">Function to map with</param>
    /// <returns>Mapped `Eff` monad</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<B> MapIO<B>(Func<IO<A>, IO<B>> f) =>
        new(effect.MapIO(f));

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
    public Eff<B> BiMap<B>(Func<A, B> Succ, Func<Error, Error> Fail) =>
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
    public Eff<B> Match<B>(Func<A, B> Succ, Func<Error, B> Fail) =>
        Map(Succ).Catch(Fail).As();

    /// <summary>
    /// Map the failure to a success value
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO in a success state</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<A> IfFail(Func<Error, A> Fail) =>
        this.Catch(Fail).As();

    /// <summary>
    /// Map the failure to a new IO effect
    /// </summary>
    /// <param name="f">Function to map the fail value</param>
    /// <returns>IO that encapsulates that IfFail</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<A> IfFailEff(Func<Error, Eff<A>> Fail) =>
        this.Catch(Fail).As();

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
    public Eff<A> Filter(Func<A, bool> predicate) =>
        new(effect.Filter(predicate));

    /// <summary>
    /// Only allow values through the effect if the predicate returns `true` for the bound value
    /// </summary>
    /// <param name="predicate">Predicate to apply to the bound value></param>
    /// <returns>Filtered IO</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<A> Where(Func<A, bool> predicate) =>
        new(effect.Where(predicate));

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
    public Eff<B> Bind<B>(Func<A, Eff<B>> f) =>
        new(effect.Bind(x => f(x).effect));

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<B> Bind<B>(Func<A, IO<B>> f) =>
        new(effect.Bind(f));

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<B> Bind<B>(Func<A, Ask<MinRT, B>> f) =>
        new(effect.Bind(f));

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, B> Bind<RT, B>(Func<A, Eff<RT, B>> f) =>
        WithRuntime<RT>().Bind(f);

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, B> Bind<RT, B>(Func<A, K<Eff<RT>, B>> f) =>
        Bind(a => f(a).As());

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<B> Bind<B>(Func<A, K<Eff, B>> f) =>
        Bind(a => f(a).As());

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<B> Bind<B>(Func<A, Pure<B>> f) =>
        Map(x => f(x).Value);

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="f">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<A> Bind(Func<A, Fail<Error>> f) =>
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
    public Eff<C> SelectMany<B, C>(Func<A, Eff<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, C> SelectMany<RT, B, C>(Func<A, Eff<RT, B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<RT, C> SelectMany<RT, B, C>(Func<A, K<Eff<RT>, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).As(), project);

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<C> SelectMany<B, C>(Func<A, K<Eff, B>> bind, Func<A, B, C> project) =>
        SelectMany(x => bind(x).As(), project);

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<C> SelectMany<B, C>(Func<A, Pure<B>> bind, Func<A, B, C> project) =>
        new(effect.SelectMany(bind, project));

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        new(effect.SelectMany(bind, project));

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<C> SelectMany<B, C>(Func<A, Ask<MinRT, B>> bind, Func<A, B, C> project) =>
        new(effect.SelectMany(bind, project));

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<C> SelectMany<B, C>(Func<A, Fail<Error>> bind, Func<A, B, C> project) =>
        SelectMany(x => Eff<B>.Fail(bind(x).Value), project);

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<C> SelectMany<C>(Func<A, Guard<Error, Unit>> bind, Func<A, Unit, C> project) =>
        new(effect.SelectMany(bind, project));

    /// <summary>
    /// Monadic bind operation.  This runs the current `Eff` monad and feeds its result to the
    /// function provided; which in turn returns a new `Eff` monad.  This can be thought of as
    /// chaining IO operations sequentially.
    /// </summary>
    /// <param name="bind">Bind operation</param>
    /// <returns>Composition of this monad and the result of the function provided</returns>
    [Pure, MethodImpl(Opt.Default)]
    public Eff<C> SelectMany<C>(Func<A, Guard<Fail<Error>, Unit>> bind, Func<A, Unit, C> project) =>
        new(effect.SelectMany(bind, project));

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Conversion operators
    //

    /// <summary>
    /// Convert to an `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<A>(Pure<A> ma) =>
        ma.ToEff();

    /// <summary>
    /// Convert to an `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<A>(Fail<Error> ma) =>
        ma.ToEff<A>();

    /// <summary>
    /// Convert to an `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<A>(in Lift<A> ma) =>
        Lift(ma.Function);

    /// <summary>
    /// Convert to an `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<A>(in Lift<Fin<A>> ma) =>
        Lift(ma.Function);

    /// <summary>
    /// Convert to an `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<A>(in Lift<MinRT, A> ma) =>
        Lift(ma.Function);

    /// <summary>
    /// Convert to an `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<A>(in Lift<MinRT, Fin<A>> ma) =>
        Lift(ma.Function);

    /// <summary>
    /// Convert to an `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<A>(in Either<Error, A> ma) =>
        ma.Match(Left: Fail, Right: Pure);

    /// <summary>
    /// Convert to an `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<A>(in Fin<A> ma) =>
        ma.Match(Succ: Pure, Fail: Fail);

    /// <summary>
    /// Convert to an `Eff` monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<A>(in IO<A> ma) =>
        LiftIO(ma);

    [Pure, MethodImpl(Opt.Default)]
    public static implicit operator Eff<A>(in Error fail) => 
        Fail(fail);

    public override string ToString() => 
        "Eff";

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Trait implementations for `Eff<RT, A>`
    //
    // It's important to remember that the code below is the trait implementations for `Eff<RT, A>`, and not
    // related to `Eff<A>` in any way at all.  `A` in this instance is the `RT` in `Eff<RT, A>`.  
    //
    // It is this way to make it easier to work with Eff traits, even if this is a bit ugly.
    //

    static K<Eff<A>, T> Fallible<Error, Eff<A>>.Fail<T>(Error error) => 
        FailEff<A, T>(error);

    static K<Eff<A>, T> Fallible<Error, Eff<A>>.Catch<T>(
        K<Eff<A>, T> ma,
        Func<Error, bool> pred,
        Func<Error, K<Eff<A>, T>> f) =>
        new Eff<A, T>(
            new ReaderT<A, IO, T>(
                env => ma.As().RunIO(env).Catch(pred, e => f(e).As().effect.Run(env))));

    static K<Eff<A>, T> Final<Eff<A>>.Finally<X, T>(K<Eff<A>, T> fa, K<Eff<A>, X> @finally) => 
        new Eff<A, T>(
            new ReaderT<A, IO, T>(
                env => fa.As().RunIO(env).Finally(@finally.As().effect.Run(env))));
    
    static K<Eff<A>, T> Applicative<Eff<A>>.Actions<T>(IEnumerable<K<Eff<A>, T>> fas) =>
        new Eff<A, T>(
            new ReaderT<A, IO, T>(
                rt => fas.Select(fa => fa.RunIO(rt)).Actions())); 

    static K<Eff<A>, T> Applicative<Eff<A>>.Actions<T>(IAsyncEnumerable<K<Eff<A>, T>> fas) =>
        new Eff<A, T>(
            new ReaderT<A, IO, T>(
                rt => fas.Select(fa => fa.RunIO(rt)).Actions())); 
    
    static K<Eff<A>, T> MonoidK<Eff<A>>.Empty<T>() =>
        Eff<A, T>.Fail(Errors.None);

    static K<ReaderT<A, IO>, A1> Natural<Eff<A>, ReaderT<A, IO>>.Transform<A1>(K<Eff<A>, A1> fa) =>
        fa.As().effect;

    static K<Eff<A>, T> CoNatural<Eff<A>, ReaderT<A, IO>>.CoTransform<T>(K<ReaderT<A, IO>, T> fa) => 
        new Eff<A, T>(fa.As());
    
    static K<Eff<A>, A1> SemigroupK<Eff<A>>.Combine<A1>(K<Eff<A>, A1> lhs, K<Eff<A>, A1> rhs) => 
        lhs.Catch(e1 => rhs.Catch(e2 => e1 + e2));

    static K<Eff<A>, A1> Choice<Eff<A>>.Choose<A1>(K<Eff<A>, A1> lhs, K<Eff<A>, A1> rhs) => 
        lhs.Catch(rhs);

    static K<Eff<A>, A1> Choice<Eff<A>>.Choose<A1>(K<Eff<A>, A1> lhs, Memo<Eff<A>, A1> rhs) => 
        lhs.Catch(_ => rhs.Value);

    static K<Eff<A>, T> Alternative<Eff<A>>.Empty<T>() => 
        Eff<A, T>.Fail(Errors.None);

    static K<Eff, T> Natural<Eff<MinRT>, Eff>.Transform<T>(K<Eff<MinRT>, T> fa) => 
        new Eff<T>(fa.As());

    static K<Eff<MinRT>, T> CoNatural<Eff<MinRT>, Eff>.CoTransform<T>(K<Eff, T> fa) =>
        fa.As().effect;
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Transformer helpers
    //

    internal static ReaderT<A, IO, X> getsM<X>(Func<A, IO<X>> f) =>
        from e in ReaderT.ask<IO, A>()
        from r in ReaderT.liftIO<A, IO, X>(IO.lift(() => f(e)).Flatten())
        select r;

    internal static ReaderT<A, IO, X> getsIO<X>(Func<A, Task<X>> f) =>
        from e in ReaderT.ask<IO, A>()
        from r in ReaderT.liftIO<A, IO, X>(IO.liftAsync(() => f(e)))
        select r;

    internal static ReaderT<A, IO, X> gets<X>(Func<A, X> f) =>
        from e in ReaderT.ask<IO, A>()
        from r in ReaderT.liftIO<A, IO, X>(IO.lift(() => f(e)))
        select r;

    internal static ReaderT<A, IO, X> gets<X>(Func<A, Fin<X>> f) =>
        from e in ReaderT.ask<IO, A>()
        from r in ReaderT.liftIO<A, IO, X>(IO.lift(() => f(e)))
        select r;

    internal static ReaderT<A, IO, X> gets<X>(Func<A, Either<Error, X>> f) =>
        from e in ReaderT.ask<IO, A>()
        from r in ReaderT.liftIO<A, IO, X>(IO.lift(() => f(e)))
        select r;

    internal static ReaderT<A, IO, X> fail<X>(Error value) =>
        ReaderT.liftIO<A, IO, X>(IO.fail<X>(value));

    internal static ReaderT<A, IO, X> pure<X>(X value) =>
        ReaderT<A, IO, X>.Pure(value);

    internal static readonly ReaderT<A, IO, (A Runtime, EnvIO EnvIO)> getState = 
        from rt in ReaderT.ask<IO, A>()
        from io in IO.env
        select (rt, io);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Obsolete
    //

    /// <summary>
    /// Lift a value into the `Eff` monad
    /// </summary>
    [Obsolete("Use either: `Prelude.Pure` or `Eff<A>.Pure`")]
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> Success(A value) =>
        Pure(value);

    /// <summary>
    /// Lift a synchronous effect into the `Eff` monad
    /// </summary>
    [Obsolete("Use either: `Prelude.lift` or `Eff<A>.Lift`")]
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> Effect(Func<A> f) =>
        Lift(_ => f());

    /// <summary>
    /// Lift a synchronous effect into the `Eff` monad
    /// </summary>
    [Obsolete("Use either: `Prelude.lift` or `Eff<A>.Lift`")]
    [Pure, MethodImpl(Opt.Default)]
    public static Eff<A> EffectMaybe(Func<Fin<A>> f) =>
        Lift(_ => f());
}
