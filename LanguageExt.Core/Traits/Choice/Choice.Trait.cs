using System;

namespace LanguageExt.Traits;

/// <summary>
/// Choose between two structures.  If the first succeeds, then return it; otherwise return the second.
/// </summary>
/// <typeparam name="F">Structure type</typeparam>
public interface Choice<F>
    where F : Choice<F>
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
    static abstract K<F, A> Choose<A>(K<F, A> fa, K<F, A> fb);
    
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
    static abstract K<F, A> Choose<A>(K<F, A> fa, Func<K<F, A>> fb);    
}
