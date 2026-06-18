using System.Buffers;
using System.Linq;

namespace LanguageExt;

public abstract partial class Iterator<A> 
{
    /// <summary>
    /// .NET Generic List iterator
    /// </summary>
    internal class IterGenList(System.Collections.Generic.IList<A> list, int index, int remaining) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            remaining == 0
                ? Head.Nil<A>()
                : Head.Exist(list[index], new IterGenList(list, index + 1, remaining - 1));

        public override string ToString() => 
            "System.Collections.Generic.List";

        public override Arr<A> ToArr()
        {
            var arr = list.Skip(index).Take(remaining).ToArray();
            return new (arr, 0, remaining);
        }
    }
    
    /// <summary>
    /// .NET Generic List iterator
    /// </summary>
    internal class IterGenListBkwd(System.Collections.Generic.IList<A> list, int index, int remaining) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            remaining == 0
                ? Head.Nil<A>()
                : Head.Exist(list[index], new IterGenListBkwd(list, index - 1, remaining - 1));
    
        public override string ToString() => 
            "System.Collections.Generic.List";

        public override Arr<A> ToArr()
        {
            var arr = list.Skip(index).Take(remaining).ToArray();
            System.Array.Reverse(arr);
            return new (arr, 0, remaining);
        }
    }
}
