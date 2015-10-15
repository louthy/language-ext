using LanguageExt;
using System;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExtTests
{
    public class TryOptionApply
    {
        Func<int, int, int> add = (a, b) => a + b;
        TryOption<Func<int, int, int>> tryadd = () => fun((int a, int b) => a + b);
        TryOption<int> three = () => 3;
        TryOption<int> four = () => 4;
        TryOption<int> seven = () => 7;
        TryOption<int> fail = () => failwith<int>("fail");

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

            comp.Iter(
                Some: x  => Assert.True(false),
                None: () => Assert.True(false),
                Fail: ex => Assert.True(ex.Message == "fail")
                );
        }

        [Fact]
        public void ApplyNoneArgsF()
        {
            var comp = apply(apply(tryadd, fail), four);

            comp.Iter(
                Some: x => Assert.True(false),
                None: () => Assert.True(false),
                Fail: ex => Assert.True(ex.Message == "fail")
                );
        }

        [Fact]
        public void ApplyNoneArgsF2()
        {
            var comp = apply(tryadd, fail, four);

            comp.Iter(
                Some: x => Assert.True(false),
                None: () => Assert.True(false),
                Fail: ex => Assert.True(ex.Message == "fail")
                );
        }

        [Fact]
        public void ApplicativeLawHolds()
        {
            var first = tryadd
                .Apply(three)
                .Apply(four);

            var second = three
                .Map(add)
                .Apply(four);

            Assert.Equal(first.Try(), second.Try());
        }

        [Fact]
        public void ApplicativeLawHoldsF()
        {
            var first = apply(apply(tryadd, three), four);
            var second = apply(map(three, add), four);

            Assert.Equal(first.Try(), second.Try());
        }

        [Fact]
        public void ApplicativeLawHoldsF2()
        {
            var first = apply(tryadd, three, four);
            var second = apply(map(three, add), four);

            Assert.Equal(first.Try(), second.Try());
        }
    }
}
