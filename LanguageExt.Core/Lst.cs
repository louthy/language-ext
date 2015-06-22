using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageExt
{
    /// <summary>
    /// Immutable list
    /// </summary>
    /// <remarks>Wraps System.Collections.Immutable.ImmutableList</remarks>
    /// <typeparam name="T">List item type</typeparam>
    public class Lst<T> : IImmutableList<T>
    {
        IImmutableList<T> lst;

        internal Lst()
        {
            lst = ImmutableList.Create<T>();
        }

        internal Lst(IEnumerable<T> items)
        {
            lst = ImmutableList.CreateRange(items);
        }

        internal Lst(IImmutableList<T> wrapped)
        {
            lst = wrapped;
        }

        public T this[int index]
        {
            get
            {
                return lst[index];
            }
        }

        public int Count
        {
            get
            {
                return lst.Count;
            }
        }

        int IReadOnlyCollection<T>.Count
        {
            get
            {
                return Count;
            }
        }

        T IReadOnlyList<T>.this[int index]
        {
            get
            {
                return this[index];
            }
        }

        private Lst<T> Wrap(IImmutableList<T> lst)
        {
            return new Lst<T>(lst);
        }

        public Lst<T> Add(T value)
        {
            if (value == null) throw new ArgumentNullException("'value' cannot be null.");
            return Wrap(lst.Add(value));
        }

        public Lst<T> AddRange(IEnumerable<T> items)
        {
            if (items == null) return this;
            if (items.Any(x => x == null)) throw new ArgumentNullException("'items' cannot be null.");
            return Wrap(lst.AddRange(items));
        }

        public Lst<T> Clear()
        {
            return Wrap(lst.Clear());
        }

        public IEnumerator<T> GetEnumerator()
        {
            return lst.GetEnumerator();
        }

        public int IndexOf(T item, int index, int count, IEqualityComparer<T> equalityComparer)
        {
            return lst.IndexOf(item, index, count, equalityComparer);
        }

        public Lst<T> Insert(int index, T element)
        {
            if (element == null) throw new ArgumentNullException("'element' cannot be null.");
            return Wrap(lst.Insert(index,element));
        }

        public Lst<T> InsertRange(int index, IEnumerable<T> items)
        {
            if (items == null) return this;
            if (items.Any(x => x == null)) throw new ArgumentNullException("'items' cannot be null.");
            return Wrap(lst.InsertRange(index,items));
        }

        public int LastIndexOf(T item, int index, int count, IEqualityComparer<T> equalityComparer)
        {
            return lst.LastIndexOf(item, index, count, equalityComparer);
        }

        public Lst<T> Remove(T value)
        {
            return Wrap(lst.Remove(value));
        }

        public Lst<T> Remove(T value, IEqualityComparer<T> equalityComparer)
        {
            return Wrap(lst.Remove(value,equalityComparer));
        }

        public Lst<T> RemoveAll(Predicate<T> match)
        {
            return Wrap(lst.RemoveAll(match));
        }

        public Lst<T> RemoveAt(int index)
        {
            return Wrap(lst.RemoveAt(index));
        }

        public Lst<T> RemoveRange(int index, int count)
        {
            return Wrap(lst.RemoveRange(index, count));
        }

        public Lst<T> RemoveRange(Lst<T> items, IEqualityComparer<T> equalityComparer)
        {
            return Wrap(lst.RemoveRange(items, equalityComparer));
        }

        public Lst<T> RemoveRange(IEnumerable<T> items, IEqualityComparer<T> equalityComparer)
        {
            return Wrap(lst.RemoveRange(items,equalityComparer));
        }

        public Lst<T> Replace(T oldValue, T newValue, IEqualityComparer<T> equalityComparer)
        {
            if (oldValue == null) return this;
            if (newValue == null) throw new ArgumentNullException("'newValue' cannot be null.");
            return Wrap(lst.Replace(oldValue,newValue,equalityComparer));
        }

        public Lst<T> SetItem(int index, T value)
        {
            if (value == null) throw new ArgumentNullException("'value' cannot be null.");
            return Wrap(lst.SetItem(index, value));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return lst.GetEnumerator();
        }

        IImmutableList<T> IImmutableList<T>.Clear()
        {
            return lst.Clear();
        }

        int IImmutableList<T>.IndexOf(T item, int index, int count, IEqualityComparer<T> equalityComparer)
        {
            return lst.IndexOf(item,index,count,equalityComparer);
        }

        int IImmutableList<T>.LastIndexOf(T item, int index, int count, IEqualityComparer<T> equalityComparer)
        {
            return lst.LastIndexOf(item, index, count, equalityComparer);
        }

        IImmutableList<T> IImmutableList<T>.Add(T value)
        {
            if (value == null) throw new ArgumentNullException("'value' cannot be null.");
            return lst.Add(value);
        }

        IImmutableList<T> IImmutableList<T>.AddRange(IEnumerable<T> items)
        {
            if (items == null) return this;
            if (items.Any(x => x == null)) throw new ArgumentNullException("'items' cannot be null.");
            return lst.AddRange(items);
        }

        IImmutableList<T> IImmutableList<T>.Insert(int index, T element)
        {
            if (element == null) throw new ArgumentNullException("'element' cannot be null.");
            return lst.Insert(index,element);
        }

        IImmutableList<T> IImmutableList<T>.InsertRange(int index, IEnumerable<T> items)
        {
            if (items == null) return this;
            if (items.Any(x => x == null)) throw new ArgumentNullException("'items' cannot be null.");
            return lst.InsertRange(index,items);
        }

        IImmutableList<T> IImmutableList<T>.Remove(T value, IEqualityComparer<T> equalityComparer)
        {
            return lst.Remove(value,equalityComparer);
        }

        IImmutableList<T> IImmutableList<T>.RemoveAll(Predicate<T> match)
        {
            return lst.RemoveAll(match);
        }

        IImmutableList<T> IImmutableList<T>.RemoveRange(IEnumerable<T> items, IEqualityComparer<T> equalityComparer)
        {
            return lst.RemoveRange(items,equalityComparer);
        }

        IImmutableList<T> IImmutableList<T>.RemoveRange(int index, int count)
        {
            return lst.RemoveRange(index,count);
        }

        IImmutableList<T> IImmutableList<T>.RemoveAt(int index)
        {
            return lst.RemoveAt(index);
        }

        IImmutableList<T> IImmutableList<T>.SetItem(int index, T value)
        {
            if (value == null) throw new ArgumentNullException("'value' cannot be null.");
            return lst.SetItem(index, value);
        }

        IImmutableList<T> IImmutableList<T>.Replace(T oldValue, T newValue, IEqualityComparer<T> equalityComparer)
        {
            if (oldValue == null) return this;
            if (newValue == null) throw new ArgumentNullException("'newValue' cannot be null.");
            return lst.Replace(oldValue, newValue, equalityComparer);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return lst.GetEnumerator();
        }


        public S Fold<S>(S state, Func<S, T, S> folder) =>
            this.Aggregate(state, folder);

        public Lst<U> Map<U>(Func<T, U> map) =>
            new Lst<U>(this.Select(map));

        public Lst<U> Bind<U>(Func<T, Lst<U>> bind) =>
            new Lst<U>(this.SelectMany(bind));

        public Lst<T> Filter(Func<T,bool> pred) =>
            new Lst<T>(this.Where(pred));
    }

    public static class LstExt
    {
        public static Lst<V> SelectMany<T, U, V>(this Lst<T> self, Func<T, Lst<U>> bind, Func<T, U, V> project)
        {
            return new Lst<V>(self.AsEnumerable().SelectMany(bind, project));
        }

        public static int Count<T>(this Lst<T> self) =>
            self.Count;
    }
}
