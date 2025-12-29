using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public readonly record struct Effect<RT, A>(PipeT<Unit, Void, Eff<RT>, A> Proxy) : K<Effect<RT>, A>
{
    [Pure]
    public Effect<RT, B> Map<B>(Func<A, B> f) =>
        Proxy.Map(f);

    [Pure]
    public Effect<RT, B> MapM<B>(Func<Eff<RT, A>, Eff<RT, B>> f) =>
        Proxy.MapM(mx => f(mx.As())).ToEffect();

    [Pure]
    public Effect<RT, B> MapIO<B>(Func<IO<A>, IO<B>> f) =>
        Proxy.MapIO(f);
    
    [Pure]
    public Effect<RT, B> ApplyBack<B>(Effect<RT, Func<A, B>> ff) =>
        Proxy.ApplyBack(ff.Proxy);
    
    [Pure]
    public Effect<RT, B> Action<B>(Effect<RT, B> fb) =>
        Proxy.Action(fb.Proxy);
    
    [Pure]
    public Effect<RT, B> Bind<B>(Func<A, Effect<RT, B>> f) =>
        Proxy.Bind(x => f(x).Proxy);
    
    [Pure]
    public Effect<RT, B> Bind<B>(Func<A, K<Eff<RT>, B>> f) => 
        Proxy.Bind(f);
    
    [Pure]
    public Effect<RT, B> Bind<B>(Func<A, IO<B>> f) => 
        Proxy.Bind(f);
       
    [Pure]
    public Effect<RT, B> Bind<B>(Func<A, Pure<B>> f) =>
        Proxy.Bind(f);
   
    [Pure]
    public Effect<RT, B> Bind<B>(Func<A, Lift<B>> f) =>
        Proxy.Bind(f);

    [Pure]
    public Effect<RT, B> Select<B>(Func<A, B> f) =>
        Proxy.Map(f);
   
    [Pure]
    public Effect<RT, C> SelectMany<B, C>(Func<A, Effect<RT, B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(x => f(x).Proxy, g);

    [Pure]
    public Effect<RT, C> SelectMany<B, C>(Func<A, K<Eff<RT>, B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(f, g);

    [Pure]
    public Effect<RT, C> SelectMany<B, C>(Func<A, IO<B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(f, g);
   
    [Pure]
    public Effect<RT, C> SelectMany<B, C>(Func<A, Pure<B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(f, g);
   
    [Pure]
    public Effect<RT, C> SelectMany<B, C>(Func<A, Lift<B>> f, Func<A, B, C> g) =>
        Proxy.SelectMany(f, g);

    [Pure]
    public Eff<RT, A> Run() =>
        Proxy.Run().As();
    
    [Pure]
    public static implicit operator Effect<RT, A>(PipeT<Unit, Void, Eff<RT>, A> pipe) =>
        pipe.ToEffect();
    
    [Pure]
    public static implicit operator Effect<RT, A>(EffectT<Eff<RT>, A> pipe) =>
        pipe.Proxy;
    
    [Pure]
    public static implicit operator Effect<RT, A>(Pure<A> rhs) =>
        Effect.pure<RT, A>(rhs.Value);
    
    [Pure]
    public static Effect<RT, A> operator |(Schedule lhs, Effect<RT, A> rhs) =>
        ReferenceEquals(lhs, Schedule.Forever)
            ? Effect.repeat(rhs)
            : Effect.repeat(lhs, rhs);
    
    [Pure]
    public static Effect<RT, A> operator |(Effect<RT, A> lhs, Schedule rhs) =>
        ReferenceEquals(rhs, Schedule.Forever)
            ? Effect.repeat(lhs)
            : Effect.repeat(rhs, lhs);      
}
