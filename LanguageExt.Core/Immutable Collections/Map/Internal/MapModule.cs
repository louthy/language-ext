#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System;
using LanguageExt.Traits;
using System.Collections.Generic;
using static LanguageExt.Prelude;

namespace LanguageExt;

static class MapModule
{
    public static S Fold<S, K, V>(MapItem<K, V> node, S state, Func<S, K, V, S> folder)
    {
        if (node.IsEmpty)
        {
            return state;
        }

        state = Fold(node.Left, state, folder);
        state = folder(state, node.KeyValue.Key, node.KeyValue.Value);
        state = Fold(node.Right, state, folder);
        return state;
    }

    public static S Fold<S, K, V>(MapItem<K, V> node, S state, Func<S, V, S> folder)
    {
        if (node.IsEmpty)
        {
            return state;
        }

        state = Fold(node.Left, state, folder);
        state = folder(state, node.KeyValue.Value);
        state = Fold(node.Right, state, folder);
        return state;
    }

    public static bool ForAll<K, V>(MapItem<K, V> node, Func<K, V, bool> pred) =>
        node.IsEmpty || pred(node.KeyValue.Key, node.KeyValue.Value) && ForAll(node.Left, pred) && ForAll(node.Right, pred);

    public static bool Exists<K, V>(MapItem<K, V> node, Func<K, V, bool> pred) =>
        !node.IsEmpty && (pred(node.KeyValue.Key, node.KeyValue.Value) || Exists(node.Left, pred) || Exists(node.Right, pred));

    public static MapItem<K, V> Add<OrdK, K, V>(MapItem<K, V> node, K key, V value)
        where OrdK : Ord<K>
    {
        if (node.IsEmpty)
        {
            return new MapItem<K, V>(1, 1, (key, value), MapItem<K, V>.Empty, MapItem<K, V>.Empty);
        }
        return OrdK.Compare(key, node.KeyValue.Key) switch
               {
                   < 0 => Balance(Make(node.KeyValue, Add<OrdK, K, V>(node.Left, key, value), node.Right)),
                   > 0 => Balance(Make(node.KeyValue, node.Left, Add<OrdK, K, V>(node.Right, key, value))),
                   _   => throw new ArgumentException("An element with the same key already exists in the Map")
               };
    }

    public static MapItem<K, V> SetItem<OrdK, K, V>(MapItem<K, V> node, K key, V value)
        where OrdK : Ord<K>
    {
        if (node.IsEmpty)
        {
            throw new ArgumentException("Key not found in Map");
        }
        return OrdK.Compare(key, node.KeyValue.Key) switch
               {
                   < 0 => Balance(Make(node.KeyValue, SetItem<OrdK, K, V>(node.Left, key, value), node.Right)),
                   > 0 => Balance(Make(node.KeyValue, node.Left, SetItem<OrdK, K, V>(node.Right, key, value))),
                   _   => new MapItem<K, V>(node.Height, node.Count, (key, value), node.Left, node.Right)
               };
    }

    public static MapItem<K, V> TrySetItem<OrdK, K, V>(MapItem<K, V> node, K key, V value)
        where OrdK : Ord<K>
    {
        if (node.IsEmpty)
        {
            return node;
        }
        return OrdK.Compare(key, node.KeyValue.Key) switch
               {
                   < 0 => Balance(Make(node.KeyValue, TrySetItem<OrdK, K, V>(node.Left, key, value), node.Right)),
                   > 0 => Balance(Make(node.KeyValue, node.Left, TrySetItem<OrdK, K, V>(node.Right, key, value))),
                   _   => new MapItem<K, V>(node.Height, node.Count, (key, value), node.Left, node.Right)
               };
    }

