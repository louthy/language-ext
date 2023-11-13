#nullable enable
using System.Threading;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Effects;

/// <summary>
/// Minimal runtime for running the non-runtime based IO monads
/// </summary>
public readonly struct MinimalRT : 
    HasCancel<MinimalRT>,
    HasFromError<MinimalRT, Error>
{
    public MinimalRT(
        CancellationTokenSource cancellationTokenSource,
        CancellationToken cancellationToken) =>
        (CancellationTokenSource, CancellationToken) = (cancellationTokenSource, cancellationToken);

    public MinimalRT(
        CancellationTokenSource cancellationTokenSource) =>
        (CancellationTokenSource, CancellationToken) = (cancellationTokenSource, cancellationTokenSource.Token);
    
    public MinimalRT LocalCancel =>
        new MinimalRT(CancellationTokenSource);
    
    public CancellationToken CancellationToken { get; }
    
    public CancellationTokenSource CancellationTokenSource { get; }
    
    public Error FromError(Error error) => 
        error;
}
