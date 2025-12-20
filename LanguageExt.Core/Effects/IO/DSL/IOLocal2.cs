using System;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.DSL;

record IOLocal<X, A>(Func<EnvIO, EnvIO> MapEnvIO, K<IO, X> Operation, Func<X, K<IO, A>> Next) : InvokeAsyncIO<A>
{
    public override IO<B> Map<B>(Func<A, B> f) => 
        new IOLocal<X, B>(MapEnvIO, Operation, x => Next(x).Map(f));

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) => 
        new IOLocal<X, B>(MapEnvIO, Operation, x => Next(x).Bind(f));

    public override IO<B> BindAsync<B>(Func<A, ValueTask<K<IO, B>>> f) => 
        new IOLocal<X, B>(MapEnvIO, Operation, x => Next(x).As().BindAsync(f));

    public override async ValueTask<IO<A>> Invoke(EnvIO envIO)
    {
        using var local = MapEnvIO(envIO);
        return +Next(await Operation.RunAsync(local));
    }

    public override string ToString() => 
        "IO local";
}

record IOLocalOnFailOnly<X, A>(Func<EnvIO, EnvIO> MapEnvIO, K<IO, X> Operation, Func<X, K<IO, A>> Next) : InvokeAsyncIO<A>
{
    public override IO<B> Map<B>(Func<A, B> f) => 
        new IOLocalOnFailOnly<X, B>(MapEnvIO, Operation, x => Next(x).Map(f));

    public override IO<B> Bind<B>(Func<A, K<IO, B>> f) => 
        new IOLocalOnFailOnly<X, B>(MapEnvIO, Operation, x => Next(x).Bind(f));

    public override IO<B> BindAsync<B>(Func<A, ValueTask<K<IO, B>>> f) => 
        new IOLocalOnFailOnly<X, B>(MapEnvIO, Operation, x => Next(x).As().BindAsync(f));

    public override async ValueTask<IO<A>> Invoke(EnvIO envIO)
    {
        var local = MapEnvIO(envIO);
        try
        {
            return +Next(await Operation.RunAsync(local));
        }
        catch
        {
            local.DisposeResources();
            throw;
        }
        finally
        {
            local.DisposeNonResources();
        }
    }

    public override string ToString() => 
        "IO local";
}
