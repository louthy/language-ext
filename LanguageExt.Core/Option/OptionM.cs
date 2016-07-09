using System;
using LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// Option monad
    /// </summary>
    /// <typeparam name="A">Bound value</typeparam>
    public struct OptionM<A> : M<A>, Foldable<A>
    {
        internal readonly Option<A> Value;

        /// <summary>
        /// Cached None of A
        /// </summary>
        public static readonly OptionM<A> None = new OptionM<A>(Option<A>.None);

        /// <summary>
        /// Takes the value-type Option<A>
        /// </summary>
        internal OptionM(Option<A> value)
        {
            Value = value;
        }

        /// <summary>
        /// Iterates the bound value and passes it to f
        /// </summary>
        /// <param name="ia">Iterable</param>
        /// <param name="f">Operation to perform on a</param>
        [Pure]
        public Unit Iter(Iterable<A> ia, Action<A> f)
        {
            if (ia == null) return unit;
            var maybe = Optional(ia);

            if (maybe is Some<A>)
            {
                f(((Some<A>)maybe).Value);
            }
            return unit;
        }

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
            if (ma == null) return OptionM<B>.None;
            var maybe = Optional(ma);

            return maybe is Some<A>
                ? new OptionM<B>(Option<B>.Optional(f(((Some<A>)maybe).Value)))
                : OptionM<B>.None;
        }

        /// <summary>
        /// Option cast from Iterable
        /// </summary>
        [Pure]
        private static Option<A> Optional(Iterable<A> a) =>
            ((OptionM<A>)a).Value ?? Option<A>.None;

        /// <summary>
        /// Option cast from Foldable
        /// </summary>
        [Pure]
        private static Option<A> Optional(Foldable<A> a) =>
            ((OptionM<A>)a).Value ?? Option<A>.None;

        /// <summary>
        /// Cast an Option to a Some
        /// </summary>
        [Pure]
        private static Some<A> Some(Option<A> a) =>
            (Some<A>)a;

        /// <summary>
        /// Cast an option to Some and return its bound value
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        [Pure]
        private static A SomeA(Option<A> a) =>
            Some(a).Value;

        /// <summary>
        /// Monad return
        /// 
        /// Constructs a Monad of A
        /// </summary>
        /// <param name="a">Value to bind</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public M<A> Return(A a) =>
            new OptionM<A>(Option<A>.Optional(a));

        /// <summary>
        /// Monad bind
        /// </summary>
        /// <typeparam name="B">Type the bind operation returns</typeparam>
        /// <param name="ma">Monad of A</param>
        /// <param name="f">Bind operation</param>
        /// <returns>Monad of B</returns>
        [Pure]
        public M<B> Bind<B>(M<A> ma, Func<A, M<B>> f)
        {
            if (ma == null) return OptionM<B>.None;
            var maybe = Optional(ma);
            return maybe is Some<A>
                ? f(SomeA(maybe))
                : OptionM<B>.None;
        }

        /// <summary>
        /// Monad fail
        /// </summary>
        /// <param name="err">Optional error message - not supported for Option</param>
        /// <returns>Monad of A (for Option this returns a None state)</returns>
        [Pure]
        public M<A> Fail(string err = "") =>
            None;

        /// <summary>
        /// Applicative pure
        /// 
        /// Constructs an Applicative of A
        /// </summary>
        /// <param name="a">Value to bind</param>
        /// <returns>Applicative of A</returns>
        [Pure]
        public AP<A> Pure(A a) =>
            Return(a);

        /// <summary>
        /// Apply y to x
        /// </summary>
        [Pure]
        public AP<B> Apply<B>(AP<Func<A, B>> x, AP<A> y) =>
            from a in x
            from b in y
            select a(b);

        /// <summary>
        /// Apply y and z to x
        /// </summary>
        [Pure]
        public AP<C> Apply<B, C>(AP<Func<A, B, C>> x, AP<A> y, AP<B> z) =>
            from a in x
            from b in y
            from c in z
            select a(b, c);

        /// <summary>
        /// Apply y to x
        /// </summary>
        [Pure]
        public AP<Func<B, C>> Apply<B, C>(AP<Func<A, Func<B, C>>> x, AP<A> y) =>
            from a in x
            from b in y
            select a(b);

        /// <summary>
        /// Apply x, then y, ignoring the result of x
        /// </summary>
        [Pure]
        public AP<B> Action<B>(AP<A> x, AP<B> y) =>
            from a in x
            from b in y
            select b;

        /// <summary>
        /// Applicative bind
        /// </summary>
        /// <typeparam name="B">The type of the bind result</typeparam>
        /// <param name="ma">Applicative of A</param>
        /// <param name="f">Bind operation to perform</param>
        /// <returns>Applicative of B</returns>
        [Pure]
        public AP<B> Bind<B>(AP<A> ma, Func<A, AP<B>> f)
        {
            var maybe = Optional(ma);
            return maybe is Some<A>
                ? f(SomeA(maybe))
                : OptionM<B>.None;
        }

        /// <summary>
        /// Fold the bound value
        /// </summary>
        /// <typeparam name="S">Initial state type</typeparam>
        /// <param name="ma">Monad to fold</param>
        /// <param name="state">Initial state</param>
        /// <param name="f">Fold operation</param>
        /// <returns>Aggregated state</returns>
        public S Fold<S>(Foldable<A> ma, S state, Func<S, A, S> f)
        {
            var maybe = Optional(ma);
            return maybe is Some<A>
                ? f(state, SomeA(maybe))
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
        public S FoldBack<S>(Foldable<A> ma, S state, Func<S, A, S> f)
        {
            var maybe = Optional(ma);
            return maybe is Some<A>
                ? f(state, SomeA(maybe))
                : state;
        }
    }
}
