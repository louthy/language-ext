using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LanguageExt;

public partial class Seq
{
    /// <summary>
    /// Readonly ref struct used to track the state of a fold operation.
    /// </summary>
    public readonly ref struct FoldState
    {
        readonly ref object Span;
        readonly int Index;
        readonly int Length;
        readonly IEnumerator? Enum; 

        public FoldState(ref object span, int index, int length, IEnumerator? @enum)
        {
            Span = span;
            Index = index;
            Length = length;
            Enum = @enum;
        }

        public static void FromSpan<A>(ref FoldState state, ReadOnlySpan<A> span) =>
            state = new FoldState(ref state.Span, -1, span.Length, null);

        public static void FromSpanBack<A>(ref FoldState state, ReadOnlySpan<A> span) =>
            state = new FoldState(ref state.Span, span.Length, span.Length, null);

        public static void FromEnumerator(ref FoldState state, IEnumerator @enum) =>
            state = new FoldState(ref Unsafe.NullRef<object>(), 0, -1, @enum);

        public static bool MoveNext<A>(ref FoldState state, out A value)
        {
            if (state.Enum is null)
            {
                var ix   = state.Index + 1;
                if (ix == state.Length)
                {
                    value = default!;
                    return false;
                }
                else
                {
                    var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<object, A>(ref state.Span), state.Length);
                    value = span[ix];
                    state = new FoldState(ref state.Span, ix, state.Length, null);
                    return true;
                }
            }
            else
            {
                if (state.Enum.MoveNext())
                {
                    value = (A)state.Enum.Current!;
                    return true;
                }
                else
                {
                    value = default!;
                    return false;
                }
            }
        }
        
        public static bool MovePrev<A>(ref FoldState state, out A value)
        {
            var ix = state.Index - 1;
            if (ix < 0)
            {
                value = default!;
                return false;
            }
            else
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<object, A>(ref state.Span), state.Length);
                value = span[ix];
                state = new FoldState(ref state.Span, ix, state.Length, null);
                return true;
            }
        }
    }
}
