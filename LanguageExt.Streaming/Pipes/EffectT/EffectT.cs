using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public readonly record struct EffectT<M, A>(PipeT<Unit, Void, M, A> Proxy) : K<EffectT<M>, A>
    where M : MonadIO<M>
{
    [Pure]
    public EffectT<M, B> Map<B>(Func<A, B> f) =>
        Proxy.Map(f);

    [Pure]
    public EffectT<M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        Proxy.MapM(f);
    
    [Pure]
    public EffectT<M, B> ApplyBack<B>(EffectT<M, Func<A, B>> ff) =>
        Proxy.ApplyBack(ff.Proxy);
    
    [Pure]
    public EffectT<M, B> Action<B>(EffectT<M, B> fb) =>
        Proxy.Action(fb.Proxy);
    
    [Pure]
    public EffectT<M, B> Bind<B>(Func<A, EffectT<M, B>> f) =>
        Proxy.Bind(x => f(x).Proxy);
    
    [Pure]
    public EffectT<M, B> Bind<B>(Func<A, K<M, B>> f) => 
        Proxy.Bind(f);
    
    [Pure]
    public EffectT<M, B> Bind<B>(Func<A, IO<B>> f) => 
        Proxy.Bind(f);
       
    [Pure]
    public EffectT<M, B> Bind<B>(Func<A, Pure<B>> f) =>
        Proxy.Bind(f);
   
    [Pure]
    public EffectT<M, B> Bind<B>(Func<A, Lift<B>> f) =>
        Proxy.Bind(f);

    [Pure]
    public EffectT<M, B> Select<B>(Func<A, B> f) =>
        Proxy.Map(f);
   
    [Pure]
    public EffectT<M, C> SelectMany<B, C>(Func<A, EffectT<M, B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(x => f(x).Proxy, g);

    [Pure]
    public EffectT<M, C> SelectMany<B, C>(Func<A, K<M, B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(f, g);

    [Pure]
    public EffectT<M, C> SelectMany<B, C>(Func<A, IO<B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(f, g);
   
    [Pure]
    public EffectT<M, C> SelectMany<B, C>(Func<A, Pure<B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(f, g);
   
    [Pure]
    public EffectT<M, C> SelectMany<B, C>(Func<A, Lift<B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(f, g);

    [Pure]
    public K<M, A> Run() =>
        Proxy.Run();

    [Pure]
    public ValueTask<K<M, A>> RunAsync() =>
        Proxy.RunAsync();
    
    [Pure]
    public static implicit operator EffectT<M, A>(PipeT<Unit, Void, M, A> pipe) =>
        pipe.ToEffect();
    
    [Pure]
    public static implicit operator EffectT<M, A>(Pure<A> rhs) =>
        EffectT.pure<M, A>(rhs.Value);
    
    [Pure]
    public static EffectT<M, A> operator |(Schedule lhs, EffectT<M, A> rhs) =>
        ReferenceEquals(lhs, Schedule.Forever)
            ? EffectT.repeat(rhs)
            : EffectT.repeat(lhs, rhs);
    
    [Pure]
    public static EffectT<M, A> operator |(EffectT<M, A> lhs, Schedule rhs) =>
        ReferenceEquals(rhs, Schedule.Forever)
            ? EffectT.repeat(lhs)
            : EffectT.repeat(rhs, lhs);    
}
