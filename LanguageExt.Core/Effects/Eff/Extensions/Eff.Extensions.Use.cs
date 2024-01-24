/*
using System;
using LanguageExt.Effects.Traits;

namespace LanguageExt
{
    public static partial class EffExtensions
    {
        /// <summary>
        /// Safely use a disposable resource
        /// </summary>
        /// <param name="Acq">Acquire resource</param>
        /// <param name="Use">Use resource</param>
        public static Aff<R> Use<H, R>(this Eff<H> Acq, Func<H, Aff<R>> Use) where H : IDisposable =>
            Prelude.use(Acq, Use);

        /// <summary>
        /// Safely use a disposable resource
        /// </summary>
        /// <param name="Acq">Acquire resource</param>
        /// <param name="Use">Use resource</param>
        public static Aff<RT, R> Use<RT, H, R>(this Eff<H> Acq, Func<H, Aff<RT, R>> Use)
            where RT : HasCancel<RT>
            where H : IDisposable =>
            Prelude.use(Acq, Use);

        /// <summary>
        /// Safely use a disposable resource
        /// </summary>
        /// <param name="Acq">Acquire resource</param>
        /// <param name="Use">Use resource</param>
        public static Eff<R> Use<H, R>(this Eff<H> Acq, Func<H, Eff<R>> Use) where H : IDisposable =>
            Prelude.use(Acq, Use);

        /// <summary>
        /// Safely use a disposable resource
        /// </summary>
        /// <param name="Acq">Acquire resource</param>
        /// <param name="Use">Use resource</param>
        public static Eff<RT, R> Use<RT, H, R>(this Eff<H> Acq, Func<H, Eff<RT, R>> Use)
            where RT : HasCancel<RT>
            where H : IDisposable =>
            Prelude.use(Acq, Use);

        /// <summary>
        /// Safely use a disposable resource
        /// </summary>
        /// <param name="Acq">Acquire resource</param>
        /// <param name="Use">Use resource</param>
        public static Aff<RT, R> Use<RT, H, R>(this Eff<RT, H> Acq, Func<H, Aff<R>> Use)
            where RT : HasCancel<RT>
            where H : IDisposable =>
            Prelude.use(Acq, Use);

        /// <summary>
        /// Safely use a disposable resource
        /// </summary>
        /// <param name="Acq">Acquire resource</param>
        /// <param name="Use">Use resource</param>
        public static Aff<RT, R> Use<RT, H, R>(this Eff<RT, H> Acq, Func<H, Aff<RT, R>> Use)
            where RT : HasCancel<RT>
            where H : IDisposable =>
            Prelude.use(Acq, Use);

        /// <summary>
        /// Safely use a disposable resource
        /// </summary>
        /// <param name="Acq">Acquire resource</param>
        /// <param name="Use">Use resource</param>
        public static Eff<RT, R> Use<RT, H, R>(this Eff<RT, H> Acq, Func<H, Eff<R>> Use)
            where RT : HasCancel<RT>
            where H : IDisposable =>
            Prelude.use(Acq, Use);

        /// <summary>
        /// Safely use a disposable resource
        /// </summary>
        /// <param name="Acq">Acquire resource</param>
        /// <param name="Use">Use resource</param>
        public static Eff<RT, R> Use<RT, H, R>(this Eff<RT, H> Acq, Func<H, Eff<RT, R>> Use)
            where RT : HasCancel<RT>
            where H : IDisposable =>
            Prelude.use(Acq, Use);
    }
}
*/
