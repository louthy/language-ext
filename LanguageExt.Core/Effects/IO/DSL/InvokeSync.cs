namespace LanguageExt.DSL;

public abstract record InvokeSync<A> : IO<A>
{
    public abstract A Invoke(EnvIO envIO);
}
