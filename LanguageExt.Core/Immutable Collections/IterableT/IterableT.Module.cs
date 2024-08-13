using LanguageExt.Traits;
using System.Collections.Generic;

namespace LanguageExt;

/// <summary>
/// IterableT module
/// </summary>
public static class IterableT
{
    public static IterableT<M, A> pure<M, A>(A value) 
        where M : Monad<M> =>
        new IterablePureT<M, A>(value);
    
    public static IterableT<M, A> lift<M, A>(IAsyncEnumerable<A> items) 
        where M : Monad<M> =>
        IterableT<M, A>.Lift(items);

    public static IterableT<M, A> lift<M, A>(IEnumerable<A> items) 
        where M : Monad<M> =>
        IterableT<M, A>.Lift(items);

    public static IterableT<M, A> lift<M, A>(Seq<A> items)
        where M : Monad<M> =>
        IterableT<M, A>.Lift(items);

    public static IterableT<M, A> liftIO<M, A>(IO<A> ma) 
        where M : Monad<M> =>
        IterableT<M, A>.LiftIO(ma);
}
