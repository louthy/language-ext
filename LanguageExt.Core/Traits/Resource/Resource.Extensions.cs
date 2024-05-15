/*
using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ResourceExtensions
{
    public static K<M, A> Use<M, A>(this IO<A> ma, Func<A, IO<Unit>> release)
        where M : Resource<M> =>
        M.Use(ma, release);

    public static K<M, A> Use<M, A>(this IO<A> ma, Action<A> release) 
        where M : Resource<M> =>
        Resource.use<M, A>(ma, value => IO<Unit>.Lift(_ => { release(value); return Prelude.unit; }));

    public static K<M, A> Use<M, A>(this IO<A> ma)
        where A : IDisposable 
        where M : Resource<M> =>
        Resource.use<M, A>(ma, a => IO.lift(() => { a.Dispose(); return default(Unit); }));
}
*/
