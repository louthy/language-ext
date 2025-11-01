using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static class CoreadableExtensions
{
    public static K<M, Env1, A> Local<M, Env, Env1, A>(this K<M, Env, A> ma, Func<Env, Env1> f)
        where M : Coreadable<M> =>
        M.Local(f, ma);
}
