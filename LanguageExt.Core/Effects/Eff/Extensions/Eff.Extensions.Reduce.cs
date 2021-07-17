using System;
using System.Threading;
using LanguageExt.Common;
using LanguageExt.Thunks;
using System.Threading.Tasks;
using LanguageExt.Effects.Traits;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace LanguageExt
{
    public static partial class EffExtensions
    {
        /// <summary>
        /// Reduce over the effect repeatedly until the schedule expires or the effect fails
        /// </summary>
        /// <param name="ma">Effect to reduce over</param>
        /// <param name="schedule">Scheduler that controls the number of reduces and the delay between each reduce iteration</param>
        /// <param name="state">Initial state</param>
        /// <param name="reduce">Reducer function</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the reduce operation</returns>
        public static Eff<RT, S> Reduce<RT, S, A>(this Eff<RT, A> ma, Schedule schedule, Func<S, A, S> reduce) =>
            ScheduleEff<RT, A>.Fold(ma, schedule, default, reduce);

        /// <summary>
        /// Reduce over the effect repeatedly until the schedule expires, the effect fails, or the predicate returns false
        /// </summary>
        /// <param name="ma">Effect to reduce over</param>
        /// <param name="schedule">Scheduler that controls the number of reduces and the delay between each reduce iteration</param>
        /// <param name="state">Initial state</param>
        /// <param name="reduce">Reducer function</param>
        /// <param name="pred">Predicate function - when this returns false, the reduce ends</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the reduce operation</returns>
        public static Eff<RT, S> ReduceWhile<RT, S, A>(this Eff<RT, A> ma, Schedule schedule, Func<S, A, S> reduce, Func<A, bool> pred) =>
            ScheduleEff<RT, A>.FoldWhile(ma, schedule, default, reduce, pred);

        /// <summary>
        /// Reduce over the effect repeatedly until the schedule expires, the effect fails, or the predicate returns true
        /// </summary>
        /// <param name="ma">Effect to reduce over</param>
        /// <param name="schedule">Scheduler that controls the number of reduces and the delay between each reduce iteration</param>
        /// <param name="state">Initial state</param>
        /// <param name="reduce">Reducer function</param>
        /// <param name="pred">Predicate function - when this returns true, the reduce ends</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the reduce operation</returns>
        public static Eff<RT, S> ReduceUntil<RT, S, A>(this Eff<RT, A> ma, Schedule schedule, Func<S, A, S> reduce, Func<A, bool> pred) =>
            ScheduleEff<RT, A>.FoldUntil(ma, schedule, default, reduce, pred);
        
        /// <summary>
        /// Reduce over the effect repeatedly until the effect fails
        /// </summary>
        /// <param name="ma">Effect to reduce over</param>
        /// <param name="state">Initial state</param>
        /// <param name="reduce">Reducer function</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the reduce operation</returns>
        public static Eff<RT, S> Reduce<RT, S, A>(this Eff<RT, A> ma, Func<S, A, S> reduce) =>
            ScheduleEff<RT, A>.Fold(ma, Schedule.Forever, default, reduce);

        /// <summary>
        /// Reduce over the effect repeatedly until the effect fails or the predicate returns false
        /// </summary>
        /// <param name="ma">Effect to reduce over</param>
        /// <param name="state">Initial state</param>
        /// <param name="reduce">Reducer function</param>
        /// <param name="pred">Predicate function - when this returns false, the reduce ends</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the reduce operation</returns>
        public static Eff<RT, S> ReduceWhile<RT, S, A>(this Eff<RT, A> ma, Func<S, A, S> reduce, Func<A, bool> pred) =>
            ScheduleEff<RT, A>.FoldWhile(ma, Schedule.Forever, default, reduce, pred);

        /// <summary>
        /// Reduce over the effect repeatedly until the effect fails or the predicate returns true 
        /// </summary>
        /// <param name="ma">Effect to reduce over</param>
        /// <param name="state">Initial state</param>
        /// <param name="reduce">Reducer function</param>
        /// <param name="pred">Predicate function - when this returns true, the reduce ends</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the reduce operation</returns>
        public static Eff<RT, S> ReduceUntil<RT, S, A>(this Eff<RT, A> ma, Func<S, A, S> reduce, Func<A, bool> pred) =>
            ScheduleEff<RT, A>.FoldUntil(ma, Schedule.Forever, default, reduce, pred);      
        
        
        /// <summary>
        /// Reduce over the effect repeatedly until the schedule expires or the effect fails
        /// </summary>
        /// <param name="ma">Effect to reduce over</param>
        /// <param name="schedule">Scheduler that controls the number of reduces and the delay between each reduce iteration</param>
        /// <param name="state">Initial state</param>
        /// <param name="reduce">Reducer function</param>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the reduce operation</returns>
        public static Eff<S> Reduce<S, A>(this Eff<A> ma, Schedule schedule, Func<S, A, S> reduce) =>
            ScheduleEff<A>.Fold(ma, schedule, default, reduce);

        /// <summary>
        /// Reduce over the effect repeatedly until the schedule expires, the effect fails, or the predicate returns false
        /// </summary>
        /// <param name="ma">Effect to reduce over</param>
        /// <param name="schedule">Scheduler that controls the number of reduces and the delay between each reduce iteration</param>
        /// <param name="state">Initial state</param>
        /// <param name="reduce">Reducer function</param>
        /// <param name="pred">Predicate function - when this returns false, the reduce ends</param>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the reduce operation</returns>
        public static Eff<S> ReduceWhile<S, A>(this Eff<A> ma, Schedule schedule, Func<S, A, S> reduce, Func<A, bool> pred) =>
            ScheduleEff<A>.FoldWhile(ma, schedule, default, reduce, pred);

        /// <summary>
        /// Reduce over the effect repeatedly until the schedule expires, the effect fails, or the predicate returns true
        /// </summary>
        /// <param name="ma">Effect to reduce over</param>
        /// <param name="schedule">Scheduler that controls the number of reduces and the delay between each reduce iteration</param>
        /// <param name="state">Initial state</param>
        /// <param name="reduce">Reducer function</param>
        /// <param name="pred">Predicate function - when this returns true, the reduce ends</param>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the reduce operation</returns>
        public static Eff<S> ReduceUntil<S, A>(this Eff<A> ma, Schedule schedule, Func<S, A, S> reduce, Func<A, bool> pred) =>
            ScheduleEff<A>.FoldUntil(ma, schedule, default, reduce, pred);
        
        /// <summary>
        /// Reduce over the effect repeatedly until the effect fails
        /// </summary>
        /// <param name="ma">Effect to reduce over</param>
        /// <param name="state">Initial state</param>
        /// <param name="reduce">Reducer function</param>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the reduce operation</returns>
        public static Eff<S> Reduce<S, A>(this Eff<A> ma, Func<S, A, S> reduce) =>
            ScheduleEff<A>.Fold(ma, Schedule.Forever, default, reduce);

        /// <summary>
        /// Reduce over the effect repeatedly until the effect fails or the predicate returns false
        /// </summary>
        /// <param name="ma">Effect to reduce over</param>
        /// <param name="state">Initial state</param>
        /// <param name="reduce">Reducer function</param>
        /// <param name="pred">Predicate function - when this returns false, the reduce ends</param>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the reduce operation</returns>
        public static Eff<S> ReduceWhile<S, A>(this Eff<A> ma, Func<S, A, S> reduce, Func<A, bool> pred) =>
            ScheduleEff<A>.FoldWhile(ma, Schedule.Forever, default, reduce, pred);

        /// <summary>
        /// Reduce over the effect repeatedly until the effect fails or the predicate returns true 
        /// </summary>
        /// <param name="ma">Effect to reduce over</param>
        /// <param name="state">Initial state</param>
        /// <param name="reduce">Reducer function</param>
        /// <param name="pred">Predicate function - when this returns true, the reduce ends</param>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the reduce operation</returns>
        public static Eff<S> ReduceUntil<S, A>(this Eff<A> ma, Func<S, A, S> reduce, Func<A, bool> pred) =>
            ScheduleEff<A>.FoldUntil(ma, Schedule.Forever, default, reduce, pred);  
    }
}
