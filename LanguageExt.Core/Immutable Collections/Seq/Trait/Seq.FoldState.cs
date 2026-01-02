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
    public ref struct FoldState
    {
        readonly ref object Span;
        readonly int Length;
        int Index;
        readonly IEnumerator? Enum; 

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FoldState(ref object span, int length, int index, IEnumerator? @enum)
        {
            Span = ref span;
            Length = length;
            Index = index;
            Enum = @enum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromSpan<A>(ref FoldState state, ReadOnlySpan<A> span) =>
            state = new FoldState(ref Unsafe.As<A, object>(ref MemoryMarshal.GetReference(span)), span.Length, -1, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromSpanBack<A>(ref FoldState state, ReadOnlySpan<A> span) =>
            state = new FoldState(ref Unsafe.As<A, object>(ref MemoryMarshal.GetReference(span)), span.Length, span.Length, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FromEnumerator(ref FoldState state, IEnumerator @enum) =>
            state = new FoldState(ref Unsafe.NullRef<object>(), 0, -1, @enum);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool MoveNext<A>(ref FoldState state, out A value)
        {
            if (state.Enum is null)
            {
                ref var          ix  = ref state.Index;
                ref readonly var len = ref state.Length;
                ix++;
                
                if (ix == len)
                {
                    value = default!;
                    return false;
                }
                else
                {
                    var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<object, A>(ref state.Span), state.Length);
                    value = span[ix];
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
            else
            {
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<object, A>(ref state.Span), state.Length);
                value = span[ix];
                return true;
            }
        }
    }
}
