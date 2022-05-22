using System;
using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Represents changes to a value in a collection type (i.e. a key-value collection)
    /// </summary>
    /// <typeparam name="A">Value type</typeparam>
    public abstract class Change<A> :
        IEquatable<Change<A>>
    {
        /// <summary>
        /// Returns true if nothing has changed
        /// </summary>
        public bool HasNoChange => this is NoChange<A>;

        /// <summary>
        /// Returns true if anything has changed (add, update, or removal)
        /// </summary>
        public bool HasChanged => !HasNoChange;
        
        /// <summary>
        /// Returns true if a value has been removed
        /// </summary>
        public bool HasRemoved => this is EntryRemoved<A>;
        
        /// <summary>
        /// Returns true if a value has been mapped to another
        /// </summary>
        public bool HasMapped => this is EntryMappedTo<A>;
        
        /// <summary>
        /// Returns true if a value has been mapped to another
        /// </summary>
        public bool HasMappedFrom<FROM>() => 
            this is EntryMappedFrom<FROM>;
        
        /// <summary>
        /// Returns true if a value has been added
        /// </summary>
        public bool HasAdded => this is EntryAdded<A>;

        /// <summary>
        /// If a value has been updated this will return Some(Value), else none
        /// </summary>
        public Option<A> ToOption() =>
            this switch
            {
                EntryAdded<A>(var v) => Some(v),
                EntryMappedTo<A>(var v) => Some(v),
                _ => Option<A>.None
            };

        /// <summary>
        /// Returns a `NoChange` state
        /// </summary>
        public static Change<A> None => NoChange<A>.Default;

        /// <summary>
        /// Returns a `EntryRemoved` state
        /// </summary>
        public static Change<A> Removed(A oldValue) => new EntryRemoved<A>(oldValue);

        /// <summary>
        /// Returns a `EntryAdded` state
        /// </summary>
        public static Change<A> Added(A value) => new EntryAdded<A>(value);

        /// <summary>
        /// Returns a `EntryMapped` state
        /// </summary>
        public static Change<A> Mapped<FROM>(FROM oldValue, A value) => new EntryMapped<FROM, A>(oldValue, value);
        
        /// <summary>
        /// Equality
        /// </summary>
        public override bool Equals(object obj) =>
            obj is Change<A> rhs && Equals(rhs);

        /// <summary>
        /// Equality
        /// </summary>
        public abstract bool Equals(Change<A> obj);

        /// <summary>
        /// Hash code
        /// </summary>
        public override int GetHashCode() => FNV32.OffsetBasis;
    }
}
