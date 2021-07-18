using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;
using LanguageExt.Thunks;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Construct an effect that will either succeed, have an exceptional, or unexceptional failure
        /// </summary>
        /// <param name="f">Function to capture the effect</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Synchronous IO monad that captures the effect</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, A> EffMaybe<Env, A>(Func<Env, Fin<A>> f) where Env : struct =>
            LanguageExt.Eff<Env, A>.EffectMaybe(f);
        
        /// <summary>
        /// Construct an effect that will either succeed or have an exceptional failure
        /// </summary>
        /// <param name="f">Function to capture the effect</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Synchronous IO monad that captures the effect</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, A> Eff<Env, A>(Func<Env, A> f) where Env : struct =>
            LanguageExt.Eff<Env, A>.Effect(f);
 
        /// <summary>
        /// Construct an effect that will either succeed, have an exceptional, or unexceptional failure
        /// </summary>
        /// <param name="f">Function to capture the effect</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Synchronous IO monad that captures the effect</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<A> EffMaybe<A>(Func<Fin<A>> f) =>
            LanguageExt.Eff<A>.EffectMaybe(f);

        /// <summary>
        /// Construct an effect that will either succeed or have an exceptional failure
        /// </summary>
        /// <param name="f">Function to capture the effect</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Synchronous IO monad that captures the effect</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<A> Eff<A>(Func<A> f) =>
            LanguageExt.Eff<A>.Effect(f);

        /// <summary>
        /// Construct an successful effect with a pure value
        /// </summary>
        /// <param name="value">Pure value to construct the monad with</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Synchronous IO monad that captures the pure value</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<A> SuccessEff<A>(A value) =>
            LanguageExt.Eff<A>.Success(value);

        /// <summary>
        /// Construct an successful effect with a pure value
        /// </summary>
        /// <param name="value">Pure value to construct the monad with</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Synchronous IO monad that captures the pure value</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<RT, A> SuccessEff<RT, A>(A value) where RT : struct =>
            LanguageExt.Eff<RT, A>.Success(value);

        /// <summary>
        /// Construct a failed effect
        /// </summary>
        /// <param name="error">Error that represents the failure</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Synchronous IO monad that captures the failure</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<A> FailEff<A>(Error error) =>
            LanguageExt.Eff<A>.Fail(error);

        /// <summary>
        /// Construct a failed effect
        /// </summary>
        /// <param name="error">Error that represents the failure</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Synchronous IO monad that captures the failure</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<RT, A> FailEff<RT, A>(Error error) where RT : struct =>
            LanguageExt.Eff<RT, A>.Fail(error);
        
        /// <summary>
        /// Create a new local context for the environment by mapping the outer environment and then
        /// using the result as a new context when running the IO monad provided
        /// </summary>
        /// <param name="f">Function to map the outer environment into a new one to run `ma`</param>
        /// <param name="ma">IO monad to run in the new context</param>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<OuterEnv, A> localEff<OuterEnv, InnerEnv, A>(Func<OuterEnv, InnerEnv> f, Eff<InnerEnv, A> ma) 
            where InnerEnv : struct, HasCancel<InnerEnv> 
            where OuterEnv : struct, HasCancel<OuterEnv> =>
            new Eff<OuterEnv, A>(Thunk<OuterEnv, A>.Lazy(oenv => ma.Thunk.Value(f(oenv))));

        /// <summary>
        /// Unit effect
        /// </summary>
        /// <remarks>Always succeeds with a Unit value</remarks>
        public static readonly Eff<Unit> unitEff = SuccessEff(unit);

        /// <summary>
        /// True effect
        /// </summary>
        /// <remarks>Always succeeds with a boolean true value</remarks>
        public static readonly Eff<bool> trueEff = SuccessEff(true);

        /// <summary>
        /// False effect
        /// </summary>
        /// <remarks>Always succeeds with a boolean false value</remarks>
        public static readonly Eff<bool> falseEff = SuccessEff(false);
    }
}
