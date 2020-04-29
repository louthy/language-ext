using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using LanguageExt.ClassInstances;

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
        const int DefaultCapacity = 32;
        A[] data = new A[DefaultCapacity];
        int count;
        int ncount = -1;
        IEnumerator<A> iter;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enum(IEnumerable<A> ma) =>
            iter = ma.GetEnumerator();

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => count;
        }

        public A[] Data
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => data;
        }

        public (bool Success, A Value) Get(int index)
        {
            while (true)
            {
                // Early out if the data has already been streamed
                if (index < count)
                {
                    return (true, data[index]);
                }

                // If there's nothing left to stream, we must be done
                var liter = iter;
                if (liter == null)
                {
                    // Check the index against the count again, just in case another
                    // thread has streamed something in 
                    return index < count
                        ? (true, data[index])
                        : (false, default);
                }

                var lcount = index - 1;

                // lindex is a lagging counter that gets moved on by 1 here.  It's the 
                // gatekeeper to moving along the iterator.  
                if (Interlocked.CompareExchange(ref ncount, index, lcount) == lcount)
                {
                    if (liter.MoveNext())
                    {
                        // Get the next value
                        var value = liter.Current;

                        // If we've run out of space, double it and copy.  
                        // Note, this operation is atomic 
                        if (index >= data.Length)
                        {
                            var ndata = new A[data.Length << 1];
                            Array.Copy(data, ndata, data.Length);
                            data = ndata;
                        }

                        // Store the value 
                        data[index] = value;

                        // Now, by updating the actual `count` we have essentially done an 
                        // atomic operation to get the value from the iterator and store it
                        // in our internal memory.
                        count = index + 1;

                        return (true, value);
                    }
                    else
                    {
                        // End of the iterator, so let's dispose
                        liter.Dispose();
                        iter = null;
                        ncount = count - 1;
                        return (false, default);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode() =>
            FNV32.Hash<HashableDefault<A>, A>(data, 0, count);
    }
}
