using System;
using System.Collections.Generic;

namespace LanguageExt;

static class ListModule
{
    public static S Fold<S, T>(ListItem<T> node, S state, Func<S, T, S> folder)
    {
        if (node.IsEmpty)
        {
            return state;
        }

        state = Fold(node.Left, state, folder);
        state = folder(state, node.Value);
        state = Fold(node.Right, state, folder);
        return state;
    }

    public static bool ForAll<A>(ListItem<A> node, Func<A, bool> pred) =>
        node.IsEmpty || (pred(node.Value) && ForAll(node.Left, pred) && ForAll(node.Right, pred));

    public static bool Exists<T>(ListItem<T> node, Func<T, bool> pred) =>
        !node.IsEmpty && (pred(node.Value) || Exists(node.Left, pred) || Exists(node.Right, pred));

    public static ListItem<U> Map<T, U>(ListItem<T> node, Func<T, U> f) =>
        node.IsEmpty
            ? ListItem<U>.Empty
            : new ListItem<U>(node.Height, node.Count, Map(node.Left, f), f(node.Value), Map(node.Right, f));

    public static ListItem<A> AddRange<A>(ListItem<A> node, IEnumerable<A> items) =>
        AddRange(node, ListModuleM.BuildSubTree(items));

    static ListItem<A> AddRange<A>(ListItem<A> node, ListItem<A> insertNode) =>
        node.IsEmpty
            ? insertNode
            : Balance(Make(node.Value, node.Left, AddRange(node.Right, insertNode)));

    public static ListItem<A> InsertMany<A>(ListItem<A> node, IEnumerable<A> items, long index)
    {
        var root     = node;
        var subIndex = index;
        foreach(var item in items)
        {
            root = Insert(root, item, subIndex);
            subIndex++;
        }
        return root;
    }

    public static ListItem<A> Insert<A>(ListItem<A> node, A value, long index)
    {
        if (node.IsEmpty)
        {
            return new ListItem<A>(1, 1, ListItem<A>.Empty, value, ListItem<A>.Empty);
        }
        else if (index == node.Left.Count)
        {
            var insertedLeft = Balance(Make(value, node.Left, ListItem<A>.Empty));
            var newThis      = Balance(Make(node.Value, insertedLeft, node.Right));
            return newThis;
        }
        else if (index < node.Left.Count)
        {
            return Balance(Make(node.Value, Insert(node.Left, value, index), node.Right));
        }
        else
        {
            return Balance(Make(node.Value, node.Left, Insert(node.Right, value, index - node.Left.Count - 1)));
        }
    }

    public static ListItem<A> Add<A>(ListItem<A> node, A value) =>
        node.IsEmpty
            ? new ListItem<A>(1, 1, ListItem<A>.Empty, value, ListItem<A>.Empty)
            : Balance(Make(node.Value, node.Left, Add(node.Right, value)));

    public static ListItem<A> SetItem<A>(ListItem<A> node, A value, long index)
    {
        if (node.IsEmpty)
        {
            throw new ArgumentException("Index outside the bounds of the list");
        }

        if (index == node.Left.Count)
        {
            return new ListItem<A>(node.Height, node.Count, node.Left, value, node.Right);
        }
        else if (index < node.Left.Count)
        {
            return new ListItem<A>(node.Height, node.Count, SetItem(node.Left, value, index), node.Value, node.Right);
        }
        else
        {
            return new ListItem<A>(node.Height, node.Count, node.Left, node.Value, SetItem(node.Right, value, index - node.Left.Count - 1));
        }
    }

    public static A GetItem<A>(ListItem<A> node, long index)
    {
        while (true)
        {
            if (node.IsEmpty)
            {
                throw new ArgumentException("Index outside the bounds of the list");
            }

            if (index == node.Left.Count)
            {
                return node.Value;
            }
            else if (index < node.Left.Count)
            {
                node = node.Left;
            }
            else
            {
                var node1 = node;
                node = node.Right;
                index = index - node1.Left.Count - 1;
            }
        }
    }

    public static ListItem<A> Remove<A>(ListItem<A> node, Func<A, bool> pred)
    {
        if (node.IsEmpty)
        {
            return node;
        }

        var result = node;

        var left  = node.Left.IsEmpty ? node.Left : Remove(node.Left, pred);
        var right = node.Right.IsEmpty ? node.Right : Remove(node.Right, pred);

        if (pred(node.Value))
        {
            switch (right.IsEmpty)
            {
                case true when left.IsEmpty:
                    result = ListItem<A>.Empty;
                    break;
                case true when !left.IsEmpty:
                    result = left;
                    break;
                case false when left.IsEmpty:
                    result = Balance(right);
                    break;
                default:
                {
                    var next = right;
                    while (!next.Left.IsEmpty)
                    {
                        next = next.Left;
                    }

                    right = Remove(right, 0);
                    result = Balance(Make(next.Value, left, right));
                    break;
                }
            }
        }
        else
        {
            if (!ReferenceEquals(left, node.Left) || !ReferenceEquals(right, node.Right))
            {
                result = Balance(Make(node.Value, left, right));
            }
        }

        return result.IsEmpty || result.IsBalanced ? result : Balance(result);
    }

