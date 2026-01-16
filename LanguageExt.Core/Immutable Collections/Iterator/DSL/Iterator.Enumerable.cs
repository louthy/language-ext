using System;
using System.Threading;
using System.Collections.Generic;

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
                return Head.Exist(enumerator.Current, new EnumeratorTail(new En(enumerator)));
            }
            else
            {
                enumerator.Dispose();
                return Head.Nil<A>();
            }
        }

        public override IO<(Head<A> Head, Iterator<A> Tail)> NextIO() => 
            IO.lift(Next);

        public override string ToString() =>
            "...";

        public override Iterator<A> Using()
        {
            var enumerator = enumerable.GetEnumerator();
            return new EnumeratorTail(new En(enumerator));
        }
    }
    
    /// <summary>
    /// Enumerator iterator
    /// </summary>
    internal class EnumeratorTail(En enumerator) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next()
        {
            if (!enumerator.Disposed && enumerator.Enumerator.MoveNext())
            {
                return Head.Exist(enumerator.Enumerator.Current, new EnumeratorTail(enumerator));
            }
            else
            {
                enumerator.Dispose();
                return Head.Nil<A>();
            }
        }

        public override IO<(Head<A> Head, Iterator<A> Tail)> NextIO() =>
            IO.lift(Next);

        public override Iterator<A> Using() => 
            this;

        public override void Dispose() =>
            enumerator.Dispose();

        public override string ToString() => 
            "...";
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
