//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using static LanguageExt.Prelude;
//using System.Diagnostics.Contracts;
//using LanguageExt.TypeClasses;
//using LanguageExt.ClassInstances;

//namespace LanguageExt
//{
//    /// <summary>
//    /// Internal representation of a hash-set.  This allows for the HashSet type to be
//    /// a non-nullable struct.
//    /// </summary>
//    /// <typeparam name="A">Key type</typeparam>
//    internal class HashSetInternal<EqA, A> :
//        IEquatable<HashSetInternal<EqA, A>>
//        where EqA : struct, Eq<A>
//    {
//        public static readonly HashSetInternal<EqA, A> Empty = new HashSetInternal<EqA, A>();

//        readonly MapInternal<TInt, int, SeqStrict<A>> hashTable;
//        readonly int count;
//        int hashCode;

//        internal HashSetInternal()
//        {
//            hashTable = MapInternal<TInt, int, SeqStrict<A>>.Empty;
//        }

//        internal HashSetInternal(MapInternal<TInt, int, SeqStrict<A>> hashTable, int count)
//        {
//            this.hashTable = hashTable;
//            this.count = count;
//        }

//        internal HashSetInternal(IEnumerable<A> items, bool tryAdd = false)
//        {
//            var set = new HashSetInternal<EqA, A>();

//            if (tryAdd)
//            {
//                foreach (var item in items)
//                {
//                    set = set.TryAdd(item);
//                }
//            }
//            else
//            {
//                foreach (var item in items)
//                {
//                    set = set.Add(item);
//                }
//            }
//            hashTable = set.hashTable;
//            count = set.count;
//            hashCode = set.hashCode;
//        }

//        [Pure]
//        public A this[A key] =>
//            Find(key).IfNone(() => failwith<A>("Key doesn't exist in set"));

//        [Pure]
//        public bool IsEmpty =>
//            Count == 0;

//        [Pure]
//        public int Count =>
//            count;

//        [Pure]
//        public int Length =>
//            count;

//        [Pure]
//        public object SyncRoot => 
//            this;

//        [Pure]
//        public bool IsSynchronized =>
//            true;

//        [Pure]
//        public bool IsReadOnly => 
//            true;

//        [Pure]
//        public HashSetInternal<EqB, B> Map<EqB, B>(Func<A, B> map) where EqB : struct, Eq<B>
//        {
//            var set = HashSetInternal<EqB, B>.Empty;
//            foreach(var bucket in hashTable)
//            {
//                foreach(var item in bucket.Value)
//                {
//                    set = set.AddOrUpdate(map(item));
//                }
//            }
//            return set;
//        }

//        [Pure]
//        public HashSetInternal<EqA, A> Filter(Func<A, bool> pred)
//        {
//            var ht = MapInternal<TInt, int, SeqStrict<A>>.Empty;
//            var count = 0;

//            foreach(var bucket in hashTable)
//            {
//                var b = (SeqStrict<A>)bucket.Value.Filter(pred);
//                count += b.Count;
//                if (b.Count > 0)
//                {
//                    ht = ht.Add(bucket.Key, b);
//                }
//            }
//            return new HashSetInternal<EqA, A>(ht, count);
//        }

//        [Pure]
//        public HashSetInternal<EqA, A> Add(A key)
//        {
//            if (isnull(key)) throw new ArgumentNullException(nameof(key));

//            var ht = hashTable;
//            var hash = default(EqA).GetHashCode(key);
//            var bucket = ht.Find(hash);
//            if (bucket.IsSome)
//            {
//                foreach(var item in bucket.Value)
//                {
//                    if(default(EqA).Equals(item, key))
//                    {
//                        throw new ArgumentException("Key already exists in HSet");
//                    }
//                }
//                ht = ht.SetItem(hash, (SeqStrict<A>)bucket.Value.Add(key));
//            }
//            else
//            {
//                ht = ht.Add(hash, SeqStrict<A>.FromSingleValue(key));
//            }
//            return new HashSetInternal<EqA, A>(ht, Count + 1);
//        }

