using System;
using Xunit;
using LanguageExt;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;
using static LanguageExt.TypeClass;

namespace LanguageExt.Tests
{
    public class OptionApply
    {
        Func<int, int, int> add = (a, b) => a + b;

        [Fact]
        public void ApplySomeArgs()
        {
            var opt = Some(add)
                .Apply(Some(3))
                .Apply(Some(4));

            Assert.Equal(Some(7), opt);
        }

        [Fact]
        public void ApplySomeArgsF()
        {
            var opt = apply(apply(Some(add), Some(3)), Some(4));
            Assert.Equal(Some(7), opt);
        }

        [Fact]
        public void ApplySomeArgsF2()
        {
            var opt = Some(add).Apply(Some(3), Some(4));

            Assert.True(equals<EqInt, int>(Some(7), opt));
        }

        [Fact]
        public void ApplyNoneArgs()
        {
            var opt = Some(add)
                .Apply(None)
                .Apply(Some(4));

            Assert.Equal(None, opt);
        }

        [Fact]
        public void ApplyNoneArgsF()
        {
            var opt = apply(apply(Some(add), None), Some(4));
            Assert.Equal(None, opt);
        }

        [Fact]
        public void ApplyNoneArgsF2()
        {
            var opt = Some(add).Apply(Option<int>.None, Some(4));

            Assert.True(equals<EqInt, int>(Option<int>.None, opt));
        }

        [Fact]
        public void ApplicativeLawHolds()
        {
            var first = Some(add)
                .Apply(Some(3))
                .Apply(Some(4));

            var second = Some(3)
                .ParMap(add)
                .Apply(Some(4));

            Assert.Equal(first, second);
        }

        [Fact]
        public void ApplicativeLawHoldsF()
        {
            var first = apply(apply(Some(add), Some(3)), Some(4));
            var second = apply(parmap(Some(3), add), Some(4));

            Assert.Equal(first, second);
        }

        [Fact]
        public void ApplicativeLawHoldsF2()
        {
            var first = Some(add).Apply(Some(3), Some(4));

            var second = parmap(Some(3), add).Apply(Some(4));

            Assert.True(equals<EqInt, int>(first, second));
        }
    }
}
