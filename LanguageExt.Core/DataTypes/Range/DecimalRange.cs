using LanguageExt.ClassInstances;

namespace LanguageExt
{
    public class DecimalRange : Range<DecimalRange, TDecimal, decimal>
    {
        DecimalRange(decimal min, decimal max) : base(min, max, 1) { }
        DecimalRange(decimal min, decimal max, decimal step) : base(min, max, step) { }
    }
}
