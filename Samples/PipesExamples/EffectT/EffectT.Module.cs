using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes2;

/// <summary>
/// `EffectT` streaming effect monad-transformer
/// </summary>
public static class EffectT
{
    /// <summary>
    /// Create an effect that simply returns a bound value without yielding anything
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> pure<M, A>(A value)
        where M : Monad<M> =>
        ProxyT.pure<Unit, Void, M, A>(value);
    
    /// <summary>
    /// Create an effect that always fails
    /// </summary>
    /// <typeparam name="E">Failure type</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> fail<E, M, A>(E value) 
        where M : Monad<M>, Fallible<E, M> =>
        ProxyT.fail<Unit, Void, E, M, A>(value);
    
    /// <summary>
    /// Create an effect that always fails
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> error<M, A>(Error value) 
        where M : Monad<M>, Fallible<M> =>
        ProxyT.error<Unit, Void, M, A>(value);
    
    /// <summary>
    /// Create an effect that yields nothing at all
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> empty<M, A>() 
        where M : Monad<M>, MonoidK<M> =>
        ProxyT.empty<Unit, Void, M, A>();
    
    /// <summary>
    /// Create an effect that lazily returns a bound value without yielding anything
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> lift<M, A>(Func<A> f) 
        where M : Monad<M> =>
        ProxyT.lift<Unit, Void, M, A>(f);
    
    /// <summary>
    /// Create an effect that simply returns the bound value of the lifted monad without yielding anything
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> liftM<M, A>(K<M, A> ma) 
        where M : Monad<M> =>
        ProxyT.liftM<Unit, Void, M, A>(ma);
        
    /// <summary>
    /// Create a lazy proxy 
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> liftT<M, A>(Func<EffectT<M, A>> f) 
        where M : Monad<M> =>
        ProxyT.liftT(() => f().Proxy);
    
    /// <summary>
    /// Create an asynchronous lazy proxy 
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> liftT<M, A>(Func<ValueTask<EffectT<M, A>>> f) 
        where M : Monad<M> =>
        ProxyT.liftT(() => f().Map(p => p.Proxy));
    
    /// <summary>
    /// Create an asynchronous proxy 
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> liftT<M, A>(ValueTask<EffectT<M, A>> f) 
        where M : Monad<M> =>
        ProxyT.liftT(f.Map(p => p.Proxy));
}
