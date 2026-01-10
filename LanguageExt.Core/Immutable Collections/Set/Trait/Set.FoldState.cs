using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LanguageExt;

public partial class Set
{
    /// <summary>
    /// ref struct used to track the state of a fold operation.
    /// </summary>
    public ref struct FoldState
    {
        #pragma warning disable CS0169 // Field is never used
        int Top;
        ulong FlagStack; 
        ISetItem NodeStack0; 
        ISetItem NodeStack1; 
        ISetItem NodeStack2; 
        ISetItem NodeStack3; 
        ISetItem NodeStack4; 
        ISetItem NodeStack5; 
        ISetItem NodeStack6; 
        ISetItem NodeStack7; 
        ISetItem NodeStack8; 
        ISetItem NodeStack9; 
        ISetItem NodeStack10; 
        ISetItem NodeStack11; 
        ISetItem NodeStack12; 
        ISetItem NodeStack13; 
        ISetItem NodeStack14; 
        ISetItem NodeStack15; 
        ISetItem NodeStack16; 
        ISetItem NodeStack17; 
        ISetItem NodeStack18; 
        ISetItem NodeStack19; 
        ISetItem NodeStack20; 
        ISetItem NodeStack21; 
        ISetItem NodeStack22; 
        ISetItem NodeStack23; 
        ISetItem NodeStack24; 
        ISetItem NodeStack25; 
        ISetItem NodeStack26; 
        ISetItem NodeStack27; 
        ISetItem NodeStack28; 
        ISetItem NodeStack29; 
        ISetItem NodeStack30; 
        ISetItem NodeStack31; 

        const int FlagWidth = 2; // 2 bits wide
        const ulong FlagMask = (1 << FlagWidth) - 1;
        const int StackDepth = 32;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static FoldState Setup<A>(SetItem<A> root)
        {
            FoldState state = default;
            Push(ref state, root);
            return state;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Push<A>(ref FoldState state, SetItem<A> item)
        {
            ref var top   = ref state.Top;
            ref var flags = ref state.FlagStack;
            if (top == StackDepth) throw new StackOverflowException("Map.FoldState stack-overflow");

            // Add node
            var span  = MemoryMarshal.CreateSpan(ref state.NodeStack0, StackDepth);
            span[top] = item;
            
            // Clear the flags
            var mask = FlagMask << (top * FlagWidth);
            flags &= ~mask;
            top++;
        }

        /// <summary>
        /// Increments the flags stack and returns the previous value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int IncrFlags(ref FoldState state)
        {
            var     top   = (state.Top - 1) * FlagWidth;
            ref var flags = ref state.FlagStack;
            var     mask  = FlagMask << top;
            var     val   = (int)((flags & mask) >> top);
            var     one   = 1ul << top;
            flags += one;
            return val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Pop(ref FoldState state)
        {
            ref var top = ref state.Top;
            top--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static void Peek<A>(ref FoldState state, out SetItem<A> item)
        {
            var top  = state.Top - 1;
            var span = MemoryMarshal.CreateSpan(ref state.NodeStack0, StackDepth);
            item = span[top] as SetItem<A> ?? throw new InvalidOperationException("Invalid map item type");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool Step<A>(ref FoldState state, out SetItem<A> node)
        {
            ref var top = ref state.Top;
            while (true)
            {
                if (top == 0)
                {
                    node = null!;
                    return false;
                }

                Peek<A>(ref state, out var n);

                if (n.IsEmpty)
                {
                    top--;
                    continue;
                }

                var f = IncrFlags(ref state);
                switch (f)
                {
                    case 0:
                        Push(ref state, n.Left);
                        continue;

                    case 1:
                        node = n;
                        return true;

                    case 2:
                        Push(ref state, n.Right);
                        continue;

                    default:
                        Pop(ref state);
                        continue;
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool StepBack<A>(ref FoldState state, out SetItem<A> node)
        {
            ref var top = ref state.Top;
            while (true)
            {
                if (top == 0)
                {
                    node = null!;
                    return false;
                }

                Peek<A>(ref state, out var n);

                if (n.IsEmpty)
                {
                    top--;
                    continue;
                }

                var f = IncrFlags(ref state);
                switch (f)
                {
                    case 0:
                        Push(ref state, n.Right);
                        continue;

                    case 1:
                        node = n;
                        return true;

                    case 2:
                        Push(ref state, n.Left);
                        continue;

                    default:
                        Pop(ref state);
                        continue;
                }
            }
        }
    }
}
