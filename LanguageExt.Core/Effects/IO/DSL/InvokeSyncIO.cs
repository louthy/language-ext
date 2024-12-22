namespace LanguageExt.DSL;

public abstract record InvokeSyncIO<A> : IO<A>
{
    public abstract IO<A> Invoke(EnvIO envIO);
}
