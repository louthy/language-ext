using System;
using System.Reflection;

namespace LanguageExt;

public class NonRecordAttribute : Attribute;

public class NonStructuralAttribute : Attribute;

public class NonHashAttribute : Attribute;

public class NonEqAttribute : Attribute;

public class NonOrdAttribute : Attribute;

public class NonShowAttribute : Attribute;

public class EqAttribute : Attribute
{
    public EqAttribute(Type type)
    {
        if (!type.GetTypeInfo()
                 .ImplementedInterfaces
                 .AsIterable()     
                 .Exists(i => i.ToString().StartsWith("LanguageExt.Traits.Eq`1")))
        {
            throw new Exception("Eq attribute should have a struct type that derives from LanguageExt.Traits.Eq<> passed as its argument");
        }
    }
}

public class OrdAttribute : Attribute
{
    public OrdAttribute(Type type)
    {
        if (!type.GetTypeInfo()
                 .ImplementedInterfaces
                 .AsIterable()
                 .Exists(i => i.ToString().StartsWith("LanguageExt.Traits.Ord`1")))
        {
            throw new Exception("Ord attribute should have a struct type that derives from LanguageExt.Traits.Ord<> passed as its argument");
        }
    }
}

public class HashableAttribute : Attribute
{
    public HashableAttribute(Type type)
    {
        if (!type.GetTypeInfo()
                 .ImplementedInterfaces
                 .AsIterable()
                 .Exists(i => i.ToString().StartsWith("LanguageExt.Traits.Hashable`1")))
        {
            throw new Exception("Hashable attribute should have a struct type that derives from LanguageExt.Traits.Hashable<> passed as its argument");
        }
    }
}

/// <summary>
/// Stops the base type fields being used for any Record operations
/// </summary>
/// <remarks>This is *not* used for the [Record] code-gen</remarks>
public class IgnoreBaseAttribute : Attribute;
