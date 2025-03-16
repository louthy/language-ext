using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.DSL;

record IOToken<A>(Func<CancellationToken, IO<A>> Next) :  InvokeSyncIO<A>
{
    public override IO<B> Map<B>(Func<A, B> f) => 
        new IOTokenMap<A, B>(Next, f);

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) => 
        new IOTokenBind<A, B>(Next, f);

    public override IO<B> BindAsync<B>(Func<A, ValueTask<K<IO, B>>> f) => 
        new IOTokenBindAsync<A, B>(Next, f);

    public override IO<A> Invoke(EnvIO envIO) =>
        Next(envIO.Token);
}

record IOTokenMap<A, B>(Func<CancellationToken, IO<A>> Next, Func<A, B> F) :  InvokeSyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOTokenMap<A, C>(Next, x => f(F(x)));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOTokenBind<A, C>(Next, x => f(F(x)));

    public override IO<C> BindAsync<C>(Func<B, ValueTask<K<IO, C>>> f) => 
        new IOTokenBindAsync<A, C>(Next, async x => await f(F(x)));

    public override IO<B> Invoke(EnvIO envIO) =>
        Next(envIO.Token).Map(F);
}

record IOTokenBind<A, B>(Func<CancellationToken, IO<A>> Next, Func<A, K<IO, B>> F) :  InvokeSyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOTokenBind<A, C>(Next, x => F(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOTokenBind<A, C>(Next, x => F(x).Bind(f));

    public override IO<C> BindAsync<C>(Func<B, ValueTask<K<IO, C>>> f) => 
        new IOTokenBind<A, C>(Next, x => F(x).As().BindAsync(f));

    public override IO<B> Invoke(EnvIO envIO) =>
        Next(envIO.Token).Bind(F);
}

record IOTokenBindAsync<A, B>(Func<CancellationToken, IO<A>> Next, Func<A, ValueTask<K<IO, B>>> F) :  InvokeSyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOTokenBindAsync<A, C>(Next, async x => (await F(x)).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOTokenBindAsync<A, C>(Next, async x => (await F(x)).Bind(f));

    public override IO<C> BindAsync<C>(Func<B, ValueTask<K<IO, C>>> f) => 
        new IOTokenBindAsync<A, C>(Next, async x => (await F(x)).As().BindAsync(f));

    public override IO<B> Invoke(EnvIO envIO) =>
        Next(envIO.Token).BindAsync(F);
}
