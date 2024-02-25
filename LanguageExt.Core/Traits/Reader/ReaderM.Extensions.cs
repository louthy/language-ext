using System;

namespace LanguageExt.Traits;

public static class ReaderExtensions
{
    public static K<M, A> Local<M, Env, A>(this K<M, A> ma, Func<Env, Env> f)
        where M : ReaderM<M, Env> =>
        M.Local(f, ma);
}
