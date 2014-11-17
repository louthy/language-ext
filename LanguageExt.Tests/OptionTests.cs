using NUnit.Framework;
using LanguageExt;

namespace LanguageExtTests
{
    [TestFixture]
    public class OptionTests
    {
        [Test] public void SomeGeneratorTestsObject()
        {
            var optional = Some(123);

            optional.Match( Some: i  => Assert.IsTrue(i == 123),
                            None: () => Assert.Fail("Shouldn't get here") );

            int c = optional.Match( Some: i  => i + 1, 
                                    None: () => 0 );

            Assert.IsTrue(c == 124);
        }

        [Test] public void SomeGeneratorTestsFunction()
        {
            var optional = Some(123);

            Match(optional, Some: i  => Assert.IsTrue(i == 123),
                            None: () => Assert.Fail("Shouldn't get here") );

            int c = Match(optional, Some: i => i + 1,
                                    None: () => 0 );

            Assert.IsTrue(c == 124);
        }

        [Test] public void NoneGeneratorTestsObject()
        {
            Option<int> optional = None;

            optional.Match(Some: i => Assert.Fail("Shouldn't get here"),
                           None: () => Assert.IsTrue(true));

            int c = optional.Match( Some: i => i + 1,
                                    None: () => 0);

            Assert.IsTrue(c == 0);
        }

        [Test] public void NoneGeneratorTestsFunction()
        {
            Option<int> optional = None;

            Match(optional, Some: i => Assert.Fail("Shouldn't get here"),
                            None: () => Assert.IsTrue(true));

            int c = Match(optional, Some: i => i + 1,
                                    None: () => 0);

            Assert.IsTrue(c == 0);
        }

        [Test] public void SomeLinqTest()
        {
            var two = Some(2);
            var four = Some(4);
            var six = Some(6);

           (from x in two
            from y in four
            from z in six
            select x + y + z)
           .Match(
             Some: v => Assert.IsTrue(v == 12),
             None: () => Assert.Fail("Shouldn't get here")
           );
        }

        [Test] public void NoneLinqTest()
        {
            var two = Some(2);
            var four = Some(4);
            var six = Some(6);

            (from x in two
             from y in four
             from _ in Option<int>.None
             from z in six
             select x + y + z)
            .Match(
              Some: v => Assert.Fail("Shouldn't get here"),
              None: () => Assert.IsTrue(true)
            );
        }

        [Test] public void NullIsNoneTest()
        {
            Assert.IsTrue(GetStringNone().IsNone);
        }

        [Test] public void NullIsNoneTest2()
        {
            Assert.IsTrue(GetStringNone2().IsNone);
        }


        private Option<string> GetStringNone()
        {
            string nullStr = null;
            return Some(nullStr);
        }

        private Option<string> GetStringNone2()
        {
            string nullStr = null;
            return nullStr;
        }

        private Option<int> GetValue(bool select) =>
            select
                ? Some(1000)
                : None;

        private Option<int> ImplicitConversion() => 1000;

        private Option<int> ImplicitNoneConversion() => None;
    }
}
