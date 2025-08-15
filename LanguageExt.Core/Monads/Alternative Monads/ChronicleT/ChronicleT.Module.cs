using LanguageExt.Traits;

namespace LanguageExt;

public static class ChronicleT
{
    /// <summary>
    /// Construct a new chronicle with a pure value
    /// </summary>
    /// <param name="value">Value to construct with</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> That<Ch, M, A>(A value) 
        where Ch : Semigroup<Ch>
        where M : Monad<M> =>
        new(M.Pure(These.That<Ch, A>(value)));
    
    /// <summary>
    /// Construct a new chronicle with a 'failure' value
    /// </summary>
    /// <param name="value">Value to construct with</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> This<Ch, M, A>(Ch value) 
        where Ch : Semigroup<Ch>
        where M : Monad<M> =>
        new(M.Pure(These.This<Ch, A>(value)));
    
    /// <summary>
    /// Construct a new chronicle with both a 'failure' and a success value.
    /// </summary>
    /// <param name="@this">Value to construct with</param>
    /// <param name="that">Value to construct with</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> Pair<Ch, M, A>(Ch @this, A that) 
        where Ch : Semigroup<Ch>
        where M : Monad<M> =>
        new(M.Pure(These.Pair(@this, that)));
    
    /// <summary>
    /// Lift a monad `M` into the monad-transformer
    /// </summary>
    /// <param name="ma">Monad to lift</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> lift<Ch, M, A>(K<M, A> ma) 
        where Ch : Semigroup<Ch>
        where M : Monad<M> =>
        new(ma.Map(These.That<Ch, A>));
    
    /// <summary>
    /// Lift an `IO` monad into the monad-transformer
    /// </summary>
    /// <param name="ma">Monad to lift</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> liftIO<Ch, M, A>(K<IO, A> ma)
        where Ch : Semigroup<Ch>
        where M : MonadIO<M> =>
        lift<Ch, M, A>(M.LiftIO(ma));
}
