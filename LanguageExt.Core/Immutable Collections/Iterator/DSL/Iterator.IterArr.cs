using System;

namespace LanguageExt;

public abstract partial class Iterator<A> 
{
    /// <summary>
    /// Array iterator
    /// </summary>
    internal class IterArr(Arr<A> array, int index, int remaining) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            remaining == 0
                ? (Nil<A>.Default, Nil.Default)
                : (new Exist<A>(array[index]), new IterArr(array, index + 1, remaining - 1));
    
        public override string ToString() => 
            $"Arr{array}";

        public override Iterator<A> Using() =>
            this;
    }
    
    /// <summary>
    /// Array iterator
    /// </summary>
    internal class IterArrBkwd(Arr<A> array, int index, int remaining) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            remaining == 0
                ? (Nil<A>.Default, Nil.Default)
                : (new Exist<A>(array[index]), new IterArr(array, index - 1, remaining - 1));
    
        public override string ToString() => 
            $"Arr{array}";

        public override Iterator<A> Using() =>
            this;
    }
}
