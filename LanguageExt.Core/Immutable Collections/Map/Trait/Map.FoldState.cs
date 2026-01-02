using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LanguageExt;

public partial class Map
{
    /// <summary>
    /// Readonly ref struct used to track the state of a fold operation.
    /// </summary>
    public ref struct FoldState
    {
        #pragma warning disable CS0169 // Field is never used
        int Top;
        ulong FlagStack; 
        IMapItem NodeStack0; 
        IMapItem NodeStack1; 
        IMapItem NodeStack2; 
        IMapItem NodeStack3; 
        IMapItem NodeStack4; 
        IMapItem NodeStack5; 
        IMapItem NodeStack6; 
        IMapItem NodeStack7; 
        IMapItem NodeStack8; 
        IMapItem NodeStack9; 
        IMapItem NodeStack10; 
        IMapItem NodeStack11; 
        IMapItem NodeStack12; 
        IMapItem NodeStack13; 
        IMapItem NodeStack14; 
        IMapItem NodeStack15; 
        IMapItem NodeStack16; 
        IMapItem NodeStack17; 
        IMapItem NodeStack18; 
        IMapItem NodeStack19; 
        IMapItem NodeStack20; 
        IMapItem NodeStack21; 
        IMapItem NodeStack22; 
        IMapItem NodeStack23; 
        IMapItem NodeStack24; 
        IMapItem NodeStack25; 
        IMapItem NodeStack26; 
        IMapItem NodeStack27; 
        IMapItem NodeStack28; 
        IMapItem NodeStack29; 
        IMapItem NodeStack30; 
        IMapItem NodeStack31; 

        const ulong FlagMask = 3;
        const int StackDepth = 32;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Setup<K, V>(ref FoldState state, MapItem<K, V> root) => 
            Push(ref state, root);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Push<K, V>(ref FoldState state, MapItem<K, V> item)
        {
            ref var top   = ref state.Top;
            ref var flags = ref state.FlagStack;
            
            // Add node
            var span  = MemoryMarshal.CreateSpan(ref state.NodeStack0, StackDepth);
            span[top] = item;
            
            // Clear the flags
            var mask = FlagMask << (top << 2);
            flags &= ~mask;
            top++;
        }

        /// <summary>
        /// Increments the flags stack and returns the previous value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int IncrFlags(ref FoldState state)
        {
            var     top   = (state.Top - 1) << 2;
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
        static void Peek<K, V>(ref FoldState state, out MapItem<K, V> item)
        {
            var top  = state.Top - 1;
            var span = MemoryMarshal.CreateSpan(ref state.NodeStack0, StackDepth);
            item = span[top] as MapItem<K, V> ?? throw new InvalidOperationException("Invalid map item type");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool Step<K, V>(ref FoldState state, out MapItem<K, V> node)
        {
            ref var top = ref state.Top;
            while (true)
            {
                if (top == 0)
                {
                    node = null!;
                    return false;
                }

                Peek<K, V>(ref state, out var n);

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
        internal static bool StepBack<K, V>(ref FoldState state, out MapItem<K, V> node)
        {
            ref var top = ref state.Top;
            while (true)
            {
                if (top == 0)
                {
                    node = null!;
                    return false;
                }

                Peek<K, V>(ref state, out var n);

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
