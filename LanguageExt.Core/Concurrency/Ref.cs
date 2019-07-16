using System;
using LanguageExt.ClassInstances;

namespace LanguageExt
{
    /// <summary>
    /// Refs ensure safe shared use of mutable storage locations via a software transactional 
    /// memory (STM) system. Refs are bound to a single storage location for their lifetime, and 
    /// only allow mutation of that location to occur within a transaction.
    /// </summary>
    public class Ref<A> : IEquatable<A>
    {
        readonly long Id;

        /// <summary>
        /// Internal ctor
        /// </summary>
        internal Ref(long id) =>
            Id = id;

        /// <summary>
        /// Destructor
        /// </summary>
        ~Ref() => STM.Finalise(Id);

        /// <summary>
        /// Value accessor
        /// </summary>
        public A Value
        {
            get => (A)STM.Read(Id);
            set => STM.Write(Id, value);
        }

        /// <summary>
        /// Assignment operator
        /// </summary>
        public static Ref<A> operator ^(Ref<A> left, A right)
        {
            left.Value = right;
            return left;
        }

        /// <summary>
        /// Implict conversion operator
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator A(Ref<A> value) =>
            value.Value;

        /// <summary>
        /// ToString for the bound value
        /// </summary>
        public override string ToString() =>
            Value?.ToString() ?? "[null]";

        /// <summary>
        /// Hash code of the bound value
        /// </summary>
        public override int GetHashCode() =>
            Value?.GetHashCode() ?? 0;

        /// <summary>
        /// Equality
        /// </summary>
        public override bool Equals(object obj) =>
            obj is A val && Equals(val);

        /// <summary>
        /// Equality
        /// </summary>
        public bool Equals(A other) =>
            default(EqDefault<A>).Equals(other, Value);

        /// <summary>
        /// Swap the old value for the new returned by `f`
        /// Must be run within a `dosync` transaction
        /// </summary>
        /// <param name="f">Swap function</param>
        /// <returns>The value returned from `f`</returns>
        public A Swap(Func<A, A> f)
        {
            var v = f(Value);
            Value = v;
            return v;
        }

        /// <summary>
        /// Swap the old value for the new returned by `f`
        /// Must be run within a `dosync` transaction
        /// </summary>
        /// <param name="f">Swap function</param>
        /// <returns>The value returned from `f`</returns>
        public A Swap<X>(X x, Func<X, A, A> f)
        {
            var v = f(x, Value);
            Value = v;
            return v;
        }

        /// <summary>
        /// Swap the old value for the new returned by `f`
        /// Must be run within a `dosync` transaction
        /// </summary>
        /// <param name="f">Swap function</param>
        /// <returns>The value returned from `f`</returns>
        public A Swap<X, Y>(X x, Y y,Func<X, Y, A, A> f)
        {
            var v = f(x, y, Value);
            Value = v;
            return v;
        }
    }
}
