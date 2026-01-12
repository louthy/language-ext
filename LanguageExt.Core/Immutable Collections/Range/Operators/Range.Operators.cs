using LanguageExt.Traits;

namespace LanguageExt;

public static partial class RangeExtensions
{
    extension<A>(K<Range, A>)
    {
        public static Range<A> operator +(K<Range, A> fa) =>
            fa.As();
        
        public static Range<A> operator >>(K<Range, A> fa, Lower _) =>
            fa.As();
    }
}
