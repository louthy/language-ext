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

        OldSchedule Union(OldSchedule schedule) =>
            new(
                Repeats.IsNone
                    ? schedule.Repeats
                    : schedule.Repeats.IsNone
                        ? Repeats
                        : Math.Min((int)Repeats, (int)schedule.Repeats),
                Spacing.IsNone
                    ? schedule.Spacing
                    : schedule.Spacing.IsNone
                        ? Spacing
                        : Math.Min((int)Spacing, (int)schedule.Spacing),
                (x, y) =>
                    Spacing.IsSome && schedule.Spacing.IsSome
                        ? Math.Min(BackOff(x, y), schedule.BackOff(x, y))
                        : Spacing.IsSome
                            ? BackOff(x, y)
                            : schedule.Spacing.IsSome
                                ? schedule.BackOff(x, y)
                                : y);

        OldSchedule Intersect(OldSchedule schedule) =>
            new(
                Repeats.IsNone
                    ? schedule.Repeats
                    : schedule.Repeats.IsNone
                        ? Repeats
                        : Math.Max((int)Repeats, (int)schedule.Repeats),
                Spacing.IsNone
                    ? schedule.Spacing
                    : schedule.Spacing.IsNone
                        ? Spacing
                        : Math.Max((int)Spacing, (int)schedule.Spacing),
                (x, y) =>
                    Spacing.IsSome && schedule.Spacing.IsSome
                        ? Math.Max(BackOff(x, y), schedule.BackOff(x, y))
                        : Spacing.IsSome
                            ? BackOff(x, y)
                            : schedule.Spacing.IsSome
                                ? schedule.BackOff(x, y)
                                : y);

        public static OldSchedule operator |(OldSchedule x, OldSchedule y) =>
            x.Union(y);

        public static OldSchedule operator &(OldSchedule x, OldSchedule y) =>
            x.Intersect(y);

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
            var lastSpacing = Duration.Zero;
            var repeats = Repeats.IfNone(() => 0);
            for (var i = 0; Repeats.IsNone || i < repeats; i++)
            {
                var currentSpacing = spacing;
                yield return currentSpacing;
                spacing = BackOff((int)lastSpacing, (int)currentSpacing);
                lastSpacing = currentSpacing;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    [Fact]
    public static void OnceTest()
    {
        var result1 = Schedule.Once;
        var result2 = OldSchedule.Once;
        result1.Run().Should().Equal(result2);
    }

    [Fact]
    public static void ForeverTest()
    {
        var result1 = Schedule.Forever.Take(5);
        var result2 = OldSchedule.Forever.Take(5);
        result1.Run().Should().Equal(result2);
    }

    [Fact]
    public static void SpacedTest()
    {
        var result1 = Schedule.spaced(100).Take(5);
        var result2 = OldSchedule.Spaced(100).Take(5);
        result1.Run().Should().Equal(result2);
    }

    [Fact]
    public static void ExponentialTest()
    {
        var result1 = Schedule.exponential(100).Take(5);
        var result2 = OldSchedule.Exponential(100).Take(5);
        result1.Run().Should().Equal(result2);
    }

    [Fact]
    public static void FibonacciTest()
    {
        var result1 = Schedule.fibonacci(100).Take(5).Run().ToSeq();
        var result2 = OldSchedule.Fibonacci(100).Take(5).ToSeq();
        result1.Should().Equal(result2);
    }

    [Fact]
    public static void UnionTest()
    {
        var result1 = Schedule.spaced(50) | Schedule.exponential(10) | Schedule.recurs(5);
        var result2 = OldSchedule.Spaced(50) | OldSchedule.Exponential(10) | OldSchedule.Recurs(5);
        result1.Run().Should().Equal(result2);
    }

    [Fact]
    public static void IntersectTest()
    {
        var result1 = Schedule.spaced(30) & Schedule.fibonacci(10) | Schedule.recurs(5);
        var result2 = OldSchedule.Spaced(30) & OldSchedule.Fibonacci(10) | OldSchedule.Recurs(5);

        // this is correct, fib schedule is 10,10,20,30,50 and the max of the spaced 30x5 is 30x4,50
        result1.Run().Should().Equal(30, 30, 30, 30, 50);
        // this is not correct, the schedule is influenced by the new max starting value introduced by the spaced 30, they are not independent.
        result2.Should().Equal(30, 30, 60, 90, 150);
    }
}
