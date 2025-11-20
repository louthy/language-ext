using LanguageExt.Traits;

namespace LanguageExt;

public static partial class OptionExtensions
{
    extension<A>(K<Option, A> _)
    {
        /// <summary>
        /// Downcast operator
        /// </summary>
        public static Option<A> operator +(K<Option, A> ma) =>
            (Option<A>)ma;
    }
}
