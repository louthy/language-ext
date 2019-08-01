using System;

namespace LanguageExt
{
    /// <summary>
    /// `dosync` transaction isolation level.  Used to enforce ACI properties
    /// of ACID on `Ref`s.
    /// </summary>
    public enum Isolation
    {
        /// <summary>
        /// Snapshot isolation takes a picture of the world when it starts
        /// If the transaction writes to a `Ref` that something else modified
        /// in the meantime then the operation will be retried with the latest
        /// world.
        /// </summary>
        Snapshot,

        /// <summary>
        /// Most strict form of isolation.  Causes a transaction to fail and 
        /// re-run even if a `Ref` that is read within the transaction is 
        /// changed (by another transaction).  
        /// </summary>
        Serialisable
    }
}
