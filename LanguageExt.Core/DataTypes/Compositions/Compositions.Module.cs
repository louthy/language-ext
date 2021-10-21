using LanguageExt;
using static LanguageExt.Prelude;
using LanguageExt.TypeClasses;
using System;
using System.Collections.Generic;
using LanguageExt.ClassInstances;

namespace LanguageExt
{
    public static class Compositions
    {
        /// <summary>
        /// Returns true if the given tree is appropriately right-biased.
        /// </summary>
        public static bool wellFormed<MonoidEqA, A>(Compositions<A> ca) where MonoidEqA : struct, Monoid<A>, Eq<A>
        {
            bool wellFormedNode(int n, Compositions<A>.Node node)
            {
                var ni = node.Size;
                if (n == 1 && ni == 1 && node.Children.IsNone) return true;
                else if (node.Children.IsSome && n == ni)
                {
                    var v = node.Value;
                    var (l, r) = node.Children.IfNone((default(Compositions<A>.Node), default(Compositions<A>.Node)));
                    return wellFormedNode(n / 2, l) &&
                           default(MonoidEqA).Equals(v, default(MonoidEqA).Append(l.Value, r.Value)) &&
                           wellFormedNode(n / 2, r);
                }
                else
                {
                    return false;
                }
            }

            bool go(int m, Seq<Compositions<A>.Node> ma)
            {
                if (ma.IsEmpty) return true;
                var x = ma.Head;
                var xs = ma.Tail;
                var s = x.Size;
                return s >= m && wellFormedNode(s, x) && go(s * 2, xs);
            }
            return go(1, ca.Tree);
        }

        /// <summary>
        /// Return the compositions list with the first `k` elements removed, in `O(log k)` time.
        /// </summary>
        public static Compositions<A> skip<A>(int amount, Compositions<A> compositions)
        {
            Seq<Compositions<A>.Node> go(int n, Seq<Compositions<A>.Node> nodes)
            {
                if (nodes.IsEmpty) return nodes;
                if (n <= 0) return nodes;
                var s = nodes.Head.Size;
                var c = nodes.Head.Children;
                var ri = nodes.Tail;
                var ord = n.CompareTo(s);
                if (ord < 0)
                {
                    var (l, r) = c.IfNone((default(Compositions<A>.Node), default(Compositions<A>.Node)));
                    return go(n, l.Cons(r.Cons(ri)));
                }
                else
                {
                    return go(n - s, ri);
                }
            }
            return new Compositions<A>(go(amount, compositions.Tree));
        }

        /// <summary>
        /// Return the compositions list containing only the first `k` elements
        /// of the input.  In the worst case, performs `O(k log k)` element compositions,
        /// in order to maintain the right-associative bias.  If you wish to run `composed`
        /// on the result of `take`, use `takeComposed` for better performance.
        /// </summary>
        public static Compositions<A> take<MonoidA, A>(int amount, Compositions<A> compositions) where MonoidA : struct, Monoid<A>
        {
            Seq<Compositions<A>.Node> go(int n, Seq<Compositions<A>.Node> nodes)
            {
                if (nodes.IsEmpty) return nodes;
                if (n <= 0) return Seq<Compositions<A>.Node>();
                var x = nodes.Head;
                var s = x.Size;
                var c = x.Children;
                var ri = nodes.Tail;
                var ord = n.CompareTo(s);
                if (ord < 0)
                {
                    var (l, r) = c.IfNone((default(Compositions<A>.Node), default(Compositions<A>.Node)));
                    return go(n, l.Cons(r.Cons(ri)));
                }
                else
                {
                    return default(MCompositions<MonoidA, A>)
                                .Append(new Compositions<A>(Seq1(x)), new Compositions<A>(go(n - s, ri)))
                                .Tree;
                }
            }
            return new Compositions<A>(go(amount, compositions.Tree));
        }

        /// <summary>
        /// Returns the composition of the first `k` elements of the compositions list, doing only `O(log k)` compositions.
        /// Faster than simply using `take` and then `composed` separately.
        /// </summary>
        public static A takeComposed<MonoidA, A>(int amount, Compositions<A> compositions) where MonoidA : struct, Monoid<A>
        {
            A go(int n, Seq<Compositions<A>.Node> nodes)
            {
                if (nodes.IsEmpty) return default(MonoidA).Empty();
                if (n <= 0) return default(MonoidA).Empty();
                var x = nodes.Head;
                var s = x.Size;
                var c = x.Children;
                var v = x.Value;
                var ri = nodes.Tail;
                var ord = n.CompareTo(s);
                if (ord < 0)
                {
                    var (l, r) = c.IfNone((default(Compositions<A>.Node), default(Compositions<A>.Node)));
                    return go(n, l.Cons(r.Cons(ri)));
                }
                else
                {
                    return default(MonoidA).Append(v, go(n - s, ri));
                }
            }
            return go(amount, compositions.Tree);
        }

        /// <summary>
        /// A convenience alias for 'take' and 'drop'
        /// </summary>
        public static (Compositions<A> taken, Compositions<A> skipped) splitAt<MonoidA, A>(int i, Compositions<A> c)
            where MonoidA : struct, Monoid<A> =>
                (take<MonoidA, A>(i, c), skip(i, c));

        /// <summary>
        /// Compose every element in the compositions list. Performs only `O(log n)` compositions.
        /// </summary>
        public static A composed<MonoidA, A>(Compositions<A> compositions)
            where MonoidA : struct, Monoid<A> =>
                default(FoldCompositions<A>).Fold(compositions, default(MonoidA).Empty(), default(MonoidA).Append)(unit);

        /// <summary>
        /// Construct a compositions list containing just one element.
        /// </summary>
        public static Compositions<A> singleton<A>(A value) =>
            new Compositions<A>(Seq1(new Compositions<A>.Node(1, None, value)));

        /// <summary>
        /// Get the number of elements in the compositions list, in `O(log n)` time.
        /// </summary>
        public static int count<A>(Compositions<A> compositions) =>
            compositions.Tree.Map(n => n.Size).Sum();

        /// <summary>
        /// Add a new element to the front of a compositions list. Performs `O(log n)` element compositions.
        /// </summary>
        public static Compositions<A> cons<MonoidA, A>(A x, Compositions<A> xs)
            where MonoidA : struct, Monoid<A> =>
                default(MCompositions<MonoidA, A>).Append(singleton(x), xs);

        /// <summary>
        /// Convert a compositions list into a list of elements. The other direction
        /// is provided in the 'Data.Foldable.Foldable' instance.This will perform O(n log n) element compositions.
        /// </summary>
        public static Compositions<A> fromList<MonoidA, A>(IEnumerable<A> ma)
            where MonoidA : struct, Monoid<A> =>
                ma.Fold(default(MCompositions<MonoidA, A>).Empty(), (s, x) => default(MCompositions<MonoidA, A>).Append(s, singleton(x)));
    }
}
