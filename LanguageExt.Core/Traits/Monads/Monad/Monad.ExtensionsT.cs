using System;

namespace LanguageExt.Traits;

/// <summary>
/// Monad module
/// </summary>
public static partial class Monad
{
    /// <summary>
    /// Runs a monadic bind operation on the nested monads 
    /// </summary>
    /// <remarks>
    /// If you're working with an inner monad that is concrete then you will first need to
    /// call `KindT` to cast the monad to a more general `K` version.  This enables the
    /// `T` variant extensions (like `BindT`, `MapT, etc.) to work without providing
    /// excessive generic arguments all the way down the chain.
    /// </remarks>
    /// <example>
    ///
    ///    var mx = Seq<Option<int>>(Some(1), Some(2), Some(3));
    ///         
    ///    var ma = mx.KindT<Seq, Option, Option<int>, int>()
    ///               .BindT(a => Some(a + 1))
    ///               .MapT(a => a + 1);
    ///               .AsT<Seq, Option, Option<int>, int>();
    ///
    /// </example>
    /// <param name="mna">Nested monadic value</param>
    /// <param name="f">Bind function</param>
    /// <typeparam name="NA">`N<A>`</typeparam>
    /// <typeparam name="M">Outer monad trait</typeparam>
    /// <typeparam name="N">Inner monad trait</typeparam>
    /// <typeparam name="A">Input bound value</typeparam>
    /// <typeparam name="B">Output bound value</typeparam>
    /// <returns>Mapped value</returns>
    public static K<M, K<N, B>> BindT<M, N, A, B>(this K<M, K<N, A>> mna, Func<A, K<N, B>> f)
        where M : Functor<M>
        where N : Monad<N> =>
        M.Map(na => N.Bind(na, f), mna);

    public static K<M, K<N, B>> BindT<M, N, A, B>(this K<M, K<N, A>> mna, Func<A, K<M, K<N, B>>> f)
        where M : Functor<M>
        where N : Monad<N> =>
        M.Map(na => N.Bind(na, f), mna);
    
}
