using LanguageExt.TypeClasses;
using System.Reflection;

namespace LanguageExt.ClassInstances
{
    public struct HashableTypeInfo : Hashable<TypeInfo>
    {
        public int GetHashCode(TypeInfo x) =>
            x.GetHashCode();
    }
}
