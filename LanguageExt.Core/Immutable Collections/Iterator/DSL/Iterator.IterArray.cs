using System;

namespace LanguageExt;

public abstract partial class Iterator<A> 
{
    /// <summary>
    /// Array iterator
    /// </summary>
    internal class IterArray(A[] array, long index, long remaining) : Iterator<A>
    {
        public Arr<A> Array => new (array, index, remaining);
        
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            remaining == 0
                ? Head.Nil<A>()
                : Head.Exist(array[index], new IterArray(array, index + 1, remaining - 1));

        public override string ToString() => 
            $"Array";

        public override Arr<A> ToArr() =>
            Array;
    }
    
    /// <summary>
    /// Array iterator
    /// </summary>
    internal class IterArrayBkwd(A[] array, long index, long remaining) : Iterator<A>
    {
        public Arr<A> Array => new (array, index, remaining);
        
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            remaining == 0
                ? Head.Nil<A>()
                : Head.Exist(array[index], new IterArrayBkwd(array, index - 1, remaining - 1));
    
        public override string ToString() => 
            $"Array";

        public override Arr<A> ToArr() =>
            Array.Reverse();
    }
}
