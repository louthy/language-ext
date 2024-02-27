using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt
{
    /// <summary>
    /// Atoms provide a way to manage shared, synchronous, independent state without 
    /// locks.  `AtomHashMap` wraps the language-ext `HashMap`, and makes sure all operations are atomic and thread-safe
    /// without resorting to locking
    /// </summary>
    /// <remarks>
    /// See the [concurrency section](https://github.com/louthy/language-ext/wiki/Concurrency) of the wiki for more info.
    /// </remarks>
    public static partial class AtomHashMap
    {
        public static AtomHashMap<EqK, K, V> ToAtom<EqK, K, V>(this HashMap<EqK, K, V> self) where EqK : Eq<K> =>
            new AtomHashMap<EqK, K, V>(self);
    }
}
