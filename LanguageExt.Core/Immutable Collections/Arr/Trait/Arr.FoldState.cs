using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LanguageExt;

public partial class Arr
{
    /// <summary>
    /// Readonly ref struct used to track the state of a fold operation.
    /// </summary>
    public readonly ref struct FoldState
    {
        readonly ref object Span;
        readonly int Index;
        readonly int Length;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FoldState(ref object span, int index, int length)
        {
            Span = ref span;
            Index = index;
            Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Setup<A>(ref FoldState state, ReadOnlySpan<A> span) =>
            state = new FoldState(ref Unsafe.As<A, object>(ref MemoryMarshal.GetReference(span)), -1, span.Length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetupBack<A>(ref FoldState state, ReadOnlySpan<A> span) =>
            state = new FoldState(ref Unsafe.As<A, object>(ref MemoryMarshal.GetReference(span)), span.Length, span.Length);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool MoveNext<A>(ref FoldState state, out A value)
        {
            var ix = state.Index + 1;
            if (ix == state.Length)
            {
                value = default!;
                return false;
            }
            var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<object, A>(ref state.Span), state.Length);
            value = span[ix];
            state = new FoldState(ref state.Span, ix, state.Length);
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool MovePrev<A>(ref FoldState state, out A value)
        {
            var ix = state.Index - 1;
            if (ix < 0)
            {
                value = default!;
                return false;
            }
            var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<object, A>(ref state.Span), state.Length);
            value = span[ix];
            state = new FoldState(ref state.Span, ix, state.Length);
            return true;
        }
    }
}
