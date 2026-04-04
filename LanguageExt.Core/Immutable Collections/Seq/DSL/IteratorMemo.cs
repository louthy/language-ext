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
    
    /// <summary>
    /// The count of elements in the sequence.  This is the 'true' count of the sequence,
    /// and is updated atomically after each value is written to the last element position.
    /// </summary>
    long count;
    
    /// <summary>
    /// This is the 'live' count value that can be updated atomically with CompareExchange, but when
    /// it's updated, the element at `ncount - 1` is not set (it's `null` or `default`).  So, although
    /// we're updating the value atomically, we can't safely share this value anywhere until the element
    /// has been written to. The `count` field is the atomic count value that is set after that write
    /// operation is complete and should be used as the true external count of elements in the structure.
    /// </summary>
    long ncount;
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
            var lcount = count;
            if (index < lcount)
            {
                return data[index];
            }
            
            if (iter is null)
            {
                return default;
            }
        
            if (Interlocked.CompareExchange(ref ncount, lcount + 1, lcount) == lcount)
            {
                if (iter is (Exist<A>(var value), var tail))
                {
                    // Next
                    iter = tail;
                    
                    // If we've run out of space, double it and copy.  
                    // Note, this operation is atomic 
                    if (ncount >= data.LongLength)
                    {
                        var ndata = new A[data.LongLength << 1];
                        Array.Copy(data, ndata, data.LongLength);
                        data = ndata;
                    }

                    // Store the value 
                    data[lcount] = value;
                    
                    // Complete the atomic operation
                    count = ncount;
                    
                    // Continue, we may need to load more values before we get to the 
                    // index we're looking for.
                }
                else
                {
                    // End of the iterator
                    iter = null;
                    
                    // Continue, give a chance to collect the value from the array instead.
                    // This is because another thread may have updated iter without us knowing.
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
            var lcount = ncount;
            
            if (Interlocked.CompareExchange(ref ncount, lcount + 1, lcount) == lcount)
            {
                if (iter is (Exist<A>(var value), var tail))
                {
                    // Next
                    iter = tail;
                    
                    // If we've run out of space, double it and copy.  
                    // Note, this operation is atomic 
                    if (ncount >= data.LongLength)
                    {
                        var ndata = new A[data.LongLength << 1];
                        Array.Copy(data, ndata, data.LongLength);
                        data = ndata;
                    }

                    // Store the value 
                    data[lcount] = value;
                    
                    // Complete the atomic operation
                    count = ncount;
                }
                else
                {
                    // End of the iterator
                    iter = null;
                    
                    // Continue, give a chance to collect the value from the array instead.
                    // This is because another thread may have updated iter without us knowing.
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
        hash = GetHashCode(FNV32.OffsetBasis);
        return hash.Value;
    }

    public int GetHashCode(int offsetBasis) =>
        FNV32.Hash<HashableDefault<A>, A>(data, 0, count, offsetBasis); 
}
