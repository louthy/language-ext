using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude; 

namespace LanguageExt;

public static class ChronicleTExtensions
{
    /// <summary>
    /// Downcast operator
    /// </summary>
    public static ChronicleT<Ch, M, A> As<Ch, M, A>(this K<ChronicleT<Ch, M>, A> ma) 
        where Ch : Semigroup<Ch>
        where M : Monad<M> =>
        (ChronicleT<Ch, M, A>)ma;
    
    /// <summary>
    /// Run the chronicle to yield its inner monad
    /// </summary>
    public static K<M, These<Ch, A>> Run<Ch, M, A>(this K<ChronicleT<Ch, M>, A> ma) 
        where Ch : Semigroup<Ch>
        where M : Monad<M> =>
        ma.As().Run();

    /// <summary>
    /// Filtering based on predicate.  
    /// </summary>
    /// <remarks>>
    /// If the predicate returns false, then `ChronicleT.empty()` is yielded and therefore `Ch` must be a monoid.  
    /// </remarks>
    public static ChronicleT<Ch, M, A> Where<Ch, M, A>(this K<ChronicleT<Ch, M>, A> ma, Func<A, bool> pred)
        where Ch : Monoid<Ch>
        where M : Monad<M> =>
        ma.Filter(pred);

    /// <summary>
    /// Filtering based on predicate.  
    /// </summary>
    /// <remarks>>
    /// If the predicate returns false, then `ChronicleT.empty()` is yielded and therefore `Ch` must be a monoid.  
    /// </remarks>
    public static ChronicleT<Ch, M, A> Filter<Ch, M, A>(this K<ChronicleT<Ch, M>, A> ma, Func<A, bool> pred)
        where Ch : Monoid<Ch>
        where M : Monad<M> =>
        ma.As().Bind(x => pred(x) ? ChronicleT<Ch, M, A>.Dictate(x) : ChronicleT.empty<Ch, M, A>());    
}
