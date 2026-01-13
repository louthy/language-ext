using LanguageExt.Traits;

namespace LanguageExt;

public static partial class RangeExtensions
{
    extension<A>(K<Range, A> ma)
    {
        /// <summary>
        /// Cast the structure to the actual type
        /// </summary>
        public Range<A> As() =>
            (Range<A>)ma;
    }
}
