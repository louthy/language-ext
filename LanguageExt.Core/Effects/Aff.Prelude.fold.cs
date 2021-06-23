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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.Run(env).ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run(env).ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.Run(env).ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run().ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.Run(env).ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).Run(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, Eff<S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.Run(env).ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).Run();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.Run(env).ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run(env).ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.Run(env).ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run().ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.Run(env).ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).Run(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<A> ma, S state, Func<S, A, Eff<S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.Run(env).ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).Run();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<Env, A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.Run(env).ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.Run(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run(env).ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<Env, A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.Run(env).ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.Run(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run().ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.Run(env).ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.Run();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run(env).ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.Run(env).ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.Run();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run().ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
        
        
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.Run(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run(env).ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.Run(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run().ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.Run(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).Run(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, Eff<S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.Run(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).Run();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.Run(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run(env).ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.Run(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run().ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.Run(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).Run(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<A> ma, S state, Func<S, A, Eff<S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.Run(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).Run();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<Env, A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.Run(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.Run(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run(env).ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<Env, A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.Run(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.Run(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run().ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.Run(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.Run();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run(env).ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.Run(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.Run();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run().ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Aff<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.Run().ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run(env).ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Aff<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.Run().ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run().ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, Aff<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.Run().ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).Run(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, Eff<S>> f, Func<S, Aff<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.Run().ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).Run();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Aff<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.Run().ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run(env).ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Aff<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.Run().ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run().ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, Aff<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.Run().ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).Run(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<A> ma, S state, Func<S, A, Eff<S>> f, Func<S, Aff<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.Run().ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).Run();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<Env, A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Aff<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.Run().ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.Run(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run(env).ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<Env, A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Aff<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.Run().ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.Run(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run().ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Aff<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.Run().ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.Run();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run(env).ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Aff<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.Run().ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.Run();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run().ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
        
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Eff<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.Run();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run(env).ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Eff<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.Run();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run().ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, Eff<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.Run();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).Run(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, Eff<S>> f, Func<S, Eff<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.Run();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).Run();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Eff<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.Run();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run(env).ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Eff<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.Run();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run().ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, Eff<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.Run();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).Run(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<A> ma, S state, Func<S, A, Eff<S>> f, Func<S, Eff<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.Run();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.Run().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).Run();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<Env, A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Eff<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.Run();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.Run(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run(env).ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<Env, A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Eff<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.Run();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.Run(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run().ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Eff<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.Run();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.Run();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run(env).ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<A> ma, S state, Func<S, A, Aff<S>> f, Func<S, Eff<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.Run();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.Run();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run().ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
         
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, bool> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.Clear();
                var a = await ma.Run(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run(env).ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, Aff<S>> f, Func<S, bool> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.Clear();
                var a = await ma.Run(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run().ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, bool> pred) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.Clear();
                var a = await ma.Run(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).Run(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, Eff<S>> f, Func<S, bool> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.Clear();
                var a = await ma.Run(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).Run();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, bool> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.Clear();
                var a = await ma.Run().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run(env).ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<A> ma, S state, Func<S, A, Aff<S>> f, Func<S, bool> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.Clear();
                var a = await ma.Run().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run().ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<Env, A> ma, S state, Func<S, A, Aff<S>> f, Func<S, bool> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.Clear();
                var a = ma.Run(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run().ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, bool> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.Clear();
                var a = ma.Run();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run(env).ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<A> ma, S state, Func<S, A, Aff<S>> f, Func<S, bool> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.Clear();
                var a = ma.Run();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).Run().ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
   }
}
