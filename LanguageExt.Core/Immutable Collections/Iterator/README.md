`Iterator<A>` is a functional-wrapper for `IEnumerator<A>`.  The abstraction leaks a little, so it's worth 
understanding how it works by reading the details below.  On the whole it behaves like an immutable stream 
that caches values as it goes, but there's some footguns that you should be aware of so that they can be 
avoided.

## Problem: `IEnumerator<A>`

* It is mutable which means using it in immutable data-structures is problematic. 
* If you pass an `IEnumerator` reference to two threads, each thread can 
call `MoveNext`and it will move the enumeration position for other thread, or even worse, move past the end
of the sequence due to race conditions.
* Enumerators start before the first item and use a complicated mechanism for accessing and testing the validity of
the element value.

_Nobody in their right mind would invent an interface like `IEnumerator<A>` today._

## Solution: `Iterator<A>`

`Iterator<A>` still uses `IEnumerator<A>` internally, but it makes it thread-safe and functional.  From the outside
the type acts and works exactly like any other immutable sequence, but internally it does some quite complex
processing to achieve this with an `IEnumerator<A>` reference.

> You may say "Why not just drop `IEnumerator<A>`?" - which is a completely valid position to hold. Unfortunately, 
> `IEnumerable` and `IEnumerator` are baked into the CPS state-machine that is used for `yield return` and 
> `yield break`.  So, we don't get to ignore those types, and instead we need to make them play nice.

`IEnumerable<A>` has a method called `GetEnumerator()` which is used to access an `IEnumerator<A>`.  A new extension
method is available called `GetIterator()`, this will yield an `Iterator<A>`.

You can pattern-match an `Iterator<A>` like a functional 'cons' linked-list type:

```c#
static A Sum<A>(Iterator<A> iter) where A : INumber =>
    iter switch
    {
        Iterator<A>.Nil                 => A.Zero,
        Iterator<A>.Cons(var x, var xs) => x + Sum(xs),
    }
```
Or, use `IsEmpty` and `Head`:
```c#
static A Sum<A>(Iterator<A> iter) where A : INumber =>
    iter.IsEmpty
        ? A.Zero
        : iter.Head + Sum(iter.Tail);
```
Or, use built-in operators:
```c#
static A Sum<A>(Iterator<A> iter) where A : INumber =>
    iter.Fold(A.Zero, (s, x) => s + x);
```
Or, take an imperative approach:
```c#
static A Sum<A>(Iterator<A> iter) where A : INumber
{
    var total = A.Zero;
    while(!iter.IsEmpty)
    {
        total += iter.Head;
        iter = iter.Tail;
    }
    return total;
}
```

`Iterator<A>` is `abstract` and the first type returned from `GetIterator()` will be a `Iterator<A>.ConsFirst`, this type implements 
`Iterator<A>`.  The internal fields that `ConsFirst` contains are these:
```c#
IEnumerable<A> enumerable;
int firstAcquired;
Iterator<A>? firstValue;
```
So, you can see that it doesn't actually have an `IEnumerator<A>` as this point.

The two key properties of `Iterator<A>` are `Head` (for accessing the current item) and `Tail` (for accessing
the remaining items), so let's look at those for `ConsFirst`:
```c#
public override A Head =>
    First.Head;

public override Iterator<A> Tail =>
    First.Tail;
```
They both access `First`, which is:

```c#
Iterator<A> First
{
    get
    {
        if (firstAcquired == 2) return firstValue!;
        
        SpinWait sw = default;
        while (firstAcquired < 2)
        {
            if (Interlocked.CompareExchange(ref firstAcquired, 1, 0) == 0)
            {
                try
                {
                    var enumerator = enumerable.GetEnumerator();
                    if (enumerator.MoveNext())
                    {
                        firstValue = new ConsValueEnum(enumerator.Current, enumerator);
                    }
                    else
                    {
                        enumerator.Dispose();
                        firstValue = Nil.Default;
                    }

                    firstAcquired = 2;
                }
                catch (Exception)
                {
                    firstAcquired = 0;
                    throw;
                }
            }
            else
            {
                sw.SpinOnce();
            }
        }

        return firstValue!;
    }
}
```
This all looks quite complex, but you should be able to see that the `Interlocked.CompareExchange`
then-block is where the `IEnumerator` is created.  We then either set `firstValue` to a new `ConsValueEnum` with 
the head-item and the `enumerator` as arguments; or we set it to `Nil`.  

