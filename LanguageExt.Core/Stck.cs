using System;
using System.Collections;
using System.Collections.Generic;

namespace LanguageExt
{
    public class Stck<T> : IEnumerable<T>, IEnumerable
    {
        Lst<T> stack;

        internal Stck()
        {
            stack = Lst<T>.Empty;
        }

        internal Stck(IEnumerable<T> initial)
        {
            stack = new Lst<T>(initial);
        }

        internal Stck(Lst<T> initial)
        {
            stack = initial;
        }

        public bool IsEmpty => 
            stack.Count == 0;

        public Stck<T> Clear() =>
            new Stck<T>();

        public IEnumerator<T> GetEnumerator() =>
            stack.GetEnumerator();

        public T Peek()
        {
            if (stack.Count > 0)
            {
                return stack[stack.Count - 1];
            }
            else
            {
                throw new InvalidOperationException("Stack is empty");
            }
        }

        public Stck<T> Pop()
        {
            if (stack.Count > 0)
            {
                return new Stck<T>(stack.RemoveAt(stack.Count - 1));
            }
            else
            {
                throw new InvalidOperationException("Stack is empty");
            }
        }

        public Stck<T> Pop(out T outValue)
        {
            if (stack.Count > 0)
            {
                outValue = stack[stack.Count - 1];
                return new Stck<T>(stack.RemoveAt(stack.Count - 1));
            }
            else
            {
                throw new InvalidOperationException("Stack is empty");
            }
        }

        public Tuple<Stck<T>, Option<T>> TryPop()
        {
            if (stack.Count > 0)
            {
                var value = stack[stack.Count - 1];
                return Tuple.Create(new Stck<T>(stack.RemoveAt(stack.Count - 1)), Prelude.Some(value));
            }
            else
            {
                return Tuple.Create<Stck<T>, Option<T>>(this, Prelude.None);
            }
        }

        public Option<T> TryPeek()
        {
            if (stack.Count > 0)
            {
                var value = stack[stack.Count - 1];
                return Prelude.Some(value);
            }
            else
            {
                return Prelude.None;
            }
        }

        public Stck<T> Push(T value) =>
            new Stck<T>(stack.Add(value));

        IEnumerator IEnumerable.GetEnumerator() =>
            stack.GetEnumerator();
    }
}
