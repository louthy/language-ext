`IteratorAsync<A>` is a functional-wrapper for `IAsyncEnumerator<A>`.  The abstraction leaks a little, so it's worth 
understanding how it works by reading the details below.  On the whole it behaves like an immutable stream 
that caches values as it goes, but there's some footguns that you should be aware of so that they can be 
avoided.

## Problem: `IAsyncEnumerator<A>`

* It is mutable which means using it in immutable data-structures is problematic. 
* If you pass an `IAsyncEnumerator` reference to two threads, each thread can 
call `MoveNext`and it will move the enumeration position for other thread, or even worse, move past the end
of the sequence due to race conditions.
* Enumerators start before the first item and use a complicated mechanism for accessing and testing the validity of
the element value.

_Nobody in their right mind would invent an interface like `IAsyncEnumerator<A>` today._

## Solution: `IteratorAsync<A>`

`IteratorAsync<A>` still uses `IAsyncEnumerator<A>` internally, but it makes it thread-safe and functional.  From the outside
the type acts and works exactly like any other immutable sequence, but internally it does some quite complex
processing to achieve this with an `IAsyncEnumerator<A>` reference.

> You may say "Why not just drop `IAsyncEnumerator<A>`?" - which is a completely valid position to hold. Unfortunately, 
> `IAsyncEnumerable` and `IAsyncEnumerator` are baked into the CPS state-machine that is used for `yield return` and 
> `yield break`.  So, we don't get to ignore those types, and instead we need to make them play nice.

`IAsyncEnumerable<A>` has a method called `GetAsyncEnumerator()` which is used to access an `IAsyncEnumerator<A>`.  A new extension
method is available called `GetIteratorAsync()`, this will yield an `IteratorAsync<A>`.
