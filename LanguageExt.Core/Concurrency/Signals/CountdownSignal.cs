using System;
using System.Threading;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// IO signalling, usually used to signal for cross-thread synchronisation.
/// </summary>
public class CountdownSignal<M>(CountdownEvent handle) : IDisposable
    where M : Monad<M>
{
    /// <summary>
    /// The initial value of the counter
    /// </summary>
    public int Initial =>
        handle.InitialCount;
    
    /// <summary>
    /// The current value of the counter
    /// </summary>
    public K<M, int> Count =>
        M.LiftIO(IO.lift(() => handle.CurrentCount));

    /// <summary>
    /// True if the counter has complete its countdown
    /// </summary>
    public K<M, bool> Complete =>
        M.LiftIO(IO.lift(() => handle.IsSet));

    /// <summary>
    /// Triggers a single countdown of the counter in the signal
    /// </summary>
    /// <returns>True if the counter reached zero.</returns>
    public K<M, bool> Trigger() =>
        M.LiftIO(IO.lift(handle.Signal));

    /// <summary>
    /// Triggers a single countdown of the counter in the signal
    /// </summary>
    /// <returns>True if the counter reached zero.</returns>
    public bool TriggerUnsafe() =>
        handle.Signal();

    /// <summary>
    /// Triggers `n` signals to the counter in the signal
    /// </summary>
    /// <remarks>
    /// This is marked unsafe because it's not in an `IO` operation and therefore is impure.
    /// </remarks>
    /// <returns>True if the counter reached zero.</returns>
    public K<M, bool> Trigger(int count) =>
        M.LiftIO(IO.lift(handle.Signal));

    /// <summary>
    /// Triggers `n` signals to the counter in the signal
    /// </summary>
    /// <remarks>
    /// This is marked unsafe because it's not in an `IO` operation and therefore is impure.
    /// </remarks>
    /// <returns>True if the counter reached zero.</returns>
    public bool TriggerUnsafe(int count) =>
        handle.Signal();

    /// <summary>
    /// Wait for signal to signal
    /// </summary>
    /// <returns>Monad `M` with the `Wait` operation lifted into it via `liftIO`</returns>
    public K<M, Unit> Wait() =>
        M.LiftIO(IO.lift(e =>
                         {
                             handle.Wait(e.Token);
                             return unit;
                         }));

    public void Dispose() => 
        handle.Dispose();
}
