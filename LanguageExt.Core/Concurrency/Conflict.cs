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

    /// <summary>
    /// Last-write-wins conflict resolver
    /// </summary>
    public struct LastWriteWins<V> : Conflict<V>
    {
        public (long TimeStamp, Option<V> Value) Resolve((long TimeStamp, Option<V> Value) Current, (long TimeStamp, Option<V> Value) Proposed) =>
            Proposed.TimeStamp >= Current.TimeStamp
                ? Proposed
                : Current;
    }

    /// <summary>
    /// First-write-wins conflict resolver
    /// </summary>
    public struct FirstWriteWins<V> : Conflict<V>
    {
        public (long TimeStamp, Option<V> Value) Resolve((long TimeStamp, Option<V> Value) Current, (long TimeStamp, Option<V> Value) Proposed) =>
            Current.TimeStamp <= Proposed.TimeStamp
                ? Current
                : Proposed;
    }
}
