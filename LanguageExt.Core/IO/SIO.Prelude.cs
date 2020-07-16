using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Interfaces;
using LanguageExt.Thunks;

namespace LanguageExt
{
    public static partial class SIO
    {
        [Pure, MethodImpl(IO.mops)]
        public static SIO<A> EffectMaybe<A>(Func<Fin<A>> f) =>
            SIO<A>.EffectMaybe(f);

        [Pure, MethodImpl(IO.mops)]
        public static SIO<A> Effect<A>(Func<A> f) =>
            SIO<A>.Effect(f);

        [Pure, MethodImpl(IO.mops)]
        public static SIO<A> SuccessIO<A>(A value) =>
            SIO<A>.Success(value);

        [Pure, MethodImpl(IO.mops)]
        public static SIO<A> FailIO<A>(Error error) =>
            SIO<A>.Fail(error);
        
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, A> EffectMaybe<Env, A>(Func<Env, Fin<A>> f) where Env : Cancellable =>
            SIO<Env, A>.EffectMaybe(f);
        
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, A> Effect<Env, A>(Func<Env, A> f) where Env : Cancellable =>
            SIO<Env, A>.Effect(f);
        
        /// <summary>
        /// Create a new local context for the environment by mapping the outer environment and then
        /// using the result as a new context when running the IO monad provided
        /// </summary>
        /// <param name="f">Function to map the outer environment into a new one to run `ma`</param>
        /// <param name="ma">IO monad to run in the new context</param>
        [Pure, MethodImpl(IO.mops)]
        public static SIO<OuterEnv, A> local<OuterEnv, InnerEnv, A>(Func<OuterEnv, InnerEnv> f, SIO<InnerEnv, A> ma) 
            where OuterEnv : Cancellable
            where InnerEnv : Cancellable =>
            new SIO<OuterEnv, A>(Thunk<OuterEnv, A>.Lazy(oenv => ma.thunk.Value(f(oenv))));

        /// <summary>
        /// Synchronous environment
        /// </summary>
        [Pure, MethodImpl(IO.mops)]
        public static SIO<Env, Env> env<Env>() where Env : Cancellable => 
            new SIO<Env, Env>(Thunk<Env, Env>.Lazy(Fin<Env>.Succ));
    }
}