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
        public static Eff<Env, S> foldWhile<Env, S, A>(Eff<Env, A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, Eff<Env, bool>> pred) => 
            EffMaybe<Env, S>(env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                var cont = ioCont.ReRun(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                var a = ma.ReRun(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).ReRun(env);
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
        public static Eff<Env, S> foldWhile<Env, S, A>(Eff<Env, A> ma, S state, Func<S, A, Eff<S>> f, Func<S, Eff<Env, bool>> pred) => 
            EffMaybe<Env, S>(env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                var cont = ioCont.ReRun(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                var a = ma.ReRun(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).ReRun();
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
        public static Eff<Env, S> foldWhile<Env, S, A>(Eff<Env, A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, Eff<bool>> pred) => 
            EffMaybe<Env, S>(env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                var cont = ioCont.ReRun();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                var a = ma.ReRun(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).ReRun(env);
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
        public static Eff<Env, S> foldWhile<Env, S, A>(Eff<Env, A> ma, S state, Func<S, A, Eff<S>> f, Func<S, Eff<bool>> pred) => 
            EffMaybe<Env, S>(env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                var cont = ioCont.ReRun();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                var a = ma.ReRun(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).ReRun();
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
        public static Eff<Env, S> foldWhile<Env, S, A>(Eff<A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, Eff<Env, bool>> pred) => 
            EffMaybe<Env, S>(env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                var cont = ioCont.ReRun(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                var a = ma.ReRun();
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).ReRun(env);
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
        public static Eff<Env, S> foldWhile<Env, S, A>(Eff<A> ma, S state, Func<S, A, Eff<S>> f, Func<S, Eff<Env, bool>> pred) => 
            EffMaybe<Env, S>(env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                var cont = ioCont.ReRun(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                var a = ma.ReRun();
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).ReRun();
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
        public static Eff<Env, S> foldWhile<Env, S, A>(Eff<A> ma, S state, Func<S, A, Eff<Env, S>> f, Func<S, Eff<bool>> pred) => 
            EffMaybe<Env, S>(env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                var cont = ioCont.ReRun();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                var a = ma.ReRun();
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).ReRun(env);
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
        /// <typeparam name="S">State value type</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Aggregated state value</returns>
        public static Eff<S> foldWhile<Env, S, A>(Eff<A> ma, S state, Func<S, A, Eff<S>> f, Func<S, Eff<bool>> pred) => 
            EffMaybe<S>(() =>
        {
            while (true)
            {
                var ioCont = pred(state);
                var cont = ioCont.ReRun();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                var a = ma.ReRun();
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).ReRun();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
    }
}
