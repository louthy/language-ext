using System;
using System.Linq;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using LanguageExt.Instances;
using System.ComponentModel;

namespace LanguageExt
{
    /// <summary>
    /// Discriminated union type.  Can be in one of two states:
    /// 
    ///     Some(a)
    ///     
    ///     None
    ///     
    /// The type is part of the Optional, Monad, Applicative, Functor, 
    /// Foldable, and Seq, type-classes.
    /// </summary>
    /// <typeparam name="A">Bound value</typeparam>
#if !COREFX
    [Serializable]
#endif
    public struct Option<A> : 
        Optional<A>, 
        IOptional, 
        IEquatable<Option<A>>, 
        IComparable<Option<A>>
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

        /// <summary>
        /// Equality operator
        /// </summary>
        /// <remarks>
        /// This uses the EqDefault type-class for comparison of the A value.  The EqDefault
        /// type-class wraps up the .NET EqualityComparer.Default behaviour.  For more control
        /// over equality you can call:
        /// 
        ///     equals<EQ, A>(lhs, rhs);
        ///     
        /// Where EQ is a struct derived from Eq<A>.  For example: 
        /// 
        ///     equals<EqString, string>(lhs, rhs);
        ///     equals<EqArray<int>, int>(lhs, rhs);
        ///     
        /// </remarks>
        /// <param name="lhs">Left hand side of the operation</param>
        /// <param name="rhs">RIght hand side of the operation</param>
        /// <returns>True if the values are equal</returns>
        [Pure]
        public static bool operator ==(Option<A> lhs, Option<A> rhs) =>
            equals<EqDefault<A>, A>(lhs, rhs);

        /// <summary>
        /// Non-equality operator
        /// </summary>
        /// <remarks>
        /// This uses the EqDefault type-class for comparison of the A value.  The EqDefault
        /// type-class wraps up the .NET EqualityComparer.Default behaviour.  For more control
        /// over equality you can call:
        /// 
        ///     !equals<EQ, A>(lhs, rhs);
        ///     
        /// Where EQ is a struct derived from Eq<A>.  For example: 
        /// 
        ///     !equals<EqString, string>(lhs, rhs);
        ///     !equals<EqArray<int>, int>(lhs, rhs);
        ///     
        /// </remarks>
        /// <param name="lhs">Left hand side of the operation</param>
        /// <param name="rhs">RIght hand side of the operation</param>
        /// <returns>True if the values are equal</returns>
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
            equals<EqDefault<A>, A>(this, (ReferenceEquals(obj,null) ? None : (Option<A>)obj));

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

        /// <summary>
        /// Helper accessor for the bound value
        /// </summary>
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

        /// <summary>
        /// Match operation with an untyped value for Some. This can be
        /// useful for serialisation and dealing with the IOptional interface
        /// </summary>
        /// <typeparam name="R">The return type</typeparam>
        /// <param name="Some">Operation to perform if the option is in a Some state</param>
        /// <param name="None">Operation to perform if the option is in a None state</param>
        /// <returns>The result of the match operation</returns>
        public R MatchUntyped<R>(Func<object, R> Some, Func<R> None) =>
            this.Match(
                Some: x => Some(x),
                None: () => None()
            );

        public Type GetUnderlyingType() => 
            typeof(A);

        /// <summary>
        /// Convert the Option to an enumerable of zero or one items
        /// </summary>
        /// <param name="ma">Option</param>
        /// <returns>An enumerable of zero or one items</returns>
        public A[] ToArray() =>
            IsNone
                ? new A[0]
                : new A[1] { Value };

        /// <summary>
        /// Convert the Option to an immutable list of zero or one items
        /// </summary>
        /// <param name="ma">Option</param>
        /// <returns>An immutable list of zero or one items</returns>
        public Lst<A> ToList() =>
            List(ToArray());

        /// <summary>
        /// Convert the Option to an enumerable sequence of zero or one items
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ma">Option</param>
        /// <returns>An enumerable sequence of zero or one items</returns>
        public IEnumerable<A> ToSeq() =>
            ToArray().AsEnumerable();

        /// <summary>
        /// Convert the Option to an enumerable of zero or one items
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ma">Option</param>
        /// <returns>An enumerable of zero or one items</returns>
        public IEnumerable<A> AsEnumerable() =>
            ToArray().AsEnumerable();

        /// <summary>
        /// Fluent pattern matching.  Provide a Some handler and then follow
        /// on fluently with .None(...) to complete the matching operation.
        /// This is for dispatching actions, use Some<A,B>(...) to return a value
        /// from the match operation.
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ma">Option to match</param>
        /// <param name="f">The Some(x) match operation</param>
        [Pure]
        public SomeUnitContext<Option<A>, A> Some(Action<A> f) =>
            new SomeUnitContext<Option<A>, A>(this, f, false);

