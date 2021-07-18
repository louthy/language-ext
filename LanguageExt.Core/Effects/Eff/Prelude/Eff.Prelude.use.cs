using System;
using System.Collections.Generic;
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
        /// Safely use a disposable resource
        /// </summary>
        /// <param name="Acq">Acquire resource</param>
        /// <param name="Use">Use resource</param>
        public static Aff<R> use<H, R>(Eff<H> Acq, Func<H, Aff<R>> Use) where H : IDisposable =>
            AffMaybe(async () =>
                     {
                         var h = Acq.ReRun();
                         try
                         {
                             if (h.IsFail) return h.Cast<R>();
                             return await Use(h.Value).Run();
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
        public static Aff<Env, R> use<Env, H, R>(Eff<H> Acq, Func<H, Aff<Env, R>> Use)
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
            AffMaybe<Env, R>(async env =>
                             {
                                 var h = Acq.ReRun();
                                 try
                                 {
                                     if (h.IsFail) return h.Cast<R>();
                                     return await Use(h.Value).Run(env);
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
        public static Eff<R> use<H, R>(Eff<H> Acq, Func<H, Eff<R>> Use) where H : IDisposable =>
            EffMaybe(() =>
                     {
                         var h = Acq.ReRun();
                         try
                         {
                             if (h.IsFail) return h.Cast<R>();
                             return Use(h.Value).Run();
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
        public static Eff<Env, R> use<Env, H, R>(Eff<H> Acq, Func<H, Eff<Env, R>> Use)
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
            EffMaybe<Env, R>(env =>
                             {
                                 var h = Acq.ReRun();
                                 try
                                 {
                                     if (h.IsFail) return h.Cast<R>();
                                     return Use(h.Value).Run(env);
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
        public static Aff<Env, R> use<Env, H, R>(Eff<Env, H> Acq, Func<H, Aff<R>> Use)
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
            AffMaybe<Env, R>(async env =>
                             {
                                 var h = Acq.ReRun(env);
                                 try
                                 {
                                     if (h.IsFail) return h.Cast<R>();
                                     return await Use(h.Value).Run();
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
            AffMaybe<Env, R>(async env =>
                             {
                                 var h = Acq.ReRun(env);
                                 try
                                 {
                                     if (h.IsFail) return h.Cast<R>();
                                     return await Use(h.Value).Run(env);
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
        public static Eff<Env, R> use<Env, H, R>(Eff<Env, H> Acq, Func<H, Eff<R>> Use)
            where Env : struct, HasCancel<Env>
            where H : IDisposable =>
            EffMaybe<Env, R>(env =>
                             {
                                 var h = Acq.ReRun(env);
                                 try
                                 {
                                     if (h.IsFail) return h.Cast<R>();
                                     return Use(h.Value).Run();
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
            EffMaybe<Env, R>(env =>
                             {
                                 var h = Acq.ReRun(env);
                                 try
                                 {
                                     if (h.IsFail) return h.Cast<R>();
                                     return Use(h.Value).Run(env);
                                 }
                                 finally
                                 {
                                     h.Value?.Dispose();
                                 }
                             });
    }
}
