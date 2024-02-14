namespace LanguageExt.Traits;

/// <summary>
/// A monoid on applicative functors
/// </summary>
/// <typeparam name="F">Applicative functor</typeparam>
public static class Alternative
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
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    public static K<F, A> oneOf<F, A>(K<F, A> mx, params K<F, A>[] mxs)
        where F : Alternative<F> =>
        oneOf(mx, mxs.ToSeq());

    /// <summary>
    /// Given a set of applicative functors, return the first one to succeed.
    /// </summary>
    /// <remarks>
    /// If none succeed, the last applicative functor will be returned.
    /// </remarks>
    public static K<F, A> oneOf<F, A>(K<F, A> mx, Seq<K<F, A>> mxs)
        where F : Alternative<F> =>
        mxs.IsEmpty
            ? mx
            : F.Or(mx, oneOf(mxs.Head, mxs.Tail));

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
    //
    //      guard(true)  = Applicative.pure
    //      guard(false) = Alternative.empty
    /// </summary>
    /// <param name="flag"></param>
    /// <typeparam name="F"></typeparam>
    /// <returns></returns>
    public static K<F, Unit> guard<F>(bool flag)
        where F : Alternative<F> =>
        flag ? Applicative.pure<F, Unit>(default) : empty<F, Unit>();
}
