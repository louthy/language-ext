using System;
using System.Threading.Channels;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record InboxWriter<A>(ChannelWriter<A> Writer) : Inbox<A>
{
    public override Inbox<B> ContraMap<B>(Func<B, A> f) => 
        new InboxContraMap<A, B>(f, this);

    public override IO<Unit> Post(A value) =>
        from f in IO.liftVAsync(e => Writer.WaitToWriteAsync(e.Token))
        from r in f ? IO.liftVAsync(() => Writer.WriteAsync(value).ToUnit()) 
                    : IO.fail<Unit>(Errors.NoSpaceInInbox)
        select r;

    public override IO<Unit> Complete() =>
        IO.lift(() => Writer.Complete());    

    public override IO<Unit> Fail(Error error) =>
        IO.lift(() => Writer.Complete(error.ToException()));    
}

record InboxContraMap<A, B>(Func<B, A> F, Inbox<A> Inbox) : Inbox<B>
{
    public override Inbox<C> ContraMap<C>(Func<C, B> f) =>
        new InboxContraMap<A, C>(x => F(f(x)), Inbox);

    public override IO<Unit> Post(B value) => 
        Inbox.Post(F(value));

    public override IO<Unit> Complete() => 
        Inbox.Complete();

    public override IO<Unit> Fail(Error Error) => 
        Inbox.Fail(Error);
}

record InboxEmpty<A> : Inbox<A>
{
    public static readonly Inbox<A> Default = new InboxEmpty<A>();
    
    public override Inbox<B> ContraMap<B>(Func<B, A> f) => 
        new InboxEmpty<B>();

    public override IO<Unit> Post(A value) =>
        IO.fail<Unit>(Errors.NoSpaceInInbox);

    public override IO<Unit> Complete() =>
        unitIO;

    public override IO<Unit> Fail(Error error) =>
        unitIO;
}

record InboxVoid<A> : Inbox<A>
{
    public static readonly Inbox<A> Default = new InboxVoid<A>();
    
    public override Inbox<B> ContraMap<B>(Func<B, A> f) => 
        new InboxVoid<B>();

    public override IO<Unit> Post(A value) =>
        unitIO;

    public override IO<Unit> Complete() =>
        unitIO;

    public override IO<Unit> Fail(Error error) =>
        unitIO;
}

record InboxCombine<A, B, C>(Func<A, (B Left, C Right)> F, Inbox<B> Left, Inbox<C> Right) : Inbox<A>
{
    public static readonly Inbox<A> Default = new InboxEmpty<A>();

    public override Inbox<D> ContraMap<D>(Func<D, A> f) =>
        new InboxContraMap<A, D>(f, this);

    public override IO<Unit> Post(A value) =>
        F(value) switch
        {
            var (b, c) =>
                awaitAll(Left.Post(b).Map(Seq<Error>()).Catch(_ => true, e => IO.pure(Seq(e))),
                         Right.Post(c).Map(Seq<Error>()).Catch(_ => true, e => IO.pure(Seq(e))))
                   .Map(es => es.Flatten())
                   .Bind(es => es switch
                               {
                                   [] => unitIO,
                                   _  => IO.fail<Unit>(Error.Many(es))
                               })
        };

    public override IO<Unit> Complete() =>
        Left.Complete().Bind(_ => Right.Complete());

    public override IO<Unit> Fail(Error error) =>
        Left.Fail(error).Bind(_ => Right.Fail(error));
}

record InboxChoose<A, B, C>(Func<A, Either<B, C>> F, Inbox<B> Left, Inbox<C> Right) : Inbox<A>
{
    public static readonly Inbox<A> Default = new InboxEmpty<A>();

    public override Inbox<D> ContraMap<D>(Func<D, A> f) =>
        new InboxContraMap<A, D>(f, this);

    public override IO<Unit> Post(A value) =>
        F(value) switch
        {
            Either.Left<B, C>(var left)   => Left.Post(left),
            Either.Right<B, C>(var right) => Right.Post(right),
            _                             => IO.fail<Unit>(Errors.NoSpaceInInbox)
        };

    public override IO<Unit> Complete() =>
        Left.Complete().Bind(_ => Right.Complete());

    public override IO<Unit> Fail(Error error) =>
        Left.Fail(error).Bind(_ => Right.Fail(error));
}
