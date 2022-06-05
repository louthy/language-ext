#nullable enable

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.ScheduleTest;

public class ScheduleChangesTests
{
    sealed class OldSchedule : IEnumerable<Duration>
    {
        public readonly Option<int> Repeats;
        public readonly Option<int> Spacing;
        internal readonly Func<int, int, int> BackOff;

        OldSchedule(Option<int> repeats, Option<int> spacing, Func<int, int, int> backOff)
        {
            Repeats = repeats;
            Spacing = spacing;
            BackOff = backOff;
        }

        public static readonly OldSchedule Once = new(1, None, static (_, x) => x);

        public static readonly OldSchedule Forever = new(None, None, static (_, x) => x);

        public static OldSchedule Recurs(int repetitions) =>
            new(repetitions, None, static (_, x) => x);

        public static OldSchedule Spaced(int spacingMilliseconds) =>
            new(None, spacingMilliseconds, (_, _) => spacingMilliseconds);

        public static OldSchedule Exponential(int spacingMilliseconds) =>
            new(None, spacingMilliseconds, static (_, x) => x * 2);

        public static OldSchedule Fibonacci(int spacingMilliseconds) =>
            new(None, spacingMilliseconds, static (x, y) => x + y);

        public IEnumerator<Duration> GetEnumerator()
        {
            var spacing = Spacing.Select(x => new Duration(x)).IfNone(() => Duration.Zero);
            var repeats = Repeats.IfNone(() => 0);
            for (var i = 0; Repeats.IsNone || i < repeats; i++)
            {
                yield return spacing;
                var currentSpacing = spacing;
                spacing = BackOff((int)currentSpacing, (int)spacing);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Fact]
    public static void OnceTest()
    {
        var result1 = Schedule.Once;
        var result2 = OldSchedule.Once;
        result1.Should().Equal(result2);
    }

    [Fact]
    public static void ForeverTest()
    {
        var result1 = Schedule.Forever.Take(5);
        var result2 = OldSchedule.Forever.Take(5);
        result1.Should().Equal(result2);
    }

    [Fact]
    public static void SpacedTest()
    {
        var result1 = Schedule.Spaced(100).Take(5);
        var result2 = OldSchedule.Spaced(100).Take(5);
        result1.Should().Equal(result2);
    }

    [Fact]
    public static void RecursTest()
    {
        var result1 = Schedule.Recurs(5);
        var result2 = OldSchedule.Recurs(5);
        result1.Should().Equal(result2);
    }

    [Fact]
    public static void ExponentialTest()
    {
        var result1 = Schedule.Exponential(100).Take(5);
        var result2 = OldSchedule.Exponential(100).Take(5);
        result1.Should().Equal(result2);
    }

    [Fact]
    public static void FibonacciTest()
    {
        var result1 = Schedule.Fibonacci(100).Take(5).ToSeq();
        var result2 = OldSchedule.Fibonacci(100).Take(5).ToSeq();
        // this is breaking here
        result1.Should().NotEqual(result2);
        // the existing fibonacci schedule is actually a exponential schedule with a factor of 2
        var result3 = Schedule.Exponential(100).Take(5);
        result3.Should().Equal(result2);
    }

    [Fact]
    public static void OldFibonacciAndOldExponentialAreTheSame()
    {
        var result1 = OldSchedule.Fibonacci(100).Take(6).ToSeq();
        var result2 = OldSchedule.Exponential(100).Take(6).ToSeq();
        result1.Should().Equal(result2);
        var result3 = OldSchedule.Fibonacci(150).Take(6).ToSeq();
        var result4 = OldSchedule.Exponential(150).Take(6).ToSeq();
        result3.Should().Equal(result4);
    }
}
