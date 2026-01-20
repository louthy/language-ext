using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace LanguageExt;

internal interface IMapItem;
internal interface IMapItem<K, V> : IMapItem
{
    (K Key, V Value) KeyValue
    {
        get;
    }
}

[Serializable]
class MapItem<K, V> :
    ISerializable,
    IMapItem<K, V>
{
    internal static readonly MapItem<K, V> Empty = new (0, 0, (default!, default!), default!, default!);

    internal bool IsEmpty => Count == 0;
    internal long Count;
    internal byte Height;
    internal MapItem<K, V> Left;
    internal MapItem<K, V> Right;

    /// <summary>
    /// Ctor
    /// </summary>
    internal MapItem(byte height, long count, (K Key, V Value) keyValue, MapItem<K, V> left, MapItem<K, V> right)
    {
        Count = count;
        Height = height;
        KeyValue = keyValue;
        Left = left;
        Right = right;
    }

    /// <summary>
    /// Deserialisation constructor
    /// </summary>
    MapItem(SerializationInfo info, StreamingContext context)
    {
        var key   = (K?)info.GetValue("Key", typeof(K))   ?? throw new SerializationException();
        var value = (V?)info.GetValue("Value", typeof(V)) ?? throw new SerializationException();
        KeyValue = (key, value);
        Count = 1;
        Height = 1;
        Left = Empty;
        Right = Empty;
    }

    /// <summary>
    /// Serialisation support
    /// </summary>
    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue("Key", KeyValue.Key, typeof(K));
        info.AddValue("Value", KeyValue.Value, typeof(V));
    }

    internal int BalanceFactor
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Count == 0
                   ? 0
                   : Right.Height - Left.Height;
    }

    public (K Key, V Value) KeyValue
    {
        get;
        internal set;
    }
}
