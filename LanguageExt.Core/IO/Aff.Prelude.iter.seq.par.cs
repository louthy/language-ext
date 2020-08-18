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
    public static partial class Prelude
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
        public static Aff<Env, Unit> iterParallel<Env, A>(Aff<Env, Seq<A>> ma, Func<A, Aff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO(env).ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => f(x).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(Aff<Env, Seq<A>> ma, Func<A, AffPure<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO(env).ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                    .Map(x => f(x).RunIO().AsTask())
                    .SequenceParallel(Parallelism(n))
                    .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(AffPure<Seq<A>> ma, Func<A, Aff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO().ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                    .Map(x => f(x).RunIO(env).AsTask())
                    .SequenceParallel(Parallelism(n))
                    .ConfigureAwait(false);

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
        public static AffPure<Unit> iterParallel<A>(AffPure<Seq<A>> ma, Func<A, AffPure<Unit>> f, int n = -1) =>
            AffMaybe(async () =>
            {
                var xs = await ma.RunIO().ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                    .Map(x => f(x).RunIO().AsTask())
                    .SequenceParallel(Parallelism(n))
                    .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(Aff<Env, Seq<Aff<Env, A>>> ma, Func<A, Aff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO(env).ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(Aff<Env, Seq<Aff<Env, A>>> ma, Func<A, AffPure<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO(env).ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(Aff<Env, Seq<Aff<Env, A>>> ma, Func<A, Eff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO(env).ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(Aff<Env, Seq<Aff<Env, A>>> ma, Func<A, EffPure<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO(env).ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(AffPure<Seq<Aff<Env, A>>> ma, Func<A, Aff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO().ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(AffPure<Seq<Aff<Env, A>>> ma, Func<A, AffPure<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO().ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(AffPure<Seq<Aff<Env, A>>> ma, Func<A, Eff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO().ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(AffPure<Seq<Aff<Env, A>>> ma, Func<A, EffPure<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO().ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(Aff<Env, Seq<AffPure<A>>> ma, Func<A, Aff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO(env).ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(Aff<Env, Seq<AffPure<A>>> ma, Func<A, AffPure<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO(env).ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO().AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(Aff<Env, Seq<AffPure<A>>> ma, Func<A, Eff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO(env).ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(Aff<Env, Seq<AffPure<A>>> ma, Func<A, EffPure<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO(env).ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO().AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(AffPure<Seq<AffPure<A>>> ma, Func<A, Aff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO().ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static AffPure<Unit> iterParallel<A>(AffPure<Seq<AffPure<A>>> ma, Func<A, AffPure<Unit>> f, int n = -1) =>
            AffMaybe(async () =>
            {
                var xs = await ma.RunIO().ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO().AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(AffPure<Seq<AffPure<A>>> ma, Func<A, Eff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO().ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static AffPure<Unit> iterParallel<A>(AffPure<Seq<AffPure<A>>> ma, Func<A, EffPure<Unit>> f, int n = -1) =>
            AffMaybe(async () =>
            {
                var xs = await ma.RunIO().ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO().AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(Aff<Env, Seq<Eff<Env, A>>> ma, Func<A, Aff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO(env).ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(Aff<Env, Seq<Eff<Env, A>>> ma, Func<A, AffPure<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO(env).ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(Aff<Env, Seq<Eff<Env, A>>> ma, Func<A, Eff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO(env).ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(Aff<Env, Seq<Eff<Env, A>>> ma, Func<A, EffPure<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO(env).ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(AffPure<Seq<Eff<Env, A>>> ma, Func<A, Aff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO().ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(AffPure<Seq<Eff<Env, A>>> ma, Func<A, AffPure<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO().ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(AffPure<Seq<Eff<Env, A>>> ma, Func<A, Eff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO().ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(AffPure<Seq<Eff<Env, A>>> ma, Func<A, EffPure<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO().ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(Aff<Env, Seq<EffPure<A>>> ma, Func<A, Aff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO(env).ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(Aff<Env, Seq<EffPure<A>>> ma, Func<A, AffPure<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO(env).ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO().AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(Aff<Env, Seq<EffPure<A>>> ma, Func<A, Eff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO(env).ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(Aff<Env, Seq<EffPure<A>>> ma, Func<A, EffPure<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO(env).ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO().AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(AffPure<Seq<EffPure<A>>> ma, Func<A, Aff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO().ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                .Map(x => x.Bind(f).RunIO(env).AsTask())
                                .SequenceParallel(Parallelism(n))
                                .ConfigureAwait(false);

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
        public static AffPure<Unit> iterParallel<A>(AffPure<Seq<EffPure<A>>> ma, Func<A, AffPure<Unit>> f, int n = -1) =>
            AffMaybe(async () =>
            {
                var xs = await ma.RunIO().ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO().AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static Aff<Env, Unit> iterParallel<Env, A>(AffPure<Seq<EffPure<A>>> ma, Func<A, Eff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
            {
                var xs = await ma.RunIO().ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO(env).AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

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
        public static AffPure<Unit> iterParallel<A>(AffPure<Seq<EffPure<A>>> ma, Func<A, EffPure<Unit>> f, int n = -1) =>
            AffMaybe(async () =>
            {
                var xs = await ma.RunIO().ConfigureAwait(false);
                if (xs.IsFail) return xs.Cast<Unit>();

                var results = await xs.Value
                                      .Map(x => x.Bind(f).RunIO().AsTask())
                                      .SequenceParallel(Parallelism(n))
                                      .ConfigureAwait(false);

                var err = results.Filter(r => r.IsFail).HeadOrNone();
                return err.IfNone(Fin<Unit>.Succ(default)); 
            });        
        
    }
}
