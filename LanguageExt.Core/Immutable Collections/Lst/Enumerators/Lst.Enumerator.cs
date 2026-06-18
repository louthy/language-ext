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
    long top;
    readonly ListItem<T> map;
    long remaining;
    readonly long start;
    long count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ListEnumerator(ListItem<T> root, long start, long count = long.MaxValue)
    {
        this.start = start;
        map = root;
        stack = Pool<NewStack, ListItem<T>[]>.Pop();
        this.count = count;
        top = 0;
        remaining = 0;
        NodeCurrent = null!;
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
        get => NodeCurrent.Value;
    }

    object IEnumerator.Current
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => NodeCurrent.Value!;
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
    ListItem<T> Next(ListItem<T> node) =>
        node.Right;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    ListItem<T> Prev(ListItem<T> node) =>
        node.Left;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    void Push(ListItem<T> node)
    {
        while (!node.IsEmpty)
        {
            stack[top] = node;
            top++;
            node = Prev(node);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool MoveNext()
    {
        if (count > 0 && remaining > 0 && top > 0)
        {
            top--;
            NodeCurrent = stack[top];
            Push(Next(NodeCurrent));
            remaining--;
            count--;
            return true;
        }

        NodeCurrent = null!;
        return false;
    }

    public void Reset()
    {
        var skip = start;

        top = 0;
        NodeCurrent = map;
        remaining = map.Count;

        while (!NodeCurrent.IsEmpty && skip != Prev(NodeCurrent).Count)
        {
            if (skip < Prev(NodeCurrent).Count)
            {
                stack[top] = NodeCurrent;
                top++;
                NodeCurrent = Prev(NodeCurrent);
            }
            else
            {
                skip -= Prev(NodeCurrent).Count + 1;
                NodeCurrent = Next(NodeCurrent);
            }
        }

        if (NodeCurrent.IsEmpty) return;
        stack[top] = NodeCurrent;
        top++;
    }
}
