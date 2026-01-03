#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using LanguageExt.Traits;

namespace LanguageExt;

public partial class TrieMap
{
    /// <summary>
    /// ref struct used to track the state of a fold operation.
    /// </summary>
    public ref struct FoldState
    {
        #pragma warning disable CS0169 // Field is never used
        int Top;
        
        /// <summary>
        /// 8 x 8 bits of index (64 bits total)
        /// We use 8 bits per index to allow for 128 children per node + 1 status bit.  The TrieMap only uses
        /// 32 children per node; this just gives us a bit of a buffer for future growth and possible overflow
        /// issues.
        /// </summary>
        ulong EntryIndex;       
        ITrieNode NodeStack0; 
        ITrieNode NodeStack1; 
        ITrieNode NodeStack2; 
        ITrieNode NodeStack3; 
        ITrieNode NodeStack4; 
        ITrieNode NodeStack5; 
        ITrieNode NodeStack6; 
        ITrieNode NodeStack7; 

        const int EntryWidth = 8;                      // bit-width of an entry
        const ulong EntryMask = (1 << EntryWidth) - 1; // 1111 1111 
        const int IndexMask = (int)EntryMask >> 1;     // 0111 1111
        const int NodesMask = 1 << (EntryWidth - 1);   // 1000 0000
        private const int StackDepth = 8;                          

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Setup<EqK, K, V>(ref FoldState state, TrieMap<EqK, K, V>.Node root) 
            where EqK : Eq<K> =>
            Push(ref state, root);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Push<EqK, K, V>(ref FoldState state, TrieMap<EqK, K, V>.Node item)
            where EqK : Eq<K>
        {
            ref var top   = ref state.Top;
            ref var index = ref state.EntryIndex;
            if (top == StackDepth) throw new StackOverflowException("TriMap.FoldState stack-overflow");
            
            // Add node
            var span  = MemoryMarshal.CreateSpan(ref state.NodeStack0, StackDepth);
            span[top] = item;
            
            // Clear the index
            var mask = EntryMask << (top * EntryWidth);
            index &= ~mask;
            top++;
        }

        /// <summary>
        /// Increments the index and returns the previous value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static (bool Nodes, int Index) IncrIndex(ref FoldState state)
        {
            var     top   = (state.Top - 1) * EntryWidth;
            ref var index = ref state.EntryIndex;
            var     mask  = EntryMask << top;
            var     entry = (int)((index & mask) >> top);
            var     val   = entry & IndexMask;
            var     nodes = (entry & NodesMask) == NodesMask;
            var     one   = 1ul << top;
            index += one;
            return (nodes, val);
        }

        /// <summary>
        /// Clears the index and sets the flag to process nodes instead of items
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void ProcessNodes(ref FoldState state)
        {
            var     top   = (state.Top - 1) * EntryWidth;
            ref var index = ref state.EntryIndex;
            var     flag  = (ulong)NodesMask << top;
            var     mask  = (ulong)IndexMask << top;
            index &= ~mask;
            index |= flag;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Pop(ref FoldState state)
        {
            ref var top = ref state.Top;
            top--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Peek<EqK, K, V>(ref FoldState state, out TrieMap<EqK, K, V>.Node item)
            where EqK : Eq<K>
        {
            var top  = state.Top - 1;
            var span = MemoryMarshal.CreateSpan(ref state.NodeStack0, StackDepth);
            item = span[top] as TrieMap<EqK, K, V>.Node ?? throw new InvalidOperationException("Invalid trie-map item type");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool Step<EqK, K, V>(ref FoldState state, out (K Key, V Value) node)
            where EqK : Eq<K>
        {
            ref var top = ref state.Top;
            while (true)
            {
                if (top == 0)
                {
                    node = default!;
                    return false;
                }

                Peek<EqK, K, V>(ref state, out var n);

                switch (n)
                {
                    case TrieMap<EqK, K, V>.Entries e:
                    {
                        var (isNodes, index) = IncrIndex(ref state);
                        if (isNodes)
                        {
                            var nodes = e.Nodes;
                            if (index == nodes.Length)
                            {
                                node = default!;
                                return false;
                            }
                            else
                            {
                                Push(ref state, e.Nodes[index]);
                                continue;
                            }
                        }
                        else
                        {
                            var items = e.Items;
                            if (index == items.Length)
                            {
                                ProcessNodes(ref state);
                                continue;
                            }
                            else
                            {
                                node = items[index];
                                return true;
                            }
                        }
                    }
   
                    case TrieMap<EqK, K, V>.EmptyNode:
                        top--;
                        continue;

                    case TrieMap<EqK, K, V>.Collision c:
                    {
                        var items = c.Items;
                        var (_, index) = IncrIndex(ref state);
                        if (index == items.Length)
                        {
                            node = default!;
                            return true;
                        }
                        else
                        {
                            node = items[index];
                            return true;
                        }
                    }
                }
            }
        }
    }
}
