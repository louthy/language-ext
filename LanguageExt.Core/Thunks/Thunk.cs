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
            ThunkAsync<Env, A>.Lazy(async env =>
            {
                mma.SpinIfEvaluating();

                switch (mma.state)
                {
                    case IsSuccess:
                        var sma = mma.value;
                        sma.SpinIfEvaluating();

                        return sma.state switch
                               {
                                   IsSuccess    => Fin<A>.Succ(sma.value),
                                   NotEvaluated => await sma.Value(env).ConfigureAwait(false),
                                   _            => Fin<A>.Fail(sma.error)
                               };

                    case NotEvaluated:
                        var ema = await mma.fun(env).ConfigureAwait(false);
                        return ema.IsSucc 
                                   ? await ema.Value.Value(env).ConfigureAwait(false) 
                                   : Fin<A>.Fail(ema.Error);

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

                            return sma.state switch
                                   {
                                       IsSuccess    => Fin<A>.Succ(sma.value),
                                       NotEvaluated => await sma.Value(env).ConfigureAwait(false),
                                       _            => Fin<A>.Fail(sma.error)
                                   };

                        case NotEvaluated:
                            var ema = await mma.fun().ConfigureAwait(false);
                            return ema.IsSucc 
                                       ? await ema.Value.Value(env).ConfigureAwait(false) 
                                       : Fin<A>.Fail(ema.Error);

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

                            return sma.state switch
                                   {
                                       IsSuccess    => Fin<A>.Succ(sma.value),
                                       NotEvaluated => sma.Value(env),
                                       _            => Fin<A>.Fail(sma.error)
                                   };

                        case NotEvaluated:
                            var ema = await mma.fun().ConfigureAwait(false);
                            return ema.IsSucc 
                                       ? ema.Value.Value(env) 
                                       : Fin<A>.Fail(ema.Error);

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

                            return sma.state switch
                                   {
                                       IsSuccess    => Fin<A>.Succ(sma.value),
                                       NotEvaluated => await sma.Value(env).ConfigureAwait(false),
                                       _            => Fin<A>.Fail(sma.error)
                                   };

                        case NotEvaluated:
                            var ema = mma.fun();
                            return ema.IsSucc 
                                       ? await ema.Value.Value(env).ConfigureAwait(false) 
                                       : Fin<A>.Fail(ema.Error);

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

                        return sma.state switch
                               {
                                   IsSuccess    => Fin<A>.Succ(sma.value),
                                   NotEvaluated => await sma.Value().ConfigureAwait(false),
                                   _            => Fin<A>.Fail(sma.error)
                               };

                    case NotEvaluated:
                        var ema = await mma.fun(env).ConfigureAwait(false);
                        return ema.IsSucc 
                                   ? await ema.Value.Value().ConfigureAwait(false) 
                                   : Fin<A>.Fail(ema.Error);

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

                        return sma.state switch
                               {
                                   IsSuccess    => Fin<A>.Succ(sma.value),
                                   NotEvaluated => sma.Value(env),
                                   _            => Fin<A>.Fail(sma.error)
                               };

                    case NotEvaluated:
                        var ema = await mma.fun(env).ConfigureAwait(false);
                        return ema.IsSucc 
                                   ? ema.Value.Value(env) 
                                   : Fin<A>.Fail(ema.Error);

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

                            return sma.state switch
                                   {
                                       IsSuccess    => Fin<A>.Succ(sma.value),
                                       NotEvaluated => await sma.Value(env).ConfigureAwait(false),
                                       _            => Fin<A>.Fail(sma.error)
                                   };

                        case NotEvaluated:
                            var ema = mma.fun(env);
                            return ema.IsSucc 
                                       ? await ema.Value.Value(env).ConfigureAwait(false) 
                                       : Fin<A>.Fail(ema.Error);

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

                        return sma.state switch
                               {
                                   IsSuccess    => Fin<A>.Succ(sma.value),
                                   NotEvaluated => sma.Value(),
                                   _            => Fin<A>.Fail(sma.error)
                               };

                    case NotEvaluated:
                        var ema = await mma.fun(env).ConfigureAwait(false);
                        return ema.IsSucc 
                                   ? ema.Value.Value() 
                                   : Fin<A>.Fail(ema.Error);

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

                            return sma.state switch
                                   {
                                       IsSuccess    => Fin<A>.Succ(sma.value),
                                       NotEvaluated => await sma.Value().ConfigureAwait(false),
                                       _            => Fin<A>.Fail(sma.error)
                                   };

                        case NotEvaluated:
                            var ema = mma.fun(env);
                            return ema.IsSucc 
                                       ? await ema.Value.Value().ConfigureAwait(false) 
                                       : Fin<A>.Fail(ema.Error);

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

                        return sma.state switch
                               {
                                   IsSuccess    => Fin<A>.Succ(sma.value),
                                   NotEvaluated => await sma.Value().ConfigureAwait(false),
                                   _            => Fin<A>.Fail(sma.error)
                               };

                    case NotEvaluated:
                        var ema = await mma.fun().ConfigureAwait(false);
                        return ema.IsSucc 
                                   ? await ema.Value.Value().ConfigureAwait(false) 
                                   : Fin<A>.Fail(ema.Error);

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

                        return sma.state switch
                               {
                                   IsSuccess    => Fin<A>.Succ(sma.value),
                                   NotEvaluated => sma.Value(),
                                   _            => Fin<A>.Fail(sma.error)
                               };

                    case NotEvaluated:
                        var ema = await mma.fun().ConfigureAwait(false);
                        return ema.IsSucc 
                                   ? ema.Value.Value() 
                                   : Fin<A>.Fail(ema.Error);

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

                            return sma.state switch
                                   {
                                       IsSuccess    => Fin<A>.Succ(sma.value),
                                       NotEvaluated => await sma.Value().ConfigureAwait(false),
                                       _            => Fin<A>.Fail(sma.error)
                                   };

                        case NotEvaluated:
                            var ema = mma.fun();
                            return ema.IsSucc 
                                       ? await ema.Value.Value().ConfigureAwait(false) 
                                       : Fin<A>.Fail(ema.Error);

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

                        return sma.state switch
                               {
                                   IsSuccess    => Fin<A>.Succ(sma.value),
                                   NotEvaluated => sma.Value(env),
                                   _            => Fin<A>.Fail(sma.error)
                               };

                    case NotEvaluated:
                        var ema = mma.fun(env);
                        return ema.IsSucc 
                                   ? ema.Value.Value(env) 
                                   : Fin<A>.Fail(ema.Error);

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

                            return sma.state switch
                                   {
                                       IsSuccess    => Fin<A>.Succ(sma.value),
                                       NotEvaluated => sma.Value(env),
                                       _            => Fin<A>.Fail(sma.error)
                                   };

                        case NotEvaluated:
                            var ema = mma.fun();
                            return ema.IsSucc 
                                       ? ema.Value.Value(env) 
                                       : Fin<A>.Fail(ema.Error);

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

                        return sma.state switch
                               {
                                   IsSuccess    => Fin<A>.Succ(sma.value),
                                   NotEvaluated => sma.Value(),
                                   _            => Fin<A>.Fail(sma.error)
                               };

                    case NotEvaluated:
                        var ema = mma.fun(env);
                        return ema.IsSucc 
                                   ? ema.Value.Value() 
                                   : Fin<A>.Fail(ema.Error);

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

                        return sma.state switch
                               {
                                   IsSuccess    => Fin<A>.Succ(sma.value),
                                   NotEvaluated => sma.Value(),
                                   _            => Fin<A>.Fail(sma.error)
                               };

                    case NotEvaluated:
                        var ema = mma.fun();
                        return ema.IsSucc 
                                   ? ema.Value.Value() 
                                   : Fin<A>.Fail(ema.Error);

                    default:
                        return Fin<A>.Fail(mma.error);
                }
            });
    }
}
