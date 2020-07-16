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
    public static partial class IO
    {
        public static IO<Env, S> foldWhile<Env, S, A>(IO<Env, A> ma, S state, Func<S, A, IO<Env, S>> f, Func<S, IO<Env, bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = await ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<Env, A> ma, S state, Func<S, A, IO<S>> f, Func<S, IO<Env, bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = await ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<Env, A> ma, S state, Func<S, A, SIO<Env, S>> f, Func<S, IO<Env, bool>> pred) where Env : Cancellable =>
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = await ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<Env, A> ma, S state, Func<S, A, SIO<S>> f, Func<S, IO<Env, bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = await ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<A> ma, S state, Func<S, A, IO<Env, S>> f, Func<S, IO<Env, bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = await ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<A> ma, S state, Func<S, A, IO<S>> f, Func<S, IO<Env, bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = await ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<A> ma, S state, Func<S, A, SIO<Env, S>> f, Func<S, IO<Env, bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = await ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<A> ma, S state, Func<S, A, SIO<S>> f, Func<S, IO<Env, bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = await ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(SIO<Env, A> ma, S state, Func<S, A, IO<Env, S>> f, Func<S, IO<Env, bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = await ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(SIO<Env, A> ma, S state, Func<S, A, IO<S>> f, Func<S, IO<Env, bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = await ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(SIO<A> ma, S state, Func<S, A, IO<Env, S>> f, Func<S, IO<Env, bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = await ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(SIO<A> ma, S state, Func<S, A, IO<S>> f, Func<S, IO<Env, bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = await ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
        
        
        
        
        
        
        
        public static IO<Env, S> foldWhile<Env, S, A>(IO<Env, A> ma, S state, Func<S, A, IO<Env, S>> f, Func<S, SIO<Env, bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<Env, A> ma, S state, Func<S, A, IO<S>> f, Func<S, SIO<Env, bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<Env, A> ma, S state, Func<S, A, SIO<Env, S>> f, Func<S, SIO<Env, bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<Env, A> ma, S state, Func<S, A, SIO<S>> f, Func<S, SIO<Env, bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<A> ma, S state, Func<S, A, IO<Env, S>> f, Func<S, SIO<Env, bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<A> ma, S state, Func<S, A, IO<S>> f, Func<S, SIO<Env, bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<A> ma, S state, Func<S, A, SIO<Env, S>> f, Func<S, SIO<Env, bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<A> ma, S state, Func<S, A, SIO<S>> f, Func<S, SIO<Env, bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(SIO<Env, A> ma, S state, Func<S, A, IO<Env, S>> f, Func<S, SIO<Env, bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(SIO<Env, A> ma, S state, Func<S, A, IO<S>> f, Func<S, SIO<Env, bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(SIO<A> ma, S state, Func<S, A, IO<Env, S>> f, Func<S, SIO<Env, bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(SIO<A> ma, S state, Func<S, A, IO<S>> f, Func<S, SIO<Env, bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = ioCont.RunIO(env);
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        
        
        
        
        
        
        
        
        public static IO<Env, S> foldWhile<Env, S, A>(IO<Env, A> ma, S state, Func<S, A, IO<Env, S>> f, Func<S, IO<bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = await ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<Env, A> ma, S state, Func<S, A, IO<S>> f, Func<S, IO<bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = await ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<Env, A> ma, S state, Func<S, A, SIO<Env, S>> f, Func<S, IO<bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = await ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<Env, A> ma, S state, Func<S, A, SIO<S>> f, Func<S, IO<bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = await ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<A> ma, S state, Func<S, A, IO<Env, S>> f, Func<S, IO<bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = await ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<A> ma, S state, Func<S, A, IO<S>> f, Func<S, IO<bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = await ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<A> ma, S state, Func<S, A, SIO<Env, S>> f, Func<S, IO<bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = await ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<A> ma, S state, Func<S, A, SIO<S>> f, Func<S, IO<bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = await ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(SIO<Env, A> ma, S state, Func<S, A, IO<Env, S>> f, Func<S, IO<bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = await ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(SIO<Env, A> ma, S state, Func<S, A, IO<S>> f, Func<S, IO<bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = await ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(SIO<A> ma, S state, Func<S, A, IO<Env, S>> f, Func<S, IO<bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = await ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(SIO<A> ma, S state, Func<S, A, IO<S>> f, Func<S, IO<bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = await ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
        
        
        
        
        
        
        
        public static IO<Env, S> foldWhile<Env, S, A>(IO<Env, A> ma, S state, Func<S, A, IO<Env, S>> f, Func<S, SIO<bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<Env, A> ma, S state, Func<S, A, IO<S>> f, Func<S, SIO<bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<Env, A> ma, S state, Func<S, A, SIO<Env, S>> f, Func<S, SIO<bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<Env, A> ma, S state, Func<S, A, SIO<S>> f, Func<S, SIO<bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<A> ma, S state, Func<S, A, IO<Env, S>> f, Func<S, SIO<bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<A> ma, S state, Func<S, A, IO<S>> f, Func<S, SIO<bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<A> ma, S state, Func<S, A, SIO<Env, S>> f, Func<S, SIO<bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<A> ma, S state, Func<S, A, SIO<S>> f, Func<S, SIO<bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = await ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(SIO<Env, A> ma, S state, Func<S, A, IO<Env, S>> f, Func<S, SIO<bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(SIO<Env, A> ma, S state, Func<S, A, IO<S>> f, Func<S, SIO<bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(SIO<A> ma, S state, Func<S, A, IO<Env, S>> f, Func<S, SIO<bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(SIO<A> ma, S state, Func<S, A, IO<S>> f, Func<S, SIO<bool>> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                var ioCont = pred(state);
                ioCont.thunk.Flush();
                var cont = ioCont.RunIO();
                if (cont.IsFail) return cont.Cast<S>();
                if (!cont.Value) return state;
                
                ma.thunk.Flush();
                var a = ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
         
 
         public static IO<Env, S> foldWhile<Env, S, A>(IO<Env, A> ma, S state, Func<S, A, IO<Env, S>> f, Func<S, bool> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.thunk.Flush();
                var a = await ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<Env, A> ma, S state, Func<S, A, IO<S>> f, Func<S, bool> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.thunk.Flush();
                var a = await ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<Env, A> ma, S state, Func<S, A, SIO<Env, S>> f, Func<S, bool> pred) where Env : Cancellable =>
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.thunk.Flush();
                var a = await ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<Env, A> ma, S state, Func<S, A, SIO<S>> f, Func<S, bool> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.thunk.Flush();
                var a = await ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<A> ma, S state, Func<S, A, IO<Env, S>> f, Func<S, bool> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.thunk.Flush();
                var a = await ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(IO<A> ma, S state, Func<S, A, IO<S>> f, Func<S, bool> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.thunk.Flush();
                var a = await ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(SIO<Env, A> ma, S state, Func<S, A, IO<S>> f, Func<S, bool> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.thunk.Flush();
                var a = ma.RunIO(env);
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(SIO<A> ma, S state, Func<S, A, IO<Env, S>> f, Func<S, bool> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.thunk.Flush();
                var a = ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO(env);
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
 
        public static IO<Env, S> foldWhile<Env, S, A>(SIO<A> ma, S state, Func<S, A, IO<S>> f, Func<S, bool> pred) where Env : Cancellable => 
            EffectMaybe<Env, S>(async env =>
        {
            while (true)
            {
                if (!pred(state)) return state;
                ma.thunk.Flush();
                var a = ma.RunIO();
                if (a.IsFail) return a.Cast<S>();
                var iostate = await f(state, a.Value).RunIO();
                if (iostate.IsFail) return iostate;
                state = iostate.Value;
            }
        });
   }
}