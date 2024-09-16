using System;

namespace LanguageExt;

public abstract record IOResponse<A>;
public record CompleteIO<A>(A Value) : IOResponse<A>;
public record RecurseIO<A>(IO<A> Computation) : IOResponse<A>;

public abstract record BindIO<A> : IOResponse<A>
{
    public abstract IO<A> Run();
}

public record BindIO<X, A>(X Value, Func<X, IO<A>> Computation) : BindIO<A>
{
    public override IO<A> Run() =>
        Computation(Value);
}

public static class IOResponse
{
    public static IOResponse<A> Complete<A>(A value) => new CompleteIO<A>(value);
    public static IOResponse<A> Recurse<A>(IO<A> computation) => new RecurseIO<A>(computation);
    public static IOResponse<A> Bind<X, A>(X value, Func<X, IO<A>> computation) => 
        new BindIO<X, A>(value, computation);
}
