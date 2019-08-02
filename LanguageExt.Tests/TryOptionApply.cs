using LanguageExt;
using System;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class TryOptionApply
    {
        Func<int, int, int> add = (a, b) => a + b;
        TryOption<Func<int, int, int>> tryadd = TryOption (() => fun((int a, int b) => a + b));
        TryOption<int> three = () => 3;
        TryOption<int> four = () => 4;
        TryOption<int> seven = () => 7;
        TryOption<int> fail = () => failwith<int>("fail");

        TryOption<int> none = () => Option<int>.None;

        [Fact]
        public void ApplySuccArgs()
        {
            var comp = tryadd.Apply(three).Apply(four);

            Assert.Equal(seven.Try(), comp.Try());
        }

        [Fact]
        public void ApplySuccArgsF()
        {
            var comp = apply(apply(tryadd, three), four);
            Assert.Equal(seven.Try(), comp.Try());
        }

        [Fact]
        public void ApplySuccArgsF2()
        {
            var comp = apply(tryadd, three, four);
            Assert.Equal(seven.Try(), comp.Try());
        }

        [Fact]
        public void ApplyFailArgs()
        {
            var comp = tryadd
                .Apply(fail)
                .Apply(four);

            comp.Match(
                Some: x  => Assert.True(false),
                None: () => Assert.True(false),
                Fail: ex => Assert.True(ex.Message == "fail")
                );
        }

        [Fact]
        public void ApplyFailArgsF()
        {
            var comp = apply(apply(tryadd, fail), four);

            comp.Match(
                Some: x => Assert.True(false),
                None: () => Assert.True(false),
                Fail: ex => Assert.True(ex.Message == "fail")
            );
        }

        [Fact]
        public void ApplyFailArgsF2()
        {
            var comp = apply(tryadd, fail, four);

            comp.Match(
                Some: x => Assert.True(false),
                None: () => Assert.True(false),
                Fail: ex => Assert.True(ex.Message == "fail")
            );
        }

        [Fact]
        public void ApplyNoneArgs()
        {
            var comp = tryadd
                .Apply(none)
                .Apply(four);

            comp.Match(
                Some: x => Assert.True(false),
                None: () => Assert.True(true),
                Fail: ex => Assert.True(false)
            );
        }

        [Fact]
        public void ApplyNoneArgsF()
        {
            var comp = apply(apply(tryadd, none), four);

            comp.Match(
                Some: x => Assert.True(false),
                None: () => Assert.True(true),
                Fail: ex => Assert.True(false)
            );
        }

        [Fact]
        public void ApplyNoneArgsF2()
        {
            var comp = apply(tryadd, none, four);

            comp.Match(
                Some: x => Assert.True(false),
                None: () => Assert.True(true),
                Fail: ex => Assert.True(false)
            );
        }


        [Fact]
        public void ApplicativeLawHolds()
        {
            var first = tryadd
                .Apply(three)
                .Apply(four);

            var second = three
                .ParMap(add)
                .Apply(four);

            Assert.Equal(first.Try(), second.Try());
        }

        [Fact]
        public void ApplicativeLawHoldsF()
        {
            var first = apply(apply(tryadd, three), four);
            var second = apply(parmap(three, add), four);

            Assert.Equal(first.Try(), second.Try());
        }

        [Fact]
        public void ApplicativeLawHoldsF2()
        {
            var first = apply(tryadd, three, four);
            var second = apply(parmap(three, add), four);

            Assert.Equal(first.Try(), second.Try());
        }
    }
}
