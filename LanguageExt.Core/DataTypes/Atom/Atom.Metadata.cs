using System;
using System.Runtime.CompilerServices;
using System.Threading;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Atoms provide a way to manage shared, synchronous, independent state without 
    /// locks. 
    /// </summary>
    /// <remarks>
    /// The intended use of atom is to hold one an immutable data structure. You change 
    /// the value by applying a function to the old value. This is done in an atomic 
    /// manner by `Swap`.  
    /// 
    /// Internally, `Swap` reads the current value, applies the function to it, and 
    /// attempts to `CompareExchange` it in. Since another thread may have changed the 
    /// value in the intervening time, it may have to retry, and does so in a spin loop. 
    /// 
    /// The net effect is that the value will always be the result of the application 
    /// of the supplied function to a current value, atomically. However, because the 
    /// function might be called multiple times, it must be free of side effects.
    /// 
    /// Atoms are an efficient way to represent some state that will never need to be 
    /// coordinated with any other, and for which you wish to make synchronous changes.
    /// </remarks>
    public sealed class Atom<M, A> where A : struct
    {
        Box value;
        Func<A, bool> validator;
        readonly M metadata;

        public event AtomChangedEvent<A> Change;

        /// <summary>
        /// Constructor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        Atom(M metadata, A value, Func<A, bool> validator)
        {
            this.value = Box.New(value);
            this.metadata = metadata;
            this.validator = validator;
        }

        /// <summary>
        /// Internal constructor function that runs the validator on the value
        /// before returning the Atom so that the Atom can never be in an invalid
        /// state.  The validator is then used for all state transitions going 
        /// forward.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Option<Atom<M, A>> New(M metadata, A value, Func<A, bool> validator)
        {
            var atom = new Atom<M, A>(metadata, value, validator ?? throw new ArgumentNullException(nameof(validator)));
            return validator(value)
                ? Some(atom)
                : None;
        }

        /// <summary>
        /// Internal constructor
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Atom<M, A> New(M metadata, A value) =>
            new Atom<M, A>(metadata, value, True);

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="f">Function to update the atom</param>
        /// <returns>`true` if new-value passes any validation and was successfully set.  `false`
        /// will only be returned if the `validator` fails.</returns>
        public bool Swap(Func<M, A, A> f)
        {
            f = f ?? throw new ArgumentNullException(nameof(f));

            while (true)
            {
                var current = value;
                var newValue = Box.New(f(metadata, current.Value));
                if (!validator(newValue.Value))
                {
                    return false;
                }
                if(Interlocked.CompareExchange(ref value, newValue, current) == current)
                {
                    current.Wipe();
                    Pool<NewBox, Box, A>.Push(current);
                    Change?.Invoke(newValue.Value);
                    return true;
                }
            }
        }

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>`true` if new-value passes any validation and was successfully set.  `false`
        /// will only be returned if the `validator` fails.</returns>
        public bool Swap<X>(X x, Func<M, X, A, A> f)
        {
            f = f ?? throw new ArgumentNullException(nameof(f));

            while (true)
            {
                var current = value;
                var newValue = Box.New(f(metadata, x, current.Value));
                if (!validator(newValue.Value))
                {
                    return false;
                }
                if (Interlocked.CompareExchange(ref value, newValue, current) == current)
                {
                    current.Wipe();
                    Pool<NewBox, Box, A>.Push(current);
                    Change?.Invoke(newValue.Value);
                    return true;
                }
            }
        }

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="y">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>`true` if new-value passes any validation and was successfully set.  `false`
        /// will only be returned if the `validator` fails.</returns>
        public bool Swap<X, Y>(X x, Y y, Func<M, X, Y, A, A> f)
        {
            f = f ?? throw new ArgumentNullException(nameof(f));

            while (true)
            {
                var current = value;
                var newValue = Box.New(f(metadata, x, y, current.Value));
                if (!validator(newValue.Value))
                {
                    return false;
                }
                if (Interlocked.CompareExchange(ref value, newValue, current) == current)
                {
                    current.Wipe();
                    Pool<NewBox, Box, A>.Push(current);
                    Change?.Invoke(newValue.Value);
                    return true;
                }
            }
        }

        /// <summary>
        /// Current state
        /// </summary>
        public A Value
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => value.Value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString() =>
            Value.ToString();

        /// <summary>
        /// Implicit conversion to `A`
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator A(Atom<M, A> atom) =>
            atom.Value;

        /// <summary>
        /// Helper for validator
        /// </summary>
        /// <param name="_"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool True(A _) => true;

        internal class Box
        {
            public A Value;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Box(A value) =>
                Value = value;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static Box New(A value) =>
                Pool<NewBox, Box, A>.Pop(value);

            public void Wipe() =>
                Value = default;
        }

        internal struct NewBox : New<Box, A>
        {
            public Box New(A value) =>
                new Box(value);

            public void Set(Box box, A value) =>
                box.Value = value;
        }
    }
}
