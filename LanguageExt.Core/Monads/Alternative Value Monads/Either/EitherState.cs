namespace LanguageExt;

/// <summary>
/// Possible states of an Either
/// </summary>
public enum EitherStatus : byte
{
    /// <summary>
    /// Bottom state 
    /// </summary>
    /// <remarks>
    /// If you use Filter or Where (or 'where' in a LINQ expression) with Either, then the Either 
    /// will be put into a 'Bottom' state if the predicate returns false.  When it's in this state it is 
    /// neither Right nor Left.  And any usage could trigger a BottomException.  So be aware of the issue
    /// of filtering Either.
    /// 
    /// Also note, when the Either is in a Bottom state, some operations on it will continue to give valid
    /// results or return another Either in the Bottom state and not throw.  This is so a filtered Either 
    /// doesn't needlessly break expressions. 
    /// </remarks>
    IsBottom = 0,

    /// <summary>
    /// Left state
    /// </summary>
    IsLeft = 1,

    /// <summary>
    /// Right state
    /// </summary>
    IsRight = 2,
        
    /// <summary>
    /// Lazy
    /// </summary>
    /// <remarks>
    /// Lazy means its internal computation is evaluated on-demand and when a concrete
    /// non-`Either` value needs to be computed (i.e. when `Match` is called).
    /// 
    /// If it is not lazy then the `Either` is a pure data type and the value is always
    /// ready to be read without any additional computation.
    ///
    /// Note, the only way this type can get into a lazy state is:
    ///
    ///     * If we lift a transducer into an Either
    ///     * Or, if an Either value is lifted into a monad-transformer
    ///
    /// If you don't do any of these things then it will be 'strict'. 
    /// </remarks>
    IsLazy
}
