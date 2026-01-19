using System;

namespace LanguageExt;

public abstract partial class IteratorIO<A> 
{
    /// <summary>
    /// Array IteratorIO
    /// </summary>
    internal class IterArr(Arr<A> array, long index, long remaining) : IteratorIO<A>
    {
        public Arr<A> Array => array.Splice(index, remaining);
        
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

        public override IO<Arr<A>> ToArr() =>
            IO.pure(array.Splice(index, remaining));
    }
    
    /// <summary>
    /// Array IteratorIO
    /// </summary>
    internal class IterArrBkwd(Arr<A> array, long index, long remaining) : IteratorIO<A>
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

        public override IO<Arr<A>> ToArr() =>
            IO.pure(array.Splice(index - remaining, remaining).Reverse());
    }
}
