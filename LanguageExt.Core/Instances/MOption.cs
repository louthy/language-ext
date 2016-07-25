using System;
using System.Linq;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;

namespace LanguageExt.Instances
{
    /// <summary>
    /// Option monad type-class instance
    /// </summary>
    /// <typeparam name="A">Type of the optional bound value</typeparam>
    public struct MOption<A> : MonadPlus<A>, Optional<A>
    {
        /// <summary>
        /// Functor map operation
        /// </summary>
        /// <typeparam name="B">The type that f maps to</typeparam>
        /// <param name="ma">The functor</param>
        /// <param name="f">The operation to perform on the bound value</param>
        /// <returns>A mapped functor</returns>
        [Pure]
        public Functor<B> Map<B>(Functor<A> ma, Func<A, B> f)
        {
            var maybe = AsOpt(ma);
            return maybe.IsSome()
                ? new Option<B>(OptionV<B>.Optional(f(maybe.Value())))
                : Option<B>.None;
        }

        /// <summary>
        /// Monad return
        /// 
        /// Constructs a Monad of A
        /// </summary>
        /// <param name="a">Value to bind</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public Monad<A> Return(A x, params A[] xs) =>
            new Option<A>(OptionV<A>.Optional(x));

        /// <summary>
        /// Monad bind
        /// </summary>
        /// <typeparam name="B">Type the bind operation returns</typeparam>
        /// <param name="ma">Monad of A</param>
        /// <param name="f">Bind operation</param>
        /// <returns>Monad of B</returns>
        [Pure]
        public Monad<B> Bind<B>(Monad<A> ma, Func<A, Monad<B>> f)
        {
            var maybe = AsOpt((Foldable<A>)ma);
            return maybe.IsSome()
                ? f(maybe.Value())
                : Option<B>.None;
        }

        /// <summary>
        /// Monad fail
        /// </summary>
        /// <param name="_">Not supported for OptionV</param>
        /// <returns>Monad of A (for Option this returns a None state)</returns>
        [Pure]
        public Monad<A> Fail(string _ = "") =>
            Option<A>.None;

        /// <summary>
        /// Applicative pure
        /// 
        /// Constructs an Applicative of A
        /// </summary>
        /// <param name="a">Value to bind</param>
        /// <returns>Applicative of A</returns>
        [Pure]
        public Applicative<A> Pure(A x, params A[] xs) =>
            new Option<A>(OptionV<A>.Optional(x));

        /// <summary>
        /// Applicative bind
        /// </summary>
        /// <typeparam name="B">The type of the bind result</typeparam>
        /// <param name="ma">Applicative of A</param>
        /// <param name="f">Bind operation to perform</param>
        /// <returns>Applicative of B</returns>
        [Pure]
        public Applicative<B> Bind<B>(Applicative<A> ma, Func<A, Applicative<B>> f)
        {
            var maybe = AsOpt(ma);
            return maybe.IsSome()
                ? f(maybe.Value())
                : Option<B>.None;
        }

        /// <summary>
        /// Fold the bound value
        /// </summary>
        /// <typeparam name="S">Initial state type</typeparam>
        /// <param name="ma">Monad to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="f">Fold operation</param>
        /// <returns>Aggregated state</returns>
        [Pure]
        public S Fold<S>(Foldable<A> ma, S state, Func<S, A, S> f)
        {
            var maybe = AsOpt(ma);
            return maybe.IsSome()
                ? f(state, maybe.Value())
                : state;
        }

        /// <summary>
        /// Fold the bound value
        /// </summary>
        /// <typeparam name="S">Initial state type</typeparam>
        /// <param name="ma">Monad to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="f">Fold operation</param>
        /// <returns>Aggregated state</returns>
        [Pure]
        public S FoldBack<S>(Foldable<A> ma, S state, Func<S, A, S> f)
        {
            var maybe = AsOpt(ma);
            return maybe.IsSome()
                ? f(state, maybe.Value())
                : state;
        }

        /// <summary>
        /// Returns true if the optional is in a Some state
        /// </summary>
        /// <param name="a">Optional to check</param>
        /// <returns>True if the optional is in a Some state</returns>
        public bool IsSomeA(Optional<A> a) =>
            ((Option<A>)a).IsSome;

        /// <summary>
        /// Returns true if the optional is in a None state
        /// </summary>
        /// <param name="a">Optional to check</param>
        /// <returns>True if the optional is in a None state</returns>
        public bool IsNoneA(Optional<A> a) =>
            ((Option<A>)a).IsNone;

        /// <summary>
        /// Extracts the bound value an maps it, a None function is available
        /// for when the Optional has no value to map.
        /// 
        /// Does not allow null values to be returned from Some or None
        /// </summary>
        /// <typeparam name="B">Type that the bound value is mapped to</typeparam>
        /// <param name="a">The optional structure</param>
        /// <param name="Some">The mapping function to call if the value is in a Some state</param>
        /// <param name="None">The mapping function to call if the value is in a None state</param>
        /// <returns>The bound value unwrapped and mapped to a value of type B</returns>
        public B Match<B>(Optional<A> a, Func<A, B> Some, Func<B> None) =>
            ((Option<A>)a).Match(Some, None);

        /// <summary>
        /// Extracts the bound value an maps it, a None function is available
        /// for when the Optional has no value to map.
        /// 
        /// Allows null values to be returned from Some or None (hence un Unsafe suffix)
        /// </summary>
        /// <typeparam name="B">Type that the bound value is mapped to</typeparam>
        /// <param name="a">The optional structure</param>
        /// <param name="Some">The mapping function to call if the value is in a Some state</param>
        /// <param name="None">The mapping function to call if the value is in a None state</param>
        /// <returns>The bound value unwrapped and mapped to a value of type B</returns>
        public B MatchUnsafe<B>(Optional<A> a, Func<A, B> Some, Func<B> None) =>
            ((Option<A>)a).MatchUnsafe(Some, None);

        /// <summary>
        /// True if the optional type allows nulls
        /// </summary>
        public bool IsUnsafe(Optional<A> a) => false;

        public MonadPlus<A> Plus(MonadPlus<A> a, MonadPlus<A> b)
        {
            var ma = AsOpt(a);
            return ma.IsSome()
                ? a
                : b;
        }

        public MonadPlus<A> Zero(MonadPlus<A> a) =>
            Option<A>.None;

        [Pure]
        static OptionV<A> AsOpt(Monad<A> a) => ((Option<A>)a).value ?? OptionV<A>.None;

        [Pure]
        static OptionV<A> AsOpt(Functor<A> a) => ((Option<A>)a).value ?? OptionV<A>.None;

        [Pure]
        static OptionV<A> AsOpt(Foldable<A> a) => ((Option<A>)a).value ?? OptionV<A>.None;
    }
}
