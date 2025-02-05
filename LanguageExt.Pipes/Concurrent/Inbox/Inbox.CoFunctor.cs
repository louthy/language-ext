using System;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

public class Inbox : Decidable<Inbox>
{
    public static K<Inbox, B> Contramap<A, B>(K<Inbox, B> fb, Func<A, B> f) =>
        fb.As().Contramap(f);

    public static K<Inbox, A> Divide<A, B, C>(Func<A, (B Left, C Right)> f, K<Inbox, B> fb, K<Inbox, C> fc) =>
        new InboxCombine<A, B, C>(f, fb.As(), fc.As());

    public static K<Inbox, A> Conquer<A>() =>
        Inbox<A>.Empty;

    public static K<Inbox, A> Lose<A>(Func<A, Void> f) =>
        Inbox<A>.Void;

    public static K<Inbox, A> Route<A, B, C>(Func<A, Either<B, C>> f, K<Inbox, B> fb, K<Inbox, C> fc) =>
        new InboxChoose<A, B, C>(f, fb.As(), fc.As());
}
