using System;
using System.Diagnostics;
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
            IteratorState state = default!;
            r.FastIterationSetup(ref state);
            return state;
        }
        else
        {
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
    
    public ref struct IteratorState
    {
        #pragma warning disable CS0169 // Field is never used
        long state0;
        long state1;
        long state2;
        long state3;
        long state4;
        long state5;
        long state6;
        long state7;
        long state8;
        long state9;
        long stateA;
        long stateB;
        long stateC;
        long stateD;
        long stateE;
        long stateF;
        internal IDisposable backup;

        public IteratorState(IDisposable backup) => 
            this.backup = backup;
    }
}
