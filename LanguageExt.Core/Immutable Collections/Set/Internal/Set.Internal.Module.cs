#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System;
using System.Collections.Generic;
using System.Collections;
using static LanguageExt.Prelude;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt;

internal static class SetModule
{
    [Pure]
    public static S Fold<S, K>(SetItem<K> node, S state, Func<S, K, S> folder)
    {
        if (node.IsEmpty)
        {
            return state;
        }
        state = Fold(node.Left, state, folder);
        state = folder(state, node.Key);
        state = Fold(node.Right, state, folder);
        return state;
    }

    [Pure]
    public static S FoldBack<S, K>(SetItem<K> node, S state, Func<S, K, S> folder)
    {
        if (node.IsEmpty)
        {
            return state;
        }
        state = FoldBack(node.Right, state, folder);
        state = folder(state, node.Key);
        state = FoldBack(node.Left, state, folder);
        return state;
    }

    [Pure]
    public static bool ForAll<K>(SetItem<K> node, Func<K, bool> pred) =>
        node.IsEmpty || pred(node.Key) && ForAll(node.Left, pred) && ForAll(node.Right, pred);

    [Pure]
    public static bool Exists<K>(SetItem<K> node, Func<K, bool> pred) =>
        !node.IsEmpty && (pred(node.Key) || Exists(node.Left, pred) || Exists(node.Right, pred));

    [Pure]
    public static SetItem<K> Add<OrdK, K>(SetItem<K> node, K key) where OrdK : Ord<K>
    {
        if (node.IsEmpty)
        {
            return new SetItem<K>(1, 1, key, SetItem<K>.Empty, SetItem<K>.Empty);
        }
        var cmp = OrdK.Compare(key, node.Key);
        if (cmp < 0)
        {
            return Balance(Make(node.Key, Add<OrdK, K>(node.Left, key), node.Right));
        }
        else if (cmp > 0)
        {
            return Balance(Make(node.Key, node.Left, Add<OrdK, K>(node.Right, key)));
        }
        else
        {
            throw new ArgumentException("An element with the same key already exists in the set");
        }
    }

    [Pure]
    public static SetItem<K> TryAdd<OrdK, K>(SetItem<K> node, K key) where OrdK : Ord<K>
    {
        if (node.IsEmpty)
        {
            return new SetItem<K>(1, 1, key, SetItem<K>.Empty, SetItem<K>.Empty);
        }
        var cmp = OrdK.Compare(key, node.Key);
        if (cmp < 0)
        {
            return Balance(Make(node.Key, TryAdd<OrdK, K>(node.Left, key), node.Right));
        }
        else if (cmp > 0)
        {
            return Balance(Make(node.Key, node.Left, TryAdd<OrdK, K>(node.Right, key)));
        }
        else
        {
            return node;
        }
    }

    [Pure]
    public static SetItem<K> AddOrUpdate<OrdK, K>(SetItem<K> node, K key) where OrdK : Ord<K>
    {
        if (node.IsEmpty)
        {
            return new SetItem<K>(1, 1, key, SetItem<K>.Empty, SetItem<K>.Empty);
        }
        var cmp = OrdK.Compare(key, node.Key);
        if (cmp < 0)
        {
            return Balance(Make(node.Key, TryAdd<OrdK, K>(node.Left, key), node.Right));
        }
        else if (cmp > 0)
        {
            return Balance(Make(node.Key, node.Left, TryAdd<OrdK, K>(node.Right, key)));
        }
        else
        {
            return new SetItem<K>(node.Height, node.Count, key, node.Left, node.Right);
        }
    }

    [Pure]
    public static SetItem<K> AddTreeToRight<K>(SetItem<K> node, SetItem<K> toAdd) =>
        node.IsEmpty
            ? toAdd
            : Balance(Make(node.Key, node.Left, AddTreeToRight(node.Right, toAdd)));

    [Pure]
    public static SetItem<K> Remove<OrdK, K>(SetItem<K> node, K key) where OrdK : Ord<K>
    {
        if (node.IsEmpty)
        {
            return node;
        }
        var cmp = OrdK.Compare(key, node.Key);
        if (cmp < 0)
        {
            return Balance(Make(node.Key, Remove<OrdK, K>(node.Left, key), node.Right));
        }
        else if (cmp > 0)
        {
            return Balance(Make(node.Key, node.Left, Remove<OrdK, K>(node.Right, key)));
        }
        else
        {
            return Balance(AddTreeToRight(node.Left, node.Right));
        }
    }

    [Pure]
    public static bool Contains<OrdK, K>(SetItem<K> node, K key) where OrdK : Ord<K>
    {
        if (node.IsEmpty)
        {
            return false;
        }
        var cmp = OrdK.Compare(key, node.Key);
        if (cmp < 0)
        {
            return Contains<OrdK, K>(node.Left, key);
        }
        else if (cmp > 0)
        {
            return Contains<OrdK, K>(node.Right, key);
        }
        else
        {
            return true;
        }
    }

