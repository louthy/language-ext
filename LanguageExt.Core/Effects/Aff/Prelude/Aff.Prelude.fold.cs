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
        /// Fold over the effect repeatedly until the schedule expires or the effect fails
        /// </summary>
        /// <param name="ma">Effect to fold over</param>
        /// <param name="schedule">Scheduler that controls the number of folds and the delay between each fold iteration</param>
        /// <param name="state">Initial state</param>
        /// <param name="fold">Folder function</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the fold operation</returns>
        public static Aff<RT, S> fold<RT, S, A>(Schedule schedule, Aff<RT, A> ma, S state, Func<S, A, S> fold)
            where RT : struct, HasCancel<RT> =>
            ScheduleAff<RT, A>.Fold(ma, schedule, state, fold);

        /// <summary>
        /// Fold over the effect repeatedly until the schedule expires, the effect fails, or the predicate returns false
        /// </summary>
        /// <param name="ma">Effect to fold over</param>
        /// <param name="schedule">Scheduler that controls the number of folds and the delay between each fold iteration</param>
        /// <param name="state">Initial state</param>
        /// <param name="fold">Folder function</param>
        /// <param name="pred">Predicate function - when this returns false, the fold ends</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the fold operation</returns>
        public static Aff<RT, S> foldWhile<RT, S, A>(Schedule schedule, Aff<RT, A> ma, S state, Func<S, A, S> fold, Func<A, bool> pred)
            where RT : struct, HasCancel<RT> =>
            ScheduleAff<RT, A>.FoldWhile(ma, schedule, state, fold, pred);

        /// <summary>
        /// Fold over the effect repeatedly until the schedule expires, the effect fails, or the predicate returns true
        /// </summary>
        /// <param name="ma">Effect to fold over</param>
        /// <param name="schedule">Scheduler that controls the number of folds and the delay between each fold iteration</param>
        /// <param name="state">Initial state</param>
        /// <param name="fold">Folder function</param>
        /// <param name="pred">Predicate function - when this returns true, the fold ends</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the fold operation</returns>
        public static Aff<RT, S> foldUntil<RT, S, A>(Schedule schedule, Aff<RT, A> ma, S state, Func<S, A, S> fold, Func<A, bool> pred)
            where RT : struct, HasCancel<RT> =>
            ScheduleAff<RT, A>.FoldUntil(ma, schedule, state, fold, pred);
        
        /// <summary>
        /// Fold over the effect repeatedly until the effect fails
        /// </summary>
        /// <param name="ma">Effect to fold over</param>
        /// <param name="state">Initial state</param>
        /// <param name="fold">Folder function</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the fold operation</returns>
        public static Aff<RT, S> fold<RT, S, A>(Aff<RT, A> ma, S state, Func<S, A, S> fold)
            where RT : struct, HasCancel<RT> =>
            ScheduleAff<RT, A>.Fold(ma, Schedule.Forever, state, fold);

        /// <summary>
        /// Fold over the effect repeatedly until the effect fails or the predicate returns false
        /// </summary>
        /// <param name="ma">Effect to fold over</param>
        /// <param name="state">Initial state</param>
        /// <param name="fold">Folder function</param>
        /// <param name="pred">Predicate function - when this returns false, the fold ends</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the fold operation</returns>
        public static Aff<RT, S> foldWhile<RT, S, A>(Aff<RT, A> ma, S state, Func<S, A, S> fold, Func<A, bool> pred)
            where RT : struct, HasCancel<RT> =>
            ScheduleAff<RT, A>.FoldWhile(ma, Schedule.Forever, state, fold, pred);

        /// <summary>
        /// Fold over the effect repeatedly until the effect fails or the predicate returns true 
        /// </summary>
        /// <param name="ma">Effect to fold over</param>
        /// <param name="state">Initial state</param>
        /// <param name="fold">Folder function</param>
        /// <param name="pred">Predicate function - when this returns true, the fold ends</param>
        /// <typeparam name="RT">Runtime</typeparam>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the fold operation</returns>
        public static Aff<RT, S> foldUntil<RT, S, A>(Aff<RT, A> ma, S state, Func<S, A, S> fold, Func<A, bool> pred)
            where RT : struct, HasCancel<RT> =>
            ScheduleAff<RT, A>.FoldUntil(ma, Schedule.Forever, state, fold, pred);  
        
        /// <summary>
        /// Fold over the effect repeatedly until the schedule expires or the effect fails
        /// </summary>
        /// <param name="ma">Effect to fold over</param>
        /// <param name="schedule">Scheduler that controls the number of folds and the delay between each fold iteration</param>
        /// <param name="state">Initial state</param>
        /// <param name="fold">Folder function</param>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the fold operation</returns>
        public static Aff<S> fold<S, A>(Schedule schedule, Aff<A> ma, S state, Func<S, A, S> fold) =>
            ScheduleAff<A>.Fold(ma, schedule, state, fold);

        /// <summary>
        /// Fold over the effect repeatedly until the schedule expires, the effect fails, or the predicate returns false
        /// </summary>
        /// <param name="ma">Effect to fold over</param>
        /// <param name="schedule">Scheduler that controls the number of folds and the delay between each fold iteration</param>
        /// <param name="state">Initial state</param>
        /// <param name="fold">Folder function</param>
        /// <param name="pred">Predicate function - when this returns false, the fold ends</param>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the fold operation</returns>
        public static Aff<S> foldWhile<S, A>(Schedule schedule, Aff<A> ma, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            ScheduleAff<A>.FoldWhile(ma, schedule, state, fold, pred);

        /// <summary>
        /// Fold over the effect repeatedly until the schedule expires, the effect fails, or the predicate returns true
        /// </summary>
        /// <param name="ma">Effect to fold over</param>
        /// <param name="schedule">Scheduler that controls the number of folds and the delay between each fold iteration</param>
        /// <param name="state">Initial state</param>
        /// <param name="fold">Folder function</param>
        /// <param name="pred">Predicate function - when this returns true, the fold ends</param>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the fold operation</returns>
        public static Aff<S> foldUntil<S, A>(Schedule schedule, Aff<A> ma, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            ScheduleAff<A>.FoldUntil(ma, schedule, state, fold, pred);
        
        /// <summary>
        /// Fold over the effect repeatedly until the effect fails
        /// </summary>
        /// <param name="ma">Effect to fold over</param>
        /// <param name="state">Initial state</param>
        /// <param name="fold">Folder function</param>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the fold operation</returns>
        public static Aff<S> fold<S, A>(Aff<A> ma, S state, Func<S, A, S> fold) =>
            ScheduleAff<A>.Fold(ma, Schedule.Forever, state, fold);

        /// <summary>
        /// Fold over the effect repeatedly until the effect fails or the predicate returns false
        /// </summary>
        /// <param name="ma">Effect to fold over</param>
        /// <param name="state">Initial state</param>
        /// <param name="fold">Folder function</param>
        /// <param name="pred">Predicate function - when this returns false, the fold ends</param>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the fold operation</returns>
        public static Aff<S> foldWhile<S, A>(Aff<A> ma, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            ScheduleAff<A>.FoldWhile(ma, Schedule.Forever, state, fold, pred);

        /// <summary>
        /// Fold over the effect repeatedly until the effect fails or the predicate returns true 
        /// </summary>
        /// <param name="ma">Effect to fold over</param>
        /// <param name="state">Initial state</param>
        /// <param name="fold">Folder function</param>
        /// <param name="pred">Predicate function - when this returns true, the fold ends</param>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the fold operation</returns>
        public static Aff<S> foldUntil<S, A>(Aff<A> ma, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            ScheduleAff<A>.FoldUntil(ma, Schedule.Forever, state, fold, pred);  
    }
}
