using System;

namespace LanguageExt.Traits;

public static partial class Reader
{
    public static K<M, A> Local<M, Env, A>(this K<M, A> ma, Func<Env, Env> f)
        where M : Reader<M, Env> =>
        M.Local(f, ma);
}
