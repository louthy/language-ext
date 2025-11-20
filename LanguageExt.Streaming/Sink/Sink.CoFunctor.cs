using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Sink : Decidable<Sink>
{
    static K<Sink, A> Cofunctor<Sink>.Comap<A, B>(Func<A, B> f, K<Sink, B> fb) =>
        fb.As().Comap(f);

    static K<Sink, A> Divisible<Sink>.Divide<A, B, C>(Func<A, (B Left, C Right)> f, K<Sink, B> fb, K<Sink, C> fc) =>
        new SinkCombine<A, B, C>(f, fb.As(), fc.As());

    static K<Sink, A> Divisible<Sink>.Conquer<A>() =>
        Sink<A>.Empty;

    static K<Sink, A> Decidable<Sink>.Lose<A>(Func<A, Void> f) =>
        Sink<A>.Void;

    static K<Sink, A> Decidable<Sink>.Route<A, B, C>(Func<A, Either<B, C>> f, K<Sink, B> fb, K<Sink, C> fc) =>
        new SinkChoose<A, B, C>(f, fb.As(), fc.As());
}
