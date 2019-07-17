using System;
using Xunit;
using LanguageExt;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class EitherUnsafeApply
    {
        Func<int, int, int> add = (a, b) => a + b;

        [Fact]
        public void ApplyRightArgs()
        {
            var either = RightUnsafe<string,Func<int,int,int>>(add)
                .Apply(RightUnsafe<string, int>(3))
                .Apply(RightUnsafe<string, int>(4));

            Assert.Equal(RightUnsafe<string, int>(7), either);
        }

        [Fact]
        public void ApplyRightArgsF()
        {
            var either = 
                apply(
                    apply(
                        RightUnsafe<string, Func<int, int, int>>(add),
                        RightUnsafe<string, int>(3)
                    ),
                    RightUnsafe<string, int>(4));
            Assert.Equal(RightUnsafe<string, int>(7), either);
        }

        [Fact]
        public void ApplyRightArgsF2()
        {
            var either = apply(
                    RightUnsafe<string, Func<int, int, int>>(add),
                    RightUnsafe<string, int>(3),
                    RightUnsafe<string, int>(4)
                );
            Assert.Equal(RightUnsafe<string, int>(7), either);
        }

        [Fact]
        public void ApplyLeftArgs()
        {
            var opt = Some(add)
                .Apply(None)
                .Apply(Some(4));

            Assert.Equal(None, opt);

            var either = RightUnsafe<string, Func<int, int, int>>(add)
                .Apply(LeftUnsafe<string, int>("left"))
                .Apply(RightUnsafe<string, int>(4));

            Assert.Equal(LeftUnsafe<string, int>("left"), either);
        }

        [Fact]
        public void ApplyLeftArgsF()
        {
            var either =
                apply(
                    apply(
                        RightUnsafe<string, Func<int, int, int>>(add),
                        LeftUnsafe<string, int>("left")
                    ),
                    RightUnsafe<string, int>(4));

            Assert.Equal(LeftUnsafe<string, int>("left"), either);
        }

        [Fact]
        public void ApplyLeftArgsF2()
        {
            var either = apply(
                    RightUnsafe<string, Func<int, int, int>>(add),
                    LeftUnsafe<string, int>("left"),
                    RightUnsafe<string, int>(4)
                );

            Assert.Equal(LeftUnsafe<string, int>("left"), either);
        }

        [Fact]
        public void ApplicativeLawHolds()
        {
            var first = RightUnsafe<string, Func<int, int, int>>(add)
                .Apply(RightUnsafe<string, int>(3))
                .Apply(RightUnsafe<string, int>(4));

            var second = RightUnsafe<string, int>(3)
                .ParMap(add)
                .Apply(RightUnsafe<string, int>(4));

            Assert.Equal(first, second);
        }

        [Fact]
        public void ApplicativeLawHoldsF()
        {
            var first = apply(
                    apply(
                        RightUnsafe<string, Func<int, int, int>>(add),
                        RightUnsafe<string, int>(3)
                    ),
                    RightUnsafe<string, int>(4));

            var second = apply(parmap(RightUnsafe<string, int>(3), add), RightUnsafe<string, int>(4));

            Assert.Equal(first, second);
        }

        [Fact]
        public void ApplicativeLawHoldsF2()
        {
            var first = apply(
                RightUnsafe<string, Func<int, int, int>>(add),
                RightUnsafe<string, int>(3),
                RightUnsafe<string, int>(4)
            );

            var second = apply(parmap(RightUnsafe<string, int>(3), add), RightUnsafe<string, int>(4));

            Assert.Equal(first, second);
        }
    }
}
