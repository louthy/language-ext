using LanguageExt.ClassInstances;

namespace LanguageExt
{
    public class FloatRange : Range<FloatRange, TFloat, float>
    {
        FloatRange(float min, float max) : base(min, max, 1) { }
        FloatRange(float min, float max, float step) : base(min, max, step) { }
    }
}
