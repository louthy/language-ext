using System;
using LanguageExt.TypeClasses;

namespace LanguageExt.Traits;

/// <summary>
/// WriterM trait
/// 
/// `Tell` is how you log to the writer's output. The `WriterM` carries
/// this 'packet' upwards, merging it if needed (hence the `Monoid`
/// requirement).
///
/// `Listen` listens to a monad acting, and returns what the monad "said".
///
/// `Pass` lets you provide a writer transformer which changes internals of
/// the written object.
/// </summary>
/// <typeparam name="M">Writer self trait</typeparam>
/// <typeparam name="W">Monoidal output type</typeparam>
public interface WriterM<M, W>  
    where M : WriterM<M, W>
    where W : Monoid<W>
{
    /// <summary>
    /// Tell is an action that produces the writer output
    /// </summary>
    /// <param name="item">Item to tell</param>
    /// <typeparam name="W">Writer type</typeparam>
    /// <returns>Structure with the told item</returns>
    public static abstract K<M, Unit> Tell(W item);

    /// <summary>
    /// Writes an item and returns a value at the same time
    /// </summary>
    public static abstract K<M, (A Value, W Output)> Listen<A>(K<M, A> ma);

    /// <summary>
    /// `Pass` is an action that executes the `action`, which returns a value and a
    /// function; it then returns a the value with the output having been applied to
    /// the function.
    /// </summary>
    /// <remarks>
    /// For usage, see `Writer.censor` for how it's used to filter the output.
    /// </remarks>
    public static abstract K<M, A> Pass<A>(K<M, (A Value, Func<W, W> Function)> action);

}
