using System;
using System.Runtime.CompilerServices;

namespace LanguageExt;

public partial class Map
{
    /// <summary>
    /// Struct used to track the state of a fold operation.
    /// </summary>
    public readonly struct IteratorState<K, V>
    {
        readonly int Top;
        readonly ulong FlagStack;
        readonly Stck<MapItem<K, V>> NodeStack;

        const int FlagWidth = 2; // 2 bits wide
        const ulong FlagMask = (1 << FlagWidth) - 1;
        
        public IteratorState()
        {
            Top = 0;
            FlagStack = 0;
            NodeStack = Stck<MapItem<K, V>>.Empty;
        }
        
        internal IteratorState(MapItem<K, V> root)
        {
            Top = 1;
            FlagStack = 0;
            NodeStack = Stck.singleton(root);
        }
        
        internal IteratorState(ulong flagStack, Stck<MapItem<K, V>> nodeStack, int top)
        {
            Top = top;
            FlagStack = flagStack;
            NodeStack = nodeStack;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static IteratorState<K, V> Setup(MapItem<K, V> root) => 
            new (root);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal IteratorState<K, V> Push(MapItem<K, V> item)
        {
            var top   = Top;
            var flags = FlagStack;

            // Add node
            var stack = NodeStack.Push(item);
            
            // Clear the flags
            var mask = FlagMask << (top * FlagWidth);
            flags &= ~mask;
            
            return new IteratorState<K, V>(flags, stack, top + 1);
        }

        /// <summary>
        /// Increments the flags stack and returns the previous value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IteratorState<K, V> IncrFlags(out int value)
        {
            var top   = (Top - 1) * FlagWidth;
            var flags = FlagStack;
            var mask  = FlagMask << top;
            var val   = (int)((flags & mask) >> top);
            var one   = 1ul << top;
            flags += one;
            value = val;
            return new IteratorState<K, V>(flags, NodeStack, Top);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IteratorState<K, V> Pop() =>
            new (FlagStack, NodeStack.Pop(), Top - 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        MapItem<K, V> Peek() =>
            NodeStack.PeekUnsafe();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool Step(out MapItem<K, V> value, out IteratorState<K, V> state)
        {
            state = this;
            while (true)
            {
                var top = state.Top;
                if (top == 0)
                {
                    value = null!;
                    return false;
                }

                var n = state.Peek();

                if (n.IsEmpty)
                {
                    state = state.Pop();
                    continue;
                }

                state = state.IncrFlags(out var f);
                switch (f)
                {
                    case 0:
                        state = state.Push(n.Left);
                        continue;

                    case 1:
                        value = n;
                        return true;

                    case 2:
                        state = state.Push(n.Right);
                        continue;

                    default:
                        state = state.Pop();
                        continue;
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool StepBack(out MapItem<K, V> value, out IteratorState<K, V> state)
        {
            state = this;
            while (true)
            {
                var top = state.Top;
                if (top == 0)
                {
                    value = null!;
                    return false;
                }

                var n = state.Peek();

                if (n.IsEmpty)
                {
                    state = state.Pop();
                    continue;
                }

                state = state.IncrFlags(out var f);
                switch (f)
                {
                    case 0:
                        state = state.Push(n.Right);
                        continue;

                    case 1:
                        value = n;
                        return true;

                    case 2:
                        state = state.Push(n.Left);
                        continue;

                    default:
                        state = state.Pop();
                        continue;
                }
            }
        }
    }
}