    public static MapItem<K, V> TryAdd<OrdK, K, V>(MapItem<K, V> node, K key, V value)
        where OrdK : Ord<K>
    {
        if (node.IsEmpty)
        {
            return new MapItem<K, V>(1, 1, (key, value), MapItem<K, V>.Empty, MapItem<K, V>.Empty);
        }
        return OrdK.Compare(key, node.KeyValue.Key) switch
               {
                   < 0 => Balance(Make(node.KeyValue, TryAdd<OrdK, K, V>(node.Left, key, value), node.Right)),
                   > 0 => Balance(Make(node.KeyValue, node.Left, TryAdd<OrdK, K, V>(node.Right, key, value))),
                   _   => node
               };
    }

    public static MapItem<K, V> AddOrUpdate<OrdK, K, V>(MapItem<K, V> node, K key, V value)
        where OrdK : Ord<K>
    {
        if (node.IsEmpty)
        {
            return new MapItem<K, V>(1, 1, (key, value), MapItem<K, V>.Empty, MapItem<K, V>.Empty);
        }
        return OrdK.Compare(key, node.KeyValue.Key) switch
               {
                   < 0 => Balance(Make(node.KeyValue, AddOrUpdate<OrdK, K, V>(node.Left, key, value), node.Right)),
                   > 0 => Balance(Make(node.KeyValue, node.Left, AddOrUpdate<OrdK, K, V>(node.Right, key, value))),
                   _   => new MapItem<K, V>(node.Height, node.Count, (node.KeyValue.Key, value), node.Left, node.Right)
               };
    }

    public static MapItem<K, V> Remove<OrdK, K, V>(MapItem<K, V> node, K key)
        where OrdK : Ord<K>
    {
        if (node.IsEmpty)
        {
            return node;
        }
        var cmp = OrdK.Compare(key, node.KeyValue.Key);
        switch (cmp)
        {
            case < 0:
                return Balance(Make(node.KeyValue, Remove<OrdK, K, V>(node.Left, key), node.Right));
            
            case > 0:
                return Balance(Make(node.KeyValue, node.Left, Remove<OrdK, K, V>(node.Right, key)));
            
            default:
                switch (node.Right.IsEmpty)
                {
                    // If this is a leaf, just remove it 
                    // by returning Empty.  If we have only one child,
                    // replace the node with the child.
                    case true when node.Left.IsEmpty:
                        return MapItem<K, V>.Empty;
                
                    case true when !node.Left.IsEmpty:
                        return node.Left;
                
                    case false when node.Left.IsEmpty:
                        return node.Right;
                
                    default:
                    {
                        // We have two children. Remove the next-highest node and replace
                        // this node with it.
                        var successor = node.Right;
                        while (!successor.Left.IsEmpty)
                        {
                            successor = successor.Left;
                        }

                        var newRight = Remove<OrdK, K, V>(node.Right, successor.KeyValue.Key);
                        return Balance(Make(successor.KeyValue, node.Left, newRight));
                    }
                }
        }
    }

    public static V Find<OrdK, K, V>(MapItem<K, V> node, K key) where OrdK : Ord<K>
    {
        while (true)
        {
            if (node.IsEmpty)
            {
                throw new ArgumentException("Key not found in Map");
            }

            switch (OrdK.Compare(key, node.KeyValue.Key))
            {
                case < 0:
                    node = node.Left;
                    break;
                case > 0:
                    node = node.Right;
                    break;
                default:
                    return node.KeyValue.Value;
            }
        }
    }

    /// <summary>
    /// TODO: I suspect this is suboptimal, it would be better with a custom Enumerator 
    /// that maintains a stack of nodes to retrace.
    /// </summary>
    public static IEnumerable<V> FindRange<OrdK, K, V>(MapItem<K, V> node, K a, K b) where OrdK : Ord<K>
    {
        while (true)
        {
            if (node.IsEmpty)
            {
                yield break;
            }

            if (OrdK.Compare(node.KeyValue.Key, a) < 0)
            {
                node = node.Right;
            }
            else if (OrdK.Compare(node.KeyValue.Key, b) > 0)
            {
                node = node.Left;
            }
            else
            {
                foreach (var item in FindRange<OrdK, K, V>(node.Left, a, b))
                {
                    yield return item;
                }

                yield return node.KeyValue.Value;
                node = node.Right;
            }
        }
    }

