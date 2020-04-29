using LanguageExt.TypeClasses;
using System.Reflection;

namespace LanguageExt.ClassInstances
{
    public struct EqTypeInfo : Eq<TypeInfo>
    {
        public bool Equals(TypeInfo x, TypeInfo y) =>
            x.Equals(y);

        public int GetHashCode(TypeInfo x) =>
            default(HashableTypeInfo).GetHashCode(x);
    }
}
