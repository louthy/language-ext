using System.Threading.Tasks;

namespace LanguageExt.DSL;

public abstract record InvokeAsync<A> : IO<A>
{
    public abstract ValueTask<A> Invoke(EnvIO envIO);
}
