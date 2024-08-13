using System;
using System.Collections.Generic;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// StreamT trait implementations
/// </summary>
public class StreamT<M> :
    MonadT<StreamT<M>, M>,
    Alternative<StreamT<M>>
    where M : Monad<M>
{
    public static StreamT<M, A> pure<A>(A value) =>
        new StreamMainT<M, A>(M.Pure(MList<A>.Cons(value, M.Pure(MList<A>.Nil))));

    public static StreamT<M, A> lift<A>(IEnumerable<A> items) =>
        StreamT<M, A>.Lift(items);

    public static StreamT<M, A> lift<A>(IAsyncEnumerable<A> items) =>
        StreamT<M, A>.Lift(items);

    public static StreamT<M, A> lift<A>(Seq<A> items) =>
        StreamT<M, A>.Lift(items);

    public static StreamT<M, A> liftIO<A>(IO<A> ma) =>
        StreamT<M, A>.LiftIO(ma);

    static K<StreamT<M>, B> Monad<StreamT<M>>.Bind<A, B>(
        K<StreamT<M>, A> mma,
        Func<A, K<StreamT<M>, B>> f) =>
        mma.As().Map(f).Flatten();

    static K<StreamT<M>, B> Functor<StreamT<M>>.Map<A, B>(
        Func<A, B> f,
        K<StreamT<M>, A> mma) =>
        mma.As().Map(f);

    static K<StreamT<M>, A> Applicative<StreamT<M>>.Pure<A>(A value) =>
        new StreamPureT<M, A>(value);

    static K<StreamT<M>, B> Applicative<StreamT<M>>.Apply<A, B>(
        K<StreamT<M>, Func<A, B>> mf,
        K<StreamT<M>, A> ma) =>
        mf.Bind(f => ma.Map(f));

    static K<StreamT<M>, A> MonadT<StreamT<M>, M>.Lift<A>(K<M, A> ma) =>
        new StreamLiftM<M, A>(ma);

    static K<StreamT<M>, A> SemigroupK<StreamT<M>>.Combine<A>(K<StreamT<M>, A> lhs, K<StreamT<M>, A> rhs) =>
        lhs.As().Combine(rhs.As());

    static K<StreamT<M>, A> MonoidK<StreamT<M>>.Empty<A>() =>
        StreamMainT<M, A>.Empty;

    static K<StreamT<M>, A> MonadIO<StreamT<M>>.LiftIO<A>(IO<A> ma) =>
        new StreamLiftM<M, A>(M.LiftIO(ma));
}
