using System;

namespace LanguageExt;

public partial class Range
{
    public record State<A>(A Current, bool LastWasEnd, A Stop, Func<A, A> Step, Func<A, A, bool> Eq)
    {
        public A Current = Current;
        public bool LastWasEnd = LastWasEnd;
        public readonly A Stop = Stop;
        public readonly Func<A, A> Step = Step;
        public readonly Func<A, A, bool> Eq = Eq;
    }
    
    public readonly struct IteratorState<A>
    {
        public readonly object State;
        public IteratorState(object state) =>
            State = state;

        public static IteratorState<A> Setup(A start, A stop, Func<A, A> step, Func<A, A, bool> eq) =>
            new (new State<A>(start, eq(start, stop), stop, step, eq));
    }    
    public readonly ref struct IteratorState
    {
        public readonly object State;
        public IteratorState(object state) =>
            State = state;

        public static IteratorState Setup<A>(A start, A stop, Func<A, A> step, Func<A, A, bool> eq) =>
            new (new State<A>(start, eq(start, stop), stop, step, eq));
    }
}

public static class RangeIteratorStateExtensions
{
    extension(ref Range.IteratorState self)
    {
        public bool Step<A>(out A value)
        {
            var state = (Range.State<A>)self.State;
            value = state.Current;
            if (state.LastWasEnd)
            {
                return false;
            }
            else
            {
                state.LastWasEnd = state.Eq(state.Current, state.Stop);
                state.Current = state.Step(state.Current);
                return true;
            }
        }
    }
    
    extension<A>(ref Range.IteratorState<A> self)
    {
        public bool Step(out A value)
        {
            var state = (Range.State<A>)self.State;
            value = state.Current;
            if (state.LastWasEnd)
            {
                return false;
            }
            else
            {
                state.LastWasEnd = state.Eq(state.Current, state.Stop);
                state.Current = state.Step(state.Current);
                return true;
            }
        }
    }    
}
