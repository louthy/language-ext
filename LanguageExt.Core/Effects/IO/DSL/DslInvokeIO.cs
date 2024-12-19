using System.Threading.Tasks;

namespace LanguageExt.DSL;

abstract record DslInvokeIO<A> : IODsl<A>
{
    public abstract A Invoke(EnvIO envIO);
}

abstract record DslInvokeIOAsync<A> : IODsl<A>
{
    public abstract ValueTask<A> Invoke(EnvIO envIO);
}
