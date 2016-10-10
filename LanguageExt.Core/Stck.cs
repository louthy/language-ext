using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace LanguageExt
{
    /// <summary>
    /// Immutable stack
    /// </summary>
    /// <typeparam name="T">Stack element type</typeparam>
#if !COREFX
    [Serializable]
#endif
    public class Stck<T> : IEnumerable<T>, IEnumerable, IAppendable<Stck<T>>
    {
        public readonly static Stck<T> Empty = new Stck<T>();

        readonly T value;
        readonly Stck<T> tail;

        /// <summary>
        /// Default ctor
        /// </summary>
        internal Stck()
        {
        }

        /// <summary>
        /// Ctor for Push
        /// </summary>
        /// <param name="value"></param>
        /// <param name="tail"></param>
        internal Stck(T value, Stck<T> tail)
        {
            Count = tail.Count + 1;
            this.tail = tail;
            this.value = value;
        }

        /// <summary>
        /// Ctor that takes an initial state as an IEnumerable T
        /// </summary>
        internal Stck(IEnumerable<T> initial)
        {
            tail = new Stck<T>();
            foreach (var item in initial)
            {
                value = item;
                tail = tail.Push(item);
                Count++;
            }
            tail = tail.Pop();
        }

        /// <summary>
        /// Ctor that takes an initial state as a Lst T
        /// </summary>
        internal Stck(Lst<T> initial)
        {
            tail = new Stck<T>();
            foreach (var item in initial)
            {
                value = item;
                tail = tail.Push(item);
                Count++;
            }
            tail = tail.Pop();
        }

        /// <summary>
        /// Number of items in the stack
        /// </summary>
        [Pure]
        public int Count { get; }

        /// <summary>
        /// Reverses the order of the items in the stack
        /// </summary>
        /// <returns></returns>
        [Pure]
        public Stck<T> Reverse()
        {
            var s = new Stck<T>();
            foreach (var item in this)
            {
                s = s.Push(item);
            }
            return s;
        }

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
        /// Returns the stack as an IEnumerable.  The first item in the enumerable
        /// will be the item at the top of the stack.
        /// </summary>
        /// <returns>IEnumerable of T</returns>
        [Pure]
        public IEnumerable<T> AsEnumerable()
        {
            var self = this;
            while (self.Count != 0)
            {
                yield return self.value;
                self = self.tail;
            }
        }

        /// <summary>
        /// Return the item on the top of the stack without affecting the stack itself
        /// NOTE: Will throw an InvalidOperationException if the stack is empty
        /// </summary>
        /// <exception cref="InvalidOperationException">Stack is empty</exception>
        /// <returns>Top item value</returns>
        [Pure]
        public T Peek()
        {
            if (Count > 0)
            {
                return value;
            }
            else
            {
                throw new InvalidOperationException("Stack is empty");
            }
        }

        /// <summary>
        /// Peek and match
        /// </summary>
        /// <param name="Some">Handler if there is a value on the top of the stack</param>
        /// <param name="None">Handler if the stack is empty</param>
        /// <returns>Untouched stack (this)</returns>
        [Pure]
        public Stck<T> Peek(Action<T> Some, Action None)
        {
            if (Count > 0)
            {
                Some(value);
            }
            else
            {
                None();
            }
            return this;
        }

        /// <summary>
        /// Peek and match
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Handler if there is a value on the top of the stack</param>
        /// <param name="None">Handler if the stack is empty</param>
        /// <returns>Return value from Some or None</returns>
        [Pure]
        public R Peek<R>(Func<T, R> Some, Func<R> None) =>
            Count > 0
                ? Some(value)
                : None();

        /// <summary>
        /// Safely return the item on the top of the stack without affecting the stack itself
        /// </summary>
        /// <returns>Returns the top item value, or None</returns>
        [Pure]
        public Option<T> TryPeek() =>
            Count > 0
                ? Prelude.Some(value)
                : Prelude.None;

        /// <summary>
        /// Pop an item off the top of the stack
        /// NOTE: Will throw an InvalidOperationException if the stack is empty
        /// </summary>
        /// <exception cref="InvalidOperationException">Stack is empty</exception>
        /// <returns>Stack with the top item popped</returns>
        [Pure]
        public Stck<T> Pop()
        {
            if (Count > 0)
            {
                return tail;
            }
            else
            {
                throw new InvalidOperationException("Stack is empty");
            }
        }

        /// <summary>
        /// Safe pop
        /// </summary>
        /// <returns>Tuple of popped stack and optional top-of-stack value</returns>
        [Pure]
        public Tuple<Stck<T>, Option<T>> TryPop() =>
            Count > 0
                ? Tuple.Create(tail, Prelude.Some(value))
                : Tuple.Create<Stck<T>, Option<T>>(this, Prelude.None);

        /// <summary>
        /// Pop and match
        /// </summary>
        /// <param name="Some">Handler if there is a value on the top of the stack</param>
        /// <param name="None">Handler if the stack is empty</param>
        /// <returns>Popped stack</returns>
        [Pure]
        public Stck<T> Pop(Action<T> Some, Action None)
        {
            if (Count > 0)
            {
                Some(value);
                return tail;
            }
            else
            {
                None();
                return this;
            }
        }

        /// <summary>
        /// Pop and match
        /// </summary>
        /// <typeparam name="R">Return type</typeparam>
        /// <param name="Some">Handler if there is a value on the top of the stack</param>
        /// <param name="None">Handler if the stack is empty</param>
        /// <returns>Return value from Some or None</returns>
        [Pure]
        public R Pop<R>(Func<Stck<T>, T, R> Some, Func<R> None) =>
            Count > 0
                ? Some(tail, value)
                : None();

        /// <summary>
        /// Push an item onto the stack
        /// </summary>
        /// <param name="value">Item to push</param>
        /// <returns>New stack with the pushed item on top</returns>
        [Pure]
        public Stck<T> Push(T value) =>
            new Stck<T>(value, this);

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
        public Stck<T> Append(Stck<T> rhs)
        {
            var self = this;
            foreach (var item in rhs.Rev())
            {
                self = self.Push(item);
            }
            return self;
        }
    }
}
