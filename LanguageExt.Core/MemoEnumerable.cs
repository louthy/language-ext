using System;
using System.Collections;
using System.Collections.Generic;
namespace LanguageExt
{
    /// <summary>
    /// Enumerable memoization.  As an enumerable is enumerated each item is retained
    /// in an internal list, so that future evalation of the enumerable isn't done. 
    /// Only items not seen before are evaluated.  
    /// 
    /// This minimises one of the major problems with the IEnumerable / yield return 
    /// pattern by causing at-most-once evaluation of each item.  
    /// 
    /// Use the IEnumerable extension method Memo for convenience.
    /// </summary>
    /// <remarks>
    /// Although this allows efficient lazy evaluation, it does come at a memory cost.
    /// Each item is cached internally, so this method doesn't allow for evaluation of
    /// infinite sequences.
    /// </remarks>
    class MemoEnumerable<T> : IEnumerable<T>
    {
        readonly IEnumerable<T> seq;
        readonly IEnumerator<T> iter;
        readonly object sync = new object();
        Lst<T> items = List.empty<T>();
        bool complete;

        internal MemoEnumerable(IEnumerable<T> seq)
        {
            this.seq = seq;
            this.iter = seq.GetEnumerator();
        }

        public IEnumerable<T> AsEnumerable()
        {
            lock (sync)
            {
                foreach (var item in items)
                {
                    yield return item;
                }
                if (!complete)
                {
                    while (iter.MoveNext())
                    {
                        items = items.Add(iter.Current);
                        yield return iter.Current;
                    }
                    complete = true;
                }
            }
        }

        public IEnumerator<T> GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            AsEnumerable().GetEnumerator();
    }
}
