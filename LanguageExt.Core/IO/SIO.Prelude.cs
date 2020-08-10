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
    public static partial class IO
    {
        /// <summary>
        /// Construct an effect that will either succeed, have an exceptional, or unexceptional failure
        /// </summary>
        /// <param name="f">Function to capture the effect</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Synchronous IO monad that captures the effect</returns>
        [Pure, MethodImpl(IO.mops)]
        public static SIO<A> EffectMaybe<A>(Func<Fin<A>> f) =>
            SIO<A>.EffectMaybe(f);
        
        /// <summary>
        /// Construct an effect that will either succeed, have an exceptional, or unexceptional failure
        /// </summary>
        /// <param name="f">Function to capture the effect</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Synchronous IO monad that captures the effect</returns>
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, A> EffectMaybe<Env, A>(Func<Env, Fin<A>> f) =>
            SIO<Env, A>.EffectMaybe(f);

        /// <summary>
        /// Construct an effect that will either succeed or have an exceptional failure
        /// </summary>
        /// <param name="f">Function to capture the effect</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Synchronous IO monad that captures the effect</returns>
        [Pure, MethodImpl(IO.mops)]
        public static SIO<A> Effect<A>(Func<A> f) =>
            SIO<A>.Effect(f);
        
        /// <summary>
        /// Construct an effect that will either succeed or have an exceptional failure
        /// </summary>
        /// <param name="f">Function to capture the effect</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Synchronous IO monad that captures the effect</returns>
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, A> Effect<Env, A>(Func<Env, A> f) =>
            SIO<Env, A>.Effect(f);

        /// <summary>
        /// Construct an successful effect with a pure value
        /// </summary>
        /// <param name="value">Pure value to construct the monad with</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Synchronous IO monad that captures the pure value</returns>
        [Pure, MethodImpl(IO.mops)]
        public static SIO<A> SuccessIO<A>(A value) =>
            SIO<A>.Success(value);

        /// <summary>
        /// Construct a failed effect
        /// </summary>
        /// <param name="error">Error that represents the failure</param>
        /// <typeparam name="A">Bound value type</typeparam>
        /// <returns>Synchronous IO monad that captures the failure</returns>
        [Pure, MethodImpl(IO.mops)]
        public static SIO<A> FailIO<A>(Error error) =>
            SIO<A>.Fail(error);
        
        /// <summary>
        /// Create a new local context for the environment by mapping the outer environment and then
        /// using the result as a new context when running the IO monad provided
        /// </summary>
        /// <param name="f">Function to map the outer environment into a new one to run `ma`</param>
        /// <param name="ma">IO monad to run in the new context</param>
        [Pure, MethodImpl(IO.mops)]
        public static SIO<OuterEnv, A> localIO<OuterEnv, InnerEnv, A>(Func<OuterEnv, InnerEnv> f, SIO<InnerEnv, A> ma) =>
            new SIO<OuterEnv, A>(Thunk<OuterEnv, A>.Lazy(oenv => ma.thunk.Value(f(oenv))));

        /// <summary>
        /// Encoding
        /// </summary>
        /// <typeparam name="Env">Runtime environment</typeparam>
        /// <returns>Encoding</returns>
        public static SIO<Env, Encoding> encoding<Env>()
            where Env : struct, HasEncoding<Env> =>
            default(Env).Encoding;    
    }
}
