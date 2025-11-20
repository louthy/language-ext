using System;
using static LanguageExt.Prelude;
using LanguageExt.Traits;

namespace LanguageExt;

public static class BimonadExtensions 
{
    public static K<M, Y, A> BindFirst<M, X, Y, A>(this K<M, X, A> ma, Func<X, K<M, Y, A>> f) 
        where M : Bimonad<M> =>
        M.BindFirst(ma, f);
    
    public static K<M, X, B> BindSecond<M, X, A, B>(this K<M, X, A> ma, Func<A, K<M, X, B>> f)
        where M : Bimonad<M> =>
        M.BindSecond(ma, f);
    
    public static K<M, X, A> FlattenFirst<M, X, A>(K<M, K<M, X, A>, A> mma) 
        where M : Bimonad<M> =>
        M.FlattenFirst(mma);    
    
    public static K<M, X, A> FlattenSecond<M, X, A>(K<M, X, K<M, X, A>> mma) 
        where M : Bimonad<M> =>
        M.FlattenSecond(mma);    
}
