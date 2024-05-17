namespace LanguageExt.Traits;

/// <summary>
/// A monoid on applicative functors
/// </summary>
/// <typeparam name="F">Applicative functor</typeparam>
public static partial class Alternative
{
    /// <summary>
    /// Identity
    /// </summary>
    /// <typeparam name="A"></typeparam>
    /// <returns></returns>
    public static K<F, A> empty<F, A>()
        where F : Alternative<F> =>
        F.Empty<A>();

    /// <summary>
    /// Associative binary operator
    /// </summary>
    public static K<F, A> combine<F, A>(K<F, A> ma, K<F, A> mb)
        where F : SemiAlternative<F> =>
        F.Combine(ma, mb);
    
    /// <summary>
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    public static K<F, A> oneOf<F, A>(params K<F, A>[] ms)
        where F : Alternative<F> =>
        oneOf(ms.AsEnumerableM().ToSeq());

    /// <summary>
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    public static K<F, A> oneOf<F, A>(Seq<K<F, A>> ms)
        where F : Alternative<F> =>
        ms.IsEmpty
            ? F.Empty<A>()
            : F.Combine(ms.Head.Value!, oneOf(ms.Tail));

    /// <summary>
    /// One or more...
    /// </summary>
    /// <remarks>
    /// Run the applicative functor repeatedly, collecting the results, until failure.
    ///
    /// Will always succeed if at least one item has been yielded.
    /// </remarks>
    /// <param name="v">Applicative functor</param>
    /// <returns>One or more values</returns>
    public static K<F, Seq<A>> some<F, A>(K<F, A> v)
        where F : Alternative<F> =>
        F.Some(v);

    /// <summary>
    /// Zero or more...
    /// </summary>
    /// <remarks>
    /// Run the applicative functor repeatedly, collecting the results, until failure.
    ///
    /// Will always succeed.
    /// </remarks>
    /// <param name="v">Applicative functor</param>
    /// <returns>Zero or more values</returns>
    public static K<F, Seq<A>> many<F, A>(K<F, A> v)
        where F : Alternative<F> =>
        F.Many(v);
    
    /// <summary>
    /// Conditional failure of `Alternative` computations. Defined by
    ///
    ///     guard(true)  = Applicative.pure
    ///     guard(false) = Alternative.empty
    ///
    /// </summary>
    /// <param name="flag"></param>
    /// <typeparam name="F"></typeparam>
    /// <returns></returns>
    public static K<F, Unit> guard<F>(bool flag)
        where F : Alternative<F> =>
        flag ? Applicative.pure<F, Unit>(default) : empty<F, Unit>();
}
