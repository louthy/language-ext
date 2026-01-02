using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LanguageExt;

public partial class Lst
{
    /// <summary>
    /// Readonly ref struct used to track the state of a fold operation.
    /// </summary>
    public ref struct FoldState
    {
        #pragma warning disable CS0169 // Field is never used
        int Top;
        ulong FlagStack; 
        IListItem NodeStack0; 
        IListItem NodeStack1; 
        IListItem NodeStack2; 
        IListItem NodeStack3; 
        IListItem NodeStack4; 
        IListItem NodeStack5; 
        IListItem NodeStack6; 
        IListItem NodeStack7; 
        IListItem NodeStack8; 
        IListItem NodeStack9; 
        IListItem NodeStack10; 
        IListItem NodeStack11; 
        IListItem NodeStack12; 
        IListItem NodeStack13; 
        IListItem NodeStack14; 
        IListItem NodeStack15; 
        IListItem NodeStack16; 
        IListItem NodeStack17; 
        IListItem NodeStack18; 
        IListItem NodeStack19; 
        IListItem NodeStack20; 
        IListItem NodeStack21; 
        IListItem NodeStack22; 
        IListItem NodeStack23; 
        IListItem NodeStack24; 
        IListItem NodeStack25; 
        IListItem NodeStack26; 
        IListItem NodeStack27; 
        IListItem NodeStack28; 
        IListItem NodeStack29; 
        IListItem NodeStack30; 
        IListItem NodeStack31; 

        const ulong FlagMask = 3;
        const int StackDepth = 32;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Push<A>(ref FoldState state, ListItem<A> item)
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
        static void Peek<A>(ref FoldState state, out ListItem<A> item)
        {
            var top  = state.Top - 1;
            var span = MemoryMarshal.CreateSpan(ref state.NodeStack0, StackDepth);
            item = span[top] as ListItem<A> ?? throw new InvalidOperationException("Invalid list item type");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Init<A>(ref FoldState state, ListItem<A> root) => 
            Push(ref state, root);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool Step<A>(ref FoldState state, out ListItem<A> node)
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
        internal static bool StepBack<A>(ref FoldState state, out ListItem<A> node)
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
