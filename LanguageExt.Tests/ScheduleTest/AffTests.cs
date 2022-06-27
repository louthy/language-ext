using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using LanguageExt.Common;
using LanguageExt.Sys;
using LanguageExt.Sys.Test;
using LanguageExt.Sys.Traits;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.ScheduleTest;

public static class AffTests
{
    static Schedule TestSchedule() => Schedule.fixedInterval(1 * ms) | Schedule.NoDelayOnFirst | Schedule.recurs(5);

    [Fact]
    public static async Task BailBeforeScheduleTest1()
    {
        const int counter = 0;
        var effect = FailAff<int>("Failed");
        var result = await effect.Repeat(TestSchedule()).Run();
        counter.Should().Be(0);
        result.Case.Should().Be(Error.New("Failed"));
    }

    [Fact]
    public static async Task BailBeforeScheduleTest2()
    {
        const int counter = 0;
        var effect = FailAff<Runtime, int>("Failed");
        var result = await effect.Repeat(TestSchedule()).Run(Runtime.New());
        counter.Should().Be(0);
        result.Case.Should().Be(Error.New("Failed"));
    }

    [Fact]
    public static async Task RepeatTest1()
    {
        var counter = 0;
        var effect = AffMaybe<int>(async () => await (++counter).AsValueTask());
        var result = await effect.Repeat(TestSchedule()).Run();
        counter.Should().Be(6);
        result.Case.Should().Be(6);
    }

    [Fact]
    public static async Task RepeatTest2()
    {
        var counter = 0;
        var effect = AffMaybe<Runtime, int>(async _ => await (++counter).AsValueTask());
        var result = await effect.Repeat(TestSchedule()).Run(Runtime.New());
        counter.Should().Be(6);
        result.Case.Should().Be(6);
    }

    [Fact]
    public static async Task RetryTest1()
    {
        var counter = 0;
        var effect = AffMaybe<int>(
            async () =>
            {
                await (++counter).AsValueTask();
                return Error.New("Failed");
            });
        var result = await effect.Retry(TestSchedule()).Run();
        counter.Should().Be(6);
        result.Case.Should().Be(Error.New("Failed"));
    }

    [Fact]
    public static async Task RetryTest2()
    {
        var counter = 0;
        var effect = AffMaybe<Runtime, int>(
            async _ =>
            {
                await (++counter).AsValueTask();
                return Error.New("Failed");
            });
        var result = await effect.Retry(TestSchedule()).Run(Runtime.New());
        counter.Should().Be(6);
        result.Case.Should().Be(Error.New("Failed"));
    }

    [Fact]
    public static async Task RepeatWhileTest1()
    {
        var counter = 0;
        var effect = AffMaybe<int>(async () => await (++counter).AsValueTask());
        var result = await effect.RepeatWhile(TestSchedule(), static i => i < 3).Run();
        counter.Should().Be(3);
        result.Case.Should().Be(3);
    }

    [Fact]
    public static async Task RepeatWhileTest2()
    {
        var counter = 0;
        var effect = AffMaybe<Runtime, int>(async _ => await (++counter).AsValueTask());
        var result = await effect.RepeatWhile(TestSchedule(), static i => i < 3).Run(Runtime.New());
        counter.Should().Be(3);
        result.Case.Should().Be(3);
    }

    [Fact]
    public static async Task RetryWhileTest1()
    {
        var counter = 0;
        var effect = AffMaybe<int>(
            async () =>
            {
                await (++counter).AsValueTask();
                return Error.New(counter.ToString());
            });
        var result = await effect.RetryWhile(TestSchedule(), static e => (int)parseInt(e.Message) < 3).Run();
        counter.Should().Be(3);
        result.Case.Should().Be(Error.New("3"));
    }

    [Fact]
    public static async Task RetryWhileTest2()
    {
        var counter = 0;
        var effect = AffMaybe<Runtime, int>(
            async _ =>
            {
                await (++counter).AsValueTask();
                return Error.New(counter.ToString());
            });
        var result = await effect.RetryWhile(TestSchedule(), static e => (int)parseInt(e.Message) < 3)
            .Run(Runtime.New());
        counter.Should().Be(3);
        result.Case.Should().Be(Error.New("3"));
    }

    [Fact]
    public static async Task RepeatUntilTest1()
    {
        var counter = 0;
        var effect = AffMaybe<int>(async () => await (++counter).AsValueTask());
        var result = await effect.RepeatUntil(static i => i == 10).Run();
        counter.Should().Be(10);
        result.Case.Should().Be(10);
    }

    [Fact]
    public static async Task RepeatUntilTest2()
    {
        var counter = 0;
        var effect = AffMaybe<Runtime, int>(async _ => await (++counter).AsValueTask());
        var result = await effect.RepeatUntil(static i => i == 10).Run(Runtime.New());
        counter.Should().Be(10);
        result.Case.Should().Be(10);
    }

    [Fact]
    public static async Task RetryUntilTest1()
    {
        var counter = 0;
        var effect = AffMaybe<int>(
            async () =>
            {
                await (++counter).AsValueTask();
                return Error.New(counter.ToString());
            });
        var result = await effect.RetryUntil(static e => (int)parseInt(e.Message) == 10).Run();
        counter.Should().Be(10);
        result.Case.Should().Be(Error.New("10"));
    }

