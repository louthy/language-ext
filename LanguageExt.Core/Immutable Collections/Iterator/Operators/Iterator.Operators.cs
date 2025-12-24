using LanguageExt.Traits;

namespace LanguageExt;

public static partial class IteratorExtensions
{
    extension<A>(K<Iterator, A> _)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static Iterator<A> operator +(K<Iterator, A> ma) =>
            (Iterator<A>)ma;
        
        public static Iterator<A> operator >> (K<Iterator, A> ma, Lower lower) =>
            (Iterator<A>)ma;
    }
}
