using LanguageExt.ClassInstances;
using System.Numerics;

namespace LanguageExt
{
    public class BigIntegerRange : Range<BigIntegerRange, TBigInt, BigInteger>
    {
        BigIntegerRange(BigInteger min, BigInteger max) : base(min, max, 1) { }
        BigIntegerRange(BigInteger min, BigInteger max, BigInteger step) : base(min, max, step) { }
    }
}
