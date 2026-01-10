#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class TrieSet
{
    /// <summary>
    /// Used to track the state of an iteration
    /// </summary>
    public class IteratorState<EqK, K>
        where EqK : Eq<K>
    {
        readonly int Top;
        
        /// <summary>
        /// 8 x 8 bits of index (64 bits total)
        /// We use 8 bits per index to allow for 128 children per node + 1 status bit.  The TrieSet only uses
        /// 32 children per node; this just gives us a bit of a buffer for future growth and possible overflow
        /// issues.
        /// </summary>
        readonly ulong EntryIndex;
        readonly Stck<TrieSet<EqK, K>.Node> NodeStack;

        const int EntryWidth = 8;                      // bit-width of an entry
        const ulong EntryMask = (1 << EntryWidth) - 1; // 1111 1111 
        const int IndexMask = (int)EntryMask >> 1;     // 0111 1111
        const int NodesMask = 1 << (EntryWidth - 1);   // 1000 0000
        const int StackDepth = 8;

        IteratorState() => 
            NodeStack = Stck<TrieSet<EqK, K>.Node>.Empty;

        IteratorState(int top, ulong entryIndex, Stck<TrieSet<EqK, K>.Node> nodeStack)
        {
            Top = top;
            EntryIndex = entryIndex;
            NodeStack = nodeStack;
        }

        IteratorState(TrieSet<EqK, K>.Node root)
        {
            Top = 1;
            EntryIndex = 0;
            NodeStack = Stck.singleton(root);
        }

        internal static IteratorState<EqK, K> Setup(TrieSet<EqK, K>.Node root) =>
            new (root);
        
        internal IteratorState<EqK, K> Push(TrieSet<EqK, K>.Node item)
        {
            var top   = Top;
            var index = EntryIndex;
            if (top == StackDepth) throw new StackOverflowException("TriMap.IteratorState stack-overflow");
            
            // Clear the index
            var mask = EntryMask << (top * EntryWidth);
            index &= ~mask;

            return new (top + 1, index, NodeStack.Push(item));
        }

        /// <summary>
        /// Increments the index and returns the previous value
        /// </summary>
        IteratorState<EqK, K> IncrIndex(out bool Nodes, out int Index)
        {
            var top   = (Top - 1) * EntryWidth;
            var index = EntryIndex;
            var mask  = EntryMask << top;
            var entry = (int)((index & mask) >> top);
            var val   = entry & IndexMask;
            var nodes = (entry & NodesMask) == NodesMask;
            var one   = 1ul << top;
            index += one;

            Nodes = nodes;
            Index = val;
            
            return new (Top, index, NodeStack);
        }

        /// <summary>
        /// Clears the index and sets the flag to process nodes instead of items
        /// </summary>
        IteratorState<EqK, K> ProcessNodes()
        {
            var top   = (Top - 1) * EntryWidth;
            var index = EntryIndex;
            var flag  = (ulong)NodesMask << top;
            var mask  = (ulong)IndexMask << top;
            index &= ~mask;
            index |= flag;
            
            return new (Top, index, NodeStack);
        }

        IteratorState<EqK, K> Pop() =>
            new (Top - 1, EntryIndex, NodeStack.Pop());

        bool Peek(out TrieSet<EqK, K>.Node item) =>
            NodeStack.TryPeek(out item);

        internal bool Step(out K node, out IteratorState<EqK, K> tail) =>
            Step(this, out node, out tail);
        
        static bool Step(IteratorState<EqK, K> state, out K node, out IteratorState<EqK, K> tail)
        {
            var top = state.Top;
            while (true)
            {
                if (top == 0)
                {
                    node = default!;
                    tail = state;
                    return false;
                }

                state.Peek(out var n);

                switch (n)
                {
                    case TrieSet<EqK, K>.Entries e:
                    {
                        state = state.IncrIndex(out var isNodes, out var index);
                        if (isNodes)
                        {
                            var nodes = e.Nodes;
                            if (index == nodes.Length)
                            {
                                state = state.Pop();
                                continue;
                            }
                            else
                            {
                                state = state.Push(e.Nodes[index]);
                                continue;
                            }
                        }
                        else
                        {
                            var items = e.Items;
                            if (index == items.Length)
                            {
                                state = state.ProcessNodes();
                                continue;
                            }
                            else
                            {
                                node = items[index];
                                tail = state;
                                return true;
                            }
                        }
                    }
   
                    case TrieSet<EqK, K>.EmptyNode:
                        state = state.Pop();
                        continue;

                    case TrieSet<EqK, K>.Collision c:
                    {
                        var items = c.Items;
                        state = state.IncrIndex(out _, out var index);
                        if (index == items.Length)
                        {
                            state = state.Pop();
                            continue;
                        }
                        else
                        {
                            node = items[index];
                            tail = state;
                            return true;
                        }
                    }
                }
            }
        }
    }
}
