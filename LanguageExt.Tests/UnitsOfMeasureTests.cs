using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using LanguageExt;
using LanguageExt.UnitsOfMeasure;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class UnitsOfMeasureTests
    {
        [Fact]
        public void PreludeLengthEqualityTest()
        {
            Assert.True(100*cm == 1*m);
            Assert.True(1*km == 1000*m);
        }

        [Fact]
        public void PreludeLengthEqualityTest3()
        {
            Assert.True(1*yard == 3*feet);
        }

        [Fact]
        public void PreludeLengthEqualityTest4()
        {
            Assert.True(12*inches == 1*feet);
        }

        [Fact]
        public void PreludeLengthCompareTest1()
        {
            Assert.True(1*mile > 1*km);
        }

        [Fact]
        public void PreludeLengthScalarTest2()
        {
            Assert.True(1*km / 500 == 2*metres);
        }

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
        public void LengthCompareTest1()
        {
            Assert.True(1.Miles() > 1.Kilometres());
        }

        [Fact]
        public void LengthScalarTest1()
        {
            Assert.True(1.Miles() * 10 == 10.Miles());
        }

        [Fact]
        public void LengthScalarTest2()
        {
            Assert.True(1.Kilometres() / 500 == 2.Metres());
        }

        [Fact]
        public void OperatorTests()
        {
            Length len = 1*km;

            double val = len / (1*m);   // Divide by 1 metre to get a dimensionless value

            Assert.True(val == 1000.0);

            val = len / (1000*m);

            Assert.True(val == 1.0);
        }

        [Fact]
        public void LengthCompareTest2()
        {
            Assert.True(100*mm < 2*m);
        }

        [Fact]
        public void LengthArithmetic1()
        {
            Length length = 1000*mm + 1*m;
            Assert.True(length == 2*m);
        }

        [Fact]
        public void LengthArithmetic2()
        {
            Length length = 1*cm + 10*mm;
            Assert.True(length == 2*cm);
        }

        [Fact]
        public void TimeEqualityTest()
        {
            Assert.True(60*sec == 1*min);
            Assert.True(60*mins == 1*hr);
        }

        [Fact]
        public void AreaTest1()
        {
            var a = 1000*cm * 8*m;
            var b = 80*m2;

            Assert.True(a == b);
        }

        [Fact]
        public void SpeedTest1()
        {
            Velocity v = 100*m/s;

            Length l   = v * 2*sec;

            double r   = l / (1*m);

            Assert.True(l == 200 * m);
            Assert.True(r == 200.0);
        }

        [Fact]
        public void SpeedTest2()
        {
            Velocity v = 100*mph;

            Time t     = 50*miles / v;

            Length l   = v * (4*hours);

            Assert.True(t == 30*mins);
            Assert.True(l == 400*miles);
        }

        [Fact]
        public void AccelTest1()
        {
            Accel g = 9.8*m/s/s;
            Accel g2 = 9.8*ms2;

            Velocity vel = g * 5*sec;
            Length len   = vel * 5*sec;

            Assert.True(vel.MetresPerSecond == 49.0);
            Assert.True(len.Metres == 245.0);
        }

        [Fact]
        public void AccelObjectEquals_Both0_True() =>
            AssertTypeObjectEquals<Accel>(0 * m / s / s);

        [Fact]
        public void AreaObjectEquals_Both0_True() =>
            AssertTypeObjectEquals<Area>(0 * m * m);

        [Fact]
        public void LengthObjectEquals_Both0_True() =>
            AssertTypeObjectEquals<Length>(0 * m);

        [Fact]
        public void TimeObjectEquals_Both0_True() =>
            AssertTypeObjectEquals<Time>(0 * s);

        [Fact]
        public void TimeSqObjectEquals_Both0_True() =>
            AssertTypeObjectEquals<TimeSq>(0 * s * s);

        [Fact]
        public void VelocityObjectEquals_Both0_True() =>
            AssertTypeObjectEquals<Velocity>(0 * m / s);

        [Fact]
        public void VelocitySqObjectEquals_Both0_True() =>
            AssertTypeObjectEquals<VelocitySq>((0 * m / s) * (0 * m / s));

        private void AssertTypeObjectEquals<T>(T t)
        {
            object o1 = t;
            object o2 = t;
            Assert.True(o1.Equals(o2));
        }

        [Fact]
        public void CelsiusToStringTest()
        {
            var x = 10 * degC;
            Assert.True(x.ToString() == "10 °C");
        }

        [Fact]
        public void CelsiusAddTest()
        {
            var x = 10 * degC;
            var y = 20 * degC;
            var z = 30 * degC;

            Assert.True(x + y == z);
        }

        [Fact]
        public void CelsiusSubTest()
        {
            var x = 10 * degC;
            var y = 20 * degC;
            var z = 30 * degC;

            Assert.True(z - y == x);
        }

        [Fact]
        public void CelsiusScalar1Test()
        {
            var x = 10 * degC;
            var y = 20 * degC;

            Assert.True(x * 2.0 == y);
        }

        [Fact]
        public void CelsiusScalar2Test()
        {
            var x = 10 * degC;
            var y = 20 * degC;

            Assert.True(y / 2.0 == x);
        }

        [Fact]
        public void FahrenheitToStringTest()
        {
            var x = 10 * degF;
            Assert.True(x.ToString() == "10 °F");
        }

        [Fact]
        public void FahrenheitAddTest()
        {
            var x = 10 * degF;
            var y = 20 * degF;
            var z = 30 * degF;

            Assert.True(x + y == z);
        }

        [Fact]
        public void FahrenheitSubTest()
        {
            var x = 10 * degF;
            var y = 20 * degF;
            var z = 30 * degF;

            Assert.True(z - y == x);
        }

        [Fact]
        public void FahrenheitScalar1Test()
        {
            var x = 10 * degF;
            var y = 20 * degF;

            Assert.True(x * 2.0 == y);
        }

        [Fact]
        public void FahrenheitScalar2Test()
        {
            var x = 10 * degF;
            var y = 20 * degF;

            Assert.True(y / 2.0 == x);
        }

        [Fact]
        public void KelvinToStringTest()
        {
            var x = 10 * K;
            Assert.True(x.ToString() == "10 K");
        }

        [Fact]
        public void KelvinAddTest()
        {
            var x = 10 * K;
            var y = 20 * K;
            var z = 30 * K;

            Assert.True(x + y == z);
        }

        [Fact]
        public void KelvinSubTest()
        {
            var x = 10 * K;
            var y = 20 * K;
            var z = 30 * K;

            Assert.True(z - y == x);
        }

        [Fact]
        public void KelvinScalar1Test()
        {
            var x = 10 * K;
            var y = 20 * K;

            Assert.True(x * 2.0 == y);
        }

        [Fact]
        public void KelvinScalar2Test()
        {
            var x = 10 * K;
            var y = 20 * K;

            Assert.True(y / 2.0 == x);
        }

        [Fact]
        public void KelvinAddCelsiusTest()
        {
            var x = 10 * K;
            var y = 20 * degC;
            var z = (10 * K) + (20 * degC).Kelvin;

            Assert.True(x + y == z);
        }

        [Fact]
        public void KelvinAddFahrenheitTest()
        {
            var x = 10 * K;
            var y = 20 * degF;
            var z = (10 * K) + (20 * degF).Kelvin;

            Assert.True(x + y == z);
        }

        [Fact]
        public void CtorThrowsBelowAbsZeroTest()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => -300 * degC);
        }

        [Fact]
        public void RetrieveKValueTest()
        {
            var x = 100.0 * degC;
            Assert.True(x.KValue == 373.15);
        }

        private double delta = 0.00001;

        [Fact]
        public void MassEqualityTests()
        {
            Assert.True(1000.Grams() == 1.Kilograms());
            Assert.True(1000.Kilograms() == 1.Tonnes());
            Assert.True(Math.Abs((16.Ounces() - 1.Pounds()).Pounds) < delta);
            Assert.True(Math.Abs((14.Pounds() - 1.Stones()).Pounds) < delta);
            Assert.True(Math.Abs((2240.Pounds() - 1.ImperialTons()).Pounds) < delta);
            Assert.True(Math.Abs((2000.Pounds() - 1.ShortTon()).Pounds) < delta);
        }

        [Fact]
        public void PreludeMassEqualityTest()
        {
            // Metric units
            Assert.True(1000 * g == 1 * kg, "g -> kg");
            Assert.True(1000 * kg == 1 * tonne, "kg -> tonne");
            // Imperial units. We use Math.Abs to check these, as they will differ at some point down the decimal expansion due to the fact that we store the value internally in Kg
            Mass sixteenOunces = 16 * ounce;
            Mass onePound = 1 * lb;
            Assert.True(Math.Abs((sixteenOunces - onePound).Pounds) < delta, "ounce -> pound");
            Mass fourteenPounds = 14 * lb;
            Mass oneStone = 1 * stone;
            Assert.True(Math.Abs((fourteenPounds - oneStone).Pounds) < delta, "lb -> stone");
            Mass oneHundredAndSixtyStones = 160 * stone;
            Mass oneTonUk = 1 * ton;
            Assert.True(Math.Abs((oneHundredAndSixtyStones - oneTonUk).Pounds) < delta, "stone -> tonUK");
            Mass oneTonUs = 1 * shortTon;
            Mass oneTonUsinStones = 142.857142858 * stone;
            Assert.True(Math.Abs((oneTonUsinStones - oneTonUs).Pounds) < delta, "stone -> tonUS");
            // Metric against Imperial
            Mass oneKilo = 1 * kg;
            Mass oneKiloInPounds = 2.2046226219 * lb;
            Assert.True(Math.Abs((oneKilo - oneKiloInPounds).Pounds) < delta, "kg -> pounds");
        }

        [Fact]
        public void PreludeMassCompareTest1()
        {
            Assert.True(1 * tonne > 1 * kg);
        }

        [Fact]
        public void PreludeMassScalarTest2()
        {
            Assert.True(1 * kg / 500 == 2 * g);
            Assert.True(1 * kilogram / 500 == 2 * gram);
        }
    }
}
