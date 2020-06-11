using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;
using LSeq = LanguageExt.Seq;
using System.Threading.Tasks;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Represents an empty sequence
        /// </summary>
        public static readonly SeqEmpty Empty = new SeqEmpty();

        /// <summary>
        /// Construct a sequence from any value
        /// </summary>
        /// <typeparam name="A">Type of the items in the sequence</typeparam>
        /// <param name="head">Head item in the sequence</param>
        /// <returns></returns>
        [Pure]
        public static Seq<A> Cons<A>(this A head) =>
            LSeq.FromSingleValue(head);

        /// <summary>
        /// Construct a sequence from any value
        /// </summary>
        /// <typeparam name="A">Type of the items in the sequence</typeparam>
        /// <param name="head">Head item in the sequence</param>
        /// <returns></returns>
        [Pure]
        public static Seq<A> Cons<A>(this A head, SeqEmpty empty) =>
            LSeq.FromSingleValue(head);

        /// <summary>
        /// Construct a list from head and tail; head becomes the first item in 
        /// the list.  
        /// </summary>
        /// <typeparam name="A">Type of the items in the sequence</typeparam>
        /// <param name="head">Head item in the sequence</param>
        /// <param name="tail">Tail of the sequence</param>
        /// <returns></returns>
        [Pure]
        public static Seq<A> Cons<A>(this A head, Seq<A> tail) =>
            tail.Cons(head);

        /// <summary>
        /// Construct a list from head and tail; head becomes the first item in 
        /// the list.  
        /// </summary>
        /// <typeparam name="A">Type of the items in the sequence</typeparam>
        /// <param name="head">Head item in the sequence</param>
        /// <param name="tail">Tail of the sequence</param>
        /// <returns></returns>
        [Pure]
        public static Seq<A> Cons<A>(this A head, A[] tail)
        {
            if (tail == null || tail.Length == 0)
            {
                return LSeq.FromSingleValue(head);
            }
            else
            {
                var data = new A[tail.Length + 1];
                System.Array.Copy(tail, 0, data, 1, tail.Length);
                data[0] = head;
                return LSeq.FromArray(data);
            }
        }

        /// <summary>
        /// Construct a list from head and tail; head becomes the first item in 
        /// the list.  
        /// </summary>
        /// <typeparam name="A">Type of the items in the sequence</typeparam>
        /// <param name="head">Head item in the sequence</param>
        /// <param name="tail">Tail of the sequence</param>
        /// <returns></returns>
        [Pure]
        public static Seq<A> Cons<A>(this A head, Arr<A> tail) =>
            Cons(head, tail.Value);

        /// <summary>
        /// Construct a list from head and tail; head becomes the first item in 
        /// the list.  
        /// </summary>
        /// <typeparam name="A">Type of the items in the sequence</typeparam>
        /// <param name="head">Head item in the sequence</param>
        /// <param name="tail">Tail of the sequence</param>
        /// <returns></returns>
        [Pure]
        public static Seq<A> Cons<A>(this A head, IEnumerable<A> tail) =>
            tail == null       ? LSeq.FromSingleValue(head)
          : tail is Seq<A> seq ? head.Cons(seq)
          : new Seq<A>(tail).Cons(head);

        /// <summary>
        /// Construct a list from head and tail; head becomes the first item in 
        /// the list.  
        /// </summary>
        /// <typeparam name="A">Type of the items in the sequence</typeparam>
        /// <param name="head">Head item in the sequence</param>
        /// <param name="tail">Tail of the sequence</param>
        /// <returns></returns>
        [Pure]
        public static Lst<A> Cons<A>(this A head, Lst<A> tail) =>
            tail.Insert(0, head);

        /// <summary>
        /// Lazy sequence of natural numbers up to Int32.MaxValue
        /// </summary>
        [Pure]
        public static IEnumerable<int> Naturals
        {
            get
            {
                for (var i = 0; i < Int32.MaxValue; i++)
                {
                    yield return i;
                }
            }
        }

        /// <summary>
        /// Lazy sequence of natural numbers up to Int64.MaxValue
        /// </summary>
        [Pure]
        public static IEnumerable<long> LongNaturals
        {
            get
            {
                for (var i = 0L; i < Int64.MaxValue; i++)
                {
                    yield return i;
                }
            }
        }

        /// <summary>
        /// Lazily generate a range of integers.  
        /// </summary>
        [Pure]
        public static IEnumerable<int> Range(int from, int count, int step = 1) =>
            IntegerRange.FromCount(from, count, step);

        /// <summary>
        /// Lazily generate a range of chars.  
        /// 
        ///   Remarks:
        ///     Can go in a positive direction ('a'..'z') as well as negative ('z'..'a')
        /// </summary>
        [Pure]
        public static Seq<char> Range(char from, char to) =>
            Seq(CharRange.FromMinMax(from, to));

        /// <summary>
        /// Lazily generate integers from any number of provided ranges
        /// </summary>
        [Pure]
        public static IEnumerable<int> Range(params IEnumerable<int>[] ranges)
        {
            foreach(var range in ranges)
            {
                foreach(var i in range)
                {
                    yield return i;
                }
            }
        }

        /// <summary>
        /// Lazily generate chars from any number of provided ranges
        /// </summary>
        [Pure]
        public static IEnumerable<char> Range(params IEnumerable<char>[] ranges)
        {
            foreach(var range in ranges)
            {
                foreach(var c in range)
                {
                    yield return c;
                }
            }
        }

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<K, V> Map<K, V>() =>
            LanguageExt.Map.empty<K, V>();

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<K, V> Map<K, V>(Tuple<K, V> head, params Tuple<K, V>[] tail) =>
            LanguageExt.Map.create(head, tail);

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<K, V> Map<K, V>((K, V) head, params (K, V)[] tail) =>
            LanguageExt.Map.create(head, tail);

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<K, V> Map<K, V>(KeyValuePair<K, V> head, params KeyValuePair<K, V>[] tail) =>
            LanguageExt.Map.create(head, tail);

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<K, V> toMap<K, V>(IEnumerable<Tuple<K, V>> items) =>
            LanguageExt.Map.createRange(items);

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<K, V> toMap<K, V>(IEnumerable<(K, V)> items) =>
            LanguageExt.Map.createRange(items);

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<K, V> toMap<K, V>(IEnumerable<KeyValuePair<K, V>> items) =>
            LanguageExt.Map.createRange(items);



        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<OrdK, K, V> Map<OrdK, K, V>() where OrdK : struct, Ord<K> =>
            LanguageExt.Map.empty<OrdK, K, V>();

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<OrdK, K, V> Map<OrdK, K, V>(Tuple<K, V> head, params Tuple<K, V>[] tail) where OrdK : struct, Ord<K> =>
            LanguageExt.Map.create<OrdK, K, V>(head, tail);

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<OrdK, K, V> Map<OrdK, K, V>((K, V) head, params (K, V)[] tail) where OrdK : struct, Ord<K> =>
            LanguageExt.Map.create<OrdK, K, V>(head, tail);

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<OrdK, K, V> toMap<OrdK, K, V>(KeyValuePair<K, V> head, params KeyValuePair<K, V>[] tail) where OrdK : struct, Ord<K> =>
            LanguageExt.Map.create<OrdK, K, V>(head, tail);

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<OrdK, K, V> toMap<OrdK, K, V>(IEnumerable<(K, V)> items) where OrdK : struct, Ord<K> =>
            LanguageExt.Map.createRange<OrdK, K, V>(items);

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<OrdK, K, V> toMap<OrdK, K, V>(IEnumerable<Tuple<K, V>> items) where OrdK : struct, Ord<K> =>
            LanguageExt.Map.createRange<OrdK, K, V>(items);

        /// <summary>
        /// Create an immutable map
        /// </summary>
        [Pure]
        public static Map<OrdK, K, V> toMap<OrdK, K, V>(IEnumerable<KeyValuePair<K, V>> items) where OrdK : struct, Ord<K> =>
            LanguageExt.Map.createRange<OrdK, K, V>(items);



        /// <summary>
        /// Create an immutable hash-map
        /// </summary>
        [Pure]
        public static HashMap<K, V> HashMap<K, V>() =>
            LanguageExt.HashMap.empty<K, V>();

        /// <summary>
        /// Create an immutable hash-map
        /// </summary>
        [Pure]
        public static HashMap<K, V> HashMap<K, V>(Tuple<K,V> head, params Tuple<K, V>[] tail) =>
            LanguageExt.HashMap.create(head, tail);

        /// <summary>
        /// Create an immutable hash-map
        /// </summary>
        [Pure]
        public static HashMap<K, V> HashMap<K, V>((K, V) head, params (K, V)[] tail) =>
            LanguageExt.HashMap.create(head, tail);

        /// <summary>
        /// Create an immutable hash-map
        /// </summary>
        [Pure]
        public static HashMap<K, V> HashMap<K, V>(KeyValuePair<K, V> head, params KeyValuePair<K, V>[] tail) =>
            LanguageExt.HashMap.create(head, tail);

        /// <summary>
        /// Create an immutable hash-map
        /// </summary>
        [Pure]
        public static HashMap<K, V> toHashMap<K, V>(IEnumerable<(K, V)> items) =>
            LanguageExt.HashMap.createRange(items);

        /// <summary>
        /// Create an immutable hash-map
        /// </summary>
        [Pure]
        public static HashMap<K, V> toHashMap<K, V>(IEnumerable<Tuple<K, V>> items) =>
            LanguageExt.HashMap.createRange(items);

        /// <summary>
        /// Create an immutable hash-map
        /// </summary>
        [Pure]
        public static HashMap<K, V> toHashMap<K, V>(IEnumerable<KeyValuePair<K, V>> items) =>
            LanguageExt.HashMap.createRange(items);



        /// <summary>
        /// Create an immutable hash-map
        /// </summary>
        [Pure]
        public static HashMap<EqK, K, V> HashMap<EqK, K, V>() where EqK : struct, Eq<K> =>
            LanguageExt.HashMap.empty<EqK, K, V>();

        /// <summary>
        /// Create an immutable hash-map
        /// </summary>
        [Pure]
        public static HashMap<EqK, K, V> HashMap<EqK, K, V>(Tuple<K, V> head, params Tuple<K, V>[] tail) where EqK : struct, Eq<K> =>
            LanguageExt.HashMap.create<EqK, K, V>(head, tail);

        /// <summary>
        /// Create an immutable hash-map
        /// </summary>
        [Pure]
        public static HashMap<EqK, K, V> HashMap<EqK, K, V>((K, V) head, params (K, V)[] tail) where EqK : struct, Eq<K> =>
            LanguageExt.HashMap.create<EqK, K, V>(head, tail);

        /// <summary>
        /// Create an immutable hash-map
        /// </summary>
        [Pure]
        public static HashMap<EqK, K, V> HashMap<EqK, K, V>(KeyValuePair<K, V> head, params KeyValuePair<K, V>[] tail) where EqK : struct, Eq<K> =>
            LanguageExt.HashMap.create<EqK, K, V>(head, tail);

        /// <summary>
        /// Create an immutable hash-map
        /// </summary>
        [Pure]
        public static HashMap<EqK, K, V> toHashMap<EqK, K, V>(IEnumerable<Tuple<K, V>> items) where EqK : struct, Eq<K> =>
            LanguageExt.HashMap.createRange<EqK, K, V>(items);

        /// <summary>
        /// Create an immutable hash-map
        /// </summary>
        [Pure]
        public static HashMap<EqK, K, V> toHashMap<EqK, K, V>(IEnumerable<(K, V)> items) where EqK : struct, Eq<K> =>
            LanguageExt.HashMap.createRange<EqK, K, V>(items);

        /// <summary>
        /// Create an immutable hash-map
        /// </summary>
        [Pure]
        public static HashMap<EqK, K, V> toHashMap<EqK, K, V>(IEnumerable<KeyValuePair<K, V>> items) where EqK : struct, Eq<K> =>
            LanguageExt.HashMap.createRange<EqK, K, V>(items);




        /// <summary>
        /// Create an immutable list
        /// </summary>
        [Pure]
        public static Lst<T> List<T>() =>
            Lst<T>.Empty;

        /// <summary>
        /// Create an immutable list
        /// </summary>
        [Pure]
        public static Lst<T> List<T>(T x, params T[] xs)
        {
            return new Lst<T>(Yield());

            IEnumerable<T> Yield()
            {
                yield return x;
                foreach(var item in xs)
                {
                    yield return item;
                }
            }
        }

        /// <summary>
        /// Create an immutable list
        /// </summary>
        [Pure]
        public static Lst<T> toList<T>(Arr<T> items) =>
            new Lst<T>(items.Value);

        /// <summary>
        /// Create an immutable list
        /// </summary>
        [Pure]
        public static Lst<T> toList<T>(IEnumerable<T> items) =>
            items is Lst<T>
                ? (Lst<T>)items
                : new Lst<T>(items);



        /// <summary>
        /// Create an immutable list
        /// </summary>
        [Pure]
        public static Lst<PredList, T> List<PredList, T>() where PredList : struct, Pred<ListInfo> =>
            new Lst<PredList, T>(new T[0]);

        /// <summary>
        /// Create an immutable list
        /// </summary>
        [Pure]
        public static Lst<PredList, T> List<PredList, T>(T x, params T[] xs) where PredList : struct, Pred<ListInfo> =>
            new Lst<PredList, T>(x.Cons(xs));

        /// <summary>
        /// Create an immutable list
        /// </summary>
        [Pure]
        public static Lst<PredList, T> toList<PredList, T>(Arr<T> items) where PredList : struct, Pred<ListInfo> =>
            new Lst<PredList, T>(items.Value);

        /// <summary>
        /// Create an immutable list
        /// </summary>
        [Pure]
        public static Lst<PredList, T> toList<PredList, T>(IEnumerable<T> items) where PredList : struct, Pred<ListInfo> =>
            items is Lst<PredList, T>
                ? (Lst<PredList, T>)items
                : new Lst<PredList, T>(items);

        

        /// <summary>
        /// Create an immutable list
        /// </summary>
        [Pure]
        public static Lst<PredList, PredItem, T> List<PredList, PredItem, T>() 
            where PredItem : struct, Pred<T>
            where PredList : struct, Pred<ListInfo>  =>
            new Lst<PredList, PredItem, T>(new T[0]);

        /// <summary>
        /// Create an immutable list
        /// </summary>
        [Pure]
        public static Lst<PredList, PredItem, T> List<PredList, PredItem, T>(T x, params T[] xs) 
            where PredItem : struct, Pred<T>
            where PredList : struct, Pred<ListInfo>  =>
            new Lst<PredList, PredItem, T>(x.Cons(xs));

        /// <summary>
        /// Create an immutable list
        /// </summary>
        [Pure]
        public static Lst<PredList, PredItem, T> toList<PredList, PredItem, T>(Arr<T> items) 
            where PredItem : struct, Pred<T>
            where PredList : struct, Pred<ListInfo>  =>
            new Lst<PredList, PredItem, T>(items.Value);

        /// <summary>
        /// Create an immutable list
        /// </summary>
        [Pure]
        public static Lst<PredList, PredItem, T> toList<PredList, PredItem, T>(IEnumerable<T> items) 
            where PredItem : struct, Pred<T>
            where PredList : struct, Pred<ListInfo>  =>
            items is Lst<PredList, PredItem, T>
                ? (Lst<PredList, PredItem, T>)items
                : new Lst<PredList, PredItem, T>(items);




        /// <summary>
        /// Create an immutable array
        /// </summary>
        [Pure]
        public static Arr<T> Array<T>() =>
            Arr<T>.Empty;

        /// <summary>
        /// Create an immutable array
        /// </summary>
        [Pure]
        public static Arr<T> Array<T>(T x, params T[] xs) =>
            new Arr<T>(x.Cons(xs).ToArray());

        /// <summary>
        /// Create an immutable array
        /// </summary>
        [Pure]
        public static Arr<T> toArray<T>(IEnumerable<T> items) =>
            new Arr<T>(items);

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        [Pure]
        public static Que<T> Queue<T>() =>
            Que<T>.Empty;

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        [Pure]
        public static Que<T> Queue<T>(params T[] items)
        {
            var q = new QueInternal<T>();
            foreach (var item in items)
            {
                q = q.Enqueue(item);
            }
            return new Que<T>(q);
        }

        /// <summary>
        /// Create an immutable queue
        /// </summary>
        [Pure]
        public static Que<T> toQueue<T>(IEnumerable<T> items)
        {
            var q = new QueInternal<T>();
            foreach (var item in items)
            {
                q = q.Enqueue(item);
            }
            return new Que<T>(q);
        }

        /// <summary>
        /// Create an immutable stack
        /// </summary>
        [Pure]
        public static Stck<T> Stack<T>() =>
            new Stck<T>();

        /// <summary>
        /// Create an immutable stack
        /// </summary>
        [Pure]
        public static Stck<T> Stack<T>(params T[] items) =>
            new Stck<T>(items);

        /// <summary>
        /// Create an immutable stack
        /// </summary>
        [Pure]
        public static Stck<T> toStack<T>(IEnumerable<T> items) =>
            new Stck<T>(items);

        /// <summary>
        /// Create an immutable stack
        /// </summary>
        [Pure]
        public static Stck<T> toStackRev<T>(IEnumerable<T> items) =>
            new Stck<T>(items.Reverse());

        /// <summary>
        /// Create an immutable map, updating duplicates so that the final value of any key is retained
        /// </summary>
        [Pure]
        public static Map<K, V> toMapUpdate<K, V>(IEnumerable<Tuple<K, V>> keyValues) =>
            LanguageExt.Map<K, V>.Empty.AddOrUpdateRange(keyValues);

        /// <summary>
        /// Create an immutable map, updating duplicates so that the final value of any key is retained
        /// </summary>
        [Pure]
        public static Map<K, V> toMapUpdate<K, V>(IEnumerable<(K, V)> keyValues) =>
            LanguageExt.Map<K, V>.Empty.AddOrUpdateRange(keyValues);

        /// <summary>
        /// Create an immutable map, updating duplicates so that the final value of any key is retained
        /// </summary>
        [Pure]
        public static Map<K, V> toMapUpdate<K, V>(IEnumerable<KeyValuePair<K, V>> keyValues) =>
            LanguageExt.Map<K, V>.Empty.AddOrUpdateRange(keyValues);

        /// <summary>
        /// Create an immutable map, ignoring duplicates so the first value of any key is retained
        /// </summary>
        [Pure]
        public static Map<K, V> toMapTry<K, V>(IEnumerable<Tuple<K, V>> keyValues) =>
            LanguageExt.Map<K, V>.Empty.TryAddRange(keyValues);

        /// <summary>
        /// Create an immutable map, ignoring duplicates so the first value of any key is retained
        /// </summary>
        [Pure]
        public static Map<K, V> toMapTry<K, V>(IEnumerable<(K, V)> keyValues) =>
            LanguageExt.Map<K, V>.Empty.TryAddRange(keyValues);

        /// <summary>
        /// Create an immutable map, ignoring duplicates so the first value of any key is retained
        /// </summary>
        [Pure]
        public static Map<K, V> toMapTry<K, V>(IEnumerable<KeyValuePair<K, V>> keyValues) =>
            LanguageExt.Map<K, V>.Empty.TryAddRange(keyValues);




        /// <summary>
        /// Create an immutable set
        /// </summary>
        [Pure]
        public static Set<T> Set<T>() =>
            LanguageExt.Set.create<T>();

        /// <summary>
        /// Create an immutable set
        /// </summary>
        [Pure]
        public static Set<T> Set<T>(T head, params T[] tail) =>
            LanguageExt.Set.createRange(head.Cons(tail));

        /// <summary>
        /// Create an immutable set
        /// </summary>
        [Pure]
        public static Set<T> toSet<T>(IEnumerable<T> items) =>
            LanguageExt.Set.createRange(items);



        /// <summary>
        /// Create an immutable set
        /// </summary>
        [Pure]
        public static Set<OrdT, T> Set<OrdT, T>() where OrdT : struct, Ord<T> =>
            LanguageExt.Set.create<OrdT, T>();

        /// <summary>
        /// Create an immutable set
        /// </summary>
        [Pure]
        public static Set<OrdT, T> Set<OrdT, T>(T head, params T[] tail) where OrdT : struct, Ord<T> =>
            LanguageExt.Set.createRange<OrdT, T>(head.Cons(tail));

        /// <summary>
        /// Create an immutable set
        /// </summary>
        [Pure]
        public static Set<OrdT, T> toSet<OrdT, T>(IEnumerable<T> items) where OrdT : struct, Ord<T> =>
            LanguageExt.Set.createRange<OrdT, T>(items);


        /// <summary>
        /// Create an immutable hash-set
        /// </summary>
        [Pure]
        public static HashSet<T> HashSet<T>() =>
            LanguageExt.HashSet.create<T>();

        /// <summary>
        /// Create an immutable hash-set
        /// </summary>
        [Pure]
        public static HashSet<T> HashSet<T>(T head, params T[] tail) =>
            LanguageExt.HashSet.createRange(head.Cons(tail));

        /// <summary>
        /// Create an immutable hash-set
        /// </summary>
        [Pure]
        public static HashSet<T> toHashSet<T>(IEnumerable<T> items) =>
            LanguageExt.HashSet.createRange(items);





        /// <summary>
        /// Create an immutable hash-set
        /// </summary>
        [Pure]
        public static HashSet<EqT, T> HashSet<EqT, T>() where EqT : struct, Eq<T> =>
            LanguageExt.HashSet.create<EqT, T>();

        /// <summary>
        /// Create an immutable hash-set
        /// </summary>
        [Pure]
        public static HashSet<EqT, T> HashSet<EqT, T>(T head, params T[] tail) where EqT : struct, Eq<T> =>
            LanguageExt.HashSet.createRange<EqT, T>(head.Cons(tail));

        /// <summary>
        /// Create an immutable hash-set
        /// </summary>
        [Pure]
        public static HashSet<EqT, T> toHashSet<EqT, T>(IEnumerable<T> items) where EqT : struct, Eq<T> =>
            LanguageExt.HashSet.createRange<EqT, T>(items);





        /// <summary>
        /// Create a queryable
        /// </summary>
        [Pure]
        public static IQueryable<T> Query<T>(params T[] items) =>
            toQuery(items);

        /// <summary>
        /// Convert to queryable
        /// </summary>
        [Pure]
        public static IQueryable<T> toQuery<T>(IEnumerable<T> items) =>
            items.AsQueryable();

        /// <summary>
        /// Match empty list, or multi-item list
        /// </summary>
        /// <typeparam name="B">Return value type</typeparam>
        /// <param name="Empty">Match for an empty list</param>
        /// <param name="More">Match for a non-empty</param>
        /// <returns>Result of match function invoked</returns>
        public static B match<A, B>(IEnumerable<A> list,
            Func<B> Empty,
            Func<Seq<A>, B> More) =>
            Seq(list).Match(Empty, More);

        /// <summary>
        /// List pattern matching
        /// </summary>
        [Pure]
        public static B match<A, B>(IEnumerable<A> list,
            Func<B> Empty,
            Func<A, Seq<A>, B> More) =>
            Seq(list).Match(Empty, More);

        /// <summary>
        /// List pattern matching
        /// </summary>
        [Pure]
        public static R match<T, R>(IEnumerable<T> list,
            Func<R> Empty,
            Func<T, R> One,
            Func<T, Seq<T>, R> More) =>
            Seq(list).Match(Empty, One, More);

        [Pure]
        public static R match<K, V, R>(Map<K, V> map, K key, Func<V, R> Some, Func<R> None) =>
            match(LanguageExt.Map.find(map, key),
                   Some,
                   None );

        public static Unit match<K, V>(Map<K, V> map, K key, Action<V> Some, Action None) =>
            match(LanguageExt.Map.find(map, key),
                   Some,
                   None);

        [Pure]
        public static R match<K, V, R>(HashMap<K, V> map, K key, Func<V, R> Some, Func<R> None) =>
            match(LanguageExt.HashMap.find(map, key),
                   Some,
                   None);

        public static Unit match<K, V>(HashMap<K, V> map, K key, Action<V> Some, Action None) =>
            match(LanguageExt.HashMap.find(map, key),
                   Some,
                   None);

        /// <summary>
        /// Construct a sequence from any value
        ///     T     : [x]
        ///     null  : []
        /// </summary>
        [Pure]
        public static Seq<A> Seq1<A>(A value) =>
            value.IsNull() 
                ? Empty 
                : LSeq.FromSingleValue(value);

        /// <summary>
        /// Construct an empty Seq
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>() =>
            Empty;

        /// <summary>
        /// Construct a sequence from a nullable
        ///     HasValue == true  : [x]
        ///     HasValue == false : []
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>(A? value) where A : struct =>
            value == null 
                ? Empty 
                : LSeq.FromSingleValue(value.Value);

        /// <summary>
        /// Construct a sequence from an Enumerable
        /// Deals with `value == null` by returning `[]` and also memoizes the
        /// items in the enumerable as they're being consumed.
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>(IEnumerable<A> value) =>
            value == null                ? Empty
          : value is Seq<A> seq          ? seq
          : value is Arr<A> arr          ? LSeq.FromArray(arr.Value)
          : value is A[] array           ? Seq(array)
          : value is IList<A> list       ? Seq(list)
          : value is ICollection<A> coll ? Seq(coll)
          : new Seq<A>(value);

        /// <summary>
        /// Construct a sequence from an array
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>(A[] value)
        {
            if (value == null)
            {
                return Empty;
            }
            else
            {
                var length = value.Length;
                var data = new A[length];
                System.Array.Copy(value, data, length);
                return LSeq.FromArray(data);
            }
        }

        /// <summary>
        /// Construct a sequence from an array
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>(A a, A b) =>
            LSeq.FromArray(new[] { a, b });

        /// <summary>
        /// Construct a sequence from an array
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>(A a, A b, A c, params A[] ds)
        {
            var data = new A[ds.Length + 3];
            System.Array.Copy(ds, 0, data, 3, ds.Length);
            data[0] = a;
            data[1] = b;
            data[2] = c;
            return LSeq.FromArray(data);
        }

        /// <summary>
        /// Construct a sequence from an immutable array
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>(Arr<A> value) =>
            Seq(value.Value);

        /// <summary>
        /// Construct a sequence from a list
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>(IList<A> value) =>
            Seq(value.ToArray());

        /// <summary>
        /// Construct a sequence from a list
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>(ICollection<A> value) =>
            Seq(value.ToArray());

        /// <summary>
        /// Construct a sequence from an either
        ///     Right(x) : [x]
        ///     Left(y)  : []
        /// </summary>
        [Pure]
        public static Seq<R> Seq<L, R>(Either<L, R> value) =>
            value.IsRight
                ? LSeq.FromSingleValue(value.RightValue)
                : Empty;

        /// <summary>
        /// Construct a sequence from an either
        ///     Right(x) : [x]
        ///     Left(y)  : []
        /// </summary>
        [Pure]
        public static Seq<R> Seq<L, R>(EitherUnsafe<L, R> value) =>
            value.IsRight
                ? LSeq.FromSingleValue(value.RightValue)
                : Empty;

        /// <summary>
        /// Construct a sequence from a Try
        ///     Succ(x) : [x]
        ///     Fail(e) : []
        ///     null    : []
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>(Try<A> value) =>
            Seq(value?.ToOption() ?? Option<A>.None);

        /// <summary>
        /// Construct a sequence from a TryOption
        ///     Succ(x) : [x]
        ///     Fail(e) : []
        ///     None    : []
        ///     null    : []
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>(TryOption<A> value) =>
            Seq(value?.ToOption() ?? Option<A>.None);

        /// <summary>
        /// Construct a sequence from a TryOption
        ///     Succ(x) : [x]
        ///     Fail(e) : []
        ///     None    : []
        ///     null    : []
        /// </summary>
        [Pure]
        public static Task<Seq<T>> Seq<T>(TryAsync<T> value) =>
            value?.AsEnumerable() ?? LanguageExt.Seq<T>.Empty.AsTask();

        /// <summary>
        /// Construct a sequence from a TryOption
        ///     Succ(x) : [x]
        ///     Fail(e) : []
        ///     None    : []
        ///     null    : []
        /// </summary>
        [Pure]
        public static Task<Seq<T>> Seq<T>(TryOptionAsync<T> value) =>
            value?.AsEnumerable() ?? LanguageExt.Seq<T>.Empty.AsTask();

        /// <summary>
        /// Construct a sequence from a tuple
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>(Tuple<A> tup) =>
            tup == null
                ? Empty
                : LSeq.FromSingleValue(tup.Item1);

        /// <summary>
        /// Construct a sequence from a tuple
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>(Tuple<A, A> tup) =>
            tup == null
                ? Empty
                : LSeq.FromArray(new [] { tup.Item1, tup.Item2 });

        /// <summary>
        /// Construct a sequence from a tuple
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>(Tuple<A, A, A> tup) =>
            tup == null
                ? Empty
                : LSeq.FromArray(new [] { tup.Item1, tup.Item2, tup.Item3 });

        /// <summary>
        /// Construct a sequence from a tuple
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>(Tuple<A, A, A, A> tup) =>
            tup == null
                ? Empty
                : LSeq.FromArray(new[] { tup.Item1, tup.Item2, tup.Item3, tup.Item4 });

        /// <summary>
        /// Construct a sequence from a tuple
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>(Tuple<A, A, A, A, A> tup) =>
            tup == null
                ? Empty
                : LSeq.FromArray(new[] { tup.Item1, tup.Item2, tup.Item3, tup.Item4, tup.Item5 });

        /// <summary>
        /// Construct a sequence from a tuple
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>(Tuple<A, A, A, A, A, A> tup) =>
            tup == null
                ? Empty
                : LSeq.FromArray(new[] { tup.Item1, tup.Item2, tup.Item3, tup.Item4, tup.Item5, tup.Item6 });

        /// <summary>
        /// Construct a sequence from a tuple
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>(Tuple<A, A, A, A, A, A, A> tup) =>
            tup == null
                ? Empty
                : LSeq.FromArray(new[] { tup.Item1, tup.Item2, tup.Item3, tup.Item4, tup.Item5, tup.Item6, tup.Item7 });

        /// <summary>
        /// Construct a sequence from a tuple
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>(ValueTuple<A> tup) =>
            LSeq.FromSingleValue(tup.Item1);

        /// <summary>
        /// Construct a sequence from a tuple
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>(ValueTuple<A, A> tup) =>
            LSeq.FromArray(new[] { tup.Item1, tup.Item2 });

        /// <summary>
        /// Construct a sequence from a tuple
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>(ValueTuple<A, A, A> tup) =>
            LSeq.FromArray(new[] { tup.Item1, tup.Item2, tup.Item3 });

        /// <summary>
        /// Construct a sequence from a tuple
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>(ValueTuple<A, A, A, A> tup) =>
            LSeq.FromArray(new[] { tup.Item1, tup.Item2, tup.Item3, tup.Item4 });

        /// <summary>
        /// Construct a sequence from a tuple
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>(ValueTuple<A, A, A, A, A> tup) =>
            LSeq.FromArray(new[] { tup.Item1, tup.Item2, tup.Item3, tup.Item4, tup.Item5 });

        /// <summary>
        /// Construct a sequence from a tuple
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>(ValueTuple<A, A, A, A, A, A> tup) =>
            LSeq.FromArray(new[] { tup.Item1, tup.Item2, tup.Item3, tup.Item4, tup.Item5, tup.Item6 });

        /// <summary>
        /// Construct a sequence from a tuple
        /// </summary>
        [Pure]
        public static Seq<A> Seq<A>(ValueTuple<A, A, A, A, A, A, A> tup) =>
            LSeq.FromArray(new[] { tup.Item1, tup.Item2, tup.Item3, tup.Item4, tup.Item5, tup.Item6, tup.Item7 });

        /// <summary>
        /// Compute the difference between two documents, using the Wagner-Fischer algorithm. 
        /// O(mn) time and space.
        /// 
        ///     apply(diff(d,e), d) == e
        ///     
        ///     diff(d, d) == Patch.empty
        ///     
        ///     apply(diff(d, e), d) == apply(inverse(diff(e, d)), d)
        ///     
        ///     apply(append(diff(a, b), diff(b, c), a) == apply(diff(a, c), a)
        ///     
        ///     applicable(diff(a, b) a)
        /// 
        /// </summary>
        public static Patch<EqA, A> Diff<EqA, A>(this IEnumerable<A> va, IEnumerable<A> vb) where EqA : struct, Eq<A> =>
            Patch.diff<EqA, A>(va, vb);

        /// <summary>
        /// Apply the supplied patch to this collection
        /// </summary>
        public static IEnumerable<A> Apply<EqA, A>(this IEnumerable<A> va, Patch<EqA, A> patch) where EqA : struct, Eq<A> =>
            Patch.apply(patch, va);

        /// <summary>
        /// Apply the supplied patch to this collection
        /// </summary>
        public static Lst<A> Apply<EqA, A>(this Lst<A> va, Patch<EqA, A> patch) where EqA : struct, Eq<A> =>
            Patch.apply(patch, va);

        /// <summary>
        /// Apply the supplied patch to this collection
        /// </summary>
        public static Seq<A> Apply<EqA, A>(this Seq<A> va, Patch<EqA, A> patch) where EqA : struct, Eq<A> =>
            Patch.apply(patch, va);

        /// <summary>
        /// Apply the supplied patch to this collection
        /// </summary>
        public static SpanArray<A> Apply<EqA, A>(this SpanArray<A> va, Patch<EqA, A> patch) where EqA : struct, Eq<A> =>
            Patch.apply(patch, va);

        /// <summary>
        /// Apply the supplied patch to this collection
        /// </summary>
        public static A[] Apply<EqA, A>(this A[] va, Patch<EqA, A> patch) where EqA : struct, Eq<A> =>
            Patch.apply(patch, va);

        /// <summary>
        /// Apply the supplied patch to this collection
        /// </summary>
        public static Arr<A> Apply<EqA, A>(this Arr<A> va, Patch<EqA, A> patch) where EqA : struct, Eq<A> =>
            Patch.apply(patch, va);

        /// <summary>
        /// Apply the supplied patch to this collection
        /// </summary>
        public static List<A> Apply<EqA, A>(this List<A> va, Patch<EqA, A> patch) where EqA : struct, Eq<A> =>
            Patch.apply(patch, va);

        /// <summary>
        /// Returns true if a patch can be safely applied to a document, that is,
        /// `applicable(p, d)` holds when `d` is a valid source document for the patch `p`.
        /// </summary>
        public static bool Applicable<EqA, A>(this IEnumerable<A> va, Patch<EqA, A> patch) where EqA : struct, Eq<A> =>
            Patch.applicable(patch, va);
    }
}
