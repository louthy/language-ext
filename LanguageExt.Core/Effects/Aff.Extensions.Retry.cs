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
    public static partial class AffExtensions
    {
        /// <summary>
        /// Keeps retrying the computation  
        /// </summary>
        /// <param name="ma">Computation to Retry</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<Env, A> Retry<Env, A>(this Aff<Env, A> ma) where Env : struct, HasCancel<Env> =>
            ScheduleAff<Env, A>.Retry(ma, Schedule.Forever);
        
        /// <summary>
        /// Keeps retrying the computation  
        /// </summary>
        /// <param name="ma">Computation to Retry</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<A> Retry<A>(this Aff<A> ma) => 
            ScheduleAff<A>.Retry(ma, Schedule.Forever);

        /// <summary>
        /// Keeps retrying the computation  
        /// </summary>
        /// <param name="ma">Computation to Retry</param>
        /// <param name="schedule">Scheduler strategy for Retrying</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<Env, A> Retry<Env, A>(this Aff<Env, A> ma, Schedule schedule) where Env : struct, HasCancel<Env> =>
            ScheduleAff<Env, A>.Retry(ma, schedule);
        
        /// <summary>
        /// Keeps retrying the computation  
        /// </summary>
        /// <param name="ma">Computation to Retry</param>
        /// <param name="schedule">Scheduler strategy for Retrying</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<A> Retry<A>(this Aff<A> ma, Schedule schedule) => 
            ScheduleAff<A>.Retry(ma, schedule);
        
        
        /// <summary>
        /// Keeps retrying the computation until the predicate returns false
        /// </summary>
        /// <param name="ma">Computation to Retry</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<Env, A> RetryWhile<Env, A>(this Aff<Env, A> ma, Func<A, bool> predicate) where Env : struct, HasCancel<Env> =>
            ScheduleAff<Env, A>.RetryWhile(ma, Schedule.Forever, predicate);
        
        /// <summary>
        /// Keeps retrying the computation until the predicate returns false
        /// </summary>
        /// <param name="ma">Computation to Retry</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<A> RetryWhile<A>(this Aff<A> ma, Func<A, bool> predicate) => 
            ScheduleAff<A>.RetryWhile(ma, Schedule.Forever, predicate);

        /// <summary>
        /// Keeps retrying the computation, until the scheduler expires, or the predicate returns false
        /// </summary>
        /// <param name="ma">Computation to Retry</param>
        /// <param name="schedule">Scheduler strategy for Retrying</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<Env, A> RetryWhile<Env, A>(this Aff<Env, A> ma, Schedule schedule, Func<A, bool> predicate) where Env : struct, HasCancel<Env> =>
            ScheduleAff<Env, A>.RetryWhile(ma, schedule, predicate);
        
        /// <summary>
        /// Keeps retrying the computation, until the scheduler expires, or the predicate returns false
        /// </summary>
        /// <param name="ma">Computation to Retry</param>
        /// <param name="schedule">Scheduler strategy for Retrying</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<A> RetryWhile<A>(this Aff<A> ma, Schedule schedule, Func<A, bool> predicate) => 
            ScheduleAff<A>.RetryWhile(ma, schedule, predicate);
        
        
        
        /// <summary>
        /// Keeps retrying the computation until the predicate returns true
        /// </summary>
        /// <param name="ma">Computation to Retry</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<Env, A> RetryUntil<Env, A>(this Aff<Env, A> ma, Func<A, bool> predicate) where Env : struct, HasCancel<Env> =>
            ScheduleAff<Env, A>.RetryUntil(ma, Schedule.Forever, predicate);
        
        /// <summary>
        /// Keeps retrying the computation until the predicate returns true
        /// </summary>
        /// <param name="ma">Computation to Retry</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<A> RetryUntil<A>(this Aff<A> ma, Func<A, bool> predicate) => 
            ScheduleAff<A>.RetryUntil(ma, Schedule.Forever, predicate);

        /// <summary>
        /// Keeps retrying the computation, until the scheduler expires, or the predicate returns true
        /// </summary>
        /// <param name="ma">Computation to Retry</param>
        /// <param name="schedule">Scheduler strategy for Retrying</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<Env, A> RetryUntil<Env, A>(this Aff<Env, A> ma, Schedule schedule, Func<A, bool> predicate) where Env : struct, HasCancel<Env> =>
            ScheduleAff<Env, A>.RetryUntil(ma, schedule, predicate);
        
        /// <summary>
        /// Keeps retrying the computation, until the scheduler expires, or the predicate returns true
        /// </summary>
        /// <param name="ma">Computation to Retry</param>
        /// <param name="schedule">Scheduler strategy for Retrying</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<A> RetryUntil<A>(this Aff<A> ma, Schedule schedule, Func<A, bool> predicate) => 
            ScheduleAff<A>.RetryUntil(ma, schedule, predicate);         
    }
}
