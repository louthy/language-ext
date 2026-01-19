using LanguageExt.Traits;

namespace LanguageExt;

public static partial class IterableIOExtensions
{
    extension<A>(K<IterableIO, A> _)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static IterableIO<A> operator +(K<IterableIO, A> ma) =>
            (IterableIO<A>)ma;
        
        public static IterableIO<A> operator >> (K<IterableIO, A> ma, Lower lower) =>
            (IterableIO<A>)ma;
    }
}
