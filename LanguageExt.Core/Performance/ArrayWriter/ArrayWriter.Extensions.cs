using System;
using System.Buffers;
using System.Threading;

namespace LanguageExt;

public static class ArrayWriterExtensions
{
    extension<A>(ref ArrayWriter<A> writer)
    {
        /// <summary>
        /// Add a value to the end of the array, expanding automatically if needed.
        /// </summary>
        /// <param name="writer">Writer to add to</param>
        /// <param name="value">Value to write</param>
        public void Add(A value) =>
            ArrayWriter<A>.Add(ref writer, value);

        /// <summary>
        /// Add values to the end of the array, expanding automatically if needed.
        /// </summary>
        /// <param name="writer">Writer to add to</param>
        /// <param name="values">Values to write</param>
        public void AddRange(ReadOnlySpan<A> values) =>
            ArrayWriter<A>.AddRange(ref writer, values);

        /// <summary>
        /// Use this to extract the backing buffer 
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this `ArrayWriter` was initialised without specifying an initial capacity, or if the backing-array
        /// had to expand beyond its initial capacity, then that means the backing-array is rented and therefore calling
        /// this method will copy the rented buffer to a new buffer on the heap, and then it will release the
        /// backing-array to the array-pool.
        /// </para>
        /// <para>
        /// If the `ArrayWriter` was initialised with an initial capacity and no expansion beyond that capacity was
        /// required, then the backing-buffer will already be on the heap. Therefore, no copying will occur, and
        /// you'll receive the raw reference to the heap-based array. 
        /// </para>
        /// <para>
        /// This method will also reset the entire `ArrayWriter` to an 'empty state'.  It will be unusable and
        /// effectively disposed. 
        /// </para>
        /// </remarks>
        public Span<A> ToSpan()
        {
            var (xs, start, count) = writer.ToArray();
            return start + count > int.MaxValue
                        ? throw new InvalidOperationException("Backing collection is too big to return a view")
                        : new Span<A>(xs, (int)start, (int)count);
        }

        /// <summary>
        /// Use this to extract the backing buffer (in reverse order) 
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this `ArrayWriter` was initialised without specifying an initial capacity, or if the backing-array
        /// had to expand beyond its initial capacity, then that means the backing-array is rented and therefore calling
        /// this method will copy the rented buffer to a new buffer on the heap, and then it will release the
        /// backing-array to the array-pool.
        /// </para>
        /// <para>
        /// If the `ArrayWriter` was initialised with an initial capacity and no expansion beyond that capacity was
        /// required, then the backing-buffer will already be on the heap. Therefore, no copying will occur, and
        /// you'll receive the raw reference to the heap-based array. 
        /// </para>
        /// <para>
        /// This method will also reset the entire `ArrayWriter` to an 'empty state'.  It will be unusable and
        /// effectively disposed. 
        /// </para>
        /// </remarks>
        public Span<A> ToSpanBack()
        {
            var (xs, start, count) = writer.ToArrayBack();
            return start + count > int.MaxValue
                       ? throw new InvalidOperationException("Backing collection is too big to return a view")
                       : new Span<A>(xs, (int)start, (int)count);
        }
        
        /// <summary>
        /// Use this to extract the backing buffer, start position, and element count. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this `ArrayWriter` was initialised without specifying an initial capacity, or if the backing-array
        /// had to expand beyond its initial capacity, then that means the backing-array is rented and therefore calling
        /// this method will copy the rented buffer to a new buffer on the heap, and then it will release the
        /// backing-array to the array-pool.
        /// </para>
        /// <para>
        /// If the `ArrayWriter` was initialised with an initial capacity and no expansion beyond that capacity was
        /// required, then the backing-buffer will already be on the heap. Therefore, no copying will occur, and
        /// you'll receive the raw reference to the heap-based array. 
        /// </para>
        /// <para>
        /// This method will also reset the entire `ArrayWriter` to an 'empty state'.  It will be unusable and
        /// effectively disposed. 
        /// </para>
        /// </remarks>
        public (A[] Buffer, long Start, long Count) ToArray()
        {
            if (Interlocked.CompareExchange(ref writer.disposed, 1, 0) == 0)
            {
                ref var start  = ref writer.start;
                ref var count  = ref writer.count;
                ref var buffer = ref writer.buffer;
                ref var rented = ref writer.rented;
                
                if (rented)
                {
                    var final = new A[start + count];

                    if (start + count > int.MaxValue)
                    {
                        Array.Copy(buffer, start, final, start, count);   
                    }
                    else
                    {
                        var bspan = new Span<A>(buffer, (int)start, (int)count);
                        var fspan = new Span<A>(final, (int)start, (int)count);
                        bspan.CopyTo(fspan);
                    }

                    ArrayPool<A>.Shared.Return(buffer);
                    var r = (final, start, count);
                    writer.RefDispose();
                    return r;
                }
                else
                {
                    var r = (buffer, start, count);
                    writer.RefDispose();
                    return r;
                }
            }
            else
            {
                throw new ObjectDisposedException(nameof(ArrayWriter<>));
            }
        }
        
