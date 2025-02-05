namespace LanguageExt.Pipes.Concurrent;

/// <summary>
/// Settings for `Mailbox` channels
/// </summary>
/// <typeparam name="A">Bound value type</typeparam>
public abstract record Buffer<A>
{
    internal Buffer()
    { }

    /// <summary>
    /// Store an unbounded number of messages in a FIFO queue
    /// </summary>
    public static readonly Buffer<A> Unbounded = new UnboundedBuffer<A>();
    
    /// <summary>
    /// Bounded number of messages to `1`
    /// </summary>
    public static readonly Buffer<A> Single = new SingleBuffer<A>();
    
    /// <summary>
    /// `Newest(1)`
    /// </summary>
    public static readonly Buffer<A> New = new NewBuffer<A>();
    
    /// <summary>
    /// Store a bounded number of messages, specified by the 'size' argument
    /// </summary>
    /// <param name="size">Bounded size of the buffer</param>
    public static Buffer<A> Bounded(uint size) => 
        size == 1
            ? new SingleBuffer<A>()
            : new BoundedBuffer<A>(size);
    
    /// <summary>
    /// Only store the 'Latest' message, beginning with an initial value
    /// </summary>
    /// <remarks>
    /// 'Latest' is never empty nor full.
    /// </remarks>
    public static Buffer<A> Latest(A value) => new LatestBuffer<A>(value);
    
    /// <summary>
    /// Like `Bounded`, but `Post` never fails (the buffer is never full).
    /// Instead, old elements are discarded to make room for new elements
    /// </summary>
    /// <param name="size">Size of the buffer</param>
    public static Buffer<A> Newest(uint size) => 
        size == 1
            ? new NewBuffer<A>()
            : new NewestBuffer<A>(size);
}

public sealed record UnboundedBuffer<A> : Buffer<A>;
public sealed record BoundedBuffer<A>(uint Size) : Buffer<A>;
public sealed record SingleBuffer<A> : Buffer<A>;
public sealed record LatestBuffer<A>(A Value) : Buffer<A>;
public sealed record NewestBuffer<A>(uint Size) : Buffer<A>;
public sealed record NewBuffer<A> : Buffer<A>;
