using LanguageExt.Traits;

namespace LanguageExt;

public static partial class IterableNEExtensions
{
    extension<A>(K<IterableNE, A> _)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static IterableNE<A> operator +(K<IterableNE, A> ma) =>
            (IterableNE<A>)ma;
        
        public static IterableNE<A> operator >> (K<IterableNE, A> ma, Lower lower) =>
            (IterableNE<A>)ma;
    }
}
