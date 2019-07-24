using LanguageExt.TypeClasses;
using System.Reflection;

namespace LanguageExt.ClassInstances
{
    public struct OrdTypeInfo : Ord<TypeInfo>
    {
        public int Compare(TypeInfo x, TypeInfo y) =>
            x.ToString().CompareTo(y.ToString());

        public bool Equals(TypeInfo x, TypeInfo y) =>
            default(EqTypeInfo).Equals(x, y);

        public int GetHashCode(TypeInfo x) =>
            default(EqTypeInfo).GetHashCode(x);
    }
}
