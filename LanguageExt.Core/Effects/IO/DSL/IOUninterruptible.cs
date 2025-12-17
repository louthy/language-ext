using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;
namespace LanguageExt.DSL;

record IOUninterruptible<A, B>(IO<A> Fa, Func<A, K<IO, B>> Next) : InvokeSyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOUninterruptible<A, C>(Fa, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOUninterruptible<A, C>(Fa, x => Next(x).Bind(f));

    public override IO<C> BindAsync<C>(Func<B, ValueTask<K<IO, C>>> f) => 
        new IOUninterruptible<A, C>(Fa, x => Next(x).As().BindAsync(f));

    public override IO<B> Invoke(EnvIO envIO)
    {
        var localEnv = EnvIO.New(envIO.Resources, CancellationToken.None, null, envIO.SyncContext);
        var tr = Fa.RunAsync(localEnv);
        return tr.IsCompleted
                   ? +Next(tr.Result)
                   : new IOUninterruptibleAsync<A, B>(tr, localEnv, Next);
    }
}

record IOUninterruptibleAsync<A, B>(ValueTask<A> Fa, EnvIO LocalEnv, Func<A, K<IO, B>> Next) : InvokeAsyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOUninterruptibleAsync<A, C>(Fa, LocalEnv, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOUninterruptibleAsync<A, C>(Fa, LocalEnv, x => Next(x).Bind(f));

    public override IO<C> BindAsync<C>(Func<B, ValueTask<K<IO, C>>> f) => 
        new IOUninterruptibleAsync<A, C>(Fa, LocalEnv, x => Next(x).As().BindAsync(f));

    public override async ValueTask<IO<B>> Invoke(EnvIO envIO)
    {
        var r = await Fa;
        LocalEnv.Dispose();
        return +Next(r);
    }
}
