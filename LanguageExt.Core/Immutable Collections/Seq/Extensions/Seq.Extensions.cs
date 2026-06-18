#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type

using System;
using System.Linq;
using LanguageExt.Traits;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using LanguageExt.ClassInstances;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class SeqExtensions
{
    /// <summary>
    /// Monadic join
    /// </summary>
    [Pure]
    public static Seq<A> Flatten<A>(this Seq<Seq<A>> ma) =>
        ma.Bind(identity);

    extension<A>(A head)
    {
        /// <summary>
        /// Construct a sequence from any value
        /// </summary>
        /// <typeparam name="A">Type of the items in the sequence</typeparam>
        /// <returns>Constructed sequence</returns>
        [Pure]
        public Seq<A> Cons() =>
            Seq.FromSingleValue(head);

        /// <summary>
        /// Construct a sequence from any value
        /// </summary>
        /// <typeparam name="A">Type of the items in the sequence</typeparam>
        /// <returns>Constructed sequence</returns>
        [Pure]
        public Seq<A> Cons(UnitCollection _) =>
            Seq.FromSingleValue(head);

        /// <summary>
        /// Construct a list from head and tail; head becomes the first item in 
        /// the list.  
        /// </summary>
        /// <typeparam name="A">Type of the items in the sequence</typeparam>
        /// <param name="tail">Tail of the sequence</param>
        /// <returns>Constructed sequence</returns>
        [Pure]
        public Seq<A> Cons(Seq<A> tail) =>
            tail.Cons(head);

        /// <summary>
        /// Construct a list from head and tail; head becomes the first item in 
        /// the list.  
        /// </summary>
        /// <typeparam name="A">Type of the items in the sequence</typeparam>
        /// <param name="tail">Tail of the sequence</param>
        /// <returns>Constructed sequence</returns>
        [Pure]
        public Seq<A> Cons(A[] tail)
        {
            if (tail.Length == 0)
            {
                return Seq.FromSingleValue(head);
            }
            else
            {
                var data = new A[tail.Length + 1];
                System.Array.Copy(tail, 0, data, 1, tail.Length);
                data[0] = head;
                return Seq.FromArray(data);
            }
        }

        /// <summary>
        /// Construct a list from head and tail; head becomes the first item in 
        /// the list.  
        /// </summary>
        /// <typeparam name="A">Type of the items in the sequence</typeparam>
        /// <param name="tail">Tail of the sequence</param>
        /// <returns>Constructed sequence</returns>
        [Pure]
        public Seq<A> Cons(ReadOnlySpan<A> tail)
        {
            if (tail.Length == 0)
            {
                return Seq.FromSingleValue(head);
            }
            else
            {
                var data = new A[tail.Length + 1];
                tail.CopyTo(data.AsSpan(1));
                data[0] = head;
                return Seq.FromArray(data);
            }
        }

        /// <summary>
        /// Construct a list from head and tail; head becomes the first item in 
        /// the list.  
        /// </summary>
        /// <typeparam name="A">Type of the items in the sequence</typeparam>
        /// <param name="tail">Tail of the sequence</param>
        /// <returns>Constructed sequence</returns>
        [Pure]
        public Seq<A> Cons(IEnumerable<A> tail) =>
            tail is Seq<A> seq
                ? head.Cons(seq)
                : new Seq<A>(tail).Cons(head);

    }

    /// <param name="list">sequence</param>
    /// <typeparam name="A">sequence item type</typeparam>
    extension<A>(K<Seq, A> list)
    {
        /// <summary>
        /// Applies the given function 'selector' to each element of the sequence. Returns a sequence that 
        /// consists of the results for each element where the function returns Some(f(x)).
        /// </summary>
        /// <param name="f">Selector function</param>
        /// <returns>Mapped and filtered sequence</returns>
        [Pure]
        public Seq<B> Choose<B>(Func<A, Option<B>> f) =>
            Seq.choose(+list, f);

        /// <summary>
        /// Applies the given function 'selector' to each element of the sequence. Returns a  sequence that
        /// consists of the results for each element where the function returns Some(f(x)). An index value
        /// is passed through to the selector function also.
        /// </summary>
        /// <param name="f">Selector function</param>
        /// <returns>Mapped and filtered sequence</returns>
        [Pure]
        public Seq<B> Choose<B>(Func<int, A, Option<B>> f) =>
            Seq.choose(+list, f);

        /// <summary>
        /// The tails function returns all final segments of the argument, the longest first.
        ///
        ///     tails(['a','b','c']) == [['a','b','c'], ['b','c'], ['c'], []]
        /// 
        /// </summary>
        /// <returns>Sequence of sequences</returns>
        [Pure]
        public Seq<Seq<A>> Tails() =>
            Seq.tails(+list);

        /// <summary>
        /// Span, applied to a predicate 'pred' and a list, returns a tuple where the first element has the 
        /// longest prefix (possibly empty) of elements that satisfy 'pred' and the second element is the 
        /// remainder of the list:
        /// </summary>
        /// <example>
        /// Seq.span(List(1,2,3,4,1,2,3,4), x => x 〈 3) == (List(1,2),List(3,4,1,2,3,4))
        /// </example>
        /// <example>
        /// Seq.span(List(1,2,3), x => x 〈 9) == (List(1,2,3),List())
        /// </example>
        /// <example>
        /// Seq.span(List(1,2,3), x => x 〈 0) == (List(),List(1,2,3))
        /// </example>
        /// <param name="pred">Predicate</param>
        /// <returns>Split list</returns>
        [Pure]
        public (Seq<A>, Seq<A>) Span(Func<A, bool> pred) =>
            Seq.span(+list, pred);
    }


    /// <param name="list">sequence</param>
    /// <typeparam name="A">sequence item type</typeparam>
    extension<A>(Seq<A> list)
    {
        /// <summary>
        /// Reverses the sequence (Reverse in LINQ)
        /// </summary>
        /// <returns>Reversed sequence</returns>
        [Pure]
        public Seq<A> Reverse() =>
            Seq.rev(list);

        /// <summary>
        /// Joins two sequences together either into a single sequence using the join 
        /// function provided
        /// </summary>
        /// <param name="other">Second sequence to join</param>
        /// <param name="zipper">Join function</param>
        /// <returns>Joined sequence</returns>
        [Pure]
        public Seq<V> Zip<U, V>(Seq<U> other, Func<A, U, V> zipper) =>
            toSeq(Enumerable.Zip(+list, other, zipper));

        /// <summary>
        /// Joins two sequences together either into a sequence of tuples
        /// </summary>
        /// <param name="other">Second sequence to join</param>
        /// <param name="zipper">Join function</param>
        /// <returns>Joined sequence of tuples</returns>
        [Pure]
        public Seq<(A First, U Second)> Zip<U>(Seq<U> other) =>
            toSeq(Enumerable.Zip(list, other, (t, u) => (t, u)));

        /// <summary>
        /// Return a new sequence with all duplicate values removed
        /// </summary>
        /// <returns>A new sequence with all duplicate values removed</returns>
        [Pure]
        public Seq<A> Distinct() =>
            toSeq(Enumerable.Distinct(list));

        /// <summary>
        /// Return a new sequence with all duplicate values removed
        /// </summary>
        /// <returns>A new sequence with all duplicate values removed</returns>
        [Pure]
        public Seq<A> Distinct<K>(Func<A, K> keySelector, Option<Func<K, K, bool>> compare = default) =>
            toSeq(list.Distinct(new EqCompare<A>(
                                    (a, b) => compare.IfNone(EqDefault<K>.Equals)(keySelector(a), keySelector(b)),
                                    a => compare.Match(Some: _ => 0,
                                                       None: () => EqDefault<K>.GetHashCode(keySelector(a))))));

        /// <summary>
        /// Convert to a queryable 
        /// </summary>
        /// <returns></returns>
        [Pure]
        public IQueryable<A> AsQueryable() =>
            // NOTE TO FUTURE ME: Don't delete this thinking it's not needed!
            list.Value.AsQueryable();
    }
    
    
    /// <summary>
    /// Return a new sequence with all duplicate values removed
    /// </summary>
    /// <typeparam name="T">sequence item type</typeparam>
    /// <param name="list">sequence</param>
    /// <returns>A new sequence with all duplicate values removed</returns>
    [Pure]
    public static Seq<T> Distinct<EQ, T>(this Seq<T> list) where EQ : Eq<T> =>
        toSeq(list.Distinct(new EqCompare<T>(static (x, y) => EQ.Equals(x, y), static x => EQ.GetHashCode(x))));
}
