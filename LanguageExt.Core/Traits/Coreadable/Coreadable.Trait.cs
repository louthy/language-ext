using System;

namespace LanguageExt.Traits;

public interface Coreadable<M>
    where M : Coreadable<M>
{
    public static abstract K<M, Env, A> Asks<Env, A>(Func<Env, A> f);

    public static virtual K<M, Env, Env> Ask<Env>() =>
        M.Asks<Env, Env>(Prelude.identity);

    public static abstract K<M, Env1, A> Local<Env, Env1, A>(
        Func<Env, Env1> f,
        K<M, Env, A> ma);
}
