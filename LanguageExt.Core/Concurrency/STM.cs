using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.ClassInstances;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Software transactional memory using Multi-Version Concurrency Control (MVCC)
    /// </summary>
    public static class STM
    {
        const int maxRetries = 500;
        static long refIdNext;
        static readonly Atom<TrieMap<EqLong, long, RefState>> state = Atom(TrieMap<EqLong, long, RefState>.Empty);
        static readonly AsyncLocal<Transaction> transaction = new AsyncLocal<Transaction>();

        /// <summary>
        /// Generates a new reference that can be used within a sync transaction
        /// </summary>
        internal static Ref<A> NewRef<A>(A value, Func<A, bool> validator = null)
        {
            var valid = validator == null
                ? True
                : CastPredicate(validator);

            var id = Interlocked.Increment(ref refIdNext);
            var v = new RefState(0, value, valid);
            state.Swap(s => s.Add(id, v));
            return new Ref<A>(id);
        }

        /// <summary>
        /// Run the op within a new transaction
        /// If a transaction is already running, then this becomes part of the parent transaction
        /// </summary>
        internal static R DoTransaction<R>(Func<R> op, Isolation isolation) =>
            transaction.Value == null
                ? RunTransaction(op, isolation)
                : op();

        /// <summary>
        /// Run the op within a new transaction
        /// If a transaction is already running, then this becomes part of the parent transaction
        /// </summary>
        internal static ValueTask<R> DoTransaction<R>(Func<ValueTask<R>> op, Isolation isolation) =>
            transaction.Value == null
                ? RunTransaction(op, isolation)
                : op();

        /// <summary>
        /// Run the op within a new transaction
        /// If a transaction is already running, then this becomes part of the parent transaction
        /// </summary>
        internal static Aff<R> DoTransaction<R>(Aff<R> op, Isolation isolation) =>
            transaction.Value == null
                ? RunTransaction(op, isolation)
                : op;

        /// <summary>
        /// Run the op within a new transaction
        /// If a transaction is already running, then this becomes part of the parent transaction
        /// </summary>
        internal static Aff<RT, R> DoTransaction<RT, R>(Aff<RT, R> op, Isolation isolation) where RT : struct, HasCancel<RT> =>
            transaction.Value == null
                ? RunTransaction(op, isolation)
                : op;

        /// <summary>
        /// Run the op within a new transaction
        /// If a transaction is already running, then this becomes part of the parent transaction
        /// </summary>
        internal static Eff<R> DoTransaction<R>(Eff<R> op, Isolation isolation) =>
            transaction.Value == null
                ? RunTransaction(op, isolation)
                : op;
        
        /// <summary>
        /// Run the op within a new transaction
        /// If a transaction is already running, then this becomes part of the parent transaction
        /// </summary>
        internal static Eff<RT, R> DoTransaction<RT, R>(Eff<RT, R> op, Isolation isolation) where RT : struct =>
            transaction.Value == null
                ? RunTransaction(op, isolation)
                : op;
        
        /// <summary>
        /// Run the op within a new transaction
        /// If a transaction is already running, then this becomes part of the parent transaction
        /// </summary>
        internal static R DoTransaction<R>(Func<CommuteRef<R>> op, Isolation isolation) =>
            transaction.Value == null
                ? RunTransaction(op, isolation)
                : op().Value;

        /// <summary>
        /// Runs the transaction
        /// </summary>
        static R RunTransaction<R>(Func<R> op, Isolation isolation)
        {
            var retries = maxRetries;
            while (retries > 0)
            {
                // Create a new transaction with a snapshot of the current state
                var t = new Transaction(state.Value);
                transaction.Value = t;
                try
                {
                    // Try to do the operations of the transaction
                    return ValidateAndCommit(t, isolation, op(), long.MinValue);
                }
                catch (ConflictException)
                {
                    // Conflict found, so retry
                    retries--;
                }
                finally
                {
                    // Clear the current transaction on the way out
                    transaction.Value = null;
                }
                // Wait one tick before trying again
                SpinWait sw = default;
                sw.SpinOnce();
            }
            throw new DeadlockException();
        }
        
        /// <summary>
        /// Runs the transaction
        /// </summary>
        static Aff<RT, R> RunTransaction<RT, R>(Aff<RT, R> op, Isolation isolation) where RT : struct, HasCancel<RT> =>
            AffMaybe<RT, R>(async env =>
            {
                var retries = maxRetries;
                while (retries > 0)
                {
                    // Create a new transaction with a snapshot of the current state
                    var t = new Transaction(state.Value);
                    transaction.Value = t;
                    try
                    {
                        // Try to do the operations of the transaction
                        var res = await op.ReRun(env).ConfigureAwait(false);
                        return res.IsFail 
                                   ? res 
                                   : ValidateAndCommit(t, isolation, res.Value, Int64.MinValue);
                    }
                    catch (ConflictException)
                    {
                        // Conflict found, so retry
                        retries--;
                    }
                    finally
                    {
                        // Clear the current transaction on the way out
                        transaction.Value = null;
                    }

                    // Wait one tick before trying again
                    SpinWait sw = default;
                    sw.SpinOnce();
                }

                throw new DeadlockException();
            });
                
        /// <summary>
        /// Runs the transaction
        /// </summary>
        static Eff<RT, R> RunTransaction<RT, R>(Eff<RT, R> op, Isolation isolation) where RT : struct =>
            EffMaybe<RT, R>(env =>
            {
                var retries = maxRetries;
                while (retries > 0)
                {
                    // Create a new transaction with a snapshot of the current state
                    var t = new Transaction(state.Value);
                    transaction.Value = t;
                    try
                    {
                        // Try to do the operations of the transaction
                        var res = op.ReRun(env);
                        return res.IsFail 
                                   ? res 
                                   : ValidateAndCommit(t, isolation, res.Value, Int64.MinValue);
                    }
                    catch (ConflictException)
                    {
                        // Conflict found, so retry
                        retries--;
                    }
                    finally
                    {
                        // Clear the current transaction on the way out
                        transaction.Value = null;
                    }

                    // Wait one tick before trying again
                    SpinWait sw = default;
                    sw.SpinOnce();
                }

                throw new DeadlockException();
            });

        /// <summary>
        /// Runs the transaction
        /// </summary>
        static Aff<R> RunTransaction<R>(Aff<R> op, Isolation isolation) =>
            AffMaybe(async () =>
            {
                var retries = maxRetries;
                while (retries > 0)
                {
                    // Create a new transaction with a snapshot of the current state
                    var t = new Transaction(state.Value);
                    transaction.Value = t;
                    try
                    {
                        // Try to do the operations of the transaction
                        var res = await op.ReRun().ConfigureAwait(false);
                        return res.IsFail 
                                   ? res 
                                   : ValidateAndCommit(t, isolation, res.Value, Int64.MinValue);
                    }
                    catch (ConflictException)
                    {
                        // Conflict found, so retry
                        retries--;
                    }
                    finally
                    {
                        // Clear the current transaction on the way out
                        transaction.Value = null;
                    }

                    // Wait one tick before trying again
                    SpinWait sw = default;
                    sw.SpinOnce();
                }

                throw new DeadlockException();
            });

        /// <summary>
        /// Runs the transaction
        /// </summary>
        static Eff<R> RunTransaction<R>(Eff<R> op, Isolation isolation) =>
            EffMaybe(() =>
            {
                var retries = maxRetries;
                while (retries > 0)
                {
                    // Create a new transaction with a snapshot of the current state
                    var t = new Transaction(state.Value);
                    transaction.Value = t;
                    try
                    {
                        // Try to do the operations of the transaction
                        var res = op.ReRun();
                        return res.IsFail 
                                   ? res 
                                   : ValidateAndCommit(t, isolation, res.Value, Int64.MinValue);
                    }
                    catch (ConflictException)
                    {
                        // Conflict found, so retry
                        retries--;
                    }
                    finally
                    {
                        // Clear the current transaction on the way out
                        transaction.Value = null;
                    }

                    // Wait one tick before trying again
                    SpinWait sw = default;
                    sw.SpinOnce();
                }

                throw new DeadlockException();
            });
        
        /// <summary>
        /// Runs the transaction
        /// </summary>
        static async ValueTask<R> RunTransaction<R>(Func<ValueTask<R>> op, Isolation isolation)
        {
            var retries = maxRetries;
            while (retries > 0)
            {
                // Create a new transaction with a snapshot of the current state
                var t = new Transaction(state.Value);
                transaction.Value = t;
                try
                {
                    // Try to do the operations of the transaction
                    return ValidateAndCommit(t, isolation, await op().ConfigureAwait(false), long.MinValue);
                }
                catch (ConflictException)
                {
                    // Conflict found, so retry
                    retries--;
                }
                finally
                {
                    // Clear the current transaction on the way out
                    transaction.Value = null;
                }
                // Wait one tick before trying again
                SpinWait sw = default;
                sw.SpinOnce();
            }
            throw new DeadlockException();
        }

        /// <summary>
        /// Runs the transaction
        /// </summary>
        static R RunTransaction<R>(Func<CommuteRef<R>> op, Isolation isolation)
        {
            var retries = maxRetries;
            while (retries > 0)
            {
                // Create a new transaction with a snapshot of the current state
                var t = new Transaction(state.Value);
                transaction.Value = t;
                try
                {
                    var cref = op();

                    // Try to do the operations of the transaction
                    return ValidateAndCommit<R>(t, isolation, (R)t.state[cref.Ref.Id].Value, cref.Ref.Id);
                }
                catch (ConflictException)
                {
                    // Conflict found, so retry
                    retries--;
                }
                finally
                {
                    // Clear the current transaction on the way out
                    transaction.Value = null;
                }
                // Wait one tick before trying again
                SpinWait sw = default;
                sw.SpinOnce();
            }
            throw new DeadlockException();
        }

        static R ValidateAndCommit<R>(Transaction t, Isolation isolation, R result, long returnRefId)
        {
            // No writing, so no validation or commit needed
            if (t.writes.Count == 0 && t.commutes.Count == 0)
            {
                return result;
            }

            // Attempt to apply the changes atomically
            state.Swap(s =>
            {
                if (isolation == Isolation.Serialisable)
                {
                    ValidateReads(t, s, isolation);
                }

                s = CommitWrites(t, s);
                (s, result) = CommitCommutes(t, s, returnRefId, result);
                return s;
            });

            // Changes applied successfully
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ValidateReads(Transaction t, TrieMap<EqLong, long, RefState> s, Isolation isolation)
        {
            // Check if something else wrote to what we were reading
            foreach (var read in t.reads)
            {
                if (s[read].Version != t.state[read].Version)
                {
                    throw new ConflictException();
                }
            }
        }

        static TrieMap<EqLong, long, RefState> CommitWrites(Transaction t, TrieMap<EqLong, long, RefState> s)
        {
            // Check if something else wrote to what we were writing
            foreach (var write in t.writes)
            {
                var newState = t.state[write];

                if (!newState.Validator(newState.Value))
                {
                    throw new RefValidationFailedException();
                }

                if (s[write].Version == newState.Version)
                {
                    s = s.SetItem(write, new RefState(newState.Version + 1, newState.Value, newState.Validator));
                }
                else
                {
                    throw new ConflictException();
                }
            }

            return s;
        }

        static (TrieMap<EqLong, long, RefState>, R) CommitCommutes<R>(Transaction t, TrieMap<EqLong, long, RefState> s, long returnRefId, R result)
        {
            // Run the commutative operations
            foreach (var commute in t.commutes)
            {
                var exist = s[commute.Id];

                // Re-run the commute function with what's live now
                var nval = commute.Fun(exist.Value);
                var nver = new RefState(exist.Version + 1, nval, exist.Validator);

                // Validate the result
                if (!nver.Validator(nver.Value))
                {
                    throw new RefValidationFailedException();
                }

                // Save to live state
                s = s.SetItem(commute.Id, nver);

                // If it matches our return type, then make it the result
                if (returnRefId == commute.Id)
                {
                    result = (R)nval;
                }
            }

            return (s, result);
        }

        /// <summary>
        /// Read the value for the reference ID provided
        /// If within a transaction then the in-transaction value is returned, otherwise it's
        /// the current latest value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static object Read(long id) =>
            transaction.Value == null
                ? state.Value[id].Value
                : transaction.Value.Read(id);

        /// <summary>
        /// Write the value for the reference ID provided
        /// Must be run within a transaction
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Write(long id, object value)
        {
            if (transaction.Value == null)
            {
                throw new InvalidOperationException("Refs can only be written to from within a `sync` transaction");
            }
            transaction.Value.Write(id, value);
        }

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
        /// Commute allows for more concurrency than just setting the items
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static A Commute<A>(long id, Func<A, A> f)
        {
            if (transaction.Value == null)
            {
                throw new InvalidOperationException("Refs can only commute from within a transaction");
            }
            return (A)transaction.Value.Commute(id, CastCommute(f));
        }

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
        /// Commute allows for more concurrency than just setting the items
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static A Commute<X, A>(long id, X x, Func<X, A, A> f) =>
            Commute<A>(id, (a => f(x, a)));

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
        /// Commute allows for more concurrency than just setting the items
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static A Commute<X, Y, A>(long id, X x, Y y, Func<X, Y, A, A> f) =>
            Commute<A>(id, (a => f(x, y, a)));

        /// <summary>
        /// Make sure Refs are cleaned up
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Finalise(long id) =>
            state.Swap(s => s.Remove(id));

        /// <summary>
        /// Conflict exception for internal use
        /// </summary>
        class ConflictException : Exception
        { }

        /// <summary>
        /// Predicate that always returns true
        /// </summary>
        static readonly Func<object, bool> True =
            _ => true;

        /// <summary>
        /// Wraps a (A -> bool) predicate as (object -> bool)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<object, bool> CastPredicate<A>(Func<A, bool> validator) =>
            obj => validator((A)obj);

        /// <summary>
        /// Wraps a (A -> A) predicate as (object -> object)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Func<object, object> CastCommute<A>(Func<A, A> f) =>
            obj => (A)f((A)obj);

        /// <summary>
        /// Get the currently running TransactionId
        /// </summary>
        public static long TransactionId
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => transaction.Value?.transactionId ?? throw new InvalidOperationException("Transaction not running");
        }

        /// <summary>
        /// Transaction snapshot
        /// </summary>
        class Transaction
        {
            static long transactionIdNext;
            public readonly long transactionId;
            public TrieMap<EqLong, long, RefState> state;
            public readonly System.Collections.Generic.HashSet<long> reads = new();
            public readonly System.Collections.Generic.HashSet<long> writes = new();
            public readonly System.Collections.Generic.List<(long Id, Func<object, object> Fun)> commutes = new();

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Transaction(TrieMap<EqLong, long, RefState> state)
            {
                this.state = state;
                transactionId = Interlocked.Increment(ref transactionIdNext);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public object Read(long id)
            {
                reads.Add(id);
                return state[id].Value;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public object Write(long id, object value)
            {
                var oldState = state[id];
                var newState = new RefState(oldState.Version, value, oldState.Validator);
                state = state.SetItem(id, newState);
                writes.Add(id);
                return value;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public object Commute(long id, Func<object, object> f)
            {
                var oldState = state[id];
                var nvalue = f(oldState.Value);
                var newState = new RefState(oldState.Version, nvalue, oldState.Validator);
                state = state.SetItem(id, newState);
                commutes.Add((id, f));
                return nvalue;
            }
        }

        /// <summary>
        /// The state of a Ref
        /// Includes the value and the version
        /// </summary>
        class RefState
        {
            public readonly long Version;
            public readonly object Value;
            public readonly Func<object, bool> Validator;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public RefState(long version, object value, Func<object, bool> validator)
            {
                Version = version;
                Value = value;
                Validator = validator;
            }
        }
    }
}
