using System.Threading;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

/// <summary>
/// IO signalling constructors
/// </summary>
public static class Signal
{
    /// <summary>Represents a thread synchronisation event that, when signaled, resets automatically after releasing a
    /// single waiting thread.</summary>
    /// <remarks>
    /// The internal `EventWaitHandle` is wrapped with a `use` operator so that its handle is tracked by the `IO`
    /// resource-management system and auto-cleaned up if not done manually with `release` or using `bracket`.
    /// </remarks>
    /// <param name="signaled">
    /// <see langword="true" /> to set the initial state to signaled;
    /// <see langword="false" /> to set the initial state to non-signaled.
    /// </param>
    public static K<M, Signal<M>> autoReset<M>(bool signaled = false)
        where M : Monad<M> =>
        M.LiftIOMaybe(use(() => new Signal<M>(new AutoResetEvent(false))));

    /// <summary>Represents a thread synchronisation event that, when signaled, must be reset manually. </summary>
    /// <remarks>
    /// The internal `EventWaitHandle` is wrapped with a `use` operator so that its handle is tracked by the `IO`
    /// resource-management system and auto-cleaned up if not done manually with `release` or using `bracket`.
    /// </remarks>
    /// <param name="signaled">
    /// <see langword="true" /> to set the initial state signaled;
    /// <see langword="false" /> to set the initial state to non-signaled.
    /// </param>
    public static K<M, Signal<M>> manualReset<M>(bool signaled = false) 
        where M : Monad<M> =>
        M.LiftIOMaybe(use(() => new Signal<M>(new ManualResetEvent(false))));

    /// <summary>Represents a synchronisation primitive that is signaled when its count reaches zero.</summary>
    /// <remarks>
    /// The internal `EventWaitHandle` is wrapped with a `use` operator so that its handle is tracked by the `IO`
    /// resource-management system and auto-cleaned up if not done manually with `release` or using `bracket`.
    /// </remarks>
    public static K<M, CountdownSignal<M>> countdown<M>(int count) 
        where M : Monad<M> =>
        M.LiftIOMaybe(use(() => new CountdownSignal<M>(new CountdownEvent(count))));
}
