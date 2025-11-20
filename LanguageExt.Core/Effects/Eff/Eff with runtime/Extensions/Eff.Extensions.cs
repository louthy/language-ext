using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class EffExtensions
{
    extension<RT, A>(K<Eff<RT>, A> ma)
    {
        /// <summary>
        /// Cast type to its Kind
        /// </summary>
        public Eff<RT, A> As() =>
            (Eff<RT, A>)ma;

        /// <summary>
        /// Invoke the effect
        /// </summary>
        /// <remarks>
        /// Returns the result value only 
        /// </remarks>
        [Pure, MethodImpl(Opt.Default)]
        public Fin<A> Run(RT env) =>
            ma.As().Run(env, EnvIO.New());

        /// <summary>
        /// Invoke the effect
        /// </summary>
        /// <remarks>
        /// Returns the result value only 
        /// </remarks>
        [Pure, MethodImpl(Opt.Default)]
        public Fin<A> Run(RT env, EnvIO envIO)
        {
            try
            {
                return ma.As().effect.Run(env).Run(envIO);
            }
            catch(Exception e)
            {
                return Fin.Fail<A>(e);
            }
        }

        /// <summary>
        /// Invoke the effect
        /// </summary>
        /// <remarks>
        /// This is labelled 'unsafe' because it can throw an exception, whereas
        /// `Run` will capture any errors and return a `Fin` type.
        /// </remarks>
        [Pure, MethodImpl(Opt.Default)]
        public A RunUnsafe(RT env) =>
            ma.As().effect.Run(env).Run();

        /// <summary>
        /// Invoke the effect
        /// </summary>
        /// <remarks>
        /// This is labelled 'unsafe' because it can throw an exception, whereas
        /// `Run` will capture any errors and return a `Fin` type.
        /// </remarks>
        [Pure, MethodImpl(Opt.Default)]
        public A RunUnsafe(RT env, EnvIO envIO) =>
            ma.As().effect.Run(env).Run(envIO);

        /// <summary>
        /// Invoke the effect to leave the inner IO monad
        /// </summary>
        [Pure, MethodImpl(Opt.Default)]
        public IO<A> RunIO(RT env) =>
            ma.As().effect.Run(env).As();

        /// <summary>
        /// Invoke the effect
        /// </summary>
        /// <remarks>
        /// Returns the result value only 
        /// </remarks>
        [Pure, MethodImpl(Opt.Default)]
        public async Task<Fin<A>> RunAsync(RT env)
        {
            try
            {
                return await ma.As().effect.Run(env).RunAsync();
            }
            catch(Exception e)
            {
                return Fin.Fail<A>(e);
            }
        }

        /// <summary>
        /// Invoke the effect
        /// </summary>
        /// <remarks>
        /// Returns the result value only 
        /// </remarks>
        [Pure, MethodImpl(Opt.Default)]
        public async Task<Fin<A>> RunAsync(RT env, EnvIO envIO)
        {
            try
            {
                return await ma.As().effect.Run(env).RunAsync(envIO);
            }
            catch(Exception e)
            {
                return Fin.Fail<A>(e);
            }
        }

        /// <summary>
        /// Invoke the effect
        /// </summary>
        /// <remarks>
        /// This is labelled 'unsafe' because it can throw an exception, whereas
        /// `Run` will capture any errors and return a `Fin` type.
        /// </remarks>
        [Pure, MethodImpl(Opt.Default)]
        public ValueTask<A> RunUnsafeAsync(RT env) =>
            ma.As().effect.Run(env).RunAsync(EnvIO.New());

        /// <summary>
        /// Invoke the effect
        /// </summary>
        /// <remarks>
        /// This is labelled 'unsafe' because it can throw an exception, whereas
        /// `Run` will capture any errors and return a `Fin` type.
        /// </remarks>
        [Pure, MethodImpl(Opt.Default)]
        public ValueTask<A> RunUnsafeAsync(RT env, EnvIO envIO) =>
            ma.As().effect.Run(env).RunAsync(envIO);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Invoking
    //


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
    public static Eff<RT, A> Flatten<RT, A>(this K<Eff<RT>, Eff<RT, A>> mma) =>
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
    public static Eff<RT, A> Flatten<RT, A>(this K<Eff<RT>, K<Eff<RT>, A>> mma) =>
        mma.As().Bind(ma => ma);
    

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  SelectMany extensions
    //

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<RT, D> SelectMany<RT, A, B, C, D>(
        this (K<Eff<RT>, A> First, K<Eff<RT>, B> Second) self,
        Func<(A First, B Second), K<Eff<RT>, C>> bind,
        Func<(A First, B Second), C, D> project) =>
        self.Zip().Bind(ab => bind(ab).Map(c => project(ab, c))).As();

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<RT, D> SelectMany<RT, A, B, C, D>(
        this K<Eff<RT>, A> self,
        Func<A, (K<Eff<RT>, B> First, K<Eff<RT>, C> Second)> bind,
        Func<A, (B First, C Second), D> project) =>
        self.As().Bind(a => bind(a).Zip().Map(cd => project(a, cd)));

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<RT, E> SelectMany<RT, A, B, C, D, E>(
        this (K<Eff<RT>, A> First, K<Eff<RT>, B> Second, K<Eff<RT>, C> Third) self,
        Func<(A First, B Second, C Third), K<Eff<RT>, D>> bind,
        Func<(A First, B Second, C Third), D, E> project) =>
        self.Zip().Bind(ab => bind(ab).Map(c => project(ab, c))).As();

    /// <summary>
    /// Monadic bind and project with paired IO monads
    /// </summary>
    public static Eff<RT, E> SelectMany<RT, A, B, C, D, E>(
        this K<Eff<RT>, A> self,
        Func<A, (K<Eff<RT>, B> First, K<Eff<RT>, C> Second, K<Eff<RT>, D> Third)> bind,
        Func<A, (B First, C Second, D Third), E> project) =>
        self.As().Bind(a => bind(a).Zip().Map(cd => project(a, cd)));
}
