using System;

namespace LanguageExt;

public abstract partial class Iterator<A> 
{
    /// <summary>
    /// Array iterator
    /// </summary>
    internal class IterArr(Arr<A> array, int index, int remaining) : Iterator<A>
    {
        protected override (Head<A> Head, Iterator<A> Tail) Next()
        {
            if(remaining == 0) return (Nil<A>.Default, Nil.Default);
            return (new Exist<A>(array[index]), new IterArr(array, index + 1, remaining - 1));
        }
    
        public override string ToString() => 
            $"Arr{array.ToString()}";
    }
}
