using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace LanguageExt
{
    /// <summary>
    /// Enumerates an IEnumerable at most once and caches
    /// the values in a List.  Seq uses this to iterate an
    /// enumerable by index, which allows this type to be
    /// shared.
    /// </summary>
    internal class Enum<A> : IDisposable
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

        public void Dispose()
        {
            iter?.Dispose();
            iter = null;
        }

        public int Count => list.Count;

        public void CopyTo(int sourceStart, A[] target, int targetStart, int amount) =>
            list.CopyTo(sourceStart, target, targetStart, amount);

        public (int Count, bool IsMore) GetRange(int from, int amount = 1)
        {
            if (iter == null) return (0, false);

            int count = 0;
            var start = Math.Min(from, list.Count);
            var end = from + amount;

            for (var i = start; i < end; i++ )
            {
                if (iter.MoveNext())
                {
                    list.Add(iter.Current);
                    count++;
                }
                else
                {
                    iter.Dispose();
                    iter = null;
                    return (count, false);
                }
            }
            return (count, true);
        }

        public (int Count, bool IsMore) GetTheRest(int from)
        {
            int count = 0;
            var start = Math.Min(from, list.Count);

            for (var i = start; ; i++)
            {
                if (iter.MoveNext())
                {
                    list.Add(iter.Current);
                    count++;
                }
                else
                {
                    iter.Dispose();
                    iter = null;
                    return (count, false);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (bool Success, A Value) Get(int index)
        {
            return index >= list.Count
                ? (false, default)
                : (true, list[index]);

            //if (index < list.Count)
            //{
            //    return (true, list[index]);
            //}
            //if (iter == null)
            //{
            //    return (false, default(A));
            //}
            //else
            //{
            //    bool theresMore = true;
            //    while (index >= list.Count && theresMore)
            //    {
            //        //lock (list)
            //        {
            //            if (iter == null)
            //            {
            //                theresMore = false;
            //            }
            //            else
            //            {
            //                theresMore = iter.MoveNext();
            //                if (theresMore)
            //                {
            //                    list.Add(iter.Current);
            //                }
            //                else
            //                {
            //                    iter.Dispose();
            //                    iter = null;
            //                }
            //            }
            //        }
            //    }
            //    return index < list.Count
            //        ? (true, list[index])
            //        : (false, default(A));
            //}
        }

        //public void Strict()
        //{
        //    if (iter != null)
        //    {
        //        //lock (list)
        //        {
        //            if (iter != null)
        //            {
        //                while (iter.MoveNext())
        //                {
        //                    list.Add(iter.Current);
        //                }
        //                iter.Dispose();
        //                iter = null;
        //            }
        //        }
        //    }
        //}
    }
}
