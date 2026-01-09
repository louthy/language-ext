using System.Runtime.CompilerServices;

namespace LanguageExt;

public partial class Set
{
    /// <summary>
    /// Struct used to track the state of a fold operation.
    /// </summary>
    public readonly struct IteratorState<A>
    {
        readonly int Top;
        readonly ulong FlagStack;
        readonly Stck<SetItem<A>> NodeStack;

        const int FlagWidth = 2; // 2 bits wide
        const ulong FlagMask = (1 << FlagWidth) - 1;
        
        public IteratorState()
        {
            Top = 0;
            FlagStack = 0;
            NodeStack = Stck<SetItem<A>>.Empty;
        }
        
        internal IteratorState(SetItem<A> root)
        {
            Top = 1;
            FlagStack = 0;
            NodeStack = Stck.singleton(root);
        }
        
        internal IteratorState(ulong flagStack, Stck<SetItem<A>> nodeStack, int top)
        {
            Top = top;
            FlagStack = flagStack;
            NodeStack = nodeStack;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static IteratorState<A> Setup(SetItem<A> root) => 
            new (root);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal IteratorState<A> Push(SetItem<A> item)
        {
            var top   = Top;
            var flags = FlagStack;

            // Add node
            var stack = NodeStack.Push(item);
            
            // Clear the flags
            var mask = FlagMask << (top * FlagWidth);
            flags &= ~mask;
            
            return new IteratorState<A>(flags, stack, top + 1);
        }

        /// <summary>
        /// Increments the flags stack and returns the previous value
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IteratorState<A> IncrFlags(out int value)
        {
            var top   = (Top - 1) * FlagWidth;
            var flags = FlagStack;
            var mask  = FlagMask << top;
            var val   = (int)((flags & mask) >> top);
            var one   = 1ul << top;
            flags += one;
            value = val;
            return new IteratorState<A>(flags, NodeStack, Top);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        IteratorState<A> Pop() =>
            new (FlagStack, NodeStack.Pop(), Top - 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        SetItem<A> Peek() =>
            NodeStack.PeekUnsafe();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool Step(out SetItem<A> head, out IteratorState<A> tail)
        {
            var top = Top;
            tail = this;
            while (true)
            {
                if (top == 0)
                {
                    head = null!;
                    return false;
                }

                var n = tail.Peek();

                if (n.IsEmpty)
                {
                    top--;
                    continue;
                }

                tail = IncrFlags(out var f);
                switch (f)
                {
                    case 0:
                        tail = tail.Push(n.Left);
                        continue;

                    case 1:
                        head = n;
                        return true;

                    case 2:
                        tail = tail.Push(n.Right);
                        continue;

                    default:
                        tail = tail.Pop();
                        continue;
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool StepBack(out SetItem<A> head, out IteratorState<A> tail)
        {
            var top = Top;
            tail = this;
            while (true)
            {
                if (top == 0)
                {
                    head = null!;
                    return false;
                }

                var n = tail.Peek();

                if (n.IsEmpty)
                {
                    top--;
                    continue;
                }

                tail = IncrFlags(out var f);
                switch (f)
                {
                    case 0:
                        tail = tail.Push(n.Right);
                        continue;

                    case 1:
                        head = n;
                        return true;

                    case 2:
                        tail = tail.Push(n.Left);
                        continue;

                    default:
                        tail = tail.Pop();
                        continue;
                }
            }
        }
    }
}
