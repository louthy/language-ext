using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using LanguageExt.Traits;

namespace LanguageExt;

/// <summary>
/// General purpose range trait implementation
/// </summary>
public partial class Range : Foldable<Range, Range.IteratorState>
{
    static Iterator<A> IterableK<Range>.ForwardIterator<A>(K<Range, A> fa)
    {
        var r = +fa;
        return r.ForwardIterator();
    }

    static IteratorState IterableK<Range, IteratorState>.StepSetup<A>(K<Range, A> ta)
    {
        var r = +ta;
        if (r.SupportsFastIteration)
        {
            // Attempts to fast-iterate if the Range says it supports it and the size of its state
            // is small enough. 
            IteratorState state = default!;
            r.FastIterationSetup(ref state);
            return state;
        }
        else
        {
            // Fall back to iterating normally
            return new IteratorState(r.ForwardIterator());
        }
    }

    static bool IterableK<Range, IteratorState>.Step<A>(K<Range, A> ta, ref IteratorState refState, out A value)
    {
        var r = +ta;
        if (r.SupportsFastIteration)
        {
            var c =  r.FastIterationStep(ref refState, out value);
            return c;
        }
        else
        {
            ref var disp = ref refState.backup;
            var iter = (Iterator<A>)disp;
            if (iter is (Exist<A> (var head), var tail))
            {
                disp = tail;
                value = head;
                return true;
            }
            else
            {
                disp.Dispose();
                value = default!;
                return false;
            }
        }
    }
    
    public ref struct IteratorState(IDisposable backup)
    {
        #pragma warning disable CS0169 // Field is never used
        long state0;
        long state1;
        long state2;
        long state3;        // < These are here to take space in the struct that can be used
        long state4;        //   by the various Range implementors to store their iteration
        long state5;        //   state for the fast-iterators.
        long state6;
        long state7;        //   Note: we check that they're not going to overflow the struct.
        long state8;        //   But a poorly behaved Range could lie about its usage and
        long state9;        //   cause an overflow. 
        long stateA;
        long stateB;        //   We can't use 0xDeadBeef in the normal way, so some thought
        long stateC;        //   is needed to how we can verify that ranges are well-behaved
        long stateD;        //   without incurring additional overhead.
        long stateE;
        long stateF;
        internal IDisposable backup = backup;

        public static S To<S>(scoped ref IteratorState from)
            where S : struct, allows ref struct
        {
            Debug.Assert(Unsafe.SizeOf<S>() <= Unsafe.SizeOf<IteratorState>());
            return Unsafe.ReadUnaligned<S>(ref Unsafe.As<IteratorState, byte>(ref from));
        }
        
        public static IteratorState From<S>(scoped ref S from)
            where S : struct, allows ref struct
        {
            Debug.Assert(Unsafe.SizeOf<S>() <= Unsafe.SizeOf<IteratorState>());
            return Unsafe.ReadUnaligned<IteratorState>(ref Unsafe.As<S, byte>(ref from));
        }
    }
}
