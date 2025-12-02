using System.Diagnostics.Contracts;

namespace LanguageExt.Traits;

public static partial class Applicative
{
    [Pure]
    public static K<F, A> pure<F, A>(A value) 
        where F : Applicative<F> =>
        F.Pure(value);

    [Pure]
    public static K<F, Unit> when<F>(bool flag, K<F, Unit> fx)
        where F : Applicative<F> =>
        flag ? fx : F.Pure<Unit>(default);

    [Pure]
    public static K<F, Unit> unless<F>(bool flag, K<F, Unit> fx)
        where F : Applicative<F> =>
        when(!flag, fx);
    
    /// <summary>
    /// `between(open, close, p) parses `open`, followed by `p` and `close`.
    /// </summary>
    /// <param name="open">Open computation</param>
    /// <param name="close">Close computation</param>
    /// <param name="p">Between computation</param>
    /// <typeparam name="A">Return value type</typeparam>
    /// <typeparam name="OPEN">OPEN value type</typeparam>
    /// <typeparam name="CLOSE">CLOSE value type</typeparam>
    /// <returns>The value returned by `p`</returns>
    [Pure]
    public static K<F, A> between<F, A, OPEN, CLOSE>(
        K<F, OPEN> open, 
        K<F, CLOSE> close, 
        K<F, A> p)
        where F : Applicative<F> =>
        F.Between(open, close, p);
    
    
    /// <summary>
    /// Construct a sequence of `count` repetitions of `fa`
    /// </summary>
    /// <param name="count">Number of repetitions</param>
    /// <param name="fa">Applicative computation to run</param>
    /// <typeparam name="A">Value type</typeparam>
    /// <returns>Applicative structure of `count` items</returns>
    [Pure]
    public static K<F, Seq<A>> replicate<F, A>(int count, K<F, A> fa) 
        where F : Applicative<F> =>
        F.Replicate(count, fa);
    
}
