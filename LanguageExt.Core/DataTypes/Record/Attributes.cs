using System;
using System.Reflection;

namespace LanguageExt
{
    public class OptOutOfHashCodeAttribute : Attribute
    {
    }

    public class OptOutOfEqAttribute : Attribute
    {
    }

    public class OptOutOfOrdAttribute : Attribute
    {
    }

    public class OptOutOfToStringAttribute : Attribute
    {
    }

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
