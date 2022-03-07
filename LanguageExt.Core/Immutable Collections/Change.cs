using System;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Represents changes to a collection type (i.e. a key-value collection)
    /// </summary>
    /// <typeparam name="V">Value type</typeparam>
    public abstract class Change<V> :
        IEquatable<Change<V>>
    {
        /// <summary>
        /// Returns true if nothing has changed
        /// </summary>
        public bool HasNoChange => this is NoChange<V>;

        /// <summary>
        /// Returns true if anything has changed (add, update, or removal)
        /// </summary>
        public bool HasChanged => !HasNoChange;
        
        /// <summary>
        /// Returns true if a value has been removed
        /// </summary>
        public bool HasRemoved => this is ItemRemoved<V>;
        
        /// <summary>
        /// Returns true if a value has been added
        /// </summary>
        public bool HasUpdated => this is ItemUpdated<V>;
        
        /// <summary>
        /// Returns true if a value has been added
        /// </summary>
        public bool HasAdded => this is ItemAdded<V>;

        /// <summary>
        /// If a value has been updated this will return Some(Value), else none
        /// </summary>
        public Option<V> ToOption() =>
            this switch
            {
                ItemUpdated<V>(_, var v) => Some(v),
                ItemMapped<V>(var v) => Some(v),
                _ => Option<V>.None
            };

        public override bool Equals(object obj) =>
            obj is Change<V> rhs && Equals(rhs);

        public abstract bool Equals(Change<V> obj);

        public override int GetHashCode() => FNV32.OffsetBasis;

        /// <summary>
        /// Returns a `NoChange` state
        /// </summary>
        public static Change<V> None => NoChange<V>.Default;

        /// <summary>
        /// Returns a `ItemRemoved` state
        /// </summary>
        public static Change<V> Removed(V oldValue) => new ItemRemoved<V>(oldValue);

        /// <summary>
        /// Returns a `ItemAdded` state
        /// </summary>
        public static Change<V> Added(V value) => new ItemAdded<V>(value);

        /// <summary>
        /// Returns a `ItemUpdated` state
        /// </summary>
        public static Change<V> Updated(V oldValue, V value) => new ItemUpdated<V>(oldValue, value);

        /// <summary>
        /// Returns a `ItemMapped` state
        /// </summary>
        public static Change<V> Mapped<OV>(OV oldValue, V value) => new ItemMapped<OV, V>(oldValue, value);
    }

    /// <summary>
    /// Existing item updated in a map
    /// </summary>
    /// <param name="OldValue">Value as it was before the change</param>
    /// <param name="Value">Value</param>
    /// <typeparam name="B">New value type</typeparam>
    public abstract class ItemMapped<V> : 
        Change<V>, 
        IEquatable<ItemMapped<V>>
    {
        public readonly V Value;

        protected ItemMapped(V Value) =>
            this.Value = Value;

        public override bool Equals(Change<V> obj) =>
            obj is ItemMapped<V> rhs && Equals(rhs);

        public bool Equals(ItemMapped<V> rhs) =>
            rhs != null &&
            default(EqDefault<V>).Equals(Value, rhs.Value);

        public void Deconstruct(out V value) =>
            value = Value;

        public override int GetHashCode() => Value?.GetHashCode() ?? FNV32.OffsetBasis;
    }
    
    /// <summary>
    /// Existing item updated in a map
    /// </summary>
    /// <param name="OldValue">Value as it was before the change</param>
    /// <param name="Value">Value</param>
    /// <typeparam name="A">Old value type</typeparam>
    /// <typeparam name="B">New value type</typeparam>
    public sealed class ItemMapped<A, B> : 
        ItemMapped<B>, 
        IEquatable<ItemMapped<A, B>>
    {
        public readonly A OldValue;

        public ItemMapped(A oldValue, B value) : base(value) =>
            OldValue = oldValue;

        public override bool Equals(Change<B> obj) =>
            obj is ItemMapped<A, B> rhs && Equals(rhs);

        public bool Equals(ItemMapped<A, B> rhs) =>
            rhs != null &&
            default(EqDefault<A>).Equals(OldValue, rhs.OldValue) &&
            default(EqDefault<B>).Equals(Value, rhs.Value);

        public override int GetHashCode() =>
            FNV32.Next(
                OldValue?.GetHashCode() ?? FNV32.OffsetBasis,
                Value?.GetHashCode() ?? FNV32.OffsetBasis);

        public void Deconstruct(out A oldValue, out B value)
        {
            oldValue = OldValue;
            value = Value;
        }
    }

    /// <summary>
    /// Existing item updated in a map
    /// </summary>
    /// <param name="OldValue">Value as it was before the change</param>
    /// <param name="Value">Value</param>
    /// <typeparam name="V">Value type</typeparam>
    public sealed class ItemUpdated<V> : 
        Change<V>, 
        IEquatable<ItemUpdated<V>>
    {
        public readonly V OldValue;
        public readonly V Value;

        public ItemUpdated(V oldValue, V value)
        {
            OldValue = oldValue;
            Value = value;
        }

        public override bool Equals(Change<V> obj) =>
            obj is ItemUpdated<V> rhs && Equals(rhs);

        public bool Equals(ItemUpdated<V> rhs) =>
            rhs != null &&
            default(EqDefault<V>).Equals(OldValue, rhs.OldValue) &&
            default(EqDefault<V>).Equals(Value, rhs.Value);

        public override int GetHashCode() =>
            FNV32.Next(
                OldValue?.GetHashCode() ?? FNV32.OffsetBasis,
                Value?.GetHashCode() ?? FNV32.OffsetBasis);

        public void Deconstruct(out V oldValue, out V value)
        {
            oldValue = OldValue;
            value = Value;
        }
    }

    /// <summary>
    /// Item added to a map
    /// </summary>
    /// <param name="Value">Value</param>
    /// <typeparam name="V">Value type</typeparam>
    public sealed class ItemAdded<V> :
        Change<V>, 
        IEquatable<ItemAdded<V>>
    {
        public readonly V Value;

        public ItemAdded(V value) =>
            Value = value;

        public override bool Equals(Change<V> obj) =>
            obj is ItemAdded<V> rhs && Equals(rhs);

        public bool Equals(ItemAdded<V> rhs) =>
            rhs != null &&
            default(EqDefault<V>).Equals(Value, rhs.Value);

        public override int GetHashCode() =>
            Value?.GetHashCode() ?? FNV32.OffsetBasis;

        public void Deconstruct(out V value)
        {
            value = Value;
        }
    }

    /// <summary>
    /// Existing item removed from a map
    /// </summary>
    /// <typeparam name="V">Value type</typeparam>
    public sealed class ItemRemoved<V> : 
        Change<V>, 
        IEquatable<ItemRemoved<V>>
    {
        public readonly V OldValue;

        public ItemRemoved(V oldValue) =>
            OldValue = oldValue;

        public override bool Equals(Change<V> obj) =>
            obj is ItemRemoved<V> rhs && Equals(rhs);

        public override int GetHashCode() => 
            OldValue?.GetHashCode() ?? FNV32.OffsetBasis;
        
        public bool Equals(ItemRemoved<V> rhs) =>
            rhs != null &&
            default(EqDefault<V>).Equals(OldValue, rhs.OldValue);
    }

    /// <summary>
    /// No change to the collection
    /// </summary>
    /// <typeparam name="V">Value type</typeparam>
    public sealed class NoChange<V> : 
        Change<V>, 
        IEquatable<NoChange<V>>
    {
        public static readonly Change<V> Default = new NoChange<V>();

        public override bool Equals(Change<V> obj) =>
            obj is NoChange<V>;

        public bool Equals(NoChange<V> rhs) =>
            rhs != null;

        public override int GetHashCode() => 
            FNV32.OffsetBasis;
    }
}