        /// <summary>
        /// Fluent pattern matching.  Provide a Some handler and then follow
        /// on fluently with .None(...) to complete the matching operation.
        /// This is for returning a value from the match operation, to dispatch
        /// an action instead, use Some<A>(...)
        /// </summary>
        /// <typeparam name="B">Match operation return value type</typeparam>
        /// <param name="f">The Some(x) match operation</param>
        /// <returns>The result of the match operation</returns>
        [Pure]
        public SomeContext<Option<A>, A, B> Some<B>(Func<A, B> f) =>
            new SomeContext<Option<A>, A, B>(this, f, false);

        /// <summary>
        /// Match the two states of the Option and return a non-null R.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. Must not return null.</param>
        /// <param name="None">None match operation. Must not return null.</param>
        /// <returns>A non-null R</returns>
        [Pure]
        public B Match<B>(Func<A, B> Some, Func<B> None) =>
            IsNone
                ? OptionExtensions.CheckNullNoneReturn(None())
                : OptionExtensions.CheckNullSomeReturn(Some(Value));

        /// <summary>
        /// Match the two states of the Option and return a B, which can be null.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. May return null.</param>
        /// <param name="None">None match operation. May return null.</param>
        /// <returns>R, or null</returns>
        [Pure]
        public B MatchUnsafe<B>(Func<A, B> Some, Func<B> None) =>
            IsNone
                ? None()
                : Some(Value);

        /// <summary>
        /// Match the two states of the Option A
        /// </summary>
        /// <param name="Some">Some match operation</param>
        /// <param name="None">None match operation</param>
        public Unit Match(Action<A> Some, Action None)
        {
            if (IsSome)
            {
                Some(Value);
            }
            else
            {
                None();
            }
            return unit;
        }

        /// <summary>
        /// Invokes the f action if Option is in the Some state, otherwise nothing happens.
        /// </summary>
        public Unit IfSome(Action<A> f)
        {
            if (IsSome)
            {
                f(Value);
            }
            return unit;
        }

        /// <summary>
        /// Invokes the f function if Option is in the Some state, otherwise nothing
        /// happens.
        /// </summary>
        public Unit IfSome(Func<A, Unit> f)
        {
            if (IsSome)
            {
                f(Value);
            }
            return unit;
        }

        /// <summary>
        /// Returns the result of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will not accept a null return value from the None operation</remarks>
        /// <param name="None">Operation to invoke if the structure is in a None state</param>
        /// <returns>Tesult of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.</returns>
        [Pure]
        public A IfNone(Func<A> None) =>
            Match(identity, None);

        /// <summary>
        /// Returns the noneValue if the optional is in a None state, otherwise
        /// the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will not accept a null noneValue</remarks>
        /// <param name="noneValue">Value to return if in a None state</param>
        /// <returns>noneValue if the optional is in a None state, otherwise
        /// the bound Some(x) value is returned</returns>
        [Pure]
        public A IfNone(A noneValue) =>
            Match(identity, () => noneValue);

        /// <summary>
        /// Returns the result of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will allow null the be returned from the None operation</remarks>
        /// <param name="None">Operation to invoke if the structure is in a None state</param>
        /// <returns>Tesult of invoking the None() operation if the optional 
        /// is in a None state, otherwise the bound Some(x) value is returned.</returns>
        [Pure]
        public A IfNoneUnsafe(Func<A> None) =>
            MatchUnsafe(identity, None);

        /// <summary>
        /// Returns the noneValue if the optional is in a None state, otherwise
        /// the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will allow noneValue to be null</remarks>
        /// <param name="noneValue">Value to return if in a None state</param>
        /// <returns>noneValue if the optional is in a None state, otherwise
        /// the bound Some(x) value is returned</returns>
        [Pure]
        public A IfNoneUnsafe(A noneValue) =>
            MatchUnsafe(identity, () => noneValue);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsSomeA(Optional<A> a) =>
            ((Option<A>)a).IsSome;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsNoneA(Optional<A> a) =>
            ((Option<A>)a).IsNone;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public B Match<B>(Optional<A> a, Func<A, B> Some, Func<B> None) =>
            ((Option<A>)a).Match(Some, None);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public B MatchUnsafe<B>(Optional<A> a, Func<A, B> Some, Func<B> None) =>
            ((Option<A>)a).MatchUnsafe(Some, None);

        /// <summary>
        /// True if the optional type allows nulls
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsUnsafe(Optional<A> a) => false;

        public bool Equals(Option<A> other) =>
            equals<EqDefault<A>, A>(this, other);

        public int CompareTo(Option<A> other) =>
            compare<OrdDefault<A>, A>(this, other);
    }
}
