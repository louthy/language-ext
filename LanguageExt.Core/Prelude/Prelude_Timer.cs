using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Execute a function after a specified delay
        /// </summary>
        /// <param name="f">Function to execute</param>
        /// <param name="delayFor">Time span to delay for</param>
        /// <returns>IObservable T with the result</returns>
        public static IObservable<T> delay<T>(Func<T> f, TimeSpan delayFor) =>
            delayFor.TotalMilliseconds < 1
                ? Observable.Return(f()).Take(1)
                : Task.Delay(delayFor).ContinueWith(_ => f()).ToObservable().Take(1);

        /// <summary>
        /// Execute a function at a specific time
        /// </summary>
        /// <remarks>
        /// This will fail to be accurate across a Daylight Saving Time boundary
        /// </remarks>
        /// <param name="f">Function to execute</param>
        /// <param name="delayUntil">DateTime to wake up at.</param>
        /// <returns>IObservable T with the result</returns>
        public static IObservable<T> delay<T>(Func<T> f, DateTime delayUntil) =>
            delay(f, delayUntil.ToUniversalTime() - DateTime.UtcNow);

        /// <summary>
        /// Execute an action after a specified delay
        /// </summary>
        /// <param name="f">Action to execute</param>
        /// <param name="delayFor">Time span to delay for</param>
        public static Unit delay(Action f, TimeSpan delayFor)
        {
            if (delayFor.TotalMilliseconds < 1)
            {
                f();
            }
            else
            {
                Task.Delay(delayFor).ContinueWith(_ => f());
            }
            return unit;
        }

        /// <summary>
        /// Execute a function at a specific time
        /// </summary>
        /// <remarks>
        /// This will fail to be accurate across a Daylight Saving Time boundary
        /// </remarks>
        /// <param name="f">Action to execute</param>
        /// <param name="delayUntil">DateTime to wake up at.</param>
        public static Unit delay(Action f, DateTime delayUntil) =>
            delay(f, delayUntil.ToUniversalTime() - DateTime.UtcNow);
    }
}
