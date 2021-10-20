using System;
using LanguageExt.ClassInstances;

namespace LanguageExt
{
    /// <summary>
    /// A proxy for `Ref`, returned by `commute`.  This allows the transaction system to know that the
    /// result is a commutative and therefore give you a result based on the live state rather than
    /// the transaction.  
    /// </summary>
    /// <remarks>
    /// See the [concurrency section](https://github.com/louthy/language-ext/wiki/Concurrency) of the wiki for more info.
    /// </remarks>
    public readonly struct CommuteRef<A>
    {
        internal CommuteRef(Ref<A> r) => Ref = r;
        internal readonly Ref<A> Ref;
        public A Value
        {
            get => Ref.Value;
            set => Ref.Value = value;
        }
        public static implicit operator A(CommuteRef<A> r) => r.Value;
        public override string ToString() => Value?.ToString() ?? "[null]";
        public override int GetHashCode() => Value?.GetHashCode() ?? 0;
        public override bool Equals(object obj) => obj is A val && Equals(val);
        public bool Equals(A other) => default(EqDefault<A>).Equals(other, Value);
        public A Swap(Func<A, A> f) => Ref.Swap(f);
        public A Swap<X>(X x, Func<X, A, A> f) => Ref.Swap(x, f);
        public A Swap<X, Y>(X x, Y y, Func<X, Y, A, A> f) => Ref.Swap(x, y, f);
        public CommuteRef<A> Commute(Func<A, A> f) => Ref.Commute(f);
        public A Commute<X>(X x, Func<X, A, A> f) => Ref.Commute(x, f);
        public A Commute<X, Y>(X x, Y y, Func<X, Y, A, A> f) => Ref.Commute(x, y, f);
    }
}
