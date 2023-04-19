namespace LanguageExt.SourceGen;

public abstract record Stack<A>
{
    /// <summary>
    /// Empty stack
    /// </summary>
    public static readonly Stack<A> Empty = new StackEmpty<A>();

    /// <summary>
    /// Push an item on the stack
    /// </summary>
    /// <summary>
    /// Push an item on the stack
    /// </summary>
    public Stack<A> Push(A value) =>
        new StackItem<A>(value, this);

    /// <summary>
    /// Pop an item off the stack
    /// </summary>
    public abstract Stack<A> Pop(out A value);

    /// <summary>
    /// Peek at an item at the top of the stack
    /// </summary>
    public abstract A Peek();

    /// <summary>
    /// True if the stack is empty
    /// </summary>
    public abstract bool IsEmpty { get; }

}

public record StackEmpty<A> : Stack<A>
{
    public override bool IsEmpty => 
        true;

    /// <summary>
    /// Pop an item off the stack
    /// </summary>
    public override Stack<A> Pop(out A value) =>
        throw new IndexOutOfRangeException();

    /// <summary>
    /// Peek at an item at the top of the stack
    /// </summary>
    public override A Peek() =>
        throw new IndexOutOfRangeException();
}

public record StackItem<A>(A Value, Stack<A> Next) : Stack<A>
{
    public override bool IsEmpty => 
        false;

    /// <summary>
    /// Pop an item off the stack
    /// </summary>
    public override Stack<A> Pop(out A value)
    {
        value = Value;
        return Next;
    }

    /// <summary>
    /// Peek at an item at the top of the stack
    /// </summary>
    public override A Peek() =>
        Value;
}
