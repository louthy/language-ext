using System;
using System.Linq;
using LanguageExt.Traits;
using System.Collections;
using System.Collections.Generic;
using static LanguageExt.Prelude;
using LanguageExt.ClassInstances;

namespace LanguageExt;

/// <summary>
/// Immutable list
/// </summary>
/// <typeparam name="A">Value type</typeparam>
[Serializable]
class LstInternal<A> : 
    IEnumerable<A>,
    IEquatable<LstInternal<A>>
{
    /// <summary>
    /// Empty list
    /// </summary>
    public static readonly LstInternal<A> Empty = new ();

    internal ListItem<A> root;
    internal int hashCode;

    internal LstInternal(IEnumerable<A> items)
    {
        hashCode = 0;
        if (items is Lst<A> lst)
        {
            root = lst.Value.Root;
        }
        else
        {
            root = ListItem<A>.EmptyM;
            root = ListModuleM.InsertMany(root, items, 0);
        }
    }

    internal LstInternal(Iterator<A> items)
    {
        hashCode = 0;
        root = ListItem<A>.EmptyM;
        root = ListModuleM.InsertMany(root, items, 0);
    }

    internal LstInternal(ReadOnlySpan<A> items)
    {
        hashCode = 0;
        root = ListItem<A>.EmptyM;
        root = ListModuleM.InsertMany(root, items, 0);
    }

    internal static LstInternal<A> Wrap(ListItem<A> list) =>
        new (list);
    
    internal LstInternal()
    {
        hashCode = 0;
        root = ListItem<A>.Empty;
    }

    internal LstInternal(ListItem<A> root)
    {
        hashCode = 0;
        this.root = root;
    }

    public static LstInternal<A> FromFoldable<T, FS>(K<T, A> items)
        where T : Foldable<T, FS>
        where FS : allows ref struct =>
        Wrap(ListModuleM.BuildSubTree<T, FS, A>(items));

    public static LstInternal<A> FromFoldableBack<T, FS>(K<T, A> items)
        where T : FoldableBack<T, FS>
        where FS : allows ref struct =>
        Wrap(ListModuleM.BuildSubTreeBack<T, FS, A>(items));

    internal ListItem<A> Root =>
        root;

    public A this[long index]
    {
        get
        {
            if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();
            return ListModule.GetItem(Root, index);
        }
    }

    public long Count =>
        Root.Count;

    public LstInternal<A> Add(A value) =>
        Wrap(ListModule.Add(Root, value));

    public LstInternal<A> AddRange(IEnumerable<A> items)
    {
        if (Count == 0) return new LstInternal<A>(items);
        return Wrap(ListModule.AddRange(Root, items));
    }

    public ListEnumerator<A> GetEnumerator() =>
        new (Root, 0);

    public ListEnumeratorBack<A> GetEnumeratorBack() =>
        new (Root, 0);

    public long IndexOf(A item, long index = 0, long count = -1, IEqualityComparer<A>? equalityComparer = null)
    {
        count = count == -1
                    ? Count
                    : count;

        equalityComparer ??= EqualityComparer<A>.Default;

        if (count == 0) return -1;
        if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();

        foreach (var x in Skip(index))
        {
            if (equalityComparer.Equals(x, item))
            {
                return index;
            }
            index++;
            count--;
            if (count == 0) return -1;
        }
        return -1;
    }

    public LstInternal<A> Insert(long index, A value)
    {
        if (index < 0 || index > Root.Count) throw new IndexOutOfRangeException();
        return Wrap(ListModule.Insert(Root, value, index));
    }

    public LstInternal<A> InsertRange(long index, IEnumerable<A> items)
    {
        if (index < 0 || index > Root.Count) throw new IndexOutOfRangeException();
        return Wrap(ListModule.InsertMany(Root, items, index));
    }

    public long LastIndexOf(A item, long index = 0, long count = -1, IEqualityComparer<A>? equalityComparer = null) =>
        // TODO: Use FoldSteps
        Count - Reverse().IndexOf(item, index, count, equalityComparer) - 1;

    public LstInternal<A> Remove(A value) => 
        Remove(value, EqualityComparer<A>.Default);

    public LstInternal<A> Remove(A value, IEqualityComparer<A> equalityComparer) =>
        Wrap(ListModule.Remove(Root, value, equalityComparer));

    public LstInternal<A> RemoveAll(Func<A, bool> pred) =>
        Wrap(ListModule.Remove(Root, pred));

    public LstInternal<A> RemoveAt(long index)
    {
        if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();
        return Wrap(ListModule.Remove(Root, index));
    }

    public LstInternal<A> RemoveRange(long index, long count)
    {
        if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();
        if (index + count > Root.Count) throw new IndexOutOfRangeException();

        var self = this;
        for (; count > 0; count--)
        {
            self = self.RemoveAt(index);
        }
        return self;
    }

    public LstInternal<A> SetItem(long index, A value)
    {
        if (isnull(value)) throw new ArgumentNullException(nameof(value));
        if (index < 0 || index >= Root.Count) throw new IndexOutOfRangeException();
        return new LstInternal<A>(ListModule.SetItem(Root, value, index));
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        new ListEnumerator<A>(Root, 0);

    IEnumerator<A> IEnumerable<A>.GetEnumerator() =>
        new ListEnumerator<A>(Root, 0);

    public Lst<A> Skip(long amount)
    {
        return new Lst<A>(Go());
        IEnumerable<A> Go()
        {
            var iter = new ListEnumerator<A>(Root, amount);
            while (iter.MoveNext())
            {
                yield return iter.Current;
            }
        }
    }

    public LstInternal<A> Reverse() =>
        new (this.AsEnumerable().Reverse());

    public Lst<B> Map<B>(Func<A, B> map) =>
        new (this.AsEnumerable().Select(map));

    public Iterable<A> FindRange(long index, long count)
    {
        if (index < 0 || index >= Count || count < 0) throw new ArgumentOutOfRangeException(nameof(index));
        return Iterable.createRange(Go());

        IEnumerable<A> Go()
        {
            var iter = new ListEnumerator<A>(Root, index, count);
            while (iter.MoveNext())
            {
                yield return iter.Current;
            }
        }
    }

   public Lst<A> Filter(Func<A, bool> pred)
    {
        IEnumerable<A> Yield()
        {
            foreach (var item in this)
            {
                if (pred(item))
                {
                    yield return item;
                }
            }
        }
        return new Lst<A>(Yield());
    }

    public static LstInternal<A> operator +(LstInternal<A> lhs, A rhs) =>
        lhs.Add(rhs);

    public static LstInternal<A> operator +(A rhs, LstInternal<A> lhs) =>
        new (rhs.Cons(lhs));

    public static LstInternal<A> operator +(LstInternal<A> lhs, LstInternal<A> rhs) =>
        lhs.Combine(rhs);

    public LstInternal<A> Combine(LstInternal<A> rhs) =>
        AddRange(rhs);

    public static LstInternal<A> operator -(LstInternal<A> lhs, LstInternal<A> rhs) =>
        lhs.Subtract(rhs);

    public LstInternal<A> Subtract(LstInternal<A> rhs)
    {
        var self = this;
        foreach (var item in rhs)
        {
            self = self.Remove(item);
        }
        return self;
    }

    public override bool Equals(object? obj) =>
        obj is LstInternal<A> @as &&
        Equals(@as);

    public override int GetHashCode() =>
        hashCode == 0
            ? hashCode = FNV32.Hash<HashableDefault<A>, A>(this.AsEnumerable())
            : hashCode;

    public bool Equals(LstInternal<A>? other)
    {
        if (ReferenceEquals(this, other)) return true;
        if (ReferenceEquals(other, null)) return false;
        return Count == other.Count && EqEnumerable<A>.Equals(this, other);
    }

    public static bool operator ==(LstInternal<A> lhs, LstInternal<A> rhs) =>
        lhs.Equals(rhs);

    public static bool operator !=(LstInternal<A> lhs, LstInternal<A> rhs) =>
        !lhs.Equals(rhs);

    public int CompareTo(LstInternal<A> other)
    {
        var cmp = Count.CompareTo(other.Count);
        if (cmp != 0) return cmp;
        using var iterA = GetEnumerator();
        using var iterB = other.GetEnumerator();
        while (iterA.MoveNext() && iterB.MoveNext())
        {
            cmp = OrdDefault<A>.Compare(iterA.Current, iterB.Current);
            if (cmp != 0) return cmp;
        }
        return 0;
    }

    public int CompareTo<OrdA>(LstInternal<A> other) where OrdA : Ord<A>
    {
        var cmp = Count.CompareTo(other.Count);
        if (cmp != 0) return cmp;
        using var iterA = GetEnumerator();
        using var iterB = other.GetEnumerator();
        while (iterA.MoveNext() && iterB.MoveNext())
        {
            cmp = OrdA.Compare(iterA.Current, iterB.Current);
            if (cmp != 0) return cmp;
        }
        return 0;
    }

    public bool IsEmpty =>
        Root.Count == 0; 
}
