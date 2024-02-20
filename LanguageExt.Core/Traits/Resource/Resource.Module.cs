using System;

namespace LanguageExt.Traits;

public static class Resource
{
    public static K<M, A> use<M, A>(IO<A> ma, Func<A, IO<Unit>> release)
        where M : Resource<M> =>
        M.Use(ma, release);

    public static K<M, Unit> release<M, A>(A value) 
        where M : Resource<M> =>
        M.Release(value);

    public static K<M, A> use<M, A>(IO<A> ma, Action<A> release) 
        where M : Resource<M> =>
        use<M, A>(ma, value => IO<Unit>.Lift(_ => { release(value); return Prelude.unit; }));

    public static K<M, A> use<M, A>(Func<EnvIO, A> f) 
        where A : IDisposable 
        where M : Resource<M> =>
        use<M, A>(IO<A>.Lift(f));

    public static K<M, A> use<M, A>(Func<A> f) 
        where A : IDisposable 
        where M : Resource<M> =>
        use<M, A>(IO<A>.Lift(f));

    public static K<M, A> use<M, A>(IO<A> ma)
        where A : IDisposable 
        where M : Resource<M> =>
        use<M, A>(ma, a => IO.lift(() => { a.Dispose(); return default(Unit); }));
}
