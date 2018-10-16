using System;
using System.Collections.Generic;
using System.Text;

namespace LanguageExt
{
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

        ~Enum()
        {
            iter?.Dispose();
            iter = null;
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
                lock (list)
                {
                    bool theresMore = true;
                    while (index >= list.Count && (theresMore = iter.MoveNext()))
                    {
                        list.Add(iter.Current);
                    }
                    if (!theresMore)
                    {
                        iter = null;
                    }
                }
            }
            if (index < list.Count)
            {
                return (true, list[index]);
            }
            else
            {
                return (false, default(A));
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
                        iter = null;
                    }
                }
            }
        }
    }
}
