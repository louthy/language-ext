using System;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Collections.Generic;

namespace LanguageExt
{
    /// <summary>
    /// This struct wraps the discriminated union type Option<A> to make a type 
    /// that sits in the Monad, Applicative, Functor and Foldable type-classes.
    /// </summary>
    /// <typeparam name="A">Bound value</typeparam>
    public struct OptionM<A> : Monad<A>, Foldable<A>
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
        /// To sequence operation
        /// </summary>
        public IEnumerable<A> ToSeq(Seq<A> seq)
        {
            var maybe = Optional(seq);
            if(maybe.IsSome())
            {
                yield return maybe.Value();
            }
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
            var maybe = Optional(ma);
            return maybe.IsSome()
                ? new OptionM<B>(Option<B>.Optional(f(maybe.Value())))
                : OptionM<B>.None;
        }

        /// <summary>
        /// Option cast from Seq
        /// </summary>
        [Pure]
        private static Option<A> Optional(Seq<A> a) =>
            ((OptionM<A>)a).Value ?? Option<A>.None;

        /// <summary>
        /// Option cast from Functor
        /// </summary>
        [Pure]
        private static Option<A> Optional(Functor<A> a) =>
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
        private static SomeValue<A> Some(Option<A> a) =>
            (SomeValue<A>)a;

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
        public Monad<A> Return(A a) =>
            new OptionM<A>(Option<A>.Optional(a));

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
            var maybe = Optional(ma);
            return maybe is SomeValue<A>
                ? f(SomeA(maybe))
                : OptionM<B>.None;
        }

        /// <summary>
        /// Monad fail
        /// </summary>
        /// <param name="err">Optional error message - not supported for Option</param>
        /// <returns>Monad of A (for Option this returns a None state)</returns>
        [Pure]
        public Monad<A> Fail(string err = "") =>
            None;

        /// <summary>
        /// Applicative pure
        /// 
        /// Constructs an Applicative of A
        /// </summary>
        /// <param name="a">Value to bind</param>
        /// <returns>Applicative of A</returns>
        [Pure]
        public Applicative<A> Pure(A a) =>
            new OptionM<A>(Option<A>.Optional(a));

        /// <summary>
        /// Apply y to x
        /// </summary>
        [Pure]
        public Applicative<B> Apply<B>(Applicative<Func<A, B>> x, Applicative<A> y) =>
            from a in x
            from b in y
            select a(b);

        /// <summary>
        /// Apply y and z to x
        /// </summary>
        [Pure]
        public Applicative<C> Apply<B, C>(Applicative<Func<A, B, C>> x, Applicative<A> y, Applicative<B> z) =>
            from a in x
            from b in y
            from c in z
            select a(b, c);

        /// <summary>
        /// Apply y to x
        /// </summary>
        [Pure]
        public Applicative<Func<B, C>> Apply<B, C>(Applicative<Func<A, Func<B, C>>> x, Applicative<A> y) =>
            from a in x
            from b in y
            select a(b);

        /// <summary>
        /// Apply x, then y, ignoring the result of x
        /// </summary>
        [Pure]
        public Applicative<B> Action<B>(Applicative<A> x, Applicative<B> y) =>
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
        public Applicative<B> Bind<B>(Applicative<A> ma, Func<A, Applicative<B>> f)
        {
            var maybe = Optional(ma);
            return maybe is SomeValue<A>
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
            return maybe is SomeValue<A>
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
            return maybe is SomeValue<A>
                ? f(state, SomeA(maybe))
                : state;
        }
    }
}
