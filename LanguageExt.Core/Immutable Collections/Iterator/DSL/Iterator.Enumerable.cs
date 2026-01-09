using System;
using System.Collections.Generic;
using System.Threading;

namespace LanguageExt;

public abstract partial class Iterator<A> 
{
    /// <summary>
    /// Enumerable iterator
    /// </summary>
    internal class Enumerable : Iterator<A>
    {
        readonly IEnumerable<A> enumerable;

        public Enumerable(IEnumerable<A> enumerable) => 
            this.enumerable = enumerable;

        public override (Head<A> Head, Iterator<A> Tail) Next()
        {
            var enumerator = enumerable.GetEnumerator();
            if (enumerator.MoveNext())
            {
                return new Enumerator(enumerator.Current, new En(enumerator)).Next();
            }
            else
            {
                enumerator.Dispose();
                return (Nil<A>.Default, Nil.Default);
            }
        }

        public override string ToString() => 
            "...";
    }
    
    /// <summary>
    /// Enumerator iterator
    /// </summary>
    internal class Enumerator : Iterator<A>, IDisposable
    {
        readonly A head; 
        readonly En enumerator;

        public Enumerator(A head, En enumerator)
        {
            this.head = head;
            this.enumerator = enumerator;
        }

        public override (Head<A> Head, Iterator<A> Tail) Next()
        {
            if (!enumerator.Disposed && enumerator.Enumerator.MoveNext())
            {
                return new Enumerator(enumerator.Enumerator.Current, enumerator).Next();
            }
            else
            {
                enumerator.Dispose();
                return (Nil<A>.Default, Nil.Default);
            }
        }

        public override string ToString() => 
            $"{head}, ...";
    }

    /// <summary>
    /// Simple type to carry the enumerator and handle disposal. It allows `Dispose` to be
    /// called many times (because there could be umpteen references to it, so let the
    /// devs be overzealous with their clean-up)
    /// </summary>
    internal class En : IDisposable
    {
        int disposed;
        public readonly IEnumerator<A> Enumerator;

        public bool Disposed =>
            disposed == 1; 
        
        public En(IEnumerator<A> enumerator) => 
            Enumerator = enumerator;
        
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref disposed, 1, 0) == 0)
            {
                Enumerator.Dispose();
            }
        }
    }
}
