using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct OrdOption<OrdA, A> : Ord<Option<A>>
        where OrdA : struct, Ord<A>
    {
        public int Compare(Option<A> x, Option<A> y) =>
            default(OrdOptional<OrdA, MOption<A>, Option<A>, A>).Compare(x, y);

        public bool Equals(Option<A> x, Option<A> y) =>
            default(OrdOptional<OrdA, MOption<A>, Option<A>, A>).Equals(x, y);

        public int GetHashCode(Option<A> x) =>
            default(OrdOptional<OrdA, MOption<A>, Option<A>, A>).GetHashCode(x);
    }

    public struct OrdOption<A> : Ord<Option<A>>
    {
        public int Compare(Option<A> x, Option<A> y) =>
            default(OrdOptional<MOption<A>, Option<A>, A>).Compare(x, y);

        public bool Equals(Option<A> x, Option<A> y) =>
            default(OrdOptional<MOption<A>, Option<A>, A>).Equals(x, y);

        public int GetHashCode(Option<A> x) =>
            default(OrdOptional<MOption<A>, Option<A>, A>).GetHashCode(x);
    }
}
