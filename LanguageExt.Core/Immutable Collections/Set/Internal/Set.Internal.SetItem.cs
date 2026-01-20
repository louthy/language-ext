namespace LanguageExt;

interface ISetItem;

class SetItem<K> : ISetItem
{
    public static readonly SetItem<K> Empty = new (0, 0, default!, null!, null!);

    public bool IsEmpty => Count == 0;
    public long Count;
    public byte Height;
    public SetItem<K> Left;
    public SetItem<K> Right;

    internal SetItem(byte height, long count, K key, SetItem<K> left, SetItem<K> right)
    {
        Count = count;
        Height = height;
        Key = key;
        Left = left;
        Right = right;
    }

    internal int BalanceFactor =>
        Count == 0
            ? 0
            : Right.Height - Left.Height;

    public K Key
    {
        get;
        internal set;
    }
}
