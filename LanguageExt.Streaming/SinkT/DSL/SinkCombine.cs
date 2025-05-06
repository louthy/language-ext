using System;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

record SinkTCombine<M, A, B, C>(Func<A, (B Left, C Right)> F, SinkT<M, B> Left, SinkT<M, C> Right) : SinkT<M, A>
    where M : MonadIO<M>
{
    public override SinkT<M, D> Comap<D>(Func<D, A> f) =>
        new SinkTContraMap<M, A, D>(f, this);

    public override K<M, Unit> PostM(K<M, A> ma) =>
        ma.Bind(value =>
                    F(value) switch
                    {
                        var (b, c) =>
                            awaitAll(Left.PostM(M.Pure(b)).Map(Seq<Error>()), Right.PostM(M.Pure(c)).Map(Seq<Error>()))
                               .Map(es => es.Flatten())
                               .Bind(es => es switch
                                           {
                                               [] => unitIO,
                                               _  => IO.fail<Unit>(Error.Many(es))
                                           })
                    });

    static K<M, Seq<X>> awaitAll<X>(params K<M, X>[] ms) =>
        toSeq(ms)
           .Traverse(M.ToIO)
           .Bind(Prelude.awaitAll);
    
    public override K<M, Unit> Complete() =>
        Left.Complete().Bind(_ => Right.Complete());

    public override K<M, Unit> Fail(Error error) =>
        Left.Fail(error).Bind(_ => Right.Fail(error));
}
