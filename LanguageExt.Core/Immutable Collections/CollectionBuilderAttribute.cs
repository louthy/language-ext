#if !NET8_0_OR_GREATER
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Runtime.CompilerServices;

/// <summary>Initialize the attribute to refer to the <paramref name="methodName"/> method on the <paramref name="builderType"/> type.</summary>
/// <param name="builderType">The type of the builder to use to construct the collection.</param>
/// <param name="methodName">The name of the method on the builder to use to construct the collection.</param>
/// <remarks>
/// <paramref name="methodName"/> must refer to a static method that accepts a single parameter of
/// type <see cref="ReadOnlySpan{T}"/> and returns an instance of the collection being built containing
/// a copy of the data from that span.  In future releases of .NET, additional patterns may be supported.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
public sealed class CollectionBuilderAttribute : Attribute
{
    public CollectionBuilderAttribute(Type builderType, string methodName)
    {
        BuilderType = builderType;
        MethodName = methodName;
    }

    /// <summary>Gets the type of the builder to use to construct the collection.</summary>
    public Type BuilderType { get; }

    /// <summary>Gets the name of the method on the builder to use to construct the collection.</summary>
    /// <remarks>This should match the metadata name of the target method. For example, this might be ".ctor" if targeting the type's constructor.</remarks>
    public string MethodName { get; }
}
#endif
