using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class Prelude
{
    /// <summary>
    /// `dictate` is an action that records the output `value`.
    /// Equivalent to `tell` for the `Writable` traits.
    /// </summary>
    /// <remarks>
    /// This is 'Applicative.Pure'.
    /// </remarks>
    /// <param name="value">Value to construct with</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> dictate<Ch, M, A>(A value) 
        where Ch : Semigroup<Ch>
        where M : Monad<M> =>
        ChronicleT<Ch, M, A>.Dictate(value);
    
    /// <summary>
    /// `confess` is an action that ends with a final output `value`.
    /// Equivalent to `fail` for the 'Fallible' trait.
    /// </summary>
    /// <remarks>
    /// This is akin to yielding an error.
    /// </remarks>
    /// <param name="value">Value to construct with</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> confess<Ch, M, A>(Ch value) 
        where Ch : Semigroup<Ch>
        where M : Monad<M> =>
        ChronicleT<Ch, M, A>.Confess(value);
    
    /// <summary>
    /// Construct a new chronicle with `this` and `that`.
    /// </summary>
    /// <param name="@this">Value to construct with</param>
    /// <param name="that">Value to construct with</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> chronicle<Ch, M, A>(Ch @this, A that) 
        where Ch : Semigroup<Ch>
        where M : Monad<M> =>
        ChronicleT<Ch, M, A>.Chronicle(@this, that);
    
    /// <summary>
    /// Construct a new chronicle with `these`.
    /// </summary>
    /// <param name="these">What to chronicle</param>
    /// <returns>Chronicle structure</returns>
    public static ChronicleT<Ch, M, A> chronicle<Ch, M, A>(These<Ch, A> these) 
        where Ch : Semigroup<Ch>
        where M : Monad<M> =>
        ChronicleT<Ch, M, A>.Chronicle(these);
    
    /// <summary>
    /// `Memento` is an action that executes the action within this structure, returning either
    /// its record, if it ended with `Confess`, or its final value otherwise, with any record
    /// added to the current record.
    ///
    /// Similar to 'Catch' in the 'Fallible' trait, but with a notion of non-fatal errors (which
    /// are accumulated) vs. fatal errors (which are caught without accumulating).
    /// </summary>
    public static ChronicleT<Ch, M, Either<Ch, A>> memento<Ch, M, A>(K<ChronicleT<Ch, M>, A> ma)
        where Ch : Semigroup<Ch>
        where M : MonadIO<M> =>
        ma.As().Memento();
    
    /// <summary>
    /// `absolve` is an action that executes this structure and discards any record it had.
    /// The `defaultValue` will be used if the action ended via `Confess`. 
    /// </summary>
    /// <param name="defaultValue"></param>
    public static ChronicleT<Ch, M, A> absolve<Ch, M, A>(A defaultValue, K<ChronicleT<Ch, M>, A> ma) 
        where Ch : Semigroup<Ch>
        where M : MonadIO<M> =>
        ma.As().Absolve(defaultValue);

    /// <summary>
    /// `Condemn` is an action that executes the structure and keeps its value
    /// only if it had no record. Otherwise, the value (if any) will be discarded
    /// and only the record kept.
    /// 
    /// This can be seen as converting non-fatal errors into fatal ones.
    /// </summary>
    public static ChronicleT<Ch, M, A> condemn<Ch, M, A>(K<ChronicleT<Ch, M>, A> ma) 
        where Ch : Semigroup<Ch>
        where M : MonadIO<M> =>
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
        where Ch : Semigroup<Ch>
        where M : MonadIO<M> =>
        ma.As().Censor(f);
}
