using System;
using System.Threading;

namespace LanguageExt;

/// <summary>
/// Environment for the IO monad 
/// </summary>
public record EnvIO(
    Resources Resources,
    CancellationToken Token,
    CancellationTokenSource Source,
    SynchronizationContext? SyncContext) : IDisposable
{
    public static EnvIO New(
        Resources resources,
        CancellationToken token,
        CancellationTokenSource source,
        SynchronizationContext? syncContext) =>
        new (resources, token, source, syncContext);
    
    /*
    public static EnvIO New()
    {
        var src = new CancellationTokenSource();
        return new EnvIO(new Resources(null), src.Token, src, SynchronizationContext.Current);
    }
    
    public static EnvIO New(CancellationTokenSource src) =>
        new (new Resources(null), src.Token, src, SynchronizationContext.Current);
        */

    public EnvIO LocalResources =>
        this with { Resources = new Resources(Resources) };

    public EnvIO LocalCancel
    {
        get
        {
            var src = new CancellationTokenSource();
            return this with { Token = src.Token, Source = src };
        }
    }

    public EnvIO LocalSyncContext =>
        this with { SyncContext = SynchronizationContext.Current };

    public void Dispose() => 
        Source.Dispose();

    public override string ToString() => 
        "EnvIO";
}
    
