using NUnit.Framework;
using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExtTests
{
    [TestFixture]
    public class ExceptionMatching
    {
        [Test]
        public void ExTest1()
        {
            string x = match( Number<SystemException>(10),
                              Succ: v  => "Worked",
                              Fail: ex => ex.Match<string>()
                                            .With<SystemException>(e => "It's a system exception")
                                            .With<ArgumentNullException>(e => "Arg null")
                                            .Otherwise("Not handled") );

            Assert.IsTrue(x == "Worked");
        }

        [Test]
        public void ExTest2()
        {
            string x = match( Number<SystemException>(9),
                              Succ: v  => "Worked",
                              Fail: ex => ex.Match<string>()
                                            .With<SystemException>(e => "It's a system exception")
                                            .With<ArgumentNullException>(e => "Arg null")
                                            .Otherwise("Not handled") );

            Assert.IsTrue(x == "It's a system exception");
        }

        [Test]
        public void ExTest3()
        {
            string x = match( Number<ArgumentNullException>(9),
                              Succ: v  => "Worked",
                              Fail: ex => ex.Match<string>()
                                            .With<SystemException>(e => "It's a system exception")
                                            .With<ArgumentNullException>(e => "Arg null")
                                            .Otherwise("Not handled") );

            Assert.IsTrue(x == "Arg null");
        }

        [Test]
        public void ExTest4()
        {
            string x = match( Number<Exception>(9),
                              Succ: v  => "Worked",
                              Fail: ex => ex.Match<string>()
                                            .With<SystemException>(e => "It's a system exception")
                                            .With<ArgumentNullException>(e => "Arg null")
                                            .Otherwise("Not handled") );

            Assert.IsTrue(x == "Not handled");
        }

        private static Try<int> Number<T>(int x) where T : Exception, new() => () =>
            x % 2 == 0
                ? x
                : raise<int>(new T());
    }
}
