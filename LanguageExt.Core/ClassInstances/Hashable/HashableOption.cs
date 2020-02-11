using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Option type equality
    /// </summary>
    public struct HashableOption<A> : Hashable<Option<A>>
    {
        public int GetHashCode(Option<A> x) =>
            x.GetHashCode();
    }
}
