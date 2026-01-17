using System;

namespace LanguageExt;

public abstract partial class IteratorIO<A> 
{
    /// <summary>
    /// Array IteratorIO
    /// </summary>
    internal class IterArr(Arr<A> array, int index, int remaining) : IteratorIO<A>
    {
        public Arr<A> Array => new (array.AsSpan(index, remaining));
        
        (Head<A> Head, IteratorIO<A> Tail) Next() =>
            remaining == 0
                ? Head.NilIO<A>()
                : Head.ExistIO(array[index], new IterArr(array, index + 1, remaining - 1));

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() => 
            IO.pure(Next());

        public override string ToString() => 
            $"Arr{array}";

        public override IteratorIO<A> Using() =>
            this;

        public override Arr<A> ToArr() =>
            array.Splice(index, remaining);
    }
    
    /// <summary>
    /// Array IteratorIO
    /// </summary>
    internal class IterArrBkwd(Arr<A> array, int index, int remaining) : IteratorIO<A>
    {
        (Head<A> Head, IteratorIO<A> Tail) Next() =>
            remaining == 0
                ? Head.NilIO<A>()
                : Head.ExistIO(array[index], new IterArr(array, index - 1, remaining - 1));

        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() => 
            IO.pure(Next());
    
        public override string ToString() => 
            $"Arr{array}";

        public override IteratorIO<A> Using() =>
            this;

        public override Arr<A> ToArr() =>
            array.Splice(index - remaining, remaining).Reverse();
    }
}
