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

    public static EnvIO New(
        Resources? resources = null,
        CancellationToken token = default,
        CancellationTokenSource? source = null,
        SynchronizationContext? syncContext = null,
        TimeSpan? timeout = null)
    {
        var own = 0;
        CancellationTokenRegistration? reg = null; 
        if (source is null)
        {
            source = timeout is null 
                         ? new CancellationTokenSource() 
                         : new CancellationTokenSource(timeout.Value);
            own |= 1;
        }

        if (resources is null)
        {
            resources = new Resources(null);
            own |= 2;
        }

        if ((own & 1) == 1)
        {
            if (token.CanBeCanceled)
            {
                reg = token.Register(() => source.Cancel());
            }

            token = source.Token;
        }

        syncContext ??= SynchronizationContext.Current;
        return new EnvIO(resources, token, source, syncContext, reg, own);
    }

    public EnvIO Local =>
        New(null, Token, null, SynchronizationContext.Current);

    public EnvIO LocalWithTimeout(TimeSpan timeout) =>
        New(null, Token, null, SynchronizationContext.Current, timeout);

    public EnvIO LocalResources =>
        New(null, Token, Source, SyncContext);

    public EnvIO LocalCancel =>
        New(Resources, Token, null, SyncContext);

    public EnvIO LocalSyncContext =>
        New(Resources, Token, Source, SynchronizationContext.Current);

    public EnvIO LocalCancelWithTimeout(TimeSpan timeout) =>
        New(Resources, Token, null, SyncContext, timeout);

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
    
