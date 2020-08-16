using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.Interfaces;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Generates a new reference that can be used within a `sync` transaction
        /// 
        /// `Refs` ensure safe shared use of mutable storage locations via a software transactional 
        /// memory (STM) system. `Refs` are bound to a single storage location for their lifetime, 
        /// and only allow mutation of that location to occur within a transaction.
        /// </summary>
        /// <remarks>
        /// 
        /// Transactions (within a `sync`) should be easy to understand if you’ve ever used database 
        /// transactions - they ensure that all actions on Refs are atomic, consistent, and isolated. 
        /// 
        /// * Atomic - means that every change to Refs made within a transaction occurs or none do. 
        /// * Consistent - means that each new value can be checked with a validator function before allowing 
        /// the transaction to commit. 
        /// * Isolated - means that no transaction sees the effects of any other transaction while it is 
        /// running. 
        /// 
        /// Another feature common to STMs is that, should a transaction have a conflict while running, 
        /// it is automatically retried.  The language-ext STM uses multiversion concurrency control for 
        /// snapshot and serialisable isolation.
        /// 
        /// In practice, this means:
        ///
        /// All reads of Refs will see a consistent snapshot of the 'Ref world' as of the starting point 
        /// of the transaction (its 'read point'). The transaction will see any changes it has made. 
        /// This is called the in-transaction-value.
        ///
        /// All changes made to Refs during a transaction will appear to occur at a single point in the 
        /// 'Ref world' timeline (its 'write point').
        ///
        /// No changes will have been made by any other transactions to any Refs that have been modified 
        /// by this transaction.
        ///
        /// Readers will never block writers, or other readers.
        ///
        /// Writers will never block readers.
        ///
        /// I/O and other activities with side-effects should be avoided in transactions, since transactions 
        /// will be retried. 
        ///
        /// If a constraint on the validity of a value of a Ref that is being changed depends upon the 
        /// simultaneous value of a Ref that is not being changed, that second Ref can be protected from 
        /// modification by running the `sync` transaction with Isolation.Serialisable.
        ///
        /// The language-ext STM is designed to work with the persistent collections (`Map`, `HashMap`, 
        /// `Seq`, `Lst`, `Set, `HashSet` etc.), and it is strongly recommended that you use the language-ext 
        /// collections as the values of your Refs. Since all work done in an STM transaction is speculative, 
        /// it is imperative that there be a low cost to making copies and modifications. Persistent collections 
        /// have free copies (just use the original, it can’t be changed), and 'modifications' share structure 
        /// efficiently. In any case:
        ///
        /// The values placed in Refs must be, or be considered, **immutable** Otherwise, Clojure can’t help you.
        /// </remarks>
        /// <param name="value">Initial value of the ref</param>
        /// <param name="validator">Validator that is called on the ref value just
        /// before any transaction is committed (within a `sync`)</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ref<A> Ref<A>(A value, Func<A, bool> validator = null) =>
            STM.NewRef(value);

        /// <summary>
        /// Run the op within a new transaction
        /// If a transaction is already running, then this becomes part of the parent transaction
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static R sync<R>(Func<R> op, Isolation isolation = Isolation.Snapshot) =>
            STM.DoTransaction(op, isolation);

        /// <summary>
        /// Run the op within a new transaction
        /// If a transaction is already running, then this becomes part of the parent transaction
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Unit sync(Action op, Isolation isolation = Isolation.Snapshot) =>
            STM.DoTransaction(() => { op(); return unit; }, isolation);

        /// <summary>
        /// Swap the old value for the new returned by `f`
        /// Must be run within a `sync` transaction
        /// </summary>
        /// <param name="r">`Ref` to process</param>
        /// <param name="f">Function to update the `Ref`</param>
        /// <returns>The value returned from `f`</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static A swap<A>(Ref<A> r, Func<A, A> f) =>
            r.Swap(f);

        /// <summary>
        /// Swap the old value for the new returned by `f`
        /// Must be run within a `sync` transaction
        /// </summary>
        /// <param name="r">`Ref` to process</param>
        /// <param name="f">Function to update the `Ref`</param>
        /// <returns>The value returned from `f`</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static A swap<A, X>(Ref<A> r, X x, Func<X, A, A> f) =>
            r.Swap(x, f);

        /// <summary>
        /// Swap the old value for the new returned by `f`
        /// Must be run within a `sync` transaction
        /// </summary>
        /// <param name="r">`Ref` to process</param>
        /// <param name="f">Function to update the `Ref`</param>
        /// <returns>The value returned from `f`</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static A swap<A, X, Y >(Ref<A> r, X x, Y y, Func<X, Y, A, A> f) =>
            r.Swap(x, y, f);

        /// <summary>
        /// Must be called in a transaction. Sets the in-transaction-value of
        /// ref to:  
        /// 
        ///     `f(in-transaction-value-of-ref)`
        ///     
        /// and returns the in-transaction-value when complete.
        /// 
        /// At the commit point of the transaction, `f` is run *AGAIN* with the
        /// most recently committed value:
        /// 
        ///     `f(most-recently-committed-value-of-ref)`
        /// 
        /// Thus `f` should be commutative, or, failing that, you must accept
        /// last-one-in-wins behavior.
        /// 
        /// Commute allows for more concurrency than just setting the Ref's value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static A commute<A>(Ref<A> r, Func<A, A> f) =>
            r.Commute(f);

        /// <summary>
        /// Must be called in a transaction. Sets the in-transaction-value of
        /// ref to:  
        /// 
        ///     `f(in-transaction-value-of-ref)`
        ///     
        /// and returns the in-transaction-value when complete.
        /// 
        /// At the commit point of the transaction, `f` is run *AGAIN* with the
        /// most recently committed value:
        /// 
        ///     `f(most-recently-committed-value-of-ref)`
        /// 
        /// Thus `f` should be commutative, or, failing that, you must accept
        /// last-one-in-wins behavior.
        /// 
        /// Commute allows for more concurrency than just setting the Ref's value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static A commute<A, X>(Ref<A> r, X x, Func<X, A, A> f) =>
            r.Commute(x, f);

        /// <summary>
        /// Must be called in a transaction. Sets the in-transaction-value of
        /// ref to:  
        /// 
        ///     `f(in-transaction-value-of-ref)`
        ///     
        /// and returns the in-transaction-value when complete.
        /// 
        /// At the commit point of the transaction, `f` is run *AGAIN* with the
        /// most recently committed value:
        /// 
        ///     `f(most-recently-committed-value-of-ref)`
        /// 
        /// Thus `f` should be commutative, or, failing that, you must accept
        /// last-one-in-wins behavior.
        /// 
        /// Commute allows for more concurrency than just setting the Ref's value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static A commute<A, X, Y>(Ref<A> r, X x, Y y, Func<X, Y, A, A> f) =>
            r.Commute(x, y, f);

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
        public static Atom<A> Atom<A>(A value) =>
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
        public static Option<Atom<A>> Atom<A>(A value, Func<A, bool> validator) =>
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
        public static Atom<M, A> Atom<M, A>(M metadata, A value) =>
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
        public static Option<Atom<M, A>> Atom<M, A>(M metadata, A value, Func<A, bool> validator) =>
            LanguageExt.Atom<M, A>.New(metadata, value, validator);

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Option in a Some state, with the result of the invocation of `f`, if the swap succeeded
        /// and its validation passed. None otherwise</returns>
        public static Option<A> swap<A>(Atom<A> ma, Func<A, A> f) =>
            ma.Swap(f);
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Eff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static EffPure<A> swapEff<A>(Atom<A> ma, Func<A, EffPure<A>> f) =>
            ma.SwapEff(f);
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Eff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static Eff<RT, A> swapEff<RT, A>(Atom<A> ma, Func<A, Eff<RT, A>> f) =>
            ma.SwapEff<RT>(f);

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Option in a Some state, with the result of the invocation of `f`, if the swap succeeded
        /// and its validation passed. None otherwise</returns>
        public static ValueTask<Option<A>> swapAsync<A>(Atom<A> ma, Func<A, ValueTask<A>> f) =>
            ma.SwapAsync(f);
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static AffPure<A> swapAff<A>(Atom<A> ma, Func<A, AffPure<A>> f) =>
            ma.SwapAff(f);
                
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static AffPure<A> swapAff<A>(Atom<A> ma, Func<A, ValueTask<A>> f) =>
            ma.SwapAff(f);

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static Aff<RT, A> swapAff<RT, A>(Atom<A> ma, Func<A, Aff<RT, A>> f) where RT : struct, HasCancel<RT> =>
            ma.SwapAff<RT>(f);

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Option in a Some state, with the result of the invocation of `f`, if the swap succeeded
        /// and its validation passed. None otherwise</returns>
        public static Option<A> swap<X, A>(Atom<A> ma, X x, Func<X, A, A> f) =>
            ma.Swap<X>(x, f);
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Eff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static EffPure<A> swapEff<X, A>(Atom<A> ma, X x, Func<X, A, EffPure<A>> f) =>
            ma.SwapEff<X>(x, f);
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Eff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static Eff<RT, A> swapEff<RT, X, A>(Atom<A> ma, X x, Func<X, A, Eff<RT, A>> f) =>
            ma.SwapEff<RT, X>(x, f);

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Option in a Some state, with the result of the invocation of `f`, if the swap succeeded
        /// and its validation passed. None otherwise</returns>
        public static ValueTask<Option<A>> swapAsync<X, A>(Atom<A> ma, X x, Func<X, A, ValueTask<A>> f) =>
            ma.SwapAsync<X>(x, f);
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static AffPure<A> swapAff<X, A>(Atom<A> ma, X x, Func<X, A, AffPure<A>> f) =>
            ma.SwapAff<X>(x, f);
                
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static AffPure<A> swapAff<X, A>(Atom<A> ma, X x, Func<X, A, ValueTask<A>> f) =>
            ma.SwapAff<X>(x, f);
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static Aff<RT, A> swapAff<RT, X, A>(Atom<A> ma, X x, Func<X, A, Aff<RT, A>> f) where RT : struct, HasCancel<RT> =>
            ma.SwapAff<RT, X>(x, f);

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="y">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Option in a Some state, with the result of the invocation of `f`, if the swap succeeded
        /// and its validation passed. None otherwise</returns>
        public static Option<A> swap<X, Y, A>(Atom<A> ma, X x, Y y, Func<X, Y, A, A> f) =>
            ma.Swap<X, Y>(x, y, f);
                
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="y">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Eff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static EffPure<A> swapEff<X, Y, A>(Atom<A> ma, X x, Y y, Func<X, Y, A, EffPure<A>> f) =>
            ma.SwapEff<X, Y>(x, y, f);
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="y">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Eff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static Eff<RT, A> swapEff<RT, X, Y, A>(Atom<A> ma, X x, Y y, Func<X, Y, A, Eff<RT, A>> f) =>
            ma.SwapEff<RT, X, Y>(x, y, f);

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="y">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Option in a Some state, with the result of the invocation of `f`, if the swap succeeded
        /// and its validation passed. None otherwise</returns>
        public static ValueTask<Option<A>> swapAsync<X, Y, A>(Atom<A> ma, X x, Y y, Func<X, Y, A, ValueTask<A>> f) =>
            ma.SwapAsync<X, Y>(x, y, f);
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="y">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static AffPure<A> swapAff<X, Y, A>(Atom<A> ma, X x, Y y, Func<X, Y, A, AffPure<A>> f) =>
            ma.SwapAff<X, Y>(x, y, f);
                
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="y">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static AffPure<A> swapAff<X, Y, A>(Atom<A> ma, X x, Y y, Func<X, Y, A, ValueTask<A>> f) =>
            ma.SwapAff<X, Y>(x, y, f);
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="y">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static Aff<RT, A> swapAff<RT, X, Y, A>(Atom<A> ma, X x, Y y, Func<X, Y, A, Aff<RT, A>> f) where RT : struct, HasCancel<RT> =>
            ma.SwapAff<RT, X, Y>(x, y, f);


        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Option in a Some state, with the result of the invocation of `f`, if the swap succeeded
        /// and its validation passed. None otherwise</returns>
        public static Option<A> swap<M, A>(Atom<M, A> ma, Func<M, A, A> f) =>
            ma.Swap(f);
                
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Eff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static EffPure<A> swapEff<M, A>(Atom<M, A> ma, Func<M, A, EffPure<A>> f) =>
            ma.SwapEff(f);
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Eff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static Eff<RT, A> swapEff<RT, M, A>(Atom<M, A> ma, Func<M, A, Eff<RT, A>> f) =>
            ma.SwapEff<RT>(f);

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Option in a Some state, with the result of the invocation of `f`, if the swap succeeded
        /// and its validation passed. None otherwise</returns>
        public static ValueTask<Option<A>> swapAsync<M, A>(Atom<M, A> ma, Func<M, A, ValueTask<A>> f) =>
            ma.SwapAsync(f);
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static AffPure<A> swapAff<M, A>(Atom<M, A> ma, Func<M, A, AffPure<A>> f) =>
            ma.SwapAff(f);
                
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static AffPure<A> swapAff<M, A>(Atom<M, A> ma, Func<M, A, ValueTask<A>> f) =>
            ma.SwapAff(f);
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static Aff<RT, A> swapAff<RT, M, A>(Atom<M, A> ma, Func<M, A, Aff<RT, A>> f) where RT : struct, HasCancel<RT> =>
            ma.SwapAff<RT>(f);

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Option in a Some state, with the result of the invocation of `f`, if the swap succeeded
        /// and its validation passed. None otherwise</returns>
        public static Option<A> swap<M, X, A>(Atom<M, A> ma, X x, Func<M, X, A, A> f) =>
            ma.Swap<X>(x, f);
                
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Eff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static EffPure<A> swapEff<M, X, A>(Atom<M, A> ma, X x, Func<M, X, A, EffPure<A>> f) =>
            ma.SwapEff<X>(x, f);
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Eff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static Eff<RT, A> swapEff<RT, M, X, A>(Atom<M, A> ma, X x, Func<M, X, A, Eff<RT, A>> f) =>
            ma.SwapEff<RT, X>(x, f);

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Option in a Some state, with the result of the invocation of `f`, if the swap succeeded
        /// and its validation passed. None otherwise</returns>
        public static ValueTask<Option<A>> swapAsync<M, X, A>(Atom<M, A> ma, X x, Func<M, X, A, ValueTask<A>> f) =>
            ma.SwapAsync<X>(x, f);
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static AffPure<A> swapAff<M, X, A>(Atom<M, A> ma, X x, Func<M, X, A, AffPure<A>> f) =>
            ma.SwapAff<X>(x, f);
                
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static AffPure<A> swapAff<M, X, A>(Atom<M, A> ma, X x, Func<M, X, A, ValueTask<A>> f) =>
            ma.SwapAff<X>(x, f);
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static Aff<RT, A> swapAff<RT, M, X, A>(Atom<M, A> ma, X x, Func<M, X, A, Aff<RT, A>> f) where RT : struct, HasCancel<RT> =>
            ma.SwapAff<RT, X>(x, f);

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="y">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Option in a Some state, with the result of the invocation of `f`, if the swap succeeded
        /// and its validation passed. None otherwise</returns>
        public static Option<A> swap<M, X, Y, A>(Atom<M, A> ma, X x, Y y, Func<M, X, Y, A, A> f) =>
            ma.Swap<X, Y>(x, y, f);
                
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="y">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Eff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static EffPure<A> swapEff<M, X, Y, A>(Atom<M, A> ma, X x, Y y, Func<M, X, Y, A, EffPure<A>> f) =>
            ma.SwapEff<X, Y>(x, y, f);
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="y">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Eff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static Eff<RT, A> swapEff<RT, M, X, Y, A>(Atom<M, A> ma, X x, Y y, Func<M, X, Y, A, Eff<RT, A>> f) =>
            ma.SwapEff<RT, X, Y>(x, y, f);

        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="y">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Option in a Some state, with the result of the invocation of `f`, if the swap succeeded
        /// and its validation passed. None otherwise</returns>
        public static ValueTask<Option<A>> swapAsync<M, X, Y, A>(Atom<M, A> ma, X x, Y y, Func<M, X, Y, A, ValueTask<A>> f) =>
            ma.SwapAsync<X, Y>(x, y, f);
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="y">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static AffPure<A> swapAff<X, M, Y, A>(Atom<M, A> ma, X x, Y y, Func<M, X, Y, A, AffPure<A>> f) =>
            ma.SwapAff<X, Y>(x, y, f);
                
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="y">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static AffPure<A> swapAff<X, M, Y, A>(Atom<M, A> ma, X x, Y y, Func<M, X, Y, A, ValueTask<A>> f) =>
            ma.SwapAff<X, Y>(x, y, f);
        
        /// <summary>
        /// Atomically updates the value by passing the old value to `f` and updating
        /// the atom with the result.  Note: `f` may be called multiple times, so it
        /// should be free of side-effects.
        /// </summary>
        /// <param name="x">Additional value to pass to `f`</param>
        /// <param name="y">Additional value to pass to `f`</param>
        /// <param name="f">Function to update the atom</param>
        /// <returns>Aff in a Succ state, with the result of the invocation of `f`, if the swap succeeded and its
        /// validation passed. Failure state otherwise</returns>
        public static Aff<RT, A> swapAff<RT, M, X, Y, A>(Atom<M, A> ma, X x, Y y, Func<M, X, Y, A, Aff<RT, A>> f) where RT : struct, HasCancel<RT> =>
            ma.SwapAff<RT, X, Y>(x, y, f);
    }
}
