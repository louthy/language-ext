using static LanguageExt.Prelude;
using LanguageExt.Traits;
using System.Collections.Generic;
using LanguageExt.ClassInstances;
using System.Collections;
using System;

namespace LanguageExt;

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
public struct Compositions<A> : 
    IEquatable<Compositions<A>>, 
    IEnumerable<A>, 
    Monoid<Compositions<A>> 
    where A : Monoid<A>
{
    public static Compositions<A> Empty { get; } = new(Seq<Node>());

    public readonly Seq<Node> Tree;
    int? hashCode;

    internal Compositions(Seq<Node> tree)
    {
        hashCode = null;
        Tree = tree;
    }

    public Compositions<A> Append(Compositions<A> compy)
    {
        var compx = this;
        Seq<Node> go(Seq<Node> mx, Seq<Node> my)
        {
            if (mx.IsEmpty) return my;
            if (my.IsEmpty) return go(mx.Tail, [mx.Head]);

            var x  = mx.Head;
            var sx = mx.Head.Size;
            var cx = mx.Head.Children;
            var vx = mx.Head.Value;
            var xs = mx.Tail;

            var y  = my.Head;
            var sy = my.Head.Size;
            var vy = my.Head.Value;
            var ys = my.Tail;

            var ord = sx.CompareTo(sy);
            if (ord      < 0) return go(xs, x.Cons(my));
            else if (ord > 0)
            {
                var (l, r) = ((Node, Node))cx;
                return go(r.Cons(l.Cons(xs)), my);
            }
            else
            {
                return go(new Node(sx + sy, Some((x, y)), vx.Append(vy)).Cons(xs), ys);
            }
        }
        return new Compositions<A>(go(compx.Tree, compy.Tree));
    }
    
    /// <summary>
    /// Returns true if the given tree is appropriately right-biased.
    /// </summary>
    public bool WellFormed<EqA>() 
        where EqA : Eq<A> =>
        Compositions.wellFormed<EqA, A>(this);

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
    public Compositions<A> Take(int amount) =>
        Compositions.take(amount, this);

    /// <summary>
    /// Returns the composition of the first `k` elements of the compositions list, doing only `O(log k)` compositions.
    /// Faster than simply using `take` and then `composed` separately.
    /// </summary>
    public A TakeComposed(int amount) =>
        Compositions.takeComposed(amount, this);

    /// <summary>
    /// A convenience alias for 'take' and 'skip'
    /// </summary>
    public (Compositions<A> taken, Compositions<A> skipped) SplitAt(int i) =>
        Compositions.splitAt(i, this);

    /// <summary>
    /// Compose every element in the compositions list. Performs only `O(log n)` compositions.
    /// </summary>
    public A Composed() =>
        Compositions.composed(this);

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
    public static Compositions<A> FromList(IEnumerable<A> ma) =>
        Compositions.fromList(ma);

    /// <summary>
    /// Equality operator
    /// </summary>
    public bool Equals(Compositions<A> b) =>
        EqEnumerable<A>.Equals(this, b);

    /// <summary>
    /// Equality operator
    /// </summary>
    public override bool Equals(object? obj) =>
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
        hashCode ?? (int)(hashCode = hash(this));

    /// <summary>
    /// Convert to a sequence
    /// </summary>
    public Seq<A> ToSeq() =>
        FoldCompositions<A>.Fold(this, Seq<A>(), (s, x) => x.Cons(s))(unit);

    /// <summary>
    /// Convert to an enumerable
    /// </summary>
    public IEnumerable<A> AsEnumerable() =>
        FoldCompositions<A>.Fold(this, Seq<A>(), (s, x) => x.Cons(s))(unit);

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
