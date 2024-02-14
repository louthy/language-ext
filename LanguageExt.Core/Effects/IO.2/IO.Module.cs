using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public partial class IO
{
    public static readonly IO<EnvIO> envIO = 
        IO<EnvIO>.LiftIO(ValueTask.FromResult);
    
    public static IO<A> asksIO<A>(Func<EnvIO, A> f) => 
        IO<A>.LiftIO(e => ValueTask.FromResult(f(e)));

    public static readonly IO<CancellationToken> token = 
        asksIO(e => e.Token);
    
    public static readonly IO<CancellationTokenSource> source = 
        asksIO(e => e.Source);
    
    public static readonly IO<Option<SynchronizationContext>> syncContext = 
        asksIO(e => Optional(e.SyncContext));
    
    public static IO<B> bind<A, B>(K<IO, A> ma, Func<A, K<IO, B>> f) =>
        ma.As().Bind(f);

    public static IO<B> map<A, B>(Func<A, B> f, K<IO, A> ma) => 
        ma.As().Map(f);

    public static IO<B> apply<A, B>(K<IO, Func<A, B>> mf, K<IO, A> ma) => 
        mf.As().Bind(ma.As().Map);

    public static IO<B> action<A, B>(K<IO, A> ma, K<IO, B> mb) =>
        ma.As().Bind(_ => mb);

    public static IO<A> empty<A>() =>
        IO<A>.Empty;

    public static IO<A> or<A>(K<IO, A> ma, K<IO, A> mb) => 
        ma.As() | mb.As();
}
