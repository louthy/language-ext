using System;
using System.Collections;
using System.Collections.Generic;

namespace LanguageExt
{
    class MapValueEnumerator<K, V> : IEnumerator<V>
    {
        static ObjectPool<Stack<MapItem<K, V>>> pool = new ObjectPool<Stack<MapItem<K, V>>>(32, () => new Stack<MapItem<K, V>>(32));

        Stack<MapItem<K, V>> stack;
        MapItem<K, V> map;
        int left;
        bool rev;
        int start;

        public MapValueEnumerator(MapItem<K, V> root, bool rev, int start)
        {
            this.rev = rev;
            this.start = start;
            map = root;
            stack = pool.GetItem();
            Reset();
        }

        private MapItem<K, V> NodeCurrent
        {
            get;
            set;
        }

        public V Current => NodeCurrent.KeyValue.Value;
        object IEnumerator.Current => NodeCurrent.KeyValue.Value;

        public void Dispose()
        {
            if (stack != null)
            {
                pool.Release(stack);
                stack = null;
            }
        }

        private MapItem<K, V> Next(MapItem<K, V> node) =>
            rev ? node.Left : node.Right;

        private MapItem<K, V> Prev(MapItem<K, V> node) =>
            rev ? node.Right : node.Left;

        private void Push(MapItem<K, V> node)
        {
            while (!node.IsEmpty)
            {
                stack.Push(node);
                node = Prev(node);
            }
        }

        public bool MoveNext()
        {
            if (left > 0 && stack.Count > 0)
            {
                NodeCurrent = stack.Pop();
                Push(Next(NodeCurrent));
                left--;
                return true;
            }

            NodeCurrent = null;
            return false;
        }

        public void Reset()
        {
            var skip = rev ? map.Count - start - 1 : start;

            stack.Clear();
            NodeCurrent = map;
            left = map.Count;

            while (!NodeCurrent.IsEmpty && skip != Prev(NodeCurrent).Count)
            {
                if (skip < Prev(NodeCurrent).Count)
                {
                    stack.Push(NodeCurrent);
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
                stack.Push(NodeCurrent);
            }
        }
    }
}
