using LanguageExt.Traits;

namespace LanguageExt;

public static partial class IOExtensions
{
    extension<A>(K<IO, A> _)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static IO<A> operator +(K<IO, A> ma) =>
            (IO<A>)ma;
        
        public static IO<A> operator >> (K<IO, A> ma, Lower lower) =>
            (IO<A>)ma;
    }
}
