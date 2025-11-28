using System.Diagnostics.Contracts;

namespace LanguageExt.Traits;

public static partial class Applicative
{
    [Pure]
    public static K<F, B> action<F, A ,B>(K<F, A> ma, Memo<F, B> mb) 
        where F : Applicative<F> =>
        F.Action(ma, mb);
    
    [Pure]
    public static K<F, B> action<F, A ,B>(Memo<F, A> ma, Memo<F, B> mb) 
        where F : Applicative<F> =>
        F.Action(ma, mb);    
    
    [Pure]
    public static K<F, B> action<F, A ,B>(Memo<F, A> ma, K<F, B> mb) 
        where F : Applicative<F> =>
        F.Action(ma, mb);    
}
