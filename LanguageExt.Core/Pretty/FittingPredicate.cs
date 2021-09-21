using System;

namespace LanguageExt.Pretty
{
    /// <summary>
    /// FittingPredicate  
    /// </summary>
    /// <remarks>
    ///
    ///     FP: PageWidth, Nesting Level, Width for fist line (None is unbounded(
    /// 
    /// </remarks>
    public record FittingPredicate<A>(Func<PageWidth, int, Option<int>, DocStream<A>, bool> FP);
}
