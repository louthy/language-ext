#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System;
using LanguageExt.Traits;

namespace LanguageExt;

static class MapModuleM
{
    public enum AddOpt
    {
        ThrowOnDuplicate,
        TryAdd,
        TryUpdate
    }

    public static MapItem<K, V> Add<OrdK, K, V>(MapItem<K, V> node, K key, V value, AddOpt option)
        where OrdK : Ord<K>
    {
        if (node.IsEmpty)
        {
            return new MapItem<K, V>(1, 1, (key, value), MapItem<K, V>.Empty, MapItem<K, V>.Empty);
        }
        var cmp = OrdK.Compare(key, node.KeyValue.Key);
        if (cmp < 0)
        {
            node.Left = Add<OrdK, K, V>(node.Left, key, value, option);
            return Balance(node);
        }
        else if (cmp > 0)
        {
            node.Right = Add<OrdK, K, V>(node.Right, key, value, option);
            return Balance(node);
        }
        else if(option == AddOpt.TryAdd)
        {
            // Already exists, but we don't care
            return node;
        }
        else if (option == AddOpt.TryUpdate)
        {
            // Already exists, and we want to update the content
            node.KeyValue = (key, value);
            return node;
        }
        else
        {
            throw new ArgumentException("An element with the same key already exists in the Map");
        }
    }

    public static MapItem<K, V> Balance<K, V>(MapItem<K, V> node)
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

    public static MapItem<K, V> DblRotRight<K, V>(MapItem<K, V> node)
    {
        node.Left = RotLeft(node.Left);
        return RotRight(node);
    }

    public static MapItem<K, V> DblRotLeft<K, V>(MapItem<K, V> node)
    {
        node.Right = RotRight(node.Right);
        return RotLeft(node);
    }

    public static MapItem<K, V> RotRight<K, V>(MapItem<K, V> node)
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

    public static MapItem<K, V> RotLeft<K, V>(MapItem<K, V> node)
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