//        [Pure]
//        public HashSetInternal<EqA, A> TryAdd(A key)
//        {
//            if (isnull(key)) throw new ArgumentNullException(nameof(key));

//            var ht = hashTable;
//            var hash = default(EqA).GetHashCode(key);
//            var bucket = ht.Find(hash);
//            if (bucket.IsSome)
//            {
//                foreach (var item in bucket.Value)
//                {
//                    if (default(EqA).Equals(item, key))
//                    {
//                        return this;
//                    }
//                }
//                ht = ht.SetItem(hash, (SeqStrict<A>)bucket.Value.Add(key));
//            }
//            else
//            {
//                ht = ht.Add(hash, SeqStrict<A>.FromSingleValue(key));
//            }
//            return new HashSetInternal<EqA, A>(ht, Count + 1);
//        }

//        [Pure]
//        public HashSetInternal<EqA, A> AddOrUpdate(A key)
//        {
//            if (isnull(key)) throw new ArgumentNullException(nameof(key));

//            var ht = hashTable;
//            var hash = default(EqA).GetHashCode(key);
//            var bucket = ht.Find(hash);
//            if (bucket.IsSome)
//            {
//                var bucketValue = bucket.Value;
//                var contains = false;
//                var index = 0;

//                foreach (var item in bucketValue)
//                {
//                    if (default(EqA).Equals(item, key))
//                    {
//                        contains = true;
//                        break;
//                    }
//                    index++;
//                }

//                if (contains)
//                {
//                    return new HashSetInternal<EqA, A>(ht.SetItem(hash, bucketValue.SetItem(index, key)), Count);
//                }
//                else
//                {
//                    return new HashSetInternal<EqA, A>(ht.SetItem(hash, (SeqStrict<A>)bucketValue.Add(key)), Count + 1);
//                }
//            }
//            else
//            {
//                return new HashSetInternal<EqA, A>(ht.Add(hash, SeqStrict<A>.FromSingleValue(key)), Count + 1);
//            }
//        }

//        [Pure]
//        public HashSetInternal<EqA, A> AddRange(IEnumerable<A> range)
//        {
//            if (range == null)
//            {
//                return this;
//            }
//            var self = this;
//            foreach (var item in range)
//            {
//                self = self.Add(item);
//            }
//            return self;
//        }

//        [Pure]
//        public HashSetInternal<EqA, A> TryAddRange(IEnumerable<A> range)
//        {
//            if (range == null)
//            {
//                return this;
//            }
//            var self = this;
//            foreach (var item in range)
//            {
//                self = self.TryAdd(item);
//            }
//            return self;
//        }

//        [Pure]
//        public HashSetInternal<EqA, A> AddOrUpdateRange(IEnumerable<A> range)
//        {
//            if (range == null)
//            {
//                return this;
//            }
//            var self = this;
//            foreach (var item in range)
//            {
//                self = self.AddOrUpdate(item);
//            }
//            return self;
//        }

//        [Pure]
//        public HashSetInternal<EqA, A> Remove(A key)
//        {
//            if (isnull(key)) return this;
//            var ht = hashTable;
//            var hash = default(EqA).GetHashCode(key);
//            var bucket = ht.Find(hash);
//            if (bucket.IsSome)
//            {
//                var bucketValue = bucket.Value;
//                bucketValue = (SeqStrict<A>)bucketValue.Filter(x => !default(EqA).Equals(x, key));
//                return bucketValue.Count == 0
//                    ? new HashSetInternal<EqA, A>(ht.Remove(hash), Count - 1)
//                    : new HashSetInternal<EqA, A>(ht.SetItem(hash, bucketValue), Count - 1);
//            }
//            else
//            {
//                return this;
//            }
//        }

//        [Pure]
//        public Option<A> Find(A key)
//        {
//            if (isnull(key)) return None;
//            return hashTable.Find(default(EqA).GetHashCode(key))
//                            .Bind(bucket => bucket.Find(x => default(EqA).Equals(x, key)));
//        }

//        [Pure]
//        public IEnumerable<A> FindSeq(A key) =>
//            Find(key).AsEnumerable();

