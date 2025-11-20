using System;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

/// <summary>
/// `Effect` streaming effect monad-transformer
/// </summary>
public static class Effect
{
    /// <summary>
    /// Create an effect that simply returns a bound value without yielding anything
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Effect<RT, A> pure<RT, A>(A value) =>
        PipeT.pure<Unit, Void, Eff<RT>, A>(value);
    
    /// <summary>
    /// Create an effect that always fails
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Effect<RT, A> error<RT, A>(Error value) =>
        PipeT.error<Unit, Void, Eff<RT>, A>(value);
    
    /// <summary>
    /// Create an effect that yields nothing at all
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Effect<RT, A> empty<RT, A>() =>
        PipeT.empty<Unit, Void, Eff<RT>, A>();
    
    /// <summary>
    /// Create an effect that lazily returns a bound value without yielding anything
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Effect<RT, A> lift<RT, A>(Func<A> f) =>
        PipeT.lift<Unit, Void, Eff<RT>, A>(f);
    
    /// <summary>
    /// Create an effect that simply returns the bound value of the lifted monad without yielding anything
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Effect<RT, A> liftM<RT, A>(K<Eff<RT>, A> ma) =>
        PipeT.liftM<Unit, Void, Eff<RT>, A>(ma);
    
    /// <summary>
    /// Create an effect that simply returns the bound value of the lifted monad without yielding anything
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Effect<RT, A> liftIO<RT, A>(IO<A> ma) =>
        PipeT.liftIO<Unit, Void, Eff<RT>, A>(ma);
        
    /// <summary>
    /// Create a lazy proxy 
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Effect<RT, A> liftT<RT, A>(Func<Effect<RT, A>> f) =>
        PipeT.liftT(() => f().Proxy);
    
    /// <summary>
    /// Create an asynchronous lazy proxy 
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Effect<RT, A> liftT<RT, A>(Func<ValueTask<Effect<RT, A>>> f) =>
        PipeT.liftT(() => f().Map(p => p.Proxy));
    
    /// <summary>
    /// Create an asynchronous proxy 
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Effect<RT, A> liftT<RT, A>(ValueTask<Effect<RT, A>> f) =>
        PipeT.liftT(f.Map(p => p.Proxy));

    /// <summary>
    /// Continually repeat the provided operation
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Effect<RT, A> repeat<RT, A>(Effect<RT, A> ma) =>
        PipeT.repeat(ma.Proxy).ToEffect();
    
    /// <summary>
    /// Repeat the provided operation based on the schedule provided
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Effect<RT, A> repeat<RT, A>(Schedule schedule, Effect<RT, A> ma) =>
        PipeT.repeat(schedule, ma.Proxy).ToEffect();

    /// <summary>
    /// Continually lift and repeat the provided operation
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Effect<RT, A> repeatM<RT, A>(K<Eff<RT>, A> ma) =>
        PipeT.repeatM<Unit, Void, Eff<RT>, A>(ma).ToEffect();

    /// <summary>
    /// Repeat the provided operation based on the schedule provided
    /// </summary>
    /// <typeparam name="RT">Effect runtime type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static Effect<RT, A> repeatM<RT, A>(Schedule schedule, K<Eff<RT>, A> ma) =>
        PipeT.repeatM<Unit, Void, Eff<RT>, A>(schedule, ma).ToEffect();
}
