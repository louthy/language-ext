using System.Threading.Tasks;

namespace LanguageExt.DSL;

/// <summary>
/// Base-type of the DSL types used by the `IO` monad.  Use this to extend the core `IO` functionality.   
/// </summary>
/// <typeparam name="A">Bound value type</typeparam>
public abstract record InvokeAsyncIO<A> : IO<A>
{
    public abstract ValueTask<IO<A>> Invoke(EnvIO envIO);
}
