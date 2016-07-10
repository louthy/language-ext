using System;
using static LanguageExt.TypeClass;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using LanguageExt.Instances;

namespace LanguageExt
{
    /// <summary>
    /// This struct wraps the discriminated union type OptionV<A> to make a type 
    /// that sits in the Monad, Applicative, Functor and Foldable type-classes.
    /// </summary>
    /// <typeparam name="A">Bound value</typeparam>
    public struct Option<A> : Monad<A>, Foldable<A>
    {
        internal readonly OptionV<A> value;

        /// <summary>
        /// Cached None of A
        /// </summary>
        public static readonly Option<A> None = new Option<A>(OptionV<A>.None);

        /// <summary>
        /// Construct an Option of A in a Some state
        /// </summary>
        /// <param name="value">Value to bind, must be non-null</param>
        /// <returns>Option of A</returns>
        public static Option<A> Some(A value) => 
            value;

        /// <summary>
        /// Takes the value-type OptionV<A>
        /// </summary>
        internal Option(OptionV<A> value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            this.value = value;
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
                ? new Option<B>(OptionV<B>.Optional(f(maybe.Value())))
                : Option<B>.None;
        }

        /// <summary>
        /// Option cast from Seq
        /// </summary>
        [Pure]
        private static OptionV<A> Optional(Seq<A> a) =>
            ((Option<A>)a).value ?? OptionV<A>.None;

        /// <summary>
        /// Option cast from Functor
        /// </summary>
        [Pure]
        private static OptionV<A> Optional(Functor<A> a) =>
            ((Option<A>)a).value ?? OptionV<A>.None;

        /// <summary>
        /// Option cast from Foldable
        /// </summary>
        [Pure]
        private static OptionV<A> Optional(Foldable<A> a) =>
            ((Option<A>)a).value ?? OptionV<A>.None;


        /// <summary>
        /// Monad return
        /// 
        /// Constructs a Monad of A
        /// </summary>
        /// <param name="a">Value to bind</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public Monad<A> Return(A a) =>
            new Option<A>(OptionV<A>.Optional(a));

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
            return maybe.IsSome()
                ? f(maybe.Value())
                : Option<B>.None;
        }

        /// <summary>
        /// Monad fail
        /// </summary>
        /// <param name="err">Optional error message - not supported for OptionV</param>
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
            new Option<A>(OptionV<A>.Optional(a));

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
        public S Fold<S>(Foldable<A> ma, S state, Func<S, A, S> f)
        {
            var maybe = Optional(ma);
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
        public S FoldBack<S>(Foldable<A> ma, S state, Func<S, A, S> f)
        {
            var maybe = Optional(ma);
            return maybe.IsSome()
                ? f(state, maybe.Value())
                : state;
        }

        /// <summary>
        /// Implicit conversion operator from A to Option<A>
        /// </summary>
        /// <param name="a">Unit value</param>
        [Pure]
        public static implicit operator Option<A>(A a) =>
            Prelude.Optional(a);

        /// Implicit conversion operator from Unit to Option<A>
        /// </summary>
        /// <param name="a">Unit value</param>
        [Pure]
        public static implicit operator Option<A>(OptionNone a) =>
            None;

        [Pure]
        public static bool operator ==(Option<A> lhs, Option<A> rhs) =>
            equals<EqDefault<A>, A>(lhs, rhs);

        [Pure]
        public static bool operator !=(Option<A> lhs, Option<A> rhs) =>
            !(lhs == rhs);

        [Pure]
        public static Option<A> operator |(Option<A> lhs, Option<A> rhs) =>
            lhs.IsSome
                ? lhs
                : rhs;

        [Pure]
        public static bool operator true(Option<A> value) =>
            value.IsSome;

        [Pure]
        public static bool operator false(Option<A> value) =>
            value.IsNone;

        /// <summary>
        /// DO NOT USE - Use the Structural equality variant of this method Equals<EQ, A>(y)
        /// </summary>
        [Pure]
        public override bool Equals(object obj) =>
            equals<EqDefault<A>, A>(this, (Option<A>)obj);

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() =>
            IsSome
                ? Value.GetHashCode()
                : 0;

        /// <summary>
        /// Get a string representation of the Option
        /// </summary>
        /// <returns></returns>
        public override string ToString() =>
            IsSome
                ? $"Some({Value})"
                : "None";

        /// <summary>
        /// Is the option in a Some state
        /// </summary>
        public bool IsSome =>
            (value ?? OptionV<A>.None).IsSome();

        /// <summary>
        /// Is the option in a None state
        /// </summary>
        public bool IsNone =>
            (value ?? OptionV<A>.None).IsNone();

        internal A Value =>
            IsSome
                ? value.Value()
                : default(A);

        /// <summary>
        /// Functor map operation
        /// </summary>
        [Pure]
        public Option<B> Select<B>(Func<A, B> f)
        {
            if (Prelude.isnull(f) || IsNone) return Option<B>.None;
            return f(Value);
        }

        /// <summary>
        /// Functor map operation
        /// </summary>
        [Pure]
        public Option<B> Map<B>(Func<A, B> f)
        {
            if (Prelude.isnull(f) || IsNone) return Option<B>.None;
            return f(Value);
        }

        /// <summary>
        /// Monad bind operation
        /// </summary>
        [Pure]
        public Option<B> Bind<B>(Func<A, Option<B>> f)
        {
            if (Prelude.isnull(f) || IsNone) return Option<B>.None;
            return f(Value);
        }

        /// <summary>
        /// Monad bind operation for Option
        /// </summary>
        [Pure]
        public Option<C> SelectMany<B, C>(
            Func<A, Option<B>> bind,
            Func<A, B, C> project
            )
        {
            if (Prelude.isnull(bind) || Prelude.isnull(project) || IsNone) return Option<C>.None;
            var mb = bind(Value);
            if (mb.IsNone) return Option<C>.None;
            return project(Value, mb.Value);
        }
    }
}
