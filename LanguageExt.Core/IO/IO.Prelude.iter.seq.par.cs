using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Interfaces;
using LanguageExt.Thunks;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class IO
    {
        [Pure, MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int Parallelism(int n) =>
            n < 1
                ? Sys.DefaultAsyncSequenceConcurrency
                : n;
        
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Env, Seq<A>> ma, Func<A, IO<Env, Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => f(x).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail)
                                 .HeadOrNone();

                return err.IfNone(Fin<Unit>.Succ(default)); 
            });

        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Env, Seq<A>> ma, Func<A, IO<Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                    .Map(x => f(x).RunIO().AsTask())
                    .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail)
                    .HeadOrNone();

                return err.IfNone(Fin<Unit>.Succ(default)); 
            });
        
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Seq<A>> ma, Func<A, IO<Env, Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                    .Map(x => f(x).RunIO(env).AsTask())
                    .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail)
                    .HeadOrNone();

                return err.IfNone(Fin<Unit>.Succ(default)); 
            });

        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Unit> iterPar<A>(IO<Seq<A>> ma, Func<A, IO<Unit>> f, int n = -1) =>
            IO<Unit>.EffectMaybe(async () =>
            {
                var xs = await ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                    .Map(x => f(x).RunIO().AsTask())
                    .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail)
                    .HeadOrNone();

                return err.IfNone(Fin<Unit>.Succ(default)); 
            });

        
        
        
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Env, Seq<IO<Env, A>>> ma, Func<A, IO<Env, Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 
            });

        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Env, Seq<IO<Env, A>>> ma, Func<A, IO<Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 
            });
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Env, Seq<IO<Env, A>>> ma, Func<A, SIO<Env, Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 
           });
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Env, Seq<IO<Env, A>>> ma, Func<A, SIO<Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 
            });
        
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Seq<IO<Env, A>>> ma, Func<A, IO<Env, Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 
            });

        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Seq<IO<Env, A>>> ma, Func<A, IO<Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 
            });
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Seq<IO<Env, A>>> ma, Func<A, SIO<Env, Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 
            });
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Seq<IO<Env, A>>> ma, Func<A, SIO<Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 
            });        
         

        
        
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Env, Seq<IO<A>>> ma, Func<A, IO<Env, Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 
            });

        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Env, Seq<IO<A>>> ma, Func<A, IO<Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO().AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 

            });
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Env, Seq<IO<A>>> ma, Func<A, SIO<Env, Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 

            });
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Env, Seq<IO<A>>> ma, Func<A, SIO<Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO().AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 

            });
        
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Seq<IO<A>>> ma, Func<A, IO<Env, Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 

            });

        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Unit> iterPar<A>(IO<Seq<IO<A>>> ma, Func<A, IO<Unit>> f, int n = -1) =>
            IO<Unit>.EffectMaybe(async () =>
            {
                var xs = await ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO().AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 

            });
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Seq<IO<A>>> ma, Func<A, SIO<Env, Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 

            });
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Unit> iterPar<A>(IO<Seq<IO<A>>> ma, Func<A, SIO<Unit>> f, int n = -1) =>
            IO<Unit>.EffectMaybe(async () =>
            {
                var xs = await ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO().AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 

            });   
    
    
    
    
            
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Env, Seq<SIO<Env, A>>> ma, Func<A, IO<Env, Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 
            });

        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Env, Seq<SIO<Env, A>>> ma, Func<A, IO<Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 

            });
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Env, Seq<SIO<Env, A>>> ma, Func<A, SIO<Env, Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 

            });
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Env, Seq<SIO<Env, A>>> ma, Func<A, SIO<Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 

            });
        
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Seq<SIO<Env, A>>> ma, Func<A, IO<Env, Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 

            });

        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Seq<SIO<Env, A>>> ma, Func<A, IO<Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 

            });
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Seq<SIO<Env, A>>> ma, Func<A, SIO<Env, Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 

            });
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Seq<SIO<Env, A>>> ma, Func<A, SIO<Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 

            });        
         
    
        
 
    
            
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Env, Seq<SIO<A>>> ma, Func<A, IO<Env, Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 

            });

        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Env, Seq<SIO<A>>> ma, Func<A, IO<Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO().AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 

            });
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Env, Seq<SIO<A>>> ma, Func<A, SIO<Env, Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 

            });
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Env, Seq<SIO<A>>> ma, Func<A, SIO<Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO(env);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO().AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 

            });
        
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Seq<SIO<A>>> ma, Func<A, IO<Env, Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 

            });

        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Unit> iterPar<A>(IO<Seq<SIO<A>>> ma, Func<A, IO<Unit>> f, int n = -1) =>
            IO<Unit>.EffectMaybe(async () =>
            {
                var xs = await ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO().AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 

            });
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Env, Unit> iterPar<Env, A>(IO<Seq<SIO<A>>> ma, Func<A, SIO<Env, Unit>> f, int n = -1) where Env : Cancellable =>
            IO<Env,Unit>.EffectMaybe(async env =>
            {
                var xs = await ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 

            });
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, used `Sys.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static IO<Unit> iterPar<A>(IO<Seq<SIO<A>>> ma, Func<A, SIO<Unit>> f, int n = -1) =>
            IO<Unit>.EffectMaybe(async () =>
            {
                var xs = await ma.RunIO();
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO().AsTask())
                                      .SequenceParallel(Parallelism(n));

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 
            });        
        
    }
}