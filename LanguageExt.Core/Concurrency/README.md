We prefer to work with immutable types in functional-programming.  However, it's not always possible, and sometimes we
need some shared mutable state.  With the immutable types in this library you'd need to protect the updates with locks:

    // Some global
    static HashSet<int> set = HashSet(1, 2, 3);
    static object sync = new();

    lock(sync)
    {
        set = set.Add(4);
    }

This in unsatisfactory, and so this module is all about lock-free atomic operations.  `Atom` allows you to protect any
value.  `AtomHashMap` and `AtomSeq` are `HashMap` and `Seq` wrapped up into a lock-free mutable structure.  Snapshots
of those are free!  The above code can be written:

    static AtomHashSet<int> set = AtomHashSet(1, 2, 3);
    set.Add(4);

Finally, there's the Software Transactional Memory (STM) system.  Which allows for transactional changes to multiple 
`Ref` values.  `Ref` just wrap up access to a value, and allows the state changes to be tracked by the `STM`.

 See the [concurrency section](https://github.com/louthy/language-ext/wiki/Concurrency) of the wiki for more info.
