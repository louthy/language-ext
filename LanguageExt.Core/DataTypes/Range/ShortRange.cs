using LanguageExt.ClassInstances;

namespace LanguageExt
{
    public class ShortRange : Range<ShortRange, TShort, short>
    {
        ShortRange(short min, short max) : base(min, max, 1) { }
        ShortRange(short min, short max, short step) : base(min, max, step) { }
    }
}
