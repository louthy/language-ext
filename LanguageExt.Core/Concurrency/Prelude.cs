using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Generates a new reference that can be used within a dosync transaction
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ref<A> Ref<A>(A value) =>
            STM.NewRef(value);

        /// <summary>
        /// Run the op within a new transaction
        /// If a transaction is already running, then this becomes part of the parent transaction
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R dosync<R>(Func<R> op, Isolation isolation = Isolation.Snapshot) =>
            STM.DoTransaction(op, isolation);

        /// <summary>
        /// Run the op within a new transaction
        /// If a transaction is already running, then this becomes part of the parent transaction
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<R> dosync<R>(Func<Task<R>> op, Isolation isolation = Isolation.Snapshot) =>
            STM.DoTransactionAsync(op, isolation);

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
        public static Atom<A> Atom<A>(A value) where A : struct =>
            LanguageExt.Atom<A>.New(value);

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
        public static Option<Atom<A>> Atom<A>(A value, Func<A, bool> validator) where A : struct =>
            LanguageExt.Atom<A>.New(value, validator);

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
        public static Atom<M, A> Atom<M, A>(M metadata, A value) where A : struct =>
            LanguageExt.Atom<M, A>.New(metadata, value);

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
        public static Option<Atom<M, A>> Atom<M, A>(M metadata, A value, Func<A, bool> validator) where A : struct =>
            LanguageExt.Atom<M, A>.New(metadata, value, validator);

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
        public static bool swap<A>(Atom<A> atom, Func<A, A> f) where A : struct =>
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
        public static Task<bool> swapAsync<A>(Atom<A> atom, Func<A, Task<A>> f) where A : struct =>
            atom.SwapAsync(f);

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
        public static bool swap<M, A>(Atom<M, A> atom, Func<M, A, A> f) where A : struct =>
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
        public static Task<bool> swapAsync<M, A>(Atom<M, A> atom, Func<M, A, Task<A>> f) where A : struct =>
            atom.SwapAsync(f);

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
        public static bool swap<X, A>(Atom<A> atom, X x, Func<X, A, A> f) where A : struct =>
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
        public static Task<bool> swapAsync<X, A>(Atom<A> atom, X x, Func<X, A, Task<A>> f) where A : struct =>
            atom.SwapAsync(x, f);

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
        public static bool swap<M, X, A>(Atom<M, A> atom, X x, Func<M, X, A, A> f) where A : struct =>
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
        public static Task<bool> swapAsync<M, X, A>(Atom<M, A> atom, X x, Func<M, X, A, Task<A>> f) where A : struct =>
            atom.SwapAsync(x, f);

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
        public static bool swap<X, Y, A>(Atom<A> atom, X x, Y y, Func<X, Y, A, A> f) where A : struct =>
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
        public static Task<bool> swapAsync<X, Y, A>(Atom<A> atom, X x, Y y, Func<X, Y, A, Task<A>> f) where A : struct =>
            atom.SwapAsync(x, y, f);

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
        public static bool swap<M, X, Y, A>(Atom<M, A> atom, X x, Y y, Func<M, X, Y, A, A> f) where A : struct =>
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
        public static Task<bool> swapAsync<M, X, Y, A>(Atom<M, A> atom, X x, Y y, Func<M, X, Y, A, Task<A>> f) where A : struct =>
            atom.SwapAsync(x, y, f);

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
        public static Task<bool> swapAsync<A>(AtomRef<A> atom, Func<A, Task<A>> f) where A : class =>
            atom.SwapAsync(f);

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
        /// <param name="f">Function to update the atom</param>
        /// <returns>`true` if new-value passes any validation and was successfully set.  `false`
        /// will only be returned if the `validator` fails.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<bool> swapAsync<M, A>(AtomRef<M, A> atom, Func<M, A, Task<A>> f) where A : class =>
            atom.SwapAsync(f);

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
        public static Task<bool> swapAsync<X, A>(AtomRef<A> atom, X x, Func<X, A, Task<A>> f) where A : class =>
            atom.SwapAsync(x, f);

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
        /// <param name="f">Function to update the atom</param>
        /// <returns>`true` if new-value passes any validation and was successfully set.  `false`
        /// will only be returned if the `validator` fails.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<bool> swapAsync<M, X, A>(AtomRef<M, A> atom, X x, Func<M, X, A, Task<A>> f) where A : class =>
            atom.SwapAsync(x, f);

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
        public static Task<bool> swapAsync<X, Y, A>(AtomRef<A> atom, X x, Y y, Func<X, Y, A, Task<A>> f) where A : class =>
            atom.SwapAsync(x, y, f);

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
        public static Task<bool> swapAsync<M, X, Y, A>(AtomRef<M, A> atom, X x, Y y, Func<M, X, Y, A, Task<A>> f) where A : class =>
            atom.SwapAsync(x, y, f);
    }
}
