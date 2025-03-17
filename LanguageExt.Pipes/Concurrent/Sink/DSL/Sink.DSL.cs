using System;
using System.Threading.Channels;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt.Pipes.Concurrent;

record SinkWriter<A>(ChannelWriter<A> Writer) : Sink<A>
{
    public override Sink<B> Contramap<B>(Func<B, A> f) => 
        new SinkContraMap<A, B>(f, this);

    public override IO<Unit> Post(A value) =>
        from f in IO.liftVAsync(e => Writer.WaitToWriteAsync(e.Token))
        from r in f ? IO.liftVAsync(() => Writer.WriteAsync(value).ToUnit())
                    : IO.fail<Unit>(Errors.SinkFull)
        select r;

    public override IO<Unit> Complete() =>
        IO.lift(() => Writer.Complete());    

    public override IO<Unit> Fail(Error error) =>
        IO.lift(() => Writer.Complete(error.ToException()));    
}

record SinkContraMap<A, B>(Func<B, A> F, Sink<A> Sink) : Sink<B>
{
    public override Sink<C> Contramap<C>(Func<C, B> f) =>
        new SinkContraMap<A, C>(x => F(f(x)), Sink);

    public override IO<Unit> Post(B value) => 
        Sink.Post(F(value));

    public override IO<Unit> Complete() => 
        Sink.Complete();

    public override IO<Unit> Fail(Error Error) => 
        Sink.Fail(Error);
}

record SinkEmpty<A> : Sink<A>
{
    public static readonly Sink<A> Default = new SinkEmpty<A>();
    
    public override Sink<B> Contramap<B>(Func<B, A> f) => 
        new SinkEmpty<B>();

    public override IO<Unit> Post(A value) =>
        IO.fail<Unit>(Errors.SinkFull);

    public override IO<Unit> Complete() =>
        unitIO;

    public override IO<Unit> Fail(Error error) =>
        unitIO;
}

record SinkVoid<A> : Sink<A>
{
    public static readonly Sink<A> Default = new SinkVoid<A>();
    
    public override Sink<B> Contramap<B>(Func<B, A> f) => 
        new SinkVoid<B>();

    public override IO<Unit> Post(A value) =>
        unitIO;

    public override IO<Unit> Complete() =>
        unitIO;

    public override IO<Unit> Fail(Error error) =>
        unitIO;
}

record SinkCombine<A, B, C>(Func<A, (B Left, C Right)> F, Sink<B> Left, Sink<C> Right) : Sink<A>
{
    public static readonly Sink<A> Default = new SinkEmpty<A>();

    public override Sink<D> Contramap<D>(Func<D, A> f) =>
        new SinkContraMap<A, D>(f, this);

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
                               }).As()
        };

    public override IO<Unit> Complete() =>
        Left.Complete().Bind(_ => Right.Complete());

    public override IO<Unit> Fail(Error error) =>
        Left.Fail(error).Bind(_ => Right.Fail(error));
}

record SinkChoose<A, B, C>(Func<A, Either<B, C>> F, Sink<B> Left, Sink<C> Right) : Sink<A>
{
    public static readonly Sink<A> Default = new SinkEmpty<A>();

    public override Sink<D> Contramap<D>(Func<D, A> f) =>
        new SinkContraMap<A, D>(f, this);

    public override IO<Unit> Post(A value) =>
        F(value) switch
        {
            Either.Left<B, C>(var left)   => Left.Post(left),
            Either.Right<B, C>(var right) => Right.Post(right),
            _                             => IO.fail<Unit>(Errors.SinkFull)
        };

    public override IO<Unit> Complete() =>
        Left.Complete().Bind(_ => Right.Complete());

    public override IO<Unit> Fail(Error error) =>
        Left.Fail(error).Bind(_ => Right.Fail(error));
}
