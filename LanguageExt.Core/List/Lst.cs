using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;
using System.ComponentModel;
using LanguageExt;
using LanguageExt.TypeClasses;
using static LanguageExt.TypeClass;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Immutable list
    /// </summary>
    /// <typeparam name="A">Value type</typeparam>
#if !COREFX
    [Serializable]
#endif
    public struct Lst<A> : 
        IEnumerable<A>, 
        IReadOnlyList<A>,
        IReadOnlyCollection<A>,
        Monoid<Lst<A>>,
        Difference<Lst<A>>,
        MonadPlus<A>,
        Foldable<A>,
        Eq<Lst<A>>
    {
        /// <summary>
        /// Empty list
        /// </summary>
        public static readonly Lst<A> Empty = new Lst<A>(new A[0] { });

        readonly LstInternal<A> value;

        internal LstInternal<A> Value => value ?? LstInternal<A>.Empty;

        /// <summary>
        /// Ctor
        /// </summary>
        internal Lst(IEnumerable<A> initial)
        {
            value = new LstInternal<A>(initial);
        }

        /// <summary>
        /// Ctor
        /// </summary>
        internal Lst(ListItem<A> root, bool rev)
        {
            value = new LstInternal<A>(root, rev);
        }

        private ListItem<A> Root
        {
            get
            {
                return Value.Root ?? ListItem<A>.Empty;
            }
            //set
            //{
            //    root = Root;
            //}
        }

        /// <summary>
        /// Index accessor
        /// </summary>
        [Pure]
        public A this[int index]
        {
            get
            {
                if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();
                return ListModule.GetItem(Root, Value.Rev ? Count - index - 1 : index);
            }
        }

        /// <summary>
        /// Number of items in the list
        /// </summary>
        [Pure]
        public int Count =>
            Root.Count;

        [Pure]
        int IReadOnlyCollection<A>.Count =>
            Count;

        [Pure]
        A IReadOnlyList<A>.this[int index]
        {
            get
            {
                if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();
                return ListModule.GetItem(Root, Rev ? Count - index - 1 : index);
            }
        }

        internal bool Rev => Value.Rev;

        /// <summary>
        /// Add an item to the end of the list
        /// </summary>
        [Pure]
        public Lst<A> Add(A value) =>
            Value.Add(value);

        /// <summary>
        /// Add a range of items to the end of the list
        /// </summary>
        [Pure]
        public Lst<A> AddRange(IEnumerable<A> items) =>
            Value.AddRange(items);

        /// <summary>
        /// Clear the list
        /// </summary>
        [Pure]
        public Lst<A> Clear() =>
            Empty;

        /// <summary>
        /// Get enumerator
        /// </summary>
        [Pure]
        public IEnumerator<A> GetEnumerator() =>
            new ListModule.ListEnumerator<A>(Root,Rev,0);

        /// <summary>
        /// Find the index of an item
        /// </summary>
        [Pure]
        public int IndexOf(A item, int index = 0, int count = -1, IEqualityComparer<A> equalityComparer = null) =>
            Value.IndexOf(item, index, count, equalityComparer);

        /// <summary>
        /// Insert value at specified index
        /// </summary>
        [Pure]
        public Lst<A> Insert(int index, A value) =>
            Value.Insert(index, value);

        /// <summary>
        /// Insert range of values at specified index
        /// </summary>
        [Pure]
        public Lst<A> InsertRange(int index, IEnumerable<A> items) =>
            Value.InsertRange(index, items);

        /// <summary>
        /// Find the last index of an item in the list
        /// </summary>
        [Pure]
        public int LastIndexOf(A item, int index = 0, int count = -1, IEqualityComparer<A> equalityComparer = null) =>
            Value.LastIndexOf(item, index, count, equalityComparer);

        /// <summary>
        /// Remove an item from the list
        /// </summary>
        [Pure]
        public Lst<A> Remove(A value) =>
            Value.Remove(value);

        /// <summary>
        /// Remove an item from the list
        /// </summary>
        [Pure]
        public Lst<A> Remove(A value, IComparer<A> equalityComparer) =>
            Value.Remove(value, equalityComparer);

        /// <summary>
        /// Remove all items that match a predicate
        /// </summary>
        [Pure]
        public Lst<A> RemoveAll(Predicate<A> pred) =>
            Value.RemoveAll(pred);

        /// <summary>
        /// Remove item at location
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        [Pure]
        public Lst<A> RemoveAt(int index) =>
            Value.RemoveAt(index);

        /// <summary>
        /// Remove a range of items
        /// </summary>
        [Pure]
        public Lst<A> RemoveRange(int index, int count) =>
            Value.RemoveRange(index, count);

        /// <summary>
        /// Set an item at the specified index
        /// </summary>
        [Pure]
        public Lst<A> SetItem(int index, A value) =>
            Value.SetItem(index, value);

        [Pure]
        IEnumerator IEnumerable.GetEnumerator() =>
            new ListModule.ListEnumerator<A>(Root, Rev, 0);

        [Pure]
        IEnumerator<A> IEnumerable<A>.GetEnumerator() =>
            new ListModule.ListEnumerator<A>(Root, Rev, 0);

        [Pure]
        public IEnumerable<A> Skip(int amount) =>
            Value.Skip(amount);

        /// <summary>
        /// Reverse the order of the items in the list
        /// </summary>
        [Pure]
        public Lst<A> Reverse() =>
            Value.Reverse();

        /// <summary>
        /// Fold
        /// </summary>
        [Pure]
        public S Fold<S>(S state, Func<S, A, S> folder) =>
            Value.Fold(state, folder);

        /// <summary>
        /// Map
        /// </summary>
        [Pure]
        public Lst<U> Map<U>(Func<A, U> map) =>
            Value.Map(map);

        /// <summary>
        /// Filter
        /// </summary>
        [Pure]
        public Lst<A> Filter(Func<A, bool> pred) =>
            Value.Filter(pred);

        [Pure]
        public static Lst<A> operator +(Lst<A> lhs, A rhs) =>
            lhs.Add(rhs);

        [Pure]
        public static Lst<A> operator +(A rhs, Lst<A> lhs) =>
            rhs.Cons(lhs);

        [Pure]
        public static Lst<A> operator +(Lst<A> lhs, Lst<A> rhs) =>
            lhs.Append(rhs);

        [Pure]
        public Lst<A> Append(Lst<A> rhs) =>
            Value.Append(rhs);

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Lst<A> Append(Lst<A> lhs, Lst<A> rhs) =>
            lhs.Append(rhs);

        [Pure]
        public static Lst<A> operator -(Lst<A> lhs, Lst<A> rhs) =>
            lhs.Difference(rhs);

        [Pure]
        public Lst<A> Difference(Lst<A> rhs) =>
            Value.Difference(rhs);

        [Pure]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Lst<A> Difference(Lst<A> lhs, Lst<A> rhs) =>
            lhs.Difference(rhs);

        [Pure]
        public override bool Equals(object obj) =>
            !ReferenceEquals(obj,null) && 
            obj is Lst<A> && 
            Value.Equals(((Lst<A>)obj).Value);

        /// <summary>
        /// Get the hash code
        /// Lazily (and once only) calculates the hash from the elements in the list
        /// Empty list hash == 0
        /// </summary>
        [Pure]
        public override int GetHashCode() =>
            Value.GetHashCode();

        [Pure]
        public bool Equals(Lst<A> other) =>
            Value.Equals(other.Value);

        Lst<A> AsList(Foldable<A> f)    => (Lst<A>)f;
        Lst<A> AsList(Functor<A> f)     => (Lst<A>)f;
        Lst<A> AsList(Monad<A> f)       => (Lst<A>)f;
        Lst<A> AsList(Monoid<Lst<A>> f) => (Lst<A>)f;
        Lst<A> AsList(MonadPlus<A> f)    => (Lst<A>)f;

        public S Fold<S>(Foldable<A> fa, S state, Func<S, A, S> f)
        {
            foreach (var item in AsList(fa).AsEnumerable())
            {
                state = f(state, item);
            }
            return state;
        }

        public S FoldBack<S>(Foldable<A> fa, S state, Func<S, A, S> f)
        {
            foreach (var item in AsList(fa).AsEnumerable().Reverse())
            {
                state = f(state, item);
            }
            return state;
        }

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="x">The bound monad value</param>
        /// <returns>Monad of A</returns>
        public Monad<A> Return(A x, params A[] xs) =>
            List.createRange(x.Cons(xs));

        /// <summary>
        /// Monad return
        /// </summary>
        /// <typeparam name="A">Type of the bound monad value</typeparam>
        /// <param name="x">The bound monad value(s)</param>
        /// <returns>Monad of A</returns>
        public Monad<A> Return(IEnumerable<A> xs) =>
            List.createRange(xs);

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="B">Type of the bound return value</typeparam>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Bind function</param>
        /// <returns>Monad of B</returns>
        public Monad<B> Bind<B>(Monad<A> ma, Func<A, Monad<B>> f) =>
            List.createRange(BindSeq(ma, f));

        /// <summary>
        /// Monadic bind
        /// </summary>
        /// <typeparam name="B">Type of the bound return value</typeparam>
        /// <param name="ma">Monad to bind</param>
        /// <param name="f">Bind function</param>
        /// <returns>Monad of B</returns>
        public MB Bind<MB, B>(Monad<A> ma, Func<A, MB> f) where MB : struct, Monad<B> =>
            Return<MB, B>(BindSeq<MB, B>(ma, f));

        /// <summary>
        /// Produce a failure value
        /// </summary>
        public Monad<A> Fail(Exception err = null) =>
            List.empty<A>();

        public Functor<B> Map<B>(Functor<A> fa, Func<A, B> f) =>
            List.createRange(List.map(AsList(fa), f));

        Lst<A> Monoid<Lst<A>>.Empty() => List.empty<A>();

        [Pure]
        public static bool operator ==(Lst<A> lhs, Lst<A> rhs) =>
            lhs.Value.Equals(rhs.Value);

        [Pure]
        public static bool operator !=(Lst<A> lhs, Lst<A> rhs) =>
            lhs.Value.Equals(rhs.Value);

        IEnumerable<B> BindSeq<MB, B>(Monad<A> ma, Func<A, MB> f) where MB : struct, Monad<B>
        {
            var xs = AsList(ma);
            foreach (var x in xs)
            {
                var b = f(x);
                foreach (var y in TypeClass.toSeq(b as Foldable<B>))
                {
                    yield return y;
                }
            }
        }

        IEnumerable<B> BindSeq<B>(Monad<A> ma, Func<A, Monad<B>> f)
        {
            var xs = AsList(ma);
            foreach (var x in xs)
            {
                var b = f(x);
                foreach (var y in TypeClass.toSeq(b as Foldable<B>))
                {
                    yield return y;
                }
            }
        }

        public MonadPlus<A> Plus(MonadPlus<A> a, MonadPlus<A> b) =>
            AsList(a) + AsList(b);

        public MonadPlus<A> Zero() =>
            Empty;

        public bool Equals(Lst<A> a, Lst<A> b) =>
            a == b;
    }
}