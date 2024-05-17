using System;
using System.Threading;

namespace LanguageExt;

/// <summary>
/// Environment for the IO monad 
/// </summary>
public class EnvIO : IDisposable
{
    public readonly Resources Resources;
    public readonly CancellationToken Token;
    public readonly CancellationTokenSource Source;
    public readonly SynchronizationContext? SyncContext;
    readonly int Own;

    EnvIO(Resources resources,
          CancellationToken token,
          CancellationTokenSource source,
          SynchronizationContext? syncContext,
          int own)
    {
        Resources   = resources;
        Token       = token;
        Source      = source;
        SyncContext = syncContext;
        Own         = own;
    }

    public static EnvIO New(
        Resources? resources = null,
        CancellationToken token = default,
        CancellationTokenSource? source = null,
        SynchronizationContext? syncContext = null)
    {
        var own = 0;
        if (source is null)
        {
            source =  new CancellationTokenSource();
            own    |= 1;
        }

        if (resources is null)
        {
            resources =  new Resources(null);
            own       |= 2;
        }

        token       =   token.CanBeCanceled ? token : source.Token;
        syncContext ??= SynchronizationContext.Current;
        return new EnvIO(resources, token, source, syncContext, own);
    }

    public EnvIO LocalResources =>
        New(null, Token, Source, SyncContext);

    public EnvIO LocalCancel =>
        New(Resources, default, null, SyncContext);

    public EnvIO LocalSyncContext =>
        New(Resources, Token, Source, SynchronizationContext.Current);

    public void Dispose()
    {
        if ((Own & 2) == 2) Resources.DisposeU(this);
        if ((Own & 1) == 1) Source.Dispose();
    }

    public override string ToString() => 
        "EnvIO";
}
    
