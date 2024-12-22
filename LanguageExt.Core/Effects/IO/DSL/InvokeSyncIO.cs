namespace LanguageExt.DSL;

/// <summary>
/// Base-type of the DSL types used by the `IO` monad.  Use this to extend the core `IO` functionality.   
/// </summary>
/// <typeparam name="A">Bound value type</typeparam>
public abstract record InvokeSyncIO<A> : IO<A>
{
    public abstract IO<A> Invoke(EnvIO envIO);
}
