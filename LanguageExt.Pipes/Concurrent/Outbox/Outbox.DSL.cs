using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Pipes.Concurrent;

record OutboxPure<A>(A Value) : Outbox<A>
{
    public override Outbox<B> Map<B>(Func<A, B> f) => 
        new OutboxMap<A, B>(this, f);

    public override Outbox<B> Bind<B>(Func<A, Outbox<B>> f) => 
        new OutboxBind<A, B>(this, f);

    public override Outbox<B> ApplyBack<B>(Outbox<Func<A, B>> ff) => 
        new OutboxApply<A, B>(this, ff);

    public override IO<A> Read() =>
        IO.pure(Value);
    
    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new(true);
}

record OutboxEmpty<A> : Outbox<A>
{
    public static readonly Outbox<A> Default = new OutboxEmpty<A>();
    
    public override Outbox<B> Map<B>(Func<A, B> f) => 
        new OutboxMap<A, B>(this, f);

    public override Outbox<B> Bind<B>(Func<A, Outbox<B>> f) => 
        new OutboxBind<A, B>(this, f);

    public override Outbox<B> ApplyBack<B>(Outbox<Func<A, B>> ff) => 
        new OutboxApply<A, B>(this, ff);

    public override IO<A> Read() =>
        IO.fail<A>(Errors.OutboxChannelClosed);
    
    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        new(false);
}

record OutboxReader<A>(ChannelReader<A> Reader) : Outbox<A>
{
    public override Outbox<B> Map<B>(Func<A, B> f) => 
        new OutboxMap<A, B>(this, f);

    public override Outbox<B> Bind<B>(Func<A, Outbox<B>> f) => 
        new OutboxBind<A, B>(this, f);

    public override Outbox<B> ApplyBack<B>(Outbox<Func<A, B>> ff) => 
        new OutboxApply<A, B>(this, ff);

    public override IO<A> Read() =>
        IO.liftVAsync(e => Reader.WaitToReadAsync(e.Token))
          .Bind(f => f ? IO.liftVAsync(e => Reader.ReadAsync(e.Token))
                       : IO.fail<A>(Errors.OutboxChannelClosed));
    
    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        Reader.WaitToReadAsync(token);
}

record OutboxMap<A, B>(Outbox<A> Outbox, Func<A, B> F) : Outbox<B>
{
    public override Outbox<C> Map<C>(Func<B, C> f) =>
        new OutboxMap<A, C>(Outbox, x => f(F(x)));

    public override Outbox<C> Bind<C>(Func<B, Outbox<C>> f) => 
        new OutboxBind<A, C>(Outbox, x => f(F(x)));

    public override Outbox<C> ApplyBack<C>(Outbox<Func<B, C>> ff) => 
        new OutboxApply<B, C>(this, ff);

    public override IO<B> Read() =>
        Outbox.Read().Map(F);
    
    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        Outbox.ReadyToRead(token);
}

record OutboxApply<A, B>(Outbox<A> Outbox, Outbox<Func<A, B>> FF) : Outbox<B>
{
    public override Outbox<C> Map<C>(Func<B, C> f) =>
        new OutboxMap<B, C>(this, f);

    public override Outbox<C> Bind<C>(Func<B, Outbox<C>> f) => 
        new OutboxBind<B, C>(this, f);

    public override Outbox<C> ApplyBack<C>(Outbox<Func<B, C>> ff) => 
        new OutboxApply<B, C>(this, ff);

    public override IO<B> Read() =>
        FF.Read().Apply(Outbox.Read());
    
    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        Outbox.ReadyToRead(token);
}

record OutboxBind<A, B>(Outbox<A> Outbox, Func<A, Outbox<B>> F) : Outbox<B>
{
    public override Outbox<C> Map<C>(Func<B, C> f) =>
        new OutboxMap<B, C>(this, f);

    public override Outbox<C> Bind<C>(Func<B, Outbox<C>> f) =>
        new OutboxBind<B, C>(this, f);

    public override Outbox<C> ApplyBack<C>(Outbox<Func<B, C>> ff) =>
        new OutboxApply<B, C>(this, ff);

    public override IO<B> Read() =>
        Outbox.Read().Bind(x => F(x).Read());

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        Outbox.ReadyToRead(token);
}

record OutboxCombine<A>(Seq<Outbox<A>> Sources) : Outbox<A>
{
    public override Outbox<B> Map<B>(Func<A, B> f) =>
        new OutboxMap<A, B>(this, f);

    public override Outbox<B> Bind<B>(Func<A, Outbox<B>> f) =>
        new OutboxBind<A, B>(this, f);

    public override Outbox<B> ApplyBack<B>(Outbox<Func<A, B>> ff) =>
        new OutboxApply<A, B>(this, ff);

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        OutboxInternal.ReadyToRead(Sources, token);

    public override IO<A> Read() =>
        IO.liftVAsync<A>(e => OutboxInternal.Read(Sources, e));
}

record OutboxChoose<A>(Outbox<A> Left, Outbox<A> Right) : Outbox<A>
{
    public override Outbox<B> Map<B>(Func<A, B> f) =>
        new OutboxMap<A, B>(this, f);

    public override Outbox<B> Bind<B>(Func<A, Outbox<B>> f) =>
        new OutboxBind<A, B>(this, f);

    public override Outbox<B> ApplyBack<B>(Outbox<Func<A, B>> ff) =>
        new OutboxApply<A, B>(this, ff);

    public override IO<A> Read() =>
        Left.Read() | Right.Read();

    internal override ValueTask<bool> ReadyToRead(CancellationToken token) =>
        OutboxInternal.ReadyToRead([Left, Right], token);
}
