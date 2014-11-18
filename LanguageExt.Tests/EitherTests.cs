using NUnit.Framework;
using LanguageExt;
using LanguageExt.Prelude;

namespace LanguageExtTests
{
    [TestFixture]
    public class EitherTests
    {
        [Test] public void RightGeneratorTestsObject()
        {
            var either = Right<int,string>(123);

            either.Match( Right: i => Assert.IsTrue(i == 123),
                          Left: _  => Assert.Fail("Shouldn't get here") );

            int c = either.Match( Right: i  => i + 1, 
                                  Left: _ => 0 );

            Assert.IsTrue(c == 124);
        }

        [Test] public void SomeGeneratorTestsFunction()
        {
            var either = Right<int, string>(123);

            match(either, Right: i => Assert.IsTrue(i == 123),
                          Left:  _ => Assert.Fail("Shouldn't get here") );

            int c = match(either, Right: i => i + 1,
                                  Left:  _ => 0 );

            Assert.IsTrue(c == 124);
        }

        [Test] public void LeftGeneratorTestsObject()
        {
            var either = ItsLeft;

            either.Match( Right: r => Assert.Fail("Shouldn't get here"),
                          Left:  l => Assert.IsTrue(l == "Left") );

            int c = either.Match( Right: r => r + 1, 
                                  Left:  l => 0 );

            Assert.IsTrue(c == 0);
        }

        [Test] public void LeftGeneratorTestsFunction()
        {
            var either = ItsLeft;

            match(either, Right: r => Assert.Fail("Shouldn't get here"),
                          Left:  l => Assert.IsTrue(l == "Left") );

            int c = match(either, Right: r => r + 1,
                                  Left:  l => 0 );

            Assert.IsTrue(c == 0);
        }

        [Test] public void SomeLinqTest()
        {
           (from x in Two
            from y in Four
            from z in Six
            select x + y + z)
           .Match(
             Right: r => Assert.IsTrue(r == 12),
             Left:  l => Assert.Fail("Shouldn't get here")
           );
        }

        [Test] public void LeftLinqTest()
        {
            (from x in Two
             from y in Four
             from _ in ItsLeft
             from z in Six
             select x + y + z)
            .Match(
              r => Assert.Fail("Shouldn't get here"),
              l => Assert.IsTrue(l == "Left")
            );
        }

        private Either<int, string> GetValue(bool select)
        {
            if (select)
            {
                return 1000;
            }
            else
            {
                return "Left";
            }
        }

        private Either<int, string> ImplicitConversion() => 1000;

        private Either<int, string> ItsLeft => "Left";
        private Either<int, string> Two => 2;
        private Either<int, string> Four => 4;
        private Either<int, string> Six => 6;
    }
}
