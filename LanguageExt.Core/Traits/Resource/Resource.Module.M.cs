/*
using System;

namespace LanguageExt.Traits;

public partial interface Resource<M>
    where M : Resource<M>
{
    public static K<M, A> use<A>(IO<A> acquire, Func<A, IO<Unit>> release) =>
        M.Use(acquire, release);

    public static K<M, A> use<A>(IO<A> acquire, Action<A> release) =>
        use(acquire, value => IO<Unit>.Lift(_ => { release(value); return Prelude.unit; }));

    public static K<M, A> use<A>(Func<A> acquire, Func<A, IO<Unit>> release) => 
        use(IO<A>.Lift(acquire), release);

    public static K<M, A> use<A>(Func<A> acquire, Func<A, Unit> release) => 
        use(IO<A>.Lift(acquire), x => release(x));

    public static K<M, A> use<A>(Func<A> acquire, Action<A> release) => 
        use(IO<A>.Lift(acquire), release);
    
    // Disposables
    
    public static K<M, A> use<A>(Func<EnvIO, A> acquire) 
        where A : IDisposable =>
        use(IO<A>.Lift(acquire));

    public static K<M, A> use<A>(Func<A> acquire) 
        where A : IDisposable =>
        use(IO<A>.Lift(acquire));

    public static K<M, A> use<A>(IO<A> acquire)
        where A : IDisposable =>
        use(acquire, a => IO.lift(() => { a.Dispose(); return default(Unit); }));
    
    public static K<M, Unit> release<A>(A value) => 
        M.Release(value);

    public static K<M, Resources> resources =>
        M.Resources;

    public static K<M, A> local<A>(K<M, A> ma) =>
        M.Local(ma);
}
*/
