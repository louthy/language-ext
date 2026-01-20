using System.Collections;
using System.Collections.Generic;

namespace LanguageExt;

public struct MapKeyEnumerator<K, V> : IEnumerator<K>
{
    struct NewStack : New<MapItem<K, V>[]>
    {
        public MapItem<K, V>[] New() =>
            new MapItem<K, V>[32];
    }

    int stackDepth;
    MapItem<K, V>[] stack;
    readonly MapItem<K, V> map;
    long left;
    readonly bool rev;
    readonly long start;

    internal MapKeyEnumerator(MapItem<K, V> root, bool rev, long start)
    {
        this.rev = rev;
        this.start = start;
        map = root;
        stack = Pool<NewStack, MapItem<K, V>[]>.Pop();
        stackDepth = 0;
        left = 0;
        NodeCurrent = null!;
        Reset();
    }

    MapItem<K, V> NodeCurrent
    {
        get;
        set;
    }

    public readonly K Current => NodeCurrent.KeyValue.Key;
    readonly object IEnumerator.Current => NodeCurrent.KeyValue.Key!;

    public void Dispose()
    {
        if (stack is not null)
        {
            Pool<NewStack, MapItem<K, V>[]>.Push(stack);
            stack = null!;
        }
    }

    MapItem<K, V> Next(MapItem<K, V> node) =>
        rev ? node.Left : node.Right;

    MapItem<K, V> Prev(MapItem<K, V> node) =>
        rev ? node.Right : node.Left;

    void Push(MapItem<K, V> node)
    {
        while (!node.IsEmpty)
        {
            stack[stackDepth] = node;
            stackDepth++;
            node = Prev(node);
        }
    }

    public bool MoveNext()
    {
        if (left > 0 && stackDepth > 0)
        {
            stackDepth--;
            NodeCurrent = stack[stackDepth];
            Push(Next(NodeCurrent));
            left--;
            return true;
        }

        NodeCurrent = null!;
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
