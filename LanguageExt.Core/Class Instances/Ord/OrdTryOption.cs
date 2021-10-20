using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct OrdTryOption<OrdA, A> : Ord<TryOption<A>> where OrdA : struct, Ord<A>
    {
        public int Compare(TryOption<A> x, TryOption<A> y)
        {
            var dx = x.Try();
            var dy = y.Try();
            if (dx.IsBottom && dy.IsBottom) return 0;
            if (dx.IsFaulted && dy.IsFaulted) return 0;
            if (dx.IsNone && dy.IsNone) return 0;
            if (dx.IsSome && dy.IsSome)
            {
                return default(OrdA).Compare(dx.Value.Value, dy.Value.Value);
            }

            if (dx.IsBottom && !dy.IsBottom) return -1;
            if (!dx.IsBottom && dy.IsBottom) return 1;

            if (dx.IsFaulted && !dy.IsFaulted) return -1;
            if (!dx.IsFaulted && dy.IsFaulted) return 1;

            if (dx.IsNone && !dy.IsNone) return -1;
            if (!dx.IsNone && dy.IsNone) return 1;

            if (dx.IsSome && !dy.IsSome) return -1;
            if (!dx.IsSome && dy.IsSome) return 1;
            return 0;
        }

        public bool Equals(TryOption<A> x, TryOption<A> y) =>
            default(EqTryOption<OrdA, A>).Equals(x, y);

        public int GetHashCode(TryOption<A> x) =>
            default(HashableTryOption<OrdA, A>).GetHashCode(x);
    
        public Task<int> GetHashCodeAsync(TryOption<A> x) => 
            GetHashCode(x).AsTask();

        public Task<bool> EqualsAsync(TryOption<A> x, TryOption<A> y) =>
            Equals(x, y).AsTask();

        public Task<int> CompareAsync(TryOption<A> x, TryOption<A> y) =>
            Compare(x, y).AsTask();
    }

    public struct OrdTryOption<A> : Ord<TryOption<A>>
    {
        public int Compare(TryOption<A> x, TryOption<A> y) =>
            default(OrdTryOption<OrdDefault<A>, A>).Compare(x, y);

        public bool Equals(TryOption<A> x, TryOption<A> y) =>
            default(EqTryOption<EqDefault<A>, A>).Equals(x, y);

        public int GetHashCode(TryOption<A> x) =>
            default(HashableTryOption<A>).GetHashCode(x);

        public Task<int> GetHashCodeAsync(TryOption<A> x) => 
            GetHashCode(x).AsTask();

        public Task<bool> EqualsAsync(TryOption<A> x, TryOption<A> y) =>
            Equals(x, y).AsTask();

        public Task<int> CompareAsync(TryOption<A> x, TryOption<A> y) =>
            Compare(x, y).AsTask();
    }
}
