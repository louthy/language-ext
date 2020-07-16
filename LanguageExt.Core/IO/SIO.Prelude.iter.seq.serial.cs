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
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iter<Env, A>(SIO<Env, Seq<A>> ma, Func<A, IO<Env, Unit>> f) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var x in xs.Value)
                {
                    var r = await f(x).RunIO(env);
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });

        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iter<Env, A>(SIO<Env, Seq<A>> ma, Func<A, IO<Unit>> f) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var x in xs.Value)
                {
                    var r = await f(x).RunIO();
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static SIO<Env, Unit> iter<Env, A>(SIO<Env, Seq<A>> ma, Func<A, SIO<Env, Unit>> f) where Env : Cancellable =>
            SIO<Env,Unit>.EffectMaybe(env =>
            {
                var xs = ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var x in xs.Value)
                {
                    var r = f(x).RunIO(env);
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static SIO<Env, Unit> iter<Env, A>(SIO<Env, Seq<A>> ma, Func<A, SIO<Unit>> f) where Env : Cancellable =>
            SIO<Env,Unit>.EffectMaybe(env =>
            {
                var xs = ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var x in xs.Value)
                {
                    var r = f(x).RunIO();
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
        
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iter<Env, A>(SIO<Seq<A>> ma, Func<A, IO<Env, Unit>> f) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var x in xs.Value)
                {
                    var r = await f(x).RunIO(env);
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });

        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Unit> iter<A>(SIO<Seq<A>> ma, Func<A, IO<Unit>> f) =>
            IO<Unit>.EffectMaybe(async () =>
            {
                var xs = ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var x in xs.Value)
                {
                    var r = await f(x).RunIO();
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static SIO<Env, Unit> iter<Env, A>(SIO<Seq<A>> ma, Func<A, SIO<Env, Unit>> f) where Env : Cancellable =>
            SIO<Env,Unit>.EffectMaybe(env =>
            {
                var xs = ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var x in xs.Value)
                {
                    var r = f(x).RunIO(env);
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static SIO<Unit> iter<A>(SIO<Seq<A>> ma, Func<A, SIO<Unit>> f) =>
            SIO<Unit>.EffectMaybe(() =>
            {
                var xs = ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var x in xs.Value)
                {
                    var r = f(x).RunIO();
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
        

        
        
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iter<Env, A>(SIO<Env, Seq<IO<Env, A>>> ma, Func<A, IO<Env, Unit>> f) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = await iox.RunIO(env);
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = await f(x.Value).RunIO(env);
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });

        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iter<Env, A>(SIO<Env, Seq<IO<Env, A>>> ma, Func<A, IO<Unit>> f) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = await iox.RunIO(env);
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = await f(x.Value).RunIO();
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iter<Env, A>(SIO<Env, Seq<IO<Env, A>>> ma, Func<A, SIO<Env, Unit>> f) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = await iox.RunIO(env);
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = f(x.Value).RunIO(env);
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iter<Env, A>(SIO<Env, Seq<IO<Env, A>>> ma, Func<A, SIO<Unit>> f) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = await iox.RunIO(env);
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = f(x.Value).RunIO();
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
        
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iter<Env, A>(SIO<Seq<IO<Env, A>>> ma, Func<A, IO<Env, Unit>> f) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = await iox.RunIO(env);
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = await f(x.Value).RunIO(env);
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });

        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iter<Env, A>(SIO<Seq<IO<Env, A>>> ma, Func<A, IO<Unit>> f) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = await iox.RunIO(env);
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = await f(x.Value).RunIO();
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iter<Env, A>(SIO<Seq<IO<Env, A>>> ma, Func<A, SIO<Env, Unit>> f) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = await iox.RunIO(env);
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = f(x.Value).RunIO(env);
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iter<Env, A>(SIO<Seq<IO<Env, A>>> ma, Func<A, SIO<Unit>> f) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = await iox.RunIO(env);
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = f(x.Value).RunIO();
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });        
         

        
        
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iter<Env, A>(SIO<Env, Seq<IO<A>>> ma, Func<A, IO<Env, Unit>> f) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = await iox.RunIO();
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = await f(x.Value).RunIO(env);
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });

        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iter<Env, A>(SIO<Env, Seq<IO<A>>> ma, Func<A, IO<Unit>> f) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = await iox.RunIO();
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = await f(x.Value).RunIO();
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iter<Env, A>(SIO<Env, Seq<IO<A>>> ma, Func<A, SIO<Env, Unit>> f) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = await iox.RunIO();
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = f(x.Value).RunIO(env);
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iter<Env, A>(SIO<Env, Seq<IO<A>>> ma, Func<A, SIO<Unit>> f) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = await iox.RunIO();
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = f(x.Value).RunIO();
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
        
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iter<Env, A>(SIO<Seq<IO<A>>> ma, Func<A, IO<Env, Unit>> f) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = await iox.RunIO();
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = await f(x.Value).RunIO(env);
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });

        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Unit> iter<A>(SIO<Seq<IO<A>>> ma, Func<A, IO<Unit>> f) =>
            IO<Unit>.EffectMaybe(async () =>
            {
                var xs = ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = await iox.RunIO();
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = await f(x.Value).RunIO();
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iter<Env, A>(SIO<Seq<IO<A>>> ma, Func<A, SIO<Env, Unit>> f) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = await iox.RunIO();
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = f(x.Value).RunIO(env);
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Unit> iter<A>(SIO<Seq<IO<A>>> ma, Func<A, SIO<Unit>> f) =>
            IO<Unit>.EffectMaybe(async () =>
            {
                var xs = ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = await iox.RunIO();
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = f(x.Value).RunIO();
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });   
    
    
    
    
            
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iter<Env, A>(SIO<Env, Seq<SIO<Env, A>>> ma, Func<A, IO<Env, Unit>> f) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = iox.RunIO(env);
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = await f(x.Value).RunIO(env);
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });

        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iter<Env, A>(SIO<Env, Seq<SIO<Env, A>>> ma, Func<A, IO<Unit>> f) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = iox.RunIO(env);
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = await f(x.Value).RunIO();
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static SIO<Env, Unit> iter<Env, A>(SIO<Env, Seq<SIO<Env, A>>> ma, Func<A, SIO<Env, Unit>> f) where Env : Cancellable =>
            SIO<Env,Unit>.EffectMaybe(env =>
            {
                var xs = ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = iox.RunIO(env);
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = f(x.Value).RunIO(env);
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static SIO<Env, Unit> iter<Env, A>(SIO<Env, Seq<SIO<Env, A>>> ma, Func<A, SIO<Unit>> f) where Env : Cancellable =>
            SIO<Env,Unit>.EffectMaybe(env =>
            {
                var xs = ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = iox.RunIO(env);
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = f(x.Value).RunIO();
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
        
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iter<Env, A>(SIO<Seq<SIO<Env, A>>> ma, Func<A, IO<Env, Unit>> f) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = iox.RunIO(env);
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = await f(x.Value).RunIO(env);
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });

        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iter<Env, A>(SIO<Seq<SIO<Env, A>>> ma, Func<A, IO<Unit>> f) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = iox.RunIO(env);
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = await f(x.Value).RunIO();
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static SIO<Env, Unit> iter<Env, A>(SIO<Seq<SIO<Env, A>>> ma, Func<A, SIO<Env, Unit>> f) where Env : Cancellable =>
            SIO<Env,Unit>.EffectMaybe(env =>
            {
                var xs = ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = iox.RunIO(env);
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = f(x.Value).RunIO(env);
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static SIO<Env, Unit> iter<Env, A>(SIO<Seq<SIO<Env, A>>> ma, Func<A, SIO<Unit>> f) where Env : Cancellable =>
            SIO<Env,Unit>.EffectMaybe(env =>
            {
                var xs = ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = iox.RunIO(env);
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = f(x.Value).RunIO();
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });        
         
    
        
 
    
            
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iter<Env, A>(SIO<Env, Seq<SIO<A>>> ma, Func<A, IO<Env, Unit>> f) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = iox.RunIO();
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = await f(x.Value).RunIO(env);
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });

        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iter<Env, A>(SIO<Env, Seq<SIO<A>>> ma, Func<A, IO<Unit>> f) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = iox.RunIO();
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = await f(x.Value).RunIO();
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static SIO<Env, Unit> iter<Env, A>(SIO<Env, Seq<SIO<A>>> ma, Func<A, SIO<Env, Unit>> f) where Env : Cancellable =>
            SIO<Env,Unit>.EffectMaybe(env =>
            {
                var xs = ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = iox.RunIO();
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = f(x.Value).RunIO(env);
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static SIO<Env, Unit> iter<Env, A>(SIO<Env, Seq<SIO<A>>> ma, Func<A, SIO<Unit>> f) where Env : Cancellable =>
            SIO<Env,Unit>.EffectMaybe(env =>
            {
                var xs = ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = iox.RunIO();
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = f(x.Value).RunIO();
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
        
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iter<Env, A>(SIO<Seq<SIO<A>>> ma, Func<A, IO<Env, Unit>> f) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = iox.RunIO();
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = await f(x.Value).RunIO(env);
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });

        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static IO<Unit> iter<A>(SIO<Seq<SIO<A>>> ma, Func<A, IO<Unit>> f) =>
            IO<Unit>.EffectMaybe(async () =>
            {
                var xs = ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = iox.RunIO();
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = await f(x.Value).RunIO();
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static SIO<Env, Unit> iter<Env, A>(SIO<Seq<SIO<A>>> ma, Func<A, SIO<Env, Unit>> f) where Env : Cancellable =>
            SIO<Env,Unit>.EffectMaybe(env =>
            {
                var xs = ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = iox.RunIO();
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = f(x.Value).RunIO(env);
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static SIO<Unit> iter<A>(SIO<Seq<SIO<A>>> ma, Func<A, SIO<Unit>> f) =>
            SIO<Unit>.EffectMaybe(() =>
            {
                var xs = ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                foreach (var iox in xs.Value)
                {
                    var x = iox.RunIO();
                    if (x.IsFail) return x.Cast<Unit>();
                    var r = f(x.Value).RunIO();
                    if (r.IsFail) return r.Cast<Unit>();
                }

                return Fin<Unit>.Succ(default);
            });        
        
    }
}