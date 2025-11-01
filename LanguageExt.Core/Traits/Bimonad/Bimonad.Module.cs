using System;
using static LanguageExt.Prelude;
using LanguageExt.Traits;

namespace LanguageExt;

public static class Bimonad 
{
    public static K<M, Y, A> bindFirst<M, X, Y, A>(K<M, X, A> ma, Func<X, K<M, Y, A>> f) 
        where M : Bimonad<M> =>
        M.BindFirst(ma, f);
    
    
    public static K<M, X, B> bindSecond<M, X, A, B>(K<M, X, A> ma, Func<A, K<M, X, B>> f)
        where M : Bimonad<M> =>
        M.BindSecond(ma, f);
    
    public static K<M, X, A> flattenFirst<M, X, A>(K<M, K<M, X, A>, A> mma) 
        where M : Bimonad<M> =>
        M.FlattenFirst(mma);    
    
    public static K<M, X, A> flattenSecond<M, X, A>(K<M, X, K<M, X, A>> mma) 
        where M : Bimonad<M> =>
        M.FlattenSecond(mma);    
}
