using Xunit;
using System;
using System.Collections.Generic;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    
    public class OptionTests
    {
        [Fact]
        public void SomeGeneratorTestsObject()
        {
            var optional = Some(123);

            optional.Match(Some: i => Assert.True(i == 123),
                           None: () => Assert.Fail("Shouldn't get here"));

            int c = optional.Match(Some: i => i + 1,
                                   None: () => 0);

            Assert.True(c == 124);
        }

        [Fact]
        public void SomeGeneratorTestsFunction()
        {
            var optional = Some(123);

            match(optional, Some: i => Assert.True(i == 123),
                            None: () => Assert.Fail("Shouldn't get here"));

            int c = match(optional, Some: i => i + 1,
                                    None: () => 0);

            Assert.True(c == 124);
        }

        [Fact]
        public void NoneGeneratorTestsObject()
        {
            Option<int> optional = None;

            optional.Match(Some: i => Assert.Fail("Shouldn't get here"),
                           None: () => Assert.True(true));

            int c = optional.Match(Some: i => i + 1,
                                   None: () => 0);

            Assert.True(c == 0);
        }

        [Fact]
        public void NoneGeneratorTestsFunction()
        {
            Option<int> optional = None;

            match(optional, Some: i => Assert.Fail("Shouldn't get here"),
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

            var expr = from x in two
                       from y in four
                       from z in six
                       select x + y + z;

            match(expr,
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
            Assert.Throws<ValueIsNullException>(
                    () =>
                    {
                        var res = GetNullable(false)
                                    .Some(v => v)
                                    .None(() => 0);
                    }
                );
        }

        [Fact]
        public void BiIterSomeTest()
        {
            var x = Some(3);
            int way = 0;
            var dummy = x.BiIter(_ => way = 1, () => way = 2);
            Assert.Equal(1, way);
        }

        [Fact]
        public void BiIterNoneTest()
        {
            var x = Option<int>.None;
            int way = 0;
            var dummy = x.BiIter(_ => way = 1, () => way = 2);
            Assert.Equal(2, way);
        }

        [Fact]
        public void IfNoneSideEffect()
        {
            int sideEffectResult = 0;

            Action sideEffectNone = () => sideEffectResult += 1;

            Assert.Equal(0, Option<string>.Some("test").IfNone(sideEffectNone).Return(sideEffectResult));
            Assert.Equal(1, Option<string>.None.IfNone(sideEffectNone).Return(sideEffectResult));
        }

        [Fact]
        public void ISomeSideEffect()
        {
            int sideEffectResult = 0;

            Action<string> sideEffectSome = _ => sideEffectResult += 2;

            Assert.Equal(0, Option<string>.None.IfSome(sideEffectSome).Return(sideEffectResult));
            Assert.Equal(2, Option<string>.Some("test").IfSome(sideEffectSome).Return(sideEffectResult));
        }

        [Fact]
        public void OptionToFin()
        {
            var e = LanguageExt.Common.Error.New("Example error");
            var some = Some(123);
            var none = Option<int>.None;

            var mx = FinSucc(123);
            var my = some.ToFin();
            var me = none.ToFin(e);

            var e1 = mx == my;
            var e2 = mx.Equals(my);
            var e3 = mx.Equals((object)my);
            
            Assert.True(e1);
            Assert.True(e2);
            Assert.True(e3);
            Assert.True(me.IsFail);
        }

        private Option<string?> GetStringNone()
        {
            string? nullStr = null;
            return Some(nullStr);
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
