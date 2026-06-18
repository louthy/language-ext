using LanguageExt.Traits;

namespace LanguageExt;

public static partial class IteratorIOExtensions
{
    extension<A>(K<IteratorIO, A> _)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static IteratorIO<A> operator +(K<IteratorIO, A> ma) =>
            (IteratorIO<A>)ma;
        
        public static IteratorIO<A> operator >> (K<IteratorIO, A> ma, Lower lower) =>
            (IteratorIO<A>)ma;
        
    }
}
