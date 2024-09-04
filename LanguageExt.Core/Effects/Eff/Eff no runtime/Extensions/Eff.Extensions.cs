using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.Effects;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class EffExtensions
{
    /// <summary>
    /// Cast type to its Kind
    /// </summary>
    public static Eff<A> As<A>(this K<Eff, A> ma) =>
        (Eff<A>)ma;

    /// <summary>
    /// Cast type to its Kind
    /// </summary>
    public static Eff<A> As<A>(this Eff<MinRT, A> ma) =>
        new(ma);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Invoking
    //

    /// <summary>
    /// Invoke the effect
    /// </summary>
    /// <remarks>
    /// Returns the result value only 
    /// </remarks>
    [Pure, MethodImpl(Opt.Default)]
    public static Fin<A> Run<A>(this K<Eff, A> ma) =>
        ma.As().effect.Run(default);

    /// <summary>
    /// Invoke the effect
    /// </summary>
    /// <remarks>
    /// Returns the result value only 
    /// </remarks>
    [Pure, MethodImpl(Opt.Default)]
    public static Fin<A> Run<A>(this K<Eff, A> ma, EnvIO envIO) =>
        ma.As().effect.Run(default, envIO);
    
    /// <summary>
    /// Invoke the effect
    /// </summary>
    /// <remarks>
    /// This is labelled 'unsafe' because it can throw an exception, whereas
    /// `Run` will capture any errors and return a `Fin` type.
    /// </remarks>
    [Pure, MethodImpl(Opt.Default)]
    public static A RunUnsafe<A>(this K<Eff, A> ma) =>
        ma.As().effect.RunUnsafe(default);

    /// <summary>
    /// Invoke the effect
    /// </summary>
    /// <remarks>
    /// This is labelled 'unsafe' because it can throw an exception, whereas
    /// `Run` will capture any errors and return a `Fin` type.
    /// </remarks>
    [Pure, MethodImpl(Opt.Default)]
    public static A RunUnsafe<A>(this K<Eff, A> ma, EnvIO envIO) =>
        ma.As().effect.RunUnsafe(default, envIO);

    /// <summary>
    /// Invoke the effect to leave the inner IO monad
    /// </summary>
    [Pure, MethodImpl(Opt.Default)]
    public static IO<A> RunIO<A>(this K<Eff, A> ma) =>
        ma.As().effect.RunIO(default);

    /// <summary>
    /// Invoke the effect
    /// </summary>
    /// <remarks>
    /// Returns the result value only 
    /// </remarks>
    [Pure, MethodImpl(Opt.Default)]
    public static Task<Fin<A>> RunAsync<A>(this K<Eff, A> ma) =>
        ma.As().effect.RunAsync(default);

    /// <summary>
    /// Invoke the effect
    /// </summary>
    /// <remarks>
    /// Returns the result value only 
    /// </remarks>
    [Pure, MethodImpl(Opt.Default)]
    public static Task<Fin<A>> RunAsync<A>(this K<Eff, A> ma, EnvIO envIO) =>
        ma.As().effect.RunAsync(default, envIO);
    
    /// <summary>
    /// Invoke the effect
    /// </summary>
    /// <remarks>
    /// This is labelled 'unsafe' because it can throw an exception, whereas
    /// `Run` will capture any errors and return a `Fin` type.
    /// </remarks>
    [Pure, MethodImpl(Opt.Default)]
    public static ValueTask<A> RunUnsafeAsync<A>(this K<Eff, A> ma) =>
        ma.As().effect.RunUnsafeAsync(default);

    /// <summary>
    /// Invoke the effect
    /// </summary>
    /// <remarks>
    /// This is labelled 'unsafe' because it can throw an exception, whereas
    /// `Run` will capture any errors and return a `Fin` type.
    /// </remarks>
    [Pure, MethodImpl(Opt.Default)]
    public static ValueTask<A> RunUnsafeAsync<A>(this K<Eff, A> ma, EnvIO envIO) =>
        ma.As().effect.RunUnsafeAsync(default, envIO);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Monadic join
    //

    /// <summary>
    /// Monadic join operator 
    /// </summary>
    /// <remarks>
    /// Collapses a nested IO monad so there is no nesting.
    /// </remarks>
    /// <param name="mma">Nest IO monad to flatten</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Bound value</typeparam>
    /// <returns>Flattened IO monad</returns>
    public static Eff<A> Flatten<A>(this K<Eff, K<Eff, A>> mma) =>
        mma.As().Bind(ma => ma);

    /// <summary>
    /// Monadic join operator 
    /// </summary>
    /// <remarks>
    /// Collapses a nested IO monad so there is no nesting.
    /// </remarks>
    /// <param name="mma">Nest IO monad to flatten</param>
    /// <typeparam name="RT">Runtime</typeparam>
    /// <typeparam name="A">Bound value</typeparam>
    /// <returns>Flattened IO monad</returns>
    public static Eff<A> Flatten<A>(this K<Eff, Eff<A>> mma) =>
        mma.As().Bind(ma => ma);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  SelectMany extensions
    //

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<D> SelectMany<A, B, C, D>(
        this (K<Eff, A> First, K<Eff, B> Second) self,
        Func<(A First, B Second), K<Eff, C>> bind,
        Func<(A First, B Second), C, D> project) =>
        self.ZipIO().Bind(ab => bind(ab).Map(c => project(ab, c))).As();

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<D> SelectMany<A, B, C, D>(
        this K<Eff, A> self,
        Func<A, (K<Eff, B> First, K<Eff, C> Second)> bind,
        Func<A, (B First, C Second), D> project) =>
        self.As().Bind(a => bind(a).ZipIO().Map(cd => project(a, cd)));

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<E> SelectMany<A, B, C, D, E>(
        this (K<Eff, A> First, K<Eff, B> Second, K<Eff, C> Third) self,
        Func<(A First, B Second, C Third), K<Eff, D>> bind,
        Func<(A First, B Second, C Third), D, E> project) =>
        self.ZipIO().Bind(ab => bind(ab).Map(c => project(ab, c))).As();

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<E> SelectMany<A, B, C, D, E>(
        this K<Eff, A> self,
        Func<A, (K<Eff, B> First, K<Eff, C> Second, K<Eff, D> Third)> bind,
        Func<A, (B First, C Second, D Third), E> project) =>
        self.As().Bind(a => bind(a).ZipIO().Map(cd => project(a, cd)));
}
