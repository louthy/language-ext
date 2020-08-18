using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Interfaces;
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
                var cont = await ioCont.RunIO(env).ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env).ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, AffPure<S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.RunIO(env).ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO().ConfigureAwait(false);
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
                var cont = await ioCont.RunIO(env).ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO(env);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, EffPure<S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.RunIO(env).ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO();
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
        public static Aff<Env, S> foldWhile<Env, S, A>(AffPure<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.RunIO(env).ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env).ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(AffPure<A> ma, S state, Func<S, A, AffPure<S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.RunIO(env).ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO().ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(AffPure<A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.RunIO(env).ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO(env);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(AffPure<A> ma, S state, Func<S, A, EffPure<S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.RunIO(env).ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO();
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
                var cont = await ioCont.RunIO(env).ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env).ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<Env, A> ma, S state, Func<S, A, AffPure<S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.RunIO(env).ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO().ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(EffPure<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.RunIO(env).ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env).ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(EffPure<A> ma, S state, Func<S, A, AffPure<S>> f, Func<S, Aff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.RunIO(env).ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO().ConfigureAwait(false);
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
                var cont = ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env).ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, AffPure<S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO().ConfigureAwait(false);
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
                var cont = ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO(env);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, EffPure<S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO();
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
        public static Aff<Env, S> foldWhile<Env, S, A>(AffPure<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env).ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(AffPure<A> ma, S state, Func<S, A, AffPure<S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO().ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(AffPure<A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO(env);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(AffPure<A> ma, S state, Func<S, A, EffPure<S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO();
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
                var cont = ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env).ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<Env, A> ma, S state, Func<S, A, AffPure<S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO().ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(EffPure<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env).ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(EffPure<A> ma, S state, Func<S, A, AffPure<S>> f, Func<S, Eff<Env, bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO().ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, AffPure<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.RunIO().ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env).ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, AffPure<S>> f, Func<S, AffPure<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.RunIO().ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO().ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, AffPure<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.RunIO().ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO(env);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, EffPure<S>> f, Func<S, AffPure<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.RunIO().ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO();
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
        public static Aff<Env, S> foldWhile<Env, S, A>(AffPure<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, AffPure<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.RunIO().ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env).ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(AffPure<A> ma, S state, Func<S, A, AffPure<S>> f, Func<S, AffPure<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.RunIO().ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO().ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(AffPure<A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, AffPure<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.RunIO().ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO(env);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(AffPure<A> ma, S state, Func<S, A, EffPure<S>> f, Func<S, AffPure<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.RunIO().ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO();
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<Env, A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, AffPure<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.RunIO().ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env).ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<Env, A> ma, S state, Func<S, A, AffPure<S>> f, Func<S, AffPure<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.RunIO().ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO().ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(EffPure<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, AffPure<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.RunIO().ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env).ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(EffPure<A> ma, S state, Func<S, A, AffPure<S>> f, Func<S, AffPure<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = await ioCont.RunIO().ConfigureAwait(false);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO().ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, EffPure<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env).ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, AffPure<S>> f, Func<S, EffPure<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO().ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, EffPure<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO(env);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, EffPure<S>> f, Func<S, EffPure<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO();
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
        public static Aff<Env, S> foldWhile<Env, S, A>(AffPure<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, EffPure<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env).ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(AffPure<A> ma, S state, Func<S, A, AffPure<S>> f, Func<S, EffPure<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO().ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(AffPure<A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, EffPure<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO(env);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(AffPure<A> ma, S state, Func<S, A, EffPure<S>> f, Func<S, EffPure<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = await ma.RunIO().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO();
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<Env, A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, EffPure<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env).ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<Env, A> ma, S state, Func<S, A, AffPure<S>> f, Func<S, EffPure<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO().ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(EffPure<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, EffPure<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env).ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(EffPure<A> ma, S state, Func<S, A, AffPure<S>> f, Func<S, EffPure<bool>> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.Clear();
                var cont = ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.Clear();
                var a = ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO().ConfigureAwait(false);
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
                var a = await ma.RunIO(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env).ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, AffPure<S>> f, Func<S, bool> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.Clear();
                var a = await ma.RunIO(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO().ConfigureAwait(false);
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
                var a = await ma.RunIO(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO(env);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Aff<Env, A> ma, S state, Func<S, A, EffPure<S>> f, Func<S, bool> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.Clear();
                var a = await ma.RunIO(env).ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO();
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
        public static Aff<Env, S> foldWhile<Env, S, A>(AffPure<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, bool> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.Clear();
                var a = await ma.RunIO().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env).ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(AffPure<A> ma, S state, Func<S, A, AffPure<S>> f, Func<S, bool> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.Clear();
                var a = await ma.RunIO().ConfigureAwait(false);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO().ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(Eff<Env, A> ma, S state, Func<S, A, AffPure<S>> f, Func<S, bool> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.Clear();
                var a = ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO().ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(EffPure<A> ma, S state, Func<S, A, Aff<Env, S>> f, Func<S, bool> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.Clear();
                var a = ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env).ConfigureAwait(false);
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
        public static Aff<Env, S> foldWhile<Env, S, A>(EffPure<A> ma, S state, Func<S, A, AffPure<S>> f, Func<S, bool> pred) where Env : struct, HasCancel<Env> => 
            AffMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.Clear();
                var a = ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO().ConfigureAwait(false);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
   }
}
