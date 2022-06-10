#nullable enable

using System;
using System.Diagnostics.Contracts;
using System.Linq;
using FluentAssertions;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.ScheduleTest
{
    public sealed class ScheduleTests
    {
        [Fact]
        public static void ForeverTest()
        {
            var result = Schedule.Forever;
            result
                .Run()
                .Take(10)
                .Should()
                .HaveCount(10)
                .And
                .OnlyContain(x => x == Duration.Zero);
        }

        [Fact]
        public static void NeverTest()
        {
            var result = Schedule.Never;
            result
                .Run()
                .Should()
                .BeEmpty();
        }

        [Fact]
        public static void OnceTest()
        {
            var result = Schedule.Once;
            result
                .Run()
                .Should()
                .ContainSingle(x => x == Duration.Zero);
        }

        [Fact]
        public static void FromDurationsTest()
        {
            var result = Schedule.TimeSeries(1 * sec, 2 * sec, 3 * sec);
            result
                .Run()
                .Should()
                .Equal(1 * sec, 2 * sec, 3 * sec);
        }

        [Fact]
        public static void FromDurationsTest2()
        {
            var result = Schedule.TimeSeries(
                Seq(1, 2, 3, 4, 5)
                    .Filter(x => x % 2 == 0)
                    .Map<Duration>(x => x * seconds));
            result
                .Run()
                .Should()
                .Equal(2 * sec, 4 * sec);
        }

        [Fact]
        public static void RecursTest()
        {
            var results = Schedule.recurs(5) | Schedule.Forever;
            results
                .Run()
                .Should()
                .HaveCount(5)
                .And
                .Contain(x => x == Duration.Zero);
        }

        [Fact]
        public static void SpacedTest()
        {
            var results = Schedule.spaced(5 * sec);
            results
                .Run()
                .Take(5)
                .Should()
                .HaveCount(5)
                .And
                .OnlyContain(x => x == 5 * sec);
        }

        [Fact]
        public static void LinearTest()
        {
            var results = Schedule.linear(1 * sec);
            results
                .Run()
                .Take(5)
                .Should()
                .Equal(1 * sec, 2 * sec, 3 * sec, 4 * sec, 5 * sec);
        }

        [Fact]
        public static void LinearTest2()
        {
            var results = Schedule.linear(100 * ms, 2);
            results
                .Run()
                .Take(5)
                .Should()
                .Equal(100, 300, 500, 700, 900);
        }

        [Fact]
        public static void ExponentialTest()
        {
            var results = Schedule.exponential(1 * sec);
            results
                .Run()
                .Take(5)
                .Should()
                .Equal(1 * sec, 2 * sec, 4 * sec, 8 * sec, 16 * sec);
        }

        [Fact]
        public static void ExponentialTest2()
        {
            var results = Schedule.exponential(1 * sec, 3);
            results
                .Run()
                .Take(5)
                .Should()
                .Equal(1 * sec, 3 * sec, 9 * sec, 27 * sec, 81 * sec);
        }

        [Fact]
        public static void FibonacciTest()
        {
            var results = Schedule.fibonacci(1 * sec);
            results
                .Run()
                .Take(6)
                .Should()
                .Equal(1 * sec, 1 * sec, 2 * sec, 3 * sec, 5 * sec, 8 * sec);
        }

        [Fact]
        public static void NoDelayOnFirstTest()
        {
            var transformer = Schedule.NoDelayOnFirst;
            var results = transformer.Apply(Schedule.spaced(10 * sec));
            results
                .Run()
                .Take(5)
                .Should()
                .Equal(0 * sec, 10 * sec, 10 * sec, 10 * sec, 10 * sec);
        }

        [Fact]
        public static void MaxDelayTest()
        {
            var transformer = Schedule.maxDelay(25 * sec);
            var results = transformer.Apply(Schedule.linear(10 * sec));
            results
                .Run()
                .Take(5)
                .Max()
                .Should().Be(25 * sec);
        }

        [Fact]
        public static void MaxCumulativeDelayTest()
        {
            var transformer = Schedule.maxCumulativeDelay(40 * sec);
            var results = transformer.Apply(Schedule.linear(10 * sec));
            results
                .Run()
                .ToSeq()
                .Should()
                .HaveCount(3)
                .And
                .Subject.Max()
                .Should().Be(30 * sec);
        }

        [Fact]
        public static void UnionTest()
        {
            var results = Schedule.spaced(5 * sec).Union(Schedule.exponential(1 * sec));
            results
                .Run()
                .Take(5)
                .Should()
                .Equal(1 * sec, 2 * sec, 4 * sec, 5 * sec, 5 * sec);
        }

        [Fact]
        public static void IntersectTest()
        {
            var results = Schedule.spaced(5 * sec).Intersect(Schedule.exponential(1 * sec));
            results
                .Run()
                .Take(5)
                .Should()
                .Equal(5 * sec, 5 * sec, 5 * sec, 8 * sec, 16 * sec);
        }

        [Fact]
        public static void AppendTest()
        {
            var results =
                Schedule.TimeSeries(1 * sec, 2 * sec, 3 * sec) +
                Schedule.TimeSeries(4 * sec, 5 * sec, 6 * sec);
            results
                .Run()
                .Should()
                .Equal(1 * sec, 2 * sec, 3 * sec, 4 * sec, 5 * sec, 6 * sec);
        }

        [Pure]
        static Seq<DateTime> FromDuration(Duration duration)
        {
            var now = DateTime.Now;
            return Range(0, (int)((TimeSpan)duration).TotalSeconds)
                .Select(i => now + TimeSpan.FromSeconds(i))
                .ToSeq();
        }

        [Pure]
        static Seq<DateTime> FromDurations(Seq<Duration> durations) =>
            durations.Fold(Seq1(DateTime.Now), (times, duration) =>
            {
                var last = times.Head();
                return times.Add(last + (TimeSpan)duration);
            });

        [Pure]
        static Func<DateTime> FromDates(Seq<DateTime> dates) =>
            () =>
            {
                var date = dates.HeadOrNone().IfNone(() => DateTime.Now);
                dates = dates.Tail;
                return date;
            };

        [Fact]
        public static void UpToTest()
        {
            var results = Schedule.upto(5 * sec, FromDates(FromDuration(2 * min)));
            results
                .Run()
                .Should()
                .Equal(0, 0, 0, 0);
        }

        [Fact]
        public static void FixedTest()
        {
            var results = Schedule.fixedInterval(5 * sec, FromDates(FromDurations(Seq<Duration>(
                6 * sec,
                1 * sec,
                4 * sec
            ))));
            results
                .Run()
                .Take(3)
                .Should()
                .Equal(0, 4 * sec, 1 * sec);
        }

        [Fact]
        public static void WindowedTest()
        {
            var results = Schedule.windowed(5 * sec, FromDates(FromDurations(Seq<Duration>(
                6 * sec,
                1 * sec,
                7 * sec
            ))));
            results
                .Run()
                .Take(3)
                .Should()
                .Equal(4 * sec, 4 * sec, 3 * sec);
        }

        [Fact]
        public static void SecondOfMinuteTest()
        {
            var results = Schedule.secondOfMinute(3, FromDates(Seq(
                new DateTime(2022, 1, 1, 1, 1, 26),
                new DateTime(2022, 1, 1, 1, 1, 1),
                new DateTime(2022, 1, 1, 1, 1, 47)
            )));
            results
                .Run()
                .Take(3)
                .Should()
                .Equal(37 * sec, 2 * sec, 16 * sec);
        }

        [Fact]
        public static void MinuteOfHourTest()
        {
            var results = Schedule.minuteOfHour(3, FromDates(Seq(
                new DateTime(2022, 1, 1, 1, 26, 0),
                new DateTime(2022, 1, 1, 1, 1, 0),
                new DateTime(2022, 1, 1, 1, 47, 0)
            )));
            results
                .Run()
                .Take(3)
                .Should()
                .Equal(37 * min, 2 * min, 16 * min);
        }

        [Fact]
        public static void HourOfDayTest()
        {
            var results = Schedule.hourOfDay(3, FromDates(Seq(
                new DateTime(2022, 1, 1, 1, 0, 0),
                new DateTime(2022, 1, 1, 4, 0, 0),
                new DateTime(2022, 1, 1, 6, 0, 0),
                new DateTime(2022, 1, 1, 3, 0, 0)
            )));
            results
                .Run()
                .Take(4)
                .Should()
                .Equal(2 * hours, 23 * hours, 21 * hour, 24 * hours);
        }

        [Fact]
        public static void DayOfWeekTest()
        {
            var results = Schedule.dayOfWeek(DayOfWeek.Wednesday, FromDates(Seq(
                new DateTime(2022, 1, 1, 0, 0, 0), // Saturday
                new DateTime(2022, 1, 3, 0, 0, 0), // Monday
                new DateTime(2022, 1, 7, 0, 0, 0), // Friday
                new DateTime(2022, 1, 5, 0, 0, 0) // Wednesday
            )));
            results
                .Run()
                .Take(4)
                .Should()
                .Equal(4 * days, 2 * days, 5 * days, 7 * days);
        }

        const int Seed = 98192732;

        [Fact]
        public static void JitterTest1()
        {
            var noJitter = (
                Schedule.linear(10 * seconds)
                & Schedule.recurs(5)).Run().ToSeq();
            var withJitter = (
                Schedule.linear(10 * seconds)
                & Schedule.recurs(5)
                & Schedule.jitter(1 * ms, 10 * ms)).Run().ToSeq();
            withJitter.Should()
                .HaveCount(5)
                .And
                .Subject.Zip(noJitter)
                .Should()
                .Contain(x => x.Item1 > x.Item2)
                .And
                .Contain(x => x.Item1 - x.Item2 <= 100);
        }

        [Fact]
        public static void JitterTest2()
        {
            var noJitter = (
                Schedule.linear(10 * seconds)
                & Schedule.recurs(5)).Run().ToSeq();
            var withJitter = (
                Schedule.linear(10 * seconds)
                & Schedule.recurs(5)
                & Schedule.jitter(1.5)).Run().ToSeq();
            withJitter.Should()
                .HaveCount(5)
                .And
                .Subject.Zip(noJitter)
                .Should()
                .Contain(x => x.Item1 > x.Item2)
                .And
                .Contain(x => x.Item1 - x.Item2 <= x.Item2 * 1.5);
        }

        [Fact]
        public static void DecorrelatedTest()
        {
            var schedule = Schedule.linear(10 * sec) | Schedule.decorrelate(seed: Seed);
            var result = schedule.Take(5).Run().ToSeq();
            result.Zip(result.Skip(1))
                .Should()
                .Contain(x => x.Left > x.Right);
        }

        [Fact]
        public static void ResetAfterTest()
        {
            var results =
                Schedule.linear(10 * sec)
                | Schedule.resetAfter(25 * sec);
            results
                .Run()
                .Take(4)
                .Should()
                .Equal(10 * sec, 20 * sec, 10 * sec, 20 * sec);
        }

        [Fact]
        public static void RepeatForeverTest()
        {
            var results = Schedule.TimeSeries(1 * sec, 5 * sec, 20 * sec) | Schedule.RepeatForever;
            
            results
                .Run()
                .Take(12)
                .Should()
                .Equal(1 * sec, 5 * sec, 20 * sec,
                    1 * sec, 5 * sec, 20 * sec,
                    1 * sec, 5 * sec, 20 * sec,
                    1 * sec, 5 * sec, 20 * sec);
        }

        [Fact]
        public static void RepeatTest()
        {
            var results = Schedule.TimeSeries(1 * sec, 5 * sec, 20 * sec) | Schedule.repeat(3);
            
            results
                .Run()
                .Should()
                .HaveCount(9)
                .And
                .Equal(1 * sec, 5 * sec, 20 * sec,
                    1 * sec, 5 * sec, 20 * sec,
                    1 * sec, 5 * sec, 20 * sec);
        }

        [Fact]
        public static void IntersperseTest()
        {
            var results = Schedule.TimeSeries(1 * sec, 5 * sec, 20 * sec) | Schedule.intersperse(2 * sec);
            
            results
                .Run()
                .Should()
                .HaveCount(6)
                .And
                .Equal(1 * sec, 2 * sec, 5 * sec, 2 * sec, 20 * sec, 2 * sec);
        }

        [Fact]
        public static void InterleaveTest()
        {
            var schedule1 = Schedule.TimeSeries(1 * sec, 5 * sec, 20 * sec);
            var schedule2 = Schedule.TimeSeries(2 * sec, 7 * sec, 25 * sec);

            var results = schedule1.Interleave(schedule2);
            results
                .Run()
                .Should()
                .HaveCount(6)
                .And
                .Equal(1 * sec, 2 * sec, 5 * sec, 7 * sec, 20 * sec, 25 * sec);
        }

        [Fact]
        public static void RandomDurationTest()
        {
            var schedule1 =
                Schedule.linear(Duration.random(10 * ms, 50 * ms))
                    | Schedule.decorrelate()
                    | Schedule.recurs(5);
            var schedule2 =
                Schedule.linear(Duration.random(10 * ms, 50 * ms))
                    | Schedule.decorrelate()
                    | Schedule.recurs(5);
            var schedule3 =
                Schedule.linear(Duration.random(10 * ms, 50 * ms))
                    | Schedule.decorrelate()
                    | Schedule.recurs(5);

            schedule1.Run().Should().HaveCount(5);
            schedule2.Run().Should().HaveCount(5);
            schedule3.Run().Should().HaveCount(5);
        }

        [Fact]
        public static void MapTest()
        {
            var schedule = Schedule.linear(1 * ms).Map((x, i) => x % 2 == 0 ? x + i : x - i).Take(4);
            schedule.Run().Should().Equal(1 * ms, 3 * ms, 1 * ms, 7 * ms);
        }

        [Fact]
        public static void FilterTest()
        {
            var schedule = Schedule.linear(1 * ms).Filter(x => x % 2 == 0).Take(4);
            schedule.Run().Should().Equal(2 * ms, 4 * ms, 6 * ms, 8 * ms);
        }

        [Fact]
        public static void BindTest1()
        {
            var schedule =
                Schedule
                    .linear(1 * ms)
                    .Filter(x => x % 2 == 0)
                    .Take(2)
                    .Bind(even =>
                        Schedule
                            .linear(1 * ms)
                            .Filter(x => x % 2 != 0)
                            .Take(2)
                            .Bind(odd => Schedule.TimeSeries(even, odd)));

            schedule.Run().Should().Equal(
                2 * ms, 1 * ms,
                2 * ms, 3 * ms,
                4 * ms, 1 * ms,
                4 * ms, 3 * ms);
        }

        [Fact]
        public static void BindTest2()
        {
            var schedule =
                from even in Schedule
                    .linear(1 * ms)
                    .Filter(x => x % 2 == 0)
                    .Take(2)
                from odd in Schedule
                    .linear(1 * ms)
                    .Filter(x => x % 2 != 0)
                    .Take(2)
                select Math.Pow(even, odd);

            schedule.Run().Should().Equal(2 * ms, 8 * ms, 4 * ms, 64 * ms);
        }
    }
}
