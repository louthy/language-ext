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
        internal static R DoTransaction<R>(Func<R> op) =>
            transaction.Value == null
                ? RunTransaction(op)
                : op();

        /// <summary>
        /// Run the op within a new transaction
        /// If a transaction is already running, then this becomes part of the parent transaction
        /// </summary>
        internal static Task<R> DoTransactionAsync<R>(Func<Task<R>> op) =>
            transaction.Value == null
                ? RunTransactionAsync(op)
                : op();

        /// <summary>
        /// Runs the transaction
        /// </summary>
        static R RunTransaction<R>(Func<R> op)
        {
            var retries = maxRetries;
            while (retries > 0)
            {
                // Create a new transaction with a snapshot of the current state
                var t = new Transaction(state.Value, HashSet<long>());
                transaction.Value = t;
                try
                {
                    // Try to do the operations of the transaction
                    var result = op();

                    // Attempt to apply the changes atomically
                    if (state.Swap(s =>
                    {
                        foreach (var change in t.changes)
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
            throw new Exception("dosync failed - maximum retries reached");
        }

        /// <summary>
        /// Runs the transaction
        /// </summary>
        static async Task<R> RunTransactionAsync<R>(Func<Task<R>> op)
        {
            var retries = maxRetries;
            while (retries > 0)
            {
                // Create a new transaction with a snapshot of the current state
                var t = new Transaction(state.Value, HashSet<long>());
                transaction.Value = t;
                try
                {
                    // Try to do the operations of the transaction
                    var result = await op();

                    // Attempt to apply the changes atomically
                    if (state.Swap(s =>
                    {
                        foreach (var change in t.changes)
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
            throw new Exception("dosync failed - maximum retries reached");
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
                throw new Exception("Refs can only be read from within a transaction");
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
            public HashSet<long> changes;

            public Transaction(HashMap<long, RefState> state, HashSet<long> changes)
            {
                this.state = state;
                this.changes = changes;
            }

            public object Read(long id) =>
                state[id].Value;

            public void Write(long id, object value)
            {
                var oldState = state[id];
                var newState = new RefState(oldState.Version, value);
                state = state.SetItem(id, newState);
                changes = changes.AddOrUpdate(id);
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