    [Pure]
    public static K Find<OrdK, K>(SetItem<K> node, K key) where OrdK : Ord<K>
    {
        if (node.IsEmpty)
        {
            throw new ArgumentException("Key not found in set");
        }
        var cmp = OrdK.Compare(key, node.Key);
        if (cmp < 0)
        {
            return Find<OrdK, K>(node.Left, key);
        }
        else if (cmp > 0)
        {
            return Find<OrdK, K>(node.Right, key);
        }
        else
        {
            return node.Key;
        }
    }

    /// <summary>
    /// TODO: I suspect this is suboptimal, it would be better with a custom Enumerator 
    /// that maintains a stack of nodes to retrace.
    /// </summary>
    [Pure]
    public static IEnumerable<K> FindRange<OrdK, K>(SetItem<K> node, K a, K b) where OrdK : Ord<K>
    {
        if (node.IsEmpty)
        {
            yield break;
        }
        if (OrdK.Compare(node.Key, a) < 0)
        {
            foreach (var item in FindRange<OrdK, K>(node.Right, a, b))
            {
                yield return item;
            }
        }
        else if (OrdK.Compare(node.Key, b) > 0)
        {
            foreach (var item in FindRange<OrdK, K>(node.Left, a, b))
            {
                yield return item;
            }
        }
        else
        {
            foreach (var item in FindRange<OrdK, K>(node.Left, a, b))
            {
                yield return item;
            }
            yield return node.Key;
            foreach (var item in FindRange<OrdK, K>(node.Right, a, b))
            {
                yield return item;
            }
        }
    }

    [Pure]
    public static Option<K> TryFind<OrdK, K>(SetItem<K> node, K key) where OrdK : Ord<K>
    {
        if (node.IsEmpty)
        {
            return None;
        }
        var cmp = OrdK.Compare(key, node.Key);
        if (cmp < 0)
        {
            return TryFind<OrdK, K>(node.Left, key);
        }
        else if (cmp > 0)
        {
            return TryFind<OrdK, K>(node.Right, key);
        }
        else
        {
            return Some(node.Key);
        }
    }

    [Pure]
    public static SetItem<K> Skip<K>(SetItem<K> node, int amount)
    {
        if (amount == 0 || node.IsEmpty)
        {
            return node;
        }
        if (amount >= node.Count)
        {
            return SetItem<K>.Empty;
        }
        if (!node.Left.IsEmpty && node.Left.Count == amount)
        {
            return Balance(Make(node.Key, SetItem<K>.Empty, node.Right));
        }
        if (!node.Left.IsEmpty && node.Left.Count == amount - 1)
        {
            return node.Right;
        }
        if (node.Left.IsEmpty)
        {
            return Skip(node.Right, amount - 1);
        }

        var newleft   = Skip(node.Left, amount);
        var remaining = amount - node.Left.Count - newleft.Count;
        if (remaining > 0)
        {
            return Skip(Balance(Make(node.Key, newleft, node.Right)), remaining);
        }
        else
        {
            return Balance(Make(node.Key, newleft, node.Right));
        }
    }

    [Pure]
    public static SetItem<K> Make<K>(K k, SetItem<K> l, SetItem<K> r) =>
        new ((byte)(1 + Math.Max(l.Height, r.Height)), l.Count + r.Count + 1, k, l, r);

    [Pure]
    public static SetItem<K> Balance<K>(SetItem<K> node) =>
        node.BalanceFactor >= 2
            ? node.Right.BalanceFactor < 0
                  ? DblRotLeft(node)
                  : RotLeft(node)
            : node.BalanceFactor <= -2
                ? node.Left.BalanceFactor > 0
                      ? DblRotRight(node)
                      : RotRight(node)
                : node;

    [Pure]
    public static SetItem<K> RotRight<K>(SetItem<K> node) =>
        node.IsEmpty || node.Left.IsEmpty
            ? node
            : Make(node.Left.Key, node.Left.Left, Make(node.Key, node.Left.Right, node.Right));

    [Pure]
    public static SetItem<K> RotLeft<K>(SetItem<K> node) =>
        node.IsEmpty || node.Right.IsEmpty
            ? node
            : Make(node.Right.Key, Make(node.Key, node.Left, node.Right.Left), node.Right.Right);

    [Pure]
    public static SetItem<K> DblRotRight<K>(SetItem<K> node) =>
        node.IsEmpty
            ? node
            : RotRight(Make(node.Key, RotLeft(node.Left), node.Right));

    [Pure]
    public static SetItem<K> DblRotLeft<K>(SetItem<K> node) =>
        node.IsEmpty
            ? node
            : RotLeft(Make(node.Key, node.Left, RotRight(node.Right)));

    internal static Option<A> Max<A>(SetItem<A> node) =>
        node.Right.IsEmpty
            ? node.Key
            : Max(node.Right);

    internal static Option<A> Min<A>(SetItem<A> node) =>
        node.Left.IsEmpty
            ? node.Key
            : Min(node.Left);

