This a suite of [very high-performance immutable-collections](https://github.com/louthy/language-ext/blob/master/Performance.md).

* For lists you should always prefer to use `Seq<A>` - it is about 10x faster than `Lst<A>`.  The only reason you'd pick 
  `Lst<A>` is if you needed to do inserts into the middle of the list: `Seq<A>` doesn't allow this (it only allows prepending or 
  appending), as it would be a performance hit.
  `Seq<A>` is backed by an array, and so it has exceptional memory locality, `Lst<A>` is an AVL tree to allow for efficient  
  insertion, but suffers from poorer memory locality.
* For 'dictionaries' or maps as we prefer to call them, then `HashMap` is the fastest implemention you'll find in .NET-land.  It is 
  unsorted.  If you need a sorted dictionary, use `Map`.  `HashMap` uses the CHAMP data-structure, `Map` uses an AVL tree.
* The same goes for sets, prefer `HashSet` over `Set`, unless you need the set to be sorted.

You can construct the collection types using the constrctor functions in the `Prelude`:

    HashMap<string, int> hashSet = HashMap(("a", 1), ("b", 2), ("c", 3));
    HashSet<int> hashMap         = HashSet(1, 2, 3);
    Map<string, int> hashSet     = Map(("a", 1), ("b", 2), ("c", 3));
    Set<int> hashMap             = Set(1, 2, 3);
    Seq<int> list                = Seq(1, 2, 3);
    Lst<int> list                = List(1, 2, 3);
