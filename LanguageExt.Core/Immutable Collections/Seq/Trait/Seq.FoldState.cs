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
        readonly ref byte Span;
        readonly int Length;
        int Index;
        object? Iter; 

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        FoldState(ref byte span, int length, int index, object? @enum)
        {
            Span = ref span;
            Length = length;
            Index = index;
            Iter = @enum;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FoldState FromSpan<A>(ReadOnlySpan<A> span) =>
            new (ref Unsafe.As<A, byte>(ref MemoryMarshal.GetReference(span)), span.Length, -1, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FoldState FromSpanBack<A>(ReadOnlySpan<A> span) =>
            new (ref Unsafe.As<A, byte>(ref MemoryMarshal.GetReference(span)), span.Length, span.Length, null);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FoldState FromIterator<A>(Iterator<A> iterator) =>
            new (ref Unsafe.NullRef<byte>(), 0, -1, iterator);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool MoveNext<A>(ref FoldState state, out A value)
        {
            ref var iter = ref Unsafe.As<object?, Iterator<A>?>(ref state.Iter);
            if (iter is null)
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
                    var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<byte, A>(ref state.Span), state.Length);
                    value = span[ix];
                    return true;
                }
            }
            else
            {
                if (iter is (Exist<A> (var head), var tail))
                {
                    value = head;
                    iter = tail;
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
                var span = MemoryMarshal.CreateReadOnlySpan(ref Unsafe.As<byte, A>(ref state.Span), state.Length);
                value = span[ix];
                return true;
            }
        }
    }
}