    public static ListItem<A> Remove<A>(ListItem<A> node, A value, IEqualityComparer<A> compare)
    {
        if (node.IsEmpty)
        {
            return node;
        }

        var result = node;

        var left  = node.Left.IsEmpty ? node.Left : Remove(node.Left, value, compare);
        var right = node.Right.IsEmpty ? node.Right : Remove(node.Right, value, compare);

        if (ReferenceEquals(node.Value, value) || compare.Equals(node.Value, value))
        {
            switch (right.IsEmpty)
            {
                case true when left.IsEmpty:
                    result = ListItem<A>.Empty;
                    break;
                
                case true when !left.IsEmpty:
                    result = left;
                    break;
                
                case false when left.IsEmpty:
                    result = Balance(right);
                    break;
                
                default:
                {
                    var next = right;
                    while (!next.Left.IsEmpty)
                    {
                        next = next.Left;
                    }

                    right = Remove(right, 0);
                    result = Balance(Make(next.Value, left, right));
                    break;
                }
            }
        }
        else
        {
            if(!ReferenceEquals(left, node.Left) || !ReferenceEquals(right, node.Right))
            {
                result = Balance(Make(node.Value, left, right));
            }
        }

        return result.IsEmpty || result.IsBalanced ? result : Balance(result);
    }

    public static ListItem<A> Remove<A>(ListItem<A> node, long index)
    {
        if (node.IsEmpty)
        {
            return node;
        }

        ListItem<A> result;

        if (index == node.Left.Count)
        {
            switch (node.Right.IsEmpty)
            {
                case true when node.Left.IsEmpty:
                    result = ListItem<A>.Empty;
                    break;
                
                case true when !node.Left.IsEmpty:
                    result = node.Left;
                    break;
                
                case false when node.Left.IsEmpty:
                    result = Balance(node.Right);
                    break;
                
                default:
                {
                    var next = node.Right;
                    while (!next.Left.IsEmpty)
                    {
                        next = next.Left;
                    }

                    var right = Remove(node.Right, 0);
                    result = Balance(Make(next.Value, node.Left, right));
                    break;
                }
            }
        }
        else if (index < node.Left.Count)
        {
            var left = Remove(node.Left, index);
            result = Make(node.Value, left, node.Right);
        }
        else
        {
            var right = Remove(node.Right, index - node.Left.Count - 1);
            result = Make(node.Value, node.Left, right);
        }
        return result.IsEmpty || result.IsBalanced ? result : Balance(result);
    }

    public static long Find<A>(ListItem<A> node, A key, long index, long count, IComparer<A> comparer)
    {
        while (true)
        {
            if (node.IsEmpty || node.Count <= 0)
            {
                return ~index;
            }

            var nodeIndex = node.Left.Count;
            if (index + count <= nodeIndex)
            {
                node = node.Left;
                continue;
            }
            else if (index > nodeIndex)
            {
                var result = Find(node.Right, key, index - nodeIndex - 1, count, comparer);
                var offset = nodeIndex + 1;
                return result < 0 ? result - offset : result + offset;
            }

            var compare = comparer.Compare(key, node.Value);
            switch (compare)
            {
                case 0:
                    return nodeIndex;

                case > 0:
                {
                    var adjcount = count - (nodeIndex - index) - 1;
                    var result   = adjcount < 0 ? -1 : Find(node.Right, key, 0, adjcount, comparer);
                    var offset   = nodeIndex + 1;
                    return result < 0 ? result - offset : result + offset;
                }
                default:
                {
                    if (index == nodeIndex)
                    {
                        return ~index;
                    }

                    node = node.Left;
                    continue;
                }
            }
        }
    }

    public static ListItem<T> Skip<T>(ListItem<T> node, long amount)
    {
        if (amount == 0 || node.IsEmpty)
        {
            return node;
        }
        if (amount > node.Count)
        {
            return ListItem<T>.Empty;
        }
        switch (node.Left.IsEmpty)
        {
            case false when node.Left.Count == amount:
                return Balance(Make(node.Value, ListItem<T>.Empty, node.Right));
            
            case false when node.Left.Count == amount - 1:
                return node.Right;
            
            case true:
                return Skip(node.Right, amount - 1);
        }

        var newleft   = Skip(node.Left, amount);
        var remaining = amount - node.Left.Count - newleft.Count;
        return remaining > 0 
                   ? Skip(Balance(Make(node.Value, newleft, node.Right)), remaining) 
                   : Balance(Make(node.Value, newleft, node.Right));
    }

    static ListItem<A> Make<A>(A k, ListItem<A> l, ListItem<A> r) =>
        new ((byte)(1 + Math.Max(l.Height, r.Height)), l.Count + r.Count + 1, l, k, r);

    static ListItem<A> Balance<A>(ListItem<A> node) =>
        node.BalanceFactor >= 2
            ? node.Right.BalanceFactor < 0
                  ? DblRotLeft(node)
                  : RotLeft(node)
            : node.BalanceFactor <= -2
                ? node.Left.BalanceFactor > 0
                      ? DblRotRight(node)
                      : RotRight(node)
                : node;

    static ListItem<A> RotRight<A>(ListItem<A> node) =>
        node.IsEmpty || node.Left.IsEmpty
            ? node
            : Make(node.Left.Value, node.Left.Left, Make(node.Value, node.Left.Right, node.Right));

    static ListItem<A> RotLeft<A>(ListItem<A> node) =>
        node.IsEmpty || node.Right.IsEmpty
            ? node
            : Make(node.Right.Value, Make(node.Value, node.Left, node.Right.Left), node.Right.Right);

    static ListItem<A> DblRotRight<A>(ListItem<A> node) =>
        node.IsEmpty
            ? node
            : RotRight(Make(node.Value, RotLeft(node.Left), node.Right));

    static ListItem<A> DblRotLeft<A>(ListItem<A> node) =>
        node.IsEmpty
            ? node
            : RotLeft(Make(node.Value, node.Left, RotRight(node.Right)));
}
