using System;
using System.Threading;

namespace LanguageExt;

/// <summary>
/// Environment for the IO monad 
/// </summary>
public record EnvIO(
    CancellationToken Token,
    CancellationTokenSource Source,
    SynchronizationContext? SyncContext) : IDisposable
{
    public static EnvIO New(
        CancellationToken token,
        CancellationTokenSource source,
        SynchronizationContext? syncContext) =>
        new (token, source, syncContext);
    
    public static EnvIO New()
    {
        var src = new CancellationTokenSource();
        return new EnvIO(src.Token, src, SynchronizationContext.Current);
    }
    
    public static EnvIO New(CancellationTokenSource src) =>
        new (src.Token, src, SynchronizationContext.Current);

    public EnvIO LocalCancel
    {
        get
        {
            var src = new CancellationTokenSource();
            return new EnvIO(src.Token, src, SyncContext);
        }
    }

    public EnvIO LocalSyncContext =>
        this with { SyncContext = SynchronizationContext.Current };

    public void Dispose() => 
        Source.Dispose();
}
    
