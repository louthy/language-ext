using System.Threading.Tasks;

namespace LanguageExt.DSL;

public abstract record InvokeSyncIO<A> : IO<A>
{
    public abstract IO<A> Invoke(EnvIO envIO);
}

public abstract record InvokeAsyncIO<A> : IO<A>
{
    public abstract ValueTask<IO<A>> Invoke(EnvIO envIO);
}

public abstract record InvokeSync<A> : IO<A>
{
    public abstract A Invoke(EnvIO envIO);
}

public abstract record InvokeAsync<A> : IO<A>
{
    public abstract ValueTask<A> Invoke(EnvIO envIO);
}
