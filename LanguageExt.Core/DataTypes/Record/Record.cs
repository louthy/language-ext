using System;
using System.Runtime.Serialization;

namespace LanguageExt
{
    /// <summary>
    /// Base class for types that are 'records'.  A record has a set of readonly *fields(
    /// that make up its data structure.  By deriving from this you get structural equality,
    /// structural ordering (`IComparable`), structural hashing (`GetHashCode`) as well as the
    /// operators `==`, `!=`, `<`, `<=`, `>`, `>=`, 
    /// </summary>
    /// <typeparam name="RECORDTYPE"></typeparam>
    [Serializable]
    public abstract class Record<RECORDTYPE> : IEquatable<RECORDTYPE>, IComparable<RECORDTYPE>
    {
        protected Record() { }

        protected Record(SerializationInfo info, StreamingContext context) =>
            RecordType<RECORDTYPE>.SetObjectData((RECORDTYPE)(object)this, info);

        public static bool operator==(Record<RECORDTYPE> x, Record<RECORDTYPE> y) =>
            RecordType<RECORDTYPE>.EqualityTyped((RECORDTYPE)(object)x, (RECORDTYPE)(object)y);

        public static bool operator !=(Record<RECORDTYPE> x, Record<RECORDTYPE> y) =>
            !(x == y);

        public static bool operator >(Record<RECORDTYPE> x, Record<RECORDTYPE> y) =>
            x?.CompareTo((RECORDTYPE)(object)y) > 0;

        public static bool operator >=(Record<RECORDTYPE> x, Record<RECORDTYPE> y) =>
            x?.CompareTo((RECORDTYPE)(object)y) >= 0;

        public static bool operator <(Record<RECORDTYPE> x, Record<RECORDTYPE> y) =>
            x?.CompareTo((RECORDTYPE)(object)y) < 0;

        public static bool operator <=(Record<RECORDTYPE> x, Record<RECORDTYPE> y) =>
            x?.CompareTo((RECORDTYPE)(object)y) <= 0;

        public override int GetHashCode() =>
            RecordType<RECORDTYPE>.Hash((RECORDTYPE)(object)this);

        public override bool Equals(object obj) =>
            RecordType<RECORDTYPE>.Equality((RECORDTYPE)(object)this, obj);

        public virtual int CompareTo(RECORDTYPE other) =>
            RecordType<RECORDTYPE>.Compare((RECORDTYPE)(object)this, other);

        public virtual bool Equals(RECORDTYPE other) =>
            RecordType<RECORDTYPE>.EqualityTyped((RECORDTYPE)(object)this, other);

        public override string ToString() =>
            RecordType<RECORDTYPE>.ToString((RECORDTYPE)(object)this);

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context) =>
            RecordType<RECORDTYPE>.GetObjectData((RECORDTYPE)(object)this, info);
    }
}
