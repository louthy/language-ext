using System;
using LanguageExt.Traits;

namespace LanguageExt;

internal static class SetModuleM
{
    public enum AddOpt
    {
        ThrowOnDuplicate,
        TryAdd,
        TryUpdate
    }

    public static SetItem<K> Add<OrdK, K>(SetItem<K> node, K key, AddOpt option)
        where OrdK : Ord<K>
    {
        if (node.IsEmpty)
        {
            return new SetItem<K>(1, 1, key, SetItem<K>.Empty, SetItem<K>.Empty);
        }
        var cmp = OrdK.Compare(key, node.Key);
        if (cmp < 0)
        {
            node.Left = Add<OrdK, K>(node.Left, key, option);
            return Balance(node);
        }
        else if (cmp > 0)
        {
            node.Right = Add<OrdK, K>(node.Right, key, option);
            return Balance(node);
        }
        else if (option == AddOpt.TryAdd)
        {
            // Already exists, but we don't care
            return node;
        }
        else if (option == AddOpt.TryUpdate)
        {
            // Already exists, and we want to update the content
            node.Key = key;
            return node;
        }
        else
        {
            throw new ArgumentException("An element with the same key already exists in the Map");
        }
    }

    public static SetItem<K> Balance<K>(SetItem<K> node)
    {
        node.Height = (byte)(1 + Math.Max(node.Left.Height, node.Right.Height));
        node.Count = 1 + node.Left.Count + node.Right.Count;

        return node.BalanceFactor >= 2
                   ? node.Right.BalanceFactor < 0
                         ? DblRotLeft(node)
                         : RotLeft(node)
                   : node.BalanceFactor <= -2
                       ? node.Left.BalanceFactor > 0
                             ? DblRotRight(node)
                             : RotRight(node)
                       : node;
    }

    public static SetItem<K> DblRotRight<K>(SetItem<K> node)
    {
        node.Left = RotLeft(node.Left);
        return RotRight(node);
    }

    public static SetItem<K> DblRotLeft<K>(SetItem<K> node)
    {
        node.Right = RotRight(node.Right);
        return RotLeft(node);
    }

    public static SetItem<K> RotRight<K>(SetItem<K> node)
    {
        if (node.IsEmpty || node.Left.IsEmpty) return node;

        var y  = node;
        var x  = y.Left;
        var t2 = x.Right;
        x.Right = y;
        y.Left = t2;
        y.Height = (byte)(1 + Math.Max(y.Left.Height, y.Right.Height));
        x.Height = (byte)(1 + Math.Max(x.Left.Height, x.Right.Height));
        y.Count = 1 + y.Left.Count + y.Right.Count;
        x.Count = 1 + x.Left.Count + x.Right.Count;

        return x;
    }

    public static SetItem<K> RotLeft<K>(SetItem<K> node)
    {
        if (node.IsEmpty || node.Right.IsEmpty) return node;

        var x  = node;
        var y  = x.Right;
        var t2 = y.Left;
        y.Left = x;
        x.Right = t2;
        x.Height = (byte)(1 + Math.Max(x.Left.Height, x.Right.Height));
        y.Height = (byte)(1 + Math.Max(y.Left.Height, y.Right.Height));
        x.Count = 1 + x.Left.Count + x.Right.Count;
        y.Count = 1 + y.Left.Count + y.Right.Count;

        return y;
    }
}