    /// <summary>
    /// TODO: I suspect this is suboptimal, it would be better with a custom Enumerator 
    /// that maintains a stack of nodes to retrace.
    /// </summary>
    public static IEnumerable<(K, V)> FindRangePairs<OrdK, K, V>(MapItem<K, V> node, K a, K b) where OrdK : Ord<K>
    {
        while (true)
        {
            if (node.IsEmpty)
            {
                yield break;
            }

            if (OrdK.Compare(node.KeyValue.Key, a) < 0)
            {
                node = node.Right;
            }
            else if (OrdK.Compare(node.KeyValue.Key, b) > 0)
            {
                node = node.Left;
            }
            else
            {
                foreach (var item in FindRangePairs<OrdK, K, V>(node.Left, a, b))
                {
                    yield return item;
                }

                yield return node.KeyValue;
                node = node.Right;
            }
        }
    }

    public static Option<V> TryFind<OrdK, K, V>(MapItem<K, V> node, K key) where OrdK : Ord<K>
    {
        while (true)
        {
            if (node.IsEmpty)
            {
                return None;
            }

            switch (OrdK.Compare(key, node.KeyValue.Key))
            {
                case < 0:
                    node = node.Left;
                    break;
                case > 0:
                    node = node.Right;
                    break;
                default:
                    return Some(node.KeyValue.Value);
            }
        }
    }

    public static MapItem<K, V> Skip<K, V>(MapItem<K, V> node, long amount)
    {
        while (true)
        {
            if (amount == 0 || node.IsEmpty)
            {
                return node;
            }

            if (amount > node.Count)
            {
                return MapItem<K, V>.Empty;
            }

            switch (node.Left.IsEmpty)
            {
                case false when node.Left.Count == amount:
                    return Balance(Make(node.KeyValue, MapItem<K, V>.Empty, node.Right));
                
                case false when node.Left.Count == amount - 1:
                    return node.Right;
                
                case true:
                    node = node.Right;
                    amount -= 1;
                    continue;
            }

            var newleft   = Skip(node.Left, amount);
            var remaining = amount - node.Left.Count - newleft.Count;
            if (remaining > 0)
            {
                node = Balance(Make(node.KeyValue, newleft, node.Right));
                amount = remaining;
            }
            else
            {
                return Balance(Make(node.KeyValue, newleft, node.Right));
            }
        }
    }

    static MapItem<K, V> Make<K, V>((K,V) kv, MapItem<K, V> l, MapItem<K, V> r) =>
        new ((byte)(1 + Math.Max(l.Height, r.Height)), l.Count + r.Count + 1, kv, l, r);

    public static MapItem<K, V> Make<K, V>(K k, V v, MapItem<K, V> l, MapItem<K, V> r) =>
        new ((byte)(1 + Math.Max(l.Height, r.Height)), l.Count + r.Count + 1, (k, v), l, r);

    static MapItem<K, V> Balance<K, V>(MapItem<K, V> node) =>
        node.BalanceFactor >= 2
            ? node.Right.BalanceFactor < 0
                  ? DblRotLeft(node)
                  : RotLeft(node)
            : node.BalanceFactor <= -2
                ? node.Left.BalanceFactor > 0
                      ? DblRotRight(node)
                      : RotRight(node)
                : node;

    static MapItem<K, V> RotRight<K, V>(MapItem<K, V> node) =>
        node.IsEmpty || node.Left.IsEmpty
            ? node
            : Make(node.Left.KeyValue, node.Left.Left, Make(node.KeyValue, node.Left.Right, node.Right));

    static MapItem<K, V> RotLeft<K, V>(MapItem<K, V> node) =>
        node.IsEmpty || node.Right.IsEmpty
            ? node
            : Make(node.Right.KeyValue, Make(node.KeyValue, node.Left, node.Right.Left), node.Right.Right);

