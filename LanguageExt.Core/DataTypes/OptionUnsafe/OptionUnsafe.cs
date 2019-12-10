using System;
using System.Linq;
using static LanguageExt.TypeClass;
using static LanguageExt.OptionalUnsafe;
using static LanguageExt.Prelude;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using LanguageExt.ClassInstances;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Collections;
using System.Runtime.CompilerServices;

namespace LanguageExt
{
    /// <summary>
    /// Discriminated union type.  Can be in one of two states:
    /// 
    ///     Some(a)
    ///     None
    ///     
    /// Typeclass instances available for this type:
    /// 
    ///     BiFoldable  : MOptionUnsafe
    ///     Eq          : EqOpt
    ///     Foldable    : MOptionUnsafe
    ///     Functor     : FOptionUnsafe
    ///     MonadPlus   : MOptionUnsafe
    ///     Optional    : MOptionUnsafe
    ///     Ord         : OrdOpt
    /// </summary>
    /// <typeparam name="A">Bound value</typeparam>
    public struct OptionUnsafe<A> :
        IEnumerable<A>,
        IOptional,
        IEquatable<OptionUnsafe<A>>,
        IComparable<OptionUnsafe<A>>
    {
        internal readonly A Value;
        internal readonly bool isSome;

        /// <summary>
        /// None
        /// </summary>
        public static readonly OptionUnsafe<A> None =
            default;

        /// <summary>
        /// Construct an OptionUnsafe of A in a Some state
        /// </summary>
        /// <param name="value">Value to bind, must be non-null</param>
        /// <returns>OptionUnsafe of A</returns>
        [Pure]
        public static OptionUnsafe<A> Some(A value) =>
            new OptionUnsafe<A>(value, true);

        /// <summary>
        /// Constructor
        /// </summary>
        internal OptionUnsafe(A value, bool isSome)
        {
            Value = value;
            this.isSome = isSome;
        }

        /// <summary>
        /// Ctor that facilitates serialisation
        /// </summary>
        /// <param name="option">None or Some A.</param>
        [Pure]
        public OptionUnsafe(IEnumerable<A> option)
        {
            var first = option.Take(1).ToArray();
            isSome = first.Length == 1;
            Value = isSome
                ? first[0]
                : default;
        }

        [Pure]
        OptionUnsafe(SerializationInfo info, StreamingContext context)
        {
            isSome = (bool)info.GetValue("IsSome", typeof(bool));
            if (isSome)
            {
                Value = (A)info.GetValue("Value", typeof(A));
            }
            else
            {
                Value = default;
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IsSome", IsSome);
            if (IsSome) info.AddValue("Value", Value);
        }

        /// <summary>
        /// Reference version of option for use in pattern-matching
        /// </summary>
        [Pure]
        public OptionCase<A> Case =>
            IsSome
                ? SomeCase<A>.New(Value)
                : NoneCase<A>.Default;

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<A> GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IEnumerator IEnumerable.GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        /// <summary>
        /// Uses the `EqDefault` instance to do an equality check on the bound value.  
        /// To use anything other than the default call `oa.Equals<EqA>(ob)`
        /// where `EqA` is an instance derived from `Eq<A>`
        /// </summary>
        /// <remarks>
        /// This uses the `EqDefault` instance for comparison of the bound `A` values.  
        /// The `EqDefault` instance wraps up the .NET `EqualityComparer.Default`
        /// behaviour.  
        /// </remarks>
        /// <param name="other">The `OptionUnsafe` type to compare this type with</param>
        /// <returns>`True` if `this` and `other` are equal</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(OptionUnsafe<A> other) =>
            Equals<EqDefault<A>>(other);

        /// <summary>
        /// Uses the `EqA` instance to do an equality check on the bound value.  
        /// </summary>
        /// <param name="other">The `OptionUnsafe` type to compare this type with</param>
        /// <returns>`True` if `this` and `other` are equal</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals<EqA>(OptionUnsafe<A> other) where EqA : struct, Eq<A>
        {
            var yIsSome = other.IsSome;
            var xIsNone = !isSome;
            var yIsNone = !yIsSome;
            return (xIsNone && yIsNone) || (isSome && yIsSome && default(EqA).Equals(Value, other.Value));
        }

        /// <summary>
        /// Uses the `OrdDefault` instance to do an ordering comparison on the bound 
        /// value.  To use anything other than the default call  `this.Compare<OrdA>(this, other)`, 
        /// where `OrdA` is an instance derived  from `Ord<A>`
        /// </summary>
        /// <param name="other">The `OptionUnsafe` type to compare `this` type with</param>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo(OptionUnsafe<A> other) =>
            CompareTo<OrdDefault<A>>(other);

        /// <summary>
        /// Uses the `Ord` instance provided to do an ordering comparison on the bound 
        /// value.  
        /// </summary>
        /// <param name="other">The `OptionUnsafe` type to compare `this` type with</param>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int CompareTo<OrdA>(OptionUnsafe<A> other) where OrdA : struct, Ord<A>
        {
            var yIsSome = other.IsSome;
            var xIsNone = !isSome;
            var yIsNone = !yIsSome;

            if (xIsNone && yIsNone) return 0;
            if (isSome && yIsNone) return 1;
            if (xIsNone && yIsSome) return -1;

            return default(OrdA).Compare(Value, other.Value);
        }

        /// <summary>
        /// Implicit conversion operator from A to OptionUnsafe<A>
        /// </summary>
        /// <param name="a">Unit value</param>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator OptionUnsafe<A>(A a) =>
            OptionUnsafe<A>.Some(a);

        /// Implicit conversion operator from None to OptionUnsafe<A>
        /// </summary>
        /// <param name="a">None value</param>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator OptionUnsafe<A>(OptionNone a) =>
            default;

        /// <summary>
        /// Implicit conversion operator from `Option<A>` to `A1
        /// </summary>
        /// <param name="a">None value</param>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator A(OptionUnsafe<A> ma) =>
            ma.isSome
                ? ma.Value
                : throw new InvalidCastException("OptionNone is not in a Some state");

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="lhs">The left hand side of the operation</param>
        /// <param name="rhs">The right hand side of the operation</param>
        /// <returns>True if lhs < rhs</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(OptionUnsafe<A> lhs, OptionUnsafe<A> rhs) =>
            lhs.CompareTo(rhs) < 0;

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="lhs">The left hand side of the operation</param>
        /// <param name="rhs">The right hand side of the operation</param>
        /// <returns>True if lhs <= rhs</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(OptionUnsafe<A> lhs, OptionUnsafe<A> rhs) =>
            lhs.CompareTo(rhs) <= 0;

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="lhs">The left hand side of the operation</param>
        /// <param name="rhs">The right hand side of the operation</param>
        /// <returns>True if lhs > rhs</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(OptionUnsafe<A> lhs, OptionUnsafe<A> rhs) =>
            lhs.CompareTo(rhs) > 0;

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="lhs">The left hand side of the operation</param>
        /// <param name="rhs">The right hand side of the operation</param>
        /// <returns>True if lhs >= rhs</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(OptionUnsafe<A> lhs, OptionUnsafe<A> rhs) =>
            lhs.CompareTo(rhs) >= 0;

        /// <summary>
        /// Equality operator
        /// </summary>
        /// <remarks>
        /// This uses the EqDefault instance for comparison of the bound A values.  
        /// The EqDefault instance wraps up the .NET EqualityComparer.Default 
        /// behaviour.  For more control over equality you can call:
        /// 
        ///     equals<EQ, A>(lhs, rhs);
        ///     
        /// Where EQ is a struct derived from Eq<A>.  For example: 
        /// 
        ///     equals<EqString, string>(lhs, rhs);
        ///     equals<EqArray<int>, int[]>(lhs, rhs);
        ///     
        /// </remarks>
        /// <param name="lhs">Left hand side of the operation</param>
        /// <param name="rhs">Right hand side of the operation</param>
        /// <returns>True if the values are equal</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(OptionUnsafe<A> lhs, OptionUnsafe<A> rhs) =>
            lhs.Equals(rhs);

        /// <summary>
        /// Non-equality operator
        /// </summary>
        /// <remarks>
        /// This uses the EqDefault instance for comparison of the A value.  
        /// The EqDefault type-class wraps up the .NET EqualityComparer.Default 
        /// behaviour.  For more control over equality you can call:
        /// 
        ///     !equals<EQ, A>(lhs, rhs);
        ///     
        /// Where EQ is a struct derived from Eq<A>.  For example: 
        /// 
        ///     !equals<EqString, string>(lhs, rhs);
        ///     !equals<EqArray<int>, int[]>(lhs, rhs);
        ///     
        /// </remarks>
        /// <param name="lhs">Left hand side of the operation</param>
        /// <param name="rhs">Right hand side of the operation</param>
        /// <returns>True if the values are equal</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(OptionUnsafe<A> lhs, OptionUnsafe<A> rhs) =>
            !lhs.Equals(rhs);

        /// <summary>
        /// Coalescing operator
        /// </summary>
        /// <param name="lhs">Left hand side of the operation</param>
        /// <param name="rhs">Right hand side of the operation</param>
        /// <returns>if lhs is Some then lhs, else rhs</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static OptionUnsafe<A> operator |(OptionUnsafe<A> lhs, OptionUnsafe<A> rhs) =>
            lhs.isSome
                ? lhs
                : rhs;

        /// <summary>
        /// Truth operator
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator true(OptionUnsafe<A> value) =>
            value.isSome;

        /// <summary>
        /// Falsity operator
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator false(OptionUnsafe<A> value) =>
            value.IsNone;

        /// <summary>
        /// DO NOT USE - Use the Structural equality variant of this method Equals<EQ, A>(y)
        /// </summary>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object obj) =>
            obj is OptionUnsafe<A> opt && Equals(opt);

        /// <summary>
        /// Calculate the hash-code from the bound value, unless the OptionUnsafe is in a None
        /// state, in which case the hash-code will be 0
        /// </summary>
        /// <returns>Hash-code from the bound value, unless the OptionUnsafe is in a None
        /// state, in which case the hash-code will be 0</returns>
        [Pure]
        public override int GetHashCode() =>
            isSome
                ? Value?.GetHashCode() ?? 0
                : 0;

        /// <summary>
        /// Get a string representation of the OptionUnsafe
        /// </summary>
        /// <returns>String representation of the OptionUnsafe</returns>
        [Pure]
        public override string ToString() =>
            isSome
                ? $"Some({Value})"
                : "None";

        /// <summary>
        /// Is the option in a Some state
        /// </summary>
        [Pure]
        public bool IsSome =>
            isSome;

        /// <summary>
        /// Is the option in a None state
        /// </summary>
        [Pure]
        public bool IsNone =>
            !isSome;

        /// <summary>
        /// Impure iteration of the bound value in the structure
        /// </summary>
        /// <returns>
        /// Returns the original unmodified structure
        /// </returns>
        public OptionUnsafe<A> Do(Action<A> f)
        {
            Iter(f);
            return this;
        }

        /// <summary>
        /// Projection from one value to another 
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public OptionUnsafe<B> Select<B>(Func<A, B> f) =>
            isSome
                ? OptionUnsafe<B>.Some(f(Value))
                : default;

        /// <summary>
        /// Projection from one value to another 
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public OptionUnsafe<B> Map<B>(Func<A, B> f) =>
            isSome
                ? OptionUnsafe<B>.Some(f(Value))
                : default;

        /// <summary>
        /// Monad bind operation
        /// </summary>
        [Pure]
        public OptionUnsafe<B> Bind<B>(Func<A, OptionUnsafe<B>> f) =>
            isSome
                ? f(Value)
                : default;

        /// <summary>
        /// Bi-bind.  Allows mapping of both monad states
        /// </summary>
        [Pure]
        public OptionUnsafe<B> BiBind<B>(Func<A, OptionUnsafe<B>> Some, Func<OptionUnsafe<B>> None) =>
            isSome
                ? Some(Value)
                : None();

        /// <summary>
        /// Monad bind operation
        /// </summary>
        [Pure]
        public OptionUnsafe<C> SelectMany<B, C>(
            Func<A, OptionUnsafe<B>> bind,
            Func<A, B, C> project)
        {
            if (IsNone) return default;
            var mb = bind(Value);
            if (mb.IsNone) return default;
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
        [Pure]
        public R MatchUntyped<R>(Func<object, R> Some, Func<R> None) =>
            matchUntypedUnsafe<MOptionUnsafe<A>, OptionUnsafe<A>, A, R>(this, Some, None);

        /// <summary>
        /// Match operation with an untyped value for Some. This can be
        /// useful for serialisation and dealing with the IOptional interface
        /// </summary>
        /// <typeparam name="R">The return type</typeparam>
        /// <param name="Some">Operation to perform if the option is in a Some state</param>
        /// <param name="None">Operation to perform if the option is in a None state</param>
        /// <returns>The result of the match operation</returns>
        [Pure]
        public R MatchUntypedUnsafe<R>(Func<object, R> Some, Func<R> None) =>
            matchUntypedUnsafe<MOptionUnsafe<A>, OptionUnsafe<A>, A, R>(this, Some, None);

        /// <summary>
        /// Get the Type of the bound value
        /// </summary>
        /// <returns>Type of the bound value</returns>
        [Pure]
        public Type GetUnderlyingType() =>
            typeof(A);

        /// <summary>
        /// Convert the OptionUnsafe to an enumerable of zero or one items
        /// </summary>
        /// <returns>An enumerable of zero or one items</returns>
        [Pure]
        public Arr<A> ToArray() =>
            isSome
                ? Arr.create(Value)
                : Empty;

        /// <summary>
        /// Convert the OptionUnsafe to an immutable list of zero or one items
        /// </summary>
        /// <returns>An immutable list of zero or one items</returns>
        [Pure]
        public Lst<A> ToList() =>
            isSome
                ? List.create(Value)
                : Empty;

        /// <summary>
        /// Convert the OptionUnsafe to an enumerable sequence of zero or one items
        /// </summary>
        /// <returns>An enumerable sequence of zero or one items</returns>
        [Pure]
        public Seq<A> ToSeq() =>
            isSome
                ? Seq1(Value)
                : Empty;

        /// <summary>
        /// Convert the OptionUnsafe to an enumerable of zero or one items
        /// </summary>
        /// <returns>An enumerable of zero or one items</returns>
        [Pure]
        public IEnumerable<A> AsEnumerable() =>
            ToSeq();

        [Pure]
        public Validation<FAIL, A> ToValidation<FAIL>(FAIL defaultFailureValue) =>
            isSome
                ? Success<FAIL, A>(Value)
                : Fail<FAIL, A>(defaultFailureValue);

        /// <summary>
        /// Convert the structure to an EitherUnsafe
        /// </summary>
        /// <param name="defaultLeftValue">Default value if the structure is in a None state</param>
        /// <returns>An Either representation of the structure</returns>
        [Pure]
        public EitherUnsafe<L, A> ToEither<L>(L defaultLeftValue) =>
            isSome
                ? RightUnsafe<L, A>(Value)
                : LeftUnsafe<L, A>(defaultLeftValue);

        /// <summary>
        /// Convert the structure to an EitherUnsafe
        /// </summary>
        /// <param name="Left">Function to invoke to get a default value if the 
        /// structure is in a None state</param>
        /// <returns>An Either representation of the structure</returns>
        [Pure]
        public EitherUnsafe<L, A> ToEither<L>(Func<L> Left) =>
            isSome
                ? RightUnsafe<L, A>(Value)
                : LeftUnsafe<L, A>(Left());

        /// <summary>
        /// Fluent pattern matching.  Provide a Some handler and then follow
        /// on fluently with .None(...) to complete the matching operation.
        /// This is for dispatching actions, use Some<A,B>(...) to return a value
        /// from the match operation.
        /// </summary>
        /// <param name="f">The Some(x) match operation</param>
        [Pure]
        public SomeUnsafeUnitContext<MOptionUnsafe<A>, OptionUnsafe<A>, A> Some(Action<A> f) =>
            new SomeUnsafeUnitContext<MOptionUnsafe<A>, OptionUnsafe<A>, A>(this, f);

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
        public SomeUnsafeContext<MOptionUnsafe<A>, OptionUnsafe<A>, A, B> Some<B>(Func<A, B> f) =>
            new SomeUnsafeContext<MOptionUnsafe<A>, OptionUnsafe<A>, A, B>(this, f);

        /// <summary>
        /// Match the two states of the OptionUnsafe and return a B, which can be null.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. May return null.</param>
        /// <param name="None">None match operation. May return null.</param>
        /// <returns>B, or null</returns>
        [Pure]
        public B MatchUnsafe<B>(Func<A, B> Some, Func<B> None) =>
            isSome
                ? Some(Value)
                : None();

        /// <summary>
        /// Match the two states of the OptionUnsafe and return a B, which can be null.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. May return null.</param>
        /// <param name="None">None match operation. May return null.</param>
        /// <returns>B, or null</returns>
        [Pure]
        public B MatchUnsafe<B>(Func<A, B> Some, B None) =>
            isSome
                ? Some(Value)
                : None;

        /// <summary>
        /// Match the two states of the OptionUnsafe
        /// </summary>
        /// <param name="Some">Some match operation</param>
        /// <param name="None">None match operation</param>
        public Unit MatchUnsafe(Action<A> Some, Action None)
        {
            if (isSome)
            {
                Some(Value);
            }
            else
            {
                None();
            }
            return default;
        }

        /// <summary>
        /// Invokes the action if OptionUnsafe is in the Some state, otherwise nothing happens.
        /// </summary>
        /// <param name="f">Action to invoke if OptionUnsafe is in the Some state</param>
        public Unit IfSomeUnsafe(Action<A> f)
        {
            if (isSome)
            {
                f(Value);
            }
            return default;
        }

        /// <summary>
        /// Invokes the f function if OptionUnsafe is in the Some state, otherwise nothing
        /// happens.
        /// </summary>
        /// <param name="f">Function to invoke if OptionUnsafe is in the Some state</param>
        public Unit IfSomeUnsafe(Func<A, Unit> f)
        {
            if (isSome)
            {
                f(Value);
            }
            return default;
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
        public A IfNoneUnsafe(Func<A> None) =>
            isSome
                ? Value
                : None();

        /// <summary>
        /// Returns the noneValue if the optional is in a None state, otherwise
        /// the bound Some(x) value is returned.
        /// </summary>
        /// <remarks>Will not accept a null noneValue</remarks>
        /// <param name="noneValue">Value to return if in a None state</param>
        /// <returns>noneValue if the optional is in a None state, otherwise
        /// the bound Some(x) value is returned</returns>
        [Pure]
        public A IfNoneUnsafe(A noneValue) =>
            isSome
                ? Value
                : noneValue;

        /// <summary>
        /// <para>
        /// OptionUnsafe types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para><para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Folder function, applied if OptionUnsafe is in a Some state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public S Fold<S>(S state, Func<S, A, S> folder) =>
            isSome
                ? folder(state, Value)
                : state;

        /// <summary>
        /// <para>
        /// OptionUnsafe types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para><para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Folder function, applied if OptionUnsafe is in a Some state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public S FoldBack<S>(S state, Func<S, A, S> folder) =>
            isSome
                ? folder(state, Value)
                : state;

        /// <summary>
        /// <para>
        /// OptionUnsafe types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para><para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Folder function, applied if OptionUnsafe is in a Some state</param>
        /// <param name="None">Folder function, applied if OptionUnsafe is in a None state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public S BiFold<S>(S state, Func<S, A, S> Some, Func<S, Unit, S> None) =>
            isSome
                ? Some(state, Value)
                : None(state, unit);

        /// <summary>
        /// <para>
        /// OptionUnsafe types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the left-identity of the operator),
        /// and a list, reduces the list using the binary operator, from left to
        /// right:
        /// </para><para>
        /// Note that, since the head of the resulting expression is produced by
        /// an application of the operator to the first element of the list,
        /// 'Fold' can produce a terminating expression from an infinite list.
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Some">Folder function, applied if OptionUnsafe is in a Some state</param>
        /// <param name="None">Folder function, applied if OptionUnsafe is in a None state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public S BiFold<S>(S state, Func<S, A, S> Some, Func<S, S> None) =>
            isSome
                ? Some(state, Value)
                : None(state);

        /// <summary>
        /// Projection from one value to another
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="Some">Projection function</param>
        /// <param name="None">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public OptionUnsafe<B> BiMap<B>(Func<A, B> Some, Func<Unit, B> None) =>
            isSome
                ? Some(Value)
                : None(unit);

        /// <summary>
        /// Projection from one value to another
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="Some">Projection function</param>
        /// <param name="None">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public OptionUnsafe<B> BiMap<B>(Func<A, B> Some, Func<B> None) =>
            isSome
                ? Some(Value)
                : None();

        /// <summary>
        /// <para>
        /// Return the number of bound values in this structure:
        /// </para>
        /// <para>
        ///     None = 0
        /// </para>
        /// <para>
        ///     Some = 1
        /// </para>
        /// </summary>
        /// <returns></returns>
        [Pure]
        public int Count() =>
            isSome ? 1 : 0;

        /// <summary>
        /// Apply a predicate to the bound value.  If the OptionUnsafe is in a None state
        /// then True is returned (because the predicate applies for-all values).
        /// If the OptionUnsafe is in a Some state the value is the result of running 
        /// applying the bound value to the predicate supplied.        
        /// </summary>
        /// <param name="pred"></param>
        /// <returns>If the OptionUnsafe is in a None state then True is returned (because 
        /// the predicate applies for-all values).  If the OptionUnsafe is in a Some state
        /// the value is the result of running applying the bound value to the 
        /// predicate supplied.</returns>
        [Pure]
        public bool ForAll(Func<A, bool> pred) =>
            isSome
                ? pred(Value)
                : true;

        /// <summary>
        /// Apply a predicate to the bound value.  If the OptionUnsafe is in a None state
        /// then True is returned if invoking None returns True.
        /// If the OptionUnsafe is in a Some state the value is the result of running 
        /// applying the bound value to the Some predicate supplied.        
        /// </summary>
        /// <param name="Some">Predicate to apply if in a Some state</param>
        /// <param name="None">Predicate to apply if in a None state</param>
        /// <returns>If the OptionUnsafe is in a None state then True is returned if 
        /// invoking None returns True. If the OptionUnsafe is in a Some state the value 
        /// is the result of running applying the bound value to the Some predicate 
        /// supplied.</returns>
        [Pure]
        public bool BiForAll(Func<A, bool> Some, Func<Unit, bool> None) =>
            isSome
                ? Some(Value)
                : None(unit);

        /// <summary>
        /// Apply a predicate to the bound value.  If the OptionUnsafe is in a None state
        /// then True is returned if invoking None returns True.
        /// If the OptionUnsafe is in a Some state the value is the result of running 
        /// applying the bound value to the Some predicate supplied.        
        /// </summary>
        /// <param name="Some">Predicate to apply if in a Some state</param>
        /// <param name="None">Predicate to apply if in a None state</param>
        /// <returns>If the OptionUnsafe is in a None state then True is returned if 
        /// invoking None returns True. If the OptionUnsafe is in a Some state the value 
        /// is the result of running applying the bound value to the Some predicate 
        /// supplied.</returns>
        [Pure]
        public bool BiForAll(Func<A, bool> Some, Func<bool> None) =>
            isSome
                ? Some(Value)
                : None();

        /// <summary>
        /// Apply a predicate to the bound value.  If the OptionUnsafe is in a None state
        /// then True is returned if invoking None returns True.
        /// If the OptionUnsafe is in a Some state the value is the result of running 
        /// applying the bound value to the Some predicate supplied.        
        /// </summary>
        /// <param name="pred"></param>
        /// <returns>If the OptionUnsafe is in a None state then True is returned if 
        /// invoking None returns True. If the OptionUnsafe is in a Some state the value 
        /// is the result of running applying the bound value to the Some predicate 
        /// supplied.</returns>
        [Pure]
        public bool Exists(Func<A, bool> pred) =>
            isSome
                ? pred(Value)
                : false;

        /// <summary>
        /// Apply a predicate to the bound value.  If the OptionUnsafe is in a None state
        /// then True is returned if invoking None returns True.
        /// If the OptionUnsafe is in a Some state the value is the result of running 
        /// applying the bound value to the Some predicate supplied.        
        /// </summary>
        /// <param name="pred"></param>
        /// <returns>If the OptionUnsafe is in a None state then True is returned if 
        /// invoking None returns True. If the OptionUnsafe is in a Some state the value 
        /// is the result of running applying the bound value to the Some predicate 
        /// supplied.</returns>
        [Pure]
        public bool BiExists(Func<A, bool> Some, Func<Unit, bool> None) =>
            isSome
                ? Some(Value)
                : None(unit);

        /// <summary>
        /// Apply a predicate to the bound value.  If the OptionUnsafe is in a None state
        /// then True is returned if invoking None returns True.
        /// If the OptionUnsafe is in a Some state the value is the result of running 
        /// applying the bound value to the Some predicate supplied.        
        /// </summary>
        /// <param name="pred"></param>
        /// <returns>If the OptionUnsafe is in a None state then True is returned if 
        /// invoking None returns True. If the OptionUnsafe is in a Some state the value 
        /// is the result of running applying the bound value to the Some predicate 
        /// supplied.</returns>
        [Pure]
        public bool BiExists(Func<A, bool> Some, Func<bool> None) =>
            isSome
                ? Some(Value)
                : None();

        /// <summary>
        /// Invoke an action for the bound value (if in a Some state)
        /// </summary>
        /// <param name="Some">Action to invoke</param>
        [Pure]
        public Unit Iter(Action<A> Some)
        {
            if (isSome)
            {
                Some(Value);
            }
            return unit;
        }

        /// <summary>
        /// Invoke an action depending on the state of the OptionUnsafe
        /// </summary>
        /// <param name="Some">Action to invoke if in a Some state</param>
        /// <param name="None">Action to invoke if in a None state</param>
        [Pure]
        public Unit BiIter(Action<A> Some, Action<Unit> None)
        {
            if (isSome)
            {
                Some(Value);
            }
            else
            {
                None(unit);
            }
            return unit;
        }

        /// <summary>
        /// Invoke an action depending on the state of the OptionUnsafe
        /// </summary>
        /// <param name="Some">Action to invoke if in a Some state</param>
        /// <param name="None">Action to invoke if in a None state</param>
        [Pure]
        public Unit BiIter(Action<A> Some, Action None)
        {
            if (isSome)
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
        /// Apply a predicate to the bound value (if in a Some state)
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>Some(x) if the OptionUnsafe is in a Some state and the predicate
        /// returns True.  None otherwise.</returns>
        [Pure]
        public OptionUnsafe<A> Filter(Func<A, bool> pred) =>
            isSome && pred(Value)
                ? this
                : default;

        /// <summary>
        /// Apply a predicate to the bound value (if in a Some state)
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>Some(x) if the OptionUnsafe is in a Some state and the predicate
        /// returns True.  None otherwise.</returns>
        [Pure]
        public OptionUnsafe<A> Where(Func<A, bool> pred) =>
            isSome && pred(Value)
                ? this
                : default;

        /// <summary>
        /// Monadic join
        /// </summary>
        [Pure]
        public OptionUnsafe<D> Join<B, C, D>(
            OptionUnsafe<B> inner,
            Func<A, C> outerKeyMap,
            Func<B, C> innerKeyMap,
            Func<A, B, D> project) =>
            join<EqDefault<C>, MOptionUnsafe<A>, MOptionUnsafe<B>, MOptionUnsafe<D>, OptionUnsafe<A>, OptionUnsafe<B>, OptionUnsafe<D>, A, B, C, D>(
                this, inner, outerKeyMap, innerKeyMap, project
                );

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public OptionUnsafe<Func<B, C>> ParMap<B, C>(Func<A, B, C> func) =>
            Map(curry(func));

        /// <summary>
        /// Partial application map
        /// </summary>
        /// <remarks>TODO: Better documentation of this function</remarks>
        [Pure]
        public OptionUnsafe<Func<B, Func<C, D>>> ParMap<B, C, D>(Func<A, B, C, D> func) =>
            Map(curry(func));

        /// <summary>
        /// Apply a map operation that returns a Task result
        /// </summary>
        /// <typeparam name="B">Type of the bound result value</typeparam>
        /// <param name="map">Mapping function to apply</param>
        /// <returns>A task</returns>
        public async Task<OptionUnsafe<B>> MapAsync<B>(Func<A, Task<B>> map) =>
            isSome
                ? OptionUnsafe<B>.Some(await map(Value))
                : OptionUnsafe<B>.None;
    }
}
