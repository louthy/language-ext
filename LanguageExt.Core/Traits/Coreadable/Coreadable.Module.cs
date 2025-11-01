using System;

namespace LanguageExt.Traits;

public static class Coreadable
{
    public static K<M, Env, Env> ask<M, Env>()
        where M : Coreadable<M> =>
        M.Ask<Env>();

    public static K<M, Env, A> asks<M, Env, A>(Func<Env, A> f)
        where M : Coreadable<M> =>
        M.Asks(f);

    public static K<M, Env, A> asksM<M, Env, A>(Func<Env, K<M, Env, A>> f)
        where M : Coreadable<M>, Bimonad<M> =>
        M.FlattenSecond(M.Asks(f));

    public static K<M, Env1, A> local<M, Env, Env1, A>(Func<Env, Env1> f, K<M, Env, A> ma)
        where M : Coreadable<M> =>
        M.Local(f, ma);
}
