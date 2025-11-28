using System;
using System.Collections.Generic;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt;

public static partial class MemoExtensions
{
    extension<FF, A>(Memo<K<FF, A>> self)
    {
        public Memo<FF, A> Lift() =>
            (self.acquire, self.state) switch
            {
                (null, 2) => new Memo<FF, A>(self.acquire, self.value),
                (null, _) => new Memo<FF, A>(self.value!),
                _         => new Memo<FF, A>(self.acquire)
            };
    }
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Functor extensions
    //

    extension<FF, A>(Memo<FF, A> self)
    {
        public Memo<FF, B> MapM<B>(Func<K<FF, A>, K<FF, B>> f) =>
            new (() => f(self.Value));
    }
    
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //
    // Functor extensions
    //

    extension<F, G, A>(Memo<F, A> self)
        where F : Natural<F, G>
    {
        public Memo<G, A> Transform() =>
            new (() => F.Transform(self.Value));
    }
    
    /// <summary>
    /// Returns a Func〈T〉 that wraps func.  The first
    /// call to the resulting Func〈T〉 will cache the result.
    /// Subsequent calls return the cached item.
    /// </summary>
    public static Memo<T> Memo<T>(this Func<T> func) =>
        memo(func);

    /// <summary>
    /// Returns a Func〈T, R〉 that wraps func.  Each time the resulting
    /// Func〈T, R〉 is called with a new value, its result is memoized (cached).
    /// Subsequent calls use the memoized value.  
    /// 
    /// Remarks: 
    ///     Thread-safe and memory-leak safe.  
    /// </summary>
    public static Func<T, R> Memo<T, R>(this Func<T, R> func) where T : notnull =>
        memo(func);

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
    public static Func<T, R> MemoUnsafe<T, R>(this Func<T, R> func) where T : notnull =>
        Prelude.memoUnsafe(func);

    /// <summary>
    /// Enumerable memoization.  As an enumerable is enumerated each item is retained
    /// in an internal list, so that future evaluation of the enumerable isn't done. 
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
        Prelude.memo(seq);
}
