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
        /// Keeps repeating the computation until it fails  
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<Env, A> repeat<Env, A>(Eff<Env, A> ma) =>
            ScheduleEff<Env, A>.Repeat(ma, Schedule.Forever);
        
        /// <summary>
        /// Keeps repeating the computation until it fails  
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<A> repeat<A>(Eff<A> ma) => 
            ScheduleEff<A>.Repeat(ma, Schedule.Forever);
        
        /// <summary>
        /// Keeps repeating the computation until it fails  
        /// </summary>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<Env, A> repeat<Env, A>(Schedule schedule, Eff<Env, A> ma) =>
            ScheduleEff<Env, A>.Repeat(ma, schedule);
        
        /// <summary>
        /// Keeps repeating the computation until it fails  
        /// </summary>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<A> repeat<A>(Schedule schedule, Eff<A> ma) => 
            ScheduleEff<A>.Repeat(ma, schedule);      
        
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns false
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<Env, A> repeatWhile<Env, A>(Eff<Env, A> ma, Func<A, bool> predicate) =>
            ScheduleEff<Env, A>.RepeatWhile(ma, Schedule.Forever, predicate);
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns false
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<A> repeatWhile<A>(Eff<A> ma, Func<A, bool> predicate) => 
            ScheduleEff<A>.RepeatWhile(ma, Schedule.Forever, predicate);
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns false
        /// </summary>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<Env, A> repeatWhile<Env, A>(Schedule schedule, Eff<Env, A> ma, Func<A, bool> predicate) =>
            ScheduleEff<Env, A>.RepeatWhile(ma, schedule, predicate);
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns false
        /// </summary>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<A> repeatWhile<A>(Schedule schedule, Eff<A> ma, Func<A, bool> predicate) => 
            ScheduleEff<A>.RepeatWhile(ma, schedule, predicate);  
        
                
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns true
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<Env, A> repeatUntil<Env, A>(Eff<Env, A> ma, Func<A, bool> predicate) =>
            ScheduleEff<Env, A>.RepeatUntil(ma, Schedule.Forever, predicate);
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns true
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<A> repeatUntil<A>(Eff<A> ma, Func<A, bool> predicate) => 
            ScheduleEff<A>.RepeatUntil(ma, Schedule.Forever, predicate);
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns true
        /// </summary>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<Env, A> repeatUntil<Env, A>(Schedule schedule, Eff<Env, A> ma, Func<A, bool> predicate) =>
            ScheduleEff<Env, A>.RepeatUntil(ma, schedule, predicate);
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns true  
        /// </summary>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<A> repeatUntil<A>(Schedule schedule, Eff<A> ma, Func<A, bool> predicate) => 
            ScheduleEff<A>.RepeatUntil(ma, schedule, predicate);  
    }
}