Upon success we set `firstAcquired` to `2`.  So, subsequent calls to `First` will just return `firstValue`.  This
locking technique without using locks is a way to efficiently protect the enumerator from race-conditions.

So, upon first access to either `Head` or `Tail` we launch the `IEnumerator` and cache the first item in the sequence. 
All subsequent access goes to `Head` or `Tail` on either `Nil` or `ConsValueEnum`.  We never touch the `IEnumerator`
again in `ConsFirst`.

The `Nil` implementation isn't so surprising:
```c#
public override A Head =>
    throw new InvalidOperationException("Nil iterator has no head");

public override Iterator<A> Tail =>
    this;
```
`ConsValueEnum` is where it gets interesting.  It has the following internal fields:
```c#
Exception? exception;
IEnumerator<A>? enumerator;
int tailAcquired;
Iterator<A>? tailValue;
```
It also has a `Head` property that is set in the constructor:
```c#
public override A Head { get; }
```
So, we can access the `Head` value at any time, but the `Tail` value isn't yet set:
```c#
public override Iterator<A> Tail
{
    get
    {
        if(tailAcquired == 2) return tailValue!;
        if(tailAcquired == 3) exception!.Rethrow();

        SpinWait sw = default;
        while (tailAcquired < 2)
        {
            if (Interlocked.CompareExchange(ref tailAcquired, 1, 0) == 0)
            {   
                try
                {
                    if (enumerator!.MoveNext())
                    {
                        tailValue = new ConsValueEnum(enumerator.Current, enumerator);
                    }
                    else
                    {
                        enumerator?.Dispose();
                        enumerator = null;
                        tailValue = Nil.Default;
                    }

                    tailAcquired = 2;
                }
                catch (Exception e)
                {
                    exception = e;
                    tailAcquired = 3;
                    throw;
                }
            }
            else
            {
                sw.SpinOnce();
            }
        }

        if(tailAcquired == 3) exception!.Rethrow();
        return tailValue!;
    }
}
```
This does a similar thing to `ConsFirst` of protecting a section with `Interlocked.CompareExchange`.  So, we can
only ever access the 'then' part [of that `if` statement] once.  In that block we `MoveNext` the `IEnumerator` 
which will either return `true` or `false`.  

If `true` then we create another `ConsValueEnum`, if `false` then we use `Nil`.  Whichever is created gets assigned
to `tailValue` and `tailAcquired` gets set to `2`.  That means subsequent calls to `Tail` will just return
`tailValue`.

That process continues for each item of the sequence until the `IEnumerator` runs out of items to yield.  The end
result is a linked-list of `ConsValueEnum` objects that have a `ConsFirst` object at the head of the linked-list.

So, `Iterator<A>` effectively caches the sequence as you go.  If you hold on to the head of the sequence then the
whole list may end up in memory at once.  This could be problematic when working with large lazy sequences or even
infinite sequences.

This for example is fine:
```c#
for(var iter = Naturals.GetIterator(); !iter.IsEmpty; iter = iter.Tail)
{
    Console.WriteLine(iter.Head);
}
```
Because the `iter` reference keeps getting updated in-place, meaning that nothing is holding on to the head-item in
the sequence, and so the garbage-collector can collect those unreferenced items.

Whereas this will cause memory-usage to grow and grow:
```c#
var start = Naturals.GetIterator();
for(var iter = start; !iter.IsEmpty; iter = iter.Tail)
{
    Console.WriteLine(iter.Head);
}
```
Because `start` is holding a reference to the first item, so it must hold a reference (indirectly) to every 
subsequent item.  Meaning the garbage-collector can't collect.

To get around this you can use `Clone`:
```c#
var start = Naturals.GetIterator();
for(var iter = start.Clone(); !iter.IsEmpty; iter = iter.Tail)
{
    Console.WriteLine(iter.Head);
}
```
This creates a new 'head' for the sequence and so `iter` is the only reference, meaning updates to `iter`
make the head elements free for garbage collection.

So, `Iterator<A>` is much, much more powerful than `IEnumerator<A>`.  It is mostly useful for immutable data-types 
that need to carry an `IEnumerator<A>`, but can't due it its limitations.  `Iterator<A>` has some limitations of its
own, but they are relatively easy to workaround, whereas that isn't the case with `IEnumerator<A>` (without writing
a type like `Iterator<A>`!).