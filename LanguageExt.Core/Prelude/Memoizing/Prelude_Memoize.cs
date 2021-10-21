using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Returns a Func<A> that wraps func.  The first
        /// call to the resulting Func<A> will cache the result.
        /// Subsequent calls return the cached item.
        /// </summary>
        public static Func<A> memo<A>(Func<A> func)
        {
            var sync = new object();
            var value = default(A);
            bool valueSet = false;
            return () =>
            {
                if(valueSet)
                {
                    return value;
                }
                lock(sync)
                {
                    if (valueSet)
                    {
                        return value;
                    }
                    else
                    {
                        value = func();
                        valueSet = true;
                        return value;
                    }
                }
            };
        }

        /// <summary>
        /// Returns a `Func<A, B>` that wraps func.  Each time the resulting
        /// `Func<A, B>` is called with a new value, its result is memoized (cached).
        /// Subsequent calls use the memoized value.  
        /// 
        /// Remarks: 
        ///     Thread-safe and memory-leak safe.  
        /// </summary>
        public static Func<A, B> memo<A, B>(Func<A, B> func)
        {
            var cache = new WeakDict<A, B> ();
            var syncMap = new ConcurrentDictionary<A, object>();

            return inp =>
            {
                if(cache.TryGetValue(inp, out var x))
                {
                    return x;
                }
                else
                {
                    B res;
                    var sync = syncMap.GetOrAdd(inp, new object());
                    lock (sync)
                    {
                        res = cache.GetOrAdd(inp, func);
                    }
                    syncMap.TryRemove(inp, out sync);
                    return res;
                }
            };
        }

        /// <summary>
        /// Returns a Func<T,R> that wraps func.  Each time the resulting
        /// Func<T,R> is called with a new value, its result is memoized (cached).
        /// Subsequent calls use the memoized value.  
        /// 
        /// Remarks: 
        ///     No mechanism for freeing cached values and therefore can cause a
        ///     memory leak when holding onto the Func<T,R> reference.
        ///     Uses a ConcurrentDictionary for the cache and is thread-safe
        /// </summary>
        public static Func<T, R> memoUnsafe<T, R>(Func<T, R> func)
        {
            var cache = new ConcurrentDictionary<T, R>();
            var syncMap = new ConcurrentDictionary<T, object>();
            return inp =>
            {
                R res;
                if (!cache.TryGetValue(inp, out res))
                {
                    var sync = syncMap.GetOrAdd(inp, new object());
                    lock (sync)
                    {
                        res = cache.GetOrAdd(inp, func);
                    }
                    syncMap.TryRemove(inp, out sync);
                }
                return res;
            };
        }

        /// <summary>
        /// Enumerable memoization.  As an enumerable is enumerated each item is retained
        /// in an internal list, so that future evalation of the enumerable isn't done. 
        /// Only items not seen before are evaluated.  
        /// 
        /// This minimises one of the major problems with the IEnumerable / yield return 
        /// pattern by causing at-most-once evaluation of each item.  
        /// 
        /// Use the IEnumerable extension method Memo for convenience.
        /// </summary>
        /// <remarks>
        /// Although this allows efficient lazy evaluation, it does come at a memory cost.
        /// Each item is cached internally, so this method doesn't allow for evaluation of
        /// infinite sequences.
        /// </remarks>
        /// <param name="seq">Enumerable to memoize</param>
        /// <returns>IEnumerable with caching properties</returns>
        public static Seq<T> memo<T>(IEnumerable<T> seq) =>
            toSeq(seq);

        /// <summary>
        /// Used internally by the memo function.  It wraps a concurrent dictionary that has 
        /// its value objects wrapped in a WeakReference<OnFinalise<...>>
        /// The OnFinalise type is a private class within WeakDict and does nothing but hold
        /// the value and an Action to call when its finalised.  So when the WeakReference is
        /// collected by the GC, it forces the finaliser to be called on the OnFinalise object,
        /// which in turn executes the action which removes it from the ConcurrentDictionary.  
        /// That means that both the key and value are collected when the GC fires rather than 
        /// just the value.  Mitigates memory leak of keys.
        /// </summary>
        private class WeakDict<T, R>
        {
            private class OnFinalise<V>
            {
                public readonly V Value;
                readonly Action onFinalise;

                public OnFinalise(Action onFinalise, V value)
                {
                    this.Value = value;
                    this.onFinalise = onFinalise;
                }

                ~OnFinalise() =>
                    onFinalise?.Invoke();
            }

            ConcurrentDictionary<T, WeakReference<OnFinalise<R>>> dict = new ConcurrentDictionary<T, WeakReference<OnFinalise<R>>>();

            private WeakReference<OnFinalise<R>> NewRef(T key, Func<T, R> addFunc) =>
                new WeakReference<OnFinalise<R>>(
                    new OnFinalise<R>(() =>
                        {
                            WeakReference<OnFinalise<R>> ignore = null;
                            dict?.TryRemove(key, out ignore);
                        },
                        addFunc(key)));

            public bool TryGetValue(T key, out R value)
            {
                if(dict.TryGetValue(key, out var res) && res.TryGetTarget(out var target))
                {
                    value = target.Value;
                    return true;
                }
                else
                {
                    value = default;
                    return false;
                }
            }

            public R GetOrAdd(T key, Func<T, R> addFunc)
            {
                var res = dict.GetOrAdd(key, _ => NewRef(key, addFunc));

                OnFinalise<R> target = null;
                if (res.TryGetTarget(out target))
                {
                    return target.Value;
                }
                else
                {
                    var upd = NewRef(key, addFunc);
                    res = dict.AddOrUpdate(key, upd, (_, __) => upd);
                    if (res.TryGetTarget(out target))
                    {
                        return target.Value;
                    }
                    else
                    {
                        // This is a best guess of why the target can't be got.
                        // It might not be the best approach, perhaps a retry, or a 
                        // better/more-descriptive exception.
                        throw new OutOfMemoryException();
                    }
                }
            }

            public bool TryRemove(T key)
            {
                WeakReference<OnFinalise<R>> ignore = null;
                return dict.TryRemove(key, out ignore);
            }
        }
    }


    public static class MemoExtensions
    {
        /// <summary>
        /// Returns a Func<T> that wraps func.  The first
        /// call to the resulting Func<T> will cache the result.
        /// Subsequent calls return the cached item.
        /// </summary>
        public static Func<T> Memo<T>(this Func<T> func) =>
            LanguageExt.Prelude.memo(func);

        /// <summary>
        /// Returns a Func<T,R> that wraps func.  Each time the resulting
        /// Func<T,R> is called with a new value, its result is memoized (cached).
        /// Subsequent calls use the memoized value.  
        /// 
        /// Remarks: 
        ///     Thread-safe and memory-leak safe.  
        /// </summary>
        public static Func<T, R> Memo<T, R>(this Func<T, R> func) =>
            LanguageExt.Prelude.memo(func);

        /// <summary>
        /// Returns a Func<T,R> that wraps func.  Each time the resulting
        /// Func<T,R> is called with a new value, its result is memoized (cached).
        /// Subsequent calls use the memoized value.  
        /// 
        /// Remarks: 
        ///     No mechanism for freeing cached values and therefore can cause a
        ///     memory leak when holding onto the Func<T,R> reference.
        ///     Uses a ConcurrentDictionary for the cache and is thread-safe
        /// </summary>
        public static Func<T, R> MemoUnsafe<T, R>(this Func<T, R> func) =>
            LanguageExt.Prelude.memoUnsafe(func);

        /// <summary>
        /// Enumerable memoization.  As an enumerable is enumerated each item is retained
        /// in an internal list, so that future evalation of the enumerable isn't done. 
        /// Only items not seen before are evaluated.  
        /// 
        /// This minimises one of the major problems with the IEnumerable / yield return 
        /// pattern by causing at-most-once evaluation of each item.  
        /// 
        /// Use the IEnumerable extension method Memo for convenience.
        /// </summary>
        /// <remarks>
        /// Although this allows efficient lazy evaluation, it does come at a memory cost.
        /// Each item is cached internally, so this method doesn't allow for evaluation of
        /// infinite sequences.
        /// </remarks>
        /// <param name="seq">Enumerable to memoize</param>
        /// <returns>IEnumerable with caching properties</returns>
        public static IEnumerable<T> Memo<T>(this IEnumerable<T> seq) =>
            LanguageExt.Prelude.memo(seq);
    }
}