    static MapItem<K, V> DblRotRight<K, V>(MapItem<K, V> node) =>
        node.IsEmpty || node.Left.IsEmpty
            ? node
            : RotRight(Make(node.KeyValue, RotLeft(node.Left), node.Right));

    static MapItem<K, V> DblRotLeft<K, V>(MapItem<K, V> node) =>
        node.IsEmpty || node.Right.IsEmpty
            ? node
            : RotLeft(Make(node.KeyValue, node.Left, RotRight(node.Right)));

    internal static Option<(K, V)> Max<K, V>(MapItem<K, V> node) =>
        node.Right.IsEmpty
            ? node.KeyValue
            : Max(node.Right);

    internal static Option<(K, V)> Min<K, V>(MapItem<K, V> node) =>
        node.Left.IsEmpty
            ? node.KeyValue
            : Min(node.Left);

    internal static Option<(K, V)> TryFindPredecessor<OrdK, K, V>(MapItem<K, V> root, K key) where OrdK : Ord<K>
    {
        Option<(K, V)> predecessor = None;
        var            current     = root;

        if (root.IsEmpty)
        {
            return None;
        }

        do
        {
            var cmp = OrdK.Compare(key, current.KeyValue.Key);
            if (cmp < 0)
            {
                current = current.Left;
            }
            else if (cmp > 0)
            {
                predecessor = current.KeyValue;
                current = current.Right;
            }
            else
            {
                break;
            }
        }
        while (!current.IsEmpty);

        if (current is { IsEmpty: false, Left.IsEmpty: false })
        {
            predecessor = Max(current.Left);
        }

        return predecessor;
    }

    internal static Option<(K, V)> TryFindOrPredecessor<OrdK, K, V>(MapItem<K, V> root, K key) where OrdK : Ord<K>
    {
        Option<(K, V)> predecessor = None;
        var            current     = root;

        if (root.IsEmpty)
        {
            return None;
        }

        do
        {
            var cmp = OrdK.Compare(key, current.KeyValue.Key);
            switch (cmp)
            {
                case < 0:
                    current = current.Left;
                    break;
                case > 0:
                    predecessor = current.KeyValue;
                    current = current.Right;
                    break;
                default:
                    return current.KeyValue;
            }
        }
        while (!current.IsEmpty);

        if (current is { IsEmpty: false, Left.IsEmpty: false })
        {
            predecessor = Max(current.Left);
        }

        return predecessor;
    }

    internal static Option<(K, V)> TryFindSuccessor<OrdK, K, V>(MapItem<K, V> root, K key) where OrdK : Ord<K>
    {
        Option<(K, V)> successor = None;
        var            current   = root;

        if (root.IsEmpty)
        {
            return None;
        }

        do
        {
            var cmp = OrdK.Compare(key, current.KeyValue.Key);
            if (cmp < 0)
            {
                successor = current.KeyValue;
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

        if (current is { IsEmpty: false, Right.IsEmpty: false })
        {
            successor = Min(current.Right);
        }

        return successor;        }

    internal static Option<(K, V)> TryFindOrSuccessor<OrdK, K, V>(MapItem<K, V> root, K key) where OrdK : Ord<K>
    {
        Option<(K, V)> successor = None;
        var            current   = root;

        if (root.IsEmpty)
        {
            return None;
        }

        do
        {
            var cmp = OrdK.Compare(key, current.KeyValue.Key);
            switch (cmp)
            {
                case < 0:
                    successor = current.KeyValue;
                    current = current.Left;
                    break;
                case > 0:
                    current = current.Right;
                    break;
                default:
                    return current.KeyValue;
            }
        }
        while (!current.IsEmpty);

        if (current is { IsEmpty: false, Right.IsEmpty: false })
        {
            successor = Min(current.Right);
        }

        return successor;
    }
}
