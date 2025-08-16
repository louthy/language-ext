using System;

namespace LanguageExt.Traits;

public static class Chronicaler<Ch>
{
    /// <summary>
    /// `Dictate` is an action that records the output `value`.
    /// Equivalent to `tell` for the `Writable` traits.
    /// </summary>
    /// <param name="value">Value to construct with</param>
    /// <returns>Chronicle structure</returns>
    public static K<M, A> dictate<M, A>(A value) 
        where M : Chronicaler<M, Ch> =>
        M.Dictate(value);
    
    /// <summary>
    /// `Confess` is an action that ends with a final output `value`.
    /// Equivalent to `fail` for the 'Fallible' trait.
    /// </summary>
    /// <param name="confession">Value to construct with</param>
    /// <returns>Chronicle structure</returns>
    public static K<M, A> confess<M, A>(Ch confession) 
        where M : Chronicaler<M, Ch> =>
        M.Confess<A>(confession);

    /// <summary>
    /// `Memento` is an action that executes the action within this structure, returning either
    /// its record, if it ended with `Confess`, or its final value otherwise, with any record
    /// added to the current record.
    ///
    /// Similar to 'Catch' in the 'Fallible' trait, but with a notion of non-fatal errors (which
    /// are accumulated) vs. fatal errors (which are caught without accumulating).
    /// </summary>
    /// <param name="ma">Action to memento</param>
    public static K<M, Either<Ch, A>> memento<M, A>(K<M, A> ma)
        where M : Chronicaler<M, Ch> =>
        M.Memento(ma);
        
    /// <summary>
    /// `Absolve` is an action that executes this structure and discards any record it had.
    /// The `defaultValue` will be used if the action ended via `Confess`. 
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <param name="ma">Action to absolve</param>
    public static K<M, A> absolve<M, A>(A defaultValue, K<M, A> ma)
        where M : Chronicaler<M, Ch> =>
        M.Absolve(defaultValue, ma);
        
    /// <summary>
    /// `Condemn` is an action that executes the structure and keeps its value
    /// only if it had no record. Otherwise, the value (if any) will be discarded
    /// and only the record kept.
    /// 
    /// This can be seen as converting non-fatal errors into fatal ones.
    /// </summary>
    /// <param name="ma">Action to condemn</param>
    public static K<M, A> condemn<M, A>(K<M, A> ma)
        where M : Chronicaler<M, Ch> =>
        M.Condemn(ma);
        
    /// <summary>
    /// An action that executes the structure and applies the function `f` to its output, leaving
    /// the return value unchanged.-
    /// </summary>
    /// <remarks>
    /// Equivalent to `censor` for the 'Writable` trait.
    /// </remarks>
    /// <param name="f">Censoring function</param>
    /// <param name="ma">Action to censor</param>
    public static K<M, A> censor<M, A>(Func<Ch, Ch> f, K<M, A> ma)
        where M : Chronicaler<M, Ch> =>
        M.Censor(f, ma);
        
    /// <summary>
    /// Construct a new chronicle with `these`.
    /// </summary>
    /// <param name="these">What to chronicle</param>
    /// <returns>Chronicle structure</returns>
    public static K<M, A> chronicle<M, A>(These<Ch, A> ma)
        where M : Chronicaler<M, Ch> =>
        M.Chronicle(ma);    
}
