using LanguageExt.Traits;

namespace LanguageExt;

public static class ChronicleTExtensions
{
    /// <summary>
    /// Downcast operator
    /// </summary>
    public static ChronicleT<Ch, M, A> As<Ch, M, A>(this K<ChronicleT<Ch, M>, A> ma) 
        where Ch : Monoid<Ch>
        where M : Monad<M> =>
        (ChronicleT<Ch, M, A>)ma;
    
    /// <summary>
    /// Run the chronicle to yield its inner monad
    /// </summary>
    public static K<M, These<Ch, A>> Run<Ch, M, A>(this K<ChronicleT<Ch, M>, A> ma) 
        where Ch : Monoid<Ch>
        where M : Monad<M> =>
        ma.As().Run();
}
