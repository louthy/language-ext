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
    public static partial class EffExtensions
    {
        /// <summary>
        /// Keeps retrying the computation  
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<RT, A> Retry<RT, A>(this Eff<RT, A> ma) where RT : struct =>
            ScheduleEff<RT, A>.Retry(ma, Schedule.Forever);
        
        /// <summary>
        /// Keeps retrying the computation  
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<A> Retry<A>(this Eff<A> ma) => 
            ScheduleEff<A>.Retry(ma, Schedule.Forever);
        
        /// <summary>
        /// Keeps retrying the computation, until the scheduler expires
        /// </summary>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<RT, A> Retry<RT, A>(this Eff<RT, A> ma, Schedule schedule) where RT : struct =>
            ScheduleEff<RT, A>.Retry(ma, schedule);
        
        /// <summary>
        /// Keeps retrying the computation, until the scheduler expires 
        /// </summary>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<A> Retry<A>(this Eff<A> ma, Schedule schedule) => 
            ScheduleEff<A>.Retry(ma, schedule);       
        
        
        /// <summary>
        /// Keeps retrying the computation until the predicate returns false
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<RT, A> RetryWhile<RT, A>(this Eff<RT, A> ma, Func<Error, bool> predicate) where RT : struct =>
            ScheduleEff<RT, A>.RetryWhile(ma, Schedule.Forever, predicate);
        
        /// <summary>
        /// Keeps retrying the computation until the predicate returns false
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<A> RetryWhile<A>(this Eff<A> ma, Func<Error, bool> predicate) => 
            ScheduleEff<A>.RetryWhile(ma, Schedule.Forever, predicate);
        
        /// <summary>
        /// Keeps retrying the computation, until the scheduler expires, or the predicate returns false
        /// </summary>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<RT, A> RetryWhile<RT, A>(this Eff<RT, A> ma, Schedule schedule, Func<Error, bool> predicate) where RT : struct =>
            ScheduleEff<RT, A>.RetryWhile(ma, schedule, predicate);
        
        /// <summary>
        /// Keeps retrying the computation, until the scheduler expires, or the predicate returns false
        /// </summary>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<A> RetryWhile<A>(this Eff<A> ma, Schedule schedule, Func<Error, bool> predicate) => 
            ScheduleEff<A>.RetryWhile(ma, schedule, predicate);     
        
        
        /// <summary>
        /// Keeps retrying the computation until the predicate returns true
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<RT, A> RetryUntil<RT, A>(this Eff<RT, A> ma, Func<Error, bool> predicate) where RT : struct =>
            ScheduleEff<RT, A>.RetryUntil(ma, Schedule.Forever, predicate);
        
        /// <summary>
        /// Keeps retrying the computation until the predicate returns true
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<A> RetryUntil<A>(this Eff<A> ma, Func<Error, bool> predicate) => 
            ScheduleEff<A>.RetryUntil(ma, Schedule.Forever, predicate);
        
        /// <summary>
        /// Keeps retrying the computation, until the scheduler expires, or the predicate returns true
        /// </summary>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<RT, A> RetryUntil<RT, A>(this Eff<RT, A> ma, Schedule schedule, Func<Error, bool> predicate) where RT : struct =>
            ScheduleEff<RT, A>.RetryUntil(ma, schedule, predicate);
        
        /// <summary>
        /// Keeps retrying the computation, until the scheduler expires, or the predicate returns true 
        /// </summary>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<A> RetryUntil<A>(this Eff<A> ma, Schedule schedule, Func<Error, bool> predicate) => 
            ScheduleEff<A>.RetryUntil(ma, schedule, predicate);     
    }
}
