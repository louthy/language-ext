using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt;

public static partial class AffExtensions
{
    /// <summary>
    /// Safely use a disposable resource
    /// </summary>
    /// <param name="Acq">Acquire resource</param>
    /// <param name="Use">Use resource</param>
    public static Aff<R> Use<H, R>(this Aff<H> Acq, Func<H, Aff<R>> Use) where H : IDisposable =>
        Prelude.use(Acq, Use);

    /// <summary>
    /// Safely use a disposable resource
    /// </summary>
    /// <param name="Acq">Acquire resource</param>
    /// <param name="Use">Use resource</param>
    public static Aff<RT, R> Use<RT, H, R>(this Aff<H> Acq, Func<H, Aff<RT, R>> Use)
        where RT : HasIO<RT, Error>
        where H : IDisposable =>
        Prelude.use(Acq, Use);
        
    /// <summary>
    /// Safely use a disposable resource
    /// </summary>
    /// <param name="Acq">Acquire resource</param>
    /// <param name="Use">Use resource</param>
    public static Aff<R> Use<H, R>(this Aff<H> Acq, Func<H, Eff<R>> Use) where H : IDisposable =>
        Prelude.use(Acq, Use);

    /// <summary>
    /// Safely use a disposable resource
    /// </summary>
    /// <param name="Acq">Acquire resource</param>
    /// <param name="Use">Use resource</param>
    public static Aff<RT, R> Use<RT, H, R>(this Aff<H> Acq, Func<H, Eff<RT, R>> Use)
        where RT : HasIO<RT, Error>
        where H : IDisposable =>
        Prelude.use(Acq, Use);
        
    /// <summary>
    /// Safely use a disposable resource
    /// </summary>
    /// <param name="Acq">Acquire resource</param>
    /// <param name="Use">Use resource</param>
    public static Aff<RT, R> Use<RT, H, R>(this Aff<RT, H> Acq, Func<H, Aff<R>> Use) 
        where RT : HasIO<RT, Error>
        where H : IDisposable =>
        Prelude.use(Acq, Use);

    /// <summary>
    /// Safely use a disposable resource
    /// </summary>
    /// <param name="Acq">Acquire resource</param>
    /// <param name="Use">Use resource</param>
    public static Aff<RT, R> Use<RT, H, R>(this Aff<RT, H> Acq, Func<H, Aff<RT, R>> Use)
        where RT : HasIO<RT, Error>
        where H : IDisposable =>
        Prelude.use(Acq, Use);
        
    /// <summary>
    /// Safely use a disposable resource
    /// </summary>
    /// <param name="Acq">Acquire resource</param>
    /// <param name="Use">Use resource</param>
    public static Aff<RT, R> Use<RT, H, R>(this Aff<RT, H> Acq, Func<H, Eff<R>> Use) 
        where RT : HasIO<RT, Error>
        where H : IDisposable =>
        Prelude.use(Acq, Use);

    /// <summary>
    /// Safely use a disposable resource
    /// </summary>
    /// <param name="Acq">Acquire resource</param>
    /// <param name="Use">Use resource</param>
    public static Aff<RT, R> Use<RT, H, R>(this Aff<RT, H> Acq, Func<H, Eff<RT, R>> Use)
        where RT : HasIO<RT, Error>
        where H : IDisposable =>
        Prelude.use(Acq, Use);
}
