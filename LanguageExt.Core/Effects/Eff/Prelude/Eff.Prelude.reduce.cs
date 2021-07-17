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
        public static Eff<RT, S> reduce<RT, S, A>(Schedule schedule, Eff<RT, A> ma, Func<S, A, S> reduce) =>
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
        public static Eff<RT, S> reduceWhile<RT, S, A>(Schedule schedule, Eff<RT, A> ma, Func<S, A, S> reduce, Func<A, bool> pred) =>
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
        public static Eff<RT, S> reduceUntil<RT, S, A>(Schedule schedule, Eff<RT, A> ma, Func<S, A, S> reduce, Func<A, bool> pred) =>
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
        public static Eff<RT, S> reduce<RT, S, A>(Eff<RT, A> ma, Func<S, A, S> reduce) =>
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
        public static Eff<RT, S> reduceWhile<RT, S, A>(Eff<RT, A> ma, Func<S, A, S> reduce, Func<A, bool> pred) =>
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
        public static Eff<RT, S> reduceUntil<RT, S, A>(Eff<RT, A> ma, Func<S, A, S> reduce, Func<A, bool> pred) =>
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
        public static Eff<S> reduce<S, A>(Schedule schedule, Eff<A> ma, Func<S, A, S> reduce) =>
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
        public static Eff<S> reduceWhile<S, A>(Schedule schedule, Eff<A> ma, Func<S, A, S> reduce, Func<A, bool> pred) =>
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
        public static Eff<S> reduceUntil<S, A>(Schedule schedule, Eff<A> ma, Func<S, A, S> reduce, Func<A, bool> pred) =>
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
        public static Eff<S> reduce<S, A>(Eff<A> ma, Func<S, A, S> reduce) =>
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
        public static Eff<S> reduceWhile<S, A>(Eff<A> ma, Func<S, A, S> reduce, Func<A, bool> pred) =>
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
        public static Eff<S> reduceUntil<S, A>(Eff<A> ma, Func<S, A, S> reduce, Func<A, bool> pred) =>
            ScheduleEff<A>.FoldUntil(ma, Schedule.Forever, default, reduce, pred);  
    }
}
