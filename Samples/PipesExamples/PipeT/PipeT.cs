using System.Diagnostics.Contracts;
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
public abstract record PipeT<IN, OUT, M, A> : K<PipeT<IN, OUT, M>, A>
    where M : Monad<M>
{
    [Pure]
    public PipeT<IN, OUT1, M, A> Compose<OUT1>(PipeT<OUT, OUT1, M, A> rhs) =>
        rhs.PairEachAwaitWithYield(_ => this);

    [Pure]
    public ConsumerT<IN, M, A> Compose(ConsumerT<OUT, M, A> rhs) =>
        rhs.Proxy.PairEachAwaitWithYield(_ => this);

    [Pure]
    public static PipeT<IN, OUT, M, A> operator | (PipeT<IN, OUT, M, A> lhs, PipeT<OUT, OUT, M, A> rhs) =>
        lhs.Compose(rhs);

    [Pure]
    public static ProducerT<OUT, M, A> operator | (ProducerT<IN, M, A> lhs, PipeT<IN, OUT, M, A> rhs) =>
        lhs.Compose(rhs);

    [Pure]
    public static ProducerT<OUT, M, A> operator | (QueueT<IN, M, A> lhs, PipeT<IN, OUT, M, A> rhs) =>
        lhs.Compose(rhs);

    [Pure]
    public static ConsumerT<IN, M, A> operator | (PipeT<IN, OUT, M, A> lhs, ConsumerT<OUT, M, A> rhs) =>
        lhs.Compose(rhs.Proxy);
    
    [Pure]
    public abstract PipeT<IN, OUT, M, B> Map<B>(Func<A, B> f);
    
    [Pure]
    public abstract PipeT<IN, OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f);
    
    [Pure]
    public abstract PipeT<IN, OUT, M, B> ApplyBack<B>(PipeT<IN, OUT, M, Func<A, B>> ff);
    
    [Pure]
    public abstract PipeT<IN, OUT, M, B> Action<B>(PipeT<IN, OUT, M, B> fb);
    
    [Pure]
    public abstract PipeT<IN, OUT, M, B> Bind<B>(Func<A, PipeT<IN, OUT, M, B>> f);
    
    [Pure]
    public PipeT<IN, OUT, M, B> Bind<B>(Func<A, IO<B>> f) =>
        Bind(x => PipeT.liftIO<IN, OUT, M, B>(f(x)));
    
    [Pure]
    public PipeT<IN, OUT, M, B> Bind<B>(Func<A, K<M, B>> f) =>
        Bind(x => PipeT.liftM<IN, OUT, M, B>(f(x)));
    
    [Pure]
    public PipeT<IN, OUT, M, B> Bind<B>(Func<A, Pure<B>> f) =>
        Map(x => f(x).Value);
    
    [Pure]
    public PipeT<IN, OUT, M, B> Bind<B>(Func<A, Lift<B>> f) =>
        Map(x => f(x).Function());
    
    [Pure]
    public PipeT<IN, OUT, M, B> BindAsync<B>(Func<A, ValueTask<PipeT<IN, OUT, M, B>>> f) =>
        Bind(x => PipeT.liftT(f(x)));
    
    [Pure]
    public PipeT<IN, OUT, M, B> MapIO<B>(Func<IO<A>, IO<B>> f) =>
        MapM(ma => ma.MapIO(f));

    [Pure]
    public PipeT<IN, OUT, M, B> Select<B>(Func<A, B> f) =>
        Map(f);
   
    [Pure]
    public PipeT<IN, OUT, M, C> SelectMany<B, C>(Func<A, PipeT<IN, OUT, M, B>> f, Func<A, B, C> g) =>
        Bind(x => f(x).Map(y => g(x, y)));
   
    [Pure]
    public PipeT<IN, OUT, M, C> SelectMany<B, C>(Func<A, K<M, B>> f, Func<A, B, C> g) =>
        Bind(x => f(x).Map(y => g(x, y)));
   
    [Pure]
    public PipeT<IN, OUT, M, C> SelectMany<B, C>(Func<A, IO<B>> f, Func<A, B, C> g) =>
        Bind(x => f(x).Map(y => g(x, y)));
   
    [Pure]
    public PipeT<IN, OUT, M, C> SelectMany<B, C>(Func<A, Pure<B>> f, Func<A, B, C> g) =>
        Map(x => g(x, f(x).Value));
   
    [Pure]
    public PipeT<IN, OUT, M, C> SelectMany<B, C>(Func<A, Lift<B>> f, Func<A, B, C> g) =>
        Map(x => g(x, f(x).Function()));
    
    [Pure]
    internal abstract PipeT<IN1, OUT, M, A> ReplaceAwait<IN1>(Func<PipeT<IN1, OUT, M, IN>> producer);
    
    [Pure]
    internal abstract PipeT<IN, OUT1, M, A> ReplaceYield<OUT1>(Func<OUT, PipeT<IN, OUT1, M, Unit>> consumer);
    
    [Pure]
    internal abstract PipeT<IN1, OUT, M, A> PairEachAwaitWithYield<IN1>(Func<Unit, PipeT<IN1, IN, M, A>> producer);
    
    [Pure]
    internal abstract PipeT<IN, OUT1, M, A> PairEachYieldWithAwait<OUT1>(Func<OUT, PipeT<OUT, OUT1, M, A>> consumer);
    
    [Pure]
    internal abstract K<M, A> Run();
    
    [Pure]
    internal virtual ValueTask<K<M, A>> RunAsync() =>
        new (Run());
}
