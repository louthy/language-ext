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

    [Obsolete("Use NonHash instead")]
    public class OptOutOfHashCodeAttribute : Attribute
    {
    }

    public class NonHashAttribute : Attribute
    {
    }

    [Obsolete("Use NonEq instead")]
    public class OptOutOfEqAttribute : Attribute
    {
    }

    public class NonEqAttribute : Attribute
    {
    }

    [Obsolete("Use NonOrd instead")]
    public class OptOutOfOrdAttribute : Attribute
    {
    }

    public class NonOrdAttribute : Attribute
    {
    }

    [Obsolete("Use NonShow instead")]
    public class OptOutOfToStringAttribute : Attribute
    {
    }

    public class NonShowAttribute : Attribute
    {
    }

    [Obsolete("Use NonSerializable instead")]
    public class OptOutOfSerializationAttribute : Attribute
    {
    }

    /// <summary>
    /// Stops the base type fields being used for any Record operations
    /// </summary>
    public class IgnoreBaseAttribute : Attribute
    {
    }

    public class EqAttribute : Attribute
    {
        public EqAttribute(Type eqType)
        {
            if(!eqType.GetTypeInfo().ImplementedInterfaces.Exists(i => i.ToString().StartsWith("LanguageExt.TypeClasses.Eq`1")))
            {
                throw new Exception("Eq attribute should have a struct type that derives from LanguageExt.TypeClasses.Eq<> passed as its argument");
            }
        }
    }
}
