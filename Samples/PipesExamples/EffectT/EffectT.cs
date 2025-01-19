using LanguageExt.Traits;

namespace LanguageExt.Pipes2;

public readonly record struct EffectT<M, A>(PipeT<Unit, Void, M, A> Proxy)
    where M : Monad<M>
{
    public EffectT<M, B> Map<B>(Func<A, B> f) =>
        Proxy.Map(f);

    public EffectT<M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        Proxy.MapM(f);
    
    public EffectT<M, B> ApplyBack<B>(EffectT<M, Func<A, B>> ff) =>
        Proxy.ApplyBack(ff.Proxy);
    
    public EffectT<M, B> Action<B>(EffectT<M, B> fb) =>
        Proxy.Action(fb.Proxy);
    
    public EffectT<M, B> Bind<B>(Func<A, EffectT<M, B>> f) =>
        Proxy.Bind(x => f(x).Proxy);
    
    public EffectT<M, B> Bind<B>(Func<A, K<M, B>> f) => 
        Bind(x => EffectT.liftM(f(x)));
    
    public EffectT<M, B> Select<B>(Func<A, B> f) =>
        Map(f);
   
    public EffectT<M, C> SelectMany<B, C>(Func<A, EffectT<M, B>> f, Func<A, B, C> g) =>
        Bind(x => f(x).Map(y => g(x, y)));
   
    public EffectT<M, C> SelectMany<B, C>(Func<A, K<M, B>> f, Func<A, B, C> g) =>
        Bind(x => f(x).Map(y => g(x, y)));

    public K<M, A> Run() =>
        Proxy.Run();

    public ValueTask<K<M, A>> RunAsync() =>
        Proxy.RunAsync();
    
    public static implicit operator EffectT<M, A>(PipeT<Unit, Void, M, A> pipe) =>
        pipe.ToEffect();
}