    [Fact]
    public static async Task RetryUntilTest2()
    {
        var counter = 0;
        var effect = AffMaybe<Runtime, int>(
            async _ =>
            {
                await (++counter).AsValueTask();
                return Error.New(counter.ToString());
            });
        var result = await effect.RetryUntil(static e => (int)parseInt(e.Message) == 10).Run(Runtime.New());
        counter.Should().Be(10);
        result.Case.Should().Be(Error.New("10"));
    }

    [Fact]
    public static async Task FoldTest1()
    {
        var counter = 0;
        var effect = AffMaybe<int>(async () => await (++counter).AsValueTask());
        var result = (await effect.Fold(TestSchedule(), 1, (i, j) => i + j).Run()).ThrowIfFail();
        counter.Should().Be(6);
        result.Should().Be(22);
    }

    [Fact]
    public static async Task FoldTest2()
    {
        var counter = 0;
        var effect = AffMaybe<Runtime, int>(async _ => await (++counter).AsValueTask());
        var result = (await effect.Fold(TestSchedule(), 1, (i, j) => i + j).Run(Runtime.New())).ThrowIfFail();
        counter.Should().Be(6);
        result.Should().Be(22);
    }

    [Fact]
    public static async Task FoldWhileTest1()
    {
        var counter = 0;
        var effect = AffMaybe<int>(async () => await (++counter).AsValueTask());
        var result = (await effect.FoldWhile(TestSchedule(), 1, (i, j) => i + j, i => i < 3).Run()).ThrowIfFail();
        counter.Should().Be(3);
        result.Should().Be(7);
    }

    [Fact]
    public static async Task FoldWhileTest2()
    {
        var counter = 0;
        var effect = AffMaybe<Runtime, int>(async _ => await (++counter).AsValueTask());
        var result = (await effect.FoldWhile(TestSchedule(), 1, (i, j) => i + j, i => i < 3).Run(Runtime.New()))
            .ThrowIfFail();
        counter.Should().Be(3);
        result.Should().Be(7);
    }

    [Fact]
    public static async Task FoldUntilTest1()
    {
        var counter = 0;
        var effect = AffMaybe<int>(async () => await (++counter).AsValueTask());
        var result = (await effect.FoldUntil(TestSchedule(), 1, (i, j) => i + j, i => i > 4).Run()).ThrowIfFail();
        counter.Should().Be(5);
        result.Should().Be(16);
    }

    [Fact]
    public static async Task FoldUntilTest2()
    {
        var counter = 0;
        var effect = AffMaybe<Runtime, int>(async _ => await (++counter).AsValueTask());
        var result = (await effect.FoldUntil(TestSchedule(), 1, (i, j) => i + j, i => i > 4).Run(Runtime.New()))
            .ThrowIfFail();
        counter.Should().Be(5);
        result.Should().Be(16);
    }

    [Fact]
    public static async Task CancelTest()
    {
        var counter = 0;
        var cts = new CancellationTokenSource();
        var runtime = Runtime.New(cts);
        var effect = AffMaybe<Runtime, int>(async _ => await (++counter).AsValueTask()).Repeat(Schedule.Forever);
        cts.Cancel();
        var result = await effect.Run(runtime);
        counter.Should().Be(1);
        result.IsSucc.Should().BeTrue();
        result.Case.Should().Be(1);
    }
    
    [Fact(DisplayName = "Schedule Run against Aff<T> should not capture state")]
    public static async Task ShouldNotCaptureState1Test()
    {
        var content = Encoding.ASCII.GetBytes("test\0test\0test\0");
        var memStream = new MemoryStream(100);
        memStream.Write(content, 0, content.Length);
        memStream.Seek(0, SeekOrigin.Begin);

        Eff<Unit> AddToBuffer(ICollection<string> buffer, string value) =>
            Eff(() => { buffer.Add(value); return unit; });

        Aff<Unit> CreateEffect(ICollection<string> buffer) =>
            repeat(
                from ln in (
                    from data in Aff(() => memStream.ReadByte().AsValueTask())
                    from _ in guard(data != -1, Errors.Cancelled)
                    select data).FoldUntil(string.Empty, (s, ch) => s + (char)ch, ch => ch == '\0')
                from _0 in AddToBuffer(buffer,ln)
                select unit)
            | @catch(exception => AddToBuffer(buffer,exception.Message));

        var buffer = new List<string>();
        var effect = CreateEffect(buffer);

        await effect.RunUnit();

        buffer.Should().Equal("test\0", "test\0", "test\0", "cancelled");
    }

    [Fact(DisplayName = "Schedule Run against Aff<RT,T> should not capture state")]
    public static async Task ShouldNotCaptureState2Test()
    {
        var content = Encoding.ASCII.GetBytes("test\0test\0test\0");
        var memStream = new MemoryStream(100);
        memStream.Write(content, 0, content.Length);
        memStream.Seek(0, SeekOrigin.Begin);

        Aff<RT, Unit> CreateEffect<RT>() where RT : struct, HasConsole<RT> =>
            repeat(
                from ln in (
                    from data in Aff(() => memStream.ReadByte().AsValueTask())
                    from _ in guard(data != -1, Errors.Cancelled)
                    select data).FoldUntil(string.Empty, (s, ch) => s + (char)ch, ch => ch == '\0')
                from _0 in Console<RT>.writeLine(ln)
                select unit)
            | @catch(exception => Console<RT>.writeLine(exception.Message));

        var runtime = Runtime.New();
        var effect = CreateEffect<Runtime>();

        await effect.RunUnit(runtime);

        runtime.Env.Console.Should().Equal("test\0", "test\0", "test\0", "cancelled");
    }
}
