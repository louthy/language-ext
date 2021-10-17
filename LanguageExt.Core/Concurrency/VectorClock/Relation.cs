namespace LanguageExt
{
    /// <summary>
    /// The relations two vector clocks may find themselves in.
    /// </summary>
    public enum Relation
    {
        Causes,
        CausedBy,
        Concurrent
    }
}
