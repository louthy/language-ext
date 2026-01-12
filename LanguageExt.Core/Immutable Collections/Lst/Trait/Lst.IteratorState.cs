namespace LanguageExt;

public partial class Lst
{
    /// <summary>
    /// Struct used to track the state of a fold operation.
    /// </summary>
    public readonly struct IteratorState<A>
    {
        readonly int Top;
        readonly ulong FlagStack;
        readonly Stck<ListItem<A>> NodeStack;

        const int FlagWidth = 2; // 2 bits wide
        const ulong FlagMask = (1 << FlagWidth) - 1;
        
        public IteratorState()
        {
            Top = 0;
            FlagStack = 0;
            NodeStack = Stck<ListItem<A>>.Empty;
        }
        
        internal IteratorState(ListItem<A> root)
        {
            Top = 1;
            FlagStack = 0;
            NodeStack = Stck.singleton(root);
        }
        
        internal IteratorState(ulong flagStack, Stck<ListItem<A>> nodeStack, int top)
        {
            Top = top;
            FlagStack = flagStack;
            NodeStack = nodeStack;
        }
        
        internal static IteratorState<A> Setup(ListItem<A> root) => 
            new (root);
        
        internal IteratorState<A> Push(ListItem<A> item)
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

        IteratorState<A> Pop() =>
            new (FlagStack, NodeStack.Pop(), Top - 1);

        ListItem<A> Peek() =>
            NodeStack.PeekUnsafe();

        internal bool Step(out ListItem<A> head, out IteratorState<A> tail)
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
                        top++;
                        continue;

                    case 1:
                        head = n;
                        return true;

                    case 2:
                        tail = tail.Push(n.Right);
                        top++;
                        continue;

                    default:
                        tail = tail.Pop();
                        top--;
                        continue;
                }
            }
        }
        
        internal bool StepBack(out ListItem<A> head, out IteratorState<A> tail)
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
                        top++;
                        continue;

                    case 1:
                        head = n;
                        return true;

                    case 2:
                        tail = tail.Push(n.Left);
                        top++;
                        continue;

                    default:
                        tail = tail.Pop();
                        top--;
                        continue;
                }
            }
        }
    }
}
