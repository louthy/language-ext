using System;

namespace LanguageExt.Traits;

public interface Readable<M, Env>
    where M : Readable<M, Env>
{
    public static abstract K<M, A> Asks<A>(Func<Env, A> f);

    public static virtual K<M, Env> Ask =>
        M.Asks(Prelude.identity);

    public static abstract K<M, A> Local<A>(
        Func<Env, Env> f,
        K<M, A> ma);
}
