using System;
using System.Linq;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;
using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using LanguageExt.ClassInstances;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Collections;

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
        readonly OptionData<A> data;

        /// <summary>
        /// None
        /// </summary>
        public static readonly OptionUnsafe<A> None = new OptionUnsafe<A>(OptionData<A>.None);

        /// <summary>
        /// Construct an OptionUnsafe of A in a Some state
        /// </summary>
        /// <param name="value">Value to bind, must be non-null</param>
        /// <returns>OptionUnsafe of A</returns>
        [Pure]
        public static OptionUnsafe<A> Some(A value) =>
            value;

        /// <summary>
        /// Takes the value-type OptionV<A>
        /// </summary>
        internal OptionUnsafe(OptionData<A> data) =>
            this.data = data;

        /// <summary>
        /// Ctor that facilitates serialisation
        /// </summary>
        /// <param name="option">None or Some A.</param>
        [Pure]
        public OptionUnsafe(IEnumerable<A> option)
        {
            var first = option.Take(1).ToArray();
            this.data = first.Length == 0
                ? OptionData<A>.None
                : OptionData.Optional(first[0]);
        }

        [Pure]
        public OptionUnsafe(SerializationInfo info, StreamingContext context)
        {
            var isSome = (bool)info.GetValue("IsSome", typeof(bool));
            var value = (A)info.GetValue("Value", typeof(A));
            data = isSome ? new OptionData<A>(OptionState.Some, value, null) : OptionData<A>.None;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IsSome", IsSome);
            info.AddValue("Value", data.Value);
        }

        public IEnumerator<A> GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        /// <summary>
        /// Uses the EqDefault instance to do an equality check on the bound value.  
        /// To use anything other than the default call equals<EQ, A>(a, b), 
        /// where EQ is an instance derived from Eq<A>
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
        /// <param name="other">The OptionUnsafe type to compare this type with</param>
        /// <returns>True if this and other are equal</returns>
        public bool Equals(OptionUnsafe<A> other) =>
            equals<EqDefault<A>, A>(this, other);

        /// <summary>
        /// Uses the OrdDefault instance to do an ordering comparison on the bound 
        /// value.  To use anything other than the default call 
        /// compare<OrdDefault<A>, A>(this, other), where EQ is an instance derived 
        /// from Eq<A>
        /// </summary>
        /// <param name="other">The OptionUnsafe type to compare this type with</param>
        /// <returns>True if this and other are equal</returns>
        public int CompareTo(OptionUnsafe<A> other) =>
            compare<OrdDefault<A>, A>(this, other);

        /// <summary>
        /// Implicit conversion operator from A to OptionUnsafe<A>
        /// </summary>
        /// <param name="a">Unit value</param>
        [Pure]
        public static implicit operator OptionUnsafe<A>(A a) =>
            OptionUnsafe<A>.Some(a);

        /// Implicit conversion operator from None to OptionUnsafe<A>
        /// </summary>
        /// <param name="a">None value</param>
        [Pure]
        public static implicit operator OptionUnsafe<A>(OptionNone a) =>
            None;

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="lhs">The left hand side of the operation</param>
        /// <param name="rhs">The right hand side of the operation</param>
        /// <returns>True if lhs < rhs</returns>
        [Pure]
        public static bool operator <(OptionUnsafe<A> lhs, OptionUnsafe<A> rhs) =>
            compare<OrdDefault<A>, A>(lhs, rhs) < 0;

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="lhs">The left hand side of the operation</param>
        /// <param name="rhs">The right hand side of the operation</param>
        /// <returns>True if lhs <= rhs</returns>
        [Pure]
        public static bool operator <=(OptionUnsafe<A> lhs, OptionUnsafe<A> rhs) =>
            compare<OrdDefault<A>, A>(lhs, rhs) <= 0;

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="lhs">The left hand side of the operation</param>
        /// <param name="rhs">The right hand side of the operation</param>
        /// <returns>True if lhs > rhs</returns>
        [Pure]
        public static bool operator >(OptionUnsafe<A> lhs, OptionUnsafe<A> rhs) =>
            compare<OrdDefault<A>, A>(lhs, rhs) > 0;

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="lhs">The left hand side of the operation</param>
        /// <param name="rhs">The right hand side of the operation</param>
        /// <returns>True if lhs >= rhs</returns>
        [Pure]
        public static bool operator >=(OptionUnsafe<A> lhs, OptionUnsafe<A> rhs) =>
            compare<OrdDefault<A>, A>(lhs, rhs) >= 0;

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
        public static bool operator ==(OptionUnsafe<A> lhs, OptionUnsafe<A> rhs) =>
            equals<EqDefault<A>, A>(lhs, rhs);

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
        public static bool operator !=(OptionUnsafe<A> lhs, OptionUnsafe<A> rhs) =>
            !(lhs == rhs);

        /// <summary>
        /// Coalescing operator
        /// </summary>
        /// <param name="lhs">Left hand side of the operation</param>
        /// <param name="rhs">Right hand side of the operation</param>
        /// <returns>if lhs is Some then lhs, else rhs</returns>
        [Pure]
        public static OptionUnsafe<A> operator |(OptionUnsafe<A> lhs, OptionUnsafe<A> rhs) =>
            MOptionUnsafe<A>.Inst.Plus(lhs, rhs);

        /// <summary>
        /// Truth operator
        /// </summary>
        [Pure]
        public static bool operator true(OptionUnsafe<A> value) =>
            value.IsSome;

        /// <summary>
        /// Falsity operator
        /// </summary>
        [Pure]
        public static bool operator false(OptionUnsafe<A> value) =>
            value.IsNone;

        /// <summary>
        /// DO NOT USE - Use the Structural equality variant of this method Equals<EQ, A>(y)
        /// </summary>
        [Pure]
        public override bool Equals(object obj) =>
            obj is OptionUnsafe<A> && equals<EqDefault<A>, A>(this, (ReferenceEquals(obj, null) ? None : (OptionUnsafe<A>)obj));

        /// <summary>
        /// Calculate the hash-code from the bound value, unless the OptionUnsafe is in a None
        /// state, in which case the hash-code will be 0
        /// </summary>
        /// <returns>Hash-code from the bound value, unless the OptionUnsafe is in a None
        /// state, in which case the hash-code will be 0</returns>
        [Pure]
        public override int GetHashCode() =>
            IsSome
                ? Value.GetHashCode()
                : 0;

        /// <summary>
        /// Get a string representation of the OptionUnsafe
        /// </summary>
        /// <returns>String representation of the OptionUnsafe</returns>
        [Pure]
        public override string ToString() =>
            IsSome
                ? $"Some({Value})"
                : "None";

        /// <summary>
        /// True if this instance evaluates lazily
        /// </summary>
        [Pure]
        public bool IsLazy =>
            data.IsLazy;

        /// <summary>
        /// Is the option in a Some state
        /// </summary>
        [Pure]
        public bool IsSome =>
            data.IsSome;

        /// <summary>
        /// Is the option in a None state
        /// </summary>
        [Pure]
        public bool IsNone =>
            !IsSome;

        /// <summary>
        /// Helper accessor for the bound value
        /// </summary>
        internal A Value =>
            data.Value;

        /// <summary>
        /// Projection from one value to another 
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public OptionUnsafe<B> Select<B>(Func<A, B> f) =>
            FOptionUnsafe<A, B>.Inst.Map(this, f);

        /// <summary>
        /// Projection from one value to another 
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="f">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public OptionUnsafe<B> Map<B>(Func<A, B> f) =>
            FOptionUnsafe<A, B>.Inst.Map(this, f);

        /// <summary>
        /// Monad bind operation
        /// </summary>
        [Pure]
        public OptionUnsafe<B> Bind<B>(Func<A, OptionUnsafe<B>> f) =>
            MOptionUnsafe<A>.Inst.Bind<MOptionUnsafe<B>, OptionUnsafe<B>, B>(this, f);

        /// <summary>
        /// Bi-bind.  Allows mapping of both monad states
        /// </summary>
        [Pure]
        public OptionUnsafe<B> BiBind<B>(Func<A, OptionUnsafe<B>> Some, Func<OptionUnsafe<B>> None) =>
            IsSome
                ? Some(Value)
                : None();

        /// <summary>
        /// Monad bind operation
        /// </summary>
        [Pure]
        public OptionUnsafe<C> SelectMany<B, C>(
            Func<A, OptionUnsafe<B>> bind,
            Func<A, B, C> project) =>
            SelectMany<MOptionUnsafe<A>, MOptionUnsafe<B>, MOptionUnsafe<C>, OptionUnsafe<A>, OptionUnsafe<B>, OptionUnsafe<C>, A, B, C>(this, bind, project);

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
            matchUntyped<MOptionUnsafe<A>, OptionUnsafe<A>, A, R>(this, Some, None);

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
            toArray<MOptionUnsafe<A>, OptionUnsafe<A>, A>(this);

        /// <summary>
        /// Convert the OptionUnsafe to an immutable list of zero or one items
        /// </summary>
        /// <returns>An immutable list of zero or one items</returns>
        [Pure]
        public Lst<A> ToList() =>
            toList<MOptionUnsafe<A>, OptionUnsafe<A>, A>(this);

        /// <summary>
        /// Convert the OptionUnsafe to an enumerable sequence of zero or one items
        /// </summary>
        /// <returns>An enumerable sequence of zero or one items</returns>
        [Pure]
        public Seq<A> ToSeq() =>
            toSeq<MOptionUnsafe<A>, OptionUnsafe<A>, A>(this);

        /// <summary>
        /// Convert the OptionUnsafe to an enumerable of zero or one items
        /// </summary>
        /// <returns>An enumerable of zero or one items</returns>
        [Pure]
        public Seq<A> AsEnumerable() =>
            asEnumerable<MOptionUnsafe<A>, OptionUnsafe<A>, A>(this);

        [Pure]
        public Validation<FAIL, A> ToValidation<FAIL>(FAIL defaultFailureValue) =>
            IsSome
                ? Success<FAIL, A>(Value)
                : Fail<FAIL, A>(defaultFailureValue);

        /// <summary>
        /// Convert the structure to an Either
        /// </summary>
        /// <param name="defaultLeftValue">Default value if the structure is in a None state</param>
        /// <returns>An Either representation of the structure</returns>
        [Pure]
        public Either<L, A> ToEither<L>(L defaultLeftValue) =>
            toEither<MOptionUnsafe<A>, OptionUnsafe<A>, L, A>(this, defaultLeftValue);

        /// <summary>
        /// Convert the structure to an Either
        /// </summary>
        /// <param name="defaultLeftValue">Function to invoke to get a default value if the 
        /// structure is in a None state</param>
        /// <returns>An Either representation of the structure</returns>
        [Pure]
        public Either<L, A> ToEither<L>(Func<L> Left) =>
            toEither<MOptionUnsafe<A>, OptionUnsafe<A>, L, A>(this, Left);

        /// <summary>
        /// Convert the structure to an EitherUnsafe
        /// </summary>
        /// <param name="defaultLeftValue">Default value if the structure is in a None state</param>
        /// <returns>An EitherUnsafe representation of the structure</returns>
        [Pure]
        public EitherUnsafe<L, A> ToEitherUnsafe<L>(L defaultLeftValue) =>
            toEitherUnsafe<MOptionUnsafe<A>, OptionUnsafe<A>, L, A>(this, defaultLeftValue);

        /// <summary>
        /// Convert the structure to an EitherUnsafe
        /// </summary>
        /// <param name="defaultLeftValue">Function to invoke to get a default value if the 
        /// structure is in a None state</param>
        /// <returns>An EitherUnsafe representation of the structure</returns>
        [Pure]
        public EitherUnsafe<L, A> ToEitherUnsafe<L>(Func<L> Left) =>
            toEitherUnsafe<MOptionUnsafe<A>, OptionUnsafe<A>, L, A>(this, Left);

        /// <summary>
        /// Convert the structure to a Option
        /// </summary>
        /// <returns>An OptionUnsafe representation of the structure</returns>
        [Pure]
        public Option<A> ToOption() =>
            toOption<MOptionUnsafe<A>, OptionUnsafe<A>, A>(this);

        /// <summary>
        /// Convert the structure to a TryOption
        /// </summary>
        /// <returns>A TryOption representation of the structure</returns>
        [Pure]
        public TryOption<A> ToTryOption() =>
            toTryOption<MOptionUnsafe<A>, OptionUnsafe<A>, A>(this);

        /// <summary>
        /// Fluent pattern matching.  Provide a Some handler and then follow
        /// on fluently with .None(...) to complete the matching operation.
        /// This is for dispatching actions, use Some<A,B>(...) to return a value
        /// from the match operation.
        /// </summary>
        /// <param name="f">The Some(x) match operation</param>
        [Pure]
        public SomeUnitContext<MOptionUnsafe<A>, OptionUnsafe<A>, A> Some(Action<A> f) =>
            new SomeUnitContext<MOptionUnsafe<A>, OptionUnsafe<A>, A>(this, f, false);

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
        public SomeContext<MOptionUnsafe<A>, OptionUnsafe<A>, A, B> Some<B>(Func<A, B> f) =>
            new SomeContext<MOptionUnsafe<A>, OptionUnsafe<A>, A, B>(this, f, false);

        /// <summary>
        /// Match the two states of the OptionUnsafe and return a B, which can be null.
        /// </summary>
        /// <typeparam name="B">Return type</typeparam>
        /// <param name="Some">Some match operation. May return null.</param>
        /// <param name="None">None match operation. May return null.</param>
        /// <returns>B, or null</returns>
        [Pure]
        public B MatchUnsafe<B>(Func<A, B> Some, Func<B> None) =>
            MOptionUnsafe<A>.Inst.MatchUnsafe(this, Some, None);

        /// <summary>
        /// Match the two states of the OptionUnsafe
        /// </summary>
        /// <param name="Some">Some match operation</param>
        /// <param name="None">None match operation</param>
        public Unit MatchUnsafe(Action<A> Some, Action None) =>
            MOptionUnsafe<A>.Inst.Match(this, Some, None);

        /// <summary>
        /// Invokes the action if OptionUnsafe is in the Some state, otherwise nothing happens.
        /// </summary>
        /// <param name="f">Action to invoke if OptionUnsafe is in the Some state</param>
        public Unit IfSomeUnsafe(Action<A> f) =>
            ifSome<MOptionUnsafe<A>, OptionUnsafe<A>, A>(this, f);

        /// <summary>
        /// Invokes the f function if OptionUnsafe is in the Some state, otherwise nothing
        /// happens.
        /// </summary>
        /// <param name="f">Function to invoke if OptionUnsafe is in the Some state</param>
        public Unit IfSomeUnsafe(Func<A, Unit> f) =>
            ifSome<MOptionUnsafe<A>, OptionUnsafe<A>, A>(this, f);

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
            ifNone<MOptionUnsafe<A>, OptionUnsafe<A>, A>(this, None);

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
            ifNone<MOptionUnsafe<A>, OptionUnsafe<A>, A>(this, noneValue);

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
            MOptionUnsafe<A>.Inst.Fold(this, state, folder)(unit);

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
            MOptionUnsafe<A>.Inst.FoldBack(this, state, folder)(unit);

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
            MOptionUnsafe<A>.Inst.BiFold(this, state, Some, None);

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
            MOptionUnsafe<A>.Inst.BiFold(this, state, Some, (s, _) => None(s));

        /// <summary>
        /// Projection from one value to another
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="Some">Projection function</param>
        /// <param name="None">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public OptionUnsafe<B> BiMap<B>(Func<A, B> Some, Func<Unit, B> None) =>
            FOptionUnsafe<A, B>.Inst.BiMap(this, Some, None);

        /// <summary>
        /// Projection from one value to another
        /// </summary>
        /// <typeparam name="B">Resulting functor value type</typeparam>
        /// <param name="Some">Projection function</param>
        /// <param name="None">Projection function</param>
        /// <returns>Mapped functor</returns>
        [Pure]
        public OptionUnsafe<B> BiMap<B>(Func<A, B> Some, Func<B> None) =>
            FOptionUnsafe<A, B>.Inst.BiMap(this, Some, _ => None());

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
            MOptionUnsafe<A>.Inst.Count(this)(unit);

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
            forall<MOptionUnsafe<A>, OptionUnsafe<A>, A>(this, pred);

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
            biForAll<MOptionUnsafe<A>, OptionUnsafe<A>, A, Unit>(this, Some, None);

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
            biForAll<MOptionUnsafe<A>, OptionUnsafe<A>, A, Unit>(this, Some, _ => None());

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
            exists<MOptionUnsafe<A>, OptionUnsafe<A>, A>(this, pred);

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
            biExists<MOptionUnsafe<A>, OptionUnsafe<A>, A, Unit>(this, Some, None);

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
            biExists<MOptionUnsafe<A>, OptionUnsafe<A>, A, Unit>(this, Some, _ => None());

        /// <summary>
        /// Invoke an action for the bound value (if in a Some state)
        /// </summary>
        /// <param name="Some">Action to invoke</param>
        [Pure]
        public Unit Iter(Action<A> Some) =>
            iter<MOptionUnsafe<A>, OptionUnsafe<A>, A>(this, Some);

        /// <summary>
        /// Invoke an action depending on the state of the OptionUnsafe
        /// </summary>
        /// <param name="Some">Action to invoke if in a Some state</param>
        /// <param name="None">Action to invoke if in a None state</param>
        [Pure]
        public Unit BiIter(Action<A> Some, Action<Unit> None) =>
            biIter<MOptionUnsafe<A>, OptionUnsafe<A>, A, Unit>(this, Some, None);

        /// <summary>
        /// Invoke an action depending on the state of the OptionUnsafe
        /// </summary>
        /// <param name="Some">Action to invoke if in a Some state</param>
        /// <param name="None">Action to invoke if in a None state</param>
        [Pure]
        public Unit BiIter(Action<A> Some, Action None) =>
            biIter<MOptionUnsafe<A>, OptionUnsafe<A>, A, Unit>(this, Some, _ => None());

        /// <summary>
        /// Apply a predicate to the bound value (if in a Some state)
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>Some(x) if the OptionUnsafe is in a Some state and the predicate
        /// returns True.  None otherwise.</returns>
        [Pure]
        public OptionUnsafe<A> Filter(Func<A, bool> pred) =>
            filter<MOptionUnsafe<A>, OptionUnsafe<A>, A>(this, pred);

        /// <summary>
        /// Apply a predicate to the bound value (if in a Some state)
        /// </summary>
        /// <param name="pred">Predicate to apply</param>
        /// <returns>Some(x) if the OptionUnsafe is in a Some state and the predicate
        /// returns True.  None otherwise.</returns>
        [Pure]
        public OptionUnsafe<A> Where(Func<A, bool> pred) =>
            filter<MOptionUnsafe<A>, OptionUnsafe<A>, A>(this, pred);

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
            IsSome
                ? OptionUnsafe<B>.Some(await map(Value))
                : OptionUnsafe<B>.None;
    }
}
