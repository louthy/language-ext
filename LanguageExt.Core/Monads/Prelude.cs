
using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Effects;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Construct identity monad
        /// </summary>
        public static Identity<A> Id<A>(A value) => 
            new (value);
        
        /// <summary>
        /// Create a new Pure monad.  This monad doesn't do much, but when combined with
        /// other monads, it allows for easier construction of pure lifted values.
        ///
        /// There are various bind operators that make it work with these types:
        ///
        ///     * Option
        ///     * Eff
        ///     * Either
        ///     * Fin
        ///     * IO
        ///     * Validation
        ///     
        /// </summary>
        /// <param name="value">Value to lift</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Pure monad</returns>
        public static Pure<A> Pure<A>(A value) =>
            new (value);
        
        /// <summary>
        /// Create a new Fail monad: the monad that always fails.  This monad doesn't do much,
        /// but when combined with other monads, it allows for easier construction of lifted 
        /// failure values.
        ///
        /// There are various bind operators that make it work with these types:
        ///
        ///     * Option
        ///     * Eff
        ///     * Either
        ///     * Fin
        ///     * IO
        ///     * Validation
        ///     
        /// </summary>
        /// <param name="value">Value to lift</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Pure monad</returns>
        public static Fail<E> Fail<E>(E error) =>
            new (error);

        /// <summary>
        /// Represents a lifted IO function.  
        /// </summary>
        /// <remarks>
        /// On its own this doesn't do much, but it allows other monads to convert
        /// from it and provide binding extensions that mean this will work in
        /// binding scenarios.
        ///
        /// It simplifies certain scenarios where additional generic arguments are
        /// needed.  This only requires the generic argument of the value which
        /// means the C# inference system can work.
        /// </remarks>
        /// <param name="f">IO function</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Lifted IO function</returns>
        public static LiftIO<A> liftIO<A>(Func<CancellationToken, Task<A>> f) =>
            new (f);

        /// <summary>
        /// Represents a lifted IO function.  
        /// </summary>
        /// <remarks>
        /// On its own this doesn't do much, but it allows other monads to convert
        /// from it and provide binding extensions that mean this will work in
        /// binding scenarios.
        ///
        /// It simplifies certain scenarios where additional generic arguments are
        /// needed.  This only requires the generic argument of the value which
        /// means the C# inference system can work.
        /// </remarks>
        /// <param name="f">IO function</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Lifted IO function</returns>
        public static LiftIO<A> liftIO<A>(Func<Task<A>> f) =>
            new (_ => f());

        /// <summary>
        /// Represents a lifted function.  
        /// </summary>
        /// <remarks>
        /// On its own this doesn't do much, but it allows other monads to convert
        /// from it and provide binding extensions that mean this will work in
        /// binding scenarios.
        ///
        /// It simplifies certain scenarios where additional generic arguments are
        /// needed.  This only requires the generic argument of the value which
        /// means the C# inference system can work.
        /// </remarks>
        /// <param name="f">Function to lift</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Lifted function</returns>
        public static Lift<Unit, A> lift<A>(Func<A> f) =>
            new (_ => f());

        /// <summary>
        /// Represents a lifted function.  
        /// </summary>
        /// <remarks>
        /// On its own this doesn't do much, but it allows other monads to convert
        /// from it and provide binding extensions that mean this will work in
        /// binding scenarios.
        ///
        /// It simplifies certain scenarios where additional generic arguments are
        /// needed.  This only requires the generic argument of the value which
        /// means the C# inference system can work.
        /// </remarks>
        /// <param name="f">Function to lift</param>
        /// <returns>Lifted function</returns>
        public static Lift<A, B> lift<A, B>(Func<A, B> f) =>
            new (f);
    }
}
