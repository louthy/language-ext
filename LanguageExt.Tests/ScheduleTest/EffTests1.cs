using Xunit;
using System.IO;
using System.Text;
using LanguageExt.Sys;
using FluentAssertions;
using LanguageExt.Common;
using LanguageExt.Sys.Test;
using LanguageExt.Sys.Traits;
using System.Collections.Generic;
using static LanguageExt.UnitsOfMeasure;

namespace LanguageExt.Tests.ScheduleTest;

public static class EffTests1
{
    static Schedule TestSchedule() => Schedule.fixedInterval(1 * ms) | Schedule.NoDelayOnFirst | Schedule.recurs(5);

    [Fact]
    public static void BailBeforeScheduleTest1()
    {
        const int counter = 0;
        var       effect  = FailEff<int>((Error)"Failed");
        var       result  = effect.RepeatIO(TestSchedule()).Run();
        counter.Should().Be(0);
        result.AssertFail(Error.New("Failed"));
    }

    [Fact]
    public static void BailBeforeScheduleTest2()
    {
        using var rt      = Runtime.New();
        const int counter = 0;
        var       effect  = FailEff<Runtime, int>((Error)"Failed");
        var       result  = effect.RepeatIO(TestSchedule()).Run(rt, EnvIO.New());
        counter.Should().Be(0);
        result.AssertFail(Error.New("Failed"));
    }

    [Fact]
    public static void RepeatTest1()
    {
        var counter = 0;
        var effect = liftEff(async () => await (++counter).AsValueTask());
        var result = effect.RepeatIO(TestSchedule()).Run();
        Assert.True(counter == 6);
        result.AssertSucc(6);
    }

    [Fact]
    public static void RepeatTest2()
    {
        using var rt      = Runtime.New();
        var       counter = 0;
        var       effect  = liftEff<Runtime, int>(async _ => await (++counter).AsValueTask());
        var       result  = effect.RepeatIO(TestSchedule()).Run(rt, EnvIO.New());
        Assert.True(counter == 6);
        result.AssertSucc(6);
    }

    [Fact]
    public static void RetryTest1()
    {
        var counter = 0;
        var effect = liftEff<int>(
            async () =>
            {
                await (++counter).AsValueTask();
                return Error.New("Failed");
            });
        var result = effect.RetryIO(TestSchedule()).Run();
        counter.Should().Be(6);
        result.AssertFail(Error.New("Failed"));
    }

    [Fact]
    public static void RetryTest2()
    {
        using var rt      = Runtime.New();
        var       counter = 0;
        var effect = liftEff<Runtime, int>(
            async _ =>
            {
                await (++counter).AsValueTask();
                return Error.New("Failed");
            });
        var result = effect.RetryIO(TestSchedule()).Run(rt, EnvIO.New());
        counter.Should().Be(6);
        result.AssertFail(Error.New("Failed"));
    }

    [Fact]
    public static void RepeatWhileTest1()
    {
        var counter = 0;
        var effect  = liftEff(async () => await (++counter).AsValueTask());
        var result  = effect.RepeatWhileIO(TestSchedule(), static i => i < 3).Run();
        counter.Should().Be(3);
        result.AssertSucc(3);
    }

    [Fact]
    public static void RepeatWhileTest2()
    {
        using var rt      = Runtime.New();
        var       counter = 0;
        var       effect  = liftEff<Runtime, int>(async _ => await (++counter).AsValueTask());
        var       result  = effect.RepeatWhileIO(TestSchedule(), static i => i < 3).Run(rt, EnvIO.New());
        counter.Should().Be(3);
        result.AssertSucc(3);
    }

    [Fact]
    public static void RetryWhileTest1()
    {
        var counter = 0;
        var effect = liftEff<int>(
            async () =>
            {
                await (++counter).AsValueTask();
                return Error.New(counter.ToString());
            });
        var result = effect.RetryWhileIO(TestSchedule(), static e => (int)parseInt(e.Message) < 3).Run();
        counter.Should().Be(3);
        result.AssertFail(Error.New("3"));
    }

    [Fact]
    public static void RetryWhileTest2()
    {
        using var rt      = Runtime.New();
        var       counter = 0;
        var effect = liftEff<Runtime, int>(
            async _ =>
            {
                await (++counter).AsValueTask();
                return Error.New(counter.ToString());
            });
        var result = effect.RetryWhileIO(TestSchedule(), static e => (int)parseInt(e.Message) < 3).Run(rt, EnvIO.New());
        counter.Should().Be(3);
        result.AssertFail(Error.New("3"));
    }

    [Fact]
    public static void RepeatUntilTest1()
    {
        var counter = 0;
        var effect  = liftEff(async () => await (++counter).AsValueTask());
        var result  = effect.RepeatUntilIO(static i => i == 10).Run();
        counter.Should().Be(10);
        result.AssertSucc(10);
    }

    [Fact]
    public static void RepeatUntilTest2()
    {
        using var rt      = Runtime.New();
        var       counter = 0;
        var       effect  = liftEff<Runtime, int>(async _ => await (++counter).AsValueTask());
        var       result  = effect.RepeatUntilIO(static i => i == 10).Run(rt, EnvIO.New());
        counter.Should().Be(10);
        result.AssertSucc(10);
    }

    [Fact]
    public static void RetryUntilTest1()
    {
        var counter = 0;
        var effect = liftEff<int>(
            async () =>
            {
                await (++counter).AsValueTask();
                return Error.New(counter.ToString());
            });
        var result = effect.RetryUntilIO(static e => (int)parseInt(e.Message) == 10).Run();
        counter.Should().Be(10);
        result.AssertFail(Error.New("10"));
    }

