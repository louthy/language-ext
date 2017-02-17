using Xunit;
using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExtTests
{
    
    public class OptionTests
    {
        [Fact]
        public void SomeGeneratorTestsObject()
        {
            var optional = Some(123);

            optional.Match(Some: i => Assert.True(i == 123),
                           None: () => Assert.False(true,"Shouldn't get here"));

            int c = optional.Match(Some: i => i + 1,
                                   None: () => 0);

            Assert.True(c == 124);
        }

        [Fact]
        public void SomeGeneratorTestsFunction()
        {
            var optional = Some(123);

            match(optional, Some: i => Assert.True(i == 123),
                            None: () => Assert.False(true,"Shouldn't get here"));

            int c = match(optional, Some: i => i + 1,
                                    None: () => 0);

            Assert.True(c == 124);
        }

        [Fact]
        public void NoneGeneratorTestsObject()
        {
            Option<int> optional = None;

            optional.Match(Some: i => Assert.False(true,"Shouldn't get here"),
                           None: () => Assert.True(true));

            int c = optional.Match(Some: i => i + 1,
                                   None: () => 0);

            Assert.True(c == 0);
        }

        [Fact]
        public void NoneGeneratorTestsFunction()
        {
            Option<int> optional = None;

            match(optional, Some: i => Assert.False(true,"Shouldn't get here"),
                            None: () => Assert.True(true));

            int c = match(optional, Some: i => i + 1,
                                    None: () => 0);

            Assert.True(c == 0);
        }

        [Fact]
        public void SomeLinqTest()
        {
            var two = Some(2);
            var four = Some(4);
            var six = Some(6);

            match(from x in two
                  from y in four
                  from z in six
                  select x + y + z,
                   Some: v => Assert.True(v == 12),
                   None: failwith("Shouldn't get here"));
        }

        [Fact]
        public void NoneLinqTest()
        {
            var two = Some(2);
            var four = Some(4);
            var six = Some(6);
            Option<int> none = None;

            match(from x in two
                  from y in four
                  from _ in none
                  from z in six
                  select x + y + z,
                   Some: v => failwith<int>("Shouldn't get here"),
                   None: () => Assert.True(true));
        }

        [Fact]
        public void NullIsNotSomeTest()
        {
            Assert.Throws(
                typeof(ValueIsNullException),
                () =>
                {
                    GetStringNone();
                }
            );
        }

        [Fact]
        public void NullIsNoneTest()
        {
            Assert.True(GetStringNone2().IsNone);
        }

        [Fact]
        public void OptionFluentSomeNoneTest()
        {
            int res1 = GetValue(true)
                        .Some(x => x + 10)
                        .None(0);

            int res2 = GetValue(false)
                        .Some(x => x + 10)
                        .None(() => 0);

            Assert.True(res1 == 1010);
            Assert.True(res2 == 0);
        }

        [Fact]
        public void NullInSomeOrNoneTest()
        {
            Assert.Throws(
                typeof(ResultIsNullException),
                () =>
                {
                    GetValue(true)
                       .Some(x => (string)null)
                       .None((string)null);
                }
            );

            Assert.Throws(
                typeof(ResultIsNullException),
                () =>
                {
                    GetValue(false)
                       .Some(x => (string)null)
                       .None((string)null);
                }
            );
        }

        [Fact]
        public void NullableTest()
        {
            var res = GetNullable(true)
                        .Some(v => v)
                        .None(() => 0);

            Assert.True(res == 1000);
        }

        [Fact]
        public void NullableDenySomeNullTest()
        {
            Assert.Throws(
                    typeof(ValueIsNullException),
                    () =>
                    {
                        var res = GetNullable(false)
                                    .Some(v => v)
                                    .None(() => 0);
                    }
                );
        }

        [Fact]
        public void NullableNullIsNone()
        {
#pragma warning disable CS0612 // Type or member is obsolete

            Assert.True(Option<int>.None == null);
            Assert.True(null == Option<int>.None);
            Assert.True(Option<int>.None == default(int?));
            Assert.True(default(int?) == Option<int>.None);

            Assert.False(Option<int>.None != null);
            Assert.False(null != Option<int>.None);
            Assert.False(Option<int>.None != default(int?));
            Assert.False(default(int?) != Option<int>.None);

            var some = Some(1);

            Assert.False(some == null);
            Assert.False(null == some);
            Assert.False(some == default(int?));
            Assert.False(default(int?) == some);

            Assert.True(some != null);
            Assert.True(null != some);
            Assert.True(some != default(int?));
            Assert.True(default(int?) != some);

#pragma warning restore CS0612 // Type or member is obsolete
        }

        [Fact]
        public void NullableNullIsNotSome()
        {
#pragma warning disable CS0612 // Type or member is obsolete

            var some = Some(1);

            Assert.False(some == null);
            Assert.False(null == some);
            Assert.False(some == default(int?));
            Assert.False(default(int?) == some);

            Assert.True(some != null);
            Assert.True(null != some);
            Assert.True(some != default(int?));
            Assert.True(default(int?) != some);

#pragma warning restore CS0612 // Type or member is obsolete
        }

        private Option<string> GetStringNone()
        {
            // This should fail
            string nullStr = null;
            return Some(nullStr);
        }

        private Option<string> GetStringNone2()
        {
            // This should be coerced to None
            string nullStr = null;
            return nullStr;
        }

        private Option<int> GetNullable(bool select) =>
            select
                ? Some((int?)1000)
                : Some((int?)null);

        private Option<int> GetValue(bool select) =>
            select
                ? Some(1000)
                : None;

        private Option<Option<int>> GetSomeOptionValue(bool select) =>
            select
                ? Some(Some(1000))
                : Some(Option<int>.None);

        private Option<int> ImplicitConversion() => 1000;

        private Option<int> ImplicitNoneConversion() => None;

        private void InferenceTest1()
        {
            Action<int> actionint = v => v = v * 2;
            Option<int> optional1 = Some(123);
            optional1.Some(actionint) //Compiler tries to call:  public static Option<T> Some(T value)
                     .None(() => { });
        }
    }
}
