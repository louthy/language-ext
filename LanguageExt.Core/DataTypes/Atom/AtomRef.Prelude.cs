using System;
using System.Runtime.CompilerServices;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Atoms provide a way to manage shared, synchronous, independent state without 
        /// locks. 
        /// </summary>
        /// <param name="value">Initial value of the atom</param>
        /// <returns>The constructed Atom</returns>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AtomRef<A> AtomRef<A>(A value) where A : class =>
            LanguageExt.AtomRef<A>.New(value);

        /// <summary>
        /// Atoms provide a way to manage shared, synchronous, independent state without 
        /// locks. 
        /// </summary>
        /// <param name="value">Initial value of the atom</param>
        /// <param name="validator">Function to run on the value after each state change.  
        /// 
        /// If the function returns false for any proposed new state, then the `swap` 
        /// function will return `false`, else it will return `true` on successful setting 
        /// of the atom's state
        /// </param>
        /// <returns>The constructed Atom or None if the validation faled for the initial 
        /// `value` </returns>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<AtomRef<A>> AtomRef<A>(A value, Func<A, bool> validator) where A : class =>
            LanguageExt.AtomRef<A>.New(value, validator);

        /// <summary>
        /// Atoms provide a way to manage shared, synchronous, independent state without 
        /// locks. 
        /// </summary>
        /// <param name="metadata">Metadata to be passed to the validation function</param>
        /// <param name="value">Initial value of the atom</param>
        /// <returns>The constructed Atom</returns>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AtomRef<M, A> AtomRef<M, A>(M metadata, A value) where A : class =>
            LanguageExt.AtomRef<M, A>.New(metadata, value);

        /// <summary>
        /// Atoms provide a way to manage shared, synchronous, independent state without 
        /// locks. 
        /// </summary>
        /// <param name="metadata">Metadata to be passed to the validation function</param>
        /// <param name="value">Initial value of the atom</param>
        /// <param name="validator">Function to run on the value after each state change.  
        /// 
        /// If the function returns false for any proposed new state, then the `swap` 
        /// function will return `false`, else it will return `true` on successful setting 
        /// of the atom's state
        /// </param>
        /// <returns>The constructed Atom or None if the validation faled for the initial 
        /// `value` </returns>
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Option<AtomRef<M, A>> AtomRef<M, A>(M metadata, A value, Func<A, bool> validator) where A : class =>
            LanguageExt.AtomRef<M, A>.New(metadata, value, validator);

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="atom">Atom to process</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>`true` if new-value passes any validation and was successfully set.  `false`
        /// will only be returned if the `validator` fails.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool swap<A>(AtomRef<A> atom, Func<A, A> f) where A : class =>
            atom.Swap(f);

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="atom">Atom to process</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>`true` if new-value passes any validation and was successfully set.  `false`
        /// will only be returned if the `validator` fails.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool swap<M, A>(AtomRef<M, A> atom, Func<M, A, A> f) where A : class => 
            atom.Swap(f);

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="atom">Atom to process</param>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>`true` if new-value passes any validation and was successfully set.  `false`
        /// will only be returned if the `validator` fails.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool swap<X, A>(AtomRef<A> atom, X x, Func<X, A, A> f) where A : class =>
            atom.Swap(x, f);

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="atom">Atom to process</param>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>`true` if new-value passes any validation and was successfully set.  `false`
        /// will only be returned if the `validator` fails.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool swap<M, X, A>(AtomRef<M, A> atom, X x, Func<M, X, A, A> f) where A : class =>
            atom.Swap(x, f);

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="atom">Atom to process</param>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="y">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>`true` if new-value passes any validation and was successfully set.  `false`
        /// will only be returned if the `validator` fails.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool swap<X, Y, A>(AtomRef<A> atom, X x, Y y, Func<X, Y, A, A> f) where A : class =>
            atom.Swap(x, y, f);

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="atom">Atom to process</param>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="y">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>`true` if new-value passes any validation and was successfully set.  `false`
        /// will only be returned if the `validator` fails.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool swap<M, X, Y, A>(AtomRef<M, A> atom, X x, Y y, Func<M, X, Y, A, A> f) where A : class =>
            atom.Swap(x, y, f);
    }
}
