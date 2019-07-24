using LanguageExt.ClassInstances;

namespace LanguageExt
{
    public class IntegerRange : Range<IntegerRange, TInt, int>
    {
        IntegerRange(int min, int max) : base(min, max, 1) { }
        IntegerRange(int min, int max, int step) : base(min, max, step) { }
    }
}
