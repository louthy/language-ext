using System;
using System.Collections.Concurrent;

namespace LanguageExt
{
    public static partial class Prelude
    {
        /// <summary>
        /// Returns a Func<T> that wraps func.  The first
        /// call to the resulting Func<T> will cache the result.
        /// Subsequent calls return the cached item.
        /// </summary>
        public static Func<T> memo<T>(this Func<T> func)
        {
            var value = new Lazy<T>(func, true);
            return () => value.Value;
        }

#if !DOT_NET_CORE
        /// <summary>
        /// Returns a Func<T,R> that wraps func.  Each time the resulting
        /// Func<T,R> is called with a new value, its result is memoized (cached).
        /// Subsequent calls use the memoized value.  
        /// 
        /// Remarks: 
        ///     Thread-safe and memory-leak safe.  
        /// </summary>
        public static Func<T, R> memo<T, R>(this Func<T, R> func)
        {
            var cache = new WeakDict<T, R>();
            var syncMap = new ConcurrentDictionary<T, object>();

            return inp =>
                matchUnsafe(cache.TryGetValue(inp),
                    Some: x => x,
                    None: () =>
                    {
                        R res;
                        var sync = syncMap.GetOrAdd(inp, new object());
                        lock (sync)
                        {
                            res = cache.GetOrAdd(inp, func);
                        }
                        syncMap.TryRemove(inp, out sync);
                        return res;
                    });
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
        public static Func<T, R> memoUnsafe<T, R>(this Func<T, R> func)
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
        /// Used internally by the memo function.  It wraps a concurrent dictionary that has 
        /// its value objects wrapped in a WeakReference<OnFinalise<...>>
        /// The OnFinalise type is a private class within WeakDict and does nothing but hold
        /// the value and an Action to call when its finalised.  So when the WeakReference is
        /// collected by the GC, it forces the finaliser to be called on the OnFinalise object,
        /// which in turn executes the action which renmoves it from the ConcurrentDictionary.  
        /// That means that both the key and value are collected when the GC fires rather than 
        /// just the value.  That should mitigate a memory leak of keys.
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

                ~OnFinalise()
                {
                    onFinalise();
                }
            }

            ConcurrentDictionary<T, WeakReference<OnFinalise<R>>> dict = new ConcurrentDictionary<T, WeakReference<OnFinalise<R>>>();

            private WeakReference<OnFinalise<R>> NewRef(T key, Func<T, R> addFunc) =>
                new WeakReference<OnFinalise<R>>(
                    new OnFinalise<R>(() =>
                        {
                            WeakReference<OnFinalise<R>> ignore = null;
                            dict.TryRemove(key, out ignore);
                        },
                        addFunc(key)));

            public OptionUnsafe<R> TryGetValue(T key)
            {
                WeakReference<OnFinalise<R>> res = null;
                OnFinalise<R> target = null;
                return dict.TryGetValue(key, out res)
                    ? res.TryGetTarget(out target)
                        ? SomeUnsafe(target.Value)
                        : None
                    : None;
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
#endif
    }
}
