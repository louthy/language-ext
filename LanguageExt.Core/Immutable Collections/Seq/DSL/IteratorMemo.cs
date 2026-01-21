using System;
using System.Threading;
using LanguageExt.ClassInstances;

namespace LanguageExt;

/// <summary>
/// Enumerates an IEnumerable at most once and caches
/// the values in a List.  Seq uses this to iterate an
/// enumerable by index, which allows this type to be
/// shared.
/// </summary>
class IteratorMemo<A>(Iterator<A> ma) : Iterator<A>
{
    const int DefaultCapacity = 32;
    A[] data = new A[DefaultCapacity];
    long count;
    long ncount = -1;
    int? hash;
    
    Iterator<A>? iter = ma;

    public long Count =>
        count;

    public A[] Data =>
        data;

    public Option<A> Get(long index)
    {
        SpinWait sw = default;
        while (true)
        {
            // Early out if the data has already been streamed
            if (index < count)
            {
                return data[index];
            }

            // If there's nothing left to stream, we must be done
            var liter = iter;
            var lcount = index - 1;

            // lcount is a lagging counter that gets moved on by 1 here.  It's the 
            // gatekeeper to moving along the iterator.  
            if (Interlocked.CompareExchange(ref ncount, index, lcount) == lcount)
            {
                if (liter is (Exist<A>(var value), var tail))
                {
                    // Next
                    iter = tail;
                    
                    // If we've run out of space, double it and copy.  
                    // Note, this operation is atomic 
                    if (index >= data.LongLength)
                    {
                        var ndata = new A[data.LongLength << 1];
                        Array.Copy(data, ndata, data.LongLength);
                        data = ndata;
                    }

                    // Store the value 
                    data[index] = value;

                    // Now, by updating the actual `count` we have essentially done an 
                    // atomic operation to get the value from the iterator and store it
                    // in our internal memory.
                    count = index + 1;

                    return value;
                }
                else
                {
                    // End of the iterator
                    iter = null;
                    ncount = count - 1;
                    return default;
                }
            }
            else
            {
                sw.SpinOnce();
            }
        }
    }

    public long GetAll()
    {
        SpinWait sw = default;
        while (true)
        {
            if(iter is null) return count;
            
            var index = count;
            
            // If there's nothing left to stream, we must be done
            var liter  = iter;
            var lcount = index - 1;

            // lcount is a lagging counter that gets moved on by 1 here.  It's the 
            // gatekeeper to moving along the iterator.  
            if (Interlocked.CompareExchange(ref ncount, index, lcount) == lcount)
            {
                if (liter is (Exist<A>(var value), var tail))
                {
                    // Next
                    iter = tail;
                    
                    // If we've run out of space, double it and copy.  
                    // Note, this operation is atomic 
                    if (index >= data.LongLength)
                    {
                        var ndata = new A[data.LongLength << 1];
                        Array.Copy(data, ndata, data.LongLength);
                        data = ndata;
                    }

                    // Store the value 
                    data[index] = value;

                    // Now, by updating the actual `count` we have essentially done an 
                    // atomic operation to get the value from the iterator and store it
                    // in our internal memory.
                    count = index + 1;
                }
                else
                {
                    // End of the iterator
                    iter = null;
                    ncount = count - 1;
                    return count;
                }
            }
            else
            {
                sw.SpinOnce();
            }
        }
    }

    public override (Head<A> Head, Iterator<A> Tail) Next() =>
        Get(0) switch
        {
            { IsSome: true, Value: var value } => Head.Exist(value!, new Reader(1, this)),
            _                                  => Head.Nil<A>()
        };

    class Reader(long index, IteratorMemo<A> memo) : Iterator<A>
    {
        public override (Head<A> Head, Iterator<A> Tail) Next() =>
            memo.Get(index) switch
            {
                { IsSome: true, Value: var value } => Head.Exist(value!, new Reader(index + 1, memo)),
                _                                  => Head.Nil<A>()
            };
    }

    public override int GetHashCode()
    {
        if (hash is not null) return hash.Value;
        GetAll();
        hash = FNV32.Hash<HashableDefault<A>, A>(data, 0, count); 
        return hash.Value;
    }
}
