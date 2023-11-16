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
        CancellationTokenSource cancellationTokenSource,
        CancellationToken cancellationToken) =>
        (CancellationTokenSource, CancellationToken) = (cancellationTokenSource, cancellationToken);

    public MinimalRT(
        CancellationTokenSource cancellationTokenSource) =>
        (CancellationTokenSource, CancellationToken) = (cancellationTokenSource, cancellationTokenSource.Token);

    public MinimalRT()
    {
        CancellationTokenSource = new CancellationTokenSource();
        CancellationToken = CancellationTokenSource.Token;
    }

    public MinimalRT LocalCancel =>
        new MinimalRT(CancellationTokenSource);
    
    public CancellationToken CancellationToken { get; }
    
    public CancellationTokenSource CancellationTokenSource { get; }
    
    public Error FromError(Error error) => 
        error;
}
