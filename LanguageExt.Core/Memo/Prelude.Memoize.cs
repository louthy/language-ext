using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using LanguageExt.Traits;

namespace LanguageExt;

public static partial class Prelude
{
    /// <summary>
    /// Returns a `Memo〈A〉` that wraps a function.  The first
    /// call to the resulting `Memo〈A〉` will cache the result.
    /// Subsequent calls return the cached item.
    /// </summary>
    public static Memo<A> memo<A>(Func<A> f) =>
        new (f);

    /// <summary>
    /// Create a preloaded memo structure with a pure value.  
    /// </summary>
    public static Memo<A> memoPure<A>(A value) =>
        new (value);

    /// <summary>
    /// Returns a `Memo〈F, A〉` that wraps a function that yields a higher-kind `K〈F, A〉`.
    /// The first call to the resulting `Memo〈F, A〉` will cache the resulting `K〈F, A〉`.
    /// Subsequent calls return the cached item.
    /// </summary>
    /// <remarks>
    /// NOTE: This does not invoke the `K〈F, A〉` computation, it merely caches the construction
    /// of the `K〈F, A〉`. If `K〈F, A〉` itself is lazy, then it can be invoked multiple times;
    /// this memoisation won't affect that.
    /// </remarks>
    public static Memo<F, A> memoF<F, A>(Func<K<F, A>> f) =>
        new(f);

    /// <summary>
    /// Create a preloaded memo structure with a pure value
    /// </summary>
    public static Memo<F, A> memoPureF<F, A>(A value) 
        where F : Applicative<F> =>
        new (F.Pure(value));

    /// <summary>
    /// Create a preloaded memo structure.  
    /// </summary>
    public static Memo<F, A> memoF<F, A>(K<F, A> fa) 
        where F : Applicative<F> =>
        new (fa);
    
    /// <summary>
    /// Returns a `Func〈A, B〉` that wraps func.  Each time the resulting
    /// `Func〈A, B〉` is called with a new value, its result is memoized (cached).
    /// Subsequent calls use the memoized value.  
    /// 
    /// Remarks: 
    ///     Thread-safe and memory-leak safe.  
    /// </summary>
    public static Func<A, B> memo<A, B>(Func<A, B> func) where A : notnull
    {
        var cache   = new WeakDict<A, B> ();
        var syncMap = new ConcurrentDictionary<A, object>();

        return inp =>
               {
                   if(cache.TryGetValue(inp, out var x))
                   {
                       return x;
                   }
                   else
                   {
                       B   res;
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
    /// Returns a Func〈T, R〉 that wraps func.  Each time the resulting
    /// Func〈T, R〉 is called with a new value, its result is memoized (cached).
    /// Subsequent calls use the memoized value.  
    /// 
    /// Remarks: 
    ///     No mechanism for freeing cached values and therefore can cause a
    ///     memory leak when holding onto the Func〈T, R〉 reference.
    ///     Uses a ConcurrentDictionary for the cache and is thread-safe
    /// </summary>
    public static Func<T, R> memoUnsafe<T, R>(Func<T, R> func) where T : notnull
    {
        var cache   = new ConcurrentDictionary<T, R>();
        var syncMap = new ConcurrentDictionary<T, object>();
        return inp =>
               {
                   if (!cache.TryGetValue(inp, out var res))
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
    /// its value objects wrapped in a WeakReference〈OnFinalise〈...〉〉
    /// The OnFinalise type is a private class within WeakDict and does nothing but hold
    /// the value and an Action to call when its finalised.  So when the WeakReference is
    /// collected by the GC, it forces the finaliser to be called on the OnFinalise object,
    /// which in turn executes the action which removes it from the ConcurrentDictionary.  
    /// That means that both the key and value are collected when the GC fires rather than 
    /// just the value.  Mitigates memory leak of keys.
    /// </summary>
    private class WeakDict<T, R> where T : notnull
    {
        private class OnFinalise<V>(Action onFinalise, V value)
        {
            public readonly V Value = value;

            ~OnFinalise() =>
                onFinalise.Invoke();
        }

        ConcurrentDictionary<T, WeakReference<OnFinalise<R>>> dict = new ConcurrentDictionary<T, WeakReference<OnFinalise<R>>>();

        private WeakReference<OnFinalise<R>> NewRef(T key, Func<T, R> addFunc) =>
            new (new OnFinalise<R>(() => dict.TryRemove(key, out _), addFunc(key)));

        public bool TryGetValue(T key, out R value)
        {
            if(dict.TryGetValue(key, out var res) && res.TryGetTarget(out var target))
            {
                value = target.Value;
                return true;
            }
            else
            {
                value = default!;
                return false;
            }
        }

        public R GetOrAdd(T key, Func<T, R> addFunc)
        {
            var res = dict.GetOrAdd(key, _ => NewRef(key, addFunc));

            if (res.TryGetTarget(out var target))
            {
                return target.Value;
            }
            else
            {
                var upd = NewRef(key, addFunc);
                res = dict.AddOrUpdate(key, upd, (_, _) => upd);
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
    }
}
