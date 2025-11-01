using System;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public partial class ChronicleT<Ch, M>
{
    /// <summary>
    /// `dictate` is an action that records the output `value`.
    /// Equivalent to `tell` for the `Writable` traits.
    /// </summary>
    /// <param name="value">Value to construct with</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> dictate<A>(A value) =>
        lift(That<Ch, A>(value));
    
    /// <summary>
    /// `confess` is an action that ends with a final output `value`.
    /// Equivalent to `fail` for the 'Fallible' trait.
    /// </summary>
    /// <param name="value">Value to construct with</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> confess<A>(Ch value) =>
        lift(This<Ch, A>(value));
    
    /// <summary>
    /// Construct a new chronicle with `this` and `that`.
    /// </summary>
    /// <param name="@this">Value to construct with</param>
    /// <param name="that">Value to construct with</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> chronicle<A>(Ch @this, A that) =>
        lift(Both(@this, that));
    
    /// <summary>
    /// Construct a new chronicle with `these`.
    /// </summary>
    /// <param name="these">What to chronicle</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> lift<A>(These<Ch, A> these) =>
        new(_ => M.Pure(these));

    /// <summary>
    /// Lift a monad `M` into the monad-transformer
    /// </summary>
    /// <param name="ma">Monad to lift</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> lift<A>(K<M, A> ma) =>
        new(_ =>ma.Map(That<Ch, A>));
    
    /// <summary>
    /// Lift an `IO` monad into the monad-transformer
    /// </summary>
    /// <param name="ma">Monad to lift</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> liftIO<A>(K<IO, A> ma) =>
        lift(M.LiftIOMaybe(ma));

    /// <summary>
    /// `Memento` is an action that executes the action within this structure, returning either
    /// its record, if it ended with `Confess`, or its final value otherwise, with any record
    /// added to the current record.
    ///
    /// Similar to 'Catch' in the 'Fallible' trait, but with a notion of non-fatal errors (which
    /// are accumulated) vs. fatal errors (which are caught without accumulating).
    /// </summary>
    public static ChronicleT<Ch, M, Either<Ch, A>> memento<A>(K<ChronicleT<Ch, M>, A> ma) =>
        ma.As().Memento();
    
    /// <summary>
    /// `absolve` is an action that executes this structure and discards any record it had.
    /// The `defaultValue` will be used if the action ended via `Confess`. 
    /// </summary>
    /// <param name="defaultValue"></param>
    public static ChronicleT<Ch, M, A> absolve<A>(A defaultValue, K<ChronicleT<Ch, M>, A> ma) =>
        ma.As().Absolve(defaultValue);

    /// <summary>
    /// `Condemn` is an action that executes the structure and keeps its value
    /// only if it had no record. Otherwise, the value (if any) will be discarded
    /// and only the record kept.
    /// 
    /// This can be seen as converting non-fatal errors into fatal ones.
    /// </summary>
    public static ChronicleT<Ch, M, A> condemn<A>(K<ChronicleT<Ch, M>, A> ma) =>
        ma.As().Condemn();
    
    /// <summary>
    /// An action that executes the structure and applies the function `f` to its output, leaving
    /// the return value unchanged.-
    /// </summary>
    /// <remarks>
    /// Equivalent to `censor` for the 'Writable` trait.
    /// </remarks>
    /// <param name="f">Censoring function</param>
    public static ChronicleT<Ch, M, A> censor<A>(Func<Ch, Ch> f, K<ChronicleT<Ch, M>, A> ma) =>
        ma.As().Censor(f);

    /// <summary>
    /// Access to the internal semigroup instance. 
    /// </summary>
    internal static readonly ChronicleT<Ch, M, SemigroupInstance<Ch>> semigroup = 
        new (semi => M.Pure(That<Ch, SemigroupInstance<Ch>>(semi)));
}
