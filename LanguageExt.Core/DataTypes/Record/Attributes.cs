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
