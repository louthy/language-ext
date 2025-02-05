using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

public abstract record Outbox<A> : 
    K<Outbox, A>, 
    Monoid<Outbox<A>>
{
    /// <summary>
    /// An outbox that never yields a value
    /// </summary>
    public static Outbox<A> Empty =>
        OutboxEmpty<A>.Default;

    /// <summary>
    /// Read value from the outbox
    /// </summary>
    /// <remarks>
    /// Raises a `Errors.OutboxChannelClosed` if the channel is closed or empty
    /// </remarks>
    /// <returns>First available value from the channel</returns>
    public abstract IO<A> Read();
    
    /// <summary>
    /// Functor map
    /// </summary>
    public abstract Outbox<B> Map<B>(Func<A, B> f);
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public abstract Outbox<B> Bind<B>(Func<A, Outbox<B>> f);
    
    /// <summary>
    /// Applicative apply
    /// </summary>
    public abstract Outbox<B> ApplyBack<B>(Outbox<Func<A, B>> ff);

    /// <summary>
    /// Combine two outboxes into a single outbox.  The value streams are both
    /// merged into a new stream.  Values are yielded as they become available.
    /// </summary>
    /// <param name="rhs">Right hand side</param>
    /// <returns>Merged stream of values</returns>
    public Outbox<A> Combine(Outbox<A> rhs) =>
        (this, rhs) switch
        {
            (OutboxEmpty<A>, OutboxEmpty<A>)         => OutboxEmpty<A>.Default,
            (var l, OutboxEmpty<A>)                  => l,
            (OutboxEmpty<A>, var r)                  => r,
            (OutboxCombine<A> l, OutboxCombine<A> r) => new OutboxCombine<A>(l.Sources + r.Sources),
            (OutboxCombine<A> l, var r)              => new OutboxCombine<A>(l.Sources.Add(r)),
            (var l, OutboxCombine<A> r)              => new OutboxCombine<A>(l.Cons(r.Sources)),
            _                                        => new OutboxCombine<A>([this, rhs])
        };
    
    /// <summary>
    /// Choose a value from the first `Outbox` to successfully yield 
    /// </summary>
    /// <param name="rhs"></param>
    /// <returns>Value from this `Outbox` if there are any available, if not, from `rhs`.  If
    /// `rhs` is also empty then `Errors.OutboxChannelClosed` is raised</returns>
    public Outbox<A> Choose(Outbox<A> rhs) =>
        new OutboxChoose<A>(this, rhs);

    /// <summary>
    /// Monad bind
    /// </summary>
    public Outbox<B> Bind<B>(Func<A, K<Outbox, B>> f) =>
        Bind(x => f(x).As());

    /// <summary>
    /// Convert `Outbox` to a `ProducerT` pipe component
    /// </summary>
    /// <typeparam name="M">Monad to lift (must support `IO`)</typeparam>
    /// <returns>`ProducerT`</returns>
    public ProducerT<A, M, Unit> ToProducerT<M>()
        where M : Monad<M> =>
        Read().Bind(ProducerT.yield<M, A>)
              .Bind(_ => ToProducerT<M>());

    /// <summary>
    /// Convert `Outbox` to a `Producer` pipe component
    /// </summary>
    /// <returns>`Producer`</returns>
    public Producer<RT, A, Unit> ToProducer<RT>() =>
        ToProducerT<Eff<RT>>();
    
    /// <summary>
    /// Combine two outboxes into a single outbox.  The value streams are both
    /// merged into a new stream.  Values are yielded as they become available.
    /// </summary>
    /// <param name="lhs">Left hand side</param>
    /// <param name="rhs">Right hand side</param>
    /// <returns>Merged stream of values</returns>
    public static Outbox<A> operator +(Outbox<A> lhs, Outbox<A> rhs) =>
        lhs.Combine(rhs);

    /// <summary>
    /// Choose a value from the first `Outbox` to successfully yield 
    /// </summary>
    /// <param name="lhs">Left hand side</param>
    /// <param name="rhs">Right hand side</param>
    /// <returns>Value from the `lhs` `Outbox` if there are any available, if not, from `rhs`.  If
    /// `rhs` is also empty then `Errors.OutboxChannelClosed` is raised</returns>
    public static Outbox<A> operator |(Outbox<A> lhs, Outbox<A> rhs) =>
        lhs.Choose(rhs);

    internal abstract ValueTask<bool> ReadyToRead(CancellationToken token);
}
