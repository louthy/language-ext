using static LanguageExt.Prelude;
using LanguageExt.Traits;
using System.Collections.Generic;
using LanguageExt.ClassInstances;

namespace LanguageExt;

public static class Compositions
{
    /// <summary>
    /// Returns true if the given tree is appropriately right-biased.
    /// </summary>
    public static bool wellFormed<EqA, A>(Compositions<A> ca) 
        where EqA : Eq<A>
        where A : Monoid<A> 
    {
        bool wellFormedNode(int n, Compositions<A>.Node node)
        {
            var ni = node.Size;
            if (n == 1                    && ni == 1 && node.Children.IsNone) return true;
            else if (node.Children.IsSome && n  == ni)
            {
                var v = node.Value;
                var (l, r) = ((Compositions<A>.Node, Compositions<A>.Node))node.Children;
                return wellFormedNode(n / 2, l)                          &&
                       EqA.Equals(v, l.Value.Combine(r.Value)) &&
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
            var x  = ma.Head;
            var xs = ma.Tail;
            var s  = x.Size;
            return s >= m && wellFormedNode(s, x) && go(s * 2, xs);
        }
        return go(1, ca.Tree);
    }

    /// <summary>
    /// Return the compositions list with the first `k` elements removed, in `O(log k)` time.
    /// </summary>
    public static Compositions<A> skip<A>(int amount, Compositions<A> compositions)
        where A : Monoid<A>
    {
        Seq<Compositions<A>.Node> go(int n, Seq<Compositions<A>.Node> nodes)
        {
            if (nodes.IsEmpty) return nodes;
            if (n <= 0) return nodes;
            var s   = nodes.Head.Size;
            var c   = nodes.Head.Children;
            var ri  = nodes.Tail;
            var ord = n.CompareTo(s);
            if (ord < 0)
            {
                var (l, r) = ((Compositions<A>.Node, Compositions<A>.Node))c;
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
    public static Compositions<A> take<A>(int amount, Compositions<A> compositions) where A : Monoid<A>
    {
        Seq<Compositions<A>.Node> go(int n, Seq<Compositions<A>.Node> nodes)
        {
            if (nodes.IsEmpty) return nodes;
            if (n <= 0) return Seq<Compositions<A>.Node>();
            var x   = nodes.Head;
            var s   = x.Size;
            var c   = x.Children;
            var ri  = nodes.Tail;
            var ord = n.CompareTo(s);
            if (ord < 0)
            {
                var (l, r) = ((Compositions<A>.Node, Compositions<A>.Node))c;
                return go(n, l.Cons(r.Cons(ri)));
            }
            else
            {
                return new Compositions<A>([x])
                        .Combine(new Compositions<A>(go(n - s, ri)))
                        .Tree;
            }
        }
        return new Compositions<A>(go(amount, compositions.Tree));
    }

    /// <summary>
    /// Returns the composition of the first `k` elements of the compositions list, doing only `O(log k)` compositions.
    /// Faster than simply using `take` and then `composed` separately.
    /// </summary>
    public static A takeComposed<A>(int amount, Compositions<A> compositions) where A : Monoid<A>
    {
        A go(int n, Seq<Compositions<A>.Node> nodes)
        {
            if (nodes.IsEmpty) return A.Empty;
            if (n <= 0) return A.Empty;
            var x   = nodes.Head;
            var s   = x.Size;
            var c   = x.Children;
            var v   = x.Value;
            var ri  = nodes.Tail;
            var ord = n.CompareTo(s);
            if (ord < 0)
            {
                var (l, r) = ((Compositions<A>.Node, Compositions<A>.Node))c;
                return go(n, l.Cons(r.Cons(ri)));
            }
            else
            {
                return v.Combine(go(n - s, ri));
            }
        }
        return go(amount, compositions.Tree);
    }

    /// <summary>
    /// A convenience alias for 'take' and 'drop'
    /// </summary>
    public static (Compositions<A> taken, Compositions<A> skipped) splitAt<A>(int i, Compositions<A> c)
        where A : Monoid<A> =>
        (take(i, c), skip(i, c));

    /// <summary>
    /// Compose every element in the compositions list. Performs only `O(log n)` compositions.
    /// </summary>
    public static A composed<A>(Compositions<A> compositions)
        where A : Monoid<A> =>
        FoldCompositions<A>.Fold(compositions, A.Empty, (x, y) => x.Combine(y))(unit);

    /// <summary>
    /// Construct a compositions list containing just one element.
    /// </summary>
    public static Compositions<A> singleton<A>(A value) 
        where A : Monoid<A> =>
        new ([new Compositions<A>.Node(1, None, value)]);

    /// <summary>
    /// Get the number of elements in the compositions list, in `O(log n)` time.
    /// </summary>
    public static int count<A>(Compositions<A> compositions)
        where A : Monoid<A> =>
        compositions.Tree.Map(n => n.Size).Sum();

    /// <summary>
    /// Add a new element to the front of a compositions list. Performs `O(log n)` element compositions.
    /// </summary>
    public static Compositions<A> cons<A>(A x, Compositions<A> xs)
        where A : Monoid<A> =>
        singleton(x).Combine(xs);

    /// <summary>
    /// Convert a compositions list into a list of elements. The other direction
    /// is provided in the 'Data.Foldable.Foldable' instance.This will perform O(n log n) element compositions.
    /// </summary>
    public static Compositions<A> fromList<A>(IEnumerable<A> ma)
        where A : Monoid<A> =>
        ma.AsEnumerableM().Fold(Compositions<A>.Empty, (s, x) => s.Combine(singleton(x)));
}
