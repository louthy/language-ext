using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;
using static LanguageExt.Prelude; 

namespace LanguageExt;

public static partial class ChronicleTExtensions
{
    /// <summary>
    /// Downcast operator
    /// </summary>
    [Pure]
    public static ChronicleT<Ch, M, A> As<Ch, M, A>(this K<ChronicleT<Ch, M>, A> ma) 
        where M : Monad<M> =>
        (ChronicleT<Ch, M, A>)ma;
    
    /// <summary>
    /// Downcast operator
    /// </summary>
    [Pure]
    public static ChronicleT<Ch, M, A> As2<Ch, M, A>(this K<ChronicleT<M>, Ch, A> ma) 
        where M : Monad<M> =>
        (ChronicleT<Ch, M, A>)ma;
    
    /// <summary>
    /// Run the chronicle to yield its inner monad
    /// </summary>
    [Pure]
    public static K<M, These<Ch, A>> Run<Ch, M, A>(this K<ChronicleT<Ch, M>, A> ma)
        where Ch : Semigroup<Ch>
        where M : Monad<M> =>
        ma.As().Run(Ch.Instance);
    
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static ChronicleT<Ch, M, A> Flatten<Ch, M, A>(this ChronicleT<Ch, M, ChronicleT<Ch, M, A>> mma)
        where M : Monad<M> =>
        mma.Bind(identity);

    /// <summary>
    /// Functor map operation
    /// </summary>
    /// <param name="f">Dictation mapping function</param>
    /// <typeparam name="B">Dictation type to map to</typeparam>
    /// <returns></returns>
    [Pure]
    public static ChronicleT<Ch, M, B> BiMap<Ch, M, A, B>(
        this ChronicleT<Ch, M, A> ma, 
        Func<A, B> f)
        where Ch : Semigroup<Ch> 
        where M : Monad<M> =>
        ma.As().Map(f);

    /// <summary>
    /// Bifunctor map operation
    /// </summary>
    /// <param name="This">Chronicle mapping function</param>
    /// <param name="That">Dictation mapping function</param>
    /// <typeparam name="Ch1">Chronicle type to map to</typeparam>
    /// <typeparam name="B">Dictation type to map to</typeparam>
    /// <returns></returns>
    [Pure]
    public static ChronicleT<Ch1, M, B> BiMap<Ch, Ch1, M, A, B>(
        this ChronicleT<Ch, M, A> ma, 
        Func<Ch, Ch1> This, 
        Func<A, B> That)
        where Ch : Semigroup<Ch> 
        where Ch1 : Semigroup<Ch1> 
        where M : Monad<M> =>
        Bifunctor.bimap(This, That, ma).As2();

    /// <summary>
    /// Filtering based on predicate.  
    /// </summary>
    /// <remarks>>
    /// If the predicate returns false, then `ChronicleT.empty()` is yielded and therefore `Ch` must be a monoid.  
    /// </remarks>
    [Pure]
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
    [Pure]
    public static ChronicleT<Ch, M, A> Filter<Ch, M, A>(this K<ChronicleT<Ch, M>, A> ma, Func<A, bool> pred)
        where Ch : Monoid<Ch>
        where M : Monad<M> =>
        ma.As().Bind(x => pred(x) ? ChronicleT.dictate<Ch, M, A>(x) : ChronicleT.empty<Ch, M, A>());

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="A">Source bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    [Pure]
    public static ChronicleT<Ch, M, C> SelectMany<Ch, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, K<ChronicleT<Ch, M>, B>> bind, 
        Func<A, B, C> project)
        where Ch : Semigroup<Ch>
        where M : Monad<M> =>
        ChronicleT.lift<Ch, M, A>(ma).SelectMany(bind, project);

    /// <summary>
    /// Monad bind operation
    /// </summary>
    /// <param name="bind">Monadic bind function</param>
    /// <param name="project">Projection function</param>
    /// <typeparam name="A">Source bound value type</typeparam>
    /// <typeparam name="B">Intermediate bound value type</typeparam>
    /// <typeparam name="C">Target bound value type</typeparam>
    /// <returns>`EitherT`</returns>
    [Pure]
    public static ChronicleT<Ch, M, C> SelectMany<Ch, M, A, B, C>(
        this K<M, A> ma, 
        Func<A, ChronicleT<Ch, M, B>> bind, 
        Func<A, B, C> project)
        where Ch : Semigroup<Ch>
        where M : Monad<M> =>
        ChronicleT.lift<Ch, M, A>(ma).SelectMany(bind, project);
}
