using System;
using LanguageExt.Common;

namespace LanguageExt.Traits;

public static class Local
{
    public static K<M, A> with<M, Env, InnerEnv, A>(Func<InnerEnv, InnerEnv> f, K<M, A> ma)
        where Env : Local<M, InnerEnv> =>
        Env.With(f, ma);
}
