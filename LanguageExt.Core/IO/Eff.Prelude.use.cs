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
    public static partial class Prelude
    {
        /// <summary>
        /// Safely use a disposable resource
        /// </summary>
        /// <param name="Acq">Acquire resource</param>
        /// <param name="Use">Use resource</param>
        public static AffPure<R> use<H, R>(EffPure<H> Acq, Func<H, AffPure<R>> Use) where H : IDisposable =>
            AffMaybe(async () => {
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
        public static Aff<Env, R> use<Env, H, R>(EffPure<H> Acq, Func<H, Aff<Env, R>> Use)
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
            AffMaybe<Env, R>(async env => {
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
        public static EffPure<R> use<H, R>(EffPure<H> Acq, Func<H, EffPure<R>> Use) where H : IDisposable =>
            EffMaybe(() => {
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
        public static Eff<Env, R> use<Env, H, R>(EffPure<H> Acq, Func<H, Eff<Env, R>> Use)
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
            EffMaybe<Env, R>(env => {
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
        public static Aff<Env, R> use<Env, H, R>(Eff<Env, H> Acq, Func<H, AffPure<R>> Use) 
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
            AffMaybe<Env, R>(async env => {
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
        public static Aff<Env, R> use<Env, H, R>(Eff<Env, H> Acq, Func<H, Aff<Env, R>> Use)
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
            AffMaybe<Env, R>(async env => {
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
        public static Eff<Env, R> use<Env, H, R>(Eff<Env, H> Acq, Func<H, EffPure<R>> Use) 
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
            EffMaybe<Env, R>(env => {
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
        public static Eff<Env, R> use<Env, H, R>(Eff<Env, H> Acq, Func<H, Eff<Env, R>> Use)
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
            EffMaybe<Env, R>(env => {
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
