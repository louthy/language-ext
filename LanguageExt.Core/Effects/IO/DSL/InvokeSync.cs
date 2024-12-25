namespace LanguageExt.DSL;

/// <summary>
/// Base-type of the DSL types used by the `IO` monad.  Use this to extend the core `IO` functionality.   
/// </summary>
/// <typeparam name="A">Bound value type</typeparam>
public abstract record InvokeSync<A> : IO<A>
{
    public abstract A Invoke(EnvIO envIO);
    
    public override string ToString() => 
        "IO invoke sync";
}
