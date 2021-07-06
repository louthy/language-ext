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
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<Env, Seq<A>> ma, Func<A, Aff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);

        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<Env, Seq<A>> ma, Func<A, Aff<Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Eff<Env, Unit> Iter<Env, A>(this Eff<Env, Seq<A>> ma, Func<A, Eff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Eff<Env, Unit> Iter<Env, A>(this Eff<Env, Seq<A>> ma, Func<A, Eff<Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);
        
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<Seq<A>> ma, Func<A, Aff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);

        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Unit> Iter<A>(this Eff<Seq<A>> ma, Func<A, Aff<Unit>> f) =>
            Prelude.iter(ma, f);
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Eff<Env, Unit> Iter<Env, A>(this Eff<Seq<A>> ma, Func<A, Eff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Eff<Unit> Iter<A>(this Eff<Seq<A>> ma, Func<A, Eff<Unit>> f) =>
            Prelude.iter(ma, f);
        
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<Env, Seq<Aff<Env, A>>> ma, Func<A, Aff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);

        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<Env, Seq<Aff<Env, A>>> ma, Func<A, Aff<Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<Env, Seq<Aff<Env, A>>> ma, Func<A, Eff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<Env, Seq<Aff<Env, A>>> ma, Func<A, Eff<Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);
        
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<Seq<Aff<Env, A>>> ma, Func<A, Aff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);

        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<Seq<Aff<Env, A>>> ma, Func<A, Aff<Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<Seq<Aff<Env, A>>> ma, Func<A, Eff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<Seq<Aff<Env, A>>> ma, Func<A, Eff<Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);
        
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<Env, Seq<Aff<A>>> ma, Func<A, Aff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);

        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<Env, Seq<Aff<A>>> ma, Func<A, Aff<Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<Env, Seq<Aff<A>>> ma, Func<A, Eff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<Env, Seq<Aff<A>>> ma, Func<A, Eff<Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);
        
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<Seq<Aff<A>>> ma, Func<A, Aff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);

        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Unit> Iter<A>(this Eff<Seq<Aff<A>>> ma, Func<A, Aff<Unit>> f) =>
            Prelude.iter(ma, f);
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<Seq<Aff<A>>> ma, Func<A, Eff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Unit> Iter<A>(this Eff<Seq<Aff<A>>> ma, Func<A, Eff<Unit>> f) =>
            Prelude.iter(ma, f);
            
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<Env, Seq<Eff<Env, A>>> ma, Func<A, Aff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);

        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<Env, Seq<Eff<Env, A>>> ma, Func<A, Aff<Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Eff<Env, Unit> Iter<Env, A>(this Eff<Env, Seq<Eff<Env, A>>> ma, Func<A, Eff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Eff<Env, Unit> Iter<Env, A>(this Eff<Env, Seq<Eff<Env, A>>> ma, Func<A, Eff<Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);
        
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<Seq<Eff<Env, A>>> ma, Func<A, Aff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);

        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<Seq<Eff<Env, A>>> ma, Func<A, Aff<Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Eff<Env, Unit> Iter<Env, A>(this Eff<Seq<Eff<Env, A>>> ma, Func<A, Eff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Eff<Env, Unit> Iter<Env, A>(this Eff<Seq<Eff<Env, A>>> ma, Func<A, Eff<Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);
            
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<Env, Seq<Eff<A>>> ma, Func<A, Aff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);

        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<Env, Seq<Eff<A>>> ma, Func<A, Aff<Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Eff<Env, Unit> Iter<Env, A>(this Eff<Env, Seq<Eff<A>>> ma, Func<A, Eff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Eff<Env, Unit> Iter<Env, A>(this Eff<Env, Seq<Eff<A>>> ma, Func<A, Eff<Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);
        
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Iter<Env, A>(this Eff<Seq<Eff<A>>> ma, Func<A, Aff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);

        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Aff<Unit> Iter<A>(this Eff<Seq<Eff<A>>> ma, Func<A, Aff<Unit>> f) =>
            Prelude.iter(ma, f);
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Eff<Env, Unit> Iter<Env, A>(this Eff<Seq<Eff<A>>> ma, Func<A, Eff<Env, Unit>> f) where Env : struct, HasCancel<Env> =>
            Prelude.iter(ma, f);
 
        /// <summary>
        /// Sequentially iterate items in a collection
        /// </summary>
        /// <param name="ma">Collection to iterate</param>
        /// <param name="f">Function to apply to each item in the collection</param>
        /// <returns>Unit</returns>
        public static Eff<Unit> Iter<A>(this Eff<Seq<Eff<A>>> ma, Func<A, Eff<Unit>> f) =>
            Prelude.iter(ma, f);
    }
}
