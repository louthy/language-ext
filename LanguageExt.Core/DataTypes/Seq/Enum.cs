using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageExt
{
    /// <summary>
    /// Enumerates an IEnumerable at most once and caches
    /// the values in a List.  Seq uses this to iterate an
    /// enumerable by index, which allows this type to be
    /// shared.
    /// </summary>
    internal class Enum<A>
    {
        IEnumerator<A> iter;
        List<A> list;
        public Enum(IEnumerable<A> seq)
        {
            this.iter = seq.GetEnumerator();
            this.list = new List<A>();
        }

        public Enum(IEnumerator<A> iter)
        {
            this.iter = iter;
            this.list = new List<A>();
        }

        public (bool Success, A Value) Get(int index)
        {
            if (index < list.Count)
            {
                return (true, list[index]);
            }
            if (iter == null)
            {
                return (false, default(A));
            }
            else
            {
                bool theresMore = true;
                while (index >= list.Count && theresMore)
                {
                    lock (list)
                    {
                        if (iter == null)
                        {
                            theresMore = false;
                        }
                        else
                        {
                            theresMore = iter.MoveNext();
                            if (theresMore)
                            {
                                list.Add(iter.Current);
                            }
                            else
                            {
                                iter.Dispose();
                                iter = null;
                            }
                        }
                    }
                }
                return index < list.Count
                    ? (true, list[index])
                    : (false, default(A));
            }
        }

        public void Strict()
        {
            if (iter != null)
            {
                lock (list)
                {
                    if (iter != null)
                    {
                        while (iter.MoveNext())
                        {
                            list.Add(iter.Current);
                        }
                        iter.Dispose();
                        iter = null;
                    }
                }
            }
        }
    }
}
