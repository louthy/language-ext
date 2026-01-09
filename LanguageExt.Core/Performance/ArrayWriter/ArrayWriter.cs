using System;
using System.Buffers;
using System.Threading;

namespace LanguageExt;

/// <summary>
/// High-performance, but safe, array writer.
/// </summary>
/// <remarks>
/// <para>
/// This type is very much about facilitating the building of other collection types, where we want the
/// internals to be fast and the surface to be pure and lovely.  By providing a safe way to grow a backing
/// array, it makes implementing types like `Arr` and `Seq` much easier.  
/// </para>
/// <para>
/// Make sure you read the remarks for each `Init` and `InitOffset` method to understand how to use this
/// type in the most efficient manner.
/// </para>
/// </remarks>
/// <typeparam name="A">Value type</typeparam>
public ref struct ArrayWriter<A>
{
    /// <summary>
    /// Rented buffers may as well start at a reasonable size so that we're not doing
    /// lots of freeing and releasing for small lists.  We only copy the part of the
    /// rented array that we need, so it makes no real difference.
    /// </summary>
    const int MinimumRentedSize = 1024;
    
    /// <summary>
    /// If the array is owned (pre-allocated), then make sure it's at least this size
    /// in case it ends up unexpectedly growing.  
    /// </summary>
    const int MinimumOwnedSize = 16;
    
    /// <summary>
    /// Start offset into the backing array
    /// </summary>
    internal int start;
    
    /// <summary>
    /// Number of items written so far
    /// </summary>
    internal int count;
    
    /// <summary>
    /// Length of the backing array
    /// </summary>
    internal int length;
    
    /// <summary>
    /// Backing array.  This can be rented from the ArrayPool, or it can be pre-allocated. 
    /// </summary>
    internal A[] buffer;
    
    /// <summary>
    /// True if the backing array is rented from the ArrayPool.  False if it's pre-allocated.
    /// </summary>
    internal bool rented;

    /// <summary>
    /// 1 if the writer has been disposed. 0 otherwise
    /// </summary>
    internal int disposed;

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="buffer">Backing buffer</param>
    /// <param name="start">Start offset</param>
    /// <param name="count">Number of items written</param>
    ArrayWriter(A[] buffer, int start, bool rented)
    {
        this.start = start;
        this.buffer = buffer;
        this.length = buffer.Length;
        this.rented = rented;
    }

    /// <summary>
    /// Get a read-only span of the values written so far.  This is a snapshot of the values only.
    /// </summary>
    public ReadOnlySpan<A> View =>
        new (buffer, start, count);
    
    /// <summary>
    /// Get a span of values of the empty remaining space in the buffer
    /// </summary>
    Span<A> Top =>
        new (buffer, start + count, length - start - count);

    /// <summary>
    /// Create a new ArrayWriter
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use this overload if you don't know ahead of time what the size of the array will be. 
    /// </para>
    /// <para>
    /// The backing array is rented from an `ArrayPool` and will grow (return and re-rent) as needed. Upon realisation
    /// of the array - via `ToArray`, `ToArr`, etc. - then a new array of the expanded-to size will be allocated on the
    /// heap and copied to.  That means that there's no GC pressure whilst the array is growing and being written to.
    /// Only at the realisation stage do we hit the heap as normal.
    /// </para>
    /// <para>
    /// If you know ahead of time the size of the array you're going to write to, then you can use the other `Init`
    /// overload.  It will allocate the backing array on the heap upfront, and that will avoid the final copying stage.
    /// </para>
    /// </remarks>
    /// <returns>ArrayWriter</returns>
    public static ArrayWriter<A> Init() =>
        new (ArrayPool<A>.Shared.Rent(MinimumRentedSize), 0, true);

    /// <summary>
    /// Create a new ArrayWriter
    /// </summary>
    /// <remarks>
    /// <para>
    /// Because this overload of `Init` takes an explicit initial capacity, it's possible to pre-allocate the backing
    /// array on the heap and not use intermediate array(s) from the ArrayPool. This is useful if you know upfront that
    /// the array isn't going to expand any more because it will avoid the final copying stage that happens with rented
    /// buffers.
    /// </para>
    /// <para>
    /// That doesn't mean the array can't grow automatically, but you would have been better off not specifying an
    /// initial-capacity if you're not sure what the final size of the array will be.
    /// </para>
    /// </remarks>
    /// <param name="initialCapacity">The initial capacity of the backing array.  Use this if you have an idea ahead of
    /// time of what the space requirements will be.  If you don't know, use the other `Init` overload.</param>
    public static ArrayWriter<A> Init(int initialCapacity) =>
        new (new A[PowerOf2(AssertMinOwnedSize(initialCapacity))], 0, false);

    /// <summary>
    /// Create a new ArrayWriter
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use this overload if you don't know ahead of time what the size of the array will be. 
    /// </para>
    /// <para>
    /// The backing array is rented from an `ArrayPool` and will grow (return and re-rent) as needed. Upon realisation
    /// of the array - via `ToArray`, `ToArr`, etc. - then a new array of the expanded-to size will be allocated on the
    /// heap and copied to.  That means that there's no GC pressure whilst the array is growing and being written to.
    /// Only at the realisation stage do we hit the heap as normal.
    /// </para>
    /// <para>
    /// If you know ahead of time the size of the array you're going to write to, then you can use the other
    /// `InitOffset` overload.  It will allocate the backing array on the heap upfront, and that will avoid the final
    /// copying stage.
    /// </para>
    /// </remarks>
    /// <param name="offset">The offset into the array. Sometimes you may want to leave some space
    /// at the start of the array, so you have a pre-buffer, use this to reserve some capacity at the start.</param>
    /// <returns>ArrayWriter</returns>
    public static ArrayWriter<A> InitOffset(int offset) =>
        // We use the offset for the size here in case the initialOffset is bigger than the 
        // minimum rented size.  This is to avoid having to resize the array immediately.
        new (ArrayPool<A>.Shared.Rent(PowerOf2(AssertMinRentedSize(offset))), offset, true);

    /// <summary>
    /// Create a new ArrayWriter
    /// </summary>
    /// <remarks>
    /// <para>
    /// Because this overload of `InitOffset` takes an explicit initial capacity, it's possible to pre-allocate the
    /// backing array on the heap and not use intermediate array(s) from the ArrayPool. This is useful if you know
    /// upfront that the array isn't going to expand any more because it will avoid the final copying stage that happens
    /// with rented
    /// buffers.
    /// </para>
    /// <para>
    /// That doesn't mean the array can't grow automatically, but you would have been better off not specifying an
    /// initial-capacity if you're not sure what the final size of the array will be.
    /// </para>
    /// </remarks>
    /// <param name="initialCapacity">The initial capacity of the backing array.  Use this if you have an idea ahead of
    /// time of what the space requirements will be.  If you don't know, use the other `InitOffset` overload.</param>
    /// <param name="offset">The offset into the array. Sometimes you may want to leave some space
    /// at the start of the array, so you have a pre-buffer, use this to reserve some capacity at the start.</param>
    /// <returns>ArrayWriter</returns>
    /// <exception cref="ArgumentOutOfRangeException">If the initialOffset is greater than the initialCapacity</exception>
    public static ArrayWriter<A> InitOffset(int initialCapacity, int offset) =>
        offset > initialCapacity
            ? throw new ArgumentOutOfRangeException(nameof(offset))
            : new (new A[PowerOf2(AssertMinOwnedSize(initialCapacity))], offset, false);

    /// <summary>
    /// Add a value to the end of the array, expanding automatically if needed.
    /// </summary>
    /// <param name="writer">Writer to add to</param>
    /// <param name="value">Value to write</param>
    internal static void Add(ref ArrayWriter<A> writer, A value)
    {
        Expand(ref writer);
        var     start  = writer.start;
        ref var count  = ref writer.count;
        var     buffer = writer.buffer;
        buffer[start + count] = value;
        count++;
    }

    /// <summary>
    /// Add values to the end of the array, expanding automatically if needed.
    /// </summary>
    /// <param name="writer">Writer to add to</param>
    /// <param name="values">Values to write</param>
    internal static void AddRange(ref ArrayWriter<A> writer, ReadOnlySpan<A> values)
    {
        Expand(ref writer, values.Length);
        ref var count  = ref writer.count;
        values.CopyTo(writer.Top);
        count+=values.Length;
    }

    static void Expand(ref ArrayWriter<A> writer)
    {
        var     start  = writer.start;
        ref var count  = ref writer.count;
        ref var buffer = ref writer.buffer;
        ref var length = ref writer.length;
        ref var rented = ref writer.rented;

        if (start + count != length) return;
        var obuffer = buffer;
        var nlength = length << 1;
        var nbuffer = ArrayPool<A>.Shared.Rent(nlength);
        var nspan   = new Span<A>(nbuffer, start, count);
        var ospan   = new Span<A>(buffer, start, count);
        ospan.CopyTo(nspan);
        buffer = nbuffer;
        length = nlength;
        if (rented) ArrayPool<A>.Shared.Return(obuffer);
        rented = true;
    }

    static void Expand(ref ArrayWriter<A> writer, int needed)
    {
        var     start  = writer.start;
        ref var count  = ref writer.count;
        ref var buffer = ref writer.buffer;
        ref var length = ref writer.length;
        ref var rented = ref writer.rented;

        var toAdd =  needed - (length - start - count);
        if (toAdd <= 0) return;
        
        var nlength = length << 1;
        while (needed - (nlength - start - count) > 0)
        {
            nlength <<= 1;
        }
        
        var obuffer = buffer;
        var nbuffer = ArrayPool<A>.Shared.Rent(nlength);
        var nspan   = new Span<A>(nbuffer, start, count);
        var ospan   = new Span<A>(buffer, start, count);
        ospan.CopyTo(nspan);
        buffer = nbuffer;
        length = nlength;
        if (rented) ArrayPool<A>.Shared.Return(obuffer);
        rented = true;
    }

    public void Dispose()
    {
        if (rented && Interlocked.CompareExchange(ref disposed, 1, 0) == 0)
        {
            ArrayPool<A>.Shared.Return(buffer);
        }
    }

    static int AssertMinRentedSize(int size) =>
        size < MinimumRentedSize 
            ? MinimumRentedSize 
            : size;


    static int AssertMinOwnedSize(int size) =>
        size < MinimumOwnedSize 
            ? MinimumOwnedSize 
            : size;
    
    static int PowerOf2(int size)
    {
        size--;
        size |= size >> 1;
        size |= size >> 2;
        size |= size >> 4;
        size |= size >> 8;
        size |= size >> 16;
        size++;
        return size;
    }
}
