using System;
using static LanguageExt.Prelude;

namespace LanguageExt.HKT;

public static class Monad
{
    public static Monad<M, A> pure<M, A>(A value) 
        where M : Monad<M> =>
        M.Pure(value);

    public static Monad<M, A> flatten<M, A>(Monad<M, Monad<M, A>> mma)
        where M : Monad<M> =>
        M.Flatten(mma);

    public static Monad<M, B> bind<M, A, B>(Monad<M, A> ma, Transducer<A, Monad<M, B>> f)
        where M  : Monad<M> =>
        M.Bind(ma, f);

    public static Monad<M, B> bind<M, A, B>(Monad<M, A> ma, Func<A, Monad<M, B>> f)
        where M : Monad<M> =>
        M.Bind(ma, f);
    
    public static MB bind<M, MB, A, B>(Monad<M, A> ma, Transducer<A, MB> f)
        where MB : Monad<M, B>
        where M  : Monad<M> =>
        (MB)M.Bind(ma, f.Map(mb => (Monad<M, B>)mb));

    public static MB bind<M, MB, A, B>(Monad<M, A> ma, Func<A, MB> f)
        where MB : Monad<M, B>
        where M : Monad<M> =>
        bind<M, MB, A, B>(ma, lift(f));
}
