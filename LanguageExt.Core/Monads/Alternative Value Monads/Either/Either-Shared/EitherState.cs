namespace LanguageExt
{
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
        IsRight = 2
    }
}
