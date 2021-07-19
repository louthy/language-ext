using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.Thunks;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Keeps retrying the computation  
        /// </summary>
        /// <param name="ma">Computation to retry</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<RT, A> retry<RT, A>(Aff<RT, A> ma) where RT : struct, HasCancel<RT> =>
            ScheduleAff<RT, A>.Retry(ma, Schedule.Forever);
        
        /// <summary>
        /// Keeps retrying the computation  
        /// </summary>
        /// <param name="ma">Computation to retry</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<A> retry<A>(Aff<A> ma) => 
            ScheduleAff<A>.Retry(ma, Schedule.Forever);

        /// <summary>
        /// Keeps retrying the computation, until the scheduler expires  
        /// </summary>
        /// <param name="ma">Computation to retry</param>
        /// <param name="schedule">Scheduler strategy for retrying</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<RT, A> retry<RT, A>(Schedule schedule, Aff<RT, A> ma) where RT : struct, HasCancel<RT> =>
            ScheduleAff<RT, A>.Retry(ma, schedule);
        
        /// <summary>
        /// Keeps retrying the computation, until the scheduler expires  
        /// </summary>
        /// <param name="ma">Computation to retry</param>
        /// <param name="schedule">Scheduler strategy for retrying</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<A> retry<A>(Schedule schedule, Aff<A> ma) => 
            ScheduleAff<A>.Retry(ma, schedule);
        
        
        /// <summary>
        /// Keeps retrying the computation until the predicate returns false
        /// </summary>
        /// <param name="ma">Computation to retry</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<RT, A> retryWhile<RT, A>(Aff<RT, A> ma, Func<Error, bool> predicate) where RT : struct, HasCancel<RT> =>
            ScheduleAff<RT, A>.RetryWhile(ma, Schedule.Forever, predicate);
        
        /// <summary>
        /// Keeps retrying the computation until the predicate returns false
        /// </summary>
        /// <param name="ma">Computation to retry</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<A> retryWhile<A>(Aff<A> ma, Func<Error, bool> predicate) => 
            ScheduleAff<A>.RetryWhile(ma, Schedule.Forever, predicate);

        /// <summary>
        /// Keeps retrying the computation, until the scheduler expires, or the predicate returns false  
        /// </summary>
        /// <param name="ma">Computation to retry</param>
        /// <param name="schedule">Scheduler strategy for retrying</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<RT, A> retryWhile<RT, A>(Schedule schedule, Aff<RT, A> ma, Func<Error, bool> predicate) where RT : struct, HasCancel<RT> =>
            ScheduleAff<RT, A>.RetryWhile(ma, schedule, predicate);
        
        /// <summary>
        /// Keeps retrying the computation, until the scheduler expires, or the predicate returns false
        /// </summary>
        /// <param name="ma">Computation to retry</param>
        /// <param name="schedule">Scheduler strategy for retrying</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<A> retryWhile<A>(Schedule schedule, Aff<A> ma, Func<Error, bool> predicate) => 
            ScheduleAff<A>.RetryWhile(ma, schedule, predicate);
        
        
        /// <summary>
        /// Keeps retrying the computation until the predicate returns true
        /// </summary>
        /// <param name="ma">Computation to retry</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<RT, A> retryUntil<RT, A>(Aff<RT, A> ma, Func<Error, bool> predicate) where RT : struct, HasCancel<RT> =>
            ScheduleAff<RT, A>.RetryUntil(ma, Schedule.Forever, predicate);
        
        /// <summary>
        /// Keeps retrying the computation until the predicate returns true
        /// </summary>
        /// <param name="ma">Computation to retry</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<A> retryUntil<A>(Aff<A> ma, Func<Error, bool> predicate) => 
            ScheduleAff<A>.RetryUntil(ma, Schedule.Forever, predicate);

        /// <summary>
        /// Keeps retrying the computation, until the scheduler expires, or the predicate returns true
        /// </summary>
        /// <param name="ma">Computation to retry</param>
        /// <param name="schedule">Scheduler strategy for retrying</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<RT, A> retryUntil<RT, A>(Schedule schedule, Aff<RT, A> ma, Func<Error, bool> predicate) where RT : struct, HasCancel<RT> =>
            ScheduleAff<RT, A>.RetryUntil(ma, schedule, predicate);
        
        /// <summary>
        /// Keeps retrying the computation, until the scheduler expires, or the predicate returns true
        /// </summary>
        /// <param name="ma">Computation to retry</param>
        /// <param name="schedule">Scheduler strategy for retrying</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<A> retryUntil<A>(Schedule schedule, Aff<A> ma, Func<Error, bool> predicate) => 
            ScheduleAff<A>.RetryUntil(ma, schedule, predicate);
    }
}
