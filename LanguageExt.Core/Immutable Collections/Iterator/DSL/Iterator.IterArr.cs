using System;

namespace LanguageExt;

public abstract partial class Iterator<A> 
{
    /// <summary>
    /// Array iterator
    /// </summary>
    internal class IterArr(Arr<A> array, long index, long remaining) : Iterator<A>
    {
        public Arr<A> Array => array.Splice(index, remaining);
        
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            remaining == 0
                ? Head.Nil<A>()
                : Head.Exist(array[index], new IterArr(array, index + 1, remaining - 1));

        public override string ToString() => 
            $"Arr{array}";

        public override Arr<A> ToArr() =>
            array.Splice(index, remaining);
    }
    
    /// <summary>
    /// Array iterator
    /// </summary>
    internal class IterArrBkwd(Arr<A> array, long index, long remaining) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            remaining == 0
                ? Head.Nil<A>()
                : Head.Exist(array[index], new IterArr(array, index - 1, remaining - 1));
    
        public override string ToString() => 
            $"Arr{array}";

        public override Arr<A> ToArr() =>
            array.Splice(index - remaining, remaining).Reverse();
    }
}
