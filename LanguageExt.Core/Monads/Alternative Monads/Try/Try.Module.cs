using System;
using LanguageExt.Common;

namespace LanguageExt;

public partial class Try
{
    /// <summary>
    /// Lifts a pure success value into a `Try` monad
    /// </summary>
    /// <param name="value">Value to lift</param>
    /// <returns>`Try`</returns>
    public static Try<A> Succ<A>(A value) => 
        new (() => value);

    /// <summary>
    /// Lifts a failure value into a `Try` monad
    /// </summary>
    /// <param name="value">Failure value to lift</param>
    /// <returns>`Try`</returns>
    public static Try<A> Fail<A>(Error value) => 
        new (() => value);

    /// <summary>
    /// Lifts a given function into a `Try` monad
    /// </summary>
    /// <param name="f">Function to lift</param>
    /// <returns>`Try`</returns>
    public static Try<A> lift<A>(Func<Fin<A>> f) => 
        new(() =>
            {
                try
                {
                    return f();
                }
                catch (Exception e)
                {
                    return Fin.Fail<A>(e);
                }
            });
    
    /// <summary>
    /// Lifts a given value into a `Try` monad
    /// </summary>
    /// <param name="ma">Value to lift</param>
    /// <returns>`Try`</returns>
    public static Try<A> lift<A>(Fin<A> ma) => 
        new (() => ma);

    /// <summary>
    /// Lifts a given value into a `Try` monad
    /// </summary>
    /// <param name="ma">Value to lift</param>
    /// <returns>`Try`</returns>
    public static Try<A> lift<A>(Pure<A> ma) => 
        new (() => ma.Value);

    /// <summary>
    /// Lifts a given value into a `Try` monad
    /// </summary>
    /// <param name="ma">Value to lift</param>
    /// <returns>`Try`</returns>
    public static Try<A> lift<A>(Fail<Error> ma) => 
        new (() => ma.Value);

    /// <summary>
    /// Lifts a given function into a `Try` monad
    /// </summary>
    /// <param name="f">Function to lift</param>
    /// <returns>`Try`</returns>
    public static Try<A> lift<A>(Func<A> f) => 
        new(() =>
            {
                try
                {
                    return f();
                }
                catch (Exception e)
                {
                    return Fin.Fail<A>(e);
                }
            });

    /// <summary>
    /// Lifts a given function into a `Try` monad
    /// </summary>
    /// <param name="f">Function to lift</param>
    /// <returns>`Try`</returns>
    public static Try<Unit> lift(Action f) =>
        lift<Unit>(() => { f(); return default; });
}
