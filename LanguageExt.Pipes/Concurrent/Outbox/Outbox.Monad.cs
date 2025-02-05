using System;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

public partial class Outbox : 
    Monad<Outbox>, 
    Alternative<Outbox>
{
    static K<Outbox, B> Monad<Outbox>.Bind<A, B>(K<Outbox, A> ma, Func<A, K<Outbox, B>> f) => 
        ma.As().Bind(f);

    static K<Outbox, B> Functor<Outbox>.Map<A, B>(Func<A, B> f, K<Outbox, A> ma) => 
        ma.As().Map(f);

    static K<Outbox, A> Applicative<Outbox>.Pure<A>(A value) =>
        new OutboxPure<A>(value);

    static K<Outbox, B> Applicative<Outbox>.Apply<A, B>(K<Outbox, Func<A, B>> mf, K<Outbox, A> ma) => 
        ma.As().ApplyBack(mf.As());

    static K<Outbox, A> SemigroupK<Outbox>.Combine<A>(K<Outbox, A> fa, K<Outbox, A> fb) =>
        fa.As().Combine(fb.As());

    static K<Outbox, A> Choice<Outbox>.Choose<A>(K<Outbox, A> fa, K<Outbox, A> fb) =>
        fa.As().Choose(fb.As());

    static K<Outbox, A> MonoidK<Outbox>.Empty<A>() =>
        OutboxEmpty<A>.Default;
}
