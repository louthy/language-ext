using System;
using Xunit;
using System.Linq;
using LanguageExt.TypeClasses;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;
using LanguageExt;

namespace LanguageExt.Tests
{
    public class TypeClassBool
    {
        [Fact]
        public void BigIntXor()
        {
            var xorValue = TBigInt.Inst.XOr(new bigint(0b0000_1100), new bigint(0b0000_0111));

            Assert.True(new bigint(0b0000_1011) == xorValue);
            Assert.True(new bigint(0b0000_1011).Equals(xorValue));
            Assert.True(EqBigInt.Inst.Equals(new bigint(0b0000_1011), xorValue));
            // doesn't work: Assert.Equal(new bigint(0x03), xorBigint);
        }

        [Fact]
        public void BigIntNot()
        {
            var notValue = TBigInt.Inst.Not(new bigint(12));
            Assert.True(new bigint(-13) == notValue);
        }

        [Fact]
        public void BigIntBiConditional()
        {
            var biConditionValue = TBigInt.Inst.BiCondition(new bigint(0b0000_1100), new bigint(0b0000_0111));

            Assert.True(new bigint(0b1111_0100) == TBigInt.Inst.And(new bigint(0b1111_1111), biConditionValue)); // least significant 8 bits
            Assert.True(new bigint(~0 ^ 0b0000_1011) == biConditionValue); // check all bits (signed 32bit integer)
        }

        [Fact]
        public void ShortXor()
        {
            var xorValue = TShort.Inst.XOr(0b0000_1100, 0b0000_0111);

            Assert.True(0b0000_1011 == xorValue);
        }

        [Fact]
        public void ShortNot()
        {
            var notValue = TShort.Inst.Not(12);
            Assert.True(-13 == notValue);
        }

        [Fact]
        public void ShortBiConditional()
        {
            var biConditionValue = TShort.Inst.BiCondition(0b0000_1100, 0b0000_0111);

            Assert.True(0b1111_0100 == TShort.Inst.And(0b1111_1111, biConditionValue)); // least significant 8 bits
            Console.WriteLine((~0 ^ 0b0000_1011));
            Console.WriteLine(biConditionValue);
            Assert.True((~0 ^ 0b0000_1011) == biConditionValue); 
        }

        [Fact]
        public void IntXor()
        {
            var xorValue = TInt.Inst.XOr(0b0000_1100, 0b0000_0111);

            Assert.True(0b0000_1011 == xorValue);
        }

        [Fact]
        public void IntNot()
        {
            var notValue = TInt.Inst.Not(12);
            Assert.True(-13 == notValue);
        }

        [Fact]
        public void IntBiConditional()
        {
            var biConditionValue = TInt.Inst.BiCondition(0b0000_1100, 0b0000_0111);

            Assert.True(0b1111_0100 == TInt.Inst.And(0b1111_1111, biConditionValue)); // least significant 8 bits
            Assert.True((~0 ^ 0b0000_1011) == biConditionValue); 
        }

        [Fact]
        public void LongXor()
        {
            var xorValue = TLong.Inst.XOr(0b0000_1100, 0b0000_0111);

            Assert.True(0b0000_1011 == xorValue);
        }

        [Fact]
        public void LongNot()
        {
            var notValue = TLong.Inst.Not(12);
            Assert.True(-13 == notValue);
        }

        [Fact]
        public void LongBiConditional()
        {
            var biConditionValue = TLong.Inst.BiCondition(0b0000_1100, 0b0000_0111);

            Assert.True(0b1111_0100 == TLong.Inst.And(0b1111_1111, biConditionValue)); // least significant 8 bits
            Assert.True((~0L ^ 0b0000_1011L) == biConditionValue); 
        }
    }
}
