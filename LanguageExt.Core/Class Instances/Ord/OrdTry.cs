using System.Threading.Tasks;
using LanguageExt.TypeClasses;

namespace LanguageExt.ClassInstances
{
    public struct OrdTry<OrdA, A> : Ord<Try<A>> where OrdA : struct, Ord<A>
    {
        public int Compare(Try<A> x, Try<A> y)
        {
            var dx = x.Try();
            var dy = y.Try();
            if (dx.IsBottom && dy.IsBottom) return 0;
            if (dx.IsFaulted && dy.IsFaulted) return 0;
            if (dx.IsSuccess && dy.IsSuccess)
            {
                return default(OrdA).Compare(dx.Value, dy.Value);
            }

            if (dx.IsBottom && !dy.IsBottom) return -1;
            if (!dx.IsBottom && dy.IsBottom) return 1;

            if (dx.IsFaulted && !dy.IsFaulted) return -1;
            if (!dx.IsFaulted && dy.IsFaulted) return 1;

            if (dx.IsSuccess && !dy.IsSuccess) return -1;
            if (!dx.IsSuccess && dy.IsSuccess) return 1;
            return 0;
        }

        public bool Equals(Try<A> x, Try<A> y) =>
            default(EqTry<OrdA, A>).Equals(x, y);

        public int GetHashCode(Try<A> x) =>
            default(HashableTry<OrdA, A>).GetHashCode(x);
    
        public Task<int> GetHashCodeAsync(Try<A> x) => 
            GetHashCode(x).AsTask();

        public Task<bool> EqualsAsync(Try<A> x, Try<A> y) =>
            Equals(x, y).AsTask();

        public Task<int> CompareAsync(Try<A> x, Try<A> y) =>
            Compare(x, y).AsTask();
    }

    public struct OrdTry<A> : Ord<Try<A>>
    {
        public int Compare(Try<A> x, Try<A> y) =>
            default(OrdTry<OrdDefault<A>, A>).Compare(x, y);
 
        public bool Equals(Try<A> x, Try<A> y) =>
            default(EqTry<EqDefault<A>, A>).Equals(x, y);

        public int GetHashCode(Try<A> x) =>
            default(HashableTry<A>).GetHashCode(x);
    
        public Task<int> GetHashCodeAsync(Try<A> x) => 
            GetHashCode(x).AsTask();

        public Task<bool> EqualsAsync(Try<A> x, Try<A> y) =>
            Equals(x, y).AsTask();

        public Task<int> CompareAsync(Try<A> x, Try<A> y) =>
            Compare(x, y).AsTask();
    }
}
