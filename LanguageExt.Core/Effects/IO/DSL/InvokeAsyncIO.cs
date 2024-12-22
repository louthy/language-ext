using System.Threading.Tasks;

namespace LanguageExt.DSL;

public abstract record InvokeAsyncIO<A> : IO<A>
{
    public abstract ValueTask<IO<A>> Invoke(EnvIO envIO);
}
