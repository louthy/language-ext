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
        public static Eff<RT, S> fold<RT, S, A>(Schedule schedule, Eff<RT, A> ma, S state, Func<S, A, S> fold) where RT : struct =>
            ScheduleEff<RT, A>.Fold(ma, schedule, state, fold);

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
        public static Eff<RT, S> foldWhile<RT, S, A>(Schedule schedule, Eff<RT, A> ma, S state, Func<S, A, S> fold, Func<A, bool> pred) where RT : struct =>
            ScheduleEff<RT, A>.FoldWhile(ma, schedule, state, fold, pred);

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
        public static Eff<RT, S> foldUntil<RT, S, A>(Schedule schedule, Eff<RT, A> ma, S state, Func<S, A, S> fold, Func<A, bool> pred) where RT : struct =>
            ScheduleEff<RT, A>.FoldUntil(ma, schedule, state, fold, pred);
        
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
        public static Eff<RT, S> fold<RT, S, A>(Eff<RT, A> ma, S state, Func<S, A, S> fold) where RT : struct =>
            ScheduleEff<RT, A>.Fold(ma, Schedule.Forever, state, fold);

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
        public static Eff<RT, S> foldWhile<RT, S, A>(Eff<RT, A> ma, S state, Func<S, A, S> fold, Func<A, bool> pred) where RT : struct =>
            ScheduleEff<RT, A>.FoldWhile(ma, Schedule.Forever, state, fold, pred);

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
        public static Eff<RT, S> foldUntil<RT, S, A>(Eff<RT, A> ma, S state, Func<S, A, S> fold, Func<A, bool> pred) where RT : struct =>
            ScheduleEff<RT, A>.FoldUntil(ma, Schedule.Forever, state, fold, pred);  
        
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
        public static Eff<S> fold<S, A>(Schedule schedule, Eff<A> ma, S state, Func<S, A, S> fold) =>
            ScheduleEff<A>.Fold(ma, schedule, state, fold);

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
        public static Eff<S> foldWhile<S, A>(Schedule schedule, Eff<A> ma, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            ScheduleEff<A>.FoldWhile(ma, schedule, state, fold, pred);

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
        public static Eff<S> foldUntil<S, A>(Schedule schedule, Eff<A> ma, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            ScheduleEff<A>.FoldUntil(ma, schedule, state, fold, pred);
        
        /// <summary>
        /// Fold over the effect repeatedly until the effect fails
        /// </summary>
        /// <param name="ma">Effect to fold over</param>
        /// <param name="state">Initial state</param>
        /// <param name="fold">Folder function</param>
        /// <typeparam name="S">State type</typeparam>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>The result of the fold operation</returns>
        public static Eff<S> fold<S, A>(Eff<A> ma, S state, Func<S, A, S> fold) =>
            ScheduleEff<A>.Fold(ma, Schedule.Forever, state, fold);

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
        public static Eff<S> foldWhile<S, A>(Eff<A> ma, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            ScheduleEff<A>.FoldWhile(ma, Schedule.Forever, state, fold, pred);

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
        public static Eff<S> foldUntil<S, A>(Eff<A> ma, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            ScheduleEff<A>.FoldUntil(ma, Schedule.Forever, state, fold, pred);  
    }
}
