using System;
using System.Threading;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// IO signalling, usually used to signal for cross-thread synchronisation.
/// </summary>
public class Signal<M>(EventWaitHandle handle) : IDisposable
    where M : Monad<M>
{
    /// <summary>
    /// Set the signal
    /// </summary>
    /// <returns>
    /// True if the operation succeeds
    /// </returns>
    public K<M, bool> Trigger() =>
        M.LiftIOMaybe(IO.lift(_ => handle.Set()));
    
    /// <summary>
    /// Set the signal
    /// </summary>
    /// <remarks>
    /// This is marked unsafe because it's not in an `IO` operation and therefore is impure.
    /// </remarks>
    /// <returns>
    /// True if the operation succeeds
    /// </returns>
    public bool TriggerUnsafe() =>
        handle.Set();
    
    /// <summary>
    /// Wait for signal to signal
    /// </summary>
    /// <returns></returns>
    public K<M, bool> Wait() =>
        M.LiftIOMaybe(IO.liftAsync(e => handle.WaitOneAsync(e.Token)));
    
    /// <summary>
    /// Wait for signal to signal
    /// </summary>
    /// <returns></returns>
    public K<M, bool> Wait(TimeSpan timeout) =>
        M.LiftIOMaybe(IO.liftAsync(e => handle.WaitOneAsync(timeout, e.Token)));
    
    /// <summary>
    /// Wait for signal to signal
    /// </summary>
    /// <returns></returns>
    public K<M, bool> Wait(int timeoutMilliseconds) =>
        M.LiftIOMaybe(IO.liftAsync(e => handle.WaitOneAsync(timeoutMilliseconds, e.Token)));
    
    public void Dispose() => 
        handle.Dispose();
}
