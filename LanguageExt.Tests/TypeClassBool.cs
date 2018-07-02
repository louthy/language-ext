using Xunit;
using System.Linq;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using LanguageExt;

namespace LanguageExtTests
{
    public class TypeClassBool
    {
        [Fact]
        public void BigIntXor()
        {
            var bigint1 = new bigint(0b0000_1100);
            var bigint2 = new bigint(0b0000_0111);
            var xorBigint = TBigInt.Inst.XOr(bigint1, bigint2);

            Assert.True(new bigint(0b0000_1011) == xorBigint);
            Assert.True(new bigint(0b0000_1011).Equals(xorBigint));
            Assert.True(EqBigInt.Inst.Equals(new bigint(0b0000_1011), xorBigint));
            // doesn't work: Assert.Equal(new bigint(0x03), xorBigint);
        }

        [Fact]
        public void BigIntNot()
        {
            var bigint1 = new bigint(12);
            
            var notBigInt1 = TBigInt.Inst.Not(bigint1);
            Assert.True(new bigint(-13) == notBigInt1);
        }


        [Fact]
        public void BigIntBiConditional()
        {
            var bigint1 = new bigint(0b0000_1100);
            var bigint2 = new bigint(0b0000_0111);

            var biConditionBigint = TBigInt.Inst.BiCondition(bigint1, bigint2);
            System.Console.WriteLine(biConditionBigint);
            Assert.True(new bigint(0b1111_0100) == TBigInt.Inst.And(new bigint(0b1111_1111), biConditionBigint)); // least significant 8 bits
            Assert.True(new bigint(~0 ^ 0b0000_1011) == biConditionBigint); // check all bits (signed 32bit integer)
        }
    }
}
