using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.Thunks;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Env, Seq<A>> ma, Func<A, Aff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);

        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Env, Seq<A>> ma, Func<A, Aff<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);
        
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Seq<A>> ma, Func<A, Aff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);

        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Unit> IterParallel<A>(this Aff<Seq<A>> ma, Func<A, Aff<Unit>> f, int n = -1) =>
            Prelude.iterParallel(ma, f, n);
        
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Env, Seq<Aff<Env, A>>> ma, Func<A, Aff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);

        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Env, Seq<Aff<Env, A>>> ma, Func<A, Aff<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Env, Seq<Aff<Env, A>>> ma, Func<A, Eff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Env, Seq<Aff<Env, A>>> ma, Func<A, Eff<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);
        
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Seq<Aff<Env, A>>> ma, Func<A, Aff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);

        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Seq<Aff<Env, A>>> ma, Func<A, Aff<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Seq<Aff<Env, A>>> ma, Func<A, Eff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Seq<Aff<Env, A>>> ma, Func<A, Eff<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);
        
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Env, Seq<Aff<A>>> ma, Func<A, Aff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);

        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Env, Seq<Aff<A>>> ma, Func<A, Aff<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Env, Seq<Aff<A>>> ma, Func<A, Eff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Env, Seq<Aff<A>>> ma, Func<A, Eff<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);
        
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Seq<Aff<A>>> ma, Func<A, Aff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);

        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Unit> IterParallel<A>(this Aff<Seq<Aff<A>>> ma, Func<A, Aff<Unit>> f, int n = -1) =>
            Prelude.iterParallel(ma, f, n);
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Seq<Aff<A>>> ma, Func<A, Eff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Unit> IterParallel<A>(this Aff<Seq<Aff<A>>> ma, Func<A, Eff<Unit>> f, int n = -1) =>
            Prelude.iterParallel(ma, f, n);
            
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Env, Seq<Eff<Env, A>>> ma, Func<A, Aff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);

        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Env, Seq<Eff<Env, A>>> ma, Func<A, Aff<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Env, Seq<Eff<Env, A>>> ma, Func<A, Eff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Env, Seq<Eff<Env, A>>> ma, Func<A, Eff<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);
        
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Seq<Eff<Env, A>>> ma, Func<A, Aff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);

        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Seq<Eff<Env, A>>> ma, Func<A, Aff<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Seq<Eff<Env, A>>> ma, Func<A, Eff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Seq<Eff<Env, A>>> ma, Func<A, Eff<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);
            
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Env, Seq<Eff<A>>> ma, Func<A, Aff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);

        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Env, Seq<Eff<A>>> ma, Func<A, Aff<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Env, Seq<Eff<A>>> ma, Func<A, Eff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Env, Seq<Eff<A>>> ma, Func<A, Eff<Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);
        
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Seq<Eff<A>>> ma, Func<A, Aff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);

        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Unit> IterParallel<A>(this Aff<Seq<Eff<A>>> ma, Func<A, Aff<Unit>> f, int n = -1) =>
            Prelude.iterParallel(ma, f, n);
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> IterParallel<Env, A>(this Aff<Seq<Eff<A>>> ma, Func<A, Eff<Env, Unit>> f, int n = -1) where Env : struct, HasCancel<Env> =>
            Prelude.iterParallel(ma, f, n);
 
        /// <summary>
        /// Iterate items in a collection with a degree of parallelism 
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <param name="n">Maximum amount of tasks to run in parallel, uses `SysInfo.DefaultAsyncSequenceConcurrency`
        /// as a default if not supplied</param>
        /// <returns>Unit</returns>
        public static Aff<Unit> IterParallel<A>(this Aff<Seq<Eff<A>>> ma, Func<A, Eff<Unit>> f, int n = -1) =>
            Prelude.iterParallel(ma, f, n);
    }
}
