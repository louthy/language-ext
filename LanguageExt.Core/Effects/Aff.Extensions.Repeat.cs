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
        /// Keeps repeating the computation until it fails  
        /// </summary>
        /// <remarks>
        ///
        ///     Strategies for handling the failure:
        /// 
        ///     ma.Repeat() | unitEff
        ///     ma.Repeat().IfFail(...)
        ///     ma.Repeat().Match(...)
        ///     ma.Repeat().BiMap(...)
        /// 
        /// </remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> Repeat<Env, A>(this Aff<Env, A> ma) where Env : struct, HasCancel<Env> =>
            Prelude.repeat(ma);
        
        /// <summary>
        /// Keeps repeating the computation until it fails  
        /// </summary>
        /// <remarks>
        ///
        ///     Strategies for handling the failure:
        /// 
        ///     ma.Repeat() | unitEff
        ///     ma.Repeat().IfFail(...)
        ///     ma.Repeat().Match(...)
        ///     ma.Repeat().BiMap(...)
        /// 
        /// </remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Unit</returns>
        public static Aff<Unit> Repeat<A>(this Aff<A> ma) => 
            Prelude.repeat(ma);
    }
}
