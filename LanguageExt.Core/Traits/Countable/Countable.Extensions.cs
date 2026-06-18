namespace LanguageExt.Traits;

public static class CountableExtensions
{
    extension<F, A>(K<F, A> fa)
        where F : Countable<F>
    {
        /// <summary>
        /// Return the number of elements in the structure.
        /// </summary>
        /// <remarks>
        /// Countable structures are structures that already contain a running count of the number of elements in the
        /// structure, and therefore accessing the `Count` property is a constant time operation.
        /// </remarks>
        /// <param name="fa">Countable structure</param>
        /// <typeparam name="F">Countable structure trait implementation type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns></returns>
        public long Count =>
            F.Count(fa);
    }
}
