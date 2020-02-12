using System;
using System.Reflection;

namespace LanguageExt
{
    public class NonRecordAttribute : Attribute
    {
    }

    public class NonStructuralAttribute : Attribute
    {
    }

    public class NonHashAttribute : Attribute
    {
    }

    public class NonEqAttribute : Attribute
    {
    }

    public class NonOrdAttribute : Attribute
    {
    }

    public class NonShowAttribute : Attribute
    {
    }

    public class EqAttribute : Attribute
    {
        public EqAttribute(Type type)
        {
            if (!type.GetTypeInfo()
                       .ImplementedInterfaces
                       .Exists(i => i.ToString().StartsWith("LanguageExt.TypeClasses.Eq`1")))
            {
                throw new Exception("Eq attribute should have a struct type that derives from LanguageExt.TypeClasses.Eq<> passed as its argument");
            }
        }
    }

    public class OrdAttribute : Attribute
    {
        public OrdAttribute(Type type)
        {
            if (!type.GetTypeInfo()
                       .ImplementedInterfaces
                       .Exists(i => i.ToString().StartsWith("LanguageExt.TypeClasses.Ord`1")))
            {
                throw new Exception("Ord attribute should have a struct type that derives from LanguageExt.TypeClasses.Ord<> passed as its argument");
            }
        }
    }

    public class HashableAttribute : Attribute
    {
        public HashableAttribute(Type type)
        {
            if (!type.GetTypeInfo()
                       .ImplementedInterfaces
                       .Exists(i => i.ToString().StartsWith("LanguageExt.TypeClasses.Hashable`1")))
            {
                throw new Exception("Hashable attribute should have a struct type that derives from LanguageExt.TypeClasses.Hashable<> passed as its argument");
            }
        }
    }

    /// <summary>
    /// Stops the base type fields being used for any Record operations
    /// </summary>
    /// <remarks>This is *not* used for the [Record] code-gen</remarks>
    public class IgnoreBaseAttribute : Attribute
    {
    }

    [Obsolete("Use NonHash instead")]
    public class OptOutOfHashCodeAttribute : Attribute { }
    [Obsolete("Use NonEq instead")]
    public class OptOutOfEqAttribute : Attribute { }
    [Obsolete("Use NonOrd instead")]
    public class OptOutOfOrdAttribute : Attribute { }
    [Obsolete("Use NonShow instead")]
    public class OptOutOfToStringAttribute : Attribute { }
    [Obsolete("Use NonSerializable instead")]
    public class OptOutOfSerializationAttribute : Attribute { }
}
