using System;

namespace LanguageExt.Traits;

/// <summary>
/// Creates a local environment to run a computation 
/// </summary>
/// <typeparam name="M">Structure trait</typeparam>
/// <typeparam name="InnerEnv">The value extracted from an environment</typeparam>
public interface Local<M, InnerEnv> : Has<M, InnerEnv>
{
    public static abstract K<M, A> With<A>(Func<InnerEnv, InnerEnv> f, K<M, A> ma);
}