//        [Pure]
//        public R Find<R>(A key, Func<A, R> Some, Func<R> None) =>
//            Find(key).Match(Some, None);

//        [Pure]
//        public HashSetInternal<EqA, A> SetItem(A key)
//        {
//            if (isnull(key)) throw new ArgumentNullException(nameof(key));

//            var ht = hashTable;
//            var hash = default(EqA).GetHashCode(key);
//            var bucket = ht.Find(hash);
//            return bucket.IsSome
//                ? new HashSetInternal<EqA, A>(ht.SetItem(hash, (SeqStrict<A>)bucket.Value.Map(x => default(EqA).Equals(x, key) ? key : x)), Count)
//                : throw new ArgumentException("Key not found in Set");
//        }

//        [Pure]
//        public HashSetInternal<EqA, A> TrySetItem(A key)
//        {
//            if (isnull(key)) throw new ArgumentNullException(nameof(key));

//            var ht = hashTable;
//            var hash = default(EqA).GetHashCode(key);
//            var bucket = ht.Find(hash);
//            return bucket.IsSome
//                ? new HashSetInternal<EqA, A>(ht.TrySetItem(hash, (SeqStrict<A>)bucket.Value.Map(x => default(EqA).Equals(x, key) ? key : x)), Count)
//                : this;
//        }

//        [Pure]
//        public bool Contains(A key) =>
//            !isnull(key) && Find(key).IsSome;

//        [Pure]
//        public HashSetInternal<EqA, A> Clear() =>
//            Empty;

//        [Pure]
//        public HashSetInternal<EqA, A> SetItems(IEnumerable<A> items)
//        {
//            if (items == null) return this;
//            var self = this;
//            foreach (var item in items)
//            {
//                if (isnull(item)) continue;
//                self = SetItem(item);
//            }
//            return self;
//        }

//        [Pure]
//        public HashSetInternal<EqA, A> TrySetItems(IEnumerable<A> items)
//        {
//            if (items == null) return this;
//            var self = this;
//            foreach (var item in items)
//            {
//                if (isnull(item)) continue;
//                self = TrySetItem(item);
//            }
//            return self;
//        }

//        [Pure]
//        public HashSetInternal<EqA, A> RemoveRange(IEnumerable<A> keys)
//        {
//            var self = this;
//            foreach (var key in keys)
//            {
//                self = self.Remove(key);
//            }
//            return self;
//        }

//        public IEnumerator<A> GetEnumerator() =>
//            AsEnumerable().GetEnumerator();

//        public Seq<A> ToSeq() =>
//            Seq(hashTable.Values.Bind(x => x));

//        public IEnumerable<A> AsEnumerable() =>
//            hashTable.Values.Bind(x => x);

//        [Pure]
//        public static HashSetInternal<EqA, A> operator +(HashSetInternal<EqA, A> lhs, HashSetInternal<EqA, A> rhs) =>
//            lhs.Append(rhs);

//        [Pure]
//        public HashSetInternal<EqA, A> Append(HashSetInternal<EqA, A> rhs)
//        {
//            var self = this;
//            foreach (var item in rhs)
//            {
//                if (!self.Contains(item))
//                {
//                    self = self.Add(item);
//                }
//            }
//            return self;
//        }

//        [Pure]
//        public static HashSetInternal<EqA, A> operator -(HashSetInternal<EqA, A> lhs, HashSetInternal<EqA, A> rhs) =>
//            lhs.Subtract(rhs);

//        [Pure]
//        public HashSetInternal<EqA, A> Subtract(HashSetInternal<EqA, A> rhs)
//        {
//            var self = this;
//            foreach (var item in rhs)
//            {
//                self = self.Remove(item);
//            }
//            return self;
//        }

//        public bool Equals(HashSetInternal<EqA, A> other)
//        {
//            if (other == null || Count != other.Count) return false;
//            var iterx = GetEnumerator();
//            var itery = other.GetEnumerator();

//            for (int i = 0; i < count; i++)
//            {
//                iterx.MoveNext();
//                itery.MoveNext();
//                if (!default(EqA).Equals(iterx.Current, itery.Current)) return false;
//            }
//            return true;
//        }

