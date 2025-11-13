using LanguageExt.Traits;

namespace LanguageExt;

public static partial class IOExtensions
{
    extension<A>(K<IO, A> _)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static IO<A> operator -(K<IO, A> ma) =>
            (IO<A>)ma;
    }
    
    extension<A>(IO<A> _)
    {
        /// <summary>
        /// Upcast operator
        /// </summary>
        public static K<IO, A> operator +(IO<A> ma) =>
            ma;
    }
}
