using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt;

record SinkCombine<A, B, C>(Func<A, (B Left, C Right)> F, Sink<B> Left, Sink<C> Right) : Sink<A>
{
    public override Sink<D> Comap<D>(Func<D, A> f) =>
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
