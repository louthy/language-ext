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
    public abstract class Record<RECORDTYPE> : 
        IEquatable<RECORDTYPE>, IComparable<RECORDTYPE>, IComparable
        where RECORDTYPE : Record<RECORDTYPE>
    {
        protected Record() { }

        protected Record(SerializationInfo info, StreamingContext context) =>
            RecordType<RECORDTYPE>.SetObjectData((RECORDTYPE)this, info);

        public static bool operator==(Record<RECORDTYPE> x, Record<RECORDTYPE> y) =>
            RecordType<RECORDTYPE>.EqualityTyped((RECORDTYPE)x, (RECORDTYPE)y);

        public static bool operator !=(Record<RECORDTYPE> x, Record<RECORDTYPE> y) =>
            !(x == y);

        public static bool operator >(Record<RECORDTYPE> x, Record<RECORDTYPE> y) =>
            RecordType<RECORDTYPE>.Compare((RECORDTYPE)x, (RECORDTYPE)y) > 0;

        public static bool operator >=(Record<RECORDTYPE> x, Record<RECORDTYPE> y) =>
            RecordType<RECORDTYPE>.Compare((RECORDTYPE)x, (RECORDTYPE)y) >= 0;

        public static bool operator <(Record<RECORDTYPE> x, Record<RECORDTYPE> y) =>
            RecordType<RECORDTYPE>.Compare((RECORDTYPE)x, (RECORDTYPE)y) < 0;

        public static bool operator <=(Record<RECORDTYPE> x, Record<RECORDTYPE> y) =>
            RecordType<RECORDTYPE>.Compare((RECORDTYPE)x, (RECORDTYPE)y) <= 0;

        public override int GetHashCode() =>
            RecordType<RECORDTYPE>.Hash((RECORDTYPE)this);

        public override bool Equals(object obj) =>
            RecordType<RECORDTYPE>.Equality((RECORDTYPE)this, obj);

        public virtual int CompareTo(RECORDTYPE other) =>
            RecordType<RECORDTYPE>.Compare((RECORDTYPE)this, other);

        public virtual bool Equals(RECORDTYPE other) =>
            RecordType<RECORDTYPE>.EqualityTyped((RECORDTYPE)this, other);

        public override string ToString() =>
            RecordType<RECORDTYPE>.ToString((RECORDTYPE)this);

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context) =>
            RecordType<RECORDTYPE>.GetObjectData((RECORDTYPE)this, info);

        public int CompareTo(object obj) =>
            obj is RECORDTYPE rt ? CompareTo(rt) : 1;
    }
}
