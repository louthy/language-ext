using System;
using System.Linq;
using LanguageExt;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using System.Runtime.Serialization;

namespace LanguageExt
{
    /// <summary>
    /// FloatType - inspired by Haskell's 'newtype' keyword.  This is setup for floating point
    /// numeric types,and expects a Floating<A> class-instance as an argument (TFloat, TDouble, 
    /// TDecimal, etc.)
    /// 
    /// https://wiki.haskell.org/Newtype
    /// 
    /// Derive type from this one to get: Equatable, Comparable, Appendable, Foldable, 
    /// Functor, Iterable: strongly typed values.
    ///
    /// For example:
    ///
    ///     class Metres : FloatType<Metres, TDouble, double> { public Metres(double x) : base(x) {} }
    ///
    /// Will not accept null values
    /// </summary>
    /// <typeparam name="SELF">Self reference type - i.e. class Metres : FloatType<Metres, ... ></typeparam>
    /// <typeparam name="FLOATING">Num of A, e.g. TInt, TDouble, TFloat, etc.</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <typeparam name="PRED">A predicate to be applied to the value passed into the constructor.
    /// Allows for constraints on the value stored.</typeparam>
    [Serializable]
    public abstract class FloatType<SELF, FLOATING, A, PRED> :
        IEquatable<SELF>,
        IComparable<SELF>,
        IComparable
        where FLOATING : struct, Floating<A>
        where PRED : struct, Pred<A>
        where SELF : FloatType<SELF, FLOATING, A, PRED>
    {
        public readonly A Value;

        /// <summary>
        /// Constructor function
        /// </summary>
        public static readonly Func<A, SELF> New = IL.Ctor<A, SELF>();

        /// <summary>
        /// Try new
        /// </summary>
        public static Try<SELF> NewTry(A value) =>
            Try(() => New(value));

        /// <summary>
        /// Optional new
        /// </summary>
        public static Option<SELF> NewOption(A value) =>
            NewTry(value).ToOption();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Value to bind</param>
        /// <exception cref="ArgumentOutOfRangeException">If argument fails to pass the predicate provided in the generic argument PRED</exception>
        /// <exception cref="ArgumentNullException">Null values are not accepted</exception>
        public FloatType(A value)
        {
            if(!default(PRED).True(value)) throw new ArgumentOutOfRangeException(nameof(value));
            if (isnull(value)) throw new ArgumentNullException(nameof(value));
            Value = value;
        }

        /// <summary>
        /// Deserialisation ctor
        /// </summary>
        protected FloatType(SerializationInfo info, StreamingContext context)
        {
            Value = (A)info.GetValue("Value", typeof(A));
            if (!default(PRED).True(Value)) throw new ArgumentOutOfRangeException(nameof(Value));
            if (isnull(Value)) throw new ArgumentNullException(nameof(Value));
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context) =>
            info.AddValue("Value", Value);

        /// <summary>
        /// Explicit conversion operator for extracting the bound value
        /// </summary>
        /// <param name="type"></param>
        [Pure]
        public static explicit operator A(FloatType<SELF, FLOATING, A, PRED> type) =>
            ValueOrDefault(type);

        [Pure]
        static A ValueOrDefault(FloatType<SELF, FLOATING, A, PRED> floatType) =>
            ReferenceEquals(floatType, null)
                ? default(FLOATING).Empty()
                : floatType.Value;

        /// <summary>
        /// Sum of FloatType(x) and FloatType(y)
        /// </summary>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs + rhs</returns>
        [Pure]
        public virtual SELF Plus(SELF rhs) =>
            from x in this
            from y in rhs
            select plus<FLOATING, A>(x, y);

        /// <summary>
        /// Subtract of NewType(x) and NewType(y)
        /// </summary>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs - rhs</returns>
        [Pure]
        public virtual SELF Subtract(SELF rhs) =>
            from x in this
            from y in rhs
            select subtract<FLOATING, A>(x, y);

        /// <summary>
        /// Divide NewType(x) and NewType(y)
        /// </summary>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs / rhs</returns>
        [Pure]
        public virtual SELF Divide(SELF rhs) =>
            from x in this
            from y in rhs
            select subtract<FLOATING, A>(x, y);

        /// <summary>
        /// Multiply NewType(x) and NewType(y)
        /// </summary>
        /// <param name="rhs">Right-hand side of the operation</param>
        /// <returns>lhs * rhs</returns>
        [Pure]
        public virtual SELF Product(SELF rhs) =>
            from x in this
            from y in rhs
            select subtract<FLOATING, A>(x, y);

        /// <summary>
        /// Find the absolute value of a number
        /// </summary>
        /// <param name="x">The value to find the absolute value of</param>
        /// <returns>The non-negative absolute value of x</returns>
        public virtual SELF Abs() =>
            New(default(FLOATING).Abs(Value));

        /// <summary>
        /// Find the sign of x
        /// </summary>
        /// <param name="x">The value to find the sign of</param>
        /// <returns>-1, 0, or +1</returns>
        public virtual SELF Signum() =>
            New(default(FLOATING).Signum(Value));

        /// <summary>
        /// Find the minimum value between this and rhs
        /// </summary>
        public virtual SELF Min(SELF rhs) =>
            this < rhs
                ? (SELF)this
                : rhs;

        /// <summary>
        /// Find the maximum value between this and rhs
        /// </summary>
        public virtual SELF Max(SELF rhs) =>
            this > rhs
                ? (SELF)this
                : rhs;

        /// <summary>
        /// The exponential function.
        /// </summary>
        /// <param name="x">The value for which we are calculating the exponential</param>
        /// <returns>The value of <c>e^x</c></returns>
        [Pure]
        public SELF Exp() =>
            New(default(FLOATING).Exp(Value));

        /// <summary>
        /// Calculates the square root of a value.
        /// </summary>
        /// <param name="x">The value for which we are calculating the square root.</param>
        /// <returns>The value of <c>sqrt(x)</c>.</returns>
        [Pure]
        public SELF Sqrt() =>
            New(default(FLOATING).Sqrt(Value));

        /// <summary>
        /// Calculates the natural logarithm of a value.
        /// </summary>
        /// <param name="x">
        /// The value for which we are calculating the natural logarithm.
        /// </param>
        /// <returns>The value of <c>ln(x)</c>.</returns>
        [Pure]
        public SELF Log() =>
            New(default(FLOATING).Log(Value));

        /// <summary>Raises x to the power y
        /// </summary>
        /// <param name="x">The base to be raised to y</param>
        /// <param name="exp">The exponent to which we are raising x</param>
        /// <returns>The value of <c>x^y</c>.</returns>
        [Pure]
        public SELF Pow(A exp) =>
            New(default(FLOATING).Pow(Value, exp));

        /// <summary>
        /// Calculates the logarithm of a value with respect to an arbitrary base.
        /// </summary>
        /// <param name="x">The base to use for the logarithm of t</param>
        /// <param name="y">The value for which we are calculating the logarithm.</param>
        /// <returns>The value of <c>log x (y)</c>.</returns>
        [Pure]
        public SELF LogBase(A y) =>
            New(default(FLOATING).LogBase(Value, y));

        /// <summary>
        /// Calculates the sine of an angle.
        /// </summary>
        /// <param name="x">An angle, in radians</param>
        /// <returns>The value of <c>sin(x)</c></returns>
        [Pure]
        public SELF Sin() =>
            New(default(FLOATING).Sin(Value));

        /// <summary>
        /// Calculates the cosine of an angle.
        /// </summary>
        /// <param name="x">An angle, in radians</param>
        /// <returns>The value of <c>cos(x)</c></returns>
        [Pure]
        public SELF Cos() =>
            New(default(FLOATING).Tan(Value));

        /// <summary>
        ///     Calculates the tangent of an angle.
        /// </summary>
        /// <param name="x">An angle, in radians</param>
        /// <returns>The value of <c>tan(x)</c></returns>
        [Pure]
        public SELF Tan() =>
            New(default(FLOATING).Tan(Value));

        /// <summary>
        /// Calculates an arcsine.
        /// </summary>
        /// <param name="x">The value for which an arcsine is to be calculated.</param>
        /// <returns>The value of <c>asin(x)</c>, in radians.</returns>
        [Pure]
        public SELF Asin() =>
            New(default(FLOATING).Asin(Value));

        /// <summary>
        /// Calculates an arc-cosine.
        /// </summary>
        /// <param name="x">The value for which an arc-cosine is to be calculated</param>
        /// <returns>The value of <c>acos(x)</c>, in radians</returns>
        [Pure]
        public SELF Acos() =>
            New(default(FLOATING).Acos(Value));

        /// <summary>
        /// Calculates an arc-tangent.
        /// </summary>
        /// <param name="x">The value for which an arc-tangent is to be calculated</param>
        /// <returns>The value of <c>atan(x)</c>, in radians</returns>
        [Pure]
        public SELF Atan() =>
            New(default(FLOATING).Atan(Value));

        /// <summary>
        /// Calculates a hyperbolic sine.
        /// </summary>
        /// <param name="x">The value for which a hyperbolic sine is to be calculated</param>
        /// <returns>The value of <c>sinh(x)</c></returns>
        [Pure]
        public SELF Sinh() =>
            New(default(FLOATING).Sinh(Value));

        /// <summary>
        /// Calculates a hyperbolic cosine.
        /// </summary>
        /// <param name="x">The value for which a hyperbolic cosine is to be calculated</param>
        /// <returns>The value of <c>cosh(x)</c></returns>
        [Pure]
        public SELF Cosh() =>
            New(default(FLOATING).Cosh(Value));

        /// <summary>
        /// Calculates a hyperbolic tangent.
        /// </summary>
        /// <param name="x">
        /// The value for which a hyperbolic tangent is to be calculated.
        /// </param>
        /// <returns>The value of <c>tanh(x)</c></returns>
        [Pure]
        public SELF Tanh() =>
            New(default(FLOATING).Tanh(Value));

        /// <summary>Calculates an area hyperbolic sine</summary>
        /// <param name="x">The value for which an area hyperbolic sine is to be calculated.
        /// </param>
        /// <returns>The value of <c>asinh(x)</c>.</returns>
        [Pure]
        public SELF Asinh() =>
            New(default(FLOATING).Asinh(Value));

        /// <summary>
        /// Calculates an area hyperbolic cosine.
        /// </summary>
        /// <param name="x">The value for which an area hyperbolic cosine is to be calculated.
        /// </param>
        /// <returns>The value of <c>acosh(x)</c>.</returns>
        [Pure]
        public SELF Acosh() =>
            New(default(FLOATING).Acosh(Value));

        /// <summary>
        /// Calculates an area hyperbolic tangent.
        /// </summary>
        /// <param name="x">The value for which an area hyperbolic tangent is to be calculated.
        /// </param>
        /// <returns>The value of <c>atanh(x)</c></returns>
        [Pure]
        public SELF Atanh() =>
            New(default(FLOATING).Atanh(Value));

        /// <summary>
        /// Compare this to other
        /// </summary>
        [Pure]
        public virtual int CompareTo(SELF other) =>
            ReferenceEquals(other, null)
                ? 1
                : default(FLOATING).Compare(Value, other.Value);

        /// <summary>
        /// Equality test between this and other
        /// </summary>
        [Pure]
        public virtual bool Equals(SELF other) =>
            !ReferenceEquals(other, null) && default(FLOATING).Equals(Value, other.Value);

        /// <summary>
        /// Equality test between this and other
        /// </summary>
        [Pure]
        public virtual bool Equals(SELF other, SELF epsilon) =>
            (other - this).Abs() > epsilon;

        /// <summary>
        /// Equality test between this and other
        /// </summary>
        [Pure]
        public override bool Equals(object obj) =>
            !ReferenceEquals(obj, null) && obj is SELF && Equals((SELF)obj);

        /// <summary>
        /// Get the hash-code of the bound value
        /// </summary>
        /// <returns></returns>
        [Pure]
        public override int GetHashCode() =>
            Value?.GetHashCode() ?? 0;

        [Pure]
        public int CompareTo(object obj) =>
            obj is SELF t ? CompareTo(t) : 1;

        [Pure]
        public static SELF operator -(FloatType<SELF, FLOATING, A, PRED> x) =>
             New(default(FLOATING).Subtract(default(FLOATING).FromInteger(0), ValueOrDefault(x)));

        [Pure]
        public static SELF operator +(FloatType<SELF, FLOATING, A, PRED> lhs, FloatType<SELF, FLOATING, A, PRED> rhs) =>
             New(default(FLOATING).Plus(ValueOrDefault(lhs), ValueOrDefault(rhs)));

        [Pure]
        public static SELF operator -(FloatType<SELF, FLOATING, A, PRED> lhs, FloatType<SELF, FLOATING, A, PRED> rhs) =>
             New(default(FLOATING).Subtract(ValueOrDefault(lhs), ValueOrDefault(rhs)));

        [Pure]
        public static SELF operator *(FloatType<SELF, FLOATING, A, PRED> lhs, FloatType<SELF, FLOATING, A, PRED> rhs) =>
             New(default(FLOATING).Product(ValueOrDefault(lhs), ValueOrDefault(rhs)));

        [Pure]
        public static SELF operator /(FloatType<SELF, FLOATING, A, PRED> lhs, FloatType<SELF, FLOATING, A, PRED> rhs) =>
             New(default(FLOATING).Divide(ValueOrDefault(lhs), ValueOrDefault(rhs)));

        [Pure]
        public static bool operator ==(FloatType<SELF, FLOATING, A, PRED> lhs, FloatType<SELF, FLOATING, A, PRED> rhs) =>
             default(FLOATING).Equals(ValueOrDefault(lhs), ValueOrDefault(rhs));

        [Pure]
        public static bool operator !=(FloatType<SELF, FLOATING, A, PRED> lhs, FloatType<SELF, FLOATING, A, PRED> rhs) =>
             !default(FLOATING).Equals(ValueOrDefault(lhs), ValueOrDefault(rhs));

        [Pure]
        public static bool operator >(FloatType<SELF, FLOATING, A, PRED> lhs, FloatType<SELF, FLOATING, A, PRED> rhs) =>
            default(FLOATING).Compare(ValueOrDefault(lhs), ValueOrDefault(rhs)) > 0;

        [Pure]
        public static bool operator >=(FloatType<SELF, FLOATING, A, PRED> lhs, FloatType<SELF, FLOATING, A, PRED> rhs) =>
            default(FLOATING).Compare(ValueOrDefault(lhs), ValueOrDefault(rhs)) >= 0;

        [Pure]
        public static bool operator <(FloatType<SELF, FLOATING, A, PRED> lhs, FloatType<SELF, FLOATING, A, PRED> rhs) =>
            default(FLOATING).Compare(ValueOrDefault(lhs), ValueOrDefault(rhs)) < 0;

        [Pure]
        public static bool operator <=(FloatType<SELF, FLOATING, A, PRED> lhs, FloatType<SELF, FLOATING, A, PRED> rhs) =>
            default(FLOATING).Compare(ValueOrDefault(lhs), ValueOrDefault(rhs)) <= 0;

        /// <summary>
        /// Monadic bind of the bound value to a new value of the same type
        /// </summary>
        /// <param name="bind">Bind function</param>
        [Pure]
        public virtual SELF Bind(Func<A, SELF> bind) =>
            bind(Value);

        /// <summary>
        /// Run a predicate for all values in the FloatType (only ever one)
        /// </summary>
        [Pure]
        public virtual bool Exists(Func<A, bool> predicate) =>
            predicate(Value);

        /// <summary>
        /// Run a predicate for all values in the FloatType (only ever one)
        /// </summary>
        [Pure]
        public virtual bool ForAll(Func<A, bool> predicate) =>
            predicate(Value);

        /// <summary>
        /// Map the bound value to a new value of the same type
        /// </summary>
        [Pure]
        public virtual SELF Map(Func<A, A> map) =>
            Select(map);

        /// <summary>
        /// Map the bound value to a new value of the same type
        /// </summary>
        [Pure]
        public virtual SELF Select(Func<A, A> map) =>
            New(map(Value));

        /// <summary>
        /// Monadic bind of the bound value to a new value of the same type
        /// </summary>
        /// <param name="bind">Bind function</param>
        /// <param name="project">Final projection (select)</param>
        [Pure]
        public virtual SELF SelectMany(
            Func<A, FloatType<SELF, FLOATING, A, PRED>> bind,
            Func<A, A, A> project) =>
            New(project(Value, ValueOrDefault(bind(Value))));

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
        /// Generate a text representation of the FloatType and value
        /// </summary>
        [Pure]
        public override string ToString() =>
            $"{Value}";

        /// <summary>
        /// Fold
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state and FloatType bound value</returns>
        public virtual S Fold<S>(S state, Func<S, A, S> folder) =>
            folder(state, Value);

        /// <summary>
        /// Fold back
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state and FloatType bound value</returns>
        public virtual S FoldBack<S>(S state, Func<S, A, S> folder) =>
            folder(state, Value);

        [Pure]
        public static SELF FromInteger(int value) =>
            New(default(FLOATING).FromInteger(value));
    }
}
