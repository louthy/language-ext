using LanguageExt.Traits;

namespace LanguageExt.Pipes2;

/// <summary>
/// `ConsumerT` streaming consumer monad-transformer instance
/// </summary>
public readonly record struct ConsumerT<IN, M, A>(PipeT<IN, Void, M, A> Proxy)
    where M : Monad<M>
{
    public ConsumerT<IN, M, B> Map<B>(Func<A, B> f) =>
        new (Proxy.Map(f));
    
    public ConsumerT<IN, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new (Proxy.MapM(f));

    public ConsumerT<IN, M, B> ApplyBack<B>(ConsumerT<IN, M, Func<A, B>> ff) =>
        new (Proxy.ApplyBack(ff.Proxy));
    
    public ConsumerT<IN, M, B> Action<B>(ConsumerT<IN, M, B> fb) =>
        new (Proxy.Action(fb.Proxy));
    
    public ConsumerT<IN, M, B> Bind<B>(Func<A, ConsumerT<IN, M, B>> f) =>
        new (Proxy.Bind(x => f(x).Proxy));

    internal K<M, A> Run() =>
        Proxy.Run();
    
    public ConsumerT<IN, M, B> Bind<B>(Func<A, K<M, B>> f) => 
        Bind(x => ConsumerT.liftM<IN, M, B>(f(x))); 
    
    public ConsumerT<IN, M, B> Select<B>(Func<A, B> f) =>
        Map(f);
   
    public ConsumerT<IN, M, C> SelectMany<B, C>(Func<A, ConsumerT<IN, M, B>> f, Func<A, B, C> g) =>
        Bind(x => f(x).Map(y => g(x, y)));
   
    public ConsumerT<IN, M, C> SelectMany<B, C>(Func<A, K<M, B>> f, Func<A, B, C> g) =>
        Bind(x => f(x).Map(y => g(x, y)));

    public static implicit operator ConsumerT<IN, M, A>(PipeT<IN, Void, M, A> pipe) =>
        pipe.ToConsumer();
}
