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
    internal class Enum<A>
    {
        const int DefaultCapacity = 64;
        A[] data = new A[DefaultCapacity];
        int count;
        int lcount = -1;
        IEnumerator<A> iter;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enum(IEnumerable<A> ma) =>
            iter = ma.GetEnumerator();

        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => count;
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
                if (iter == null)
                {
                    // Check the index against the count again, just in case another
                    // thread has streamed something in 
                    return index < count
                        ? (true, data[index])
                        : (false, default);
                }

                var liter = iter;
                var lindex = index - 1;

                // lindex is a lagging counter that gets moved on by 1 here.  It's the 
                // gatekeeper to moving along the iterator.  
                if (Interlocked.CompareExchange(ref lcount, index, lindex) == lindex)
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
                        lcount = count - 1;
                        return (false, default);
                    }
                }
            }
        }

        public (int Taken, bool IsMore, A[] Data) GetRange(A[] data, int dataIndex, int index, int count)
        {
            int taken = 0;
            while(taken < count)
            {
                var (succ, val) = Get(index);
                if (succ)
                {
                    if (dataIndex >= data.Length)
                    {
                        var ndata = new A[data.Length << 1];
                        Array.Copy(data, ndata, data.Length);
                        data = ndata;
                    }
                    data[dataIndex++] = val;
                    taken++;
                    index++;
                }
                else
                {
                    return (taken, false, data);
                }
            }
            return (taken, true, data);
        }
    }
}
