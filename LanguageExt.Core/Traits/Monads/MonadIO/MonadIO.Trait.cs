using System;
using System.Threading;
using LanguageExt.DSL;

namespace LanguageExt.Traits;

/// <summary>
/// Monad that is either the IO monad or a transformer with the IO monad in its stack
/// </summary>
/// <typeparam name="M">Self-referring trait</typeparam>
public interface MonadIO<M> : Monad<M>
    where M : MonadIO<M>
{
    public static virtual K<M, C> SelectMany<A, B, C>(K<M, A> ma, Func<A, IO<B>> bind, Func<A, B, C> project) =>
        ma.Bind(x => bind(x) switch
                     {
                         IOTail<B> tail when typeof(B) == typeof(C) => (IO<C>)(object)tail,
                         IOTail<B> => throw new NotSupportedException("Tail calls can't transform in the `select`"),
                         var mb => mb.Map(y => project(x, y))
                     });

    public static virtual K<M, C> SelectMany<A, B, C>(IO<A> ma, Func<A, K<M, B>> bind, Func<A, B, C> project) =>
        M.SelectMany(M.LiftIO(ma), bind, project);
    
    public static virtual K<M, EnvIO> EnvIO => 
        M.LiftIO(IO.env);
    
    public static virtual K<M, CancellationToken> Token => 
        M.LiftIO(IO.token);

    public static virtual K<M, CancellationTokenSource> TokenSource =>
        M.LiftIO(IO.source);

    public static virtual K<M, Option<SynchronizationContext>> SyncContext =>
        M.LiftIO(IO.syncContext);
    
}
