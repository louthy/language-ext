using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Interfaces;
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
        public static Eff<Env, A> EffMaybe<Env, A>(Func<Env, Fin<A>> f) =>
            LanguageExt.Eff<Env, A>.EffectMaybe(f);
        
        /// <summary>
        /// Construct an effect that will either succeed or have an exceptional failure
        /// </summary>
        /// <param name="f">Function to capture the effect</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Synchronous IO monad that captures the effect</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, A> Eff<Env, A>(Func<Env, A> f) =>
            LanguageExt.Eff<Env, A>.Effect(f);
 
        /// <summary>
        /// Construct an effect that will either succeed, have an exceptional, or unexceptional failure
        /// </summary>
        /// <param name="f">Function to capture the effect</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Synchronous IO monad that captures the effect</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public static EffPure<A> EffMaybe<A>(Func<Fin<A>> f) =>
            LanguageExt.EffPure<A>.EffectMaybe(f);

        /// <summary>
        /// Construct an effect that will either succeed or have an exceptional failure
        /// </summary>
        /// <param name="f">Function to capture the effect</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Synchronous IO monad that captures the effect</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public static EffPure<A> Eff<A>(Func<A> f) =>
            LanguageExt.EffPure<A>.Effect(f);

        /// <summary>
        /// Construct an successful effect with a pure value
        /// </summary>
        /// <param name="value">Pure value to construct the monad with</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Synchronous IO monad that captures the pure value</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public static EffPure<A> SuccessEff<A>(A value) =>
            LanguageExt.EffPure<A>.Success(value);

        /// <summary>
        /// Construct a failed effect
        /// </summary>
        /// <param name="error">Error that represents the failure</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Synchronous IO monad that captures the failure</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public static EffPure<A> FailEff<A>(Error error) =>
            LanguageExt.EffPure<A>.Fail(error);
        
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
            new Eff<OuterEnv, A>(Thunk<OuterEnv, A>.Lazy(oenv => ma.thunk.Value(f(oenv))));

        /// <summary>
        /// Encoding
        /// </summary>
        /// <typeparam name="Env">Runtime environment</typeparam>
        /// <returns>Encoding</returns>
        [Pure, MethodImpl(AffOpt.mops)]
        public static Eff<Env, Encoding> encoding<Env>()
            where Env : struct, HasEncoding<Env> =>
            default(Env).Encoding;
    }
}
