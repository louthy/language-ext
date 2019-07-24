using LanguageExt.ClassInstances;

namespace LanguageExt
{
    public class DoubleRange : Range<DoubleRange, TDouble, double>
    {
        DoubleRange(double min, double max) : base(min, max, 1) { }
        DoubleRange(double min, double max, double step) : base(min, max, step) { }
    }
}
