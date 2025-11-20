using System;
using System.Runtime.CompilerServices;
using LanguageExt.Common;

namespace LanguageExt;

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
    /// Transactions (within a `sync(() => ...)`) should be easy to understand if you’ve ever used database 
    /// transactions - they ensure that all actions on Refs are atomic, consistent, and isolated. 
    /// 
    ///  * **Atomic** - means that every change to Refs made within a transaction occurs or none do. 
    ///  * **Consistent** - means that each new value can be checked with a validator function before allowing 
    /// the transaction to commit. 
    ///  * **Isolated** - means that no transaction sees the effects of any other transaction while it is 
    /// running. 
    /// 
    /// Another feature common to STMs is that, should a transaction have a conflict while running, 
    /// it is automatically retried.  The language-ext STM uses multi-version concurrency control for 
    /// snapshot and serialisable isolation.
    /// 
    /// In practice, this means:
    ///
    /// All reads of Refs will see a consistent snapshot of the _Ref world_ as of the starting point 
    /// of the transaction (its 'read point'). The transaction will see any changes it has made. 
    /// This is called the in-transaction-value.
    ///
    /// All changes made to Refs during a transaction will appear to occur at a single point in the 
    /// _Ref world_ timeline (its 'write point').
    ///
    /// No changes will have been made by any other transactions to any Refs that have been modified 
    /// by this transaction.
    ///
    ///  * Readers will never block writers, or other readers.
    ///
    ///  * Writers will never block readers.
    ///
    /// I/O and other activities with side effects should be avoided in transactions, since transactions 
    /// will be retried. 
    ///
    /// If a constraint on the validity of a value of a Ref that is being changed depends upon the 
    /// simultaneous value of a Ref that is not being changed, that second Ref can be protected from 
    /// modification by running the `sync` transaction with `Isolation.Serialisable`.
    ///
    /// The language-ext STM is designed to work with the persistent collections (`Map`, `HashMap`, 
    /// `Seq`, `Lst`, `Set, `HashSet` etc.), and it is strongly recommended that you use the language-ext 
    /// collections as the values of your Refs. Since all work done in an STM transaction is speculative, 
    /// it is imperative that there be a low cost to making copies and modifications. Persistent collections 
    /// have free copies (just use the original, it can’t be changed), and 'modifications' share structure 
    /// efficiently. In any case:
    ///
    /// The values placed in Refs must be, or be considered, **immutable**. Otherwise, this library can’t help you.
    /// </remarks>
    /// <remarks>
    /// See the [concurrency section](https://github.com/louthy/language-ext/wiki/Concurrency) of the wiki for more info.
    /// </remarks>
    /// <param name="value">Initial value of the ref</param>
    /// <param name="validator">Validator that is called on the ref value just
    /// before any transaction is committed (within a `sync`)</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<Ref<A>> refIO<A>(A value, Func<A, bool>? validator = null) =>
        lift(() => Ref(value, validator));
        
    /// <remarks>
    /// Snapshot isolation requires that nothing outside the transaction has written to any of the values that are
    /// *written-to within the transaction*.  If anything does write to the values used within the transaction, then
    /// the transaction is rolled back and retried (using the latest 'world' state). 
    /// </remarks>
    /// <remarks>
    /// Serialisable isolation requires that nothing outside the transaction has written to any of the values that
    /// are *read-from or written-to within the transaction*.  If anything does read from or written to the values used
    /// within the transaction, then it is rolled back and retried (using the latest 'world' state).
    ///
    /// It is the strictest form of isolation, and the most likely to conflict; but protects against cross read/write  
    /// inconsistencies.  For example, if you have:
    ///
    ///     var x = Ref(1);
    ///     var y = Ref(2);
    ///
    ///     snapshot(() => x.Value = y.Value + 1);
    ///
    /// Then something writing to `y` mid-way through the transaction would *not* cause the transaction to fail.
    /// Because `y` was only read-from, not written to.  However, this: 
    ///
    ///     var x = Ref(1);
    ///     var y = Ref(2);
    ///
    ///     serial(() => x.Value = y.Value + 1);
    ///
    /// ... would fail if something wrote to `y`.  
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<R> atomicIO<R>(Func<R> op, Isolation isolation = Isolation.Snapshot) =>
        lift(() => atomic(op, isolation));

    /// <summary>
    /// Run the op within a new transaction
    /// If a transaction is already running, then this becomes part of the parent transaction
    /// </summary>
    /// <remarks>
    /// Snapshot isolation requires that nothing outside the transaction has written to any of the values that are
    /// *written-to within the transaction*.  If anything does write to the values used within the transaction, then
    /// the transaction is rolled back and retried (using the latest 'world' state). 
    /// </remarks>
    /// <remarks>
    /// Serialisable isolation requires that nothing outside the transaction has written to any of the values that
    /// are *read-from or written-to within the transaction*.  If anything does read from or written to the values used
    /// within the transaction, then it is rolled back and retried (using the latest 'world' state).
    ///
    /// It is the strictest form of isolation, and the most likely to conflict; but protects against cross read/write  
    /// inconsistencies.  For example, if you have:
    ///
    ///     var x = Ref(1);
    ///     var y = Ref(2);
    ///
    ///     snapshot(() => x.Value = y.Value + 1);
    ///
    /// Then something writing to `y` mid-way through the transaction would *not* cause the transaction to fail.
    /// Because `y` was only read-from, not written to.  However, this: 
    ///
    ///     var x = Ref(1);
    ///     var y = Ref(2);
    ///
    ///     serial(() => x.Value = y.Value + 1);
    ///
    /// ... would fail if something wrote to `y`.  
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<Unit> atomicIO(Action op, Isolation isolation = Isolation.Snapshot) =>
        lift(() => atomic(op, isolation));
        
    /// <summary>
    /// Run the op within a new transaction
    /// If a transaction is already running, then this becomes part of the parent transaction
    /// </summary>
    /// <remarks>
    /// Snapshot isolation requires that nothing outside the transaction has written to any of the values that are
    /// *written-to within the transaction*.  If anything does write to the values used within the transaction, then
    /// the transaction is rolled back and retried (using the latest 'world' state). 
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<R> snapshotIO<R>(Func<R> op) =>
        lift(() => snapshot(op));

    /// <summary>
    /// Run the op within a new transaction
    /// If a transaction is already running, then this becomes part of the parent transaction
    /// </summary>
    /// <remarks>
    /// Snapshot isolation requires that nothing outside the transaction has written to any of the values that are
    /// *written-to within the transaction*.  If anything does write to the values used within the transaction, then
    /// the transaction is rolled back and retried (using the latest 'world' state). 
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<Unit> snapshotIO(Action op) =>
        lift(() => snapshot(op));

    /// <summary>
    /// Run the op within a new transaction
    /// If a transaction is already running, then this becomes part of the parent transaction
    /// </summary>
    /// <remarks>
    /// Serialisable isolation requires that nothing outside the transaction has written to any of the values that
    /// are *read-from or written-to within the transaction*.  If anything does read from or written to the values used
    /// within the transaction, then it is rolled back and retried (using the latest 'world' state).
    ///
    /// It is the strictest form of isolation, and the most likely to conflict; but protects against cross read/write  
    /// inconsistencies.  For example, if you have:
    ///
    ///     var x = Ref(1);
    ///     var y = Ref(2);
    ///
    ///     snapshot(() => x.Value = y.Value + 1);
    ///
    /// Then something writing to `y` mid-way through the transaction would *not* cause the transaction to fail.
    /// Because `y` was only read-from, not written to.  However, this: 
    ///
    ///     var x = Ref(1);
    ///     var y = Ref(2);
    ///
    ///     serial(() => x.Value = y.Value + 1);
    ///
    /// ... would fail if something wrote to `y`.  
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<R> serialIO<R>(Func<R> op) =>
        lift(() => serial(op));

    /// <summary>
    /// Run the op within a new transaction
    /// If a transaction is already running, then this becomes part of the parent transaction
    /// </summary>
    /// <remarks>
    /// Serialisable isolation requires that nothing outside the transaction has written to any of the values that
    /// are *read-from or written-to within the transaction*.  If anything does read from or written to the values used
    /// within the transaction, then it is rolled back and retried (using the latest 'world' state).
    ///
    /// It is the strictest form of isolation, and the most likely to conflict; but protects against cross read/write  
    /// inconsistencies.  For example, if you have:
    ///
    ///     var x = Ref(1);
    ///     var y = Ref(2);
    ///
    ///     snapshot(() => x.Value = y.Value + 1);
    ///
    /// Then something writing to `y` mid-way through the transaction would *not* cause the transaction to fail.
    /// Because `y` was only read-from, not written to.  However, this: 
    ///
    ///     var x = Ref(1);
    ///     var y = Ref(2);
    ///
    ///     serial(() => x.Value = y.Value + 1);
    ///
    /// ... would fail if something wrote to `y`.  
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<Unit> serialIO(Action op) =>
        lift(() => serial(op));        

    /// <summary>
    /// Swap the old value for the new returned by `f`
    /// Must be run within a `sync` transaction
    /// </summary>
    /// <param name="r">`Ref` to process</param>
    /// <param name="f">Function to update the `Ref`</param>
    /// <returns>The value returned from `f`</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IO<A> swapIO<A>(Ref<A> r, Func<A, A> f) =>
        r.SwapIO(f);

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
    public static IO<A> commuteIO<A>(Ref<A> r, Func<A, A> f) =>
        lift(() => commute(r, f));

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
    public static IO<Atom<A>> atomIO<A>(A value) =>
        lift(() => Atom(value));

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
    public static IO<Atom<A>> atomIO<A>(A value, Func<A, bool> validator) =>
        lift(() => Atom(value, validator) switch
                   {
                       { IsSome: true } atom => (Atom<A>)atom,
                       _                     => throw Exceptions.ValidationFailed
                   });

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
    public static IO<Atom<M, A>> atomIO<M, A>(M metadata, A value) =>
        lift(() => Atom(metadata, value));

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
    public static IO<Atom<M, A>> atomIO<M, A>(M metadata, A value, Func<A, bool> validator) =>
        lift(() => Atom(metadata, value, validator) switch
                   {
                       { IsSome: true } atom => (Atom<M, A>)atom,
                       _                     => throw Exceptions.ValidationFailed
                   });
        
    /// <summary>
    /// Atomically updates the value by passing the old value to `f` and updating
    /// the atom with the result.  Note: `f` may be called multiple times, so it
    /// should be free of side effects.
    /// </summary>
    /// <param name="f">Function to update the atom</param>
    /// <returns>
    /// If the swap operation succeeded then a snapshot of the value that was set is returned.
    /// If the swap operation fails (which can only happen due to its validator returning false),
    /// then a snapshot of the current value within the Atom is returned.
    /// If there is no validator for the Atom then the return value is always the snapshot of
    /// the successful `f` function. 
    /// </returns>
    public static IO<A> swapIO<A>(Atom<A> ma, Func<A, A> f) =>
        ma.SwapIO(f);
        
    /// <summary>
    /// Atomically updates the value by passing the old value to `f` and updating
    /// the atom with the result.  Note: `f` may be called multiple times, so it
    /// should be free of side effects.
    /// </summary>
    /// <param name="f">Function to update the atom</param>
    /// <returns>
    /// If the swap operation succeeded then a snapshot of the value that was set is returned.
    /// If the swap operation fails (which can only happen due to its validator returning false),
    /// then a snapshot of the current value within the Atom is returned.
    /// If there is no validator for the Atom then the return value is always the snapshot of
    /// the successful `f` function. 
    /// </returns>
    public static IO<A> swapIO<M, A>(Atom<M, A> ma, Func<M, A, A> f) =>
        ma.SwapIO(f);
        
    /// <summary>
    /// Atomically updates the value by passing the old value to `f` and updating
    /// the atom with the result.  Note: `f` may be called multiple times, so it
    /// should be free of side effects.
    /// </summary>
    /// <param name="f">Function to update the atom</param>
    /// <returns>
    /// * If `f` returns `None` then no update occurs and the result of the call
    ///   to `Swap` will be the latest (unchanged) value of `A`.
    /// * If the swap operation fails, due to its validator returning false, then a snapshot of
    ///   the current value within the Atom is returned.
    /// * If the swap operation succeeded then a snapshot of the value that was set is returned.
    /// * If there is no validator for the Atom then the return value is always the snapshot of
    ///   the successful `f` function. 
    /// </returns>
    public static IO<A> swapIO<A>(Atom<A> ma, Func<A, Option<A>> f) =>
        ma.SwapMaybeIO(f);
        
    /// <summary>
    /// Atomically updates the value by passing the old value to `f` and updating
    /// the atom with the result.  Note: `f` may be called multiple times, so it
    /// should be free of side effects.
    /// </summary>
    /// <param name="f">Function to update the atom</param>
    /// <returns>
    /// * If `f` returns `None` then no update occurs and the result of the call
    ///   to `Swap` will be the latest (unchanged) value of `A`.
    /// * If the swap operation fails, due to its validator returning false, then a snapshot of
    ///   the current value within the Atom is returned.
    /// * If the swap operation succeeded then a snapshot of the value that was set is returned.
    /// * If there is no validator for the Atom then the return value is always the snapshot of
    ///   the successful `f` function. 
    /// </returns>
    public static IO<A> swapIO<M, A>(Atom<M, A> ma, Func<M, A, Option<A>> f) =>
        ma.SwapIO(f);

    /// <summary>
    /// Retrieve an IO computation that on each invocation will snapshot of the
    /// value in an atom
    /// </summary>
    /// <param name="ma">Atom to snapshot</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>IO representation of the snapshot</returns>
    public static IO<A> valueIO<A>(Atom<A> ma) =>
        ma.ValueIO;

    /// <summary>
    /// Retrieve an IO computation that on each invocation will snapshot of the
    /// value in an atom
    /// </summary>
    /// <param name="ma">Atom to snapshot</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>IO representation of the snapshot</returns>
    public static IO<A> valueIO<M, A>(Atom<M, A> ma) =>
        ma.ValueIO;

    /// <summary>
    /// Write a value to an atom.
    /// </summary>
    /// <remarks>
    /// Note, this can be dangerous and is usually better to use `swapIO` if the 
    /// `value` is derived from a snapshot of the atom's value (via `valueIO`).
    /// 
    /// The computation is better run inside the swap operation for atomic consistency.
    ///
    /// However, using `writeIO` is reasonable if this is simply a forceful overwrite
    /// of the atomic value without any dependency on the previous state.   
    /// </remarks>
    /// <param name="ma">Atom to write</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>IO representation of the write operation</returns>
    public static IO<A> writeIO<A>(Atom<A> ma, A value) =>
        ma.SwapIO(_ => value);

    /// <summary>
    /// Write a value to an atom.
    /// </summary>
    /// <remarks>
    /// Note, this can be dangerous and is usually better to use `swapIO` if the 
    /// `value` is derived from a snapshot of the atom's value (via `valueIO`).
    /// 
    /// The computation is better run inside the swap operation for atomic consistency.
    ///
    /// However, using `writeIO` is reasonable if this is simply a forceful overwrite
    /// of the atomic value without any dependency on the previous state.   
    /// </remarks>
    /// <param name="ma">Atom to write</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>IO representation of the write operation</returns>
    public static IO<A> writeIO<M, A>(Atom<M, A> ma, A value) =>
        ma.SwapIO((_, _) => value);
}
