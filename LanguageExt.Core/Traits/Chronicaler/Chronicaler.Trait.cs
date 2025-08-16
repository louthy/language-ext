using System;

namespace LanguageExt.Traits;

/// <summary>
/// Hybrid error/writer monad class that allows both accumulating outputs and aborting computation with a final output.
/// 
/// The expected use case is for computations with a notion of fatal vs. non-fatal errors.
/// </summary>
/// <typeparam name="M">Self</typeparam>
/// <typeparam name="Ch">Chronicle type</typeparam>
public interface Chronicaler<M, Ch>
    where M : Chronicaler<M, Ch>
{
    /// <summary>
    /// `Dictate` is an action that records the output `value`.
    /// Equivalent to `tell` for the `Writable` traits.
    /// </summary>
    /// <param name="value">Value to construct with</param>
    /// <returns>Chronicle structure</returns>
    public static abstract K<M, A> Dictate<A>(A value);
    
    /// <summary>
    /// `Confess` is an action that ends with a final output `value`.
    /// Equivalent to `fail` for the 'Fallible' trait.
    /// </summary>
    /// <param name="confession">Value to construct with</param>
    /// <returns>Chronicle structure</returns>
    public static abstract K<M, A> Confess<A>(Ch confession);

    /// <summary>
    /// `Memento` is an action that executes the action within this structure, returning either
    /// its record, if it ended with `Confess`, or its final value otherwise, with any record
    /// added to the current record.
    ///
    /// Similar to 'Catch' in the 'Fallible' trait, but with a notion of non-fatal errors (which
    /// are accumulated) vs. fatal errors (which are caught without accumulating).
    /// </summary>
    /// <param name="ma">Action to memento</param>
    public static abstract K<M, Either<Ch, A>> Memento<A>(K<M, A> ma);
        
    /// <summary>
    /// `Absolve` is an action that executes this structure and discards any record it had.
    /// The `defaultValue` will be used if the action ended via `Confess`. 
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <param name="ma">Action to absolve</param>
    public static abstract K<M, A> Absolve<A>(A defaultValue, K<M, A> ma);
        
    /// <summary>
    /// `Condemn` is an action that executes the structure and keeps its value
    /// only if it had no record. Otherwise, the value (if any) will be discarded
    /// and only the record kept.
    /// 
    /// This can be seen as converting non-fatal errors into fatal ones.
    /// </summary>
    /// <param name="ma">Action to condemn</param>
    public static abstract K<M, A> Condemn<A>(K<M, A> ma);
        
    /// <summary>
    /// An action that executes the structure and applies the function `f` to its output, leaving
    /// the return value unchanged.-
    /// </summary>
    /// <remarks>
    /// Equivalent to `censor` for the 'Writable` trait.
    /// </remarks>
    /// <param name="f">Censoring function</param>
    /// <param name="ma">Action to censor</param>
    public static abstract K<M, A> Censor<A>(Func<Ch, Ch> f, K<M, A> ma);
        
    /// <summary>
    /// Construct a new chronicle with `these`.
    /// </summary>
    /// <param name="these">What to chronicle</param>
    /// <returns>Chronicle structure</returns>
    public static abstract K<M, A> Chronicle<A>(These<Ch, A> ma);    
}
