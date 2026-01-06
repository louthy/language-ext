using System;

namespace LanguageExt;

/// <summary>
/// High-performance, but safe, array writer.
/// </summary>
/// <remarks>
/// This type is very much about facilitating the building of other collection types, where we want the
/// internals to be fast and the surface to be pure and lovely.  By providing a safe way to grow a backing
/// array, it makes implementing types like `Arr` and `Seq` much easier.  
/// </remarks>
/// <remarks>
/// This could be kept internal and private, but it's useful to expose it for others to leverage.
/// </remarks>
/// <typeparam name="A">Value type</typeparam>
public ref struct ArrayWriter<A>
{
    readonly int start;
    int count;
    int length;
    A[] buffer;

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="buffer">Backing buffer</param>
    /// <param name="start">Start offset</param>
    /// <param name="count">Number of items written</param>
    ArrayWriter(A[] buffer, int start)
    {
        this.buffer = buffer;
        this.start = start;
        this.length = buffer.Length;
    }

    /// <summary>
    /// Get a span of the values written so far.  This is a snapshot of the values only.
    /// </summary>
    public ReadOnlySpan<A> Span =>
        new (buffer, start, count);

    /// <summary>
    /// Use this to access the raw backing data.  This is obviously dangerous in the wrong hands, so make sure you
    /// only use the returned values AFTER you've finished working with the ArrayWriter instance and when you need
    /// those values to efficiently construct something else (like a ReadOnlySpan or another data structure)
    /// </summary>
    /// <remarks>NOTE: No array copying or realisation happens here, these are the raw references.</remarks>
    /// <returns>Raw backing data</returns>
    public (A[] Buffer, int Start, int Count) ToArray() =>
        new(buffer, start, count);
    
    /// <summary>
    /// Create an immutable array from the values written so far. The backing buffer is shared, so make sure
    /// you don't do any more writing to this ArrayWriter instance after calling this method.
    /// </summary>
    /// <returns>Arr</returns>
    public Arr<A> ToArr() =>
        new (buffer, start, count);
    
    Span<A> Top =>
        new (buffer, start + count, length - start - count);

    /// <summary>
    /// Create a new ArrayWriter
    /// </summary>
    /// <param name="initialCapacity">The initial capacity of the backing array</param>
    /// <param name="initialOffset">The initial offset into the array. Sometimes you may want to leave some space
    /// at the start of the array, so you have a pre-buffer, so use this to reserve some capacity near the start.</param>
    /// <returns></returns>
    public static ArrayWriter<A> Init(int initialCapacity = 16, int initialOffset = 0) =>
        initialOffset > initialCapacity
            ? throw new ArgumentOutOfRangeException(nameof(initialOffset))
            : new (new A[Math.Max(16, initialCapacity)], initialOffset);

    /// <summary>
    /// Add a value to the end of the array, expanding automatically if needed.
    /// </summary>
    /// <param name="writer">Writer to add to</param>
    /// <param name="value">Value to write</param>
    public static void Add(ref ArrayWriter<A> writer, A value)
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
    public static void AddRange(ref ArrayWriter<A> writer, ReadOnlySpan<A> values)
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
        
        if (start + count == length)
        {
            var nlength = length << 1;
            var nbuffer = new A[nlength];
            var nspan   = new Span<A>(nbuffer, start, count);
            var ospan   = new Span<A>(buffer, start, count);
            ospan.CopyTo(nspan);
            buffer = nbuffer;
            length = nlength;
        }
    }

    static void Expand(ref ArrayWriter<A> writer, int needed)
    {
        var     start  = writer.start;
        ref var count  = ref writer.count;
        ref var buffer = ref writer.buffer;
        ref var length = ref writer.length;

        var toAdd =  needed - (length - start - count);
        if (toAdd <= 0) return;
        
        var nlength = length << 1;
        while (needed - (nlength - start - count) > 0)
        {
            nlength <<= 1;
        }
        
        var nbuffer = new A[nlength];
        var nspan   = new Span<A>(nbuffer, start, count);
        var ospan   = new Span<A>(buffer, start, count);
        ospan.CopyTo(nspan);
        buffer = nbuffer;
        length = nlength;
    }
}
