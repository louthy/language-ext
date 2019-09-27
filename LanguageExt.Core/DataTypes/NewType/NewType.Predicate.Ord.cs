using System;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using System.Runtime.Serialization;

namespace LanguageExt
{
    /// <summary>
    /// NewType - inspired by Haskell's 'newtype' keyword.
    /// https://wiki.haskell.org/Newtype
    /// Derive type from this one to get: Equatable, Comparable, Appendable, Subtractable, 
    /// Multiplicable, Divisible, Foldable, Monadic, Functor, Interable: strongly typed values.  
    ///
    /// For example:
    ///
    ///     class Metres : NewType<Metres, double> { public Metres(double x) : base(x) {} }
    ///     class Hours : NewType<Hours, double> { public Hours(double x) : base(x) {} }
    ///
    /// Will not accept null values
    ///
    /// </summary>
    /// <typeparam name="NEWTYPE">Implementing class</typeparam>
    /// <typeparam name="A">Inner bound value type</typeparam>
    /// <typeparam name="PRED">Predicate instance to run when the type is constructed</typeparam>
    /// <typeparam name="ORD">`Ord<A>` class instance</typeparam>
    [Serializable]
    public abstract class NewType<NEWTYPE, A, PRED, ORD> :
        IEquatable<NEWTYPE>,
        IComparable<NEWTYPE>
        where PRED    : struct, Pred<A>
        where NEWTYPE : NewType<NEWTYPE, A, PRED, ORD>
        where ORD     : struct, Ord<A>
    {
        public readonly A Value;

        /// <summary>
        /// Constructor function
        /// </summary>
        public static readonly Func<A, NEWTYPE> New = IL.Ctor<A, NEWTYPE>();

        /// <summary>
        /// Try new
        /// </summary>
        public static Try<NEWTYPE> NewTry(A value) =>
            Try(() => New(value));

        /// <summary>
        /// Optional new
        /// </summary>
        public static Option<NEWTYPE> NewOption(A value) =>
            NewTry(value).ToOption();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">Value to bind</param>
        /// <exception cref="ArgumentOutOfRangeException">If argument fails to pass the predicate provided in the generic argument PRED</exception>
        /// <exception cref="ArgumentNullException">Null values are not accepted</exception>
        public NewType(A value)
        {
            if (!default(PRED).True(value)) throw new ArgumentOutOfRangeException(nameof(value));
            if (isnull(value)) throw new ArgumentNullException(nameof(value));
            Value = value;
        }

        /// <summary>
        /// Deserialisation ctor
        /// </summary>
        protected NewType(SerializationInfo info, StreamingContext context)
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
        public static explicit operator A(NewType<NEWTYPE, A, PRED, ORD> type) =>
            ReferenceEquals(type, null)
                ? throw new ArgumentException($"Can't explictly convert from a null {typeof(NEWTYPE).Name}")
                : type.Value;

        [Pure]
        public virtual int CompareTo(NEWTYPE other) =>
             default(OrdNewType<NEWTYPE, ORD, A, PRED>).Compare(this, other);

        [Pure]
        public virtual bool Equals(NEWTYPE other) =>
            default(OrdNewType<NEWTYPE, ORD, A, PRED>).Equals(this, other);

        [Pure]
        public override bool Equals(object obj) =>
            !ReferenceEquals(obj, null) && obj is NEWTYPE b && Equals(b);

        [Pure]
        public override int GetHashCode() =>
            default(OrdNewType<NEWTYPE, ORD, A, PRED>).GetHashCode(this);

        [Pure]
        public static bool operator ==(NewType<NEWTYPE, A, PRED, ORD> lhs, NewType<NEWTYPE, A, PRED, ORD> rhs) =>
            default(OrdNewType<NEWTYPE, ORD, A, PRED>).Equals(lhs, rhs);

        [Pure]
        public static bool operator !=(NewType<NEWTYPE, A, PRED, ORD> lhs, NewType<NEWTYPE, A, PRED, ORD> rhs) =>
            !default(OrdNewType<NEWTYPE, ORD, A, PRED>).Equals(lhs, rhs);

        [Pure]
        public static bool operator >(NewType<NEWTYPE, A, PRED, ORD> lhs, NewType<NEWTYPE, A, PRED, ORD> rhs) =>
            default(OrdNewType<NEWTYPE, ORD, A, PRED>).Compare(lhs, rhs) > 0;

        [Pure]
        public static bool operator >=(NewType<NEWTYPE, A, PRED, ORD> lhs, NewType<NEWTYPE, A, PRED, ORD> rhs) =>
            default(OrdNewType<NEWTYPE, ORD, A, PRED>).Compare(lhs, rhs) >= 0;

        [Pure]
        public static bool operator <(NewType<NEWTYPE, A, PRED, ORD> lhs, NewType<NEWTYPE, A, PRED, ORD> rhs) =>
            default(OrdNewType<NEWTYPE, ORD, A, PRED>).Compare(lhs, rhs) < 0;

        [Pure]
        public static bool operator <=(NewType<NEWTYPE, A, PRED, ORD> lhs, NewType<NEWTYPE, A, PRED, ORD> rhs) =>
            default(OrdNewType<NEWTYPE, ORD, A, PRED>).Compare(lhs, rhs) <= 0;

        /// <summary>
        /// Monadic bind of the bound value to a new value of the same type
        /// </summary>
        /// <param name="bind">Bind function</param>
        [Pure]
        public virtual NEWTYPE Bind(Func<A, NEWTYPE> bind) =>
            bind(Value);

        /// <summary>
        /// Run a predicate for all values in the NewType (only ever one)
        /// </summary>
        [Pure]
        public virtual bool Exists(Func<A, bool> predicate) =>
            predicate(Value);

        /// <summary>
        /// Run a predicate for all values in the NewType (only ever one)
        /// </summary>
        [Pure]
        public virtual bool ForAll(Func<A, bool> predicate) =>
            predicate(Value);

        /// <summary>
        /// Map the bound value to a new value of the same type
        /// </summary>
        [Pure]
        public virtual NEWTYPE Map(Func<A, A> map) =>
            Select(map);

        /// <summary>
        /// Map the bound value to a new value of the same type
        /// </summary>
        [Pure]
        public virtual NEWTYPE Select(Func<A, A> map) =>
            New(map(Value));

        /// <summary>
        /// Monadic bind of the bound value to a new value of the same type
        /// </summary>
        /// <param name="bind">Bind function</param>
        /// <param name="project">Final projection (select)</param>
        [Pure]
        public virtual NEWTYPE SelectMany(
            Func<A, NewType<NEWTYPE, A, PRED, ORD>> bind,
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
        /// Generate a text representation of the NewType and value
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
        /// <returns>Folded state and NewType bound value</returns>
        public virtual S Fold<S>(S state, Func<S, A, S> folder) =>
            folder(state, Value);

        /// <summary>
        /// Fold back
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="folder">Fold function</param>
        /// <returns>Folded state and NewType bound value</returns>
        public virtual S FoldBack<S>(S state, Func<S, A, S> folder) =>
            folder(state, Value);
    }
}
