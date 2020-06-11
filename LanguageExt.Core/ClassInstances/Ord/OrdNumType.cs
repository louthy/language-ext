using LanguageExt.TypeClasses;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static LanguageExt.TypeClass;

namespace LanguageExt.ClassInstances
{
    /// <summary>
    /// Compare the equality and ordering of any type in the NumType
    /// type-class
    /// </summary>
    public struct OrdNumType<NUMTYPE, NUM, A> : Ord<NumType<NUMTYPE, NUM, A>>
        where NUM : struct, Num<A>
        where NUMTYPE : NumType<NUMTYPE, NUM, A>
    {
        public static readonly OrdNumType<NUMTYPE, NUM, A> Inst = default(OrdNumType<NUMTYPE, NUM, A>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(NumType<NUMTYPE, NUM, A> x, NumType<NUMTYPE, NUM, A> y) =>
            default(EqNumType<NUMTYPE, NUM, A>).Equals(x, y);

        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x">Left hand side of the compare operation</param>
        /// <param name="y">Right hand side of the compare operation</param>
        /// <returns>
        /// if x greater than y : 1
        /// if x less than y    : -1
        /// if x equals y       : 0
        /// </returns>
        [Pure]
        public int Compare(NumType<NUMTYPE, NUM, A> mx, NumType<NUMTYPE, NUM, A> my)
        {
            if (ReferenceEquals(mx, my)) return 0;
            if (ReferenceEquals(mx, null)) return -1;
            if (ReferenceEquals(my, null)) return 1;
            return default(NUM).Compare((A)mx, (A)my);
        }

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(NumType<NUMTYPE, NUM, A> x) =>
            x.IsNull() ? 0 : x.GetHashCode();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(NumType<NUMTYPE, NUM, A> x, NumType<NUMTYPE, NUM, A> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(NumType<NUMTYPE, NUM, A> x) =>
            GetHashCode(x).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(NumType<NUMTYPE, NUM, A> x, NumType<NUMTYPE, NUM, A> y) =>
            Compare(x, y).AsTask();
    }

    /// <summary>
    /// Compare the equality and ordering of any type in the NumType
    /// type-class
    /// </summary>
    public struct OrdNumType<NUMTYPE, NUM, A, PRED> : Ord<NumType<NUMTYPE, NUM, A, PRED>>
        where NUM     : struct, Num<A>
        where PRED    : struct, Pred<A>
        where NUMTYPE : NumType<NUMTYPE, NUM, A, PRED>
    {
        public static readonly OrdNumType<NUMTYPE, NUM, A, PRED> Inst = default(OrdNumType<NUMTYPE, NUM, A, PRED>);

        /// <summary>
        /// Equality test
        /// </summary>
        /// <param name="x">The left hand side of the equality operation</param>
        /// <param name="y">The right hand side of the equality operation</param>
        /// <returns>True if x and y are equal</returns>
        [Pure]
        public bool Equals(NumType<NUMTYPE, NUM, A, PRED> x, NumType<NUMTYPE, NUM, A, PRED> y) =>
            default(EqNumType<NUMTYPE, NUM, A, PRED>).Equals(x, y);

        /// <summary>
        /// Compare two values
        /// </summary>
        /// <param name="x">Left hand side of the compare operation</param>
        /// <param name="y">Right hand side of the compare operation</param>
        /// <returns>
        /// if x greater than y : 1
        /// if x less than y    : -1
        /// if x equals y       : 0
        /// </returns>
        [Pure]
        public int Compare(NumType<NUMTYPE, NUM, A, PRED> mx, NumType<NUMTYPE, NUM, A, PRED> my)
        {
            if (ReferenceEquals(mx, my)) return 0;
            if (ReferenceEquals(mx, null)) return -1;
            if (ReferenceEquals(my, null)) return 1;
            return default(NUM).Compare((A)mx, (A)my);
        }

        /// <summary>
        /// Get the hash-code of the provided value
        /// </summary>
        /// <returns>Hash code of x</returns>
        [Pure]
        public int GetHashCode(NumType<NUMTYPE, NUM, A, PRED> x) =>
            x.IsNull() ? 0 : x.GetHashCode();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<bool> EqualsAsync(NumType<NUMTYPE, NUM, A, PRED> x, NumType<NUMTYPE, NUM, A, PRED> y) =>
            Equals(x, y).AsTask();

        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> GetHashCodeAsync(NumType<NUMTYPE, NUM, A, PRED> x) =>
            GetHashCode(x).AsTask();        
        
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task<int> CompareAsync(NumType<NUMTYPE, NUM, A, PRED> x, NumType<NUMTYPE, NUM, A, PRED> y) =>
            Compare(x, y).AsTask();    
    }
}
