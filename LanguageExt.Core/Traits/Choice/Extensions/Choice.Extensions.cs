using System;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class ChoiceExtensions
{
    /// <param name="fa">First structure to test</param>
    /// <typeparam name="F">Alternative structure type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    extension<F, A>(K<F, A> fa) 
        where F : Choice<F>
    {
        /// <summary>
        /// Where `F` defines some notion of failure or choice, this function picks the
        /// first argument that succeeds.  So, if `fa` succeeds, then `fa` is returned;
        /// if it fails, then `fb` is returned.
        /// </summary>
        /// <param name="fb">Second structure to return if the first one fails</param>
        /// <returns>First argument to succeed</returns>
        public K<F, A> Choose(K<F, A> fb) =>
            F.Choose(fa, fb);

        /// <summary>
        /// Where `F` defines some notion of failure or choice, this function picks the
        /// first argument that succeeds.  So, if `fa` succeeds, then `fa` is returned;
        /// if it fails, then `fb` is returned.
        /// </summary>
        /// <param name="fb">Second structure to return if the first one fails</param>
        /// <returns>First argument to succeed</returns>
        public K<F, A> Choose(Memo<F, A> fb) =>
            F.Choose(fa, fb);
    }

    /// <param name="v">Applicative functor</param>
    extension<F, A>(K<F, A> v) 
        where F : Choice<F>, Applicative<F>
    {
        /// <summary>
        /// One or more...
        /// </summary>
        /// <remarks>
        /// Run the applicative functor repeatedly, collecting the results, until failure.
        /// 
        /// Will always succeed if at least one item has been yielded.
        /// </remarks>
        /// <returns>One or more values</returns>
        public K<F, Seq<A>> Some() =>
            Choice.some(v);

        /// <summary>
        /// Zero or more...
        /// </summary>
        /// <remarks>
        /// Run the applicative functor repeatedly, collecting the results, until failure.
        /// 
        /// Will always succeed.
        /// </remarks>
        /// <returns>Zero or more values</returns>
        public K<F, Seq<A>> Many() =>
            Choice.many(v);
    }
}
