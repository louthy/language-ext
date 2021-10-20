using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

// TODO: Retrying
// TODO: Folding

namespace LanguageExt.Thunks
{
    public static class Thunk
    {
        internal const MethodImplOptions mops = MethodImplOptions.AggressiveInlining;
        
        public const int NotEvaluated = 0; 
        public const int Evaluating = 1; 
        public const int IsSuccess = 2; 
        public const int IsFailed = 4; 
        public const int IsCancelled = 8;
        public const int HasEvaluated = IsSuccess | IsFailed | IsCancelled;
        
        [Pure, MethodImpl(mops)]
        public static ThunkAsync<Env, A> Flatten<Env, A>(this ThunkAsync<Env, ThunkAsync<Env, A>> mma) where Env : struct, HasCancel<Env> =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
            ThunkAsync<Env, A>.Lazy(
                async env =>
                {
                    var ra = await mma.ReValue(env).ConfigureAwait(false);
                    return ra.IsSucc
                               ? await ra.Value.ReValue(env).ConfigureAwait(false)
                               : Fin<A>.Fail(ra.Error);
                });      
        
        [Pure, MethodImpl(Thunk.mops)]
        public static ThunkAsync<Env, A> Flatten<Env, A>(this ThunkAsync<ThunkAsync<Env, A>> mma) where Env : struct, HasCancel<Env> =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
                ThunkAsync<Env, A>.Lazy(
                    async env =>
                    {
                        var ra = await mma.ReValue().ConfigureAwait(false);
                        return ra.IsSucc
                                   ? await ra.Value.ReValue(env).ConfigureAwait(false)
                                   : Fin<A>.Fail(ra.Error);
                    });      
        
        [Pure, MethodImpl(Thunk.mops)]
        public static ThunkAsync<Env, A> Flatten<Env, A>(this ThunkAsync<Thunk<Env, A>> mma) where Env : struct, HasCancel<Env> =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
                ThunkAsync<Env, A>.Lazy(
                    async env =>
                    {
                        var ra = await mma.ReValue().ConfigureAwait(false);
                        return ra.IsSucc
                                   ? ra.Value.ReValue(env)
                                   : Fin<A>.Fail(ra.Error);
                    });      
        
        [Pure, MethodImpl(Thunk.mops)]
        public static ThunkAsync<Env, A> Flatten<Env, A>(this Thunk<ThunkAsync<Env, A>> mma) where Env : struct, HasCancel<Env> =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
                ThunkAsync<Env, A>.Lazy(
                    async env =>
                    {
                        var ra = mma.ReValue();
                        return ra.IsSucc
                                   ? await ra.Value.ReValue(env).ConfigureAwait(false)
                                   : Fin<A>.Fail(ra.Error);
                    });      
        
        [Pure, MethodImpl(Thunk.mops)]
        public static ThunkAsync<Env, A> Flatten<Env, A>(this ThunkAsync<Env, ThunkAsync<A>> mma) where Env : struct, HasCancel<Env> =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
                ThunkAsync<Env, A>.Lazy(
                    async env =>
                    {
                        var ra = await mma.ReValue(env).ConfigureAwait(false);
                        return ra.IsSucc
                                   ? await ra.Value.ReValue().ConfigureAwait(false)
                                   : Fin<A>.Fail(ra.Error);
                    });      
        
        [Pure, MethodImpl(Thunk.mops)]
        public static ThunkAsync<Env, A> Flatten<Env, A>(this ThunkAsync<Env, Thunk<Env, A>> mma) where Env : struct, HasCancel<Env> =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
                ThunkAsync<Env, A>.Lazy(
                    async env =>
                    {
                        var ra = await mma.ReValue(env).ConfigureAwait(false);
                        return ra.IsSucc
                                   ? ra.Value.ReValue(env)
                                   : Fin<A>.Fail(ra.Error);
                    });      
        
        [Pure, MethodImpl(Thunk.mops)]
        public static ThunkAsync<Env, A> Flatten<Env, A>(this Thunk<Env, ThunkAsync<Env, A>> mma) where Env : struct, HasCancel<Env> =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
                ThunkAsync<Env, A>.Lazy(
                    async env =>
                    {
                        var ra = mma.ReValue(env);
                        return ra.IsSucc
                                   ? await ra.Value.ReValue(env).ConfigureAwait(false)
                                   : Fin<A>.Fail(ra.Error);
                    });      
 
