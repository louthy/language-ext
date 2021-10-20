using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(Option<A> x, Option<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Option<A> x) =>
            GetHashCode(x).AsTask();       

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(Option<A> x, Option<A> y) =>
            Compare(x, y).AsTask();
    }

    public struct OrdOption<A> : Ord<Option<A>>
    {
        public int Compare(Option<A> x, Option<A> y) =>
            default(OrdOptional<MOption<A>, Option<A>, A>).Compare(x, y);

        public bool Equals(Option<A> x, Option<A> y) =>
            default(OrdOptional<MOption<A>, Option<A>, A>).Equals(x, y);

        public int GetHashCode(Option<A> x) =>
            default(OrdOptional<MOption<A>, Option<A>, A>).GetHashCode(x);

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(Option<A> x, Option<A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(Option<A> x) =>
            GetHashCode(x).AsTask();       

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(Option<A> x, Option<A> y) =>
            Compare(x, y).AsTask();
    }
}
