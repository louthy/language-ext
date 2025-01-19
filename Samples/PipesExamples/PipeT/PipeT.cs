using LanguageExt.Traits;

namespace LanguageExt.Pipes2;

/// <summary>
/// `PipeT` monad-transformer base-type for Pipes streaming components:
///
///   * `ProducerT` is a `PipeT` with the `IN` 'closed off' with `Unit`
///   * `ConsumerT` is a `PipeT` with the `OUT` 'closed off' with `Void`
///   * `EffectT` is a `PipeT` with the `IN` and `OUT` 'closed off' with `Unit` upstream and `Void` downstream
/// 
/// </summary>
/// <typeparam name="IN">Type of values to be consumed</typeparam>
/// <typeparam name="OUT">Type of values to be produced</typeparam>
/// <typeparam name="M">Lifted monad type</typeparam>
/// <typeparam name="A">Bound value type</typeparam>
public abstract record PipeT<IN, OUT, M, A>
    where M : Monad<M>
{
    public PipeT<IN, OUT1, M, A> Compose<OUT1>(PipeT<OUT, OUT1, M, A> rhs) =>
        rhs.PairEachAwaitWithYield(_ => this);

    public static PipeT<IN, OUT, M, A> operator | (PipeT<IN, OUT, M, A> lhs, PipeT<OUT, OUT, M, A> rhs) =>
        lhs.Compose(rhs);

    public static ProducerT<OUT, M, A> operator | (ProducerT<IN, M, A> lhs, PipeT<IN, OUT, M, A> rhs) =>
        lhs.Compose(rhs);

    public static ConsumerT<IN, M, A> operator | (PipeT<IN, OUT, M, A> lhs, ConsumerT<OUT, M, A> rhs) =>
        lhs.Compose(rhs.Proxy);
    
    public abstract PipeT<IN, OUT, M, B> Map<B>(Func<A, B> f);
    public abstract PipeT<IN, OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f);
    public abstract PipeT<IN, OUT, M, B> ApplyBack<B>(PipeT<IN, OUT, M, Func<A, B>> ff);
    public abstract PipeT<IN, OUT, M, B> Action<B>(PipeT<IN, OUT, M, B> fb);
    public abstract PipeT<IN, OUT, M, B> Bind<B>(Func<A, PipeT<IN, OUT, M, B>> f);
    public PipeT<IN, OUT, M, B> BindAsync<B>(Func<A, ValueTask<PipeT<IN, OUT, M, B>>> f) =>
        Bind(x => ProxyT.liftT(f(x)));
    
    internal abstract PipeT<IN1, OUT, M, A> ReplaceAwait<IN1>(Func<PipeT<IN1, OUT, M, IN>> request);
    internal abstract PipeT<IN, OUT1, M, A> ReplaceYield<OUT1>(Func<OUT, PipeT<IN, OUT1, M, Unit>> response);
    internal abstract PipeT<IN1, OUT, M, A> PairEachAwaitWithYield<IN1>(Func<Unit, PipeT<IN1, IN, M, A>> request);
    internal abstract PipeT<IN, OUT1, M, A> PairEachYieldWithAwait<OUT1>(Func<OUT, PipeT<OUT, OUT1, M, A>> response);
    
    internal abstract K<M, A> Run();
    internal virtual ValueTask<K<M, A>> RunAsync() =>
        new (Run());
}
