using System;

namespace LanguageExt.Traits;

public static partial class Resource
{
    public static K<M, A> use<M, A>(IO<A> acquire, Func<A, IO<Unit>> release)
        where M : Resource<M> =>
        M.Use(acquire, release);

    public static K<M, A> use<M, A>(IO<A> acquire, Action<A> release) 
        where M : Resource<M> =>
        use<M, A>(acquire, value => IO<Unit>.Lift(_ => { release(value); return Prelude.unit; }));

    public static K<M, A> use<M, A>(Func<A> acquire, Func<A, IO<Unit>> release) 
        where M : Resource<M> =>
        use<M, A>(IO<A>.Lift(acquire), release);

    public static K<M, A> use<M, A>(Func<A> acquire, Func<A, Unit> release) 
        where M : Resource<M> =>
        use<M, A>(IO<A>.Lift(acquire), x => release(x));

    public static K<M, A> use<M, A>(Func<A> acquire, Action<A> release) 
        where M : Resource<M> =>
        use<M, A>(IO<A>.Lift(acquire), release);
    
    // Disposables
    
    public static K<M, A> use<M, A>(Func<EnvIO, A> acquire) 
        where A : IDisposable 
        where M : Resource<M> =>
        use<M, A>(IO<A>.Lift(acquire));

    public static K<M, A> use<M, A>(Func<A> acquire) 
        where A : IDisposable 
        where M : Resource<M> =>
        use<M, A>(IO<A>.Lift(acquire));

    public static K<M, A> use<M, A>(IO<A> acquire)
        where A : IDisposable 
        where M : Resource<M> =>
        use<M, A>(acquire, a => IO.lift(() => { a.Dispose(); return default(Unit); }));
    
    public static K<M, Unit> release<M, A>(A value) 
        where M : Resource<M> =>
        M.Release(value);

    public static K<M, Resources> resources<M>()
        where M : Resource<M> =>
        M.Resources;
}
