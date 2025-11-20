using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static class ChronicleTGuardExtensions
{
    
    /// <summary>
    /// Natural transformation to `ChronicleT`
    /// </summary>
    public static ChronicleT<Ch, M, Unit> ToChronicleT<Ch, M>(this Guard<Ch, Unit> guard)
        where M :  Monad<M> =>
        guard.Flag
            ? ChronicleT.dictate<Ch, M, Unit>(default)
            : ChronicleT.confess<Ch, M, Unit>(guard.OnFalse());
 
    /// <summary>
    /// Monadic binding support for `ChronicleT`
    /// </summary>
    public static ChronicleT<Ch, M, B> Bind<Ch, M, B>(
        this Guard<Ch, Unit> guard,
        Func<Unit, ChronicleT<Ch, M, B>> f) 
        where M : Monad<M> =>
        guard.Flag
            ? f(default).As()
            : ChronicleT.confess<Ch, M, B>(guard.OnFalse());
       
    /// <summary>
    /// Monadic binding support for `ChronicleT`
    /// </summary>
    public static ChronicleT<Ch, M, C> SelectMany<Ch, M, B, C>(
        this Guard<Ch, Unit> guard,
        Func<Unit, ChronicleT<Ch, M, B>> bind, 
        Func<Unit, B, C> project) 
        where M : Monad<M> =>
        guard.Flag
            ? bind(default).Map(b => project(default, b)).As()
            : ChronicleT.confess<Ch, M, C>(guard.OnFalse());    
}
