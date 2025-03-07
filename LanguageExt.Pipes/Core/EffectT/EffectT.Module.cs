using System;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

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
        PipeT.pure<Unit, Void, M, A>(value);
    
    /// <summary>
    /// Create an effect that always fails
    /// </summary>
    /// <typeparam name="E">Failure type</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> fail<E, M, A>(E value) 
        where M : Monad<M>, Fallible<E, M> =>
        PipeT.fail<Unit, Void, E, M, A>(value);
    
    /// <summary>
    /// Create an effect that always fails
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> error<M, A>(Error value) 
        where M : Monad<M>, Fallible<M> =>
        PipeT.error<Unit, Void, M, A>(value);
    
    /// <summary>
    /// Create an effect that yields nothing at all
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> empty<M, A>() 
        where M : Monad<M>, MonoidK<M> =>
        PipeT.empty<Unit, Void, M, A>();
    
    /// <summary>
    /// Create an effect that lazily returns a bound value without yielding anything
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> lift<M, A>(Func<A> f) 
        where M : Monad<M> =>
        PipeT.lift<Unit, Void, M, A>(f);
    
    /// <summary>
    /// Create an effect that simply returns the bound value of the lifted monad without yielding anything
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> liftM<M, A>(K<M, A> ma) 
        where M : Monad<M> =>
        PipeT.liftM<Unit, Void, M, A>(ma);
    
    /// <summary>
    /// Create an effect that simply returns the bound value of the lifted monad without yielding anything
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> liftIO<M, A>(IO<A> ma) 
        where M : Monad<M> =>
        PipeT.liftIO<Unit, Void, M, A>(ma);
        
    /// <summary>
    /// Create a lazy proxy 
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> liftT<M, A>(Func<EffectT<M, A>> f) 
        where M : Monad<M> =>
        PipeT.liftT(() => f().Proxy);
    
    /// <summary>
    /// Create an asynchronous lazy proxy 
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> liftT<M, A>(Func<ValueTask<EffectT<M, A>>> f) 
        where M : Monad<M> =>
        PipeT.liftT(() => f().Map(p => p.Proxy));
    
    /// <summary>
    /// Create an asynchronous proxy 
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> liftT<M, A>(ValueTask<EffectT<M, A>> f) 
        where M : Monad<M> =>
        PipeT.liftT(f.Map(p => p.Proxy));

    /// <summary>
    /// Continually repeat the provided operation
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> repeat<M, A>(EffectT<M, A> ma)
        where M : Monad<M> =>
        PipeT.repeat(ma.Proxy).ToEffect();
    
    /// <summary>
    /// Repeat the provided operation based on the schedule provided
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> repeat<M, A>(Schedule schedule, EffectT<M, A> ma)
        where M : Monad<M> =>
        PipeT.repeat(schedule, ma.Proxy).ToEffect();

    /// <summary>
    /// Continually lift and repeat the provided operation
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> repeatM<M, A>(K<M, A> ma)
        where M : Monad<M> =>
        PipeT.repeatM<Unit, Void, M, A>(ma).ToEffect();

    /// <summary>
    /// Repeat the provided operation based on the schedule provided
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> repeatM<M, A>(Schedule schedule, K<M, A> ma)
        where M : Monad<M> =>
        PipeT.repeatM<Unit, Void, M, A>(schedule, ma).ToEffect();
}
