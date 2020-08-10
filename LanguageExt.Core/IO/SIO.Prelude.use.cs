using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Interfaces;
using LanguageExt.Thunks;

namespace LanguageExt
{
    public static partial class IO
    {
        /// <summary>
        /// Safely use a disposable resource
        /// </summary>
        /// <param name="Acq">Acquire resource</param>
        /// <param name="Use">Use resource</param>
        public static IO<R> use<H, R>(SIO<H> Acq, Func<H, IO<R>> Use) where H : IDisposable =>
            IO<R>.EffectMaybe(async () => {
                var h = Acq.RunIO();
                try
                {
                    if (h.IsFail) return h.Cast<R>();
                    return await Use(h.Value).RunIO();
                }
                finally
                {
                    h.Value?.Dispose();
                }
            });

        /// <summary>
        /// Safely use a disposable resource
        /// </summary>
        /// <param name="Acq">Acquire resource</param>
        /// <param name="Use">Use resource</param>
        public static IO<Env, R> use<Env, H, R>(SIO<H> Acq, Func<H, IO<Env, R>> Use)
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
            IO<Env, R>.EffectMaybe(async env => {
                var h = Acq.RunIO();
                try
                {
                    if (h.IsFail) return h.Cast<R>();
                    return await Use(h.Value).RunIO(env);
                }
                finally
                {
                    h.Value?.Dispose();
                }
            });
        
        /// <summary>
        /// Safely use a disposable resource
        /// </summary>
        /// <param name="Acq">Acquire resource</param>
        /// <param name="Use">Use resource</param>
        public static SIO<R> use<H, R>(SIO<H> Acq, Func<H, SIO<R>> Use) where H : IDisposable =>
            SIO<R>.EffectMaybe(() => {
                var h = Acq.RunIO();
                try
                {
                    if (h.IsFail) return h.Cast<R>();
                    return Use(h.Value).RunIO();
                }
                finally
                {
                    h.Value?.Dispose();
                }
            });

        /// <summary>
        /// Safely use a disposable resource
        /// </summary>
        /// <param name="Acq">Acquire resource</param>
        /// <param name="Use">Use resource</param>
        public static SIO<Env, R> use<Env, H, R>(SIO<H> Acq, Func<H, SIO<Env, R>> Use)
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
            SIO<Env, R>.EffectMaybe(env => {
                var h = Acq.RunIO();
                try
                {
                    if (h.IsFail) return h.Cast<R>();
                    return Use(h.Value).RunIO(env);
                }
                finally
                {
                    h.Value?.Dispose();
                }
            });    
        
        
        /// <summary>
        /// Safely use a disposable resource
        /// </summary>
        /// <param name="Acq">Acquire resource</param>
        /// <param name="Use">Use resource</param>
        public static IO<Env, R> use<Env, H, R>(SIO<Env, H> Acq, Func<H, IO<R>> Use) 
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
            IO<Env, R>.EffectMaybe(async env => {
                var h = Acq.RunIO(env);
                try
                {
                    if (h.IsFail) return h.Cast<R>();
                    return await Use(h.Value).RunIO();
                }
                finally
                {
                    h.Value?.Dispose();
                }
            });

        /// <summary>
        /// Safely use a disposable resource
        /// </summary>
        /// <param name="Acq">Acquire resource</param>
        /// <param name="Use">Use resource</param>
        public static IO<Env, R> use<Env, H, R>(SIO<Env, H> Acq, Func<H, IO<Env, R>> Use)
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
            IO<Env, R>.EffectMaybe(async env => {
                var h = Acq.RunIO(env);
                try
                {
                    if (h.IsFail) return h.Cast<R>();
                    return await Use(h.Value).RunIO(env);
                }
                finally
                {
                    h.Value?.Dispose();
                }
            });
        
        /// <summary>
        /// Safely use a disposable resource
        /// </summary>
        /// <param name="Acq">Acquire resource</param>
        /// <param name="Use">Use resource</param>
        public static SIO<Env, R> use<Env, H, R>(SIO<Env, H> Acq, Func<H, SIO<R>> Use) 
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
            SIO<Env, R>.EffectMaybe(env => {
                var h = Acq.RunIO(env);
                try
                {
                    if (h.IsFail) return h.Cast<R>();
                    return Use(h.Value).RunIO();
                }
                finally
                {
                    h.Value?.Dispose();
                }
            });

        /// <summary>
        /// Safely use a disposable resource
        /// </summary>
        /// <param name="Acq">Acquire resource</param>
        /// <param name="Use">Use resource</param>
        public static SIO<Env, R> use<Env, H, R>(SIO<Env, H> Acq, Func<H, SIO<Env, R>> Use)
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
            SIO<Env, R>.EffectMaybe(env => {
                var h = Acq.RunIO(env);
                try
                {
                    if (h.IsFail) return h.Cast<R>();
                    return Use(h.Value).RunIO(env);
                }
                finally
                {
                    h.Value?.Dispose();
                }
            });          
    }
}
