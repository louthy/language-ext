using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

/// <summary>
/// Represents a channel.  A channel has:
///
///   * An `Inbox`: a queue of values that are its input.
///   * An `Outbox`:  a stream of values that are its output.
///
/// Both sides of the mailbox can be manipulated:
///
/// The `Inbox` is a `Cofunctor` and can be mapped using `Contramap`, this
/// transforms values before they get to the channel.
/// 
/// The `Outbox` is a `Monad`, so you can `Map`, `Bind`, `Apply`, in the
/// usual way to map values on their way out.  They manipulate values as theu
/// leave the channel. 
///
/// `Outbox` values can be both merged (using `+` or `Combine) and
/// 'chosen' using `|` or `Choose`.
///
/// Incoming `Inbox` values can be split and passed to multiple `Inbox`
/// channels using (using `+` or `Combine)
///
/// `ToProducer` and `ToConsumer` allows the `Mailbox` components to be used
/// in composed pipe effects.
/// </summary>
/// <param name="Inbox">Inbox</param>
/// <param name="Outbox">Outbox</param>
/// <typeparam name="A">Input value type</typeparam>
/// <typeparam name="B">Output value type</typeparam>
public record Mailbox<A, B>(Inbox<A> Inbox, Outbox<B> Outbox)
{
    /// <summary>
    /// Post a value to the inbox
    /// </summary>
    /// <remarks>
    /// Raises `Errors.NoSpaceInInbox` if the inbox is full or closed.
    /// </remarks>
    /// <param name="value">Value to post</param>
    /// <returns>IO computation that represents the posting</returns>
    public IO<Unit> Post(A value) =>
        Inbox.Post(value);

    /// <summary>
    /// Read value from the outbox
    /// </summary>
    /// <remarks>
    /// Raises a `Errors.OutboxChannelClosed` if the channel is closed or empty
    /// </remarks>
    /// <returns>First available value from the channel</returns>
    public IO<B> Read() =>
        Outbox.Read();
    
    /// <summary>
    /// Complete and close the inbox
    /// </summary>
    public IO<Unit> Complete() =>
        Inbox.Complete();
    
    /// <summary>
    /// Complete and close the inbox with an `Error`
    /// </summary>
    public IO<Unit> Fail(Error Error) =>
        Inbox.Fail(Error);
    
    /// <summary>
    /// Functor map
    /// </summary>
    public Mailbox<A, C> Map<C>(Func<B, C> f) =>
        new (Inbox, Outbox.Map(f));
    
    /// <summary>
    /// Monad bind
    /// </summary>
    public Mailbox<A, C> Bind<C>(Func<B, Outbox<C>> f) =>
        new (Inbox, Outbox.Bind(f));
    
    /// <summary>
    /// Applicative apply
    /// </summary>
    public Mailbox<A, C> ApplyBack<C>(Outbox<Func<B, C>> ff) =>
        new (Inbox, Outbox.ApplyBack(ff));
    
    /// <summary>
    /// Contravariant functor map
    /// </summary>
    public Mailbox<C, B> ContraMap<C>(Func<C, A> f) =>
        new (Inbox.ContraMap(f), Outbox);

    /// <summary>
    /// Convert the `Inbox` to a `ConsumerT` pipe component
    /// </summary>
    /// <typeparam name="M">Monad to lift (must support `IO`)</typeparam>
    /// <returns>`ConsumerT`</returns>
    public ConsumerT<A, M, Unit> ToConsumer<M>()
        where M : Monad<M> =>
        Inbox.ToConsumer<M>();

    /// <summary>
    /// Convert `Outbox` to a `ProducerT` pipe component
    /// </summary>
    /// <typeparam name="M">Monad to lift (must support `IO`)</typeparam>
    /// <returns>`ProducerT`</returns>
    public ProducerT<B, M, Unit> ToProducer<M>()
        where M : Monad<M> =>
        Outbox.ToProducer<M>();
    
    /// <summary>
    /// Combine two Inboxes: `lhs` and `rhs` into a single inbox that takes incoming
    /// values and then posts the to the `lhs` and `rhs` inboxes. 
    /// </summary>
    public Mailbox<A, B> Combine(Inbox<A> rhs) =>
        this with { Inbox = Inbox.Combine(rhs) };
    
    /// <summary>
    /// Combine two Inboxes: `lhs` and `rhs` into a single inbox that takes incoming
    /// values, maps them to an `(A, B)` tuple, and the posts the first and second
    /// elements to the `lhs` and `rhs` inboxes. 
    /// </summary>
    public Mailbox<X, B> Combine<X, C>(Func<X, (A Left, C Right)> f, Inbox<C> rhs) =>
        new (Inbox.Combine(f, rhs), Outbox);

    /// <summary>
    /// Combine two outboxes into a single outbox.  The value streams are both
    /// merged into a new stream.  Values are yielded as they become available.
    /// </summary>
    /// <param name="rhs">Right hand side</param>
    /// <returns>Merged stream of values</returns>
    public Mailbox<A, B> Combine(Outbox<B> rhs) =>
        this with { Outbox = Outbox.Combine(rhs) };
    
    /// <summary>
    /// Choose a value from the first `Outbox` to successfully yield 
    /// </summary>
    /// <param name="rhs"></param>
    /// <returns>Value from this `Outbox` if there are any available, if not, from `rhs`.  If
    /// `rhs` is also empty then `Errors.OutboxChannelClosed` is raised</returns>
    public Mailbox<A, B> Choose(Outbox<B> rhs) =>
        this with { Outbox = Outbox.Choose(rhs) };
    
    /// <summary>
    /// Combine two inboxes into a single outbox.  The values are both
    /// merged into a new inbox.  
    /// </summary>
    /// <param name="lhs">Left hand side</param>
    /// <param name="rhs">Right hand side</param>
    /// <returns>Merged stream of values</returns>
    public static Mailbox<A, B> operator +(Inbox<A> lhs, Mailbox<A, B> rhs) =>
        rhs with { Inbox = lhs.Combine(rhs.Inbox) };
    
    /// <summary>
    /// Combine two outboxes into a single outbox.  The value streams are both
    /// merged into a new stream.  Values are yielded as they become available.
    /// </summary>
    /// <param name="lhs">Left hand side</param>
    /// <param name="rhs">Right hand side</param>
    /// <returns>Merged stream of values</returns>
    public static Mailbox<A, B> operator +(Mailbox<A, B> lhs, Outbox<B> rhs) =>
        lhs.Combine(rhs);

    /// <summary>
    /// Choose a value from the first `Outbox` to successfully yield 
    /// </summary>
    /// <param name="lhs">Left hand side</param>
    /// <param name="rhs">Right hand side</param>
    /// <returns>Value from the `lhs` `Outbox` if there are any available, if not, from `rhs`.  If
    /// `rhs` is also empty then `Errors.OutboxChannelClosed` is raised</returns>
    public static Mailbox<A, B> operator |(Mailbox<A, B> lhs, Outbox<B> rhs) =>
        lhs.Choose(rhs);
}
