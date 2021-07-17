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
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<Env, A> retry<Env, A>(Eff<Env, A> ma) =>
            ScheduleEff<Env, A>.Retry(ma, Schedule.Forever);
        
        /// <summary>
        /// Keeps retrying the computation 
        /// </summary>
        /// <param name="ma">Computation to retry</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<A> retry<A>(Eff<A> ma) => 
            ScheduleEff<A>.Retry(ma, Schedule.Forever);
        
        /// <summary>
        /// Keeps retrying the computation, until the scheduler expires  
        /// </summary>
        /// <param name="schedule">Scheduler strategy for retrying</param>
        /// <param name="ma">Computation to retry</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<Env, A> retry<Env, A>(Schedule schedule, Eff<Env, A> ma) =>
            ScheduleEff<Env, A>.Retry(ma, schedule);
        
        /// <summary>
        /// Keeps retrying the computation, until the scheduler expires   
        /// </summary>
        /// <param name="schedule">Scheduler strategy for retrying</param>
        /// <param name="ma">Computation to retry</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<A> retry<A>(Schedule schedule, Eff<A> ma) => 
            ScheduleEff<A>.Retry(ma, schedule);      
        
        
        /// <summary>
        /// Keeps retrying the computation until the predicate returns false
        /// </summary>
        /// <param name="ma">Computation to retry</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<Env, A> retryWhile<Env, A>(Eff<Env, A> ma, Func<A, bool> predicate) =>
            ScheduleEff<Env, A>.RetryWhile(ma, Schedule.Forever, predicate);
        
        /// <summary>
        /// Keeps retrying the computation until the predicate returns false
        /// </summary>
        /// <param name="ma">Computation to retry</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<A> retryWhile<A>(Eff<A> ma, Func<A, bool> predicate) => 
            ScheduleEff<A>.RetryWhile(ma, Schedule.Forever, predicate);
        
        /// <summary>
        /// Keeps retrying the computation, until the scheduler expires, or the predicate returns false
        /// </summary>
        /// <param name="schedule">Scheduler strategy for retrying</param>
        /// <param name="ma">Computation to retry</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<Env, A> retryWhile<Env, A>(Schedule schedule, Eff<Env, A> ma, Func<A, bool> predicate) =>
            ScheduleEff<Env, A>.RetryWhile(ma, schedule, predicate);
        
        /// <summary>
        /// Keeps retrying the computation, until the scheduler expires, or the predicate returns false
        /// </summary>
        /// <param name="schedule">Scheduler strategy for retrying</param>
        /// <param name="ma">Computation to retry</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<A> retryWhile<A>(Schedule schedule, Eff<A> ma, Func<A, bool> predicate) => 
            ScheduleEff<A>.RetryWhile(ma, schedule, predicate);  
        
                
        
        /// <summary>
        /// Keeps retrying the computation until the predicate returns true
        /// </summary>
        /// <param name="ma">Computation to retry</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<Env, A> retryUntil<Env, A>(Eff<Env, A> ma, Func<A, bool> predicate) =>
            ScheduleEff<Env, A>.RetryUntil(ma, Schedule.Forever, predicate);
        
        /// <summary>
        /// Keeps retrying the computation until the predicate returns true
        /// </summary>
        /// <param name="ma">Computation to retry</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<A> retryUntil<A>(Eff<A> ma, Func<A, bool> predicate) => 
            ScheduleEff<A>.RetryUntil(ma, Schedule.Forever, predicate);
        
        /// <summary>
        /// Keeps retrying the computation, until the scheduler expires, or the predicate returns true
        /// </summary>
        /// <param name="schedule">Scheduler strategy for retrying</param>
        /// <param name="ma">Computation to retry</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<Env, A> retryUntil<Env, A>(Schedule schedule, Eff<Env, A> ma, Func<A, bool> predicate) =>
            ScheduleEff<Env, A>.RetryUntil(ma, schedule, predicate);
        
        /// <summary>
        /// Keeps retrying the computation, until the scheduler expires, or the predicate returns true  
        /// </summary>
        /// <param name="schedule">Scheduler strategy for retrying</param>
        /// <param name="ma">Computation to retry</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<A> retryUntil<A>(Schedule schedule, Eff<A> ma, Func<A, bool> predicate) => 
            ScheduleEff<A>.RetryUntil(ma, schedule, predicate);  
    }
}
