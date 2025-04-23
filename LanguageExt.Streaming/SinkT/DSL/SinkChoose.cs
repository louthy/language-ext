using System;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt;

record SinkTChoose<M, A, B, C>(Func<A, Either<B, C>> F, SinkT<M, B> Left, SinkT<M, C> Right) : SinkT<M, A>
    where M : MonadIO<M>
{
    public override SinkT<M, D> Comap<D>(Func<D, A> f) =>
        new SinkTContraMap<M, A, D>(f, this);

    public override K<M, Unit> PostM(K<M, A> ma) =>
        ma.Bind(value =>
            F(value) switch
            {
                Either.Left<B, C>(var left)   => Left.PostM(M.Pure(left)),
                Either.Right<B, C>(var right) => Right.PostM(M.Pure(right)),
                _                             => M.LiftIO(IO.fail<Unit>(Errors.SinkFull))
            });

    public override K<M, Unit> Complete() =>
        Left.Complete().Bind(_ => Right.Complete());

    public override K<M, Unit> Fail(Error error) =>
        Left.Fail(error).Bind(_ => Right.Fail(error));
}
