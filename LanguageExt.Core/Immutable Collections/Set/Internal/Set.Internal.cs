#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;
using L = LanguageExt;

namespace LanguageExt;

/// <summary>
/// Immutable set
/// AVL tree implementation
/// AVL tree is a self-balancing binary search tree. 
/// [wikipedia.org/wiki/AVL_tree](http://en.wikipedia.org/wiki/AVL_tree)
/// </summary>
/// <typeparam name="A">List item type</typeparam>
[Serializable]
internal class SetInternal<OrdA, A> :
    IEnumerable<A>,
    IEquatable<SetInternal<OrdA, A>>
    where OrdA : Ord<A>
{
    public static readonly SetInternal<OrdA, A> Empty = new ();
    internal readonly SetItem<A> Root;
    int hashCode;

    /// <summary>
    /// Default ctor
    /// </summary>
    internal SetInternal() => Root = SetItem<A>.Empty;

    /// <summary>
    /// Ctor that takes a root element
    /// </summary>
    /// <param name="root"></param>
    internal SetInternal(SetItem<A> root) => 
        Root = root;

    /// <summary>
    /// Ctor from an enumerable 
    /// </summary>
    public SetInternal(IEnumerable<A> items) : this(items, SetModuleM.AddOpt.TryAdd)
    {
    }

    public override int GetHashCode() =>
        hashCode == 0
            ? hashCode = FNV32.Hash<OrdA, A>(AsIterable())
            : hashCode;

    public Iterable<A> AsIterable() =>
        new Iterator<A>.IterSetFwd(Set.IteratorState<A>.Setup(Root)).AsIterable();

    public SetInternal<OrdA, A> Skip(long amount)
    {
        var skip = amount;
        var node = Root;

        while (!node.IsEmpty && skip != node.Left.Count)
        {
            if (skip < node.Left.Count)
            {
                node = node.Left;
            }
            else
            {
                skip -= node.Left.Count + 1;
                node = node.Right;
            }
        }

        return new SetInternal<OrdA, A>(
            node.IsEmpty
                ? SetItem<A>.Empty
                : SetModule.Add<OrdA, A>(node.Right, node.Key));
    }

    /// <summary>
    /// Ctor that takes an initial (distinct) set of items
    /// </summary>
    /// <param name="items"></param>
    internal SetInternal(IEnumerable<A> items, SetModuleM.AddOpt option)
    {
        Root = SetItem<A>.Empty;

        foreach (var item in items)
        {
            Root = SetModuleM.Add<OrdA, A>(Root, item, option);
        }
    }

    /// <summary>
    /// Ctor that takes an initial (distinct) set of items
    /// </summary>
    /// <param name="items"></param>
    internal SetInternal(Iterator<A> items, SetModuleM.AddOpt option)
    {
        Root = SetItem<A>.Empty;

        foreach (var item in items)
        {
            Root = SetModuleM.Add<OrdA, A>(Root, item, option);
        }
    }

    /// <summary>
    /// Ctor that takes an initial (distinct) set of items
    /// </summary>
    /// <param name="items"></param>
    internal SetInternal(ReadOnlySpan<A> items, SetModuleM.AddOpt option)
    {
        Root = SetItem<A>.Empty;

        foreach (var item in items)
        {
            Root = SetModuleM.Add<OrdA, A>(Root, item, option);
        }
    }

    /// <summary>
    /// Number of items in the set
    /// </summary>
    [Pure]
    public long Count =>
        Root.Count;

    [Pure]
    public Option<A> Min => 
        Root.IsEmpty
            ? None
            : SetModule.Min(Root);

    [Pure]
    public Option<A> Max =>
        Root.IsEmpty
            ? None
            : SetModule.Max(Root);

    /// <summary>
    /// Add an item to the set
    /// </summary>
    /// <param name="value">Value to add to the set</param>
    /// <returns>New set with the item added</returns>
    [Pure]
    public SetInternal<OrdA, A> Add(A value) =>
        new (SetModule.Add<OrdA, A>(Root,value));

    /// <summary>
    /// Attempt to add an item to the set.  If an item already
    /// exists then return the Set as-is.
    /// </summary>
    /// <param name="value">Value to add to the set</param>
    /// <returns>New set with the item maybe added</returns>
    [Pure]
    public SetInternal<OrdA, A> TryAdd(A value) =>
        Contains(value)
            ? this
            : Add(value);

    /// <summary>
    /// Add an item to the set.  If an item already
    /// exists then replace it.
    /// </summary>
    /// <param name="value">Value to add to the set</param>
    /// <returns>New set with the item maybe added</returns>
    [Pure]
    public SetInternal<OrdA, A> AddOrUpdate(A value) =>
        new (SetModule.AddOrUpdate<OrdA, A>(Root, value));

    [Pure]
    public SetInternal<OrdA, A> AddRange(IEnumerable<A> xs)
    {
        if(Count == 0)
        {
            return new SetInternal<OrdA, A>(xs, SetModuleM.AddOpt.ThrowOnDuplicate);
        }

        var set = this;
        foreach(var x in xs)
        {
            set = set.Add(x);
        }
        return set;
    }

    [Pure]
    public SetInternal<OrdA, A> TryAddRange(IEnumerable<A> xs)
    {
        if (Count == 0)
        {
            return new SetInternal<OrdA, A>(xs, SetModuleM.AddOpt.TryAdd);
        }

        var set = this;
        foreach (var x in xs)
        {
            set = set.TryAdd(x);
        }
        return set;
    }

    [Pure]
    public SetInternal<OrdA, A> AddOrUpdateRange(IEnumerable<A> xs)
    {
        if (Count == 0)
        {
            return new SetInternal<OrdA, A>(xs, SetModuleM.AddOpt.TryUpdate);
        }

        var set = this;
        foreach (var x in xs)
        {
            set = set.AddOrUpdate(x);
        }
        return set;
    }

    /// <summary>
    /// Attempts to find an item in the set.  
    /// </summary>
    /// <param name="value">Value to find</param>
    /// <returns>Some(T) if found, None otherwise</returns>
    [Pure]
    public Option<A> Find(A value) =>
        SetModule.TryFind<OrdA, A>(Root, value);

    /// <summary>
    /// Retrieve the value from predecessor item to specified key
    /// </summary>
    /// <param name="key">Key to find</param>
    /// <returns>Found key</returns>
    [Pure]
    public Option<A> FindPredecessor(A key) => SetModule.TryFindPredecessor<OrdA, A>(Root, key);

    /// <summary>
    /// Retrieve the value from exact key, or if not found, the predecessor item 
    /// </summary>
    /// <param name="key">Key to find</param>
    /// <returns>Found key</returns>
    [Pure]
    public Option<A> FindOrPredecessor(A key) => SetModule.TryFindOrPredecessor<OrdA, A>(Root, key);

    /// <summary>
    /// Retrieve the value from next item to specified key
    /// </summary>
    /// <param name="key">Key to find</param>
    /// <returns>Found key</returns>
    [Pure]
    public Option<A> FindSuccessor(A key) => SetModule.TryFindSuccessor<OrdA, A>(Root, key);

    /// <summary>
    /// Retrieve the value from exact key, or if not found, the next item 
    /// </summary>
    /// <param name="key">Key to find</param>
    /// <returns>Found key</returns>
    [Pure]
    public Option<A> FindOrSuccessor(A key) => SetModule.TryFindOrSuccessor<OrdA, A>(Root, key);

    /// <summary>
    /// Retrieve a range of values 
    /// </summary>
    /// <param name="keyFrom">Range start (inclusive)</param>
    /// <param name="keyTo">Range to (inclusive)</param>
    /// <exception cref="ArgumentNullException">Throws ArgumentNullException the keyFrom or keyTo are null</exception>
    /// <returns>Range of values</returns>
    [Pure]
    public Iterable<A> FindRange(A keyFrom, A keyTo)
    {
        if (isnull(keyFrom)) throw new ArgumentNullException(nameof(keyFrom));
        if (isnull(keyTo)) throw new ArgumentNullException(nameof(keyTo));
        return OrdA.Compare(keyFrom, keyTo) > 0
                   ? SetModule.FindRange<OrdA, A>(Root, keyTo, keyFrom).AsIterable()
                   : SetModule.FindRange<OrdA, A>(Root, keyFrom, keyTo).AsIterable();
    }


    /// <summary>
    /// Returns the elements that are in both this and other
    /// </summary>
    [Pure]
    public SetInternal<OrdA, A> Intersect(IEnumerable<A> other)
    {
        var root = SetItem<A>.Empty;
        foreach (var item in other)
        {
            if (Contains(item))
            {
                root = SetModuleM.Add<OrdA, A>(root, item, SetModuleM.AddOpt.TryAdd);
            }
        }
        return new SetInternal<OrdA, A>(root);
    }

    /// <summary>
    /// Returns this - other.  Only the items in this that are not in 
    /// other will be returned.
    /// </summary>
    [Pure]
    public SetInternal<OrdA, A> Except(SetInternal<OrdA, A> rhs)
    {
        var root = SetItem<A>.Empty;
        foreach (var item in this)
        {
            if (!rhs.Contains(item))
            {
                root = SetModuleM.Add<OrdA, A>(root, item, SetModuleM.AddOpt.TryAdd);
            }
        }
        return new SetInternal<OrdA, A>(root);
    }

    /// <summary>
    /// Returns this - other.  Only the items in this that are not in 
    /// other will be returned.
    /// </summary>
    [Pure]
    public SetInternal<OrdA, A> Except(IEnumerable<A> other) =>
        Except(new SetInternal<OrdA, A>(other));

    /// <summary>
    /// Only items that are in one set or the other will be returned.
    /// If an item is in both, it is dropped.
    /// </summary>
    [Pure]
    public SetInternal<OrdA, A> SymmetricExcept(SetInternal<OrdA, A> rhs)
    {
        var root = SetItem<A>.Empty;

        foreach (var item in this)
        {
            if (!rhs.Contains(item))
            {
                root = SetModuleM.Add<OrdA, A>(root, item, SetModuleM.AddOpt.TryAdd);
            }
        }

        foreach (var item in rhs)
        {
            if (!Contains(item))
            {
                root = SetModuleM.Add<OrdA, A>(root, item, SetModuleM.AddOpt.TryAdd);
            }
        }

        return new SetInternal<OrdA, A>(root);
    }

    /// <summary>
    /// Only items that are in one set or the other will be returned.
    /// If an item is in both, it is dropped.
    /// </summary>
    [Pure]
    public SetInternal<OrdA, A> SymmetricExcept(IEnumerable<A> other) =>
        SymmetricExcept(new SetInternal<OrdA, A>(other));

    /// <summary>
    /// Finds the union of two sets and produces a new set with 
    /// the results
    /// </summary>
    /// <param name="other">Other set to union with</param>
    /// <returns>A set which contains all items from both sets</returns>
    [Pure]
    public SetInternal<OrdA, A> Union(IEnumerable<A> other)
    {
        var root = SetItem<A>.Empty;

        foreach(var item in this)
        {
            root = SetModuleM.Add<OrdA, A>(root, item, SetModuleM.AddOpt.TryAdd);
        }

        foreach (var item in other)
        {
            root = SetModuleM.Add<OrdA, A>(root, item, SetModuleM.AddOpt.TryAdd);
        }

        return new SetInternal<OrdA, A>(root);
    }

    /// <summary>
    /// Clears the set
    /// </summary>
    /// <returns>An empty set</returns>
    [Pure]
    public SetInternal<OrdA, A> Clear() =>
        Empty;

    /// <summary>
    /// Get enumerator
    /// </summary>
    /// <returns>IEnumerator T</returns>
    [Pure]
    public IEnumerator<A> GetEnumerator() =>
        new SetModule.SetEnumerator<A>(Root, false, 0);

    /// <summary>
    /// Removes an item from the set (if it exists)
    /// </summary>
    /// <param name="value">Value to check</param>
    /// <returns>New set with item removed</returns>
    [Pure]
    public SetInternal<OrdA, A> Remove(A value) =>
        new (SetModule.Remove<OrdA, A>(Root, value));

    /// <summary>
    /// Applies a function 'folder' to each element of the collection, threading an accumulator 
    /// argument through the computation. The fold function takes the state argument, and 
    /// applies the function 'folder' to it and the first element of the set. Then, it feeds this 
    /// result into the function 'folder' along with the second element, and so on. It returns the 
    /// final result. (Aggregate in LINQ)
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public S Fold<S>(S state, Func<S, A, S> folder) =>
        SetModule.Fold(Root,state,folder);

    /// <summary>
    /// Applies a function 'folder' to each element of the collection (from last element to first), 
    /// threading an aggregate state through the computation. The fold function takes the state 
    /// argument, and applies the function 'folder' to it and the first element of the set. Then, 
    /// it feeds this result into the function 'folder' along with the second element, and so on. It 
    /// returns the final result.
    /// </summary>
    /// <typeparam name="S">State type</typeparam>
    /// <param name="state">Initial state</param>
    /// <param name="folder">Fold function</param>
    /// <returns>Aggregate value</returns>
    [Pure]
    public S FoldBack<S>(S state, Func<S, A, S> folder) =>
        SetModule.FoldBack(Root, state, folder);

    /// <summary>
    /// Maps the values of this set into a new set of values using the
    /// mapper function to tranform the source values.
    /// </summary>
    /// <typeparam name="R">Mapped element type</typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped Set</returns>
    [Pure]
    public SetInternal<OrdB, B> Map<OrdB, B>(Func<A, B> f) where OrdB : Ord<B> =>
        new (AsIterable().Map(f));

    /// <summary>
    /// Maps the values of this set into a new set of values using the
    /// mapper function to tranform the source values.
    /// </summary>
    /// <typeparam name="R">Mapped element type</typeparam>
    /// <param name="f">Mapping function</param>
    /// <returns>Mapped Set</returns>
    [Pure]
    public SetInternal<OrdA, A> Map(Func<A, A> f) =>
        new (AsIterable().Map(f));

    /// <summary>
    /// Filters items from the set using the predicate.  If the predicate
    /// returns True for any item then it remains in the set, otherwise
    /// it's dropped.
    /// </summary>
    /// <param name="pred">Predicate</param>
    /// <returns>Filtered enumerable</returns>
    [Pure]
    public SetInternal<OrdA, A> Filter(Func<A, bool> pred) =>
        new (AsIterable().Filter(pred), SetModuleM.AddOpt.TryAdd);

    /// <summary>
    /// Check the existence of an item in the set using a 
    /// predicate.
    /// </summary>
    /// <remarks>Note this scans the entire set.</remarks>
    /// <param name="pred">Predicate</param>
    /// <returns>True if predicate returns true for any item</returns>
    [Pure]
    public bool Exists(Func<A, bool> pred) =>
        SetModule.Exists(Root, pred);

    /// <summary>
    /// Returns True if the value is in the set
    /// </summary>
    /// <param name="value">Value to check</param>
    /// <returns>True if the item 'value' is in the Set 'set'</returns>
    [Pure]
    public bool Contains(A value) =>
        SetModule.Contains<OrdA, A>(Root, value);

    /// <summary>
    /// Returns true if both sets contain the same elements
    /// </summary>
    /// <param name="other">Other distinct set to compare</param>
    /// <returns>True if the sets are equal</returns>
    [Pure]
    public bool SetEquals(IEnumerable<A> other)
    {
        var rhs = new SetInternal<OrdA, A>(other);
        if (rhs.Count != Count) return false;
        foreach (var item in rhs)
        {
            if (!Contains(item)) return false;
        }
        return true;
    }

    /// <summary>
    /// True if the set has no elements
    /// </summary>
    [Pure]
    public bool IsEmpty => 
        Count == 0;

    /// <summary>
    /// IsReadOnly - Always true
    /// </summary>
    [Pure]
    public bool IsReadOnly
    {
        get
        {
            return true;
        }
    }

    /// <summary>
    /// Returns True if 'other' is a proper subset of this set
    /// </summary>
    /// <returns>True if 'other' is a proper subset of this set</returns>
    [Pure]
    public bool IsProperSubsetOf(IEnumerable<A> other)
    {
        if (IsEmpty)
        {
            return other.Any();
        }

        var otherSet = new Set<A>(other);
        if (Count >= otherSet.Count)
        {
            return false;
        }

        int  matches    = 0;
        bool extraFound = false;
        foreach (A item in otherSet)
        {
            if (Contains(item))
            {
                matches++;
            }
            else
            {
                extraFound = true;
            }

            if (matches == Count && extraFound)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns True if 'other' is a proper superset of this set
    /// </summary>
    /// <returns>True if 'other' is a proper superset of this set</returns>
    [Pure]
    public bool IsProperSupersetOf(IEnumerable<A> other)
    {
        if (IsEmpty)
        {
            return false;
        }

        int matchCount = 0;
        foreach (A item in other)
        {
            matchCount++;
            if (!Contains(item))
            {
                return false;
            }
        }

        return Count > matchCount;
    }

    /// <summary>
    /// Returns True if 'other' is a superset of this set
    /// </summary>
    /// <returns>True if 'other' is a superset of this set</returns>
    [Pure]
    public bool IsSubsetOf(IEnumerable<A> other)
    {
        if (IsEmpty)
        {
            return true;
        }

        var otherSet = new SetInternal<OrdA, A>(other);
        int matches  = 0;
        foreach (A item in otherSet)
        {
            if (Contains(item))
            {
                matches++;
            }
        }
        return matches == Count;
    }

    /// <summary>
    /// Returns True if 'other' is a superset of this set
    /// </summary>
    /// <returns>True if 'other' is a superset of this set</returns>
    [Pure]
    public bool IsSupersetOf(IEnumerable<A> other)
    {
        foreach (A item in other)
        {
            if (!Contains(item))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Returns True if other overlaps this set
    /// </summary>
    /// <typeparam name="T">Element type</typeparam>
    /// <param name="setA">Set A</param>
    /// <param name="setB">Set B</param>
    /// <returns>True if other overlaps this set</returns>
    [Pure]
    public bool Overlaps(IEnumerable<A> other)
    {
        if (IsEmpty)
        {
            return false;
        }

        foreach (A item in other)
        {
            if (Contains(item))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Copy the items from the set into the specified array
    /// </summary>
    /// <param name="array">Array to copy to</param>
    /// <param name="index">Index into the array to start</param>
    public void CopyTo(A[] array, int index)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (index < 0 || index > array.Length) throw new IndexOutOfRangeException();
        if (index + Count > array.Length) throw new IndexOutOfRangeException();

        foreach (var element in this)
        {
            array[index++] = element;
        }
    }

    /// <summary>
    /// Copy the items from the set into the specified array
    /// </summary>
    /// <param name="array">Array to copy to</param>
    /// <param name="index">Index into the array to start</param>
    public void CopyTo(Array array, int index)
    {
        if (array == null) throw new ArgumentNullException(nameof(array));
        if (index < 0 || index > array.Length) throw new IndexOutOfRangeException();
        if (index + Count > array.Length) throw new IndexOutOfRangeException();

        foreach (var element in this)
        {
            array.SetValue(element, index++);
        }
    }

    /// <summary>
    /// Add operator + performs a union of the two sets
    /// </summary>
    /// <param name="lhs">Left hand side set</param>
    /// <param name="rhs">Right hand side set</param>
    /// <returns>Unioned set</returns>
    [Pure]
    public static SetInternal<OrdA, A> operator +(SetInternal<OrdA, A> lhs, SetInternal<OrdA, A> rhs) =>
        lhs.Append(rhs);

    /// <summary>
    /// Append performs a union of the two sets
    /// </summary>
    /// <param name="rhs">Right hand side set</param>
    /// <returns>Unioned set</returns>
    [Pure]
    public SetInternal<OrdA, A> Append(SetInternal<OrdA, A> rhs) =>
        Union(rhs.AsIterable());

    /// <summary>
    /// Subtract operator - performs a subtract of the two sets
    /// </summary>
    /// <param name="lhs">Left hand side set</param>
    /// <param name="rhs">Right hand side set</param>
    /// <returns>Subtractd set</returns>
    [Pure]
    public static SetInternal<OrdA, A> operator -(SetInternal<OrdA, A> lhs, SetInternal<OrdA, A> rhs) =>
        lhs.Subtract(rhs);

    /// <summary>
    /// Subtract operator - performs a subtract of the two sets
    /// </summary>
    /// <param name="rhs">Right hand side set</param>
    /// <returns>Subtractd set</returns>
    [Pure]
    public SetInternal<OrdA, A> Subtract(SetInternal<OrdA, A> rhs)
    {
        if (Count     == 0) return Empty;
        if (rhs.Count == 0) return this;

        if (rhs.Count < Count)
        {
            var self = this;
            foreach (var item in rhs)
            {
                self = self.Remove(item);
            }
            return self;
        }
        else
        {
            var root = SetItem<A>.Empty;
            foreach (var item in this)
            {
                if (!rhs.Contains(item))
                {
                    root = SetModuleM.Add<OrdA, A>(root, item, SetModuleM.AddOpt.TryAdd);
                }
            }
            return new SetInternal<OrdA, A>(root);
        }
    }

    /// <summary>
    /// Equality test
    /// </summary>
    /// <param name="other">Other set to test</param>
    /// <returns>True if sets are equal</returns>
    [Pure]
    public bool Equals(SetInternal<OrdA, A>? other) =>
        other is not null && SetEquals(other.AsIterable());

    [Pure]
    public int CompareTo(SetInternal<OrdA, A> other)
    {
        var cmp = Count.CompareTo(other.Count);
        if (cmp != 0) return cmp;
        using var iterA = GetEnumerator();
        using var iterB = other.GetEnumerator();
        while (iterA.MoveNext() && iterB.MoveNext())
        {
            cmp = OrdA.Compare(iterA.Current, iterB.Current);
            if (cmp != 0) return cmp;
        }
        return 0;
    }

    [Pure]
    public int CompareTo<OrdAlt>(SetInternal<OrdA, A> other) where OrdAlt : Ord<A>
    {
        var cmp = Count.CompareTo(other.Count);
        if (cmp != 0) return cmp;
        using var iterA = GetEnumerator();
        using var iterB = other.GetEnumerator();
        while (iterA.MoveNext() && iterB.MoveNext())
        {
            cmp = OrdAlt.Compare(iterA.Current, iterB.Current);
            if (cmp != 0) return cmp;
        }
        return 0;
    }
    
    IEnumerator IEnumerable.GetEnumerator() =>
        new SetModule.SetEnumerator<A>(Root, false, 0);
}
