using System;

namespace LanguageExt.Traits;

public static partial class Resource
{
    public static K<M, A> Use<M, A>(this IO<A> ma, Func<A, IO<Unit>> release)
        where M : Resource<M> =>
        M.Use(ma, release);

    public static K<M, A> Use<M, A>(this IO<A> ma, Action<A> release) 
        where M : Resource<M> =>
        use<M, A>(ma, value => IO<Unit>.Lift(_ => { release(value); return Prelude.unit; }));

    public static K<M, A> Use<M, A>(this IO<A> ma)
        where A : IDisposable 
        where M : Resource<M> =>
        use<M, A>(ma, a => IO.lift(() => { a.Dispose(); return default(Unit); }));
}
