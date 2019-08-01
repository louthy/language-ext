using System;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests
{
    public class OptionUnsafeApply
    {
        Func<int, int, int> add = (a, b) => a + b;

        // TODO: Restore

        //[Fact]
        //public void ApplySomeArgs()
        //{
        //    var opt = SomeUnsafe(add)
        //        .Apply(SomeUnsafe(3))
        //        .Apply(SomeUnsafe(4));

        //    Assert.Equal(SomeUnsafe(7), opt);
        //}

        //[Fact]
        //public void ApplySomeArgsF()
        //{
        //    var opt = apply(apply(SomeUnsafe(add), SomeUnsafe(3)), SomeUnsafe(4));
        //    Assert.Equal(SomeUnsafe(7), opt);
        //}

        //[Fact]
        //public void ApplyNoneArgs()
        //{
        //    var opt = SomeUnsafe(add)
        //        .Apply(None)
        //        .Apply(SomeUnsafe(4));

        //    Assert.Equal(None, opt);
        //}

        //[Fact]
        //public void ApplyNoneArgsF()
        //{
        //    var opt = apply(apply(SomeUnsafe(add), None), SomeUnsafe(4));
        //    Assert.Equal(None, opt);
        //}

        //[Fact]
        //public void ApplicativeLawHolds()
        //{
        //    var first = SomeUnsafe(add)
        //        .Apply(SomeUnsafe(3))
        //        .Apply(SomeUnsafe(4));

        //    var second = SomeUnsafe(3)
        //        .ParMap(add)
        //        .Apply(SomeUnsafe(4));

        //    Assert.Equal(first, second);
        //}

        //[Fact]
        //public void ApplicativeLawHoldsF()
        //{
        //    var first = apply(apply(SomeUnsafe(add), SomeUnsafe(3)), SomeUnsafe(4));
        //    var second = apply(parmap(SomeUnsafe(3), add), SomeUnsafe(4));

        //    Assert.Equal(first, second);
        //}
    }
}