//        /// <summary>
//        /// Returns True if 'other' is a proper subset of this set
//        /// </summary>
//        /// <returns>True if 'other' is a proper subset of this set</returns>
//        [Pure]
//        public bool IsProperSubsetOf(IEnumerable<A> other)
//        {
//            if (IsEmpty)
//            {
//                return other.Any();
//            }

//            var otherSet = new HashSetInternal<EqA, A>(other);
//            if (Count >= otherSet.Count)
//            {
//                return false;
//            }

//            int matches = 0;
//            bool extraFound = false;
//            foreach (var item in otherSet)
//            {
//                if (Contains(item))
//                {
//                    matches++;
//                }
//                else
//                {
//                    extraFound = true;
//                }

//                if (matches == Count && extraFound)
//                {
//                    return true;
//                }
//            }

//            return false;
//        }

//        /// <summary>
//        /// Returns True if 'other' is a proper superset of this set
//        /// </summary>
//        /// <returns>True if 'other' is a proper superset of this set</returns>
//        [Pure]
//        public bool IsProperSupersetOf(IEnumerable<A> other)
//        {
//            if (IsEmpty)
//            {
//                return false;
//            }

//            int matchCount = 0;
//            foreach (var item in other)
//            {
//                matchCount++;
//                if (!Contains(item))
//                {
//                    return false;
//                }
//            }

//            return Count > matchCount;
//        }

//        /// <summary>
//        /// Returns True if 'other' is a superset of this set
//        /// </summary>
//        /// <returns>True if 'other' is a superset of this set</returns>
//        [Pure]
//        public bool IsSubsetOf(IEnumerable<A> other)
//        {
//            if (IsEmpty)
//            {
//                return true;
//            }

//            var otherSet = new HashSetInternal<EqA, A>(other);
//            int matches = 0;
//            foreach (var item in otherSet)
//            {
//                if (Contains(item))
//                {
//                    matches++;
//                }
//            }
//            return matches == Count;
//        }

//        /// <summary>
//        /// Returns True if 'other' is a superset of this set
//        /// </summary>
//        /// <returns>True if 'other' is a superset of this set</returns>
//        [Pure]
//        public bool IsSupersetOf(IEnumerable<A> other)
//        {
//            foreach (var item in other)
//            {
//                if (!Contains(item))
//                {
//                    return false;
//                }
//            }
//            return true;
//        }

//        /// <summary>
//        /// Returns True if other overlaps this set
//        /// </summary>
//        /// <typeparam name="T">Element type</typeparam>
//        /// <param name="setA">Set A</param>
//        /// <param name="setB">Set B</param>
//        /// <returns>True if other overlaps this set</returns>
//        [Pure]
//        public bool Overlaps(IEnumerable<A> other)
//        {
//            if (IsEmpty)
//            {
//                return false;
//            }

//            foreach (var item in other)
//            {
//                if (Contains(item))
//                {
//                    return true;
//                }
//            }
//            return false;
//        }

//        public bool SetEquals(IEnumerable<A> other) =>
//            Equals(new HashSetInternal<EqA, A>(other));

//        public void CopyTo(A[] array, int index)
//        {
//            if (array == null) throw new ArgumentNullException(nameof(array));
//            if (index < 0 || index > array.Length) throw new IndexOutOfRangeException();
//            if (index + Count > array.Length) throw new IndexOutOfRangeException();

//            foreach (var element in this)
//            {
//                array[index++] = element;
//            }
//        }

//        public void CopyTo(System.Array array, int index)
//        {
//            if (array == null) throw new ArgumentNullException(nameof(array));
//            if (index < 0 || index > array.Length) throw new IndexOutOfRangeException();
//            if (index + Count > array.Length) throw new IndexOutOfRangeException();

//            foreach (var element in this)
//            {
//                array.SetValue(element, index++);
//            }
//        }
//        public override int GetHashCode()
//        {
//            if (hashCode != 0) return hashCode;
//            return hashCode = hash(AsEnumerable());
//        }
//    }
//}
