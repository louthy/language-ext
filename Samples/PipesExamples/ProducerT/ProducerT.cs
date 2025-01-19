using LanguageExt.Traits;

namespace LanguageExt.Pipes2;

/// <summary>
/// `ProducerT` streaming producer monad-transformer instance
/// </summary>
public readonly record struct ProducerT<OUT, M, A>(PipeT<Unit, OUT, M, A> Proxy)
    where M : Monad<M>
{
    public ProducerT<OUT1, M, A> Compose<OUT1>(PipeT<OUT, OUT1, M, A> rhs)
    {
        var p = Proxy;
        return rhs.PairEachAwaitWithYield(_ => p);
    }

    public EffectT<M, A> Compose(ConsumerT<OUT, M, A> rhs)
    {
        var p = Proxy;
        return rhs.Proxy.PairEachAwaitWithYield(_ => p);
    }

    public static ProducerT<OUT, M, A> operator | (ProducerT<OUT, M, A> lhs, PipeT<OUT, OUT, M, A> rhs) =>
        lhs.Compose(rhs);

    public static EffectT<M, A> operator | (ProducerT<OUT, M, A> lhs, ConsumerT<OUT, M, A> rhs) =>
        lhs.Compose(rhs);
    
    public ProducerT<OUT, M, B> Map<B>(Func<A, B> f) =>
        Proxy.Map(f);
    
    public ProducerT<OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        Proxy.MapM(f);

    public ProducerT<OUT, M, B> ApplyBack<B>(ProducerT<OUT, M, Func<A, B>> ff) =>
        Proxy.ApplyBack(ff.Proxy);
    
    public ProducerT<OUT, M, B> Action<B>(ProducerT<OUT, M, B> fb) =>
        Proxy.Action(fb.Proxy);
    
    public ProducerT<OUT, M, B> Bind<B>(Func<A, ProducerT<OUT, M, B>> f) =>
        Proxy.Bind(x => f(x).Proxy);

    internal K<M, A> Run() =>
        Proxy.Run();
    
    public ProducerT<OUT, M, B> Bind<B>(Func<A, K<M, B>> f) => 
        Bind(x => ProducerT.liftM<OUT, M, B>(f(x))); 
    
    public ProducerT<OUT, M, B> Select<B>(Func<A, B> f) =>
        Map(f);
   
    public ProducerT<OUT, M, C> SelectMany<B, C>(Func<A, ProducerT<OUT, M, B>> f, Func<A, B, C> g) =>
        Bind(x => f(x).Map(y => g(x, y)));
   
    public ProducerT<OUT, M, C> SelectMany<B, C>(Func<A, K<M, B>> f, Func<A, B, C> g) =>
        Bind(x => f(x).Map(y => g(x, y)));

    public static implicit operator ProducerT<OUT, M, A>(PipeT<Unit, OUT, M, A> pipe) =>
        pipe.ToProducer();
}
