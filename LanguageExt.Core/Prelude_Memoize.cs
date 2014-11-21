using System;
using System.Collections.Concurrent;
using TvdP.Collections;

namespace LanguageExt
{
    /// <summary>
    /// Usage:  Add 'using LanguageExt.Prelude' to your code.
    /// </summary>
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

        /// <summary>
        /// Returns a Func<T,R> that wraps func.  Each time the resulting
        /// Func<T,R> is called with a new value, its result is memoized (cached).
        /// Subsequent calls use the memoized value.  
        /// 
        /// Remarks: 
        ///     Thread-safe and memory-leak safe.  
        ///     R is limited to reference types.
        /// </summary>
        public static Func<T, R> memo<T, R>(this Func<T, R> func) where R : class
        {
            var cache = new WeakDictionary<T, R>();
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
        /// Returns a Func<T,R> that wraps func.  Each time the resulting
        /// Func<T,R> is called with a new value, its result is memoized (cached).
        /// Subsequent calls use the memoized value.  
        /// 
        /// Remarks: 
        ///     No mechanism for freeing cached values and can therefore causes a
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
    }
}
