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
        public static Eff<Env, Unit> repeat<Env, A>(Eff<Env, A> ma) =>
            EffMaybe<Env, Unit>(env =>
                                {
                                    while (true)
                                    {
                                        var a = ma.ReRun(env);
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
        public static Eff<Unit> repeat<A>(Eff<A> ma) => 
            EffMaybe<Unit>(() =>
                                {
                                    while (true)
                                    {
                                        var a = ma.ReRun();
                                        if (a.IsFail) return a.Cast<Unit>();
                                    }
                                });
    }
}
