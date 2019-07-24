using LanguageExt.ClassInstances;
using System.Numerics;

namespace LanguageExt
{
    public class BigIntegerRange : Range<BigIntegerRange, TBigInt, bigint>
    {
        BigIntegerRange(bigint min, bigint max) : base(min, max, 1) { }
        BigIntegerRange(bigint min, bigint max, bigint step) : base(min, max, step) { }
    }
}
