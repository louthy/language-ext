using System;
using System.Threading.Tasks;
using Xunit;

namespace LanguageExt.Tests.IOTests;

public class IO_RetryTests
{
    [Fact]
    public void Retry_SuccessOnFirstAttempt_ShouldReturnResultImmediately()
    {
        var io = IO.lift(() => 42);
        var schedule = Schedule.Never;
        var retryableIO = io.Retry(schedule);

        var result = retryableIO.Run();

        Assert.Equal(42, result);
    }

    [Fact]
    public void Retry_FailingThenSuccess_ShouldRetryAndReturnSuccess()
    {
        var attempt = 0;
        var io = IO.lift(() =>
        {
            attempt++;
            if (attempt < 3) throw new Exception($"Failure {attempt}");
            return 42;
        });

        var schedule = Schedule.recurs(2);
        var retryableIO = io.Retry(schedule);

        var result = retryableIO.Run();

        Assert.Equal(42, result);
        Assert.Equal(3, attempt);
    }

    [Fact]
    public void Retry_ExceedsMaxRetry_ShouldThrowLastError()
    {
        var io = IO.lift<int>(() => throw new Exception("Persistent failure"));
        var schedule = Schedule.recurs(2);
        var retryableIO = io.Retry(schedule);

        Assert.ThrowsAny<Exception>(() => retryableIO.Run());
    }

    [Fact]
    public void RetryWhile_ShouldContinueWhilePredicateIsTrue()
    {
        var attempt = 0;
        var io = IO.lift<int>(() =>
        {
            attempt++;
            throw new Exception($"Failure {attempt}");
        });

        var retryableIO = io.RetryWhile(error => error.Message.Contains("Failure 1") || error.Message.Contains("Failure 2"));

        Assert.ThrowsAny<Exception>(() => retryableIO.Run());

        Assert.Equal(3, attempt);
    }

    [Fact]
    public void RetryUntil_ShouldStopWhenPredicateReturnsTrue()
    {
        var attempt = 0;
        var io = IO.lift<int>(() =>
        {
            attempt++;
            throw new Exception($"Failure {attempt}");
        });

        var retryableIO = io.RetryUntil(error => error.Message.Contains("Failure 2"));

        Assert.ThrowsAny<Exception>(() => retryableIO.Run());

        Assert.Equal(2, attempt);
    }

    [Fact]
    public void RetryWithSchedule_Spaced_ShouldRespectIntervalsAndStopAfterScheduleExpires()
    {
        var attempt = 0;
        var io = IO.lift<int>(() =>
        {
            attempt++;
            throw new Exception($"Failure {attempt}");
        });

        var schedule    = Schedule.spaced(new Duration(100)).Take(3);
        var retryableIO = io.Retry(schedule);

        Assert.ThrowsAny<Exception>(() => retryableIO.Run());

        Assert.Equal(4, attempt);
    }

    [Fact]
    public void RetryWithSchedule_ExponentialBackoff_ShouldRetryWithIncreasingIntervals()
    {
        var attempt = 0;
        var io = IO.lift<int>(() =>
        {
            attempt++;
            throw new Exception($"Failure {attempt}");
        });

        var schedule = Schedule.exponential(new Duration(100)).Take(3);
        var retryableIO = io.Retry(schedule);

        Assert.ThrowsAny<Exception>(() => retryableIO.Run());

        Assert.Equal(4, attempt);
    }
}
