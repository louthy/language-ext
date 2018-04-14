using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct OrdOptionUnsafe<OrdA, A> : Ord<OptionUnsafe<A>>
        where OrdA : struct, Ord<A>
    {
        public int Compare(OptionUnsafe<A> x, OptionUnsafe<A> y) =>
            default(OrdOptionalUnsafe<OrdA, MOptionUnsafe<A>, OptionUnsafe<A>, A>).Compare(x, y);

        public bool Equals(OptionUnsafe<A> x, OptionUnsafe<A> y) =>
            default(OrdOptionalUnsafe<OrdA, MOptionUnsafe<A>, OptionUnsafe<A>, A>).Equals(x, y);

        public int GetHashCode(OptionUnsafe<A> x) =>
            default(OrdOptionalUnsafe<OrdA, MOptionUnsafe<A>, OptionUnsafe<A>, A>).GetHashCode(x);
    }

    public struct OrdOptionUnsafe<A> : Ord<OptionUnsafe<A>>
    {
        public int Compare(OptionUnsafe<A> x, OptionUnsafe<A> y) =>
            default(OrdOptionalUnsafe<MOptionUnsafe<A>, OptionUnsafe<A>, A>).Compare(x, y);

        public bool Equals(OptionUnsafe<A> x, OptionUnsafe<A> y) =>
            default(OrdOptionalUnsafe<MOptionUnsafe<A>, OptionUnsafe<A>, A>).Equals(x, y);

        public int GetHashCode(OptionUnsafe<A> x) =>
            default(OrdOptionalUnsafe<MOptionUnsafe<A>, OptionUnsafe<A>, A>).GetHashCode(x);
    }
}
