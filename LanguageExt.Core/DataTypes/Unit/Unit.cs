using System;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// A unit type is a type that allows only one value (and thus can hold no information)
    /// </summary>
    [Serializable]
    public readonly struct Unit : IEquatable<Unit>, IComparable<Unit>
    {
        public static readonly Unit Default = new Unit();

        [Pure]
        public override int GetHashCode() => 
            0;

        [Pure]
        public override bool Equals(object obj) =>
            obj is Unit;

        [Pure]
        public override string ToString() => 
            "()";

        [Pure]
        public bool Equals(Unit other) =>
            true;

        [Pure]
        public static bool operator ==(Unit lhs, Unit rhs) =>
            true;

        [Pure]
        public static bool operator !=(Unit lhs, Unit rhs) =>
            false;

        [Pure]
        public static bool operator >(Unit lhs, Unit rhs) =>
            false;

        [Pure]
        public static bool operator >=(Unit lhs, Unit rhs) =>
            true;

        [Pure]
        public static bool operator <(Unit lhs, Unit rhs) =>
            false;

        [Pure]
        public static bool operator <=(Unit lhs, Unit rhs) =>
            true;

        /// <summary>
        /// Provide an alternative value to unit
        /// </summary>
        /// <typeparam name="T">Alternative value type</typeparam>
        /// <param name="anything">Alternative value</param>
        /// <returns>Alternative value</returns>
        [Pure]
        public T Return<T>(T anything) => anything;

        /// <summary>
        /// Provide an alternative value to unit
        /// </summary>
        /// <typeparam name="T">Alternative value type</typeparam>
        /// <param name="anything">Alternative value</param>
        /// <returns>Alternative value</returns>
        [Pure]
        public T Return<T>(Func<T> anything) => anything();

        /// <summary>
        /// Always equal
        /// </summary>
        [Pure]
        public int CompareTo(Unit other) =>
            0;

        [Pure]
        public static Unit operator +(Unit a, Unit b) =>
            Default;

        [Pure]
        public static implicit operator ValueTuple(Unit _) => 
            default;
        
        [Pure]
        public static implicit operator Unit(ValueTuple _) => 
            default;
    }
}
