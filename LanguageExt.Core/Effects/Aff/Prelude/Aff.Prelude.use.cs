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
        public static Aff<R> use<H, R>(Aff<H> Acq, Func<H, Aff<R>> Use) where H : IDisposable =>
            AffMaybe<R>(async () =>
                        {
                            var h = await Acq.Run().ConfigureAwait(false);
                            if (h.IsFail) return h.Cast<R>();
                            try
                            {
                                return await Use(h.Value).Run().ConfigureAwait(false);
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
        public static Aff<RT, R> use<RT, H, R>(Aff<H> Acq, Func<H, Aff<RT, R>> Use)
            where RT : struct, HasCancel<RT>
            where H : IDisposable =>
            AffMaybe<RT, R>(async env =>
                             {
                                 var h = await Acq.Run().ConfigureAwait(false);
                                 if (h.IsFail) return h.Cast<R>();
                                 try
                                 {
                                     return await Use(h.Value).Run(env).ConfigureAwait(false);
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
        public static Aff<R> use<H, R>(Aff<H> Acq, Func<H, Eff<R>> Use) where H : IDisposable =>
            AffMaybe(async () =>
                     {
                         var h = await Acq.Run().ConfigureAwait(false);
                         if (h.IsFail) return h.Cast<R>();
                         try
                         {
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
        public static Aff<RT, R> use<RT, H, R>(Aff<H> Acq, Func<H, Eff<RT, R>> Use)
            where RT : struct, HasCancel<RT>
            where H : IDisposable =>
            AffMaybe<RT, R>(async env =>
                             {
                                 var h = await Acq.Run().ConfigureAwait(false);
                                 if (h.IsFail) return h.Cast<R>();
                                 try
                                 {
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
        public static Aff<RT, R> use<RT, H, R>(Aff<RT, H> Acq, Func<H, Aff<R>> Use)
            where RT : struct, HasCancel<RT>
            where H : IDisposable =>
            AffMaybe<RT, R>(async env =>
                             {
                                 var h = await Acq.Run(env).ConfigureAwait(false);
                                 if (h.IsFail) return h.Cast<R>();
                                 try
                                 {
                                     return await Use(h.Value).Run().ConfigureAwait(false);
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
        public static Aff<RT, R> use<RT, H, R>(Aff<RT, H> Acq, Func<H, Aff<RT, R>> Use)
            where RT : struct, HasCancel<RT>
            where H : IDisposable =>
            AffMaybe<RT, R>(async env =>
                             {
                                 var h = await Acq.Run(env).ConfigureAwait(false);
                                 if (h.IsFail) return h.Cast<R>();
                                 try
                                 {
                                     return await Use(h.Value).Run(env).ConfigureAwait(false);
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
        public static Aff<RT, R> use<RT, H, R>(Aff<RT, H> Acq, Func<H, Eff<R>> Use)
            where RT : struct, HasCancel<RT>
            where H : IDisposable =>
            AffMaybe<RT, R>(async env =>
                             {
                                 var h = await Acq.Run(env).ConfigureAwait(false);
                                 if (h.IsFail) return h.Cast<R>();
                                 try
                                 {
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
        public static Aff<RT, R> use<RT, H, R>(Aff<RT, H> Acq, Func<H, Eff<RT, R>> Use)
            where RT : struct, HasCancel<RT>
            where H : IDisposable =>
            AffMaybe<RT, R>(async env =>
                             {
                                 var h = await Acq.Run(env).ConfigureAwait(false);
                                 if (h.IsFail) return h.Cast<R>();
                                 try
                                 {
                                     return Use(h.Value).Run(env);
                                 }
                                 finally
                                 {
                                     h.Value?.Dispose();
                                 }
                             });
    }
}
