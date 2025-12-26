using LanguageExt.Traits;

namespace LanguageExt;

public static partial class SourceExtensions
{
    extension<A>(K<Source, A> _)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static Source<A> operator +(K<Source, A> ma) =>
            (Source<A>)ma;
        
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static Source<A> operator >> (K<Source, A> ma, Lower lower) =>
            +ma;
    }
}
