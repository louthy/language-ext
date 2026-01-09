using System;
using System.Collections.Generic;
using System.Threading;

namespace LanguageExt;

public abstract partial class Iterator<A> 
{
    /// <summary>
    /// Enumerable iterator
    /// </summary>
    internal class Enumerable(IEnumerable<A> enumerable) : Iterator<A>
    {
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
    internal class Enumerator(A head, En enumerator) : Iterator<A>, IDisposable
    {
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
    internal class En(IEnumerator<A> enumerator) : IDisposable
    {
        int disposed;
        public readonly IEnumerator<A> Enumerator = enumerator;

        public bool Disposed =>
            disposed == 1;

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref disposed, 1, 0) == 0)
            {
                Enumerator.Dispose();
            }
        }
    }
}
