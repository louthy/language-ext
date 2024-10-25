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
    private readonly CancellationTokenRegistration? Registration;
    readonly int Own;
    int disposed;

    EnvIO(Resources resources,
          CancellationToken token,
          CancellationTokenSource source,
          SynchronizationContext? syncContext,
          CancellationTokenRegistration? registration,
          int own)
    {
        Resources    = resources;
        Token        = token;
        Source       = source;
        SyncContext  = syncContext;
        Registration = registration;
        Own          = own;
    }

    /// <summary>
    /// Creates a new EnvIO and registers it with the `token` provided, so if the `token` gets
    /// a cancellation signal then so will any computation that uses the `EnvIO`. 
    /// </summary>
    /// <param name="token">Token to register with</param>
    /// <returns>Constructed EnvIO</returns>
    public static EnvIO FromToken(CancellationToken token)
    {
        if (!token.CanBeCanceled) return New();
        var       nsource    = new CancellationTokenSource();
        var       ntoken     = nsource.Token;
        var       ncontext   = SynchronizationContext.Current;
        using var nreg       = token.Register(() => nsource.Cancel());
        var       nresources = new Resources(null);
        var       own        = 3;
        return new EnvIO(nresources, ntoken, nsource, ncontext, nreg, own);
    }

    /// <summary>
    /// Creates a new EnvIO and registers it with the `token` provided, so if the `token` gets
    /// a cancellation signal then so will any computation that uses the `EnvIO`. 
    /// </summary>
    /// <param name="resources">Resources collection to use instead of letting EnvIO construct one</param>
    /// <param name="token">Token to register with</param>
    /// <returns>Constructed EnvIO</returns>
    public static EnvIO FromToken(Resources resources, CancellationToken token)
    {
        if (!token.CanBeCanceled) return New(resources);
        var       nsource    = new CancellationTokenSource();
        var       ntoken     = nsource.Token;
        var       ncontext   = SynchronizationContext.Current;
        using var nreg       = token.Register(() => nsource.Cancel());
        var       own        = 1;
        return new EnvIO(resources, ntoken, nsource, ncontext, nreg, own);
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
        return new EnvIO(resources, token, source, syncContext, null, own);
    }

    public EnvIO LocalResources =>
        New(null, Token, Source, SyncContext);

    public EnvIO LocalCancel =>
        New(Resources, default, null, SyncContext);

    public EnvIO LocalSyncContext =>
        New(Resources, Token, Source, SynchronizationContext.Current);

    public void Dispose()
    {
        if (Interlocked.CompareExchange(ref disposed, 1, 0) == 0)
        {
            if ((Own & 2) == 2) Resources.DisposeU(this);
            if ((Own & 1) == 1) Source.Dispose();
            Registration?.Dispose();
        }
    }

    public override string ToString() => 
        "EnvIO";
}
    
