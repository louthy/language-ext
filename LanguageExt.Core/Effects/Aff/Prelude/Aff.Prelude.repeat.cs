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
        public static Aff<Env, A> repeat<Env, A>(Aff<Env, A> ma) where Env : struct, HasCancel<Env> =>
            ScheduleAff<Env, A>.Repeat(ma, Schedule.Forever);
        
        /// <summary>
        /// Keeps repeating the computation until it fails  
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<A> repeat<A>(Aff<A> ma) => 
            ScheduleAff<A>.Repeat(ma, Schedule.Forever);

        /// <summary>
        /// Keeps repeating the computation until it fails  
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<Env, A> repeat<Env, A>(Schedule schedule, Aff<Env, A> ma) where Env : struct, HasCancel<Env> =>
            ScheduleAff<Env, A>.Repeat(ma, schedule);
        
        /// <summary>
        /// Keeps repeating the computation until it fails  
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<A> repeat<A>(Schedule schedule, Aff<A> ma) => 
            ScheduleAff<A>.Repeat(ma, schedule);
        
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns false
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<Env, A> repeatWhile<Env, A>(Aff<Env, A> ma, Func<A, bool> predicate) where Env : struct, HasCancel<Env> =>
            ScheduleAff<Env, A>.RepeatWhile(ma, Schedule.Forever, predicate);
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns false
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<A> repeatWhile<A>(Aff<A> ma, Func<A, bool> predicate) => 
            ScheduleAff<A>.RepeatWhile(ma, Schedule.Forever, predicate);

        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns false  
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<Env, A> repeatWhile<Env, A>(Schedule schedule, Aff<Env, A> ma, Func<A, bool> predicate) where Env : struct, HasCancel<Env> =>
            ScheduleAff<Env, A>.RepeatWhile(ma, schedule, predicate);
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns false
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<A> repeatWhile<A>(Schedule schedule, Aff<A> ma, Func<A, bool> predicate) => 
            ScheduleAff<A>.RepeatWhile(ma, schedule, predicate);
        
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns true
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<Env, A> repeatUntil<Env, A>(Aff<Env, A> ma, Func<A, bool> predicate) where Env : struct, HasCancel<Env> =>
            ScheduleAff<Env, A>.RepeatUntil(ma, Schedule.Forever, predicate);
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns true
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<A> repeatUntil<A>(Aff<A> ma, Func<A, bool> predicate) => 
            ScheduleAff<A>.RepeatUntil(ma, Schedule.Forever, predicate);

        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns true
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<Env, A> repeatUntil<Env, A>(Schedule schedule, Aff<Env, A> ma, Func<A, bool> predicate) where Env : struct, HasCancel<Env> =>
            ScheduleAff<Env, A>.RepeatUntil(ma, schedule, predicate);
        
        /// <summary>
        /// Keeps repeating the computation until it fails or the predicate returns true
        /// </summary>
        /// <param name="ma">Computation to repeat</param>
        /// <param name="schedule">Scheduler strategy for repeating</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>The result of the last invocation of ma</returns>
        public static Aff<A> repeatUntil<A>(Schedule schedule, Aff<A> ma, Func<A, bool> predicate) => 
            ScheduleAff<A>.RepeatUntil(ma, schedule, predicate);
    }
}
