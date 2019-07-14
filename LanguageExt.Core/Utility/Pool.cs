using System;
using System.Collections.Concurrent;

namespace LanguageExt
{
    /// <summary>
    /// Type class for newing an object
    /// </summary>
    internal interface New<A>
    {
        A New();
    }

    /// <summary>
    /// Type class for newing an object with one constructor argument
    /// Also provides a Set for setting the value when being popped off a
    /// pool stack (see `Pool` below).
    /// </summary>
    internal interface New<A, B>
    {
        A New(B value);
        void Set(A item, B value);
    }

    /// <summary>
    /// Thread-safe pooling 
    /// Manages a concurrent stack of values that will grow as needed
    /// When spent new objects are allocated used the `New<A>` type-class
    /// </summary>
    internal static class Pool<NewA, A> where NewA : struct, New<A>
    {
        static ConcurrentStack<A> stack = new ConcurrentStack<A>();

        public static A Pop() =>
            stack.TryPop(out A var)
                ? var
                : default(NewA).New();

        public static void Push(A value) =>
            stack.Push(value);
    }

    /// <summary>
    /// Thread-safe pooling 
    /// Manages a concurrent stack of values that will grow as needed
    /// When spent new objects are allocated used the `New<A>` type-class
    /// </summary>
    internal static class Pool<NewA, A, B> where NewA : struct, New<A, B>
    {
        static ConcurrentStack<A> stack = new ConcurrentStack<A>();

        public static A Pop(B value)
        {
            if(stack.TryPop(out A var))
            {
                default(NewA).Set(var, value);
                return var;
            }
            else
            {
                return default(NewA).New(value);
            }
        }

        public static void Push(A value) =>
            stack.Push(value);
    }
}
