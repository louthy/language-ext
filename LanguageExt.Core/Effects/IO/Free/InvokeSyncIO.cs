using System.Threading.Tasks;

namespace LanguageExt;

abstract record InvokeSyncIO<A> : IO<A>
{
    public abstract IO<A> Invoke(EnvIO envIO);
}

abstract record InvokeAsyncIO<A> : IO<A>
{
    public abstract ValueTask<IO<A>> Invoke(EnvIO envIO);
}

abstract record InvokeSync<A> : IO<A>
{
    public abstract A Invoke(EnvIO envIO);
}

abstract record InvokeAsync<A> : IO<A>
{
    public abstract ValueTask<A> Invoke(EnvIO envIO);
}
