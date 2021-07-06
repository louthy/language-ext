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
        ///     repeat(ma) | unitEff
        ///     repeat(ma).IfFail(...)
        ///     repeat(ma).Match(...)
        ///     repeat(ma).BiMap(...)
        /// 
        /// </remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Unit</returns>
        public static Aff<Env, Unit> repeat<Env, A>(Aff<Env, A> ma)  where Env : struct, HasCancel<Env> =>
            AffMaybe<Env, Unit>(async env =>
                                {
                                    while (true)
                                    {
                                        var a = await ma.ReRun(env).ConfigureAwait(false);
                                        if (a.IsFail) return a.Cast<Unit>();
                                    }
                                });
        
        /// <summary>
        /// Keeps repeating the computation until it fails  
        /// </summary>
        /// <remarks>
        ///
        ///     Strategies for handling the failure:
        /// 
        ///     repeat(ma) | unitAff
        ///     repeat(ma).IfFail(...)
        ///     repeat(ma).Match(...)
        ///     repeat(ma).BiMap(...)
        /// 
        /// </remarks>
        /// <param name="ma">IO computation to fold over</param>
        /// <typeparam name="Env">Environment</typeparam>
        /// <typeparam name="A">Computation bound value type</typeparam>
        /// <returns>Unit</returns>
        public static Aff<Unit> repeat<A>(Aff<A> ma) => 
            AffMaybe<Unit>(async () =>
                           {
                               while (true)
                               {
                                   var a = await ma.ReRun().ConfigureAwait(false);
                                   if (a.IsFail) return a.Cast<Unit>();
                               }
                           });
    }
}
