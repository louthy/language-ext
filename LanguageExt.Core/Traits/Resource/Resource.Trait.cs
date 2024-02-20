using System;

namespace LanguageExt.Traits;

/// <summary>
/// Resource tracking trait
/// </summary>
/// <typeparam name="M"></typeparam>
public interface Resource<in M>
    where M : Resource<M>
{
    public static abstract K<M, A> Use<A>(IO<A> ma, Func<A, IO<Unit>> release);

    public static abstract K<M, Unit> Release<A>(A value);
}