    [Fact]
    public static void RetryUntilTest2()
    {
        using var rt      = Runtime.New();
        var       counter = 0;
        var effect = liftEff<Runtime, int>(
            async _ =>
            {
                await (++counter).AsValueTask();
                return Error.New(counter.ToString());
            });
        var result = effect.RetryUntilIO(static e => (int)parseInt(e.Message) == 10).Run(rt, EnvIO.New());
        counter.Should().Be(10);
        result.AssertFail(Error.New("10"));
    }

    [Fact]
    public static void FoldTest1()
    {
        var counter = 0;
        var effect  = liftEff(async () => await (++counter).AsValueTask());
        var result  = effect.FoldIO(TestSchedule(), 1, (i, j) => i + j).Run();
        counter.Should().Be(6);
        result.AssertSucc(22);
    }

    [Fact]
    public static void FoldTest2()
    {
        using var rt      = Runtime.New();
        var       counter = 0;
        var       effect  = liftEff<Runtime, int>(async _ => await (++counter).AsValueTask());
        var       result  = effect.FoldIO(TestSchedule(), 1, (i, j) => i + j).Run(rt, EnvIO.New());
        counter.Should().Be(6);
        result.AssertSucc(22);
    }

    [Fact]
    public static void FoldWhileTest1()
    {
        var counter = 0;
        var effect  = liftEff(async () => await (++counter).AsValueTask());
        var result  = effect.FoldWhileIO(TestSchedule(), 1, (i, j) => i + j, valueIs: i => i < 3).Run();
        counter.Should().Be(3);
        result.AssertSucc(7);
    }

    [Fact]
    public static void FoldWhileTest2()
    {
        using var rt = Runtime.New();
        var counter = 0;
        var effect  = liftEff<Runtime, int>(async _ => await (++counter).AsValueTask());
        var result  = effect.FoldWhileIO(TestSchedule(), 1, (i, j) => i + j, valueIs: i => i < 3).Run(rt, EnvIO.New());
        counter.Should().Be(3);
        result.AssertSucc(7);
    }

    [Fact]
    public static void FoldUntilTest1()
    {
        var counter = 0;
        var effect  = liftEff(async () => await (++counter).AsValueTask());
        var result  = effect.FoldUntilIO(TestSchedule(), 1, (i, j) => i + j, valueIs: i => i > 4).Run();
        counter.Should().Be(5);
        result.AssertSucc(16);
    }

    [Fact]
    public static void FoldUntilTest2()
    {
        using var rt      = Runtime.New();
        var       counter = 0;
        var       effect  = liftEff<Runtime, int>(async _ => await (++counter).AsValueTask());
        var result = effect.FoldUntilIO(TestSchedule(), 1, (i, j) => i + j, valueIs: i => i > 4)
                           .Run(rt, EnvIO.New());
        
        counter.Should().Be(5);
        result.AssertSucc(16);
    }

    [Fact]
    public static void CancelTest()
    {
        using var rt = Runtime.New();
        var counter = 0;
        var envIO   = EnvIO.New();
        var effect  = liftEff<Runtime, int>(async _ => await (++counter).AsValueTask()).RepeatIO(Schedule.Forever);
        envIO.Source.Cancel();
        var result = effect.Run(rt, envIO);
        Assert.True(counter == 0);
        result.AssertFail(Errors.Cancelled);
    }
    
    [Fact(DisplayName = "Schedule Run against Aff<T> should not capture state")]
    public static void ShouldNotCaptureState1Test()
    {
        var content = Encoding.ASCII.GetBytes("test\0test\0test\0");
        var memStream = new MemoryStream(100);
        memStream.Write(content, 0, content.Length);
        memStream.Seek(0, SeekOrigin.Begin);

        Eff<Unit> AddToBuffer(ICollection<string> buffer, string value) =>
            lift(() => { buffer.Add(value); return unit; });

        Eff<Unit> CreateEffect(ICollection<string> buffer) =>
            repeatIO(from ln in (from data in liftEff(() => memStream.ReadByte().AsTask())
                                 from _ in guard(data != -1, Errors.Cancelled)
                                 select data)
                                .FoldUntilIO(string.Empty, (s, ch) => s + (char)ch, ch => ch == '\0')
                     from _0 in AddToBuffer(buffer,ln)
                     select unit).As()
            | @catch(error => AddToBuffer(buffer,error.Message));

        var buffer = new List<string>();
        var effect = CreateEffect(buffer);

        effect.Run(EnvIO.New()).Ignore();

        buffer.Should().Equal("test\0", "test\0", "test\0", "cancelled");
    }

    [Fact(DisplayName = "Schedule Run against Aff<RT,T> should not capture state")]
    public static void ShouldNotCaptureState2Test()
    {
        var content = Encoding.ASCII.GetBytes("test\0test\0test\0");
        var memStream = new MemoryStream(100);
        memStream.Write(content, 0, content.Length);
        memStream.Seek(0, SeekOrigin.Begin);

        Eff<RT, Unit> CreateEffect<RT>() where RT : Has<Eff<RT>, ConsoleIO> =>
            repeatIO(from ln in (from data in liftEff(() => memStream.ReadByte().AsTask())
                                 from _ in guard(data != -1, Errors.Cancelled)
                                 select data)
                                .FoldUntilIO(string.Empty, (s, ch) => s + (char)ch, ch => ch == '\0')
                                .As()
                     from _0 in Console<RT>.writeLine(ln)
                     select unit).As()
            | @catch(error => Console<RT>.writeLine(error.Message));

        using var runtime = Runtime.New();
        var effect = CreateEffect<Runtime>();

        effect.Run(runtime, EnvIO.New()).Ignore();

        runtime.Env.Console.Should().Equal("test\0", "test\0", "test\0", "cancelled");
    }
}
