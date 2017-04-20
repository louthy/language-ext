using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Immutable stack
    /// </summary>
    /// <typeparam name="T">Stack element type</typeparam>
    [Serializable]
    public struct Stck<T> : 
        IEnumerable<T>, 
        IEnumerable,
        IEquatable<Stck<T>>
    {
        public readonly static Stck<T> Empty = new Stck<T>(StckInternal<T>.Empty);

        int hashCode;
        readonly StckInternal<T> value;
        StckInternal<T> Value => value ?? StckInternal<T>.Empty;

        /// <summary>
        /// Default ctor
        /// </summary>
        internal Stck(StckInternal<T> value)
        {
            this.value = value;
            this.hashCode = 0;
        }

        /// <summary>
        /// Ctor that takes an initial state as an IEnumerable T
        /// </summary>
        public Stck(IEnumerable<T> initial)
        {
            value = new StckInternal<T>(initial);
            this.hashCode = 0;
        }

        /// <summary>
        /// Number of items in the stack
        /// </summary>
        [Pure]
        public int Count => Value.Count;

        /// <summary>
        /// Reverses the order of the items in the stack
        /// </summary>
        /// <returns></returns>
        [Pure]
        public Stck<T> Reverse() =>
            new Stck<T>(Value.Reverse());

        /// <summary>
        /// True if the stack is empty
        /// </summary>
        [Pure]
        public bool IsEmpty => 
            Count == 0;

        /// <summary>
        /// Clear the stack (returns Empty)
        /// </summary>
        /// <returns>Stck.Empty of T</returns>
        [Pure]
        public Stck<T> Clear() =>
            Empty;

        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns>IEnumerator of T</returns>
        [Pure]
        public IEnumerator<T> GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        /// <summary>
        /// Returns the stack as a Sq.  The first item in the sequence
        /// will be the item at the top of the stack.
        /// </summary>
        /// <returns>IEnumerable of T</returns>
        [Pure]
        public Seq<T> ToSeq() =>
            Seq(Value);

        /// <summary>
        /// Returns the stack as an IEnumerable.  The first item in the enumerable
        /// will be the item at the top of the stack.
        /// </summary>
        /// <returns>IEnumerable of T</returns>
        [Pure]
        public IEnumerable<T> AsEnumerable() =>
            Value;

        /// <summary>
        /// Return the item on the top of the stack without affecting the stack itself
        /// NOTE: Will throw an InvalidOperationException if the stack is empty
        /// </summary>
        /// <exception cref="InvalidOperationException">Stack is empty</exception>
        /// <returns>Top item value</returns>
        [Pure]
        public T Peek() =>
            Value.Peek();

        /// <summary>
        /// Peek and match
        /// </summary>
        /// <param name="Some">Handler if there is a value on the top of the stack</param>
        /// <param name="None">Handler if the stack is empty</param>
        /// <returns>Untouched stack (this)</returns>
        [Pure]
        public Stck<T> Peek(Action<T> Some, Action None) =>
            new Stck<T>(Value.Peek(Some, None));

        /// <summary>
        /// Peek and match
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Handler if there is a value on the top of the stack</param>
        /// <param name="None">Handler if the stack is empty</param>
        /// <returns>Return value from Some or None</returns>
        [Pure]
        public R Peek<R>(Func<T, R> Some, Func<R> None) =>
            Value.Peek(Some, None);

        /// <summary>
        /// Safely return the item on the top of the stack without affecting the stack itself
        /// </summary>
        /// <returns>Returns the top item value, or None</returns>
        [Pure]
        public Option<T> TryPeek() =>
            Value.TryPeek();

        /// <summary>
        /// Pop an item off the top of the stack
        /// NOTE: Will throw an InvalidOperationException if the stack is empty
        /// </summary>
        /// <exception cref="InvalidOperationException">Stack is empty</exception>
        /// <returns>Stack with the top item popped</returns>
        [Pure]
        public Stck<T> Pop() =>
            new Stck<T>(Value.Pop());

        /// <summary>
        /// Safe pop
        /// </summary>
        /// <returns>Tuple of popped stack and optional top-of-stack value</returns>
        [Pure]
        public (Stck<T> Stack, Option<T> Value) TryPop() =>
            Value.TryPop().MapFirst(x => new Stck<T>(x));

        /// <summary>
        /// Pop and match
        /// </summary>
        /// <param name="Some">Handler if there is a value on the top of the stack</param>
        /// <param name="None">Handler if the stack is empty</param>
        /// <returns>Popped stack</returns>
        [Pure]
        public Stck<T> Pop(Action<T> Some, Action None) =>
            new Stck<T>(Value.Pop(Some, None));

        /// <summary>
        /// Pop and match
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Handler if there is a value on the top of the stack</param>
        /// <param name="None">Handler if the stack is empty</param>
        /// <returns>Return value from Some or None</returns>
        [Pure]
        public R Pop<R>(Func<Stck<T>, T, R> Some, Func<R> None) =>
            Value.Pop((s, t) => Some(new Stck<T>(s), t), None);

        /// <summary>
        /// Push an item onto the stack
        /// </summary>
        /// <param name="value">Item to push</param>
        /// <returns>New stack with the pushed item on top</returns>
        [Pure]
        public Stck<T> Push(T value) =>
            new Stck<T>(Value.Push(value));

        /// <summary>
        /// Get enumerator
        /// </summary>
        /// <returns>IEnumerator of T</returns>
        [Pure]
        IEnumerator IEnumerable.GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        /// <summary>
        /// Append another stack to the top of this stack
        /// The rhs will be reversed and pushed onto 'this' stack.  That will
        /// maintain the order of the items in the resulting stack.  So the top
        /// of 'rhs' will be the top of the newly created stack.  'this' stack
        /// will be under the 'rhs' stack.
        /// </summary>
        [Pure]
        public static Stck<T> operator +(Stck<T> lhs, Stck<T> rhs) =>
            lhs.Append(rhs);

        /// <summary>
        /// Append another stack to the top of this stack
        /// The rhs will be reversed and pushed onto 'this' stack.  That will
        /// maintain the order of the items in the resulting stack.  So the top
        /// of 'rhs' will be the top of the newly created stack.  'this' stack
        /// will be under the 'rhs' stack.
        /// </summary>
        /// <param name="rhs">Stack to append</param>
        /// <returns>Appended stacks</returns>
        [Pure]
        public Stck<T> Append(Stck<T> rhs) =>
            new Stck<T>(Value.Append(rhs.Value));

        /// <summary>
        /// Subtract one stack from another: lhs except rhs
        /// </summary>
        [Pure]
        public static Stck<T> operator -(Stck<T> lhs, Stck<T> rhs) =>
            lhs.Subtract(rhs);

        /// <summary>
        /// Append another stack to the top of this stack
        /// The rhs will be reversed and pushed onto 'this' stack.  That will
        /// maintain the order of the items in the resulting stack.  So the top
        /// of 'rhs' will be the top of the newly created stack.  'this' stack
        /// will be under the 'rhs' stack.
        /// </summary>
        /// <param name="rhs">Stack to append</param>
        /// <returns>Appended stacks</returns>
        [Pure]
        public Stck<T> Subtract(Stck<T> rhs) =>
            new Stck<T>(Enumerable.Except(this, rhs));

        [Pure]
        public static bool operator ==(Stck<T> lhs, Stck<T> rhs) =>
            lhs.Equals(rhs);

        [Pure]
        public static bool operator !=(Stck<T> lhs, Stck<T> rhs) =>
            !(lhs == rhs);

        [Pure]
        public override int GetHashCode() =>
            hashCode == 0
                ? hashCode = hash(this)
                : hashCode;

        [Pure]
        public override bool Equals(object obj) =>
            obj is Stck<T> && Equals((Stck<T>)obj);

        [Pure]
        public bool Equals(Stck<T> other) =>
            hashCode == other.hashCode && Enumerable.Equals(this.Value, other.Value);
    }
}
