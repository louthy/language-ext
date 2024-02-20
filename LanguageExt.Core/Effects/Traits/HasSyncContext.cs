/*
// TODO: Decide whether to keep this
    
using System.Threading;
using LanguageExt.Attributes;

namespace LanguageExt.Effects.Traits;

/// <summary>
/// Type-class giving a struct the trait of supporting thread synchronisation context
/// </summary>
/// <typeparam name="RT">Runtime</typeparam>
[Trait("*")]
public interface HasSyncContext<out RT>
    where RT : HasSyncContext<RT>
{
    /// <summary>
    /// Creates a new runtime from this with a new SyncContext and token
    /// </summary>
    /// <remarks>This is for sub-systems to run in their own local `SynchronizationContext` contexts</remarks>
    /// <returns>New runtime</returns>
    RT WithSyncContext(SynchronizationContext? syncContext);

    /// <summary>
    /// Directly access the SynchronizationContext
    /// </summary>
    /// <returns>SynchronizationContext</returns>
    SynchronizationContext? SynchronizationContext { get; }
}
*/
