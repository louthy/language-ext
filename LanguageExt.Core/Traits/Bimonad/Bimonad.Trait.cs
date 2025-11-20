using System;
using static LanguageExt.Prelude;

namespace LanguageExt.Traits;

public interface Bimonad<M> : Bifunctor<M> 
    where M : Bimonad<M>
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Abstract members
    //
    
    public static abstract K<M, Y, A> BindFirst<X, Y, A>(K<M, X, A> ma, Func<X, K<M, Y, A>> f);
    
    public static abstract K<M, X, B> BindSecond<X, A, B>(K<M, X, A> ma, Func<A, K<M, X, B>> f);

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    //  Default implementations
    //
    
    public static virtual K<M, X, A> FlattenFirst<X, A>(K<M, K<M, X, A>, A> mma) =>
        M.BindFirst(mma, identity);    
    
    public static virtual K<M, X, A> FlattenSecond<X, A>(K<M, X, K<M, X, A>> mma) =>
        M.BindSecond(mma, identity);    
}
