using LanguageExt.Traits;

namespace LanguageExt;

public static partial class TryExtensions
{
    extension<A>(K<Try, A> _)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static Try<A> operator +(K<Try, A> ma) =>
            (Try<A>)ma;
    }
}
