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
            var end = start + amount;

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (bool IsMore, A Value) GetNext(int from)
        {
            var liter = iter;
            if (iter == null)
            {
                return from < list.Count
                    ? (true, list[from])
                    : (false, default);
            }
            else if (from < list.Count)
            {
                return (true, list[from]);
            }
            else if (liter.MoveNext())
            {
                list.Add(liter.Current);
                return (true, list[from]);
            }
            else
            {
                liter.Dispose();
                iter = null;
                return (false, default);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int Count, bool IsMore) GetTheRest(int from)
        {
            var liter = iter;
            if (iter == null)
            {
                return (list.Count - from, default);
            }

            int count = 0;
            var start = Math.Min(from, list.Count);

            for (var i = start; ; i++)
            {
                if (liter.MoveNext())
                {
                    list.Add(liter.Current);
                    count++;
                }
                else
                {
                    liter.Dispose();
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
        }
    }
}
