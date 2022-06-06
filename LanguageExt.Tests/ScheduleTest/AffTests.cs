using System.Threading.Tasks;
using FluentAssertions;
using LanguageExt.Common;
using LanguageExt.Sys.Test;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.ScheduleTest;

public static class AffTests
{
    static Schedule TestSchedule() => Schedule.fix(1 * ms) | Schedule.NoDelayOnFirst | Schedule.recurs(5);

    [Fact]
    public static async Task BailBeforeScheduleTest1()
    {
        const int counter = 0;
        var effect = FailAff<int>("Failed");
        await effect.Repeat(TestSchedule()).Run();
        counter.Should().Be(0);
    }

    [Fact]
    public static async Task BailBeforeScheduleTest2()
    {
        const int counter = 0;
        var effect = FailAff<Runtime, int>("Failed");
        await effect.Repeat(TestSchedule()).Run(Runtime.New());
        counter.Should().Be(0);
    }

    [Fact]
    public static async Task RepeatTest1()
    {
        var counter = 0;
        var effect = AffMaybe<int>(async () => await (++counter).AsValueTask());
        await effect.Repeat(TestSchedule()).RunUnit();
        counter.Should().Be(6);
    }

    [Fact]
    public static async Task RepeatTest2()
    {
        var counter = 0;
        var effect = AffMaybe<Runtime, int>(async _ => await (++counter).AsValueTask());
        await effect.Repeat(TestSchedule()).RunUnit(Runtime.New());
        counter.Should().Be(6);
    }

    [Fact]
    public static async Task RetryTest1()
    {
        var counter = 0;
        var effect = AffMaybe<int>(async () =>
        {
            await (++counter).AsValueTask();
            return Error.New("Failed");
        });
        await effect.Retry(TestSchedule()).Run();
        counter.Should().Be(6);
    }

    [Fact]
    public static async Task RetryTest2()
    {
        var counter = 0;
        var effect = AffMaybe<Runtime, int>(async _ =>
        {
            await (++counter).AsValueTask();
            return Error.New("Failed");
        });
        await effect.Retry(TestSchedule()).Run(Runtime.New());
        counter.Should().Be(6);
    }

    [Fact]
    public static async Task RepeatWhileTest1()
    {
        var counter = 0;
        var effect = AffMaybe<int>(async () => await (++counter).AsValueTask());
        await effect.RepeatWhile(TestSchedule(), static i => i < 3).RunUnit();
        counter.Should().Be(3);
    }

    [Fact]
    public static async Task RepeatWhileTest2()
    {
        var counter = 0;
        var effect = AffMaybe<Runtime, int>(async _ => await (++counter).AsValueTask());
        await effect.RepeatWhile(TestSchedule(), static i => i < 3).RunUnit(Runtime.New());
        counter.Should().Be(3);
    }

    [Fact]
    public static async Task RetryWhileTest1()
    {
        var counter = 0;
        var effect = AffMaybe<int>(async () =>
        {
            await (++counter).AsValueTask();
            return Error.New(counter.ToString());
        });
        await effect.RetryWhile(TestSchedule(), static e => (int)parseInt(e.Message) < 3).Run();
        counter.Should().Be(3);
    }

    [Fact]
    public static async Task RetryWhileTest2()
    {
        var counter = 0;
        var effect = AffMaybe<Runtime, int>(async _ =>
        {
            await (++counter).AsValueTask();
            return Error.New(counter.ToString());
        });
        await effect.RetryWhile(TestSchedule(), static e => (int)parseInt(e.Message) < 3).Run(Runtime.New());
        counter.Should().Be(3);
    }

    [Fact]
    public static async Task RepeatUntilTest1()
    {
        var counter = 0;
        var effect = AffMaybe<int>(async () => await (++counter).AsValueTask());
        await effect.RepeatUntil(static i => i == 10).RunUnit();
        counter.Should().Be(10);
    }

    [Fact]
    public static async Task RepeatUntilTest2()
    {
        var counter = 0;
        var effect = AffMaybe<Runtime, int>(async _ => await (++counter).AsValueTask());
        await effect.RepeatUntil(static i => i == 10).RunUnit(Runtime.New());
        counter.Should().Be(10);
    }

    [Fact]
    public static async Task RetryUntilTest1()
    {
        var counter = 0;
        var effect = AffMaybe<int>(async () =>
        {
            await (++counter).AsValueTask();
            return Error.New(counter.ToString());
        });
        await effect.RetryUntil(static e => (int)parseInt(e.Message) == 10).Run();
        counter.Should().Be(10);
    }

    [Fact]
    public static async Task RetryUntilTest2()
    {
        var counter = 0;
        var effect = AffMaybe<Runtime, int>(async _ =>
        {
            await (++counter).AsValueTask();
            return Error.New(counter.ToString());
        });
        await effect.RetryUntil(static e => (int)parseInt(e.Message) == 10).Run(Runtime.New());
        counter.Should().Be(10);
    }

    [Fact]
    public static async Task FoldTest1()
    {
        var counter = 0;
        var effect = AffMaybe<int>(async () => await (++counter).AsValueTask());
        var result = (await effect.Fold(TestSchedule(), 1, (i, j) => i + j).Run()).ThrowIfFail();
        result.Should().Be(22);
    }

    [Fact]
    public static async Task FoldTest2()
    {
        var counter = 0;
        var effect = AffMaybe<Runtime, int>(async _ => await (++counter).AsValueTask());
        var result = (await effect.Fold(TestSchedule(), 1, (i, j) => i + j).Run(Runtime.New())).ThrowIfFail();
        result.Should().Be(22);
    }

    [Fact]
    public static async Task FoldWhileTest1()
    {
        var counter = 0;
        var effect = AffMaybe<int>(async () => await (++counter).AsValueTask());
        var result = (await effect.FoldWhile(TestSchedule(), 1, (i, j) => i + j, i => i < 3).Run()).ThrowIfFail();
        result.Should().Be(7);
    }

    [Fact]
    public static async Task FoldWhileTest2()
    {
        var counter = 0;
        var effect = AffMaybe<Runtime, int>(async _ => await (++counter).AsValueTask());
        var result = (await effect.FoldWhile(TestSchedule(), 1, (i, j) => i + j, i => i < 3).Run(Runtime.New()))
            .ThrowIfFail();
        result.Should().Be(7);
    }

    [Fact]
    public static async Task FoldUntilTest1()
    {
        var counter = 0;
        var effect = AffMaybe<int>(async () => await (++counter).AsValueTask());
        var result = (await effect.FoldUntil(TestSchedule(), 1, (i, j) => i + j, i => i > 4).Run()).ThrowIfFail();
        result.Should().Be(16);
    }

    [Fact]
    public static async Task FoldUntilTest2()
    {
        var counter = 0;
        var effect = AffMaybe<Runtime, int>(async _ => await (++counter).AsValueTask());
        var result = (await effect.FoldUntil(TestSchedule(), 1, (i, j) => i + j, i => i > 4).Run(Runtime.New()))
            .ThrowIfFail();
        result.Should().Be(16);
    }
}
