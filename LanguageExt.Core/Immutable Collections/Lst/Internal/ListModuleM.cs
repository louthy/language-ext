using System;
using LanguageExt.Traits;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LanguageExt;

static class ListModuleM
{
    public static ListItem<A> InsertMany<A>(ListItem<A> node, IEnumerable<A> items, long index) =>
        Insert(node, BuildSubTree(items), index);
    
    public static ListItem<A> InsertMany<A>(ListItem<A> node, Iterator<A> items, long index) =>
        Insert(node, BuildSubTree(items), index);

    public static ListItem<A> InsertMany<A>(ListItem<A> node, ReadOnlySpan<A> items, long index) =>
        Insert(node, BuildSubTree(items), index);

    public static ListItem<A> BuildSubTree<T, FS, A>(K<T, A> items)
        where T : Foldable<T, FS>
        where FS : allows ref struct
    {
        var root      = ListItem<A>.EmptyM;
        var subIndex  = 0L;
        var foldState = T.StepSetup(items);
        while (T.Step(items, ref foldState, out var item))
        {
            root = Insert(root, new ListItem<A>(1, 1, ListItem<A>.Empty, item, ListItem<A>.Empty), subIndex);
            subIndex++;
        }
        return root;
    }

    public static ListItem<A> BuildSubTree<A>(Iterator<A> items)
    {
        var root      = ListItem<A>.EmptyM;
        var subIndex  = 0L;
        foreach(var item in items)
        {
            root = Insert(root, new ListItem<A>(1, 1, ListItem<A>.Empty, item, ListItem<A>.Empty), subIndex);
            subIndex++;
        }
        return root;
    }

    public static ListItem<A> BuildSubTreeBack<T, FS, A>(K<T, A> items)
        where T : FoldableBack<T, FS>
        where FS : allows ref struct
    {
        var root      = ListItem<A>.EmptyM;
        var subIndex  = 0L;
        var foldState = T.StepBackSetup(items);
        while (T.StepBack(items, ref foldState, out var item))
        {
            root = Insert(root, new ListItem<A>(1, 1, ListItem<A>.Empty, item, ListItem<A>.Empty), subIndex);
            subIndex++;
        }
        return root;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ListItem<A> BuildSubTree<A>(IEnumerable<A> items)
    {
        var root = ListItem<A>.EmptyM;

        var subIndex = 0L;
        foreach (var item in items)
        {
            root = Insert(root, new ListItem<A>(1, 1, ListItem<A>.Empty, item, ListItem<A>.Empty), subIndex);
            subIndex++;
        }

        return root;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ListItem<A> BuildSubTree<A>(ReadOnlySpan<A> items)
    {
        var root = ListItem<A>.EmptyM;

        var subIndex = 0L;
        foreach (var item in items)
        {
            root = Insert(root, new ListItem<A>(1, 1, ListItem<A>.Empty, item, ListItem<A>.Empty), subIndex);
            subIndex++;
        }

        return root;
    }

    public static ListItem<A> Insert<A>(ListItem<A> node, ListItem<A> insertNode, long index)
    {
        if (node.IsEmpty)
        {
            return insertNode;
        }
        else if (index == node.Left.Count)
        {
            insertNode.Left = node.Left;
            insertNode = Balance(insertNode);

            node.Left = insertNode;
            node = Balance(node);

            return node;
        }
        else if (index < node.Left.Count)
        {
            node.Left = Insert(node.Left, insertNode, index);
            return Balance(node);
        }
        else
        {
            node.Right = Insert(node.Right, insertNode, index - node.Left.Count - 1);
            return Balance(node);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ListItem<T> Balance<T>(ListItem<T> node)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ListItem<T> DblRotRight<T>(ListItem<T> node)
    {
        node.Left = RotLeft(node.Left);
        return RotRight(node);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ListItem<T> DblRotLeft<T>(ListItem<T> node)
    {
        node.Right = RotRight(node.Right);
        return RotLeft(node);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ListItem<A> RotRight<A>(ListItem<A> node)
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ListItem<A> RotLeft<A>(ListItem<A> node)
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
