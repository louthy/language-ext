using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static class ChronicleT
{
    /// <summary>
    /// Monoid empty confession
    /// </summary>
    /// <typeparam name="Ch">Chronicle type (a monoid)</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ChronicleT<Ch, M, A> empty<Ch, M, A>()
        where Ch : Monoid<Ch>
        where M : Monad<M> =>
        confess<Ch, M, A>(Ch.Empty);
    
    /// <summary>
    /// `dictate` is an action that records the output `value`.
    /// Equivalent to `tell` for the `Writable` traits.
    /// </summary>
    /// <param name="value">Value to construct with</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> dictate<Ch, M, A>(A value) 
        where M : Monad<M> =>
        new(_ => M.Pure(That<Ch, A>(value)));
    
    /// <summary>
    /// `confess` is an action that ends with a final output `value`.
    /// Equivalent to `fail` for the 'Fallible' trait.
    /// </summary>
    /// <param name="value">Value to construct with</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> confess<Ch, M, A>(Ch value) 
        where M : Monad<M> =>
        new(_ => M.Pure(This<Ch, A>(value)));
    
    /// <summary>
    /// Construct a new chronicle with `this` and `that`.
    /// </summary>
    /// <param name="@this">Value to construct with</param>
    /// <param name="that">Value to construct with</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> chronicle<Ch, M, A>(Ch @this, A that) 
        where M : Monad<M> =>
        new(_ => M.Pure(Both(@this, that)));
    
    /// <summary>
    /// Construct a new chronicle with `these`.
    /// </summary>
    /// <param name="these">What to chronicle</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> chronicle<Ch, M, A>(These<Ch, A> these) 
        where M : Monad<M> =>
        new(_ => M.Pure(these));

    /// <summary>
    /// Lift a monad `M` into the monad-transformer
    /// </summary>
    /// <param name="ma">Monad to lift</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> lift<Ch, M, A>(K<M, A> ma)
        where M : Monad<M> =>
        new(_ =>ma.Map(That<Ch, A>));
    
    /// <summary>
    /// Lift an `IO` monad into the monad-transformer
    /// </summary>
    /// <param name="ma">Monad to lift</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> liftIO<Ch, M, A>(K<IO, A> ma)
        where M : Monad<M> =>
        lift<Ch, M, A>(M.LiftIOMaybe(ma));

    /// <summary>
    /// `Memento` is an action that executes the action within this structure, returning either
    /// its record, if it ended with `Confess`, or its final value otherwise, with any record
    /// added to the current record.
    ///
    /// Similar to 'Catch' in the 'Fallible' trait, but with a notion of non-fatal errors (which
    /// are accumulated) vs. fatal errors (which are caught without accumulating).
    /// </summary>
    public static ChronicleT<Ch, M, Either<Ch, A>> memento<Ch, M, A>(K<ChronicleT<Ch, M>, A> ma)
        where M : Monad<M> =>
        ma.As().Memento();
    
    /// <summary>
    /// `absolve` is an action that executes this structure and discards any record it had.
    /// The `defaultValue` will be used if the action ended via `Confess`. 
    /// </summary>
    /// <param name="defaultValue"></param>
    public static ChronicleT<Ch, M, A> absolve<Ch, M, A>(A defaultValue, K<ChronicleT<Ch, M>, A> ma) 
        where M : Monad<M> =>
        ma.As().Absolve(defaultValue);

    /// <summary>
    /// `Condemn` is an action that executes the structure and keeps its value
    /// only if it had no record. Otherwise, the value (if any) will be discarded
    /// and only the record kept.
    /// 
    /// This can be seen as converting non-fatal errors into fatal ones.
    /// </summary>
    public static ChronicleT<Ch, M, A> condemn<Ch, M, A>(K<ChronicleT<Ch, M>, A> ma) 
        where M : Monad<M> =>
        ma.As().Condemn();
    
    /// <summary>
    /// An action that executes the structure and applies the function `f` to its output, leaving
    /// the return value unchanged.-
    /// </summary>
    /// <remarks>
    /// Equivalent to `censor` for the 'Writable` trait.
    /// </remarks>
    /// <param name="f">Censoring function</param>
    public static ChronicleT<Ch, M, A> censor<Ch, M, A>(Func<Ch, Ch> f, K<ChronicleT<Ch, M>, A> ma) 
        where M : Monad<M> =>
        ma.As().Censor(f);

    /// <summary>
    /// Access to the internal semigroup instance. 
    /// </summary>
    internal static ChronicleT<Ch, M, SemigroupInstance<Ch>> semigroup<Ch, M>()
        where M : Monad<M> =>
        ChronicleT<Ch, M>.semigroup;
}
