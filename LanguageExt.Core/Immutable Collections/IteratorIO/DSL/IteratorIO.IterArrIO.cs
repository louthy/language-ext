using System;

namespace LanguageExt;

public abstract partial class IteratorIO<A> 
{
    /// <summary>
    /// Array IteratorIO
    /// </summary>
    internal class IterArrIO(IO<Arr<A>> arrayIO, int index, int remaining) : IteratorIO<A>
    {
        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() => 
            arrayIO * (array =>
                           remaining == 0
                               ? Head.NilIO<A>()
                               : Head.ExistIO(array[index], new IterArr(array, index + 1, remaining - 1)));

        public override string ToString() => 
            $"ArrIO[...]";

        public override IteratorIO<A> Using() =>
            this;

        public override IO<Arr<A>> ToArr() =>
            arrayIO.Map(arr => arr.Slice(index, remaining));
    }
    
    /// <summary>
    /// Array IteratorIO
    /// </summary>
    internal class IterArrBkwdIO(IO<Arr<A>> arrayIO, int index, int remaining) : IteratorIO<A>
    {
        public override IO<(Head<A> Head, IteratorIO<A> Tail)> NextIO() => 
            arrayIO * (array =>
                           remaining == 0
                               ? Head.NilIO<A>()
                               : Head.ExistIO(array[index], new IterArr(array, index - 1, remaining - 1)));
    
        public override string ToString() => 
            $"ArrIO[...]";

        public override IteratorIO<A> Using() =>
            this;

        public override IO<Arr<A>> ToArr() =>
            arrayIO.Map(arr => arr.Slice(index - remaining, remaining).Reverse());
    }
}
