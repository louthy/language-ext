using System;
using LanguageExt.UnitsOfMeasure;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Holds the 'global' state for a strategy.  i.e the state that will
    /// survive between invocations of the stratgey computation.
    /// </summary>
    public class StrategyState
    {
        public readonly Time BackoffAmount;
        public readonly int Failures;
        public readonly DateTime LastFailure;
        public readonly Map<string, object> Metadata;

        public static readonly StrategyState Empty = new StrategyState(0 * seconds, 0, DateTime.MaxValue, Map.empty<string, object>());

        public StrategyState(
            Time backoffAmount,
            int failures,
            DateTime lastFailure,
            Map<string, object> metadata
            )
        {
            BackoffAmount = backoffAmount;
            Failures = failures;
            LastFailure = lastFailure;
            Metadata = metadata;
        }

        /// <summary>
        /// Adds or updates an item of meta-data
        /// 
        /// This is for extending the default strategies behaviours and 
        /// allows for state to survive in-between Process errors
        /// </summary>
        public StrategyState SetMetaData<T>(string key, T value) =>
            With(Metadata: Metadata.AddOrUpdate(key, value));

        /// <summary>
        /// Attempts to set a meta-data item.  If it is already set, nothing 
        /// happens.
        /// 
        /// This is for extending the default strategies behaviours and 
        /// allows for state to survive in-between Process errors
        /// </summary>
        public StrategyState TrySetMetaData<T>(string key, T value) =>
            With(Metadata: Metadata.TryAdd(key, value));

        /// <summary>
        /// Attempts to set a meta-data item.  If it is already set, nothing 
        /// happens.
        /// 
        /// This is for extending the default strategies behaviours and 
        /// allows for state to survive in-between Process errors
        /// </summary>
        public StrategyState RemoveMetaData<T>(string key) =>
            With(Metadata: Metadata.Remove(key));

        /// <summary>
        /// Returns True if the meta-data contains the key specified
        /// </summary>
        public bool MetaDataContains<T>(string key) =>
            Metadata.ContainsKey(key);

        public StrategyState With(
            Time? BackoffAmount = null,
            int? Failures = null,
            DateTime? LastFailure = null,
            Map<string, object>? Metadata = null
            ) =>
            new StrategyState(
                BackoffAmount ?? this.BackoffAmount,
                Failures ?? this.Failures,
                LastFailure ?? this.LastFailure,
                Metadata ?? this.Metadata
            );
    }
}
