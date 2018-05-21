using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

using LanguageExt;
using LanguageExt.UnitsOfMeasure;
using static LanguageExt.Prelude;

namespace LanguageExtTests
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
        public void ForceTests()
        {
            Assert.Equal(1*N, 1.Newtons());

            var oneLbfInNewtons = 4.448222;
            Assert.Equal(1.Lbf().Newtons, oneLbfInNewtons);
        }

        [Fact]
        public void ForceArithmeticTests()
        {
            var oneNewton = 1*N;

            var oneSQM = 1.SqMetres();
            object onePascal = oneNewton / oneSQM;
            Assert.True(onePascal is Pressure p && p == 1.Pascal());

            var oneMS2 = 1.MetresPerSecond2();
            object oneKg = oneNewton / oneMS2;
            Assert.True(oneKg is Mass);
        }

        [Fact]
        public void MassTests()
        {
            var oneKg = 1.Kilograms();

            Assert.Equal(oneKg, 1000*grams);
            Assert.Equal(oneKg, 1000000*milligrams);
            Assert.Equal(oneKg, 1000000000*micrograms);
            Assert.Equal(oneKg, 0.001*tonnes);
            Assert.Equal(oneKg, 0.000001*kilotonnes);
            Assert.Equal(oneKg, 0.000000001*megatonnes);

            Assert.Equal(oneKg.Ounces, oneKg.Pounds * 16, 5);
        }

        [Fact]
        public void MassArithmeticTests()
        {
            var oneKg = 1.Kilograms();
            var oneMS2 = 1.MetresPerSecond2();
            object oneNewton = oneKg * oneMS2;
            Assert.True(oneNewton is Force f && f == 1.Newtons());
        }

        [Fact]
        public void PressureTests()
        {
            var onePa = 1.Pascal();

            Assert.Equal(onePa, 0.01*hPa);
            Assert.Equal(onePa, 1*newtonPerMetre2);
            Assert.Equal(onePa, 0.00001*bar);
            Assert.Equal(onePa, 0.01*millibars);
            Assert.Equal(onePa, 0.001*centibars);

            Assert.Equal(1*atm, 1013.25*mb);
            Assert.Equal((1*atm).Pascal, (14.696*psi).Pascal, 0);
        }

        [Fact]
        public void PressureArithmeticTests()
        {
            var onePa = 1.Pascal();
            var oneM2 = 1.SqMetres();
            object oneNewton = onePa * oneM2;
            Assert.True(oneNewton is Force f && f == 1.Newtons());
        }

        [Fact]
        public void TemperatureTests()
        {
            var oneHundredCelsiusInKelvin = 373.15.Kelvin();

            Assert.Equal(oneHundredCelsiusInKelvin.Kelvin, 100.Celsius().Kelvin);

            Assert.Equal(100.Celsius(), 212.Fahrenheit());

            Assert.True(373.15*K == 100*degC);

            Assert.True(100.Celsius() > 100.Kelvin());
        }
    }
}
