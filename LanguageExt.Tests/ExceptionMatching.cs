using Xunit;
using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    
    public class ExceptionMatching
    {
        [Fact]
        public void ExTestMatchSuccess()
        {
            string x = match( Number<InvalidOperationException>(10),
                              Succ: v  => "Worked",
                              Fail: ex => ex.Match<string>()
                                            .With<InvalidOperationException>(e => "It's an invalid operation exception")
                                            .With<ArgumentNullException>(e => "Arg null")
                                            .Otherwise("Not handled") );

            Assert.True(x == "Worked");
        }

        [Fact]
        public void ExTestMatchException1()
        {
            string x = match( Number<InvalidOperationException>(9),
                              Succ: v  => "Worked",
                              Fail: ex => ex.Match<string>()
                                            .With<InvalidOperationException>(e => "It's an invalid operation exception")
                                            .With<ArgumentNullException>(e => "Arg null")
                                            .Otherwise("Not handled") );

            Assert.True(x == "It's an invalid operation exception");
        }

        [Fact]
        public void ExTestMatchException2()
        {
            string x = match( Number<ArgumentNullException>(9),
                              Succ: v  => "Worked",
                              Fail: ex => ex.Match<string>()
                                            .With<InvalidOperationException>(e => "It's an invalid operation exception")
                                            .With<ArgumentNullException>(e => "Arg null")
                                            .Otherwise("Not handled") );

            Assert.True(x == "Arg null");
        }

        [Fact]
        public void ExTestMatchExceptionOtherwise()
        {
            string x = match( Number<Exception>(9),
                              Succ: v  => "Worked",
                              Fail: ex => ex.Match<string>()
                                            .With<InvalidOperationException>(e => "It's an invalid operation exception")
                                            .With<ArgumentNullException>(e => "Arg null")
                                            .Otherwise("Not handled") );

            Assert.True(x == "Not handled");
        }

        private static Try<int> Number<T>(int x) where T : Exception, new() => () =>
            x % 2 == 0
                ? x
                : raise<int>(new T());
    }
}
