using System;

namespace LanguageExt
{
    /// <summary>
    /// A unit type is a type that allows only one value (and thus can hold no information)
    /// </summary>
#if !COREFX
    [Serializable]
#endif
    public struct Unit : IEquatable<Unit>
    {
        public static readonly Unit Default = new Unit();

        public override int GetHashCode() => 
            0;

        public override bool Equals(object obj) =>
            obj is Unit;

        public override string ToString() => 
            "()";

        public bool Equals(Unit other) =>
            true;

        public static bool operator ==(Unit lhs, Unit rhs) =>
            true;

        public static bool operator !=(Unit lhs, Unit rhs) =>
            false;

        /// <summary>
        /// Provide an alternative value to unit
        /// </summary>
        /// <typeparam name="T">Alternative value type</typeparam>
        /// <param name="anything">Alternative value</param>
        /// <returns>Alternative value</returns>
        public T Return<T>(T anything) => anything;

        /// <summary>
        /// Provide an alternative value to unit
        /// </summary>
        /// <typeparam name="T">Alternative value type</typeparam>
        /// <param name="anything">Alternative value</param>
        /// <returns>Alternative value</returns>
        public T Return<T>(Func<T> anything) => anything();
    }
}
