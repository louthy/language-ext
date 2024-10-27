namespace LanguageExt.Traits;

public interface Alternative<F> : Applicative<F>, MonoidK<F>
    where F : Alternative<F>
{
    /// <summary>
    /// Where `F` defines some notion of failure or choice, this function picks the
    /// first argument that succeeds.  So, if `fa` succeeds, then `fa` is returned;
    /// if it fails, then `fb` is returned.
    /// </summary>
    /// <param name="fa">First structure to test</param>
    /// <param name="fb">Second structure to return if the first one fails</param>
    /// <typeparam name="F">Alternative structure type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns>First argument to succeed</returns>
    static abstract K<F, A> Choice<A>(K<F, A> fa, K<F, A> fb);
    
    static K<F, A> SemigroupK<F>.Combine<A>(K<F, A> fa, K<F, A> fb) => 
        F.Choice(fa, fb);
}
