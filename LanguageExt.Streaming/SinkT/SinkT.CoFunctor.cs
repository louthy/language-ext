using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class SinkT<M> : Decidable<SinkT<M>>
    where M : MonadIO<M>
{
    static K<SinkT<M>, A> Cofunctor<SinkT<M>>.Comap<A, B>(Func<A, B> f, K<SinkT<M>, B> fb) =>
        fb.As().Comap(f);

    static K<SinkT<M>, A> Divisible<SinkT<M>>.Divide<A, B, C>(Func<A, (B Left, C Right)> f, K<SinkT<M>, B> fb, K<SinkT<M>, C> fc) =>
        new SinkTCombine<M, A, B, C>(f, fb.As(), fc.As());

    static K<SinkT<M>, A> Divisible<SinkT<M>>.Conquer<A>() =>
        SinkT<M, A>.Empty;

    static K<SinkT<M>, A> Decidable<SinkT<M>>.Lose<A>(Func<A, Void> f) =>
        SinkT<M, A>.Void;

    static K<SinkT<M>, A> Decidable<SinkT<M>>.Route<A, B, C>(Func<A, Either<B, C>> f, K<SinkT<M>, B> fb, K<SinkT<M>, C> fc) =>
        new SinkTChoose<M, A, B, C>(f, fb.As(), fc.As());
}
