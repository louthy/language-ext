using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace LanguageExt;

public partial class Arr
{
    /// <summary>
    /// ref struct used to track the state of a fold operation.
    /// </summary>
    public ref struct FoldState
    {
        readonly ref object Span;
        readonly int Length;
        int Index;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FoldState(ref object span, int length, int index)
        {
            Span = ref span;
            Index = index;
            Length = length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Setup<A>(ref FoldState state, ReadOnlySpan<A> span) =>
            state = new FoldState(ref Unsafe.As<A, object>(ref MemoryMarshal.GetReference(span)), span.Length, -1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetupBack<A>(ref FoldState state, ReadOnlySpan<A> span) =>
            state = new FoldState(ref Unsafe.As<A, object>(ref MemoryMarshal.GetReference(span)), span.Length, span.Length);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool MoveNext<A>(ref FoldState state, out A value)
        {
            ref var          ix  = ref state.Index;
            ref readonly var len = ref state.Length;
            
            ix++;
            if (ix == len)
            {
                value = default!;
                return false;
            }
            var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<object, A>(ref state.Span), state.Length);
            value = span[ix];
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool MovePrev<A>(ref FoldState state, out A value)
        {
            ref var ix = ref state.Index;
            ix--;
            if (ix < 0)
            {
                value = default!;
                return false;
            }
            var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<object, A>(ref state.Span), state.Length);
            value = span[ix];
            return true;
        }
    }
}
