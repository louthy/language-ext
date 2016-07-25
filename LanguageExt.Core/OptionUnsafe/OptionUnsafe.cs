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
    /// The type is part of the Monad, Applicative, Functor, Foldable, and
    /// Seq, type-classes.
    /// </summary>
    /// <typeparam name="A">Bound value</typeparam>
#if !COREFX
    [Serializable]
#endif
    public struct OptionUnsafe<A> : 
        Optional<A>, 
        MonadPlus<A>,
        IOptional,
        IEquatable<OptionUnsafe<A>>,
        IComparable<OptionUnsafe<A>>
    {
        internal readonly OptionV<A> value;

        /// <summary>
        /// Cached None of A
        /// </summary>
        public static readonly OptionUnsafe<A> None = new OptionUnsafe<A>(OptionV<A>.None);

        /// <summary>
        /// Construct an Option of A in a Some state
        /// </summary>
        /// <param name="value">Value to bind, must be non-null</param>
        /// <returns>Option of A</returns>
        public static OptionUnsafe<A> Some(A value) => 
            value;

        /// <summary>
        /// Takes the value-type OptionV<A>
        /// </summary>
        internal OptionUnsafe(OptionV<A> value)
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
            var maybe = AsOpt(ma);
            return maybe.IsSome()
                ? new OptionUnsafe<B>(OptionV<B>.Optional(f(maybe.Value())))
                : OptionUnsafe<B>.None;
        }

        /// <summary>
        /// Option cast from Functor
        /// </summary>
        [Pure]
        private static OptionV<A> AsOpt(Functor<A> a) =>
            ((OptionUnsafe<A>)a).value ?? OptionV<A>.None;

        /// <summary>
        /// Option cast from Foldable
        /// </summary>
        [Pure]
        private static OptionV<A> AsOpt(Foldable<A> a) =>
            ((OptionUnsafe<A>)a).value ?? OptionV<A>.None;


        /// <summary>
        /// Monad return
        /// 
        /// Constructs a Monad of A
        /// </summary>
        /// <param name="a">Value to bind</param>
        /// <returns>Monad of A</returns>
        [Pure]
        public Monad<A> Return(A x, params A[] xs) =>
            new OptionUnsafe<A>(OptionV<A>.Optional(x));

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
                : OptionUnsafe<B>.None;
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
        public Applicative<A> Pure(A x, params A[] xs) =>
            new OptionUnsafe<A>(OptionV<A>.Optional(x));

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
                : OptionUnsafe<B>.None;
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
        public S FoldBack<S>(Foldable<A> ma, S state, Func<S, A, S> f)
        {
            var maybe = AsOpt(ma);
            return maybe.IsSome()
                ? f(state, maybe.Value())
                : state;
        }

        /// <summary>
        /// Implicit conversion operator from A to Option<A>
        /// </summary>
        /// <param name="a">Unit value</param>
        [Pure]
        public static implicit operator OptionUnsafe<A>(A a) =>
            Prelude.SomeUnsafe(a);

        /// Implicit conversion operator from Unit to Option<A>
        /// </summary>
        /// <param name="a">Unit value</param>
        [Pure]
        public static implicit operator OptionUnsafe<A>(OptionNone a) =>
            None;

        [Pure]
        public static bool operator ==(OptionUnsafe<A> lhs, OptionUnsafe<A> rhs) =>
            equals<EqDefault<A>, A>(lhs, rhs);

        [Pure]
        public static bool operator !=(OptionUnsafe<A> lhs, OptionUnsafe<A> rhs) =>
            !(lhs == rhs);

        [Pure]
        public static OptionUnsafe<A> operator |(OptionUnsafe<A> lhs, OptionUnsafe<A> rhs) =>
            lhs.IsSome
                ? lhs
                : rhs;

        [Pure]
        public static bool operator true(OptionUnsafe<A> value) =>
            value.IsSome;

        [Pure]
        public static bool operator false(OptionUnsafe<A> value) =>
            value.IsNone;

        /// <summary>
        /// DO NOT USE - Use the Structural equality variant of this method Equals<EQ, A>(y)
        /// </summary>
        [Pure]
        public override bool Equals(object obj) =>
            equals<EqDefault<A>, A>(this, (ReferenceEquals(obj, null) ? None : (OptionUnsafe<A>)obj));

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
        public OptionUnsafe<B> Select<B>(Func<A, B> f)
        {
            if (isnull(f) || IsNone) return OptionUnsafe<B>.None;
            return f(Value);
        }

        /// <summary>
        /// Functor map operation
        /// </summary>
        [Pure]
        public OptionUnsafe<B> Map<B>(Func<A, B> f)
        {
            if (isnull(f) || IsNone) return OptionUnsafe<B>.None;
            return f(Value);
        }

        /// <summary>
        /// Monad bind operation
        /// </summary>
        [Pure]
        public OptionUnsafe<B> Bind<B>(Func<A, OptionUnsafe<B>> f)
        {
            if (isnull(f) || IsNone) return OptionUnsafe<B>.None;
            return f(Value);
        }

        /// <summary>
        /// Monad bind operation for Option
        /// </summary>
        [Pure]
        public OptionUnsafe<C> SelectMany<B, C>(
            Func<A, OptionUnsafe<B>> bind,
            Func<A, B, C> project
            )
        {
            if (isnull(bind) || isnull(project) || IsNone) return OptionUnsafe<C>.None;
            var mb = bind(Value);
            if (mb.IsNone) return OptionUnsafe<C>.None;
            return project(Value, mb.Value);
        }

        public R MatchUntyped<R>(Func<object, R> Some, Func<R> None) =>
            MatchUnsafe(
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
        [Pure]
        public A[] ToArray() =>
            IsNone
                ? new A[0]
                : new A[1] { Value };

        /// <summary>
        /// Convert the Option to an immutable list of zero or one items
        /// </summary>
        /// <param name="ma">Option</param>
        /// <returns>An immutable list of zero or one items</returns>
        [Pure]
        public Lst<A> ToList() =>
            List(ToArray());

        /// <summary>
        /// Convert the Option to an enumerable sequence of zero or one items
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ma">Option</param>
        /// <returns>An enumerable sequence of zero or one items</returns>
        [Pure]
        public IEnumerable<A> ToSeq() =>
            ToArray().AsEnumerable();

        /// <summary>
        /// Convert the Option to an enumerable of zero or one items
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <param name="ma">Option</param>
        /// <returns>An enumerable of zero or one items</returns>
        [Pure]
        public IEnumerable<A> AsEnumerable() =>
            ToArray().AsEnumerable();

        /// <summary>
        /// Convert the structure to an Either
        /// </summary>
        [Pure]
        public Either<L, A> ToEither<L>(L defaultLeftValue) =>
            IsSome
                ? Right<L, A>(Value)
                : Left<L, A>(defaultLeftValue);

        /// <summary>
        /// Convert the structure to an Either
        /// </summary>
        [Pure]
        public Either<L, A> ToEither<L>(Func<L> Left) =>
            IsSome
                ? Right<L, A>(Value)
                : Left<L, A>(Left());

        /// <summary>
        /// Convert the structure to an EitherUnsafe
        /// </summary>
        [Pure]
        public EitherUnsafe<L, A> ToEitherUnsafe<L>(L defaultLeftValue) =>
            IsSome
                ? RightUnsafe<L, A>(Value)
                : LeftUnsafe<L, A>(defaultLeftValue);

        /// <summary>
        /// Convert the structure to an EitherUnsafe
        /// </summary>
        [Pure]
        public EitherUnsafe<L, A> ToEitherUnsafe<L>(Func<L> Left) =>
            IsSome
                ? RightUnsafe<L, A>(Value)
                : LeftUnsafe<L, A>(Left());

        /// <summary>
        /// Convert the structure to a OptionUnsafe
        /// </summary>
        [Pure]
        public Option<A> ToOption() =>
            IsSome && !isnull(Value)
                ? Option<A>.Some(Value)
                : Option<A>.None;

        /// <summary>
        /// Convert the structure to a TryOption
        /// </summary>
        [Pure]
        public TryOption<A> ToTryOption<L>(L defaultLeftValue)
        {
            var self = this;
            return () => self.ToOption();
        }

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
        public SomeUnitContext<OptionUnsafe<A>, A> Some(Action<A> f) =>
            new SomeUnitContext<OptionUnsafe<A>, A>(this, f, true);

        /// <summary>
        /// Fluent pattern matching.  Provide a Some handler and then follow
        /// on fluently with .None(...) to complete the matching operation.
        /// This is for returning a value from the match operation, to dispatch
        /// an action instead, use Some<A>(...)
        /// </summary>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <typeparam name="B">Match operation return value type</typeparam>
        /// <param name="ma">Option to match</param>
        /// <param name="f">The Some(x) match operation</param>
        /// <returns>The result of the match operation</returns>
        [Pure]
        public SomeContext<OptionUnsafe<A>, A, B> SomeUnsafe<B>(Func<A, B> someHandler) =>
            new SomeContext<OptionUnsafe<A>, A, B>(this, someHandler, true);

        /// <summary>
        /// Match the two states of the Option and return a non-null R.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="None">None handler.  Must not return null.</param>
        /// <returns>A non-null R</returns>
        [Pure]
        public B MatchUnsafe<B>(Func<A, B> Some, Func<B> None) =>
            IsNone
                ? None()
                : Some(Value);

        /// <summary>
        /// Match the two states of the Option A
        /// </summary>
        /// <param name="Some">Some match</param>
        /// <param name="None">None match</param>
        /// <returns></returns>
        public Unit MatchUnsafe(Action<A> Some, Action None)
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
        /// Invokes the someHandler if Option is in the Some state, otherwise nothing
        /// happens.
        /// </summary>
        public Unit IfSomeUnsafe(Action<A> someHandler)
        {
            if (IsSome)
            {
                someHandler(Value);
            }
            return unit;
        }

        /// <summary>
        /// Invokes the someHandler if Option is in the Some state, otherwise nothing
        /// happens.
        /// </summary>
        public Unit IfSomeUnsafe(Func<A, Unit> someHandler)
        {
            if (IsSome)
            {
                someHandler(Value);
            }
            return unit;
        }

        [Pure]
        public A IfNoneUnsafe(Func<A> None) =>
            MatchUnsafe(identity, None);

        [Pure]
        public A IfNoneUnsafe(A noneValue) =>
            MatchUnsafe(identity, () => noneValue);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsSomeA(Optional<A> a) =>
            ((OptionUnsafe<A>)a).IsSome;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsNoneA(Optional<A> a) =>
            ((OptionUnsafe<A>)a).IsNone;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public B Match<B>(Optional<A> a, Func<A, B> Some, Func<B> None) =>
            ((OptionUnsafe<A>)a).MatchUnsafe(Some, None);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public B MatchUnsafe<B>(Optional<A> a, Func<A, B> Some, Func<B> None) =>
            ((OptionUnsafe<A>)a).MatchUnsafe(Some, None);

        /// <summary>
        /// True if the optional type allows nulls
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsUnsafe(Optional<A> a) => true;

        public bool Equals(OptionUnsafe<A> other) =>
            equals<EqDefault<A>, A>(this, other);

        public int CompareTo(OptionUnsafe<A> other) =>
            compare<OrdDefault<A>, A>(this, other);

        public MonadPlus<A> Plus(MonadPlus<A> a, MonadPlus<A> b)
        {
            var ma = AsOpt((Functor<A>)a);
            var mb = AsOpt((Functor<A>)b);
            if (ma.IsNone() && mb.IsNone()) return None; // 0 solutions + 0 solutions = 0 solutions
            if (ma.IsSome() && mb.IsNone()) return a;    // 1 solutions + 0 solutions = 1 solutions
            if (ma.IsNone() && mb.IsSome()) return b;    // 0 solutions + 1 solutions = 1 solutions
            return a;                                    // 1 solutions + 1 solutions = 2 solutions
                                                         // but Option can only have up to one solution,
                                                         // so we disregard the second one.
        }

        public MonadPlus<A> Zero(MonadPlus<A> a) =>
            None;
    }
}
