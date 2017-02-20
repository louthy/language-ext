using System;
using System.Linq;
using LanguageExt;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

namespace LanguageExt
{
    /// <summary>
    /// NumType - inspired by Haskell's 'newtype' keyword.  This is setup for numeric
    /// types,and expects a Num<A> class-instance as an argument (TInt, TDouble, etc.)
    /// 
    /// https://wiki.haskell.org/Newtype
    /// 
    /// Derive type from this one to get: Equatable, Comparable, Appendable, Foldable, 
    /// Functor, Iterable: strongly typed values.
    ///
    /// For example:
    ///
    ///     class Metres : NumType<Metres, TDouble, double> { public Metres(double x) : base(x) {} }
    ///
    /// Will not accept null values
    /// </summary>
    /// <typeparam name="NUMTYPE">Self reference type - i.e. class Metres : NumType<Metres, ... ></typeparam>
    /// <typeparam name="NUM">Num of A, e.g. TInt, TDouble, TFloat, etc.</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <typeparam name="PRED">A predicate to be applied to the value passed into the constructor.
    /// Allows for constraints on the value stored.</typeparam>
    [Serializable]
    public abstract class NumType<NUMTYPE, NUM, A, PRED> :
        IEquatable<NUMTYPE>,
        IComparable<NUMTYPE>
        where NUM : struct, Num<A>
        where PRED : struct, Pred<A>
        where NUMTYPE : NumType<NUMTYPE, NUM, A, PRED>
    {
        protected readonly A Value;

        /// <summary>
        /// Constructor function
        /// </summary>
        public static readonly Func<A, NUMTYPE> New = Reflect.Util.CtorInvoke<A, NUMTYPE>();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Value to bind</param>
        /// <exception cref="ArgumentOutOfRangeException">If argument fails to pass the predicate provided in the generic argument PRED</exception>
        /// <exception cref="ArgumentNullException">Null values are not accepted</exception>
        public NumType(A value)
        {
            if(!default(PRED).True(value)) throw new ArgumentOutOfRangeException(nameof(value),value,$"Argument failed {typeof(NUMTYPE).Name} NumType predicate");
            if (isnull(value)) throw new ArgumentNullException(nameof(value));
            Value = value;
        }

        /// <summary>
        /// Explicit conversion operator for extracting the bound value
        /// </summary>
        /// <param name="type"></param>
        [Pure]
        public static explicit operator A(NumType<NUMTYPE, NUM, A, PRED> type) =>
            type.Value;

        /// <summary>
        /// Sum of NumType(x) and NumType(y)
        /// </summary>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs + rhs</returns>
        [Pure]
        public virtual NUMTYPE Plus(NUMTYPE rhs) =>
            from x in this
            from y in rhs
            select plus<NUM, A>(x, y);

        /// <summary>
        /// Subtract of NewType(x) and NewType(y)
        /// </summary>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs - rhs</returns>
        [Pure]
        public virtual NUMTYPE Subtract(NUMTYPE rhs) =>
            from x in this
            from y in rhs
            select subtract<NUM, A>(x, y);

        /// <summary>
        /// Divide NewType(x) and NewType(y)
        /// </summary>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs / rhs</returns>
        [Pure]
        public virtual NUMTYPE Divide(NUMTYPE rhs) =>
            from x in this
            from y in rhs
            select subtract<NUM, A>(x, y);

        /// <summary>
        /// Multiply NewType(x) and NewType(y)
        /// </summary>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs * rhs</returns>
        [Pure]
        public virtual NUMTYPE Product(NUMTYPE rhs) =>
            from x in this
            from y in rhs
            select subtract<NUM, A>(x, y);

        /// <summary>
        /// Find the absolute value of a number
        /// </summary>
        /// <param name="x">The value to find the absolute value of</param>
        /// <returns>The non-negative absolute value of x</returns>
        public virtual NUMTYPE Abs() =>
            New(default(NUM).Abs(Value));

        /// <summary>
        /// Find the sign of x
        /// </summary>
        /// <param name="x">The value to find the sign of</param>
        /// <returns>-1, 0, or +1</returns>
        public virtual NUMTYPE Signum() =>
            New(default(NUM).Signum(Value));

        public virtual NUMTYPE Min(NUMTYPE rhs) =>
            this < rhs
                ? (NUMTYPE)this
                : rhs;

        public virtual NUMTYPE Max(NUMTYPE rhs) =>
            this > rhs
                ? (NUMTYPE)this
                : rhs;

        [Pure]
        public virtual int CompareTo(NUMTYPE other) =>
            default(NUM).Compare(Value, other.Value);

        [Pure]
        public virtual bool Equals(NUMTYPE other) =>
            default(NUM).Equals(Value, other.Value);

        [Pure]
        public virtual bool Equals(NUMTYPE other, NUMTYPE epsilon) =>
            (other - this).Abs() > epsilon;

        [Pure]
        public override bool Equals(object obj) =>
            !ReferenceEquals(obj, null) && obj is NUMTYPE && Equals((NUMTYPE)obj);

        [Pure]
        public override int GetHashCode() =>
            Value?.GetHashCode() ?? 0;

        [Pure]
        public static NUMTYPE operator -(NumType<NUMTYPE, NUM, A, PRED> x) =>
             New(default(NUM).Subtract(default(NUM).FromInteger(0), x.Value));

        [Pure]
        public static NUMTYPE operator +(NumType<NUMTYPE, NUM, A, PRED> lhs, NumType<NUMTYPE, NUM, A, PRED> rhs) =>
             New(default(NUM).Plus(lhs.Value, rhs.Value));

        [Pure]
        public static NUMTYPE operator -(NumType<NUMTYPE, NUM, A, PRED> lhs, NumType<NUMTYPE, NUM, A, PRED> rhs) =>
             New(default(NUM).Subtract(lhs.Value, rhs.Value));

        [Pure]
        public static NUMTYPE operator *(NumType<NUMTYPE, NUM, A, PRED> lhs, NumType<NUMTYPE, NUM, A, PRED> rhs) =>
             New(default(NUM).Product(lhs.Value, rhs.Value));

        [Pure]
        public static NUMTYPE operator /(NumType<NUMTYPE, NUM, A, PRED> lhs, NumType<NUMTYPE, NUM, A, PRED> rhs) =>
             New(default(NUM).Divide(lhs.Value, rhs.Value));

        [Pure]
        public static bool operator ==(NumType<NUMTYPE, NUM, A, PRED> lhs, NumType<NUMTYPE, NUM, A, PRED> rhs) =>
             default(NUM).Equals(lhs.Value, rhs.Value);

        [Pure]
        public static bool operator !=(NumType<NUMTYPE, NUM, A, PRED> lhs, NumType<NUMTYPE, NUM, A, PRED> rhs) =>
             !default(NUM).Equals(lhs.Value, rhs.Value);

        [Pure]
        public static bool operator >(NumType<NUMTYPE, NUM, A, PRED> lhs, NumType<NUMTYPE, NUM, A, PRED> rhs) =>
            default(NUM).Compare(lhs.Value, rhs.Value) > 0;

        [Pure]
        public static bool operator >=(NumType<NUMTYPE, NUM, A, PRED> lhs, NumType<NUMTYPE, NUM, A, PRED> rhs) =>
            default(NUM).Compare(lhs.Value, rhs.Value) >= 0;

        [Pure]
        public static bool operator <(NumType<NUMTYPE, NUM, A, PRED> lhs, NumType<NUMTYPE, NUM, A, PRED> rhs) =>
            default(NUM).Compare(lhs.Value, rhs.Value) < 0;

        [Pure]
        public static bool operator <=(NumType<NUMTYPE, NUM, A, PRED> lhs, NumType<NUMTYPE, NUM, A, PRED> rhs) =>
            default(NUM).Compare(lhs.Value, rhs.Value) <= 0;

        /// <summary>
        /// Monadic bind of the bound value to a new value of the same type
        /// </summary>
        /// <param name="bind">Bind function</param>
        [Pure]
        public virtual NUMTYPE Bind(Func<A, NUMTYPE> bind) =>
            bind(Value);

        /// <summary>
        /// Run a predicate for all values in the NumType (only ever one)
        /// </summary>
        [Pure]
        public virtual bool Exists(Func<A, bool> predicate) =>
            predicate(Value);

        /// <summary>
        /// Run a predicate for all values in the NumType (only ever one)
        /// </summary>
        [Pure]
        public virtual bool ForAll(Func<A, bool> predicate) =>
            predicate(Value);

        /// <summary>
        /// Map the bound value to a new value of the same type
        /// </summary>
        [Pure]
        public virtual NUMTYPE Map(Func<A, A> map) =>
            Select(map);

        /// <summary>
        /// Map the bound value to a new value of the same type
        /// </summary>
        [Pure]
        public virtual NUMTYPE Select(Func<A, A> map) =>
            New(map(Value));

        /// <summary>
        /// Monadic bind of the bound value to a new value of the same type
        /// </summary>
        /// <param name="bind">Bind function</param>
        /// <param name="project">Final projection (select)</param>
        [Pure]
        public virtual NUMTYPE SelectMany(
            Func<A, NumType<NUMTYPE, NUM, A, PRED>> bind,
            Func<A, A, A> project) =>
            New(project(Value, bind(Value).Value));

        /// <summary>
        /// Invoke an action that takes the bound value as an argument
        /// </summary>
        /// <param name="f">Action to invoke</param>
        public virtual Unit Iter(Action<A> f)
        {
            f(Value);
            return unit;
        }

        /// <summary>
        /// Generate a text representation of the NumType and value
        /// </summary>
        [Pure]
        public override string ToString() =>
            $"{GetType().Name}({Value})";

        /// <summary>
        /// Fold
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state and NumType bound value</returns>
        public virtual S Fold<S>(S state, Func<S, A, S> folder) =>
            folder(state, Value);

        /// <summary>
        /// Fold back
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state and NumType bound value</returns>
        public virtual S FoldBack<S>(S state, Func<S, A, S> folder) =>
            folder(state, Value);
        
        /// <summary>
        /// Sum
        /// </summary>
        public virtual A Sum() => Value;
    }
}