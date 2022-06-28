using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.Contracts;
using LanguageExt.ClassInstances;
using LanguageExt.Common;

namespace LanguageExt
{
    /// <summary>
    /// Identity monad
    /// </summary>
    /// <remarks>
    /// Simply carries the bound value through its bind expressions without imparting any additional behaviours.  It can
    /// be constructed using:
    ///
    ///     Identity<int> ma = Id(123);
    /// 
    /// </remarks>
    /// <typeparam name="A">Bound value type</typeparam>
    public readonly struct Identity<A> : 
        IEquatable<Identity<A>>, 
        IComparable<Identity<A>>, 
        IComparable
    {
        public static readonly Identity<A> Bottom = default(Identity<A>);

        public readonly A value;
        public readonly bool IsBottom;

        public Identity(A value)
        {
            this.value = value;
            IsBottom = false;
        }

        [Pure]
        public A Value
        {
            get
            {
                if (IsBottom) throw new BottomException();
                return value;
            }
        }
        
        public static bool operator ==(Identity<A> lhs, Identity<A> rhs) =>
            lhs.Equals(rhs);

        public static bool operator !=(Identity<A> lhs, Identity<A> rhs) =>
            !(lhs == rhs);

        public static bool operator >(Identity<A> lhs, Identity<A> rhs) =>
            lhs.CompareTo(rhs) > 0;

        public static bool operator >=(Identity<A> lhs, Identity<A> rhs) =>
            lhs.CompareTo(rhs) >= 0;

        public static bool operator <(Identity<A> lhs, Identity<A> rhs) =>
            lhs.CompareTo(rhs) < 0;

        public static bool operator <=(Identity<A> lhs, Identity<A> rhs) =>
            lhs.CompareTo(rhs) <= 0;

        [Pure]
        public bool Equals(Identity<A> other) =>
            default(EqDefault<A>).Equals(value, other.value) && IsBottom == other.IsBottom;

        [Pure]
        public override bool Equals(object obj) =>
            obj is Identity<A> other && Equals(other);

        [Pure]
        public override int GetHashCode() =>
            default(HashableDefault<A>).GetHashCode(value);

        [Pure]
        public int CompareTo(object obj) =>
            obj is Identity<A> t ? CompareTo(t) : 1;

        [Pure]
        public int CompareTo(Identity<A> other) =>
            default(OrdDefault<A>).Compare(value, other.value);

        [Pure]
        public Identity<B> Map<B>(Func<A, B> f) =>
            new Identity<B>(f(Value));

        [Pure]
        public Identity<B> Select<B>(Func<A, B> f) =>
            new Identity<B>(f(Value));

        [Pure]
        public Identity<B> Bind<B>(Func<A, Identity<B>> f) =>
            f(Value);

        [Pure]
        public Identity<B> SelectMany<B>(Func<A, Identity<B>> f) =>
            f(Value);

        [Pure]
        public Identity<C> SelectMany<B, C>(Func<A, Identity<B>> bind, Func<A, B, C> project)
        {
            var a = Value;
            return bind(a).Map(b => project(a, b));
        }
    }
}
