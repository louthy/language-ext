using System;
using System.Collections.Generic;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using LanguageExt.TypeClasses;

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
        public static AtomHashMap<K, V> ToAtom<K, V>(this HashMap<K, V> self) =>
            new AtomHashMap<K, V>(self);
    }
}
