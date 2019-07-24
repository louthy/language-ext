using LanguageExt.ClassInstances;

namespace LanguageExt
{
    public class LongRange : Range<LongRange, TLong, long>
    {
        LongRange(int min, int max) : base(min, max, 1) { }
        LongRange(int min, int max, int step) : base(min, max, step) { }
    }
}
