using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes.Concurrent;

/// <summary>
/// Entry point to a channel.  Inboxes receive values and propagate them through the
/// channel they're attached to.  The behaviour depends on the `Buffer` type they
/// were created with.
/// </summary>
/// <typeparam name="A">Value type</typeparam>
public abstract record Inbox<A> : 
    K<Inbox, A>, 
    Monoid<Inbox<A>>
{
    /// <summary>
    /// Post a value to the inbox
    /// </summary>
    /// <remarks>
    /// Raises `Errors.NoSpaceInInbox` if the inbox is full or closed.
    /// </remarks>
    /// <param name="value">Value to post</param>
    /// <returns>IO computation that represents the posting</returns>
    public abstract IO<Unit> Post(A value);
    
    /// <summary>
    /// Complete and close the inbox
    /// </summary>
    public abstract IO<Unit> Complete();
    
    /// <summary>
    /// Complete and close the inbox with an `Error`
    /// </summary>
    public abstract IO<Unit> Fail(Error Error);

    /// <summary>
    /// Contravariant functor map
    /// </summary>
    public abstract Inbox<B> Contramap<B>(Func<B, A> f);

    /// <summary>
    /// Combine two Inboxes: `lhs` and `rhs` into a single inbox that takes incoming
    /// values and then posts the to the `lhs` and `rhs` inboxes. 
    /// </summary>
    public Inbox<A> Combine(Inbox<A> rhs) =>
        new InboxCombine<A, A, A>(x => (x, x), this, rhs);

    /// <summary>
    /// Combine two Inboxes: `lhs` and `rhs` into a single inbox that takes incoming
    /// values, maps them to an `(A, B)` tuple, and the posts the first and second
    /// elements to the `lhs` and `rhs` inboxes. 
    /// </summary>
    public Inbox<X> Combine<X, B>(Func<X, (A Left, B Right)> f, Inbox<B> rhs) =>
        new InboxCombine<X, A, B>(f, this, rhs);
    
    /// <summary>
    /// Combine two inboxes into a single outbox.  The values are both
    /// merged into a new inbox.  
    /// </summary>
    /// <param name="lhs">Left hand side</param>
    /// <param name="rhs">Right hand side</param>
    /// <returns>Merged stream of values</returns>
    public static Inbox<A> operator +(Inbox<A> lhs, Inbox<A> rhs) =>
        lhs.Combine(rhs);

    /// <summary>
    /// Inbox that is closed and can't be posted to without an error being raised
    /// </summary>
    public static Inbox<A> Empty => 
        InboxEmpty<A>.Default;

    /// <summary>
    /// Inbox that swallows everything silently
    /// </summary>
    public static Inbox<A> Void => 
        InboxVoid<A>.Default;

    /// <summary>
    /// Convert the `Inbox` to a `ConsumerT` pipe component
    /// </summary>
    /// <typeparam name="M">Monad to lift (must support `IO`)</typeparam>
    /// <returns>`ConsumerT`</returns>
    public ConsumerT<A, M, Unit> ToConsumerT<M>()
        where M : Monad<M> =>
        ConsumerT.repeat(ConsumerT.awaiting<M, A>().Bind(Post));

    /// <summary>
    /// Convert the `Inbox` to a `Consumer` pipe component
    /// </summary>
    /// <returns>`Consumer`</returns>
    public Consumer<RT, A, Unit> ToConsumer<RT>() =>
        ToConsumerT<Eff<RT>>();
}
