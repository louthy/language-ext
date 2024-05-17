using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests;

public class EitherTests
{
    [Fact] public void RightGeneratorTestsObject()
    {
        Either<string, int> either = Right(123);

        either.Match( Right: i => Assert.True(i == 123),
                      Left:  _ => Assert.Fail("Shouldn't get here") );

        int c = either.Match( Right: i  => i + 1, 
                              Left: _ => 0 );

        Assert.True(c == 124);
    }

    [Fact] public void SomeGeneratorTestsFunction()
    {
        var either = Right<string, int>(123);

        match(either, Right: i => Assert.True(i == 123),
              Left:  _ => Assert.Fail("Shouldn't get here") );

        int c = match(either, Right: i => i + 1,
                      Left:  _ => 0 );

        Assert.True(c == 124);
    }

    [Fact] public void LeftGeneratorTestsObject()
    {
        var either = ItsLeft;

        either.Match( Right: r => Assert.Fail("Shouldn't get here"),
                      Left:  l => Assert.True(l == "Left") );

        int c = either.Match( Right: r => r + 1, 
                              Left:  l => 0 );

        Assert.True(c == 0);
    }

    [Fact] public void LeftGeneratorTestsFunction()
    {
        var either = ItsLeft;

        match(either, Right: r => Assert.Fail("Shouldn't get here"),
              Left:  l => Assert.True(l == "Left") );

        int c = match(either, Right: r => r + 1,
                      Left:  l => 0 );

        Assert.True(c == 0);
    }

    [Fact]
    public void SomeLinqTest() =>
        (from x in Two
         from y in Four
         from z in Six
         select x + y + z)
       .Match(
            Right: r => Assert.True(r == 12),
            Left:  _ => Assert.Fail("Shouldn't get here"));

    [Fact] public void LeftLinqTest() =>
        (from x in Two
         from y in Four
         from _ in ItsLeft
         from z in Six
         select x + y + z)
       .Match(
            l => Assert.True(l == "Left"),
            _ => Assert.Fail("Shouldn't get here"));

    [Fact] public void EitherFluentSomeNoneTest()
    {
        int res1 = GetValue(true)
                  .Right(r => r + 10)
                  .Left (l => l.Length);

        int res2 = GetValue(false)
                  .Right(r => r + 10)
                  .Left (l => l.Length);

        Assert.True(res1 == 1010);
        Assert.True(res2 == 4);
    }

    private Either<string, int> GetValue(bool select)
    {
        if (select)
        {
            return 1000;
        }
        else
        {
            return "Left";
        }
    }

    [Fact]
    public void EitherLinqTest1() =>
        (from x in Right(2)
         from _ in Left("error")
         from z in Right(5)
         select x + z)
       .Match(Right: _ => Assert.Fail("Shouldn't get here"),
              Left: _ => Assert.True(true));

    [Fact]
    public void EitherLinqTest2() =>
        (from x in Right(2)
         from y in Right<string, int>(123)
         from z in Right(5)
         select x + y + z)
        .Match(Right: r => Assert.True(r == 130),
               Left: _ => Assert.True(false));

    [Fact]
    public void EitherCoalesce()
    {
        var x = Right<string, int>(1) || Right<string, int>(2) || Left<string, int>("error");
    }

    [Fact]
    public void EitherInfer1() => AddEithers(Right(10), Left("error"));

    void EitherInfer2(bool v)
    {
        Either<string, int> x =
            v ? Right(10)
              : Left("error");
    }

    public Either<string, int> AddEithers(Either<string, int> x, Either<string, int> y) =>
        from a in x
        from b in y
        select a + b;

    private Either<string, int> ImplicitConversion() => 1000;
    private Either<string, int> ItsLeft => "Left";
    private Either<string, int> Two => 2;
    private Either<string, int> Four => 4;
    private Either<string, int> Six => 6;
}
