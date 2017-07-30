using System;

namespace LanguageExt
{
    /// <summary>
    /// Base class for types that are 'records'.  A record has a set of readonly *fields(
    /// that make up its data structure.  By deriving from this you get structural equality,
    /// structural ordering (`IComparable`), structural hashing (`GetHashCode`) as well as the
    /// operators `==`, `!=`, `<`, `<=`, `>`, `>=`, 
    /// </summary>
    /// <typeparam name="RECORDTYPE"></typeparam>
    public abstract class Record<RECORDTYPE> : IEquatable<RECORDTYPE>, IComparable<RECORDTYPE>
    {
        public static bool operator==(Record<RECORDTYPE> x, Record<RECORDTYPE> y) =>
            RecordType<RECORDTYPE>.Equality((RECORDTYPE)(object)x, (RECORDTYPE)(object)y);

        public static bool operator !=(Record<RECORDTYPE> x, Record<RECORDTYPE> y) =>
            !(x == y);

        public static bool operator >(Record<RECORDTYPE> x, Record<RECORDTYPE> y) =>
            RecordType<RECORDTYPE>.Compare((RECORDTYPE)(object)x, (RECORDTYPE)(object)y) > 0;

        public static bool operator >=(Record<RECORDTYPE> x, Record<RECORDTYPE> y) =>
            RecordType<RECORDTYPE>.Compare((RECORDTYPE)(object)x, (RECORDTYPE)(object)y) >= 0;

        public static bool operator <(Record<RECORDTYPE> x, Record<RECORDTYPE> y) =>
            RecordType<RECORDTYPE>.Compare((RECORDTYPE)(object)x, (RECORDTYPE)(object)y) < 0;

        public static bool operator <=(Record<RECORDTYPE> x, Record<RECORDTYPE> y) =>
            RecordType<RECORDTYPE>.Compare((RECORDTYPE)(object)x, (RECORDTYPE)(object)y) <= 0;

        public override int GetHashCode() =>
            RecordType<RECORDTYPE>.Hash((RECORDTYPE)(object)this);

        public override bool Equals(object obj) =>
            RecordType<RECORDTYPE>.Equality((RECORDTYPE)(object)this, obj);

        public int CompareTo(RECORDTYPE other) =>
            RecordType<RECORDTYPE>.Compare((RECORDTYPE)(object)this, other);

        public bool Equals(RECORDTYPE other) =>
            RecordType<RECORDTYPE>.EqualityTyped((RECORDTYPE)(object)this, other);
    }
}
