using System;
using static LanguageExt.Prelude;

namespace LanguageExt.HKT;

/// <summary>
/// Monad trait
/// </summary>
/// <typeparam name="M">Self referring trait</typeparam>
public interface Monad<M> : Applicative<M> 
    where M : Monad<M>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Abstract members
    //
    
    public static abstract K<M, B> Bind<A, B>(K<M, A> ma, Func<A, K<M, B>> f);
    
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Default implementations
    //
    
    public static virtual K<M, A> Flatten<A>(K<M, K<M, A>> mma) =>
        M.Bind(mma, identity);
    
    public new static virtual K<M, B> Map<A, B>(Func<A, B> f, K<M, A> ma) =>
        M.Bind(ma, x => M.Pure(f(x)));
}
