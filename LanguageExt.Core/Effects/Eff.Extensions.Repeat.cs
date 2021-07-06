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
        public static Eff<Env, Unit> Repeat<Env, A>(this Eff<Env, A> ma) =>
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
        public static Eff<Unit> Repeat<A>(this Eff<A> ma) => 
            Prelude.repeat(ma);
    }
}
