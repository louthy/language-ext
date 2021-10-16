namespace LanguageExt
{
    /// <summary>
    /// Trait that defines how to deal with a conflict between two values
    /// </summary>
    /// <typeparam name="V">Value type</typeparam>
    public interface Conflict<V>
    {
        (long TimeStamp, Option<V> Value) Resolve((long TimeStamp, Option<V> Value) Current, (long TimeStamp, Option<V> Value) Proposed);
    }
}
