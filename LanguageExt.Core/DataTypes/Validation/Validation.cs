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
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    /// <summary>
    /// Like `Either` but collects the failed values
    /// </summary>
    /// <typeparam name="MonoidFail"></typeparam>
    /// <typeparam name="FAIL"></typeparam>
    /// <typeparam name="SUCCESS"></typeparam>
    [Serializable]
    public struct Validation<MonoidFail, FAIL, SUCCESS> :
        IEnumerable<ValidationData<MonoidFail, FAIL, SUCCESS>>,
        IComparable<Validation<MonoidFail, FAIL, SUCCESS>>,
        IComparable<SUCCESS>,
        IEquatable<Validation<MonoidFail, FAIL, SUCCESS>>,
        IEquatable<SUCCESS>,
        ISerializable
        where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL>
    {
        readonly FAIL fail;
        readonly SUCCESS success;
        readonly Validation.StateType state;

        [Pure]
        Validation(SUCCESS success)
        {
            if (isnull(success)) throw new ValueIsNullException();
            this.success = success;
            this.fail = default(FAIL);
            this.state = Validation.StateType.Success;
        }

        [Pure]
        Validation(FAIL fail)
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
        public Validation(IEnumerable<ValidationData<MonoidFail, FAIL, SUCCESS>> validationData)
        {
            var seq = Seq(validationData);
            if (seq.IsEmpty)
            {
                this.state = Validation.StateType.Fail;
                this.fail = default(MonoidFail).Empty();
                this.success = default(SUCCESS);
            }
            else
            {
                this.fail = seq.Head.Fail;
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
                    fail = default(FAIL);
                    break;
                case Validation.StateType.Fail:
                    fail = (FAIL)info.GetValue("Fail", typeof(FAIL));
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

        internal FAIL FailValue => isnull(fail) ? default(MonoidFail).Empty() : fail;
        internal SUCCESS SuccessValue => success;

        [Pure]
        public bool IsFail =>
            state == Validation.StateType.Fail;

        [Pure]
        public bool IsSuccess =>
            state == Validation.StateType.Success;

        IEnumerable<ValidationData<MonoidFail, FAIL, SUCCESS>> Enum()
        {
            yield return new ValidationData<MonoidFail, FAIL, SUCCESS>(state, success, FailValue);
        }

        public IEnumerator<ValidationData<MonoidFail, FAIL, SUCCESS>> GetEnumerator() =>
            Enum().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            Enum().GetEnumerator();

        /// <summary>
        /// Implicit conversion operator from `SUCCESS` to `Validation<MonoidFail, FAIL, SUCCESS>`
        /// </summary>
        /// <param name="value">`value`, must not be `null`.</param>
        /// <exception cref="ValueIsNullException">`value` is `null`</exception>
        [Pure]
        public static implicit operator Validation<MonoidFail, FAIL, SUCCESS>(SUCCESS value) =>
            isnull(value)
                ? throw new ValueIsNullException()
                : Success(value);

        /// <summary>
        /// Implicit conversion operator from `FAIL` to `Validation<MonoidFail, FAIL, SUCCESS>`
        /// </summary>
        /// <param name="value">`value`, must not be `null`.</param>
        /// <exception cref="ValueIsNullException">`value` is `null`</exception>
        [Pure]
        public static implicit operator Validation<MonoidFail, FAIL, SUCCESS>(FAIL value) =>
            isnull(value)
                ? throw new ValueIsNullException()
                : Fail(value);

        /// <summary>
        /// Reference version for use in pattern-matching
        /// </summary>
        [Pure]
        public ValidationCase<FAIL, SUCCESS> Case =>
            state switch
            {
                Validation.StateType.Success => SuccCase<FAIL, SUCCESS>.New(success),
                Validation.StateType.Fail    => FailCase<FAIL, SUCCESS>.New(fail),
                _                            => null
            };

        /// <summary>
        /// Success constructor
        /// </summary>
        [Pure]
        public static Validation<MonoidFail, FAIL, SUCCESS> Success(SUCCESS success) =>
            new Validation<MonoidFail, FAIL, SUCCESS>(success);

        /// <summary>
        /// Fail constructor
        /// </summary>
        [Pure]
        public static Validation<MonoidFail, FAIL, SUCCESS> Fail(FAIL fail) =>
            new Validation<MonoidFail, FAIL, SUCCESS>(fail);

        [Pure]
        public Validation<MonoidFail, FAIL, SUCCESS> Disjunction<SUCCESSB>(Validation<MonoidFail, FAIL, SUCCESSB> other)
        {
            if (IsSuccess && other.IsSuccess) return this;
            if (IsSuccess) return new Validation<MonoidFail, FAIL, SUCCESS>(other.FailValue);
            if (other.IsSuccess) return this;
            return new Validation<MonoidFail, FAIL, SUCCESS>(default(MonoidFail).Append(FailValue, other.FailValue));
        }

        /// <summary>
        /// Fluent matching
        /// </summary>
        public ValidationContext<MonoidFail, FAIL, SUCCESS, Ret> Succ<Ret>(Func<SUCCESS, Ret> f) =>
            new ValidationContext<MonoidFail, FAIL, SUCCESS, Ret>(this, f);

        /// <summary>
        /// Fluent matching
        /// </summary>
        public ValidationUnitContext<MonoidFail, FAIL, SUCCESS> Succ<Ret>(Action<SUCCESS> f) =>
            new ValidationUnitContext<MonoidFail, FAIL, SUCCESS>(this, f);

        /// <summary>
        /// Invokes the `Succ` or `Fail` function depending on the state of the `Validation`
        /// </summary>
        /// <typeparam name="Ret">Return type</typeparam>
        /// <param name="Succ">Function to invoke if in a `Success` state</param>
        /// <param name="Fail">Function to invoke if in a `Fail` state</param>
        /// <returns>The return value of the invoked function</returns>
        [Pure]
        public Ret Match<Ret>(Func<SUCCESS, Ret> Succ, Func<FAIL, Ret> Fail) =>
            Check.NullReturn(MatchUnsafe(Succ, Fail));

        /// <summary>
        /// Returns `Succ` value or invokes `Fail` function depending on the state of the `Validation`
        /// </summary>
        /// <typeparam name="Ret">Return type</typeparam>
        /// <param name="Succ">Value to return if in a `Success` state</param>
        /// <param name="Fail">Function to invoke if in a `Fail` state</param>
        /// <returns>The return value of the invoked function</returns>
        [Pure]
        public Ret Match<Ret>(Ret Succ, Func<FAIL, Ret> Fail) =>
            Check.NullReturn(MatchUnsafe(_ => Succ, Fail));

        /// <summary>
        /// Invokes the `Succ` or `Fail` function depending on the state of the `Validation`
        /// </summary>
        /// <typeparam name="Ret">Return type</typeparam>
        /// <param name="Succ">Function to invoke if in a `Success` state</param>
        /// <param name="Fail">Function to invoke if in a `Fail` state</param>
        /// <returns>The return value of the invoked function</returns>
        [Pure]
        public Ret MatchUnsafe<Ret>(Func<SUCCESS, Ret> Succ, Func<FAIL, Ret> Fail) =>
            IsFail
                ? Fail == null
                    ? throw new ArgumentNullException(nameof(Fail))
                    : Fail(FailValue)
                : Succ == null
                    ? throw new ArgumentNullException(nameof(Succ))
                    : Succ(success);

        /// <summary>
        /// Invokes the `Succ` or `Fail` action depending on the state of the `Validation`
        /// </summary>
        /// <param name="Succ">Action to invoke if in a `Success` state</param>
        /// <param name="Fail">Action to invoke if in a `Fail` state</param>
        /// <returns>Unit</returns>
        public Unit Match(Action<SUCCESS> Succ, Action<FAIL> Fail)
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
        public async Task<R2> MatchAsync<R2>(Func<SUCCESS, Task<R2>> SuccAsync, Func<FAIL, R2> Fail) =>
            await Match(SuccAsync, f => Fail(f).AsTask());

        /// <summary>
        /// Match the two states of the Validation and return a promise for a non-null R2.
        /// </summary>
        /// <returns>A promise to return a non-null R2</returns>
        public async Task<R2> MatchAsync<R2>(Func<SUCCESS, Task<R2>> SuccAsync, Func<FAIL, Task<R2>> FailAsync) =>
            await Match(SuccAsync, FailAsync);

        /// <summary>
        /// Executes the Fail function if the Validation is in a Fail state.
        /// Returns the Success value if the Validation is in a Success state.
        /// </summary>
        /// <param name="Fail">Function to generate a Success value if in the Fail state</param>
        /// <returns>Returns an unwrapped Success value</returns>
        [Pure]
        public SUCCESS IfFail(Func<SUCCESS> Fail) =>
            ifLeft<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this, Fail);

        /// <summary>
        /// Executes the FailMap function if the Validation is in a Fail state.
        /// Returns the Success value if the Validation is in a Success state.
        /// </summary>
        /// <param name="FailMap">Function to generate a Success value if in the Fail state</param>
        /// <returns>Returns an unwrapped Success value</returns>
        [Pure]
        public SUCCESS IfFail(Func<FAIL, SUCCESS> FailMap) =>
            ifLeft<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this, FailMap);

        /// <summary>
        /// Returns the SuccessValue if the Validation is in a Fail state.
        /// Returns the Success value if the Validation is in a Success state.
        /// </summary>
        /// <param name="SuccessValue">Value to return if in the Fail state</param>
        /// <returns>Returns an unwrapped Success value</returns>
        [Pure]
        public SUCCESS IfFail(SUCCESS SuccessValue) =>
            ifLeft<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this, SuccessValue);

        /// <summary>
        /// Executes the Fail action if the Validation is in a Fail state.
        /// </summary>
        /// <param name="Fail">Function to generate a Success value if in the Fail state</param>
        /// <returns>Returns an unwrapped Success value</returns>
        public Unit IfFail(Action<FAIL> Fail) =>
            ifLeft<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this, Fail);

        /// <summary>
        /// Invokes the Success action if the Validation is in a Success state, otherwise does nothing
        /// </summary>
        /// <param name="Success">Action to invoke</param>
        /// <returns>Unit</returns>
        public Unit IfSuccess(Action<SUCCESS> Success) =>
            ifRight<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this, Success);

        /// <summary>
        /// Returns the FailValue if the Validation is in a Success state.
        /// Returns the Fail value if the Validation is in a Fail state.
        /// </summary>
        /// <param name="FailValue">Value to return if in the Fail state</param>
        /// <returns>Returns an unwrapped Fail value</returns>
        [Pure]
        public FAIL IfSuccess(FAIL FailValue) =>
            ifRight<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this, FailValue);

        /// <summary>
        /// Returns the result of Success() if the Validation is in a Success state.
        /// Returns the Fail value if the Validation is in a Fail state.
        /// </summary>
        /// <param name="Success">Function to generate a Fail value if in the Success state</param>
        /// <returns>Returns an unwrapped Fail value</returns>
        [Pure]
        public FAIL IfSuccess(Func<FAIL> Success) =>
            ifRight<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this, Success);

        /// <summary>
        /// Returns the result of SuccessMap if the Validation is in a Success state.
        /// Returns the Fail value if the Validation is in a Fail state.
        /// </summary>
        /// <param name="SuccessMap">Function to generate a Fail value if in the Success state</param>
        /// <returns>Returns an unwrapped Fail value</returns>
        [Pure]
        public FAIL IfSuccess(Func<SUCCESS, FAIL> SuccessMap) =>
            ifRight<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this, SuccessMap);

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
            hashCode<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this);

        /// <summary>
        /// Equality check
        /// </summary>
        /// <param name="obj">Object to test for equality</param>
        /// <returns>True if equal</returns>
        [Pure]
        public override bool Equals(object obj) =>
            !ReferenceEquals(obj, null) &&
            obj is Validation<MonoidFail, FAIL, SUCCESS> &&
            EqChoice<
                MonoidFail, 
                EqDefault<SUCCESS>, 
                FoldValidation<MonoidFail, FAIL, SUCCESS>, 
                Validation<MonoidFail, FAIL, SUCCESS>, 
                FAIL, SUCCESS>
               .Inst.Equals(this, (Validation<MonoidFail, FAIL, SUCCESS>)obj);


        /// <summary>
        /// Project the Validation into a Lst
        /// </summary>
        [Pure]
        public Lst<SUCCESS> SuccessToList() =>
            rightToList<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this);

        /// <summary>
        /// Project the Validation into an immutable array
        /// </summary>
        [Pure]
        public Arr<SUCCESS> SuccessToArray() =>
            rightToArray<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this);

        /// <summary>
        /// Project the Validation into a Lst
        /// </summary>
        [Pure]
        public Lst<FAIL> FailToList() =>
            leftToList<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this);

        /// <summary>
        /// Project the Validation into an immutable array R
        /// </summary>
        [Pure]
        public Arr<FAIL> FailToArray() =>
            leftToArray<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this);

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
            FailAsEnumerable();

        /// <summary>
        /// Project the Validation success into a Seq
        /// </summary>
        [Pure]
        public Seq<SUCCESS> SuccessAsEnumerable() =>
            rightAsEnumerable<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this);

        /// <summary>
        /// Project the Validation fail into a Seq
        /// </summary>
        [Pure]
        public Seq<FAIL> FailAsEnumerable() =>
            leftAsEnumerable<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this);

        /// <summary>
        /// Convert the Validation to an Option
        /// </summary>
        [Pure]
        public Option<SUCCESS> ToOption() =>
            toOption<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this);

        /// <summary>
        /// Convert the Validation to an EitherUnsafe
        /// </summary>
        [Pure]
        public Either<FAIL, SUCCESS> ToEither() =>
            toEither<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this);

        /// <summary>
        /// Convert the Validation to an EitherUnsafe
        /// </summary>
        [Pure]
        public EitherUnsafe<FAIL, SUCCESS> ToEitherUnsafe() =>
            toEitherUnsafe<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this);

        /// <summary>
        /// Convert the Validation to an TryOption
        /// </summary>
        [Pure]
        public TryOption<SUCCESS> ToTryOption() =>
            toTryOption<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this);


        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="lhs">The left hand side of the operation</param>
        /// <param name="rhs">The right hand side of the operation</param>
        /// <returns>True if lhs < rhs</returns>
        [Pure]
        public static bool operator <(Validation<MonoidFail, FAIL, SUCCESS> lhs, Validation<MonoidFail, FAIL, SUCCESS> rhs) =>
            OrdChoice<
                OrdDefault<FAIL>, 
                OrdDefault<SUCCESS>, 
                FoldValidation<MonoidFail, FAIL, SUCCESS>, 
                Validation<MonoidFail, FAIL, SUCCESS>, 
                FAIL, SUCCESS>
               .Inst.Compare(lhs, rhs) < 0;

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="lhs">The left hand side of the operation</param>
        /// <param name="rhs">The right hand side of the operation</param>
        /// <returns>True if lhs <= rhs</returns>
        [Pure]
        public static bool operator <=(Validation<MonoidFail, FAIL, SUCCESS> lhs, Validation<MonoidFail, FAIL, SUCCESS> rhs) =>
            OrdChoice<
                OrdDefault<FAIL>,
                OrdDefault<SUCCESS>,
                FoldValidation<MonoidFail, FAIL, SUCCESS>,
                Validation<MonoidFail, FAIL, SUCCESS>,
                FAIL, SUCCESS>
               .Inst.Compare(lhs, rhs) <= 0;

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="lhs">The left hand side of the operation</param>
        /// <param name="rhs">The right hand side of the operation</param>
        /// <returns>True if lhs > rhs</returns>
        [Pure]
        public static bool operator >(Validation<MonoidFail, FAIL, SUCCESS> lhs, Validation<MonoidFail, FAIL, SUCCESS> rhs) =>
            OrdChoice<
                OrdDefault<FAIL>,
                OrdDefault<SUCCESS>,
                FoldValidation<MonoidFail, FAIL, SUCCESS>,
                Validation<MonoidFail, FAIL, SUCCESS>,
                FAIL, SUCCESS>
               .Inst.Compare(lhs, rhs) > 0;

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="lhs">The left hand side of the operation</param>
        /// <param name="rhs">The right hand side of the operation</param>
        /// <returns>True if lhs >= rhs</returns>
        [Pure]
        public static bool operator >=(Validation<MonoidFail, FAIL, SUCCESS> lhs, Validation<MonoidFail, FAIL, SUCCESS> rhs) =>
            OrdChoice<
                OrdDefault<FAIL>,
                OrdDefault<SUCCESS>,
                FoldValidation<MonoidFail, FAIL, SUCCESS>,
                Validation<MonoidFail, FAIL, SUCCESS>,
                FAIL, SUCCESS>
               .Inst.Compare(lhs, rhs) >= 0;

        /// <summary>
        /// Equality operator override
        /// </summary>
        [Pure]
        public static bool operator ==(Validation<MonoidFail, FAIL, SUCCESS> lhs, Validation<MonoidFail, FAIL, SUCCESS> rhs) =>
            lhs.Equals(rhs);

        /// <summary>
        /// Non-equality operator override
        /// </summary>
        [Pure]
        public static bool operator !=(Validation<MonoidFail, FAIL, SUCCESS> lhs, Validation<MonoidFail, FAIL, SUCCESS> rhs) =>
            !(lhs == rhs);

        /// <summary>
        /// Coalescing operator
        /// </summary>
        [Pure]
        public static Validation<MonoidFail, FAIL, SUCCESS> operator |(Validation<MonoidFail, FAIL, SUCCESS> lhs, Validation<MonoidFail, FAIL, SUCCESS> rhs) =>
            default(FoldValidation<MonoidFail, FAIL, SUCCESS>).Append(lhs, rhs);

        /// <summary>
        /// Override of the True operator to return True if the Validation is Success
        /// </summary>
        [Pure]
        public static bool operator true(Validation<MonoidFail, FAIL, SUCCESS> value) =>
            value.IsSuccess;

        /// <summary>
        /// Override of the False operator to return True if the Validation is Fail
        /// </summary>
        [Pure]
        public static bool operator false(Validation<MonoidFail, FAIL, SUCCESS> value) =>
            value.IsFail;

        /// <summary>
        /// CompareTo override
        /// </summary>
        [Pure]
        public int CompareTo(Validation<MonoidFail, FAIL, SUCCESS> other) =>
            OrdChoice<
                OrdDefault<FAIL>,
                OrdDefault<SUCCESS>,
                FoldValidation<MonoidFail, FAIL, SUCCESS>,
                Validation<MonoidFail, FAIL, SUCCESS>,
                FAIL, SUCCESS>
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
        public int CompareTo(FAIL fail) =>
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
        public bool Equals(FAIL fail) =>
            Equals(Fail(fail));

        /// <summary>
        /// Equality override
        /// </summary>
        [Pure]
        public bool Equals(Validation<MonoidFail, FAIL, SUCCESS> other) =>
            EqChoice<
                MonoidFail, 
                EqDefault<SUCCESS>, 
                FoldValidation<MonoidFail, FAIL, SUCCESS>, 
                Validation<MonoidFail, FAIL, SUCCESS>, 
                FAIL, SUCCESS>
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
            iter<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, SUCCESS>(this, Success);

        /// <summary>
        /// Iterate the Validation
        /// action is invoked if in the Success state
        /// </summary>
        public Unit BiIter(Action<SUCCESS> Success, Action<FAIL> Fail) =>
            biIter<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this, Fail, Success);

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
            forall<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, SUCCESS>(this, Success);

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
            biForAll<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this, Fail, Success);

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
            default(FoldValidation<MonoidFail, FAIL, SUCCESS>).Fold(this, state, Success)(unit);

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
            default(FoldValidation<MonoidFail, FAIL, SUCCESS>).BiFold(this, state, Fail, Success);

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
            exists<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, SUCCESS>(this, pred);

        /// <summary>
        /// Invokes a predicate on the value of the Validation
        /// </summary>
        /// <typeparam name="L">Fail</typeparam>
        /// <typeparam name="R">Success</typeparam>
        /// <param name="self">Validation to check existence of</param>
        /// <param name="Success">Success predicate</param>
        /// <param name="Fail">Fail predicate</param>
        [Pure]
        public bool BiExists(Func<SUCCESS, bool> Success, Func<FAIL, bool> Fail) =>
            biExists<FoldValidation<MonoidFail, FAIL, SUCCESS>, Validation<MonoidFail, FAIL, SUCCESS>, FAIL, SUCCESS>(this, Fail, Success);

        /// <summary>
        /// Impure iteration of the bound value in the structure
        /// </summary>
        /// <returns>
        /// Returns the original unmodified structure
        /// </returns>
        public Validation<MonoidFail, FAIL, SUCCESS> Do(Action<SUCCESS> f)
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
        public Validation<MonoidFail, FAIL, Ret> Map<Ret>(Func<SUCCESS, Ret> mapper) =>
            FValidation<MonoidFail, FAIL, SUCCESS, Ret>.Inst.Map(this, mapper);

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
        public Validation<MonoidFail, FAIL, Ret> BiMap<Ret>(Func<SUCCESS, Ret> Success, Func<FAIL, Ret> Fail) =>
            FValidation<MonoidFail, FAIL, SUCCESS, Ret>.Inst.BiMap(this, Fail, Success);

        /// <summary>
        /// Maps the value in the Validation if it's in a Fail state
        /// </summary>
        /// <typeparam name="MonoidRet">Monad of Fail</typeparam>
        /// <typeparam name="Ret">Fail return</typeparam>
        /// <param name="Fail">Fail map function</param>
        /// <returns>Mapped Validation</returns>
        [Pure]
        public Validation<MonoidRet, Ret, SUCCESS> MapFail<MonoidRet, Ret>(Func<FAIL, Ret> Fail) where MonoidRet : struct, Monoid<Ret>, Eq<Ret> =>
            FValidationBi<MonoidFail, FAIL, SUCCESS, MonoidRet, Ret, SUCCESS>.Inst.BiMap(this, Fail, identity);

        /// <summary>
        /// Bi-maps the value in the Validation
        /// </summary>
        /// <typeparam name="MonoidFail2">Monad of Fail</typeparam>
        /// <typeparam name="FAIL2">Fail return</typeparam>
        /// <typeparam name="SUCCESS2">Success return</typeparam>
        /// <param name="Success">Success map function</param>
        /// <param name="Fail">Fail map function</param>
        /// <returns>Mapped Validation</returns>
        [Pure]
        public Validation<MonoidFail2, FAIL2, SUCCESS2> BiMap<MonoidFail2, FAIL2, SUCCESS2>(Func<SUCCESS, SUCCESS2> Success, Func<FAIL, FAIL2> Fail) where MonoidFail2 : struct, Monoid<FAIL2>, Eq<FAIL2> =>
            FValidationBi<MonoidFail, FAIL, SUCCESS, MonoidFail2, FAIL2, SUCCESS2>.Inst.BiMap(this, Fail, Success);

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
        public Validation<MonoidFail, FAIL, U> Select<U>(Func<SUCCESS, U> map) =>
            FValidation<MonoidFail, FAIL, SUCCESS, U>.Inst.Map(this, map);

        [Pure]
        public Validation<MonoidFail, FAIL, U> Bind<U>(Func<SUCCESS, Validation<MonoidFail, FAIL, U>> f) =>
            IsSuccess
                ? f(success)
                : Validation<MonoidFail, FAIL, U>.Fail(FailValue);

        /// <summary>
        /// Bi-bind.  Allows mapping of both monad states
        /// </summary>
        [Pure]
        public Validation<MonoidFail, FAIL, B> BiBind<B>(Func<SUCCESS, Validation<MonoidFail, FAIL, B>> Succ, Func<FAIL, Validation<MonoidFail, FAIL, B>> Fail) =>
            IsSuccess
                ? Succ(SuccessValue)
                : Fail(FailValue);

        [Pure]
        public Validation<MonoidFail, FAIL, V> SelectMany<U, V>(Func<SUCCESS, Validation<MonoidFail, FAIL, U>> bind, Func<SUCCESS, U, V> project)
        {
            var t = success;
            return IsSuccess
                ? bind(t).Map(u => project(t, u))
                : Validation<MonoidFail, FAIL, V>.Fail(FailValue);
        }
    }

    /// <summary>
    /// Context for the fluent Either matching
    /// </summary>
    public struct ValidationContext<MonoidFail, FAIL, SUCCESS, Ret>
        where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL>
    {
        readonly Validation<MonoidFail, FAIL, SUCCESS> validation;
        readonly Func<SUCCESS, Ret> success;

        internal ValidationContext(Validation<MonoidFail, FAIL, SUCCESS> validation, Func<SUCCESS, Ret> success)
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
        public Ret Fail(Func<FAIL, Ret> fail) =>
            validation.Match(success, fail);
    }

    /// <summary>
    /// Context for the fluent Validation matching
    /// </summary>
    public struct ValidationUnitContext<MonoidFail, FAIL, SUCCESS>
        where MonoidFail : struct, Monoid<FAIL>, Eq<FAIL>
    {
        readonly Validation<MonoidFail, FAIL, SUCCESS> validation;
        readonly Action<SUCCESS> success;

        internal ValidationUnitContext(Validation<MonoidFail, FAIL, SUCCESS> validation, Action<SUCCESS> success)
        {
            this.validation = validation;
            this.success = success;
        }

        public Unit Left(Action<FAIL> fail) =>
            validation.Match(success, fail);
    }
}
