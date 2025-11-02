using System;
using LanguageExt.Common;

namespace LanguageExt;

record SinkChoose<A, B, C>(Func<A, Either<B, C>> F, Sink<B> Left, Sink<C> Right) : Sink<A>
{
    public override Sink<D> Comap<D>(Func<D, A> f) =>
        new SinkContraMap<A, D>(f, this);

    public override IO<Unit> Post(A value) =>
        F(value) switch
        {
            Either<B, C>.Left(var left)   => Left.Post(left),
            Either<B, C>.Right(var right) => Right.Post(right),
            _                             => IO.fail<Unit>(Errors.SinkFull)
        };

    public override IO<Unit> Complete() =>
        Left.Complete().Bind(_ => Right.Complete());

    public override IO<Unit> Fail(Error error) =>
        Left.Fail(error).Bind(_ => Right.Fail(error));
}
