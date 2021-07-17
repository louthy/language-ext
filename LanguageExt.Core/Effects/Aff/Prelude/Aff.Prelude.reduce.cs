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
        /// <param name="reduce">Folder function</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the reduce operation</returns>
        public static Aff<RT, S> reduce<RT, S, A>(Schedule schedule, Aff<RT, A> ma, Func<S, A, S> reduce)
            where RT : struct, HasCancel<RT> =>
            ScheduleAff<RT, A>.Fold(ma, schedule, default, reduce);

        /// <summary>
        /// Reduce over the effect repeatedly until the schedule expires, the effect fails, or the predicate returns false
        /// </summary>
        /// <param name="ma">Effect to reduce over</param>
        /// <param name="schedule">Scheduler that controls the number of reduces and the delay between each reduce iteration</param>
        /// <param name="state">Initial state</param>
        /// <param name="reduce">Folder function</param>
        /// <param name="pred">Predicate function - when this returns false, the reduce ends</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the reduce operation</returns>
        public static Aff<RT, S> reduceWhile<RT, S, A>(Schedule schedule, Aff<RT, A> ma, Func<S, A, S> reduce, Func<A, bool> pred)
            where RT : struct, HasCancel<RT> =>
            ScheduleAff<RT, A>.FoldWhile(ma, schedule, default, reduce, pred);

        /// <summary>
        /// Reduce over the effect repeatedly until the schedule expires, the effect fails, or the predicate returns true
        /// </summary>
        /// <param name="ma">Effect to reduce over</param>
        /// <param name="schedule">Scheduler that controls the number of reduces and the delay between each reduce iteration</param>
        /// <param name="state">Initial state</param>
        /// <param name="reduce">Folder function</param>
        /// <param name="pred">Predicate function - when this returns true, the reduce ends</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the reduce operation</returns>
        public static Aff<RT, S> reduceUntil<RT, S, A>(Schedule schedule, Aff<RT, A> ma, Func<S, A, S> reduce, Func<A, bool> pred)
            where RT : struct, HasCancel<RT> =>
            ScheduleAff<RT, A>.FoldUntil(ma, schedule, default, reduce, pred);
        
        /// <summary>
        /// Reduce over the effect repeatedly until the effect fails
        /// </summary>
        /// <param name="ma">Effect to reduce over</param>
        /// <param name="state">Initial state</param>
        /// <param name="reduce">Folder function</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the reduce operation</returns>
        public static Aff<RT, S> reduce<RT, S, A>(Aff<RT, A> ma, Func<S, A, S> reduce)
            where RT : struct, HasCancel<RT> =>
            ScheduleAff<RT, A>.Fold(ma, Schedule.Forever, default, reduce);

        /// <summary>
        /// Reduce over the effect repeatedly until the effect fails or the predicate returns false
        /// </summary>
        /// <param name="ma">Effect to reduce over</param>
        /// <param name="state">Initial state</param>
        /// <param name="reduce">Folder function</param>
        /// <param name="pred">Predicate function - when this returns false, the reduce ends</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the reduce operation</returns>
        public static Aff<RT, S> reduceWhile<RT, S, A>(Aff<RT, A> ma, Func<S, A, S> reduce, Func<A, bool> pred)
            where RT : struct, HasCancel<RT> =>
            ScheduleAff<RT, A>.FoldWhile(ma, Schedule.Forever, default, reduce, pred);

        /// <summary>
        /// Reduce over the effect repeatedly until the effect fails or the predicate returns true 
        /// </summary>
        /// <param name="ma">Effect to reduce over</param>
        /// <param name="state">Initial state</param>
        /// <param name="reduce">Folder function</param>
        /// <param name="pred">Predicate function - when this returns true, the reduce ends</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the reduce operation</returns>
        public static Aff<RT, S> reduceUntil<RT, S, A>(Aff<RT, A> ma, Func<S, A, S> reduce, Func<A, bool> pred)
            where RT : struct, HasCancel<RT> =>
            ScheduleAff<RT, A>.FoldUntil(ma, Schedule.Forever, default, reduce, pred);  
        
        /// <summary>
        /// Reduce over the effect repeatedly until the schedule expires or the effect fails
        /// </summary>
        /// <param name="ma">Effect to reduce over</param>
        /// <param name="schedule">Scheduler that controls the number of reduces and the delay between each reduce iteration</param>
        /// <param name="state">Initial state</param>
        /// <param name="reduce">Folder function</param>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the reduce operation</returns>
        public static Aff<S> reduce<S, A>(Schedule schedule, Aff<A> ma, Func<S, A, S> reduce) =>
            ScheduleAff<A>.Fold(ma, schedule, default, reduce);

        /// <summary>
        /// Reduce over the effect repeatedly until the schedule expires, the effect fails, or the predicate returns false
        /// </summary>
        /// <param name="ma">Effect to reduce over</param>
        /// <param name="schedule">Scheduler that controls the number of reduces and the delay between each reduce iteration</param>
        /// <param name="state">Initial state</param>
        /// <param name="reduce">Folder function</param>
        /// <param name="pred">Predicate function - when this returns false, the reduce ends</param>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the reduce operation</returns>
        public static Aff<S> reduceWhile<S, A>(Schedule schedule, Aff<A> ma, Func<S, A, S> reduce, Func<A, bool> pred) =>
            ScheduleAff<A>.FoldWhile(ma, schedule, default, reduce, pred);

        /// <summary>
        /// Reduce over the effect repeatedly until the schedule expires, the effect fails, or the predicate returns true
        /// </summary>
        /// <param name="ma">Effect to reduce over</param>
        /// <param name="schedule">Scheduler that controls the number of reduces and the delay between each reduce iteration</param>
        /// <param name="state">Initial state</param>
        /// <param name="reduce">Folder function</param>
        /// <param name="pred">Predicate function - when this returns true, the reduce ends</param>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the reduce operation</returns>
        public static Aff<S> reduceUntil<S, A>(Schedule schedule, Aff<A> ma, Func<S, A, S> reduce, Func<A, bool> pred) =>
            ScheduleAff<A>.FoldUntil(ma, schedule, default, reduce, pred);
        
        /// <summary>
        /// Reduce over the effect repeatedly until the effect fails
        /// </summary>
        /// <param name="ma">Effect to reduce over</param>
        /// <param name="state">Initial state</param>
        /// <param name="reduce">Folder function</param>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the reduce operation</returns>
        public static Aff<S> reduce<S, A>(Aff<A> ma, Func<S, A, S> reduce) =>
            ScheduleAff<A>.Fold(ma, Schedule.Forever, default, reduce);

        /// <summary>
        /// Reduce over the effect repeatedly until the effect fails or the predicate returns false
        /// </summary>
        /// <param name="ma">Effect to reduce over</param>
        /// <param name="state">Initial state</param>
        /// <param name="reduce">Folder function</param>
        /// <param name="pred">Predicate function - when this returns false, the reduce ends</param>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the reduce operation</returns>
        public static Aff<S> reduceWhile<S, A>(Aff<A> ma, Func<S, A, S> reduce, Func<A, bool> pred) =>
            ScheduleAff<A>.FoldWhile(ma, Schedule.Forever, default, reduce, pred);

        /// <summary>
        /// Reduce over the effect repeatedly until the effect fails or the predicate returns true 
        /// </summary>
        /// <param name="ma">Effect to reduce over</param>
        /// <param name="state">Initial state</param>
        /// <param name="reduce">Folder function</param>
        /// <param name="pred">Predicate function - when this returns true, the reduce ends</param>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the reduce operation</returns>
        public static Aff<S> reduceUntil<S, A>(Aff<A> ma, Func<S, A, S> reduce, Func<A, bool> pred) =>
            ScheduleAff<A>.FoldUntil(ma, Schedule.Forever, default, reduce, pred);  
    }
}
