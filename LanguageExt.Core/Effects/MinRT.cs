#nullable enable
using System.Threading;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Effects;

/// <summary>
/// Minimal runtime for running the non-runtime based IO monads
/// </summary>
public readonly struct MinRT : 
    HasIO<MinRT, Error>
{
    public MinRT(
        SynchronizationContext syncContext,
        CancellationTokenSource cancellationTokenSource,
        CancellationToken cancellationToken) =>
        (SynchronizationContext, CancellationTokenSource, CancellationToken) = 
            (syncContext, cancellationTokenSource, cancellationToken);

    public MinRT(
        SynchronizationContext syncContext,
        CancellationTokenSource cancellationTokenSource) =>
        (SynchronizationContext, CancellationTokenSource, CancellationToken) = 
            (syncContext, cancellationTokenSource, cancellationTokenSource.Token);

    public MinRT()
    {
        CancellationTokenSource = new CancellationTokenSource();
        CancellationToken = CancellationTokenSource.Token;
        SynchronizationContext = SynchronizationContext.Current;
    }

    public MinRT LocalCancel =>
        new (SynchronizationContext, CancellationTokenSource);
    
    public SynchronizationContext SynchronizationContext { get; }
    public CancellationToken CancellationToken { get; }
    public CancellationTokenSource CancellationTokenSource { get; }
    
    public Error FromError(Error error) => 
        error;

    public MinRT WithSyncContext(SynchronizationContext syncContext) =>
        new (syncContext, CancellationTokenSource);


}