    internal static Option<A> TryFindPredecessor<OrdA, A>(SetItem<A> root, A key) where OrdA : Ord<A>
    {
        Option<A> predecessor = None;
        var       current     = root;

        if (root.IsEmpty)
        {
            return None;
        }

        do
        {
            var cmp = OrdA.Compare(key, current.Key);
            if (cmp < 0)
            {
                current = current.Left;
            }
            else if (cmp > 0)
            {
                predecessor = current.Key;
                current = current.Right;
            }
            else
            {
                break;
            }
        }
        while (!current.IsEmpty);

        if(!current.IsEmpty && !current.Left.IsEmpty)
        {
            predecessor = Max(current.Left);
        }

        return predecessor;
    }

    internal static Option<A> TryFindOrPredecessor<OrdA, A>(SetItem<A> root, A key) where OrdA : Ord<A>
    {
        Option<A> predecessor = None;
        var       current     = root;

        if (root.IsEmpty)
        {
            return None;
        }

        do
        {
            var cmp = OrdA.Compare(key, current.Key);
            if (cmp < 0)
            {
                current = current.Left;
            }
            else if (cmp > 0)
            {
                predecessor = current.Key;
                current = current.Right;
            }
            else
            {
                return current.Key;
            }
        }
        while (!current.IsEmpty);

        if (!current.IsEmpty && !current.Left.IsEmpty)
        {
            predecessor = Max(current.Left);
        }

        return predecessor;
    }

    internal static Option<A> TryFindSuccessor<OrdA, A>(SetItem<A> root, A key) where OrdA : Ord<A>
    {
        Option<A> successor = None;
        var       current   = root;

        if (root.IsEmpty)
        {
            return None;
        }

        do
        {
            var cmp = OrdA.Compare(key, current.Key);
            if (cmp < 0)
            {
                successor = current.Key;
                current = current.Left;
            }
            else if (cmp > 0)
            {
                current = current.Right;
            }
            else
            {
                break;
            }
        }
        while (!current.IsEmpty);

        if (!current.IsEmpty && !current.Right.IsEmpty)
        {
            successor = Min(current.Right);
        }

        return successor;
    }

    internal static Option<A> TryFindOrSuccessor<OrdA, A>(SetItem<A> root, A key) where OrdA : Ord<A>
    {
        Option<A> successor = None;
        var       current   = root;

        if (root.IsEmpty)
        {
            return None;
        }

        do
        {
            var cmp = OrdA.Compare(key, current.Key);
            if (cmp < 0)
            {
                successor = current.Key;
                current = current.Left;
            }
            else if (cmp > 0)
            {
                current = current.Right;
            }
            else
            {
                return current.Key;
            }
        }
        while (!current.IsEmpty);

        if (!current.IsEmpty && !current.Right.IsEmpty)
        {
            successor = Min(current.Right);
        }

        return successor;
    }

    public class SetEnumerator<K> : IEnumerator<K>
    {
        internal struct NewStack : New<SetItem<K>[]>
        {
            public SetItem<K>[] New() =>
                new SetItem<K>[32];
        }

        int stackDepth;
        SetItem<K>[]? stack;
        readonly SetItem<K> map;
        int left;
        readonly bool rev;
        readonly int start;

        public SetEnumerator(SetItem<K> root, bool rev, int start)
        {
            this.rev = rev;
            this.start = start;
            map = root;
            stack = Pool<NewStack, SetItem<K>[]>.Pop();
            NodeCurrent = default!;
            Reset();
        }

        private SetItem<K> NodeCurrent
        {
            get;
            set;
        }

        public K Current => NodeCurrent.Key;
        object IEnumerator.Current => NodeCurrent.Key!;

        public void Dispose()
        {
            if (stack is not null)
            {
                Pool<NewStack, SetItem<K>[]>.Push(stack);
                stack = default!;
            }
        }

        private SetItem<K> Next(SetItem<K> node) =>
            rev ? node.Left : node.Right;

        private SetItem<K> Prev(SetItem<K> node) =>
            rev ? node.Right : node.Left;

        private void Push(SetItem<K> node)
        {
            while (!node.IsEmpty)
            {
                stack![stackDepth] = node;
                stackDepth++;
                node = Prev(node);
            }
        }

        public bool MoveNext()
        {
            if (left > 0 && stackDepth > 0)
            {
                stackDepth--;
                NodeCurrent = stack![stackDepth];
                Push(Next(NodeCurrent));
                left--;
                return true;
            }

            NodeCurrent = default!;
            return false;
        }

        public void Reset()
        {
            var skip = rev ? map.Count - start - 1 : start;

            stackDepth = 0;
            NodeCurrent = map;
            left = map.Count;

            while (!NodeCurrent.IsEmpty && skip != Prev(NodeCurrent).Count)
            {
                if (skip < Prev(NodeCurrent).Count)
                {
                    stack![stackDepth] = NodeCurrent;
                    stackDepth++;
                    NodeCurrent = Prev(NodeCurrent);
                }
                else
                {
                    skip -= Prev(NodeCurrent).Count + 1;
                    NodeCurrent = Next(NodeCurrent);
                }
            }

            if (!NodeCurrent.IsEmpty)
            {
                stack![stackDepth] = NodeCurrent;
                stackDepth++;
            }
        }
    }
}
