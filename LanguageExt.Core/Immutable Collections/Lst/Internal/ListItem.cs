using System;
using System.Runtime.CompilerServices;

interface IListItem;

[Serializable]
class ListItem<A> : IListItem
{
    public static ListItem<A> EmptyM => new (0, 0, null!, default!, null!);
    public static readonly ListItem<A> Empty = new(0, 0, null!, default!, null!);

    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Count == 0;
    }
    public long Count;
    public byte Height;
    public ListItem<A> Left;
    public ListItem<A> Right;

    /// <summary>
    /// Ctor
    /// </summary>
    internal ListItem(byte height, long count, ListItem<A> left, A value, ListItem<A> right)
    {
        Count = count;
        Height = height;
        Value = value;
        Left = left;
        Right = right;
    }

    internal int BalanceFactor =>
        Count == 0
            ? 0
            : Right.Height - Left.Height;

    public A Value
    {
        get;
        internal set;
    }

    public bool IsBalanced =>
        (uint)(BalanceFactor + 1) <= 2;

    public override string ToString() =>
        IsEmpty
            ? "(empty)"
            : Value?.ToString() ?? "[null]";
}

