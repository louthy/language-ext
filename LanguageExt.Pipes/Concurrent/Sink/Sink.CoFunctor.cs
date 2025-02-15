using System;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

public class Sink : Decidable<Sink>
{
    public static K<Sink, A> Contramap<A, B>(K<Sink, B> fb, Func<A, B> f) =>
        fb.As().Contramap(f);

    public static K<Sink, A> Divide<A, B, C>(Func<A, (B Left, C Right)> f, K<Sink, B> fb, K<Sink, C> fc) =>
        new SinkCombine<A, B, C>(f, fb.As(), fc.As());

    public static K<Sink, A> Conquer<A>() =>
        Sink<A>.Empty;

    public static K<Sink, A> Lose<A>(Func<A, Void> f) =>
        Sink<A>.Void;

    public static K<Sink, A> Route<A, B, C>(Func<A, Either<B, C>> f, K<Sink, B> fb, K<Sink, C> fc) =>
        new SinkChoose<A, B, C>(f, fb.As(), fc.As());
}
