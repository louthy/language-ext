using LanguageExt.Traits;
using System.Collections.Generic;

namespace LanguageExt;

/// <summary>
/// StreamT module
/// </summary>
public static class StreamT
{
    public static StreamT<M, A> pure<M, A>(A value)
        where M : Monad<M> =>
        new StreamPureT<M, A>(value);

    public static StreamT<M, A> lift<M, A>(IAsyncEnumerable<A> items)
        where M : Monad<M> =>
        StreamT<M, A>.Lift(items);

    public static StreamT<M, A> lift<M, A>(IEnumerable<A> items)
        where M : Monad<M> =>
        StreamT<M, A>.Lift(items);

    public static StreamT<M, A> lift<M, A>(Seq<A> items)
        where M : Monad<M> =>
        StreamT<M, A>.Lift(items);

    public static StreamT<M, A> liftIO<M, A>(IO<A> ma)
        where M : Monad<M> =>
        StreamT<M, A>.LiftIO(ma);
}
