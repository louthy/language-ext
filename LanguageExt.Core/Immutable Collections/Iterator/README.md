Iterators are lazy, immutable sequences that can be consumed one item at a time.  They are functionally pure
unlike `IEnumerator` in the .NET framework. 

All the language-ext collection types are written to support `Iterator` as a first-class citizen (by implementing 
the `IterableK` and/or the `IterableBackK` trait). That means you don't have to worry about the mutability problems of 
`IEnumerator`. You can just use them as you would any other collection type, and you can hold on to references 
mid-iteration, pass those references around to different threads, or anything you like, in the same way as any 
regular immutable data-types.

The only time you need to be careful is if you construct an `Iterator` from a regular `IEnumerable`.  The `Iterable` 
reference you get back, from the constructor, is completely safe to pass around and use as normal.  But as soon as you 
try to consume the first element, the wrapped `IEnumerable` will have to generate an `IEnumerator`, which is mutable and 
not guaranteed to be thread-safe.

> In that situation you need to make sure you're not passing intermediate `Iterator` values around, and instead you 
> simply consume the collection in one pass and `Dispose`. This is the normal usage of enumerators, so it's not a big constraint, but 
> it's worth understanding the limitation.

You may say "Why not just drop `IEnumerator<A>`?" - which is a completely valid position to hold. Unfortunately, 
`IEnumerable` and `IEnumerator` are baked into the CPS state-machine that is used for `yield return` and 
`yield break`.  So, we don't get to ignore those types, and instead we need to try our best to make them play 
nice. 

You can pattern-match an `Iterator<A>` like a functional _cons-style_ linked-list type:

```c#
static A Sum<A>(Iterator<A> iter) where A : INumber =>
    iter is (Exist<A> head, var tail)
        ? head.Value + Sum(tail) 
        : A.Zero;
```
This uses the `Iterator` deconstructor to extract the head and tail of the sequence.  The head is of type `Head<A>`.

`Head<A>` has two subtypes: `Exist<A>(A Value)` which represents an existent value and `Nil<A>` represents a 
non-existent value.  So, if the matched head is of value of type `Exist<A>` then you have consumed the next item in the sequence, if 
the head value is `Nil<A>` then you're at the end of the sequence.   

You can also take an imperative approach:
```c#
static A Sum<A>(Iterator<A> iter) where A : INumber
{
    var total = A.Zero;
    for(var i = iter; i is (Exist<A> head, var tail); i = tail)
    {
        total += head.Value;
    }
    return total;
}
```
