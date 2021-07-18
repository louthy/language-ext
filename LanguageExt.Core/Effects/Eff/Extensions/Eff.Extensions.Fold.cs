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
        public static Eff<RT, S> Fold<RT, S, A>(this Eff<RT, A> ma, Schedule schedule, S state, Func<S, A, S> fold) where RT : struct  =>
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
        public static Eff<RT, S> FoldWhile<RT, S, A>(this Eff<RT, A> ma, Schedule schedule, S state, Func<S, A, S> fold, Func<A, bool> pred) where RT : struct  =>
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
        public static Eff<RT, S> FoldUntil<RT, S, A>(this Eff<RT, A> ma, Schedule schedule, S state, Func<S, A, S> fold, Func<A, bool> pred) where RT : struct  =>
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
        public static Eff<RT, S> Fold<RT, S, A>(this Eff<RT, A> ma, S state, Func<S, A, S> fold) where RT : struct  =>
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
        public static Eff<RT, S> FoldWhile<RT, S, A>(this Eff<RT, A> ma, S state, Func<S, A, S> fold, Func<A, bool> pred) where RT : struct  =>
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
        public static Eff<RT, S> FoldUntil<RT, S, A>(this Eff<RT, A> ma, S state, Func<S, A, S> fold, Func<A, bool> pred) where RT : struct  =>
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
        public static Eff<S> Fold<S, A>(this Eff<A> ma, Schedule schedule, S state, Func<S, A, S> fold) =>
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
        public static Eff<S> FoldWhile<S, A>(this Eff<A> ma, Schedule schedule, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
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
        public static Eff<S> FoldUntil<S, A>(this Eff<A> ma, Schedule schedule, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
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
        public static Eff<S> Fold<S, A>(this Eff<A> ma, S state, Func<S, A, S> fold) =>
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
        public static Eff<S> FoldWhile<S, A>(this Eff<A> ma, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
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
        public static Eff<S> FoldUntil<S, A>(this Eff<A> ma, S state, Func<S, A, S> fold, Func<A, bool> pred) =>
            ScheduleEff<A>.FoldUntil(ma, Schedule.Forever, state, fold, pred);  
    }
}
