using System;

namespace LanguageExt
{
    /// <summary>
    /// `sync` transaction isolation level.  Used to enforce ACI properties
    /// of ACID on `Ref`s.
    /// </summary>
    /// <remarks>
    /// See the [concurrency section](https://github.com/louthy/language-ext/wiki/Concurrency) of the wiki for more info.
    /// </remarks>
    public enum Isolation
    {
        /// <remarks>
        /// Snapshot isolation requires that nothing outside of the transaction has written to any of the values that are
        /// *written-to within the transaction*.  If anything does write to the values used within the transaction, then
        /// the transaction is rolled back and retried (using the latest 'world' state). 
        /// </remarks>
        Snapshot,

        /// <remarks>
        /// Serialisable isolation requires that nothing outside of the transaction has written to any of the values that
        /// are *read-from or written-to within the transaction*.  If anything does write to the values that are used
        /// within the transaction, then it is rolled back and retried (using the latest 'world' state).
        ///
        /// It is the most strict form of isolation, and the most likely to conflict; but protects against cross read/write  
        /// inconsistencies.  For example, if you have:
        ///
        ///     var x = Ref(1);
        ///     var y = Ref(2);
        ///
        ///     snapshot(() => x.Value = y.Value + 1);
        ///
        /// Then something writing to `y` mid-way through the transaction would not cause the transaction to fail.
        /// Because `y` was only read-from, not written to.  However, this: 
        ///
        ///     var x = Ref(1);
        ///     var y = Ref(2);
        ///
        ///     serial(() => x.Value = y.Value + 1);
        ///
        /// ... would fail if something wrote to `y`.  
        /// </remarks>
        Serialisable
    }
}
