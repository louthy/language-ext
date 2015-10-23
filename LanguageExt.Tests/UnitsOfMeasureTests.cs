using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using LanguageExt;
using LanguageExt.UnitsOfMeasure;

namespace LanguageExtTests
{
    public class UnitsOfMeasureTests
    {
        [Fact]
        public void LengthEqualityTest()
        {
            Assert.True(100.Centimetres() == 1.Metres());
            Assert.True(1.Kilometres() == 1000.Metres());
        }

        [Fact]
        public void LengthEqualityTest2()
        {
            Length length = 1000.Millimetres();

            Assert.True(length.Metres == 1.0);
            Assert.True(length.Millimetres == 1000.0);
        }

        [Fact]
        public void LengthEqualityTest3()
        {
            Assert.True(1.Yards() == 3.Feet());
        }

        [Fact]
        public void LengthEqualityTest4()
        {
            Assert.True(12.Inches() == 1.Feet());
        }

        [Fact]
        public void LengthArithmetic1()
        {
            Length length = 1000.Millimetres() + 1.Metres();
            Assert.True(length == 2.Metres());
        }

        [Fact]
        public void LengthArithmetic2()
        {
            Length length = 1.Centimetres() + 10.Millimetres();
            Assert.True(length == 2.Centimetres());
        }

        [Fact]
        public void TimeEqualityTest()
        {
            Assert.True(60.Seconds() == 1.Minutes());
            Assert.True(60.Minutes() == 1.Hours());
        }
    }
}