        [Pure]
        public static ThunkAsync<Env, A> Flatten<Env, A>(this ThunkAsync<Env, Thunk<A>> mma) where Env : struct, HasCancel<Env> =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
                ThunkAsync<Env, A>.Lazy(
                    async env =>
                    {
                        var ra = await mma.ReValue(env).ConfigureAwait(false);
                        return ra.IsSucc
                                   ? ra.Value.ReValue()
                                   : Fin<A>.Fail(ra.Error);
                    });      
         
        [Pure]
        public static ThunkAsync<Env, A> Flatten<Env, A>(this Thunk<Env, ThunkAsync<A>> mma) where Env : struct, HasCancel<Env> =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
                ThunkAsync<Env, A>.Lazy(
                    async env =>
                    {
                        var ra = mma.ReValue(env);
                        return ra.IsSucc
                                   ? await ra.Value.ReValue().ConfigureAwait(false)
                                   : Fin<A>.Fail(ra.Error);
                    });      
 
        [Pure, MethodImpl(Thunk.mops)]
        public static ThunkAsync< A> Flatten<A>(this ThunkAsync<ThunkAsync<A>> mma) =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
                ThunkAsync<A>.Lazy(
                    async () =>
                    {
                        var ra = await mma.ReValue().ConfigureAwait(false);
                        return ra.IsSucc
                                   ? await ra.Value.ReValue().ConfigureAwait(false)
                                   : Fin<A>.Fail(ra.Error);
                    });      

        [Pure, MethodImpl(Thunk.mops)]
        public static ThunkAsync<A> Flatten<A>(this ThunkAsync<Thunk<A>> mma) =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
                ThunkAsync<A>.Lazy(
                    async () =>
                    {
                        var ra = await mma.ReValue().ConfigureAwait(false);
                        return ra.IsSucc
                                   ? ra.Value.ReValue()
                                   : Fin<A>.Fail(ra.Error);
                    });      

        [Pure, MethodImpl(Thunk.mops)]
        public static ThunkAsync<A> Flatten<A>(this Thunk<ThunkAsync<A>> mma) =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
                ThunkAsync<A>.Lazy(
                    async () =>
                    {
                        var ra = mma.ReValue();
                        return ra.IsSucc
                                   ? await ra.Value.ReValue().ConfigureAwait(false)
                                   : Fin<A>.Fail(ra.Error);
                    });      
        
        [Pure, MethodImpl(Thunk.mops)]
        public static Thunk<Env, A> Flatten<Env, A>(this Thunk<Env, Thunk<Env, A>> mma) =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
                Thunk<Env, A>.Lazy(
                    env =>
                    {
                        var ra = mma.ReValue(env);
                        return ra.IsSucc
                                   ? ra.Value.ReValue(env)
                                   : Fin<A>.Fail(ra.Error);
                    });      
        
        [Pure, MethodImpl(Thunk.mops)]
        public static Thunk<Env, A> Flatten<Env, A>(this Thunk<Thunk<Env, A>> mma) =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
                Thunk<Env, A>.Lazy(
                    env =>
                    {
                        var ra = mma.ReValue();
                        return ra.IsSucc
                                   ? ra.Value.ReValue(env)
                                   : Fin<A>.Fail(ra.Error);
                    });      
        
        [Pure]
        public static Thunk<Env, A> Flatten<Env, A>(this Thunk<Env, Thunk<A>> mma) =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
                Thunk<Env, A>.Lazy(
                    env =>
                    {
                        var ra = mma.ReValue(env);
                        return ra.IsSucc
                                   ? ra.Value.ReValue()
                                   : Fin<A>.Fail(ra.Error);
                    });      
        
        [Pure, MethodImpl(Thunk.mops)]
        public static Thunk<A> Flatten<A>(this Thunk<Thunk<A>> mma) =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
                Thunk< A>.Lazy(
                    () =>
                    {
                        var ra = mma.ReValue();
                        return ra.IsSucc
                                   ? ra.Value.ReValue()
                                   : Fin<A>.Fail(ra.Error);
                    });      
    }
}
