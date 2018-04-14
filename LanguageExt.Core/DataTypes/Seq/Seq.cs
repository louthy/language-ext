using LanguageExt.ClassInstances;
using LanguageExt.TypeClasses;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Cons sequence
    /// Represents a sequence of values in a similar way to IEnumerable, but without the
    /// issues of multiple evaluation for key LINQ operators like Skip, Count, etc.
    /// </summary>
    /// <typeparam name="A">Type of the values in the sequence</typeparam>
    public abstract class Seq<A> : ISeq<A>
    {
        public static readonly Seq<A> Empty = SeqEmpty<A>.Default;

        /// <summary>
        /// Head item in the sequence.  NOTE:  If `IsEmpty` is true then Head 
        /// is undefined.  Call HeadOrNone() if for maximum safety.
        /// </summary>
        public abstract A Head { get;  }

        /// <summary>
        /// Tail of the sequence
        /// </summary>
        public abstract Seq<A> Tail { get;  }

        /// <summary>
        /// Head of the sequence if this node isn't the empty node
        /// </summary>
        public Option<A> HeadOrNone() =>
            IsEmpty
                ? None
                : Some(Head);

        /// <summary>
        /// True if this cons node is the Empty node
        /// </summary>
        public abstract bool IsEmpty { get; }

        /// <summary>
        /// Returns the number of items in the sequence
        /// </summary>
        /// <returns>Number of items in the sequence</returns>
        public abstract int Count { get; }

        /// <summary>
        /// Get an enumerator for the sequence
        /// </summary>
        /// <returns>An IEnumerator of As</returns>
        public virtual IEnumerator<A> GetEnumerator() =>
            AsEnumerable().GetEnumerator();

        /// <summary>
        /// Get an enumerator for the sequence
        /// </summary>
        /// <returns>An IEnumerator of As</returns>
        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        /// <summary>
        /// Stream as an enumerable
        /// </summary>
        public virtual IEnumerable<A> AsEnumerable()
        {
            var current = this;
            while (!current.IsEmpty)
            {
                yield return current.Head;
                current = current.Tail;
            }
        }

        /// <summary>
        /// Implicit conversion operator from SeqEmpty
        /// </summary>
        public static implicit operator Seq<A>(SeqEmpty empty) =>
            Empty;

        /// <summary>
        /// Match empty sequence, or multi-item sequence
        /// </summary>
        /// <typeparam name="B">Return value type</typeparam>
        /// <param name="Empty">Match for an empty list</param>
        /// <param name="Tail">Match for a non-empty</param>
        /// <returns>Result of match function invoked</returns>
        public virtual B Match<B>(
            Func<B> Empty,
            Func<A, Seq<A>, B> Tail) =>
            IsEmpty
                ? Empty()
                : Tail(this.Head, this.Tail);

        /// <summary>
        /// Match empty sequence, or one item sequence, or multi-item sequence
        /// </summary>
        /// <typeparam name="B">Return value type</typeparam>
        /// <param name="Empty">Match for an empty list</param>
        /// <param name="Tail">Match for a non-empty</param>
        /// <returns>Result of match function invoked</returns>
        public virtual B Match<B>(
            Func<B> Empty,
            Func<A, B> Head,
            Func<A, Seq<A>, B> Tail) =>
            IsEmpty
                ? Empty()
                : this.Tail.IsEmpty
                    ? Head(this.Head)
                    : Tail(this.Head, this.Tail);

        /// <summary>
        /// Match empty sequence, or multi-item sequence
        /// </summary>
        /// <typeparam name="B">Return value type</typeparam>
        /// <param name="Empty">Match for an empty list</param>
        /// <param name="Sequence">Match for a non-empty</param>
        /// <returns>Result of match function invoked</returns>
        public virtual B Match<B>(
            Func<B> Empty,
            Func<Seq<A>, B> Seq) =>
            IsEmpty
                ? Empty()
                : Seq(this);

        /// <summary>
        /// Match empty sequence, or one item sequence, or multi-item sequence
        /// </summary>
        /// <typeparam name="B">Return value type</typeparam>
        /// <param name="Empty">Match for an empty list</param>
        /// <param name="Tail">Match for a non-empty</param>
        /// <returns>Result of match function invoked</returns>
        public virtual B Match<B>(
            Func<B> Empty,
            Func<A, B> Head,
            Func<Seq<A>, B> Tail) =>
            IsEmpty
                ? Empty()
                : this.Tail.IsEmpty
                    ? Head(this.Head)
                    : Tail(this.Tail);

        /// <summary>
        /// Map the sequence using the function provided
        /// </summary>
        /// <typeparam name="B"></typeparam>
        /// <param name="f">Mapping function</param>
        /// <returns>Mapped sequence</returns>
        public Seq<B> Map<B>(Func<A, B> f)
        {
            IEnumerable<B> Yield()
            {
                foreach(var item in this)
                {
                    yield return f(item);
                }
            }
            return SeqEnumerable<B>.New(Yield());
        }

        /// <summary>
        /// Map the sequence using the function provided
        /// </summary>
        /// <typeparam name="B"></typeparam>
        /// <param name="f">Mapping function</param>
        /// <returns>Mapped sequence</returns>
        public Seq<B> Select<B>(Func<A, B> f) => 
            Map(f);

        /// <summary>
        /// Monadic bind (flatmap) of the sequence
        /// </summary>
        /// <typeparam name="B">Bound return value type</typeparam>
        /// <param name="f">Bind function</param>
        /// <returns>Flatmapped sequence</returns>
        public Seq<B> Bind<B>(Func<A, Seq<B>> f)
        {
            IEnumerable<B> Yield()
            {
                foreach(var a in this)
                {
                    foreach(var b in f(a))
                    {
                        yield return b;
                    }
                }
            }
            return SeqEnumerable<B>.New(Yield());
        }

        /// <summary>
        /// Monadic bind (flatmap) of the sequence
        /// </summary>
        /// <typeparam name="B">Bound return value type</typeparam>
        /// <param name="bind">Bind function</param>
        /// <returns>Flatmapped sequence</returns>
        public Seq<C> SelectMany<B, C>(Func<A, Seq<B>> bind, Func<A, B, C> project)
        {
            IEnumerable<C> Yield()
            {
                foreach (var a in this)
                {
                    foreach (var b in bind(a))
                    {
                        yield return project(a, b);
                    }
                }
            }
            return SeqEnumerable<C>.New(Yield());
        }

        /// <summary>
        /// Filter the items in the sequence
        /// </summary>
        /// <param name="f">Predicate to apply to the items</param>
        /// <returns>Filtered sequence</returns>
        public Seq<A> Filter(Func<A, bool> f)
        {
            IEnumerable<A> Yield()
            {
                foreach (var item in this)
                {
                    if(f(item)) yield return item;
                }
            }
            return SeqEnumerable<A>.New(Yield());
        }

        /// <summary>
        /// Filter the items in the sequence
        /// </summary>
        /// <param name="f">Predicate to apply to the items</param>
        /// <returns>Filtered sequence</returns>
        public Seq<A> Where(Func<A, bool> f) => 
            Filter(f);

        /// <summary>
        /// Fold the sequence from the first item to the last
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="f">Fold function</param>
        /// <returns>Aggregated state</returns>
        public abstract S Fold<S>(S state, Func<S, A, S> f);

        /// <summary>
        /// Fold the sequence from the last item to the first.  For 
        /// sequences that are not lazy and are less than 5000 items
        /// long, FoldBackRec is called instead, because it is faster.
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="f">Fold function</param>
        /// <returns>Aggregated state</returns>
        public abstract S FoldBack<S>(S state, Func<S, A, S> f);

        /// <summary>
        /// Returns true if the supplied predicate returns true for any
        /// item in the sequence.  False otherwise.
        /// </summary>
        /// <param name="f">Predicate to apply</param>
        /// <returns>True if the supplied predicate returns true for any
        /// item in the sequence.  False otherwise.</returns>
        public abstract bool Exists(Func<A, bool> f);

        /// <summary>
        /// Returns true if the supplied predicate returns true for all
        /// items in the sequence.  False otherwise.  If there is an 
        /// empty sequence then true is returned.
        /// </summary>
        /// <param name="f">Predicate to apply</param>
        /// <returns>True if the supplied predicate returns true for all
        /// items in the sequence.  False otherwise.  If there is an 
        /// empty sequence then true is returned.</returns>
        public abstract bool ForAll(Func<A, bool> f);

        /// <summary>
        /// Returns true if the sequence has items in it
        /// </summary>
        /// <returns>True if the sequence has items in it</returns>
        public bool Any() => 
            !IsEmpty;

        /// <summary>
        /// Get the hash code for all of the items in the sequence, or 0 if empty
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() =>
            IsEmpty
                ? 0
                : Fold(
                    Head.IsNull() ? 0 : Head.GetHashCode(), 
                    (s, x) => s ^ (x.IsNull() ? 0 : x.GetHashCode()));

        /// <summary>
        /// Ordering operator
        /// </summary>
        public static bool operator >(Seq<A> x, Seq<A> y) =>
            default(OrdSeq<OrdDefault<A>, A>).Compare(x, y) > 0;

        /// <summary>
        /// Ordering operator
        /// </summary>
        public static bool operator >=(Seq<A> x, Seq<A> y) =>
            default(OrdSeq<OrdDefault<A>, A>).Compare(x, y) >= 0;

        /// <summary>
        /// Ordering  operator
        /// </summary>
        public static bool operator <(Seq<A> x, Seq<A> y) =>
            default(OrdSeq<OrdDefault<A>, A>).Compare(x, y) < 0;

        /// <summary>
        /// Ordering  operator
        /// </summary>
        public static bool operator <=(Seq<A> x, Seq<A> y) =>
            default(OrdSeq<OrdDefault<A>, A>).Compare(x, y) <= 0;

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(Seq<A> x, Seq<A> y) =>
            default(EqSeq<EqDefault<A>, A>).Equals(x, y);

        /// <summary>
        /// Non-equality operator
        /// </summary>
        public static bool operator !=(Seq<A> x, Seq<A> y) =>
            !(x==y);

        /// <summary>
        /// Equality test
        /// </summary>
        public override bool Equals(object obj) =>
            obj is ISeq<A> x
                ? Equals(x)
                : false;

        /// <summary>
        /// Equality test
        /// </summary>
        public virtual bool Equals(ISeq<A> rhs) =>
            default(EqSeq<EqDefault<A>, A>).Equals(this, rhs);

        /// <summary>
        /// Skip count items
        /// </summary>
        public abstract Seq<A> Skip(int count);

        /// <summary>
        /// Take count items
        /// </summary>
        public abstract Seq<A> Take(int count);

        /// <summary>
        /// Iterate the sequence, yielding items if they match the predicate 
        /// provided, and stopping as soon as one doesn't
        /// </summary>
        /// <returns>A new sequence with the first items that match the 
        /// predicate</returns>
        public abstract Seq<A> TakeWhile(Func<A, bool> pred);

        /// <summary>
        /// Iterate the sequence, yielding items if they match the predicate 
        /// provided, and stopping as soon as one doesn't.  An index value is 
        /// also provided to the predicate function.
        /// </summary>
        /// <returns>A new sequence with the first items that match the 
        /// predicate</returns>
        public abstract Seq<A> TakeWhile(Func<A, int, bool> pred);

        /// <summary>
        /// Compare to another sequence
        /// </summary>
        public int CompareTo(ISeq<A> other) =>
            default(OrdSeq<OrdDefault<A>, A>).Compare(this, other);

        protected int GetCount()
        {
            int count = 0;
            foreach (var item in this)
            {
                count++;
            }
            return count;
        }

        internal abstract bool IsTerminator { get; }
    }
}
