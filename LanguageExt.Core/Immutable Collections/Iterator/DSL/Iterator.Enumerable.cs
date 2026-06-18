using System.Collections.Generic;

namespace LanguageExt;

public abstract partial class Iterator<A> 
{
    /// <summary>
    /// Enumerable Iterator
    /// </summary>
    /// <remarks>
    ///
    ///     DO NOT DELETE - this is used by Seq (at least for now) to wrap `IEnumerable`.
    /// 
    /// </remarks>
    internal class Enumerable(IEnumerable<A> enumerable) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next()
        {
            var enumerator = enumerable.GetEnumerator();
            if (enumerator.MoveNext())
            {
                return Head.Exist(enumerator.Current, new EnumeratorTail(enumerator));
            }
            else
            {
                enumerator.Dispose();
                return Head.Nil<A>();
            }
        }

        public override string ToString() =>
            "...";
    }
    
    /// <summary>
    /// Enumerator Iterator
    /// </summary>
    internal class EnumeratorTail(IEnumerator<A> enumerator) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next()
        {
            if (enumerator.MoveNext())
            {
                return Head.Exist(enumerator.Current, new EnumeratorTail(enumerator));
            }
            else
            {
                enumerator.Dispose();
                return Head.Nil<A>();
            }
        }

        public override string ToString() => 
            "...";
    }
}
