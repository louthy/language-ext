using FluentAssertions;
using LanguageExt.Common;
using LanguageExt.Sys.Test;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.ScheduleTest;

public static class EffTests
{
    static Schedule TestSchedule() => Schedule.Fixed(1 * ms) | Schedule.NoDelayOnFirst | Schedule.Recurs(5);

    [Fact]
    public static void BailBeforeScheduleTest1()
    {
        const int counter = 0;
        var effect = FailEff<int>("Failed");
        effect.Repeat(TestSchedule()).Run();
        counter.Should().Be(0);
    }

    [Fact]
    public static void BailBeforeScheduleTest2()
    {
        const int counter = 0;
        var effect = FailEff<Runtime, int>("Failed");
        effect.Repeat(TestSchedule()).Run(Runtime.New());
        counter.Should().Be(0);
    }

    [Fact]
    public static void RepeatTest1()
    {
        var counter = 0;
        var effect = EffMaybe<int>(() => ++counter);
        effect.Repeat(TestSchedule()).RunUnit();
        counter.Should().Be(6);
    }

    [Fact]
    public static void RepeatTest2()
    {
        var counter = 0;
        var effect = EffMaybe<Runtime, int>(_ => ++counter);
        effect.Repeat(TestSchedule()).RunUnit(Runtime.New());
        counter.Should().Be(6);
    }

    [Fact]
    public static void RetryTest1()
    {
        var counter = 0;
        var effect = EffMaybe<int>(() =>
        {
            ++counter;
            return Error.New("Failed");
        });
        effect.Retry(TestSchedule()).Run();
        counter.Should().Be(6);
    }

    [Fact]
    public static void RetryTest2()
    {
        var counter = 0;
        var effect = EffMaybe<Runtime, int>(_ =>
        {
            ++counter;
            return Error.New("Failed");
        });
        effect.Retry(TestSchedule()).Run(Runtime.New());
        counter.Should().Be(6);
    }

    [Fact]
    public static void RepeatWhileTest1()
    {
        var counter = 0;
        var effect = EffMaybe<int>(() => ++counter);
        effect.RepeatWhile(TestSchedule(), static i => i < 3).RunUnit();
        counter.Should().Be(3);
    }

    [Fact]
    public static void RepeatWhileTest2()
    {
        var counter = 0;
        var effect = EffMaybe<Runtime, int>(_ => ++counter);
        effect.RepeatWhile(TestSchedule(), static i => i < 3).RunUnit(Runtime.New());
        counter.Should().Be(3);
    }

    [Fact]
    public static void RetryWhileTest1()
    {
        var counter = 0;
        var effect = EffMaybe<int>(() =>
        {
            ++counter;
            return Error.New(counter.ToString());
        });
        effect.RetryWhile(TestSchedule(), static e => (int)parseInt(e.Message) < 3).Run();
        counter.Should().Be(3);
    }

    [Fact]
    public static void RetryWhileTest2()
    {
        var counter = 0;
        var effect = EffMaybe<Runtime, int>(_ =>
        {
            ++counter;
            return Error.New(counter.ToString());
        });
        effect.RetryWhile(TestSchedule(), static e => (int)parseInt(e.Message) < 3).Run(Runtime.New());
        counter.Should().Be(3);
    }

    [Fact]
    public static void RepeatUntilTest1()
    {
        var counter = 0;
        var effect = EffMaybe<int>(() => ++counter);
        effect.RepeatUntil(static i => i == 10).RunUnit();
        counter.Should().Be(10);
    }

    [Fact]
    public static void RepeatUntilTest2()
    {
        var counter = 0;
        var effect = EffMaybe<Runtime, int>(_ => ++counter);
        effect.RepeatUntil(static i => i == 10).RunUnit(Runtime.New());
        counter.Should().Be(10);
    }

    [Fact]
    public static void RetryUntilTest1()
    {
        var counter = 0;
        var effect = EffMaybe<int>(() =>
        {
            ++counter;
            return Error.New(counter.ToString());
        });
        effect.RetryUntil(static e => (int)parseInt(e.Message) == 10).Run();
        counter.Should().Be(10);
    }

    [Fact]
    public static void RetryUntilTest2()
    {
        var counter = 0;
        var effect = EffMaybe<Runtime, int>(_ =>
        {
            ++counter;
            return Error.New(counter.ToString());
        });
        effect.RetryUntil(static e => (int)parseInt(e.Message) == 10).Run(Runtime.New());
        counter.Should().Be(10);
    }

    [Fact]
    public static void FoldTest1()
    {
        var counter = 0;
        var effect = EffMaybe<int>(() => ++counter);
        var result = effect.Fold(TestSchedule(), 1, (i, j) => i + j).Run().ThrowIfFail();
        result.Should().Be(22);
    }

    [Fact]
    public static void FoldTest2()
    {
        var counter = 0;
        var effect = EffMaybe<Runtime, int>(_ => ++counter);
        var result = effect.Fold(TestSchedule(), 1, (i, j) => i + j).Run(Runtime.New()).ThrowIfFail();
        result.Should().Be(22);
    }

    [Fact]
    public static void FoldWhileTest1()
    {
        var counter = 0;
        var effect = EffMaybe<int>(() => ++counter);
        var result = effect.FoldWhile(TestSchedule(), 1, (i, j) => i + j, i => i < 3).Run().ThrowIfFail();
        result.Should().Be(7);
    }

    [Fact]
    public static void FoldWhileTest2()
    {
        var counter = 0;
        var effect = EffMaybe<Runtime, int>(_ => ++counter);
        var result = effect.FoldWhile(TestSchedule(), 1, (i, j) => i + j, i => i < 3).Run(Runtime.New()).ThrowIfFail();
        result.Should().Be(7);
    }

    [Fact]
    public static void FoldUntilTest1()
    {
        var counter = 0;
        var effect = EffMaybe<int>(() => ++counter);
        var result = effect.FoldUntil(TestSchedule(), 1, (i, j) => i + j, i => i > 4).Run().ThrowIfFail();
        result.Should().Be(16);
    }

    [Fact]
    public static void FoldUntilTest2()
    {
        var counter = 0;
        var effect = EffMaybe<Runtime, int>(_ => ++counter);
        var result = effect.FoldUntil(TestSchedule(), 1, (i, j) => i + j, i => i > 4).Run(Runtime.New()).ThrowIfFail();
        result.Should().Be(16);
    }
}
