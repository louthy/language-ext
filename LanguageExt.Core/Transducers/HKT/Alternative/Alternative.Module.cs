namespace LanguageExt.HKT;

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
    public static Alternative<F, A> empty<F, A>()
        where F : Alternative<F> =>
        F.Empty<A>();

    /// <summary>
    /// Associative binary operator
    /// </summary>
    public static Alternative<F, A> either<F, A>(Alternative<F, A> ma, Alternative<F, A> mb)
        where F : Alternative<F> =>
        ma | mb;

    /// <summary>
    /// Associative binary operator
    /// </summary>
    public static Alternative<F, A> either<F, A>(Alternative<F, A> ma, Applicative<F, A> mb)
        where F : Alternative<F> =>
        ma | mb;

    /// <summary>
    /// Associative binary operator
    /// </summary>
    public static Alternative<F, A> either<F, A>(Applicative<F, A> ma, Alternative<F, A> mb) 
        where F : Alternative<F> =>
        ma | mb;

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
    public static Alternative<F, Seq<A>> some<F, A>(Alternative<F, A> v)
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
    public static Alternative<F, Seq<A>> many<F, A>(Alternative<F, A> v)
        where F : Alternative<F> =>
        F.Many(v);
}
