using System;
using System.Runtime.Serialization;

namespace LanguageExt;

/// <summary>
/// Supports the building of record types (classes with sets of readonly field values)
/// Provides structural equality testing, ordering, and hashing
/// </summary>
/// <typeparam name="A">Type to provide methods for</typeparam>
public static class RecordType<A>
{
    /// <summary>
    /// General hash code function for record types
    /// </summary>
    public static Func<A, int> Hash => RecordTypeHash<A>.Hash;

    /// <summary>
    /// General equality function for record types
    /// </summary>
    public static Func<A, object, bool> Equality => RecordTypeEquality<A>.Equality;

    /// <summary>
    /// General typed equality function for record types
    /// </summary>
    public static Func<A, A, bool> EqualityTyped => RecordTypeEqualityTyped<A>.EqualityTyped;

    /// <summary>
    /// General typed comparison function for record types
    /// </summary>
    public static Func<A, A, int> Compare => RecordTypeCompare<A>.Compare;

    /// <summary>
    /// General ToString function
    /// </summary>
    public new static Func<A, string> ToString => RecordTypeToString<A>.ToString;

    /// <summary>
    /// De-serialise an A
    /// </summary>
    public static Action<A, SerializationInfo> SetObjectData => RecordTypeSetObjectData<A>.SetObjectData;

    /// <summary>
    /// Serialise an A
    /// </summary>
    public static Action<A, SerializationInfo> GetObjectData => RecordTypeGetObjectData<A>.GetObjectData;

    [Obsolete("Don't use Equals - use either RecordType<A>.Equality or RecordType<A>.EqualityTyped")]
    public new static bool Equals(object objA, object objB) => 
        throw new InvalidOperationException("Don't use Equals - use either RecordType<A>.Equality or RecordType<A>.EqualityTyped");
}

internal static class RecordTypeIncludeBase<A>
{
    internal static readonly bool IncludeBase;
    
    static RecordTypeIncludeBase() =>
        IncludeBase = !typeof(A).CustomAttributes
                                .AsIterable() 
                                .Exists(a => a.AttributeType.Name == nameof(IgnoreBaseAttribute));
}

internal static class RecordTypeHash<A>
{
    internal static readonly Func<A, int> Hash;
    
    static RecordTypeHash() => 
        Hash = IL.GetHashCode<A>(RecordTypeIncludeBase<A>.IncludeBase);
}

internal static class RecordTypeEquality<A>
{
    internal static readonly Func<A, object, bool> Equality;
    
    static RecordTypeEquality() => 
        Equality = IL.Equals<A>(RecordTypeIncludeBase<A>.IncludeBase);
}

internal static class RecordTypeEqualityTyped<A>
{
    internal static readonly Func<A, A, bool> EqualityTyped;
    
    static RecordTypeEqualityTyped() => 
        EqualityTyped = IL.EqualsTyped<A>(RecordTypeIncludeBase<A>.IncludeBase);
}

internal static class RecordTypeCompare<A>
{
    internal static readonly Func<A, A, int> Compare;
    
    static RecordTypeCompare() => 
        Compare = IL.Compare<A>(RecordTypeIncludeBase<A>.IncludeBase);
}

internal static class RecordTypeToString<A>
{
    internal new static readonly Func<A, string> ToString;
    
    static RecordTypeToString() => 
        ToString = IL.ToString<A>(RecordTypeIncludeBase<A>.IncludeBase);
}

internal static class RecordTypeSetObjectData<A>
{
    internal static readonly Action<A, SerializationInfo> SetObjectData;
    
    static RecordTypeSetObjectData() => 
        SetObjectData = IL.SetObjectData<A>(RecordTypeIncludeBase<A>.IncludeBase);
}

internal static class RecordTypeGetObjectData<A>
{
    internal static readonly Action<A, SerializationInfo> GetObjectData;
    
    static RecordTypeGetObjectData() => 
        GetObjectData = IL.GetObjectData<A>(RecordTypeIncludeBase<A>.IncludeBase);
}
