#nullable enable
using System.Threading;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Effects;

/// <summary>
/// Minimal runtime for running the non-runtime based IO monads
/// </summary>
public readonly struct MinimalRT : 
    HasIO<MinimalRT, Error>
{
    public MinimalRT(
        SynchronizationContext syncContext,
        CancellationTokenSource cancellationTokenSource,
        CancellationToken cancellationToken) =>
        (SynchronizationContext, CancellationTokenSource, CancellationToken) = 
            (syncContext, cancellationTokenSource, cancellationToken);

    public MinimalRT(
        SynchronizationContext syncContext,
        CancellationTokenSource cancellationTokenSource) =>
        (SynchronizationContext, CancellationTokenSource, CancellationToken) = 
            (syncContext, cancellationTokenSource, cancellationTokenSource.Token);

    public MinimalRT()
    {
        CancellationTokenSource = new CancellationTokenSource();
        CancellationToken = CancellationTokenSource.Token;
        SynchronizationContext = SynchronizationContext.Current;
    }

    public MinimalRT LocalCancel =>
        new (SynchronizationContext, CancellationTokenSource);
    
    public SynchronizationContext SynchronizationContext { get; }
    public CancellationToken CancellationToken { get; }
    public CancellationTokenSource CancellationTokenSource { get; }
    
    public Error FromError(Error error) => 
        error;

    public MinimalRT WithSyncContext(SynchronizationContext syncContext) =>
        new (syncContext, CancellationTokenSource);


}
