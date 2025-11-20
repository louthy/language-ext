using System;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.DSL;

record IOUse<X, A>(IO<X> Acquire, Func<X, IO<Unit>> Release, Func<X, IO<A>> Next) : InvokeSyncIO<A>
{
    public override IO<B> Map<B>(Func<A, B> f) => 
        new IOUse<X, B>(Acquire, Release, x => Next(x).Map(f));

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) => 
        new IOUse<X, B>(Acquire, Release, x => Next(x).Bind(f));

    public override IO<B> BindAsync<B>(Func<A, ValueTask<K<IO, B>>> f) => 
        new IOUse<X, B>(Acquire, Release, x => Next(x).BindAsync(f));

    public override IO<A> Invoke(EnvIO envIO)
    {
        var task = Acquire.RunAsync(envIO);
        if (task.IsCompleted)
        {
            envIO.Resources.Acquire(task.Result, Release);
            return Next(task.Result);
        }
        else
        {
            return new IOAcquireAsync<X, A>(task, Release, Next);
        }
    }
    
    public override string ToString() => 
        "IO use";
}

record IOUseDisposable<X, A>(IO<X> Acquire, Func<X, IO<A>> Next) : InvokeSyncIO<A>
    where X : IDisposable
{
    public override IO<B> Map<B>(Func<A, B> f) => 
        new IOUseDisposable<X, B>(Acquire, x => Next(x).Map(f));

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) => 
        new IOUseDisposable<X, B>(Acquire, x => Next(x).Bind(f));

    public override IO<B> BindAsync<B>(Func<A, ValueTask<K<IO, B>>> f) => 
        new IOUseDisposable<X, B>(Acquire, x => Next(x).BindAsync(f));

    public override IO<A> Invoke(EnvIO envIO)
    {
        var task = Acquire.RunAsync(envIO);
        if (task.IsCompleted)
        {
            envIO.Resources.Acquire(task.Result);
            return Next(task.Result);
        }
        else
        {
            return new IOAcquireDisposableAsync<X, A>(task, Next);
        }
    }
    
    public override string ToString() => 
        "IO use disposable";
}

record IOUseAsyncDisposable<X, A>(IO<X> Acquire, Func<X, IO<A>> Next) : InvokeSyncIO<A>
    where X : IAsyncDisposable
{
    public override IO<B> Map<B>(Func<A, B> f) => 
        new IOUseAsyncDisposable<X, B>(Acquire, x => Next(x).Map(f));

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) => 
        new IOUseAsyncDisposable<X, B>(Acquire, x => Next(x).Bind(f));

    public override IO<B> BindAsync<B>(Func<A, ValueTask<K<IO, B>>> f) => 
        new IOUseAsyncDisposable<X, B>(Acquire, x => Next(x).BindAsync(f));

    public override IO<A> Invoke(EnvIO envIO)
    {
        var task = Acquire.RunAsync(envIO);
        if (task.IsCompleted)
        {
            envIO.Resources.AcquireAsync(task.Result);
            return Next(task.Result);
        }
        else
        {
            return new IOAcquireAsyncDisposableAsync<X, A>(task, Next);
        }
    }
    
    public override string ToString() => 
        "IO pure async disposable";
}

record IOAcquireAsync<X, A>(ValueTask<X> Value, Func<X, IO<Unit>> Release, Func<X, IO<A>> Next) : InvokeAsyncIO<A>
{
    public override IO<B> Map<B>(Func<A, B> f) => 
        new IOAcquireAsync<X, B>(Value, Release, x => Next(x).Map(f));

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) => 
        new IOAcquireAsync<X, B>(Value, Release, x => Next(x).Bind(f));

    public override IO<B> BindAsync<B>(Func<A, ValueTask<K<IO, B>>> f) => 
        new IOAcquireAsync<X, B>(Value, Release, x => Next(x).BindAsync(f));

    public override async ValueTask<IO<A>> Invoke(EnvIO envIO)
    {
        var value = await Value;
        envIO.Resources.Acquire(value, Release);
        return Next(value);
    }
    
    public override string ToString() => 
        "IO acquire async";
}

record IOAcquireDisposableAsync<X, A>(ValueTask<X> Value, Func<X, IO<A>> Next) : InvokeAsyncIO<A>
    where X : IDisposable
{
    public override IO<B> Map<B>(Func<A, B> f) => 
        new IOAcquireDisposableAsync<X, B>(Value, x => Next(x).Map(f));

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) => 
        new IOAcquireDisposableAsync<X, B>(Value, x => Next(x).Bind(f));

    public override IO<B> BindAsync<B>(Func<A, ValueTask<K<IO, B>>> f) => 
        new IOAcquireDisposableAsync<X, B>(Value, x => Next(x).BindAsync(f));

    public override async ValueTask<IO<A>> Invoke(EnvIO envIO)
    {
        var value = await Value;
        envIO.Resources.Acquire(value);
        return Next(value);
    }
    
    public override string ToString() => 
        "IO acquire disposable async";
}

record IOAcquireAsyncDisposableAsync<X, A>(ValueTask<X> Value, Func<X, IO<A>> Next) : InvokeAsyncIO<A>
    where X : IAsyncDisposable
{
    public override IO<B> Map<B>(Func<A, B> f) => 
        new IOAcquireAsyncDisposableAsync<X, B>(Value, x => Next(x).Map(f));

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) => 
        new IOAcquireAsyncDisposableAsync<X, B>(Value, x => Next(x).Bind(f));

    public override IO<B> BindAsync<B>(Func<A, ValueTask<K<IO, B>>> f) => 
        new IOAcquireAsyncDisposableAsync<X, B>(Value, x => Next(x).BindAsync(f));

    public override async ValueTask<IO<A>> Invoke(EnvIO envIO)
    {
        var value = await Value;
        envIO.Resources.AcquireAsync(value);
        return Next(value);
    }
    
    public override string ToString() => 
        "IO acquire async disposable async";
}
