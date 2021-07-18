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
        /// Keeps repeating the computation until it fails  
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<RT, A> Repeat<RT, A>(this Eff<RT, A> ma) where RT : struct =>
            ScheduleEff<RT, A>.Repeat(ma, Schedule.Forever);
        
        /// <summary>
        /// Keeps repeating the computation until it fails  
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<A> Repeat<A>(this Eff<A> ma) => 
            ScheduleEff<A>.Repeat(ma, Schedule.Forever);
        
        /// <summary>
        /// Keeps repeating the computation until it fails  
        /// </summary>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<RT, A> Repeat<RT, A>(this Eff<RT, A> ma, Schedule schedule) where RT : struct =>
            ScheduleEff<RT, A>.Repeat(ma, schedule);
        
        /// <summary>
        /// Keeps repeating the computation until it fails  
        /// </summary>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<A> Repeat<A>(this Eff<A> ma, Schedule schedule) => 
            ScheduleEff<A>.Repeat(ma, schedule);       
        
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns false
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<RT, A> RepeatWhile<RT, A>(this Eff<RT, A> ma, Func<A, bool> predicate) where RT : struct =>
            ScheduleEff<RT, A>.RepeatWhile(ma, Schedule.Forever, predicate);
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns false
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<A> RepeatWhile<A>(this Eff<A> ma, Func<A, bool> predicate) => 
            ScheduleEff<A>.RepeatWhile(ma, Schedule.Forever, predicate);
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns false
        /// </summary>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<RT, A> RepeatWhile<RT, A>(this Eff<RT, A> ma, Schedule schedule, Func<A, bool> predicate) where RT : struct =>
            ScheduleEff<RT, A>.RepeatWhile(ma, schedule, predicate);
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns false
        /// </summary>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<A> RepeatWhile<A>(this Eff<A> ma, Schedule schedule, Func<A, bool> predicate) => 
            ScheduleEff<A>.RepeatWhile(ma, schedule, predicate);     
        
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns true
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<RT, A> RepeatUntil<RT, A>(this Eff<RT, A> ma, Func<A, bool> predicate) where RT : struct =>
            ScheduleEff<RT, A>.RepeatUntil(ma, Schedule.Forever, predicate);
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns true
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<A> RepeatUntil<A>(this Eff<A> ma, Func<A, bool> predicate) => 
            ScheduleEff<A>.RepeatUntil(ma, Schedule.Forever, predicate);
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns true
        /// </summary>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<RT, A> RepeatUntil<RT, A>(this Eff<RT, A> ma, Schedule schedule, Func<A, bool> predicate) where RT : struct =>
            ScheduleEff<RT, A>.RepeatUntil(ma, schedule, predicate);
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns true 
        /// </summary>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Eff<A> RepeatUntil<A>(this Eff<A> ma, Schedule schedule, Func<A, bool> predicate) => 
            ScheduleEff<A>.RepeatUntil(ma, schedule, predicate);
    }
}
