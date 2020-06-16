using System;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using static LanguageExt.Choice;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using System.Runtime.Serialization;
using LanguageExt.DataTypes.Serialisation;
using System.Collections;

namespace LanguageExt
{
    /// <summary>
    /// Like `Either` but collects the failed values
    /// </summary>
    /// <typeparam name="FAIL"></typeparam>
    /// <typeparam name="SUCCESS"></typeparam>
    [Serializable]
    public struct Validation<FAIL, SUCCESS> :
        IEnumerable<ValidationData<FAIL, SUCCESS>>,
        IComparable<Validation<FAIL, SUCCESS>>,
        IComparable<SUCCESS>,
        IEquatable<Validation<FAIL, SUCCESS>>,
        IEquatable<SUCCESS>,
        ISerializable
    {
        readonly Seq<FAIL> fail;
        readonly SUCCESS success;
        readonly Validation.StateType state;

        Validation(SUCCESS success)
        {
            if (isnull(success)) throw new ValueIsNullException();
            this.success = success;
            this.fail = Seq<FAIL>.Empty;
            this.state = Validation.StateType.Success;
        }

        Validation(Seq<FAIL> fail)
        {
            if (isnull(fail)) throw new ValueIsNullException();
            this.success = default(SUCCESS);
            this.fail = fail;
            this.state = Validation.StateType.Fail;
        }

        /// <summary>
        /// Ctor that facilitates serialisation
        /// </summary>
        [Pure]
        public Validation(IEnumerable<ValidationData<FAIL, SUCCESS>> validationData)
        {
            var seq = Seq(validationData);
            if (seq.IsEmpty)
            {
                this.state = Validation.StateType.Fail;
                this.fail = Seq<FAIL>.Empty;
                this.success = default(SUCCESS);
            }
            else
            {
                this.fail = seq.Head.Fail.ToSeq();
                this.success = seq.Head.Success;
                this.state = seq.Head.State;
            }
        }

        /// <summary>
        /// Ctor that facilitates serialisation
        /// </summary>
        [Pure]
        Validation(SerializationInfo info, StreamingContext context)
        {
            state = (Validation.StateType)info.GetValue("State", typeof(Validation.StateType));
            switch (state)
            {
                case Validation.StateType.Success:
                    success = (SUCCESS)info.GetValue("Success", typeof(SUCCESS));
                    fail = Seq<FAIL>.Empty;
                    break;
                case Validation.StateType.Fail:
                    fail = (Seq<FAIL>)info.GetValue("Fail", typeof(Seq<FAIL>));
                    success = default(SUCCESS);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("State", state);
            if (IsSuccess) info.AddValue("Success", SuccessValue);
            if (IsFail) info.AddValue("Fail", FailValue);
        }

        internal Seq<FAIL> FailValue => isnull(fail) ? Seq<FAIL>.Empty : fail;
        internal SUCCESS SuccessValue => success;

        /// <summary>
        /// Reference version for use in pattern-matching
        /// </summary>
        [Pure]
        public ValidationCase<Seq<FAIL>, SUCCESS> Case =>
            state switch
            {
                Validation.StateType.Success => SuccCase<Seq<FAIL>, SUCCESS>.New(success),
                Validation.StateType.Fail    => FailCase<Seq<FAIL>, SUCCESS>.New(fail),
                _                            => null
            };

        [Pure]
        public bool IsFail =>
            state == Validation.StateType.Fail;

        [Pure]
        public bool IsSuccess =>
            state == Validation.StateType.Success;

        IEnumerable<ValidationData<FAIL, SUCCESS>> Enum()
        {
            yield return new ValidationData<FAIL, SUCCESS>(state, success, FailValue.Freeze());
        }

        public IEnumerator<ValidationData<FAIL, SUCCESS>> GetEnumerator() =>
            Enum().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            Enum().GetEnumerator();

        /// <summary>
        /// Implicit conversion operator from `SUCCESS` to `Validation<MonoidFail, FAIL, SUCCESS>`
        /// </summary>
        /// <param name="value">`value`, must not be `null`.</param>
        /// <exception cref="ValueIsNullException">`value` is `null`</exception>
        [Pure]
        public static implicit operator Validation<FAIL, SUCCESS>(SUCCESS value) =>
            isnull(value)
                ? throw new ValueIsNullException()
                : Success(value);

        /// <summary>
        /// Implicit conversion operator from `FAIL` to `Validation<MonoidFail, FAIL, SUCCESS>`
        /// </summary>
        /// <param name="value">`value`, must not be `null`.</param>
        /// <exception cref="ValueIsNullException">`value` is `null`</exception>
        [Pure]
        public static implicit operator Validation<FAIL, SUCCESS>(Seq<FAIL> value) =>
            isnull(value)
                ? throw new ValueIsNullException()
                : Fail(value);

        /// <summary>
        /// Explicit conversion operator from `Validation` to `SUCCESS`
        /// </summary>
        /// <param name="value">Value, must not be null.</param>
        /// <exception cref="ValueIsNullException">Value is null</exception>
        [Pure]
        public static explicit operator SUCCESS(Validation<FAIL, SUCCESS> ma) =>
            ma.IsSuccess
                ? ma.SuccessValue
                : throw new InvalidCastException("Validation is not in a Success state");

        /// <summary>
        /// Explicit conversion operator from `Validation` to `FAIL`
        /// </summary>
        /// <param name="value">Value, must not be null.</param>
        /// <exception cref="ValueIsNullException">Value is null</exception>
        [Pure]
        public static explicit operator Seq<FAIL>(Validation<FAIL, SUCCESS> ma) =>
            ma.IsFail
                ? ma.FailValue
                : throw new InvalidCastException("Validation is not in a Fail state");

        /// <summary>
        /// Implicit conversion operator from `FAIL` to `Validation<MonoidFail, FAIL, SUCCESS>`
        /// </summary>
        /// <param name="value">`value`, must not be `null`.</param>
        /// <exception cref="ValueIsNullException">`value` is `null`</exception>
        [Pure]
        public static implicit operator Validation<FAIL, SUCCESS>(FAIL value) =>
            isnull(value)
                ? throw new ValueIsNullException()
                : Fail(Seq1(value));

        [Pure]
        public Validation<FAIL, SUCCESS> Disjunction<SUCCESSB>(Validation<FAIL, SUCCESSB> other)
        {
            if (IsSuccess && other.IsSuccess) return this;
            if (IsSuccess) return new Validation<FAIL, SUCCESS>(other.FailValue);
            if (other.IsSuccess) return this;
            return new Validation<FAIL, SUCCESS>(FailValue.Append(other.FailValue));
        }

        /// <summary>
        /// Success constructor
        /// </summary>
        [Pure]
        public static Validation<FAIL, SUCCESS> Success(SUCCESS success) =>
            new Validation<FAIL, SUCCESS>(success);

        /// <summary>
        /// Fail constructor
        /// </summary>
        [Pure]
        public static Validation<FAIL, SUCCESS> Fail(Seq<FAIL> fail) =>
            new Validation<FAIL, SUCCESS>(fail);

        /// <summary>
        /// Fluent matching
        /// </summary>
        public ValidationContext<FAIL, SUCCESS, Ret> Succ<Ret>(Func<SUCCESS, Ret> f) =>
            new ValidationContext<FAIL, SUCCESS, Ret>(this, f);

        /// <summary>
        /// Fluent matching
        /// </summary>
        public ValidationUnitContext<FAIL, SUCCESS> Succ<Ret>(Action<SUCCESS> f) =>
            new ValidationUnitContext<FAIL, SUCCESS>(this, f);

        /// <summary>
        /// Invokes the `Succ` or `Fail` function depending on the state of the `Validation`
        /// </summary>
        /// <typeparam name="Ret">Return type</typeparam>
        /// <param name="Succ">Function to invoke if in a `Success` state</param>
        /// <param name="Fail">Function to invoke if in a `Fail` state</param>
        /// <returns>The return value of the invoked function</returns>
        [Pure]
        public Ret Match<Ret>(Func<SUCCESS, Ret> Succ, Func<Seq<FAIL>, Ret> Fail) =>
            Check.NullReturn(MatchUnsafe(Succ, Fail));

        /// <summary>
        /// Invokes the `Succ` or `Fail` function depending on the state of the `Validation`
        /// </summary>
        /// <typeparam name="Ret">Return type</typeparam>
        /// <param name="Succ">Function to invoke if in a `Success` state</param>
        /// <param name="Fail">Function to invoke if in a `Fail` state</param>
        /// <returns>The return value of the invoked function</returns>
        [Pure]
        public Ret MatchUnsafe<Ret>(Func<SUCCESS, Ret> Succ, Func<Seq<FAIL>, Ret> Fail) =>
            IsFail
                ? Fail(FailValue)
                : Succ == null
                    ? throw new ArgumentNullException(nameof(Succ))
                    : Succ(success);

        /// <summary>
        /// Invokes the `Succ` or `Fail` action depending on the state of the `Validation`
        /// </summary>
        /// <param name="Succ">Action to invoke if in a `Success` state</param>
        /// <param name="Fail">Action to invoke if in a `Fail` state</param>
        /// <returns>Unit</returns>
        public Unit Match(Action<SUCCESS> Succ, Action<Seq<FAIL>> Fail)
        {
            if (IsFail)
            {
                Fail(FailValue);
            }
            else 
            {
                Succ(success);
            }
            return unit;
        }

        /// <summary>
        /// Match the two states of the Validation and return a promise for a non-null R2.
        /// </summary>
        /// <returns>A promise to return a non-null R2</returns>
        public async Task<R2> MatchAsync<R2>(Func<SUCCESS, Task<R2>> SuccAsync, Func<Seq<FAIL>, R2> Fail) =>
            await Match(SuccAsync, f => Fail(f).AsTask());

        /// <summary>
        /// Match the two states of the Validation and return a promise for a non-null R2.
        /// </summary>
        /// <returns>A promise to return a non-null R2</returns>
        public async Task<R2> MatchAsync<R2>(Func<SUCCESS, Task<R2>> SuccAsync, Func<Seq<FAIL>, Task<R2>> FailAsync) =>
            await Match(SuccAsync, FailAsync);

        /// <summary>
        /// Executes the Fail function if the Validation is in a Fail state.
        /// Returns the Success value if the Validation is in a Success state.
        /// </summary>
        /// <param name="Fail">Function to generate a Success value if in the Fail state</param>
        /// <returns>Returns an unwrapped Success value</returns>
        [Pure]
        public SUCCESS IfFail(Func<SUCCESS> Fail) =>
            ifLeft<FoldValidation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS>, Seq<FAIL>, SUCCESS>(this, Fail);

        /// <summary>
        /// Executes the FailMap function if the Validation is in a Fail state.
        /// Returns the Success value if the Validation is in a Success state.
        /// </summary>
        /// <param name="FailMap">Function to generate a Success value if in the Fail state</param>
        /// <returns>Returns an unwrapped Success value</returns>
        [Pure]
        public SUCCESS IfFail(Func<Seq<FAIL>, SUCCESS> FailMap) =>
            ifLeft<FoldValidation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS>, Seq<FAIL>, SUCCESS>(this, FailMap);

        /// <summary>
        /// Returns the SuccessValue if the Validation is in a Fail state.
        /// Returns the Success value if the Validation is in a Success state.
        /// </summary>
        /// <param name="SuccessValue">Value to return if in the Fail state</param>
        /// <returns>Returns an unwrapped Success value</returns>
        [Pure]
        public SUCCESS IfFail(SUCCESS SuccessValue) =>
            ifLeft<FoldValidation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS>, Seq<FAIL>, SUCCESS>(this, SuccessValue);

        /// <summary>
        /// Executes the Fail action if the Validation is in a Fail state.
        /// </summary>
        /// <param name="Fail">Function to generate a Success value if in the Fail state</param>
        /// <returns>Returns an unwrapped Success value</returns>
        public Unit IfFail(Action<Seq<FAIL>> Fail) =>
            ifLeft<FoldValidation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS>, Seq<FAIL>, SUCCESS>(this, Fail);

        /// <summary>
        /// Invokes the Success action if the Validation is in a Success state, otherwise does nothing
        /// </summary>
        /// <param name="Success">Action to invoke</param>
        /// <returns>Unit</returns>
        public Unit IfSuccess(Action<SUCCESS> Success) =>
            ifRight<FoldValidation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS>, Seq<FAIL>, SUCCESS>(this, Success);

        /// <summary>
        /// Returns the FailValue if the Validation is in a Success state.
        /// Returns the Fail value if the Validation is in a Fail state.
        /// </summary>
        /// <param name="FailValue">Value to return if in the Fail state</param>
        /// <returns>Returns an unwrapped Fail value</returns>
        [Pure]
        public Seq<FAIL> IfSuccess(Seq<FAIL> FailValue) =>
            ifRight<FoldValidation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS>, Seq<FAIL>, SUCCESS>(this, FailValue);

        /// <summary>
        /// Returns the result of Success() if the Validation is in a Success state.
        /// Returns the Fail value if the Validation is in a Fail state.
        /// </summary>
        /// <param name="Success">Function to generate a Fail value if in the Success state</param>
        /// <returns>Returns an unwrapped Fail value</returns>
        [Pure]
        public Seq<FAIL> IfSuccess(Func<Seq<FAIL>> Success) =>
            ifRight<FoldValidation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS>, Seq<FAIL>, SUCCESS>(this, Success);

        /// <summary>
        /// Returns the result of SuccessMap if the Validation is in a Success state.
        /// Returns the Fail value if the Validation is in a Fail state.
        /// </summary>
        /// <param name="SuccessMap">Function to generate a Fail value if in the Success state</param>
        /// <returns>Returns an unwrapped Fail value</returns>
        [Pure]
        public Seq<FAIL> IfSuccess(Func<SUCCESS, Seq<FAIL>> SuccessMap) =>
            ifRight<FoldValidation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS>, Seq<FAIL>, SUCCESS>(this, SuccessMap);

        /// <summary>
        /// Return a string representation of the Validation
        /// </summary>
        /// <returns>String representation of the Validation</returns>
        [Pure]
        public override string ToString() =>
            IsSuccess
                ? isnull(success)
                    ? "Success(null)"
                    : $"Success({success})"
                : $"Fail({FailValue})";

        /// <summary>
        /// Returns a hash code of the wrapped value of the Validation
        /// </summary>
        /// <returns>Hash code</returns>
        [Pure]
        public override int GetHashCode() =>
            hashCode<FoldValidation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS>, Seq<FAIL>, SUCCESS>(this);

        /// <summary>
        /// Equality check
        /// </summary>
        /// <param name="obj">Object to test for equality</param>
        /// <returns>True if equal</returns>
        [Pure]
        public override bool Equals(object obj) =>
            !ReferenceEquals(obj, null) &&
            obj is Validation<FAIL, SUCCESS> &&
            EqChoice<
                EqDefault<SUCCESS>, 
                FoldValidation<FAIL, SUCCESS>, 
                Validation<FAIL, SUCCESS>,
                Seq<FAIL>, SUCCESS>
               .Inst.Equals(this, (Validation<FAIL, SUCCESS>)obj);


        /// <summary>
        /// Project the Validation into a Lst
        /// </summary>
        [Pure]
        public Lst<SUCCESS> SuccessToList() =>
            rightToList<FoldValidation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS>, Seq<FAIL>, SUCCESS>(this);

        /// <summary>
        /// Project the Validation into an immutable array
        /// </summary>
        [Pure]
        public Arr<SUCCESS> SuccessToArray() =>
            rightToArray<FoldValidation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS>, Seq<FAIL>, SUCCESS>(this);

        /// <summary>
        /// Project the Validation into a Lst
        /// </summary>
        [Pure]
        public Lst<FAIL> FailToList() =>
            Match(Succ: _ => Lst<FAIL>.Empty, Fail: e => e.Freeze());

        /// <summary>
        /// Project the Validation into an immutable array R
        /// </summary>
        [Pure]
        public Arr<FAIL> FailToArray() =>
            Match(Succ: _ => Arr<FAIL>.Empty, Fail: e => e.ToArr());

        /// <summary>
        /// Convert Validation to sequence of 0 or 1 right values
        /// </summary>
        [Pure]
        public Seq<SUCCESS> ToSeq() =>
            SuccessAsEnumerable();

        /// <summary>
        /// Convert Validation to sequence of 0 or 1 success values
        /// </summary>
        [Pure]
        public Seq<SUCCESS> SuccessToSeq() =>
            SuccessAsEnumerable();

        /// <summary>
        /// Convert Validation to sequence of 0 or 1 success values
        /// </summary>
        [Pure]
        public Seq<FAIL> FailToSeq() =>
            Match(Succ: _ => Seq<FAIL>.Empty, Fail: e => e);

        /// <summary>
        /// Project the Validation success into a Seq
        /// </summary>
        [Pure]
        public Seq<SUCCESS> SuccessAsEnumerable() =>
            rightAsEnumerable<FoldValidation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS>, Seq<FAIL>, SUCCESS>(this);

        /// <summary>
        /// Project the Validation fail into a Seq
        /// </summary>
        [Pure]
        public Seq<FAIL> FailAsEnumerable() =>
            Match(Succ: _ => Seq<FAIL>.Empty, Fail: e => e);

        /// <summary>
        /// Convert the Validation to an Option
        /// </summary>
        [Pure]
        public Option<SUCCESS> ToOption() =>
            toOption<FoldValidation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS>, Seq<FAIL>, SUCCESS>(this);

        /// <summary>
        /// Convert the Validation to an Either
        /// </summary>
        [Pure]
        public Either<Seq<FAIL>, SUCCESS> ToEither() =>
            toEither<FoldValidation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS>, Seq<FAIL>, SUCCESS>(this);

        /// <summary>
        /// Convert the Validation to an EitherUnsafe
        /// </summary>
        [Pure]
        public EitherUnsafe<Seq<FAIL>, SUCCESS> ToEitherUnsafe() =>
            toEitherUnsafe<FoldValidation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS>, Seq<FAIL>, SUCCESS>(this);

        /// <summary>
        /// Convert the Validation to an TryOption
        /// </summary>
        [Pure]
        public TryOption<SUCCESS> ToTryOption() =>
            toTryOption<FoldValidation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS>, Seq<FAIL>, SUCCESS>(this);


        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="lhs">The left hand side of the operation</param>
        /// <param name="rhs">The right hand side of the operation</param>
        /// <returns>True if lhs < rhs</returns>
        [Pure]
        public static bool operator <(Validation<FAIL, SUCCESS> lhs, Validation<FAIL, SUCCESS> rhs) =>
            OrdChoice<
                OrdDefault<Seq<FAIL>>, 
                OrdDefault<SUCCESS>, 
                FoldValidation<FAIL, SUCCESS>, 
                Validation<FAIL, SUCCESS>,
                Seq<FAIL>, SUCCESS>
               .Inst.Compare(lhs, rhs) < 0;

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="lhs">The left hand side of the operation</param>
        /// <param name="rhs">The right hand side of the operation</param>
        /// <returns>True if lhs <= rhs</returns>
        [Pure]
        public static bool operator <=(Validation<FAIL, SUCCESS> lhs, Validation<FAIL, SUCCESS> rhs) =>
            OrdChoice<
                OrdDefault<Seq<FAIL>>,
                OrdDefault<SUCCESS>,
                FoldValidation<FAIL, SUCCESS>,
                Validation<FAIL, SUCCESS>,
                Seq<FAIL>, SUCCESS>
               .Inst.Compare(lhs, rhs) <= 0;

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="lhs">The left hand side of the operation</param>
        /// <param name="rhs">The right hand side of the operation</param>
        /// <returns>True if lhs > rhs</returns>
        [Pure]
        public static bool operator >(Validation<FAIL, SUCCESS> lhs, Validation<FAIL, SUCCESS> rhs) =>
            OrdChoice<
                OrdDefault<Seq<FAIL>>,
                OrdDefault<SUCCESS>,
                FoldValidation<FAIL, SUCCESS>,
                Validation<FAIL, SUCCESS>,
                Seq<FAIL>, SUCCESS>
               .Inst.Compare(lhs, rhs) > 0;

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="lhs">The left hand side of the operation</param>
        /// <param name="rhs">The right hand side of the operation</param>
        /// <returns>True if lhs >= rhs</returns>
        [Pure]
        public static bool operator >=(Validation<FAIL, SUCCESS> lhs, Validation<FAIL, SUCCESS> rhs) =>
            OrdChoice<
                OrdDefault<Seq<FAIL>>,
                OrdDefault<SUCCESS>,
                FoldValidation<FAIL, SUCCESS>,
                Validation<FAIL, SUCCESS>,
                Seq<FAIL>, SUCCESS>
               .Inst.Compare(lhs, rhs) >= 0;

        /// <summary>
        /// Equality operator override
        /// </summary>
        [Pure]
        public static bool operator ==(Validation<FAIL, SUCCESS> lhs, Validation<FAIL, SUCCESS> rhs) =>
            lhs.Equals(rhs);

        /// <summary>
        /// Non-equality operator override
        /// </summary>
        [Pure]
        public static bool operator !=(Validation<FAIL, SUCCESS> lhs, Validation<FAIL, SUCCESS> rhs) =>
            !(lhs == rhs);

        /// <summary>
        /// Coalescing operator
        /// </summary>
        [Pure]
        public static Validation<FAIL, SUCCESS> operator |(Validation<FAIL, SUCCESS> lhs, Validation<FAIL, SUCCESS> rhs) =>
            default(FoldValidation<FAIL, SUCCESS>).Append(lhs, rhs);

        /// <summary>
        /// Override of the True operator to return True if the Validation is Success
        /// </summary>
        [Pure]
        public static bool operator true(Validation<FAIL, SUCCESS> value) =>
            value.IsSuccess;

        /// <summary>
        /// Override of the False operator to return True if the Validation is Fail
        /// </summary>
        [Pure]
        public static bool operator false(Validation<FAIL, SUCCESS> value) =>
            value.IsFail;

        /// <summary>
        /// CompareTo override
        /// </summary>
        [Pure]
        public int CompareTo(Validation<FAIL, SUCCESS> other) =>
            OrdChoice<
                OrdDefault<Seq<FAIL>>,
                OrdDefault<SUCCESS>,
                FoldValidation<FAIL, SUCCESS>,
                Validation<FAIL, SUCCESS>,
                Seq<FAIL>, SUCCESS>
               .Inst.Compare(this, other);

        /// <summary>
        /// CompareTo override
        /// </summary>
        [Pure]
        public int CompareTo(SUCCESS success) =>
            CompareTo(Success(success));

        /// <summary>
        /// CompareTo override
        /// </summary>
        [Pure]
        public int CompareTo(Seq<FAIL> fail) =>
            CompareTo(Fail(fail));

        /// <summary>
        /// Equality override
        /// </summary>
        [Pure]
        public bool Equals(SUCCESS success) =>
            Equals(Success(success));

        /// <summary>
        /// Equality override
        /// </summary>
        [Pure]
        public bool Equals(Seq<FAIL> fail) =>
            Equals(Fail(fail));

        /// <summary>
        /// Equality override
        /// </summary>
        [Pure]
        public bool Equals(Validation<FAIL, SUCCESS> other) =>
            EqChoice<
                MSeq<FAIL>,
                EqDefault<SUCCESS>, 
                FoldValidation<FAIL, SUCCESS>, 
                Validation<FAIL, SUCCESS>,
                Seq<FAIL>, SUCCESS>
               .Inst.Equals(this, other);

        /// <summary>
        /// Counts the Validation
        /// </summary>
        /// <param name="self">Validation to count</param>
        /// <returns>1 if the Validation is in a Success state, 0 otherwise.</returns>
        [Pure]
        public int Count() =>
            IsFail
                ? 0
                : 1;

        /// <summary>
        /// Iterate the Validation
        /// action is invoked if in the Success state
        /// </summary>
        public Unit Iter(Action<SUCCESS> Success) =>
            iter<FoldValidation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS>, SUCCESS>(this, Success);

        /// <summary>
        /// Iterate the Validation
        /// action is invoked if in the Success state
        /// </summary>
        public Unit BiIter(Action<SUCCESS> Success, Action<FAIL> Fail) =>
            biIter<FoldValidation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS>, FAIL, SUCCESS>(this, Fail, Success);

        /// <summary>
        /// Invokes a predicate on the value of the Validation if it's in the Success state
        /// </summary>
        /// <typeparam name="L">Fail</typeparam>
        /// <typeparam name="R">Success</typeparam>
        /// <param name="self">Validation to forall</param>
        /// <param name="Success">Predicate</param>
        /// <returns>True if the Validation is in a Fail state.  
        /// True if the Validation is in a Success state and the predicate returns True.  
        /// False otherwise.</returns>
        [Pure]
        public bool ForAll(Func<SUCCESS, bool> Success) =>
            forall<FoldValidation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS>, SUCCESS>(this, Success);

        /// <summary>
        /// Invokes a predicate on the value of the Validation if it's in the Success state
        /// </summary>
        /// <typeparam name="L">Fail</typeparam>
        /// <typeparam name="R">Success</typeparam>
        /// <param name="self">Validation to forall</param>
        /// <param name="Success">Predicate</param>
        /// <param name="Fail">Predicate</param>
        /// <returns>True if Validation Predicate returns true</returns>
        [Pure]
        public bool BiForAll(Func<SUCCESS, bool> Success, Func<FAIL, bool> Fail) =>
            biForAll<FoldValidation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS>, FAIL, SUCCESS>(this, Fail, Success);

        /// <summary>
        /// <para>
        /// Validation types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the Fail-identity of the operator),
        /// and a list, reduces the list using the binary operator, from Fail to
        /// Success:
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Success">Folder function, applied if structure is in a Success state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public S Fold<S>(S state, Func<S, SUCCESS, S> Success) =>
            default(FoldValidation<FAIL, SUCCESS>).Fold(this, state, Success)(unit);

        /// <summary>
        /// <para>
        /// Validation types are like lists of 0 or 1 items, and therefore follow the 
        /// same rules when folding.
        /// </para><para>
        /// In the case of lists, 'Fold', when applied to a binary
        /// operator, a starting value(typically the Fail-identity of the operator),
        /// and a list, reduces the list using the binary operator, from Fail to
        /// Success:
        /// </para>
        /// </summary>
        /// <typeparam name="S">Aggregate state type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="Success">Folder function, applied if Validation is in a Success state</param>
        /// <param name="Fail">Folder function, applied if Validation is in a Fail state</param>
        /// <returns>The aggregate state</returns>
        [Pure]
        public S BiFold<S>(S state, Func<S, SUCCESS, S> Success, Func<S, FAIL, S> Fail) =>
            default(FoldValidation<FAIL, SUCCESS>).BiFold(this, state, Fail, Success);

        /// <summary>
        /// Invokes a predicate on the value of the Validation if it's in the Success state
        /// </summary>
        /// <typeparam name="L">Fail</typeparam>
        /// <typeparam name="R">Success</typeparam>
        /// <param name="self">Validation to check existence of</param>
        /// <param name="pred">Predicate</param>
        /// <returns>True if the Validation is in a Success state and the predicate returns True.  False otherwise.</returns>
        [Pure]
        public bool Exists(Func<SUCCESS, bool> pred) =>
            exists<FoldValidation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS>, SUCCESS>(this, pred);

        /// <summary>
        /// Invokes a predicate on the value of the Validation
        /// </summary>
        /// <typeparam name="L">Fail</typeparam>
        /// <typeparam name="R">Success</typeparam>
        /// <param name="self">Validation to check existence of</param>
        /// <param name="Success">Success predicate</param>
        /// <param name="Fail">Fail predicate</param>
        /// <returns>True if the predicate returns True.  False otherwise.</returns>
        [Pure]
        public bool BiExists(Func<SUCCESS, bool> Success, Func<FAIL, bool> Fail) =>
            biExists<FoldValidation<FAIL, SUCCESS>, Validation<FAIL, SUCCESS>, FAIL, SUCCESS>(this, Fail, Success);

        /// <summary>
        /// Impure iteration of the bound value in the structure
        /// </summary>
        /// <returns>
        /// Returns the original unmodified structure
        /// </returns>
        public Validation<FAIL, SUCCESS> Do(Action<SUCCESS> f)
        {
            Iter(f);
            return this;
        }

        /// <summary>
        /// Maps the value in the Validation if it's in a Success state
        /// </summary>
        /// <typeparam name="L">Fail</typeparam>
        /// <typeparam name="R">Success</typeparam>
        /// <typeparam name="Ret">Mapped Validation type</typeparam>
        /// <param name="self">Validation to map</param>
        /// <param name="mapper">Map function</param>
        /// <returns>Mapped Validation</returns>
        [Pure]
        public Validation<FAIL, Ret> Map<Ret>(Func<SUCCESS, Ret> mapper) =>
            FValidation<FAIL, SUCCESS, Ret>.Inst.Map(this, mapper);

        /// <summary>
        /// Bi-maps the value in the Validation if it's in a Success state
        /// </summary>
        /// <typeparam name="L">Fail</typeparam>
        /// <typeparam name="R">Success</typeparam>
        /// <typeparam name="RRet">Success return</typeparam>
        /// <param name="self">Validation to map</param>
        /// <param name="Success">Success map function</param>
        /// <param name="Fail">Fail map function</param>
        /// <returns>Mapped Validation</returns>
        [Pure]
        public Validation<FAIL, Ret> BiMap<Ret>(Func<SUCCESS, Ret> Success, Func<Seq<FAIL>, Ret> Fail) =>
            FValidation<FAIL, SUCCESS, Ret>.Inst.BiMap(this, Fail, Success);

        /// <summary>
        /// Maps the value in the Validation if it's in a Fail state
        /// </summary>
        /// <typeparam name="Ret">Fail return</typeparam>
        /// <param name="Fail">Fail map function</param>
        /// <returns>Mapped Validation</returns>
        [Pure]
        public Validation<Ret, SUCCESS> MapFail<Ret>(Func<FAIL, Ret> Fail) =>
            FValidationBi<FAIL, SUCCESS, Ret, SUCCESS>.Inst.BiMap(this, Fail, identity);

        /// <summary>
        /// Bi-maps the value in the Validation
        /// </summary>
        /// <typeparam name="FAIL2">Fail return</typeparam>
        /// <typeparam name="SUCCESS2">Success return</typeparam>
        /// <param name="Success">Success map function</param>
        /// <param name="Fail">Fail map function</param>
        /// <returns>Mapped Validation</returns>
        [Pure]
        public Validation<FAIL2, SUCCESS2> BiMap<FAIL2, SUCCESS2>(Func<SUCCESS, SUCCESS2> Success, Func<FAIL, FAIL2> Fail) =>
            FValidationBi<FAIL, SUCCESS, FAIL2, SUCCESS2>.Inst.BiMap(this, Fail, Success);

        /// <summary>
        /// Maps the value in the Validation if it's in a Success state
        /// </summary>
        /// <typeparam name="L">Fail</typeparam>
        /// <typeparam name="TR">Success</typeparam>
        /// <typeparam name="UR">Mapped Validation type</typeparam>
        /// <param name="self">Validation to map</param>
        /// <param name="map">Map function</param>
        /// <returns>Mapped Validation</returns>
        [Pure]
        public Validation<FAIL, U> Select<U>(Func<SUCCESS, U> map) =>
            FValidation<FAIL, SUCCESS, U>.Inst.Map(this, map);

        [Pure]
        public Validation<FAIL, U> Bind<U>(Func<SUCCESS, Validation<FAIL, U>> f) =>
            IsSuccess
                ? f(success)
                : Validation<FAIL, U>.Fail(FailValue);

        [Pure]
        public Validation<FAIL, V> SelectMany<U, V>(Func<SUCCESS, Validation<FAIL, U>> bind, Func<SUCCESS, U, V> project)
        {
            var t = success;
            return IsSuccess
                ? bind(t).Map(u => project(t, u))
                : Validation<FAIL, V>.Fail(FailValue);
        }

        [Pure]
        public Validation<FAIL, SUCCESS> Filter(Func<SUCCESS, bool> f) =>
            IsSuccess && f(success)
                ? this
                : Fail(Seq<FAIL>.Empty);

        [Pure]
        public Validation<FAIL, SUCCESS> Where(Func<SUCCESS, bool> f) =>
            IsSuccess && f(success)
                ? this
                : Fail(Seq<FAIL>.Empty);
    }

    /// <summary>
    /// Context for the fluent Either matching
    /// </summary>
    public struct ValidationContext<FAIL, SUCCESS, Ret>
    {
        readonly Validation<FAIL, SUCCESS> validation;
        readonly Func<SUCCESS, Ret> success;

        internal ValidationContext(Validation<FAIL, SUCCESS> validation, Func<SUCCESS, Ret> success)
        {
            this.validation = validation;
            this.success = success;
        }

        /// <summary>
        /// Fail match
        /// </summary>
        /// <param name="Fail"></param>
        /// <returns>Result of the match</returns>
        [Pure]
        public Ret Fail(Func<Seq<FAIL>, Ret> fail) =>
            validation.Match(success, fail);
    }

    /// <summary>
    /// Context for the fluent Validation matching
    /// </summary>
    public struct ValidationUnitContext<FAIL, SUCCESS>
    {
        readonly Validation<FAIL, SUCCESS> validation;
        readonly Action<SUCCESS> success;

        internal ValidationUnitContext(Validation<FAIL, SUCCESS> validation, Action<SUCCESS> success)
        {
            this.validation = validation;
            this.success = success;
        }

        public Unit Left(Action<Seq<FAIL>> fail) =>
            validation.Match(success, fail);
    }
}