        /// <summary>
        /// Use this to extract the backing buffer (in reverse order), start position, and element count. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this `ArrayWriter` was initialised without specifying an initial capacity, or if the backing-array
        /// had to expand beyond its initial capacity, then that means the backing-array is rented and therefore calling
        /// this method will copy the rented buffer to a new buffer on the heap, and then it will release the
        /// backing-array to the array-pool.
        /// </para>
        /// <para>
        /// If the `ArrayWriter` was initialised with an initial capacity and no expansion beyond that capacity was
        /// required, then the backing-buffer will already be on the heap. Therefore, no copying will occur, and
        /// you'll receive the raw reference to the heap-based array. 
        /// </para>
        /// <para>
        /// This method will also reset the entire `ArrayWriter` to an 'empty state'.  It will be unusable and
        /// effectively disposed. 
        /// </para>
        /// </remarks>
        public (A[] Buffer, long Start, long Count) ToArrayBack()
        {
            if (Interlocked.CompareExchange(ref writer.disposed, 1, 0) == 0)
            {
                ref var start  = ref writer.start;
                ref var count  = ref writer.count;
                ref var buffer = ref writer.buffer;
                ref var rented = ref writer.rented;
                
                if (rented)
                {
                    var final = new A[start + count];
                    if (start + count > int.MaxValue)
                    {
                        Array.Copy(buffer, start, final, start, count);
                        Array.Reverse(final);
                        
                        var ns = buffer.Length - start;
                        ArrayPool<A>.Shared.Return(buffer);
                        var r = (final, ns, count);
                        writer.RefDispose();
                        return r;
                    }
                    else
                    {
                        var bspan = new Span<A>(buffer, (int)start, (int)count);
                        var fspan = new Span<A>(final, (int)start, (int)count);
                        bspan.CopyTo(fspan);
                        fspan.Reverse();

                        ArrayPool<A>.Shared.Return(buffer);
                        var r = (final, start, count);
                        writer.RefDispose();
                        return r;
                    }
                }
                else
                {
                    if (start + count > int.MaxValue)
                    {
                        Array.Reverse(buffer);
                        var ns = buffer.Length - start;
                        var r  = (buffer, ns, count);
                        writer.RefDispose();
                        return r;
                    }
                    else
                    {
                        var s = new Span<A>(buffer, (int)start, (int)count);
                        s.Reverse();
                        
                        var r = (buffer, start, count);
                        writer.RefDispose();
                        return r;
                    }
                }
            }
            else
            {
                throw new ObjectDisposedException(nameof(ArrayWriter<>));
            }
        }

        /// <summary>
        /// Use this to extract the backing buffer, start position, and element count. 
        /// </summary>
        /// <remarks>
        /// <para>
        /// If this `ArrayWriter` was initialised without specifying an initial capacity, or if the backing-array
        /// had to expand beyond its initial capacity, then that means the backing-array is rented and therefore calling
        /// this method will copy the rented buffer to a new buffer on the heap, and then it will release the
        /// backing-array to the array-pool.
        /// </para>
        /// <para>
        /// If the `ArrayWriter` was initialised with an initial capacity and no expansion beyond that capacity was
        /// required, then the backing-buffer will already be on the heap and therefore won't be copied.
        /// </para>
        /// <para>
        /// This method will also reset the entire `ArrayWriter` to an 'empty state'.  It will be unusable and
        /// effectively disposed. 
        /// </para>
        /// </remarks>
        public Arr<A> ToArr()
        {
            if (Interlocked.CompareExchange(ref writer.disposed, 1, 0) == 0)
            {
                ref var start  = ref writer.start;
                ref var count  = ref writer.count;
                ref var buffer = ref writer.buffer;
                ref var rented = ref writer.rented;
                
                if (rented)
                {
                    var final = new A[start + count];

                    if (start + count > int.MaxValue)
                    {
                        Array.Copy(buffer, start, final, start, count);   
                    }
                    else
                    {
                        var bspan = new Span<A>(buffer, (int)start, (int)count);
                        var fspan = new Span<A>(final, (int)start, (int)count);
                        bspan.CopyTo(fspan);
                    }

                    ArrayPool<A>.Shared.Return(buffer);
                    var r = new Arr<A>(final, start, count);
                    writer.RefDispose();
                    return r;
                }
                else
                {
                    var r = new Arr<A>(buffer, start, count);
                    writer.RefDispose();
                    return r;
                }
            }
            else
            {
                throw new ObjectDisposedException(nameof(ArrayWriter<>));
            }            
        }
        
        void RefDispose()
        {
            ref var start  = ref writer.start;
            ref var count  = ref writer.count;
            ref var buffer = ref writer.buffer;
            ref var length = ref writer.length;
            ref var rented = ref writer.rented;
            
            buffer = Array.Empty<A>();
            count = 0;
            start = 0;
            length = 0;
            rented = false;
        }
    }
}
