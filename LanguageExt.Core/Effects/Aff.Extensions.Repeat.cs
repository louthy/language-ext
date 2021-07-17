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
        /// Keeps repeating the computation until it fails  
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<Env, A> Repeat<Env, A>(this Aff<Env, A> ma) where Env : struct, HasCancel<Env> =>
            ScheduleAff<Env, A>.Repeat(ma, Schedule.Forever);
        
        /// <summary>
        /// Keeps repeating the computation until it fails  
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<A> Repeat<A>(this Aff<A> ma) => 
            ScheduleAff<A>.Repeat(ma, Schedule.Forever);

        /// <summary>
        /// Keeps repeating the computation until it fails  
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<Env, A> Repeat<Env, A>(this Aff<Env, A> ma, Schedule schedule) where Env : struct, HasCancel<Env> =>
            ScheduleAff<Env, A>.Repeat(ma, schedule);
        
        /// <summary>
        /// Keeps repeating the computation until it fails  
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<A> Repeat<A>(this Aff<A> ma, Schedule schedule) => 
            ScheduleAff<A>.Repeat(ma, schedule);
        
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns false
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<Env, A> RepeatWhile<Env, A>(this Aff<Env, A> ma, Func<A, bool> predicate) where Env : struct, HasCancel<Env> =>
            ScheduleAff<Env, A>.RepeatWhile(ma, Schedule.Forever, predicate);
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns false
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<A> RepeatWhile<A>(this Aff<A> ma, Func<A, bool> predicate) => 
            ScheduleAff<A>.RepeatWhile(ma, Schedule.Forever, predicate);

        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns false
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<Env, A> RepeatWhile<Env, A>(this Aff<Env, A> ma, Schedule schedule, Func<A, bool> predicate) where Env : struct, HasCancel<Env> =>
            ScheduleAff<Env, A>.RepeatWhile(ma, schedule, predicate);
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns false
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<A> RepeatWhile<A>(this Aff<A> ma, Schedule schedule, Func<A, bool> predicate) => 
            ScheduleAff<A>.RepeatWhile(ma, schedule, predicate);
        
        
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns true
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<Env, A> RepeatUntil<Env, A>(this Aff<Env, A> ma, Func<A, bool> predicate) where Env : struct, HasCancel<Env> =>
            ScheduleAff<Env, A>.RepeatUntil(ma, Schedule.Forever, predicate);
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns true
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<A> RepeatUntil<A>(this Aff<A> ma, Func<A, bool> predicate) => 
            ScheduleAff<A>.RepeatUntil(ma, Schedule.Forever, predicate);

        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns true
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<Env, A> RepeatUntil<Env, A>(this Aff<Env, A> ma, Schedule schedule, Func<A, bool> predicate) where Env : struct, HasCancel<Env> =>
            ScheduleAff<Env, A>.RepeatUntil(ma, schedule, predicate);
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns true
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<A> RepeatUntil<A>(this Aff<A> ma, Schedule schedule, Func<A, bool> predicate) => 
            ScheduleAff<A>.RepeatUntil(ma, schedule, predicate);         
    }
}
