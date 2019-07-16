using System;
using System.Threading;
using System.Threading.Tasks;
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
        static Atom<HashMap<long, RefState>> state = Atom(HashMap<long, RefState>());
        static AsyncLocal<Transaction> transaction = new AsyncLocal<Transaction>();

        /// <summary>
        /// Generates a new reference that can be used within a dosync transaction
        /// </summary>
        internal static Ref<A> NewRef<A>(A value)
        {
            var id = Interlocked.Increment(ref refIdNext);
            var v = new RefState(0, value);
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
        internal static Task<R> DoTransactionAsync<R>(Func<Task<R>> op, Isolation isolation) =>
            transaction.Value == null
                ? RunTransactionAsync(op, isolation)
                : op();

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
                    var result = op();

                    // Attempt to apply the changes atomically
                    if (state.Swap(s =>
                    {
                        if (isolation == Isolation.Serialisable)
                        {
                            // Check if something else wrote to what we were reading
                            foreach (var read in t.reads)
                            {
                                if (s[read].Version != t.state[read].Version)
                                {
                                    throw new RetryException();
                                }
                            }
                        }

                        // Check if something else wrote to what we were writing
                        foreach (var write in t.writes)
                        {
                            var newState = t.state[write];
                            if (s[write].Version == newState.Version)
                            {
                                s = s.SetItem(write, new RefState(newState.Version + 1, newState.Value));
                            }
                            else
                            {
                                throw new RetryException();
                            }
                        }
                        return s;
                    }))
                    {
                        // Changes applied successfully
                        return result;
                    }
                    else
                    {
                        // Failed, so retry
                        retries--;
                    }
                }
                catch (RetryException)
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
        static async Task<R> RunTransactionAsync<R>(Func<Task<R>> op, Isolation isolation)
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
                    var result = await op();

                    // Attempt to apply the changes atomically
                    if (state.Swap(s =>
                    {
                        if (isolation == Isolation.Serialisable)
                        {
                            // Check if something else wrote to what we were reading
                            foreach (var read in t.reads)
                            {
                                if (s[read].Version != t.state[read].Version)
                                {
                                    throw new RetryException();
                                }
                            }
                        }

                        // Check if something else wrote to what we were writing
                        foreach (var change in t.writes)
                        {
                            var newState = t.state[change];
                            if (s[change].Version == newState.Version)
                            {
                                s = s.SetItem(change, new RefState(newState.Version + 1, newState.Value));
                            }
                            else
                            {
                                throw new RetryException();
                            }
                        }
                        return s;
                    }))
                    {
                        // Changes applied successfully
                        return result;
                    }
                    else
                    {
                        // Failed, so retry
                        retries--;
                    }
                }
                catch (RetryException)
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
        /// Read the value for the reference ID provided
        /// If within a transaction then the in-transaction value is returned, otherwise it's
        /// the current latest value
        /// </summary>
        internal static object Read(long id) =>
            transaction.Value == null
                ? state.Value[id].Value
                : transaction.Value.Read(id);

        /// <summary>
        /// Write the value for the reference ID provided
        /// Must be run within a transaction
        /// </summary>
        internal static void Write(long id, object value)
        {
            if (transaction.Value == null)
            {
                throw new InvalidOperationException("Refs can only be written to from within a transaction");
            }
            transaction.Value.Write(id, value);
        }

        /// <summary>
        /// Make sure Refs are cleaned up
        /// </summary>
        internal static void Finalise(long id) =>
            state.Swap(s => s.Remove(id));

        /// <summary>
        /// Retry exception for internal use
        /// </summary>
        class RetryException : Exception
        { }

        /// <summary>
        /// Transaction snapshot
        /// </summary>
        class Transaction
        {
            public HashMap<long, RefState> state;
            public HashSet<long> reads;
            public HashSet<long> writes;

            public Transaction(HashMap<long, RefState> state)
            {
                this.state = state;
                this.reads = HashSet<long>();
                this.writes = HashSet<long>();
            }

            public object Read(long id)
            {
                reads = reads.AddOrUpdate(id);
                return state[id].Value;
            }

            public void Write(long id, object value)
            {
                var oldState = state[id];
                var newState = new RefState(oldState.Version, value);
                state = state.SetItem(id, newState);
                writes = writes.AddOrUpdate(id);
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

            public RefState(long version, object value)
            {
                Version = version;
                Value = value;
            }
        }
    }
}
