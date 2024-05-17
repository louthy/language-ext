using System;
using LanguageExt.Traits;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests;

public class EitherApply
{
    Func<int, int, int> add = (a, b) => a + b;

    [Fact]
    public void ApplyRightArgs()
    {
        var either = Right<string, Func<int,int,int>>(add)
                    .Apply(Right<string, int>(3))
                    .Apply(Right<string, int>(4));

        Assert.Equal(Right<string, int>(7), either);
    }

    [Fact]
    public void ApplyRightArgsF()
    {
        var either = 
            apply(
                apply(
                    Right<string, Func<int, int, int>>(add),
                    Right<string, int>(3)
                ),
                Right<string, int>(4));
        Assert.Equal(Right<string, int>(7), either);
    }

    [Fact]
    public void ApplyRightArgsF2()
    {
        var either = apply(
            Right<string, Func<int, int, int>>(add),
            Right<string, int>(3),
            Right<string, int>(4)
        );
        Assert.Equal(Right<string, int>(7), either);
    }

    [Fact]
    public void ApplyLeftArgs()
    {
        var opt  = Some(add);
        var none = Option<int>.None;
        var four = Some(4); 

        var res = apply(opt, none, four);

        Assert.Equal(None, res);

        var either = Right<string, Func<int, int, int>>(add)
                    .Apply(Left<string, int>("left"))
                    .Apply(Right<string, int>(4));

        Assert.Equal(Left<string, int>("left"), either);
    }

    [Fact]
    public void ApplyLeftArgsF()
    {
        var either =
            apply(
                apply(
                    Right<string, Func<int, int, int>>(add),
                    Left<string, int>("left")
                ),
                Right<string, int>(4));

        Assert.Equal(Left<string, int>("left"), either);
    }

    [Fact]
    public void ApplyLeftArgsF2()
    {
        var either = apply(
            Right<string, Func<int, int, int>>(add),
            Left<string, int>("left"),
            Right<string, int>(4)
        );

        Assert.Equal(Left<string, int>("left"), either);
    }

    [Fact]
    public void ApplicativeLawHolds()
    {
        var first = Right<string, Func<int, int, int>>(add)
                   .Apply(Right<string, int>(3))
                   .Apply(Right<string, int>(4));

        var second = add.Map(Right<string, int>(3))
                        .Apply(Right<string, int>(4));

        Assert.Equal(first, second);
    }

    [Fact]
    public void ApplicativeLawHoldsF()
    {
        var first = apply(
            apply(
                Right<string, Func<int, int, int>>(add),
                Right<string, int>(3)
            ),
            Right<string, int>(4));

        var second = add.Map(Right<string, int>(3)).Apply(Right<string, int>(4));

        Assert.Equal(first, second);
    }

    [Fact]
    public void ApplicativeLawHoldsF2()
    {
        var first = apply(
            Right<string, Func<int, int, int>>(add),
            Right<string, int>(3),
            Right<string, int>(4)
        );

        var second = add.Map(Right<string, int>(3)).Apply(Right<string, int >(4));

        Assert.Equal(first, second);
    }
}
