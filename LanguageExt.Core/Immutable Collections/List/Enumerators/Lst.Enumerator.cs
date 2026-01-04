using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace LanguageExt;

public struct ListEnumerator<T> : IEnumerator<T>
{
    internal struct NewStack : New<ListItem<T>[]>
    {
        public ListItem<T>[] New() =>
            new ListItem<T>[32];
    }

    ListItem<T>[] stack;
    int stackDepth;
    readonly ListItem<T> map;
    int left;
    readonly int start;
    int count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ListEnumerator(ListItem<T> root, int start, int count = int.MaxValue)
    {
        this.start = start;
        map = root;
        stack = Pool<NewStack, ListItem<T>[]>.Pop();
        this.count = count;
        stackDepth = default;
        left = default;
        NodeCurrent = default!;
        Reset();
    }

    private ListItem<T> NodeCurrent
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set;
    }

    public readonly T Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => NodeCurrent.Key;
    }

    object IEnumerator.Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => NodeCurrent.Key!;
    }

    public void Dispose()
    {
        if (stack != null)
        {
            Pool<NewStack, ListItem<T>[]>.Push(stack);
            stack = null!;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ListItem<T> Next(ListItem<T> node) =>
        node.Right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ListItem<T> Prev(ListItem<T> node) =>
        node.Left;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Push(ListItem<T> node)
    {
        while (!node.IsEmpty)
        {
            stack[stackDepth] = node;
            stackDepth++;
            node = Prev(node);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        if (count > 0 && left > 0 && stackDepth > 0)
        {
            stackDepth--;
            NodeCurrent = stack[stackDepth];
            Push(Next(NodeCurrent));
            left--;
            count--;
            return true;
        }

        NodeCurrent = default!;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Reset()
    {
        var skip = start;

        stackDepth = 0;
        NodeCurrent = map;
        left = map.Count;

        while (!NodeCurrent.IsEmpty && skip != Prev(NodeCurrent).Count)
        {
            if (skip < Prev(NodeCurrent).Count)
            {
                stack[stackDepth] = NodeCurrent;
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
            stack[stackDepth] = NodeCurrent;
            stackDepth++;
        }
    }
}
