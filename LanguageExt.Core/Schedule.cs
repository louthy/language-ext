using System;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.ClassInstances.Pred;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Provides a mechanism for composing scheduled events
    /// </summary>
    /// <remarks>
    /// Used heavily by repeat, retry, and fold with the Aff and Eff types.  Use the static methods to create parts
    /// of schedulers and then union them using | or intersect them using &.  Union will take the minimum of the two
    /// schedulers, intersect will take the maximum. 
    /// </remarks>
    /// <example>
    /// This example creates a schedule that repeats 5 times, with an exponential delay between each stage, starting
    /// at 10 milliseconds:
    /// 
    ///     var s = Schedule.Recurs(5) | Schedule.Exponential(10)
    /// 
    /// </example>
    /// <example>
    /// This example creates a schedule that repeats 5 times, with an exponential delay between each stage, starting
    /// at 10 milliseconds and with a maximum delay of 2000 milliseconds:
    /// 
    ///     var s = Schedule.Recurs(5) | Schedule.Exponential(10) | Schedule.Spaced(2000)
    /// 
    /// </example>
    public class Schedule
    {
        public readonly Option<int> Repeats;
        public readonly Option<int> Spacing;
        internal readonly Func<int, int, int> BackOff;
        
        internal Schedule(Option<int> repeats, Option<int> spacing, Func<int, int, int> backOff)
        {
            this.Repeats = repeats;
            this.Spacing = spacing;
            this.BackOff = backOff;
        }

        public Schedule Union(Schedule schedule) =>
            new Schedule(
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
                (x, y) => Math.Min(BackOff(x, y), schedule.BackOff(x, y)));

        public Schedule Intersect(Schedule schedule) =>
            new Schedule(
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
                (x, y) => Math.Max(BackOff(x, y), schedule.BackOff(x, y)));

        public static Schedule operator |(Schedule x, Schedule y) =>
            x.Union(y);

        public static Schedule operator &(Schedule x, Schedule y) =>
            x.Intersect(y);
        
        /// <summary>
        /// A schedule that runs once
        /// </summary>
        public static readonly Schedule Once = new Schedule(0, None, static (_, x) => x);
        
        /// <summary>
        /// A schedule that recurs forever
        /// </summary>
        public static readonly Schedule Forever = new Schedule(None, None, static (_, x) => x);
        
        /// <summary>
        /// A schedule that only recurs the specified number of times
        /// </summary>
        public static Schedule Recurs(int repetitions) => new Schedule(repetitions - 1, None, static (_, x) => x);
        
        /// <summary>
        /// A schedule that recurs continuously, each repetition spaced by the specified duration
        /// </summary>
        public static Schedule Spaced(TimeSpan spacing) => new Schedule(None, spacing.Milliseconds, (_, _) => spacing.Milliseconds);
        
        /// <summary>
        /// A schedule that recurs continuously using an exponential backoff
        /// </summary>
        public static Schedule Exponential(TimeSpan spacing) => new Schedule(None, spacing.Milliseconds, static (_, x) => x * 2);
        
        /// <summary>
        /// A schedule that recurs continuously using an fibonacci based backoff
        /// </summary>
        public static Schedule Fibonacci(TimeSpan spacing) => new Schedule(None, spacing.Milliseconds, static (x, y) => x + y);
       
        /// <summary>
        /// A schedule that recurs continuously, each repetition spaced by the specified duration
        /// </summary>
        public static Schedule Spaced(int spacingMilliseconds) => new Schedule(None, spacingMilliseconds, (_, _) => spacingMilliseconds);
        
        /// <summary>
        /// A schedule that recurs continuously using an exponential backoff
        /// </summary>
        public static Schedule Exponential(int spacingMilliseconds) => new Schedule(None, spacingMilliseconds, static (_, x) => x * 2);
        
        /// <summary>
        /// A schedule that recurs continuously using an fibonacci based backoff
        /// </summary>
        public static Schedule Fibonacci(int spacingMilliseconds) => new Schedule(None, spacingMilliseconds, static (x, y) => x + y);
    }
}
