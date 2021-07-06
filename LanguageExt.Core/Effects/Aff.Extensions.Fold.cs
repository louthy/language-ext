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
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<Env, A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> =>
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<Env, A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<Env, A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> =>
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<Env, A> ma, S state, Func<S, A, Eff<S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<A> ma, S state, Func<S, A, Eff<S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Eff<Env, A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Eff<Env, A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Eff<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Eff<A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
        
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<Env, A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<Env, A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<Env, A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<Env, A> ma, S state, Func<S, A, Eff<S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<A> ma, S state, Func<S, A, Eff<S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Eff<Env, A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Eff<Env, A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Eff<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Eff<A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<Env, A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Aff<bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<Env, A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Aff<bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<Env, A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, Aff<bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<Env, A> ma, S state, Func<S, A, Eff<S>> f, Func<S, Aff<bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Aff<bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<S> FoldWhile<S, A>(this Aff<A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Aff<bool>> pred) => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, Aff<bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<S> FoldWhile<S, A>(this Aff<A> ma, S state, Func<S, A, Eff<S>> f, Func<S, Aff<bool>> pred) => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Eff<Env, A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Aff<bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Eff<Env, A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Aff<bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Eff<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Aff<bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<S> FoldWhile<S, A>(this Eff<A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Aff<bool>> pred) => 
            Prelude.foldWhile(ma, state, f, pred);
        
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<Env, A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Eff<bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<Env, A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Eff<bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<Env, A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, Eff<bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<Env, A> ma, S state, Func<S, A, Eff<S>> f, Func<S, Eff<bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Eff<bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<S> FoldWhile<S, A>(this Aff<A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Eff<bool>> pred) =>  
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, Eff<bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<S> FoldWhile<S, A>(this Aff<A> ma, S state, Func<S, A, Eff<S>> f, Func<S, Eff<bool>> pred) => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Eff<Env, A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Eff<bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Eff<Env, A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Eff<bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Eff<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Eff<bool>> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<S> FoldWhile<S, A>(this Eff<A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Eff<bool>> pred) => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<Env, A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, bool> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<Env, A> ma, S state, Func<S, A, Aff<S>> f, Func<S, bool> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<Env, A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, bool> pred) where Env : struct, HasCancel<Env> =>
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<Env, A> ma, S state, Func<S, A, Eff<S>> f, Func<S, bool> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Aff<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, bool> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<S> FoldWhile<S, A>(this Aff<A> ma, S state, Func<S, A, Aff<S>> f, Func<S, bool> pred) => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Eff<Env, A> ma, S state, Func<S, A, Aff<S>> f, Func<S, bool> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Eff<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, bool> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
 
        /// <summary>
        /// Folds over the provided IO computation `ma` while the `pred` operation returns `true` 
        /// </summary>
        /// <remarks>The `ma` operation has its state reset before each evaluation, allowing a different result
        /// each time its called.</remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <param name="state">Initial state value</param>
        /// <param name="f">Fold function</param>
        /// <param name="pred">Predicate</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Aff<Env, S> FoldWhile<Env, S, A>(this Eff<A> ma, S state, Func<S, A, Aff<S>> f, Func<S, bool> pred) where Env : struct, HasCancel<Env> => 
            Prelude.foldWhile(ma, state, f, pred);
   }
}
