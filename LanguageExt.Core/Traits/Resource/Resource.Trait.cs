/*
using System;

namespace LanguageExt.Traits;

/// <summary>
/// Resource tracking trait
/// </summary>
/// <typeparam name="M">Self</typeparam>
public partial interface Resource<M>
    where M : Resource<M>
{
    /// <summary>
    /// Acquire a resource
    /// </summary>
    /// <param name="ma">Computation that generates the resource</param>
    /// <param name="release">Function to call to release the resource</param>
    /// <typeparam name="A">Bound value</typeparam>
    /// <returns>Acquired resource computation</returns>
    public static abstract K<M, A> Use<A>(IO<A> ma, Func<A, IO<Unit>> release);

    /// <summary>
    /// Release the resource
    /// </summary>
    /// <param name="value">Value to release</param>
    /// <typeparam name="A">Bound value</typeparam>
    /// <returns>Unit</returns>
    public static abstract K<M, Unit> Release<A>(A value);

    /// <summary>
    /// Get access to the resources
    /// </summary>
    public static abstract K<M, Resources> Resources { get; }

    /// <summary>
    /// The the computation in a local resource environment that automatically
    /// releases all resources acquired during the computation upon completion,
    /// whilst leaving the resources outside the environment alone.
    /// </summary>
    /// <param name="ma">Computation to run in a local resource environment</param>
    /// <typeparam name="A">Bound value</typeparam>
    /// <returns>Locally run computation</returns>
    public static abstract K<M, A> Local<A>(K<M, A> ma);
}
*/
