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
        public const int IsFailed = 3; 
        public const int IsCancelled = 4; 
        
        [Pure, MethodImpl(mops)]
        public static ThunkAsync<Env, A> Flatten<Env, A>(this ThunkAsync<Env, ThunkAsync<Env, A>> mma) where Env : struct, HasCancel<Env> =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
            ThunkAsync<Env, A>.Lazy(async env =>
            {
                mma.SpinIfEvaluating();

                switch (mma.state)
                {
                    case IsSuccess:
                        var sma = mma.value;
                        sma.SpinIfEvaluating();

                        switch (sma.state)
                        {
                            case IsSuccess:
                                return Fin<A>.Succ(sma.value);

                            case NotEvaluated:
                                return await sma.Value(env).ConfigureAwait(false);

                            default:
                                return Fin<A>.Fail(sma.error);
                        }

                    case NotEvaluated:
                        var ema = await mma.fun(env).ConfigureAwait(false);
                        if (ema.IsSucc)
                        {
                            return await ema.Value.Value(env).ConfigureAwait(false);
                        }
                        else
                        {
                            return Fin<A>.Fail(ema.Error);
                        }

                    default:
                        return Fin<A>.Fail(mma.error);                    
                }
            });      
        
        [Pure, MethodImpl(Thunk.mops)]
        public static ThunkAsync<Env, A> Flatten<Env, A>(this ThunkAsync<ThunkAsync<Env, A>> mma) where Env : struct, HasCancel<Env> =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
                ThunkAsync<Env, A>.Lazy(async env =>
                {
                    mma.SpinIfEvaluating();

                    switch (mma.state)
                    {
                        case IsSuccess:
                            var sma = mma.value;
                            sma.SpinIfEvaluating();

                            switch (sma.state)
                            {
                                case IsSuccess:
                                    return Fin<A>.Succ(sma.value);

                                case NotEvaluated:
                                    return await sma.Value(env).ConfigureAwait(false);

                                default:
                                    return Fin<A>.Fail(sma.error);
                            }

                        case NotEvaluated:
                            var ema = await mma.fun().ConfigureAwait(false);
                            if (ema.IsSucc)
                            {
                                return await ema.Value.Value(env).ConfigureAwait(false);
                            }
                            else
                            {
                                return Fin<A>.Fail(ema.Error);
                            }

                        default:
                            return Fin<A>.Fail(mma.error);                    
                    }
                });    
        
        
        [Pure, MethodImpl(Thunk.mops)]
        public static ThunkAsync<Env, A> Flatten<Env, A>(this ThunkAsync<Thunk<Env, A>> mma) where Env : struct, HasCancel<Env> =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
                ThunkAsync<Env, A>.Lazy(async env =>
                {
                    mma.SpinIfEvaluating();

                    switch (mma.state)
                    {
                        case IsSuccess:
                            var sma = mma.value;
                            sma.SpinIfEvaluating();

                            switch (sma.state)
                            {
                                case IsSuccess:
                                    return Fin<A>.Succ(sma.value);

                                case NotEvaluated:
                                    return sma.Value(env);

                                default:
                                    return Fin<A>.Fail(sma.error);
                            }

                        case NotEvaluated:
                            var ema = await mma.fun().ConfigureAwait(false);
                            if (ema.IsSucc)
                            {
                                return ema.Value.Value(env);
                            }
                            else
                            {
                                return Fin<A>.Fail(ema.Error);
                            }

                        default:
                            return Fin<A>.Fail(mma.error);                    
                    }
                });          
        
        [Pure, MethodImpl(Thunk.mops)]
        public static ThunkAsync<Env, A> Flatten<Env, A>(this Thunk<ThunkAsync<Env, A>> mma) where Env : struct, HasCancel<Env> =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
                ThunkAsync<Env, A>.Lazy(async env =>
                {
                    mma.SpinIfEvaluating();

                    switch (mma.state)
                    {
                        case IsSuccess:
                            var sma = mma.value;
                            sma.SpinIfEvaluating();

                            switch (sma.state)
                            {
                                case IsSuccess:
                                    return Fin<A>.Succ(sma.value);

                                case NotEvaluated:
                                    return await sma.Value(env).ConfigureAwait(false);

                                default:
                                    return Fin<A>.Fail(sma.error);
                            }

                        case NotEvaluated:
                            var ema = mma.fun();
                            if (ema.IsSucc)
                            {
                                return await ema.Value.Value(env).ConfigureAwait(false);
                            }
                            else
                            {
                                return Fin<A>.Fail(ema.Error);
                            }

                        default:
                            return Fin<A>.Fail(mma.error);                    
                    }
                });         
        
        [Pure, MethodImpl(Thunk.mops)]
        public static ThunkAsync<Env, A> Flatten<Env, A>(this ThunkAsync<Env, ThunkAsync<A>> mma) where Env : struct, HasCancel<Env> =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
            ThunkAsync<Env, A>.Lazy(async env =>
            {
                mma.SpinIfEvaluating();

                switch (mma.state)
                {
                    case IsSuccess:
                        var sma = mma.value;
                        sma.SpinIfEvaluating();

                        switch (sma.state)
                        {
                            case IsSuccess:
                                return Fin<A>.Succ(sma.value);

                            case NotEvaluated:
                                return await sma.Value().ConfigureAwait(false);

                            default:
                                return Fin<A>.Fail(sma.error);
                        }

                    case NotEvaluated:
                        var ema = await mma.fun(env).ConfigureAwait(false);
                        if (ema.IsSucc)
                        {
                            return await ema.Value.Value().ConfigureAwait(false);
                        }
                        else
                        {
                            return Fin<A>.Fail(ema.Error);
                        }

                    default:
                        return Fin<A>.Fail(mma.error);                    
                }
            });             
        
        [Pure, MethodImpl(Thunk.mops)]
        public static ThunkAsync<Env, A> Flatten<Env, A>(this ThunkAsync<Env, Thunk<Env, A>> mma) where Env : struct, HasCancel<Env> =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
            ThunkAsync<Env, A>.Lazy(async env =>
            {
                mma.SpinIfEvaluating();

                switch (mma.state)
                {
                    case IsSuccess:
                        var sma = mma.value;
                        sma.SpinIfEvaluating();

                        switch (sma.state)
                        {
                            case IsSuccess:
                                return Fin<A>.Succ(sma.value);

                            case NotEvaluated:
                                return sma.Value(env);

                            default:
                                return Fin<A>.Fail(sma.error);
                        }

                    case NotEvaluated:
                        var ema = await mma.fun(env).ConfigureAwait(false);
                        if (ema.IsSucc)
                        {
                            return ema.Value.Value(env);
                        }
                        else
                        {
                            return Fin<A>.Fail(ema.Error);
                        }

                    default:
                        return Fin<A>.Fail(mma.error);                    
                }
            });
        
        
        [Pure, MethodImpl(Thunk.mops)]
        public static ThunkAsync<Env, A> Flatten<Env, A>(this Thunk<Env, ThunkAsync<Env, A>> mma) where Env : struct, HasCancel<Env> =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
                ThunkAsync<Env, A>.Lazy(async env =>
                {
                    mma.SpinIfEvaluating();

                    switch (mma.state)
                    {
                        case IsSuccess:
                            var sma = mma.value;
                            sma.SpinIfEvaluating();

                            switch (sma.state)
                            {
                                case IsSuccess:
                                    return Fin<A>.Succ(sma.value);

                                case NotEvaluated:
                                    return await sma.Value(env).ConfigureAwait(false);

                                default:
                                    return Fin<A>.Fail(sma.error);
                            }

                        case NotEvaluated:
                            var ema = mma.fun(env);
                            if (ema.IsSucc)
                            {
                                return await ema.Value.Value(env).ConfigureAwait(false);
                            }
                            else
                            {
                                return Fin<A>.Fail(ema.Error);
                            }

                        default:
                            return Fin<A>.Fail(mma.error);                    
                    }
                });           
 
        [Pure]
        public static ThunkAsync<Env, A> Flatten<Env, A>(this ThunkAsync<Env, Thunk<A>> mma) where Env : struct, HasCancel<Env> =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
            ThunkAsync<Env, A>.Lazy(async env =>
            {
                mma.SpinIfEvaluating();

                switch (mma.state)
                {
                    case IsSuccess:
                        var sma = mma.value;
                        sma.SpinIfEvaluating();

                        switch (sma.state)
                        {
                            case IsSuccess:
                                return Fin<A>.Succ(sma.value);

                            case NotEvaluated:
                                return sma.Value();

                            default:
                                return Fin<A>.Fail(sma.error);
                        }

                    case NotEvaluated:
                        var ema = await mma.fun(env).ConfigureAwait(false);
                        if (ema.IsSucc)
                        {
                            return ema.Value.Value();
                        }
                        else
                        {
                            return Fin<A>.Fail(ema.Error);
                        }

                    default:
                        return Fin<A>.Fail(mma.error);
                }
            });  
        
         
        [Pure]
        public static ThunkAsync<Env, A> Flatten<Env, A>(this Thunk<Env, ThunkAsync<A>> mma) where Env : struct, HasCancel<Env> =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
                ThunkAsync<Env, A>.Lazy(async env =>
                {
                    mma.SpinIfEvaluating();

                    switch (mma.state)
                    {
                        case IsSuccess:
                            var sma = mma.value;
                            sma.SpinIfEvaluating();

                            switch (sma.state)
                            {
                                case IsSuccess:
                                    return Fin<A>.Succ(sma.value);

                                case NotEvaluated:
                                    return await sma.Value().ConfigureAwait(false);

                                default:
                                    return Fin<A>.Fail(sma.error);
                            }

                        case NotEvaluated:
                            var ema = mma.fun(env);
                            if (ema.IsSucc)
                            {
                                return await ema.Value.Value().ConfigureAwait(false);
                            }
                            else
                            {
                                return Fin<A>.Fail(ema.Error);
                            }

                        default:
                            return Fin<A>.Fail(mma.error);
                    }
                }); 
 
        [Pure, MethodImpl(Thunk.mops)]
        public static ThunkAsync< A> Flatten<A>(this ThunkAsync<ThunkAsync<A>> mma) =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
            ThunkAsync<A>.Lazy(async () =>
            {
                mma.SpinIfEvaluating();

                switch (mma.state)
                {
                    case IsSuccess:
                        var sma = mma.value;
                        sma.SpinIfEvaluating();

                        switch (sma.state)
                        {
                            case IsSuccess:
                                return Fin<A>.Succ(sma.value);

                            case NotEvaluated:
                                return await sma.Value().ConfigureAwait(false);

                            default:
                                return Fin<A>.Fail(sma.error);
                        }

                    case NotEvaluated:
                        var ema = await mma.fun().ConfigureAwait(false);
                        if (ema.IsSucc)
                        {
                            return await ema.Value.Value().ConfigureAwait(false);
                        }
                        else
                        {
                            return Fin<A>.Fail(ema.Error);
                        }

                    default:
                        return Fin<A>.Fail(mma.error);                    
                }
            });      

        [Pure, MethodImpl(Thunk.mops)]
        public static ThunkAsync<A> Flatten<A>(this ThunkAsync<Thunk<A>> mma) =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
            ThunkAsync<A>.Lazy(async () =>
            {
                mma.SpinIfEvaluating();

                switch (mma.state)
                {
                    case IsSuccess:
                        var sma = mma.value;
                        sma.SpinIfEvaluating();

                        switch (sma.state)
                        {
                            case IsSuccess:
                                return Fin<A>.Succ(sma.value);

                            case NotEvaluated:
                                return sma.Value();

                            default:
                                return Fin<A>.Fail(sma.error);
                        }

                    case NotEvaluated:
                        var ema = await mma.fun().ConfigureAwait(false);
                        if (ema.IsSucc)
                        {
                            return ema.Value.Value();
                        }
                        else
                        {
                            return Fin<A>.Fail(ema.Error);
                        }

                    default:
                        return Fin<A>.Fail(mma.error);                    
                }
            });      
        

        [Pure, MethodImpl(Thunk.mops)]
        public static ThunkAsync<A> Flatten<A>(this Thunk<ThunkAsync<A>> mma) =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
                ThunkAsync<A>.Lazy(async () =>
                {
                    mma.SpinIfEvaluating();

                    switch (mma.state)
                    {
                        case IsSuccess:
                            var sma = mma.value;
                            sma.SpinIfEvaluating();

                            switch (sma.state)
                            {
                                case IsSuccess:
                                    return Fin<A>.Succ(sma.value);

                                case NotEvaluated:
                                    return await sma.Value().ConfigureAwait(false);

                                default:
                                    return Fin<A>.Fail(sma.error);
                            }

                        case NotEvaluated:
                            var ema = mma.fun();
                            if (ema.IsSucc)
                            {
                                return await ema.Value.Value().ConfigureAwait(false);
                            }
                            else
                            {
                                return Fin<A>.Fail(ema.Error);
                            }

                        default:
                            return Fin<A>.Fail(mma.error);                    
                    }
                });           
        
        [Pure, MethodImpl(Thunk.mops)]
        public static Thunk<Env, A> Flatten<Env, A>(this Thunk<Env, Thunk<Env, A>> mma) =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
            Thunk<Env, A>.Lazy(env =>
            {
                mma.SpinIfEvaluating();

                switch (mma.state)
                {
                    case IsSuccess:
                        var sma = mma.value;
                        mma.SpinIfEvaluating();

                        switch (sma.state)
                        {
                            case IsSuccess:
                                return Fin<A>.Succ(sma.value);

                            case NotEvaluated:
                                return sma.Value(env);

                            default:
                                return Fin<A>.Fail(sma.error);
                        }

                    case NotEvaluated:
                        var ema = mma.fun(env);
                        if (ema.IsSucc)
                        {
                            return ema.Value.Value(env);
                        }
                        else
                        {
                            return Fin<A>.Fail(ema.Error);
                        }

                    default:
                        return Fin<A>.Fail(mma.error);                    
                }
            });
        
        [Pure, MethodImpl(Thunk.mops)]
        public static Thunk<Env, A> Flatten<Env, A>(this Thunk<Thunk<Env, A>> mma) =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
                Thunk<Env, A>.Lazy(env =>
                {
                    mma.SpinIfEvaluating();

                    switch (mma.state)
                    {
                        case IsSuccess:
                            var sma = mma.value;
                            mma.SpinIfEvaluating();

                            switch (sma.state)
                            {
                                case IsSuccess:
                                    return Fin<A>.Succ(sma.value);

                                case NotEvaluated:
                                    return sma.Value(env);

                                default:
                                    return Fin<A>.Fail(sma.error);
                            }

                        case NotEvaluated:
                            var ema = mma.fun();
                            if (ema.IsSucc)
                            {
                                return ema.Value.Value(env);
                            }
                            else
                            {
                                return Fin<A>.Fail(ema.Error);
                            }

                        default:
                            return Fin<A>.Fail(mma.error);                    
                    }
                });        
        
        [Pure]
        public static Thunk<Env, A> Flatten<Env, A>(this Thunk<Env, Thunk<A>> mma) =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
            Thunk<Env, A>.Lazy(env =>
            {
                mma.SpinIfEvaluating();

                switch (mma.state)
                {
                    case IsSuccess:
                        var sma = mma.value;
                        sma.SpinIfEvaluating();

                        switch (sma.state)
                        {
                            case IsSuccess:
                                return Fin<A>.Succ(sma.value);

                            case NotEvaluated:
                                return sma.Value();

                            default:
                                return Fin<A>.Fail(sma.error);
                        }

                    case NotEvaluated:
                        var ema = mma.fun(env);
                        if (ema.IsSucc)
                        {
                            return ema.Value.Value();
                        }
                        else
                        {
                            return Fin<A>.Fail(ema.Error);
                        }

                    default:
                        return Fin<A>.Fail(mma.error);
                }
            });          
        
        [Pure, MethodImpl(Thunk.mops)]
        public static Thunk<A> Flatten<A>(this Thunk<Thunk<A>> mma) =>
            mma is null ? throw new ArgumentNullException(nameof(mma)) :
            Thunk<A>.Lazy(() =>
            {
                mma.SpinIfEvaluating();

                switch (mma.state)
                {
                    case IsSuccess:
                        var sma = mma.value;
                        sma.SpinIfEvaluating();

                        switch (sma.state)
                        {
                            case IsSuccess:
                                return Fin<A>.Succ(sma.value);

                            case NotEvaluated:
                                return sma.Value();

                            default:
                                return Fin<A>.Fail(sma.error);
                        }

                    case NotEvaluated:
                        var ema = mma.fun();
                        if (ema.IsSucc)
                        {
                            return ema.Value.Value();
                        }
                        else
                        {
                            return Fin<A>.Fail(ema.Error);
                        }

                    default:
                        return Fin<A>.Fail(mma.error);
                }
            });
    }
}
