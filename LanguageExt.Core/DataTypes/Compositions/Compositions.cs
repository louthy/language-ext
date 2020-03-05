using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.TypeClasses;
using System.Collections.Generic;
using LanguageExt.ClassInstances;
using System.Collections;
using System;
using System.Linq;

namespace LanguageExt
{
    /// <summary>
    /// A _compositions list_ or _composition tree_ is a list data type where the elements are monoids, 
    /// and the 'mconcat' of any contiguous sublist can be computed in logarithmic time.
    /// 
    /// A common use case of this type is in a wiki, version control system, or collaborative editor, where 
    /// each change or delta would be stored in a list, and it is sometimes necessary to compute the composed 
    /// delta between any two versions.
    ///
    /// This version of a composition list is strictly biased to right-associativity, in that we only support 
    /// efficient consing to the front of the list. This also means that the 'take' operation can be inefficient. 
    /// 
    /// The append operation `append(a, b)` performs `O(a log (a + b))` element compositions, so you want
    /// the left-hand list `a` to be as small as possible.
    /// </summary>
    public struct Compositions<A> : IEquatable<Compositions<A>>, IEnumerable<A>
    {
        public static readonly Compositions<A> Empty = new Compositions<A>(Seq<Node>());

        public readonly Seq<Node> Tree;
        int? hashCode;

        internal Compositions(Seq<Node> tree)
        {
            hashCode = null;
            Tree = tree;
        }

        /// <summary>
        /// Returns true if the given tree is appropriately right-biased.
        /// </summary>
        public bool WellFormed<MonoidEqA>() where MonoidEqA : struct, Monoid<A>, Eq<A> =>
            Compositions.wellFormed<MonoidEqA, A>(this);

        /// <summary>
        /// Return the compositions list with the first `k` elements removed, in `O(log k)` time.
        /// </summary>
        public Compositions<A> Skip(int amount) =>
            Compositions.skip(amount, this);

        /// <summary>
        /// Return the compositions list containing only the first `k` elements
        /// of the input.  In the worst case, performs `O(k log k)` element compositions,
        /// in order to maintain the right-associative bias.  If you wish to run `composed`
        /// on the result of `take`, use `takeComposed` for better performance.
        /// </summary>
        public Compositions<A> Take<MonoidA>(int amount) where MonoidA : struct, Monoid<A> =>
            Compositions.take<MonoidA, A>(amount, this);

        /// <summary>
        /// Returns the composition of the first `k` elements of the compositions list, doing only `O(log k)` compositions.
        /// Faster than simply using `take` and then `composed` separately.
        /// </summary>
        public A TakeComposed<MonoidA>(int amount) where MonoidA : struct, Monoid<A> =>
            Compositions.takeComposed<MonoidA, A>(amount, this);

        /// <summary>
        /// A convenience alias for 'take' and 'skip'
        /// </summary>
        public (Compositions<A> taken, Compositions<A> skipped) SplitAt<MonoidA>(int i)
            where MonoidA : struct, Monoid<A> =>
                Compositions.splitAt<MonoidA, A>(i, this);

        /// <summary>
        /// Compose every element in the compositions list. Performs only `O(log n)` compositions.
        /// </summary>
        public A Composed<MonoidA>()
            where MonoidA : struct, Monoid<A> =>
                Compositions.composed<MonoidA, A>(this);

        /// <summary>
        /// Construct a compositions list containing just one element.
        /// </summary>
        public static Compositions<A> Singleton(A value) =>
            Compositions.singleton(value);

        /// <summary>
        /// Get the number of elements in the compositions list, in `O(log n)` time.
        /// </summary>
        public int Count() =>
            Compositions.count(this);

        /// <summary>
        /// Convert a compositions list into a list of elements. The other direction
        /// is provided in the 'Data.Foldable.Foldable' instance.This will perform O(n log n) element compositions.
        /// </summary>
        public static Compositions<A> FromList<MonoidA>(IEnumerable<A> ma)
            where MonoidA : struct, Monoid<A> =>
                Compositions.fromList<MonoidA, A>(ma);

        /// <summary>
        /// Equality operator
        /// </summary>
        public bool Equals(Compositions<A> b) =>
            default(EqEnumerable<A>).Equals(this, b);

        /// <summary>
        /// Equality operator
        /// </summary>
        public override bool Equals(object obj) =>
            obj is Compositions<A> b && Equals(b);

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(Compositions<A> a, Compositions<A> b) =>
            a.Equals(b);

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator !=(Compositions<A> a, Compositions<A> b) =>
            !(a == b);

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() =>
            hashCode ?? (hashCode = hash(this)) ?? 0;

        /// <summary>
        /// Convert to a sequence
        /// </summary>
        public Seq<A> ToSeq() =>
            default(FoldCompositions<A>).Fold(this, Seq<A>(), (s, x) => x.Cons(s))(unit);

        /// <summary>
        /// Convert to an enumerable
        /// </summary>
        public IEnumerable<A> AsEnumerable() =>
            default(FoldCompositions<A>).Fold(this, Seq<A>(), (s, x) => x.Cons(s))(unit);

        /// <summary>
        /// Get enumerator
        /// </summary>
        public IEnumerator<A> GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        /// <summary>
        /// Get enumerator
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        public class Node
        {
            public readonly int Size;
            public readonly Option<(Node, Node)> Children;
            public readonly A Value;

            public Node(int size, Option<(Node, Node)> children, A value)
            {
                Size = size;
                Children = children;
                Value = value;
            }
        }
    }
}
