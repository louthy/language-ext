# Version 2 Release Notes

Version 2.0 of Language-Ext is now in released.  This is a major overhaul of every type in the system.  I have also broken out the `LanguageExt.Process` actor system into its own repo, it is now named *Echo*, so if you're using that you should [head over to the repo](https://github.com/louthy/echo-process) and follow that.  It's still in alpha - it's feature complete, it just needs more testing - so it's lagging behind at the moment.  If you're using both lang-ext Core and the Echo Process system then wait until the Echo Process system is released before migrating to the new Core.

Version 2.0 of Language-Ext actually just started out as a branch where I was trying out a new technique for doing ad-hoc polymorphism in C# (think somewhere between Haskell typeclasses and Scala implicits).  

I didn't expect it to lead to an entire re-write.  So a word of warning, there are many areas that I know will be breaking changes, but some I don't.  Breaking changes will 99% of the time be compile time errors (rather than changes in behaviour that silently affect your code).  So although I don't expect any major issues, For any large projects I would put aside an afternoon to fix up any compilation breakages.  

Often the breakages are for things like rectifying naming inconsistencies (for example some bi-map functions were named `Map`, some named `BiMap`, they're all now `BiMap`), another example is that all collection types (`Lst`, `Map`, etc.) are now structs.  So any code that does this will fail to compile:
```c
    Map<int, string> x = null;
```
The transformer extensions have been overhauled too (they provided overloads for nested monadic types, `Option<Lst<A>>` for example).  If you were cheating trying to get directly at values by calling `Lift` or `LiftUnsafe`, well, now you can't.  It was a bad idea that was primarily to make the old transformer types work.  So they're gone.  

The overloads of `Select` and `SelectMany` are more restricted now, because combining different monadic types could lead to some type resolution issues with the compiler.  You will now need to lift your types into the context of the LINQ expression (there are now lots of extensions to do that: `ToOption`, `ToTry`, etc.)

> For the problems you will inevitablity have with upgrading to language-ext 2.0, you will also have an enormous amount of new benefits and possibilities.  
My overriding goal with this library is to try and provide a safer environment in which to write C#.  Version 1 was mostly trying to protect the programmer from null and mutable state.  Version 2 is very much focussed on improving our lot when implementing abstract types.  

Inheritance based polymorphism is pretty much accepted to be the worst performer in the polymorphic world.  Our other option is parametric polymorphism (generics).  With this release I have facilitated [ad-hoc polymorphism](https://en.wikipedia.org/wiki/Ad_hoc_polymorphism) with a little known technique in C#.  

So for the first time it's possible to write numeric methods once for all numeric types or do structural equality testing that you can rely on. 

Also there is support for the much more difficult higher-order polymorphic types like `Monad<MA, A>`.  LanguageExt 2.0 provides a fully type-safe and efficient approach to working with higher order types.  So yes, you can now write functions that take monads, or functors, or applicatives, and return specialised values (rather than abstract or dynamic values).  Instead of writing a function that takes an `Option<A>`, you can write one that takes any monadic type, bind them, join them, map them, and return the concrete type that you pushed in.

Of course without compiler or runtime support for higher-order generics some hoops need to be jumped through (and I'm sure there will be some Haskell purist losing their shit over the approach).  But at no point is the integrity of your types affected.  Often the technique requires quite a large amount of generic argument typing, but if you want to write the super generic code, it's now possible.  I don't know of any other library that provides this functionality.

This has allowed the transformer extensions to become more powerful too (because the code generator that emits them can now use the type-class/instance system).  The `Writer` monad can now work with any output type (as long as it has a `Monoid` instance), so it's not limited to telling its output to an `IEnumerable`, it can be a `Lst`, a `string`, an `int`, or whatever `Monoid` you specify.

> Personally I find this very elegant and exciting.  It has so much potential, but many will be put off by the amount of generic args typing they need to do.  If anybody from the Rosyln team is reading this, please for the love of god help out with the issues around constraints and excessive specifying of generic arguments.  The power is here, but needs more support.

Scroll down to the section on Ad-hoc polymorphism for more details.

## Documentation

[Full API documentation can be found here](https://louthy.github.io/language-ext/index.htm)

## Bug fixes

* Fix for `Lst.RemoveAt(index)` - certain tree arrangements caused this function to fail
* Fix for `HSet` (now `HashSet`) constructor bug - constructing with an enumerable always failed

## New features - LanguageExt.Core

### New collection types:

 Type                                       | Description
--------------------------------------------|--------------
`Seq<A>`                                    | Cons-like, singly-linked list
`HashSet<A>`                                | Ordering is done by `GetHashCode()`.  Existence testing is with `EqualityComparer<A>.Default.Equals(a,b)`
`HashMap<A, B>`                             | Ordering is done by `GetHashCode()`.  Existence testing is with `EqualityComparer<A>.Default.Equals(a,b)`
`HashSet<EqA, A> where EqA : struct, Eq<A>` | Ordering is done by `GetHashCode()`.  Existence testing is with `default(EqA).Equals(a,b)`
`HashMap<EqA, A, B>`                        | Ordering is done by `GetHashCode()`.  Existence testing is with `default(EqA).Equals(a,b)`
`Set<OrdA, A> where OrdA : struct, Ord<A>`  | Ordering is done by `default(OrdA).Compare(a,b)`.  Existence testing is with `default(OrdA).Equals(a,b)`
`Map<EqA, A, B>`                            | Ordering is done by `default(OrdA).Compare(a,b)`.  Existence testing is with `default(OrdA).Equals(a,b)`
`Arr<A>`                                    | Immutable array.  Has the same access speed as the built-in array type, but with immutable cells.  Modification is expensive, due to the entire array being copied per operation (although for very small arrays this would be more efficient than `Lst<T>` or `Set<T>`).
`Lst<PredList, A> where PredList : struct, Pred<ListInfo>` | This allows lists to run a predicate on the `Count` property of the list after construction.  
`Lst<PredList, PredItem, A> where PredItem : struct, Pred<A>` | This allows lists to run a predicate on the `Count` property of the list after construction and on items as they're being added to the list.  

As you can see above there are new type-safe key versions of `Set`, `HashSet`, `Map`, and `HashMap`.  Imagine you want to sort the value of a set of strings in a case-insensitive way (without losing information by calling `value.ToLower()`).
```c#
    var map = Set<TStringOrdinalIgnoreCase, string>(...)
```
The resulting type would be incompatible with:
```c#
    Set<TString, string>, or Set<TStringOrdinal, string>
```
And is therefore more type-safe than just using Set<string>.  [Examples](https://github.com/louthy/language-ext/blob/type-classes/LanguageExt.Tests/SetTests.cs)

The two new predicate versions of `Lst` allow for properties of the list to travel with the type.  So for example this shows how you can enforce a list to be non-empty:
```c#
    public int Product(Lst<NonEmpty, int> list) =>
        list.Fold(1, (s, x) => s * x);
```
There are implicit conversion operators between `Lst<A>` and `Lst<PredList, A>`, and between `Lst<A>` and `Lst<PredList, PredItem, A>`.  They don't need to reallocate the collection, but converting to a more constrained type will cause the validation to run.  This is very light for constructing `Lst<PredList, A>`, but will cause every item in the list to be validated for `Lst<PredList, PredItem, A>`.

And so it's possible to do this:
```c#
    Lst<int> list = List<int>();

    var res = Product(list);  // ArgumentOutOfRangeException
```
That will throw an `ArgumentOutOfRangeException` because the list is empty.  Whereas this is fine:
```c#
    Lst<int> list = List<int>(1, 2, 3, 4, 5);

    var res = Product(list); // 120
```
To construct the predicate list types directly, call:
```c#
    Lst<NonEmpty, int> list = List<NonEmpty, int>(1, 2, 3, 4, 5);
```
The second type of predicate `Lst` is `Lst<PredList, PredItem, A>`.  `PredItem` is a predicate that's run against every item being added to the list.  Once the item is in the list it won't be checked again (because it's an immutable list).

For example, this is a `Lst` that can't be empty and won't accept `null` items.
```c#
    var x = List<NonEmpty, NonNullItems<string>, string>("1", "2", "3");
```
Obviously declaring types like this gets quite bulky quite quickly.  So only using them for method arguments is definitely a good approach:
```c#
    public string Divify(Lst<NonEmpty, NonNullItems<string>, string> items) =>
        String.Join(items.Map(x => $"<div>{x}</div>"));
```
Then `Divify` can be invoked thus:
```c#
    var res = Divify(List("1", "2", "3")); 

    // "<div>1</div><div>2</div><div>3</div>"
```
But as mentioned above, the implicit conversion from `Lst<string>` to `Lst<NonEmpty, NonNullItems<string>, string>` will run the `NonNullItems<string>` predicate for each item in the `Lst`.

Built-in are some standard `Pred<ListInfo>` implementations:

* `AnySize` - Always succeeds
* `CountRange<MIN, MAX>` - Limits the `Count` to be >= MIN and <= MAX
* `MaxCount<MAX>` - As above but with no lower bound
* `NonEmpty` - List must have at least one item

And by default there are lots of `Pred<A>` implementations.  See the `NewType` discussion later.

### Seq<A>

A new feature is the type [`Seq<A>`](https://github.com/louthy/language-ext/tree/master/LanguageExt.Core/DataTypes/Seq) which derives from [`ISeq<A>`](https://github.com/louthy/language-ext/blob/master/LanguageExt.Core/DataTypes/Seq/ISeq.cs), which in turn is an `IEnumerable<A>`.

It works very much like `cons` in functional languages (although not a real cons, as that's not a workable solution in C#).  It's a singly-linked list, with two key properties: `Head` which is the item at the head of the sequence, and `Tail` which is the rest of the sequence.  You can convert any existing collection type (as well as `IEnumerable` types) to a `Seq<A>` by calling the `Seq` constructor:
```c#
    var seq1 = Seq(List(1, 2, 3, 4, 5));    // Lst<A> -> Seq<A>
    var seq2 = Seq(Arr(1, 2, 3, 4, 5));     // Arr<A> -> Seq<A>
    var seq3 = Seq(new [] {1, 2, 3, 4, 5}); // A[] -> Seq<A>
    ...    
```
As well as construct them directly:
```c#
    var seq1 = Seq(1, 2, 3, 4, 5);

    var seq2 = 1.Cons(2.Cons(3.Cons(4.Cons(5.Cons(Empty)))));
```
In practice if you're using `Cons` you don't need to provide `Empty`:
```c#
    var seq = 1.Cons();   // Creates a sequence with one item in
```
The primary benefits are:
* Immutable
* Thread-safe
* Much lighter weight than using `Lst<A>` (although `Lst<A>` is very efficient, it is still an AVL tree behind the scenes)
* Maintains the original type for `Lst<A>`, `Arr<A>`, and arrays.  So if you construct a `Seq` from one of those types, the `Seq` then gains extra powers for random lookups (`Skip`), and length queries (`Count`).
* If you construct a `Seq` with an `IEnumerable` then it maintains its laziness, but also __guarantees that each item in the original `IEnumerable` is only ever enumerated once.__
* Obviously easier to type `Seq` than `IEnumerable`!
* `Count` works by default for all non-`IEnumerable` sources.  If you construct with an `IEnumerable` then `Count` will only cause a single evaluation of the underlying `IEnumerable` (and subsequent access to the seq for anything else doesn't cause additional evaluation)
* Efficient pattern matching on the `Seq`, which again doesn't cause multiple evaluations of the underling collection.

`Seq` has a bigger interface than `IEnumerable` which allows for various bespoke optimisations depending on the underlying collection; which is especially powerful for the LINQ operators like `Skip`, `Take`, `TakeWhile`, 

```c#
    public interface ISeq<A> : 
        IEnumerable<A>, 
        IEquatable<ISeq<A>>, 
        IComparable<ISeq<A>>
    {
        /// <summary>
        /// Head of the sequence
        /// </summary>
        A Head { get; }

        /// <summary>
        /// Head of the sequence
        /// </summary>
        Option<A> HeadOrNone();

        /// <summary>
        /// Tail of the sequence
        /// </summary>
        Seq<A> Tail { get; }

        /// <summary>
        /// True if this cons node is the Empty node
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Returns the number of items in the sequence
        /// </summary>
        /// <returns>Number of items in the sequence</returns>
        int Count { get; }

        /// <summary>
        /// Match empty sequence, or multi-item sequence
        /// </summary>
        /// <typeparam name="B">Return value type</typeparam>
        /// <param name="Empty">Match for an empty list</param>
        /// <param name="Tail">Match for a non-empty</param>
        /// <returns>Result of match function invoked</returns>
        B Match<B>(
            Func<B> Empty,
            Func<A, Seq<A>, B> Tail);

        /// <summary>
        /// Match empty sequence, or one item sequence, or multi-item sequence
        /// </summary>
        /// <typeparam name="B">Return value type</typeparam>
        /// <param name="Empty">Match for an empty list</param>
        /// <param name="Tail">Match for a non-empty</param>
        /// <returns>Result of match function invoked</returns>
        B Match<B>(
            Func<B> Empty,
            Func<A, B> Head,
            Func<A, Seq<A>, B> Tail);

        /// <summary>
        /// Match empty sequence, or multi-item sequence
        /// </summary>
        /// <typeparam name="B">Return value type</typeparam>
        /// <param name="Empty">Match for an empty list</param>
        /// <param name="Seq">Match for a non-empty</param>
        /// <returns>Result of match function invoked</returns>
        B Match<B>(
            Func<B> Empty,
            Func<Seq<A>, B> Seq);

        /// <summary>
        /// Match empty sequence, or one item sequence, or multi-item sequence
        /// </summary>
        /// <typeparam name="B">Return value type</typeparam>
        /// <param name="Empty">Match for an empty list</param>
        /// <param name="Tail">Match for a non-empty</param>
        /// <returns>Result of match function invoked</returns>
        B Match<B>(
            Func<B> Empty,
            Func<A, B> Head,
            Func<Seq<A>, B> Tail);

        /// <summary>
        /// Map the sequence using the function provided
        /// </summary>
        /// <typeparam name="B"></typeparam>
        /// <param name="f">Mapping function</param>
        /// <returns>Mapped sequence</returns>
        Seq<B> Map<B>(Func<A, B> f);

        /// <summary>
        /// Map the sequence using the function provided
        /// </summary>
        /// <typeparam name="B"></typeparam>
        /// <param name="f">Mapping function</param>
        /// <returns>Mapped sequence</returns>
        Seq<B> Select<B>(Func<A, B> f);

        /// <summary>
        /// Filter the items in the sequence
        /// </summary>
        /// <param name="f">Predicate to apply to the items</param>
        /// <returns>Filtered sequence</returns>
        Seq<A> Filter(Func<A, bool> f);

        /// <summary>
        /// Filter the items in the sequence
        /// </summary>
        /// <param name="f">Predicate to apply to the items</param>
        /// <returns>Filtered sequence</returns>
        Seq<A> Where(Func<A, bool> f);

        /// <summary>
        /// Monadic bind (flatmap) of the sequence
        /// </summary>
        /// <typeparam name="B">Bound return value type</typeparam>
        /// <param name="f">Bind function</param>
        /// <returns>Flatmapped sequence</returns>
        Seq<B> Bind<B>(Func<A, Seq<B>> f);

        /// <summary>
        /// Monadic bind (flatmap) of the sequence
        /// </summary>
        /// <typeparam name="B">Bound return value type</typeparam>
        /// <param name="bind">Bind function</param>
        /// <returns>Flatmapped sequence</returns>
        Seq<C> SelectMany<B, C>(Func<A, Seq<B>> bind, Func<A, B, C> project);

        /// <summary>
        /// Fold the sequence from the first item to the last
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="f">Fold function</param>
        /// <returns>Aggregated state</returns>
        S Fold<S>(S state, Func<S, A, S> f);

        /// <summary>
        /// Fold the sequence from the last item to the first
        /// </summary>
        /// <typeparam name="S">State type</typeparam>
        /// <param name="state">Initial state</param>
        /// <param name="f">Fold function</param>
        /// <returns>Aggregated state</returns>
        S FoldBack<S>(S state, Func<S, A, S> f);

        /// <summary>
        /// Returns true if the supplied predicate returns true for any
        /// item in the sequence.  False otherwise.
        /// </summary>
        /// <param name="f">Predicate to apply</param>
        /// <returns>True if the supplied predicate returns true for any
        /// item in the sequence.  False otherwise.</returns>
        bool Exists(Func<A, bool> f);

        /// <summary>
        /// Returns true if the supplied predicate returns true for all
        /// items in the sequence.  False otherwise.  If there is an 
        /// empty sequence then true is returned.
        /// </summary>
        /// <param name="f">Predicate to apply</param>
        /// <returns>True if the supplied predicate returns true for all
        /// items in the sequence.  False otherwise.  If there is an 
        /// empty sequence then true is returned.</returns>
        bool ForAll(Func<A, bool> f);

        /// <summary>
        /// Skip count items
        /// </summary>
        Seq<A> Skip(int count);

        /// <summary>
        /// Take count items
        /// </summary>
        Seq<A> Take(int count);

        /// <summary>
        /// Iterate the sequence, yielding items if they match the predicate 
        /// provided, and stopping as soon as one doesn't
        /// </summary>
        /// <returns>A new sequence with the first items that match the 
        /// predicate</returns>
        Seq<A> TakeWhile(Func<A, bool> pred);

        /// <summary>
        /// Iterate the sequence, yielding items if they match the predicate 
        /// provided, and stopping as soon as one doesn't.  An index value is 
        /// also provided to the predicate function.
        /// </summary>
        /// <returns>A new sequence with the first items that match the 
        /// predicate</returns>
        Seq<A> TakeWhile(Func<A, int, bool> pred);
    }
```

__Breaking changes__

* Previously there were functions in the `Prelude` called `seq` which took many types and turned them into `IEnumerable<A>`.  Now they're constructing `Seq<A>` I have renamed them to `Seq` in line with other constructor functions.  
* Where sensible in the rest of the API I have changed `AsEnumerable()` to return a `Seq<A>`.  This is for types which are unlikely to have large sequences (like `Option`, `Either`, `Try`, etc.);  You may find casting issues in those situations, but the types returned are obviously more useful, so it feels like a win.  Let me know if you have issues (All the major types have a `ToSeq()` method where appropriate, for convenience).
* In `LanguageExt.Parsec` any parsers that previously returned `IEnumerable<A>` now return `Seq<A>`; I noticed when using the `Parsec` library extensively that I would often use `ToArray()` or `Freeze()` to force strict evaluation of a `IEnumerable` result.  Now you don't have to, but you may see some casting issues.
* `Match` and `match` for `IEnumerable` previously allowed for matching up to six items in six separate lambdas.  They're now gone because I doubt anybody uses them, and they're slightly unwieldy.  Now they use `Seq` behind the scenes to remove any multiple evaluations during the matching process:
```c#
        static int Sum(Seq<int> seq) =>
            seq.Match(
                ()      => 0,
                x       => x,
                (x, xs) => x + Sum(xs));
```

### Non-nullable types: 

In the ongoing quest to make it safer to write C# code, these types are all now structs and therefore can't be `null`:
```c#
    Stck<A>, 
    Que<A>, 
    Lst<A>, 
    Map<A, B>, 
    Map<Ord, A, B>, 
    HashMap<A, B>, 
    HashMap<Eq, A, B>, 
    Set<A>, 
    Set<Ord, A>
    HashSet<A, B>, 
    HashSet<Eq, A, B>, 
    Que<A>, 
    Arr<A>
```
This means you can create a member field and not initialise it and everything will 'just work':

```c#
    static class Test
    {
        public static Map<string, int> Foo;
    }

    Assert.True(Test.Foo == Map.empty<string, int>());
    Assert.True(Test.Foo == default(Map<string, int>);
```
### Serialisation fixes

`Map`, `Lst`, `Set`, `Option`, `Either`, etc.  All have serialisers that work with Json.NET (finally).  

Examples: https://github.com/louthy/language-ext/blob/type-classes/LanguageExt.Tests/SerialisationTests.cs

### Lazy `Option<A>` and `OptionUnsafe<A>`

`Option<A>` and `OptionUnsafe<A>` have until now been strict types.  They can now support both strict and lazy behaviour in the same type.

For example:
```c#
    var option = from w in Some(_ => 5)
                 from x in Some(_ => 10)
                 from y in Some(_ => 15)
                 from z in Some(_ => 20)
                 select w + x + y + z;

    // At this point the w + x + y + z expression hasn't run
```
Any function that returns a concrete non-Option type will force the expression to be evaluated:
```c#
    var result = option.IfNone(0);   // 50
```
The result is memoised and therefore subsequent calls to `Match` or similar won't re-evaluate the expression.  The strict behaviour is unaltered:
```c#
    var option = from w in Some(5)
                 from x in Some(10)
                 from y in Some(15)
                 from z in Some(20)
                 select w + x + y + z;

    // At this point the w + x + y + z expression *has* run
```
If strict and lazy Options are combined, then the whole expression becomes lazy:
```c#
    var option = from w in Some(5)
                 from x in Some(_ => 10)  // lazy expression
                 from y in Some(15)
                 from z in Some(20)
                 select w + x + y + z;

    // At this point the w + x + y + z expression hasn't run
```
Note if you want to write functions that return lazy options, then you will need to wrap them in `Optional` or `Some`:
```c#
    Option<int> LazyOptionFoo() => Optional(_ => ... );
```

> Either and EitherUnsafe will have lazy functionality soon

### `NewType`

NewType has gained an extra generic argument.  So this:

```c#
    class Metres : NewType<double> { ... }
```

Becomes:
```c#
    class Metres : NewType<Metres, double>  { ... } 
```
That makes lots of functionality more type-safe for `NewType` derived types.  For example monadic and functor operators like `Select`, `Map`, `Bind`, `SelectMany` can now return `Metres` rather than `NewType<double>`.  Which is very important for the integrity of the type.

There is a variant that takes an additional generic argument `PRED`.  Which is constrained to be a `struct` of `Pred<A>`.  This is called in the base constructor:
```c#
    if (!default(PRED).True(value)) 
        throw new ArgumentOutOfRangeException(nameof(value), value);
```
So you should be able to see that this allows validation to be embedded into the type.  Here's an example from the new Process system client code:
```c#
    public class ClientConnectionId : NewType<ClientConnectionId, string, StrLen<I10, I100>>
    {
        public ClientConnectionId(string value) : base(value)
        { }
    }
```
`ClientConnectionId` is like a session token, and `StrLen<I10, I100>` means the string must be `10` to `100` chars long.  It is defined thus:
```c#
    public struct StrLen<NMin, NMax> : Pred<string>
            where NMin : struct, Const<int>
            where NMax : struct, Const<int>
    {
        [Pure]
        public bool True(string value) =>
            Range<int, TInt, NMin, NMax>.Is.True(value?.Length ?? 0);
    }
```
`NMin` and `NMax` are constrained to be `Const<int>`, and from the example above are `I10` and `I100`.  They're defined as:
```c#
    public struct I10 : Const<int> { public int Value => 10; }
    public struct I100 : Const<int> { public int Value => 100; }
```
You can define your own `struct` type that derives from `Pred<A>` to provide any common validation primatives that you like.  And defined constants by deriving from `Const<A>`.

These are the built in predicate types:

* `True<A>` - Always succeeds, no matter what's passed
* `False<A>` - Always fails, no matter what's passed
* `Equal<A, EQ, CONST>` - Value `A` must be equal to `CONST`.  `EQ` is a `Eq<A>` instance
* `Exists<A, Term1, ..., Term5>` - `Term1 - Term5` are predicates.  The test passes if `A` is true when applied to any of the terms.
* `ForAll<A, Term1, ..., Term5>` - `Term1 - Term5` are predicates.  The test passes if `A` is true when applied to all of the terms.
* `GreaterThan<A, ORD, CONST>` - Value `A` must be greater than `CONST`.  `ORD` is an `Ord<A>` instance.
* `GreaterOrEq<A, ORD, CONST>` - Value `A` must be greater than or equal to `CONST`.  `ORD` is an `Ord<A>` instance.
* `LessThan<A, ORD, CONST>` - Value `A` must be less than `CONST`.  `ORD` is an `Ord<A>` instance.
* `LessOrEq<A, ORD, CONST>` - Value `A` must be less than or equal to `CONST`.  `ORD` is an `Ord<A>` instance.
* `Range<A, ORD, MIN, MAX>` - Value `A` must be in the range `MIN` to `MAX`.  `ORD` is an `Ord<A>` instance.

Constants are in the `LanguageExt.ClassInstances.Const` namespace.  Integers are prefixed with `I`;  the most commonly used are already created: `0` to `256`, then powers of two and hundreds, thousands, etc.  `Double` constants are prefixed with `D`.  Only `D0`, `D1`, and `DNeg1` exist.  `Char` constants are `'A'- 'Z'`, `'a' - 'z'`, `'0' - '9'`, `ChSpace`, `ChTab`, `ChCR`, `ChLF`.

By embedding the validation into the type, there is no 'get out of jail free' cards where a loophole can be found in the type (or sub-type).  And it also becomes fundamentally a different type to (for example) `NewType<ClientConnectionId, string, StrLen<I0, I10>>` _(See the `<I0, I10>` at the end)_; so any function that wants to work with a client connection token must accept either `ClientConnectionId` or its base type of `NewType<ClientConnectionId, string, StrLen<I10, I100>>`.  It gives a small glimpse into the world of dependently typed languages, I think this is a pretty powerful concept for improving the safety of C# types in general (with no runtime costs), and takes the original `NewType` idea to the next level. 

There is now an explicit cast operator.  So for the `Metres` example above:
```c#
    Metres m = Metres.New(100);
    double x = (double)m * 2.0;
```

### `NumType`

With the new type-classes and class-instances (see later), it's now possible to write generic code for numeric-types.  And so I have created a variant of `NewType` called `NumType`.  Numeric types like `int` are the kind of types that are very commonly made into `NewType` derived types (along with `string`), but previously there wasn't a good story for doing arithmetic on those types.  Now with the `NumType` it is possible.  They work in exactly the same way as `NewTypes`, but you must specify a `Num<A>` class-instance (below it's `TDouble`):
```c#
    public class Metres : NumType<Metres, TDouble, double> { 
        public Metres(double x) : base(x) {}
    }
```
That gives you these extras over `NewType`:
```c#
    operator+
    operator*
    operator/
    operator-
    Product()
    Divide()
    Plus()
    Subtract()
    Abs()
    Signum()
    Min()
    Max()
    Sum()
```
As with `NewType` you can also use a predicate:
```c#
    public class Age : NumType<Age, TInt, int, Range<int, TInt, I0, I120>> 
    { 
        public Age(int x) : base(x)  {}
    }
```
### `FloatType`
Even more specialised than `NumType` and `NewType` in that it only accepts class-instances from the type-class `Floating<A>`.  It adds functionality that are only useful with floating-point number types (along with the functionality from `NumType` and `NewType`):
```
 Exp(), Sqrt(), Log(), Pow(A exp), LogBase(A y), Sin(), Cos(), Asin(), Acos(), Atan(), Sinh()
 Cosh(), Tanh(), Asinh(), Acosh(), Atanh()
```

### `ValueTuple` and `Tuple`

A new feature of C# 7 is syntax for tuples.  Instead of:
```c#
    Tuple.Create(a, b)
```
You can now:
```c#
    (a, b)
```
You can also give them names:
```c#
    (K Key, V Value)
```
So wherever in lang-ext tuples are used, they now accept `ValueTuple`.  `ValueTuple` is the `struct` version of `Tuple` that C# is using to back the new syntax.

This is particularly nice for `Map`:
```c#
    var map = Map(
        (1, "Paul"), 
        (2, "Steve"), 
        (3, "Stan"), 
        (4, "Tanzeel"), 
        (5, "Dan"), 
        (6, "Andreas"));
```
Also `Map` now derives from `IEnumerable<(K Key, V Value)>`.

Tuples look like they're going to be a lot more important in C# going forwards, so I have created extension methods and `Prelude` functions for `Tuple` and `ValueTuple` with up to 7 items.
```c#
    // Add an item to a tuple
    var (a,b,c)   = (1, 2).Add(3);
    var (a,b,c,d) = (1, 2).Add(3).Add("Hello");
```
If your tuple contains Semigroups (like `Lst` and `int` for example) then you can call `Append`:
```c#
    var (a, b) = append<TLst<int>, TInt, Lst<int>, int>(
                    (List(1,2,3), 3),
                    (List(4,5,6,7), 4));

    // ([1,2,3,4,5,6,7], 7)
```
Or:
```c#
    var list = append<TLst<int>, Lst<int>>( (List(1,2,3), List(4,5,6,7)) );

    // [1,2,3,4,5,6,7]
```
`Head` and `Tail`:
```c#
    var a  = ("a", 123, true).Head();   // "a"
    var bc = ("a", 123, true).Tail();   // (123, true)
```
`Sum` and `Product`:
```c#
    var a = (100, 200, 300).Sum<TInt, int>();  // 600
    var b = (10, 10, 10).Product<TInt, int>(); // 1000
```
`Contains`:
```c#
    var a = (1,2,3,4,5).Contains<EqInt, int>(3);  // true
```
Mapping:
```c#
    x = x.Map( tuple => tuple );
    
    x = x.BiMap(a  => a * 2, b => b * 3);
    x = x.TriMap(a  => a * 2, b => b * 3, c => c + " add");
    x = x.QuadMap(a  => a * 2, b => b * 3, c => "over", d => "ride");
    // etc.

    x = x.MapFirst(a => a * 2);  // just maps the first item and leaves the rest alone
    x = x.MapSecond(b => b * 3);  
    x = x.MapThird(c => c + " add");
    x = x.MapFourth(d => "change");  
    // etc.

    var (a, b) = (100, "text").MapFirst(x => x * 2); // (200, "text")
```
Also:
```c#
    Iter, Fold, BiFold, BiFoldBack, TriFold, TriFoldBack, etc.
```
### `Cond<A>`

`Cond` allows for building conditional expressions that can be used fluently.  It also seamlessly steps between synchronous and asynchronous behaviour without any need for ceremony.  

Here's a simple example:
```c#
var cond = Cond<int>(x => x == 4)
               .Then(true)
               .Else(false);
```
That can be run like so:
```c#
bool result = cond(4); // True
bool result = cond(0); // False
```
Or,
```c#
bool result = 4.Apply(cond); // True
bool result = 0.Apply(cond); // False
```
Here's a slightly more complex  example:
```c#
    var vowels = Subj<char>().Map(Char.ToLower)
                             .Any(x => x == 'a', x => x == 'e', x => x == 'i', x => x == 'o', x => x == 'u')
                             .Then("Is a vowel")
                             .Else("Is a consonant");

    var x = vowels('a'); // "Is a vowel"
```
This can then be tagged onto anything that returns a char or a `Task<char>`:
```c#
    var res = GetCharFromRemoteServer().Apply(vowels);   // Task<string>
```
See the [pull request](https://github.com/louthy/language-ext/pull/179) for the discussion that led to this feature.  Thanks to [@ncthbrt](https://github.com/ncthbrt) for the suggestion and initial implementation.

### Range

Continuing the super generic theme, there is now a new `Range` type that can handle any type (as long as they have `Monoid` and `Ord` instances):

```c#
    public class Range<SELF, MonoidOrdA, A> : IEnumerable<A>
        where SELF : Range<SELF, MonoidOrdA, A>
        where MonoidOrdA : struct, Monoid<A>, Ord<A>, Arithmetic<A>
    {
        ...
    }
```
As with `NewType`, `FloatType`, and `NumType`, anything that derives from it should provide itself as the first generic argument.  This is the definition of `IntegerRange`:
```c#
    public class IntegerRange : Range<IntegerRange, TInt, int>
    {
        IntegerRange(int min, int max, int step) : base(min, max, step) { }
    }
```
Everything else is handled in the base class, so it's trivial to add your own.  As before they implement `IEnumerable<A>`, and are lazy.  They now support `Overlaps(SELF range)` and `InRange(A x)`.  There are two constructor functions: `Range.FromMinMax(min, max, step)` and `Range.FromCount(min, count, step)`.  There are several provided implementations: `BigIntegerRange`, `CharRange`, `DecimalRange`, `DoubleRange`, `FloatRange`, `IntegerRange`, `LongRange`, `ShortRange`.

#### `Try<A>` and `TryOption<A>`

`Try` and `TryOption` (the lazy monads that catch exceptions) have been improved to memoise everything they do.  So once you run a `Try`, running the same reference again won't re-invoke the computation, it will just return the previously cached value.  This can be useful in LINQ expressions especially.  

#### `TryAsync<A>`

There is a new monadic type called `TryAsync` which, as you may have guessed, is an asynchronous version of `Try`.  `Try` and `TryAsync` are delegate types that are invoked when you call extension methods like `Match` on them.  By default the `TryAsync` evaluation extension methods will wrap the invocation in a `Task` and will catch any exceptions thrown. 
```c#
    TryAsync<int> LongRunningOp() => TryAsync(() => 10);

    int x = await LongRunningOp().Match(
                Succ: y  => y * 2,
                Fail: ex => 0
                );
```
Unfortunately you must wrap the operation in a `TryAsync(() => ...)` because the compiler can't infer the result-type like it can with `Try`.  However you can promote a `Try` to a `TryAsync`:
```c#
    Try<int> LongRunningOp() => () => 10;

    int x = await LongRunningOp().ToAsync().Match(
                Succ: y  => y * 2,
                Fail: ex => 0
                );
```
Or use any of the new `Async` extension methods added to `Try`:
```c#
    Try<int> LongRunningOp() => () => 10;

    int x = await LongRunningOp().MatchAsync(
                Succ: y  => y * 2,
                Fail: ex => 0
                );
```
Every single method of `Try` now has an `Async` variant.  Also any method of `Try` or `TryAsync` that takes a `Func` (for example `Map(Try<A> x, Func<A, B> f)`), now has a variant that allows a `Task` to be returned instead.  For `Try` methods this will either promote the result to be a `TryAsync` (as is the case with `Map`), or a `Task` (as is the case with `Fold`).  This makes it trivial to deal with asynchronous results, as the `Try` or `TryAsync` will automatically perform the correct synchronisation required to extract a valid result.

For functions where operations could run in parallel then the type will again handle that automatically.  For example if you use the new `Applicative` functionality, this is possible:
```c#
// Placeholder functions.  Imagine they're doing some work to get some remote
// data lists.
TryAsync<Lst<int>> GetListOneFromRemote() => TryAsync(List(1, 2, 3));
TryAsync<Lst<int>> GetListTwoFromRemote() => TryAsync(List(4, 5, 6));

// Combines two lists and sorts them. 
public static IEnumerable<int> CombineAndOrder(Lst<int> x, Lst<int> y) =>
    from item in (x + y)
    orderby item
    select item;

// Uses the fact that TryAsync is an applicative, and therefore has the 
// apply function available.  The apply function will run all three parts
// of the applicative asynchronously, will handle errors from any term,
// and will then apply them to the CombineAndOrder function.
public TryAsync<IEnumerable<int>> GetRemoteListsAndCombine() =>
    apply(
        CombineAndOrder,
        GetListOneFromRemote(),
        GetListTwoFromRemote());

[Fact]
public async void ListCombineTest()
{
    var res = await GetRemoteListsAndCombine().IfFail(Enumerable.Empty<int>());

    var arr = res.ToArray();

    Assert.True(arr[0] == 1);
    Assert.True(arr[1] == 2);
    Assert.True(arr[2] == 3);
    Assert.True(arr[3] == 4);
    Assert.True(arr[4] == 5);
    Assert.True(arr[5] == 6);
}
```

#### `TryOptionAsync<A>`

As `Try` has got its `TryAsync` pairing, so has `TryOption` now got `TryOptionAsync`.  The interface is almost exactly the same as `TryAsync`.  Here are some quick examples:

```c#
    // Some example method prototypes
    public TryOptionAsync<int> LongRunningAsyncTaskOp() => TryOptionAsync(10);
    public Task<TryOption<int>> LongRunningAsyncOp()    => Task.FromResult(TryOption(10));
    public TryOption<int> LongRunningOp()               => TryOption(10);
    public Task<int> MapToTask(int x)                   => Task.FromResult(x * 2);
    public int MapTo(int x)                             => x * 2;

    public async void Test()
    {
        TryOptionAsync<int> j = LongRunningOp().ToAsync();
        TryOptionAsync<int> k = LongRunningAsyncOp().ToAsync();

        // These run synchronously
        int a = LongRunningOp().IfNoneOrFail(0);
        TryOption<int> b = LongRunningOp().Map(MapTo);

        // These run asynchronously
        int u = await LongRunningAsyncTaskOp().IfNoneOrFail(0);
        int v = await LongRunningAsyncOp().IfNoneOrFail(0);
        int x = await LongRunningOp().IfNoneOrFailAsync(0);
        TryOptionAsync<int> y1 = LongRunningOp().MapAsync(MapTo);
        TryOptionAsync<int> y2 = LongRunningOp().MapAsync(MapToTask);
        int z1 = await y1.IfNoneOrFail(0);
        int z2 = await y2.IfNoneOrFail(0);
    }
```

### Ad-hoc polymorphism

Ad-hoc polymorphism has long been believed to not be possible in C#.  However with some cunning it is.  Ad-hoc polymorphism allows programmers to add traits to a type later.  For example in C# it would be amazing if we had an interface called `INumeric` for numeric types like `int`, `long`, `double`, etc.  The reason this doesn't exist is if you write a function like:
```c#
    INumeric Add(INumeric x, INumeric y) => x + y;
```
Then it would cause boxing.  Which is slow (well, slower).  I can only assume that's why it wasn't added by the BCL team.  Anyway, it's possible to create a numeric type, very much like a type-class in Haskell, and ad-hoc _instances_ of the numeric _type-class_ that allow for generic numeric operations without boxing.  

> From now on I will call them type-classes and class-instances, or just instances.  This is not exactly the same as Haskell's type-classes.  If anything it's closer to Scala's implicits.  However to make it easier to discuss them I will steal from Haskell's lexicon.

#### `Num<A>`

So for example, this is how to create a number type-class:
```c#
    public interface Num<A>
    {
        A Add(A x, A b);
        A Subtract(A x, A b);
        ...
    }
```
Notice how there are two arguments to `Add` and `Subtract`.  Normally if I was going to implement this `interface` the left-hand-side of the `Add` and `Subtract` would be `this`.  I will implement the _ad-hoc_ class-instance to demonstrate why that is:
```c#
    public struct TInt : Num<int>
    {
        public int Add(int x, int b) => x + y;
        public int Subtract(int x, int b) => x + y;
        ...
    }
```
See how `TInt` is a `struct`?  Structs have a useful property in C# in that they can't be `null`.  So we can invoke the operations like so:
```c#
    int r = default(TInt).Add(10, 20);
```
The important thing to note is that `default(TInt)` gets optimisied out in a release build, so there's no cost to the invocation of `Add`.  The `Add` and `Subtract` methods both take `int` and return `int`.  So therefore there's no boxing at all.

If we now implement `TFloat`:
```c#
    public struct TFloat : Num<float>
    {
        public float Add(float x, float b) => x + y;
        public float Subtract(float x, float b) => x + y;
        ...
    }
```
Then we can see how a general function could be written to take any numeric type:
```c#
    public A DoubleIt<NumA, A>(A x) where NumA : struct, Num<A> =>
        default(NumA).Add(x, x);
```
The important bit is the `NumA` generic argument, and the constraint of `struct, Num<A>`.  That allows us to call `default(NumA)` to get the type-class instance and invoke `Add`.

And so this can now be called by:
```c#
    int a    = DoubleIt<TInt, int>(5);        // 10
    double b = DoubleIt<TFloat, float>(5.25); // 10.5
```
By expanding the amount of operations that the `Num<A>` type-class can do, you can perform any numeric operation you like.  If you like you can add new numeric types (say for complex numbers, or whatever), where the rules of the type are kept in the _ad-hoc_ instance.

Luckily you don't need to do that, because I have created the `Num<A>` type (in the `LanguageExt.TypeClasses` namespace), as well as `Floating<A>` (with all of the operations from `Math`; like `Sin`, `Cos`, `Exp`, etc.).  `Num<A>` also has a base-type of `Arithmetic<A>` which supports `Plus`, `Subtract`, `Product`, `Negate`.  This is for types which don't need the full spec of the `Num<A>` type.  I have also mapped all of the core numeric types to instances: `TInt`, `TShort`, `TLong`, `TFloat`, `TDouble`, `TDecimal`, `TBigInt`, etc.  So it's possible to write truly generic numeric code once.

> There's no getting around the fact that providing the class-instance in the generic arguments list is annoying (and later you'll see how annoying).  The Roslyn team are looking into a type-classes like feature for a future version of C# (variously named: 'Concepts' or 'Shapes').  So this will I'm sure be rectified, and when it is, it will be implemented exactly as I am using them here.  
> 
> Until then the pain of providing the generic arguments must continue.  You do however get a _super-powered C#_ in the mean-time.  
> 
> The need to write this kind of super-generic code is rare; but when you need it, _you need it_ - and right now this is simply the most powerful way.

#### `Eq<A>`

Next up is `Eq<A>`.  Equality testing in C# is an absolute nightmare.  From the different semantics of `Eqauls` and `==`, to `IEqualityComparer`, and the enormous hack which is `EqualityComparer.Default` (which doesn't blow up at compile-time if your code is wrong).

The `Eq<A>` type-class looks like this:
```c#
    public interface Eq<A>
    {
        bool Equals(A x, A y);
        int GetHashCode(A x);
    }
```
There are `Eq` prefixed instances for all common types (`EqInt`, `EqString`, `EqGuid` etc.), as well as for all of the types in this library (`EqLst`, `EqSet`, `EqTry`, etc).  All of the numeric types (`TInt`, `TDouble`, etc.) also implement `Eq<A>`.

To make it slightly prettier to use in code, you can use the `Prelude` `equals` function:
```c#
    bool x = equals<EqInt>(1, 1); // True
```
Or use `default` as shown before:
```c#
    bool x = default(EqInt).Equals(1, 1); // True
```
One final way is:
```c#
    bool x = EqInt.Inst.Equals(1, 1);
```
`Inst` is defined on all of the instances in lang-ext, but it's not an 'official feature'.  Anybody could implement an ad-hoc implementation of `Eq<A>` and not provide an `Inst`. 

For example you may call this directly:
```c#
    bool x = EqLst.Inst.Equals(List(1,2,3), List(1,2,3)); // true
```
Because you may be concerned about calling:
```c#
    bool x = List(1,2,3) == List(1,2,3); // ?
```
... as all C# programmers are at some point, because we have no idea most of the time whether `==` does what we think it should.  

> Just FYI `List(1,2,3) == List(1,2,3)` does work properly!  As do all types in language-ext.

There are two variants of the immutable `HashSet` in language-ext:
```c#
    HashSet<A>
    HashSet<EqA, A> where EqA : struct, Eq<A>
```
What's interesting about the second one is that the equality _definition_ is baked into the type.  So this:
```c#
    HashSet<EqString, string> 
```
Is not compatible with:
```c#
    HashSet<EqStringOrdinalIgnoreCase, string> 
```
And if you think about that, it's right.  The strings that are used as keys in the `HashSet<EqString, string>` do not have the same properties as the strings in `HashSet<EqStringOrdinalIgnoreCase, string>`.  So even though they're both strings, they have different semantics (which cause wildly different behaviour for things like set intersection, unions, etc.)

Now compare that to `HashSet<T>` in the BCL, or `ImmutableHashSet<T>` in `System.Collections.Immutable`, where two different sets with different `IEqualityComparer` types injected will cause undefined results when used together.

> That's hopefully a small glimpse into the potential for improving type-safeness in C#.

#### `Ord<A>`

`Ord` is for ordering.  i.e. a `IComparable` replacement.  By the way, these names `Eq`, `Ord`, `Num`, are all lifted from Haskell.  I much prefer the short concise names that still convey meaning than the bulky and often clumsy names of the BCL.

This is `Ord<A>`, it derives from `Eq<A>`
```c#
    public interface Ord<A> : Eq<A>
    {
        int Compare(A x, A y);
    }
```
Usage should be self-explanatory now, but the important thing to note here is that because 'type classes' are just interfaces, they can also have an inheritance hierarchy.

This is a slightly more complex example:
```c#
    public struct OrdArray<ORD, A> : Ord<A[]>
        where ORD : struct, Ord<A>
    {
        public int Compare(A[] mx, A[] my)
        {
            if (ReferenceEquals(mx, my)) return 0;
            if (ReferenceEquals(mx, null)) return -1;
            if (ReferenceEquals(my, null)) return 1;

            var cmp = mx.Length.CompareTo(my.Length);
            if (cmp == 0)
            {
                for(var i = 0; i < mx.Length; i++)
                {
                    cmp = default(ORD).Compare(mx[i], my[i]);
                    if (cmp != 0) return cmp;
                }
                return 0;
            }
            else
            {
                return cmp;
            }
        }

        public bool Equals(A[] x, A[] y) =>
            default(EqArray<ORD, A>).Equals(x, y);

        public int GetHashCode(A[] x) =>
            hash(x);
    }
```
The `OrdArray` which is an `Ord<A[]>`, does itself also take an `ORD` generic argument, which allows the contents of the array to be compared:
```c#
    int x = OrdArray<TInt, int>.Inst.Compare(Array(1,2), Array(1,2)); // 0
```

#### `Semigroup<A>`

This is where we start going a little more abstract.  Semigroups are a feature of category theory, which is soooo not important for this discussion.  They represent an associative binary operation, which can be invoked by calling `Append`.
```c#
    public interface Semigroup<A>
    {
        A Append(A x, A y);
    }
```
Positive numbers (for example) form a semigroup.  I won't dwell on it too long, because although the `Append` function is super-useful, nearly everything that falls into the `Semigroup` category is also a `Monoid`...

#### `Monoid<A>`

A monoid has something that a semigroup doesn't, and that's the concept of identity (often meaning 'empty' or 'zero').  It looks like this:

```c#
    public interface Monoid<A> : Semigroup<A>
    {
        A Empty();
    }
```
This comes with some helper functions in `LanguageExt.TypeClass`:
```c#
    public static partial class TypeClass
    {
        public static A mempty<MONOID, A>() where MONOID : struct, Monoid<A> =>
            default(MONOID).Empty();

        public static A mconcat<MONOID, A>(IEnumerable<A> xs) where MONOID : struct, Monoid<A> =>
            xs.Fold(mempty<MONOID, A>(), (s, x) => append<MONOID, A>(s, x));

        public static A mconcat<MONOID, A>(params A[] xs) where MONOID : struct, Monoid<A> =>
            xs.Fold(mempty<MONOID, A>(), (s, x) => append<MONOID, A>(s, x));
    }
```
Now the semigroup `Append` comes to life.  Examples of monoids are: `TInt`, `MLst`, `TString`, etc.  i.e.
```c#
    var x = mconcat<TString, string>("Hello", " ", "World");   // "Hello World"
    var y = mconcat<TLst<int>, Lst<int>>(List(1), List(2, 3)); // [1,2,3]
    var z = mconcat<TInt, int>(1, 2, 3, 4, 5);                 // 15
```
The `Empty()` function is what provides the _identity value_ for the concat operations.  So for `string` it's `""`, for `Lst<T>` it's `[]` and for `int` it's `0`.  So a monoid is a semigroup with a _zero_.

It's surprising how much _stuff_ just starts working when you know your type is a monoid.  For example in language-ext version 1 there is a monadic type called `Writer<W, T>`.  The writer monad collects a _log_ as well as returning the bound value.  In version 1 the log had to be an `IEnumerable<W>`, which isn't super flexible.  In language-ext version 2 the type looks like this:

```c#
    public class Writer<MonoidW, W, A> where MonoidW : struct, Monoid<W>
    {
        ...
    }
```
So now it can be a running numeric total, or a `Lst<W>`, or a `Set<W>`, or whatever monoid _you_ dream up.  

### Higher-kinds

Higher-order polymorphism would allow us to define a type like so:
```c#
    public interface MyType<M<A>>
    {
        M<B> Foo<B>(M<A> ma);
    }
```
Where not only is the `A` parametric, but so it `M`.  So for example if I wanted to implement `MyType` for `Option<A>` I could do:
```c#
    public class MyOptionType<A> : MyType<Option<A>>
    {
        public Option<B> Foo<B>(Option<A> ma) => ...;
    }
```
It would be soooo nice if C# (well, the _immutable_ CLR) would support this.  But it doesn't.  So we need to find ways around it.  The way I am using for language-ext is:
```c#
    public interface MyType<MA, A>
    {
        MB Foo<MB, B>(MA ma);
    }

    public class MyOptionType<A> : MyType<Option<A>, A>
    {
        public MB Foo<MB, B>(Option<A> ma) => ...;
    }
```
#### `Monad`
This is where some of the difficulties come in.  How do we return an `MB` if we don't know what it is?  This is a problem for the `Monad` type.  This is a simplified version:
```c#
    public interface Monad<MA, A>
    {
        MB Bind<MB, B>(MA ma, Func<B, MB> bind);
        MA Return(A a);
        MA Fail(Exception e = null);
    }
```
Looking at the prototype for `Bind` it seems at first glance that the `bind` argument will give us the `MB` value we want.  But an `Option` might be in a `None` state, in which case it shouldn't run `bind`.
```c#
    public MB Bind<MB, B>(Option<A> ma, Func<A, MB> bind) =>
        ma.IsSome
            ? bind(ma.Value)
            : ??? ; // What do we return here?
```
The key is to use constraints.  But it also requires an extra generic paramter for `Bind`:
```c#
    public interface Monad<MA, A>
    {
        MB Bind<MonadB, MB, B>(MA ma, Func<B, MB> bind) 
            where MonadB : struct, Monad<MB, B>;

        MA Return(A a);
        MA Fail(Exception e = null);
    }
```
So we now know that `MonadB` is a class-instance of the `Monad<MB, B>` type-class.  So we can now do this:
```c#
    public MB Bind<MB, B>(Option<A> ma, Func<A, MB> f) 
        where MonadB : struct, Monad<MB, B> =>
            ma.IsSome
                ? f(ma.Value)
                : default(MonadB).Fail();
```
The eagle eyed reader will notice that this actually allows binding to any resulting monad (not just `Option<B>`).  I'm not an academic, but I'm sure there will be someone throwing their toys out of their prams at this point.  However, it works, it's type-safe, and it's efficient. 

[The actual definition of `Monad`](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt.TypeClasses/Monad_Env_Out_MA_A.htm) is more complex than this, in order to unify monadic types that take arguments (`Reader` and `State`) and monads that carry internal state (`Writer` and `State`), as well as to support asynchronous monads (`TryAsync` and `TryOption`).  I won't muddy the waters too much right now, but unified and type-safe they are.  There are no hacks.

You should see that the `Return` and `Fail` functions are trivial to implement:
```c#
    public Option<A> Return(A a) =>
        Optional(a);

    public Option<A> Fail(Exception e = null) =>
        None;
```
What that means is that any function that has been constrained by a monad instance can create new instances of them:
```c#
    public M CreateNewIntegerMonad<MonadInt, M, int>(int x) 
        where MonadInt : struct, Monad<M, int> =>
            default(MonadInt).Return(x);
```
This is one of the key breakthroughs.  Imagine trying to create a `Monad` type the _old way_:
```c#
    public interface Monad<A>
    {
        Monad<B> Bind<B>(Func<A, Monad<B>> bind);
    }

    public class Option<A> : Monad<A>
    {
        public Monad<B> Bind<B>(Monad<A> ma, Func<A, Monad<B>> bind) =>
            IsSome
                ? bind(Value)
                : None;
    }

    public Monad<int> CreateNewIntegerMonad(int x) =>
        ????; // How?
```
Maybe we could parameterise it?
```c#
    public Monad<int> CreateNewIntegerMonad<M>(int x) where M : Monad<int> =>
        ????; // We still can't call new M(x)
```
But that doesn't work either because we still can't call `new M(x)`.  Being able to paramterise generic functions at the point where you know the concrete types (and therefore know the concrete class-instance) means that the generic functions can invoke the instance functions to create the concrete types.

Here's a super generic example of a function that takes two monad arguments, they're both of the same type, and their bound values are `Num<A>`.
```c#
    public static MA Add<MonadA, MA, NumA, A>(MA ma, MA mb)
        where MonadA  : struct, Monad<MA, A>
        where NumA    : struct, Num<A> =>
            default(MonadA).Bind<MonadA, MA, A>(ma, a =>
            default(MonadA).Bind<MonadA, MA, A>(mb, b =>
            default(MonadA).Return(default(NumA).Plus(a, b))));
```
You may notice that the two `Bind` calls followed by the `Return` are basically a much less attractive version of this:
```c#
        from a in ma
        from b in mb
        select default(NumA).Plus(a, b);
```
And so I can now add two options:
```c#
    var x = Some(10);
    var y = Some(20);
    var z = Option<int>.None;

    var r1 = Add<MOption<int>, Option<int>, TInt, int>(x, y); // Some(30)
    var r2 = Add<MOption<int>, Option<int>, TInt, int>(x, z); // None

    Assert.True(r1 == Some(30));
    Assert.True(r2 == None);
```
Or two lists:
```c#
    var x = List(1, 2, 3);
    var y = List(4, 5, 6);
    var z = List<int>();

    var r1 = Add<MLst<int>, Lst<int>, TInt, int>(x, y);
    var r2 = Add<MLst<int>, Lst<int>, TInt, int>(x, z);

    Assert.True(r1 == List(5, 6, 7,  6, 7, 8,  7, 8, 9));
    Assert.True(r2 == z);
```
Or any two monads.  They will follow the built in rules for the type, and produce concrete values efficiently and without any boxing or dynamic casting. 

### Transformer types

Because using the super-generic stuff is hard, and most of the time not needed.  I have kept the transformer types, but they're now implemented in terms of the instance types.  There is a new [`MonadTrans`](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/MonadTrans_OuterMonad_OuterType_InnerMonad_InnerType_A.htm) type-class and a default instance called [`Trans`](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Trans_OuterMonad_OuterType_InnerMonad_InnerType_A.htm).

For every pair of nested monads: `Lst<Option<A>>`, `Try<Either<L, A>>`, etc.  there are the following extension methods (this is for `Arr<Lst<A>>`):
```c#
A SumT<NumA, A>(this Arr<Lst<A>> ma);

int CountT<A>(this Arr<Lst<A>> ma);

Arr<Lst<B>> BindT<A, B>(this Arr<Lst<A>> ma, Func<A, Lst<B>> f);

Lst<Arr<B>> Traverse<A, B>(this Arr<Lst<A>> ma, Func<A, B> f);

Lst<Arr<A>> Sequence<A>(this Arr<Lst<A>> ma);

Arr<Lst<B>> MapT<A, B>(this Arr<Lst<A>> ma, Func<A, B> f);

S FoldT<S, A>(this Arr<Lst<A>> ma, S state, Func<S, A, S> f);

S FoldBackT<S, A>(this Arr<Lst<A>> ma, S state, Func<S, A, S> f);

bool ExistsT<A>(this Arr<Lst<A>> ma, Func<A, bool> f);

bool ForAllT<A>(this Arr<Lst<A>> ma, Func<A, bool> f);

Unit IterT<A>(this Arr<Lst<A>> ma, Action<A> f);

Arr<Lst<A>> FilterT< A>(this Arr<Lst<A>> ma, Func<A, bool> pred);

Arr<Lst<A>> Where<A>(this Arr<Lst<A>> ma, Func<A, bool> pred);

Arr<Lst<A>> Select<A, B>(this Arr<Lst<A>> ma, Func<A, B> f);

Arr<Lst<C>> SelectMany<A, B, C>(
        this Arr<Lst<A>> ma,
        Func<A, Lst<B>> bind,
        Func<A, B, C> project);

Arr<Lst<A>> PlusT<NUM, A>(this Arr<Lst<A>> x, Arr<Lst<A>> y) where NUM : struct, Num<A>;

Arr<Lst<A>> SubtractT<NUM, A>(this Arr<Lst<A>> x, Arr<Lst<A>> y) where NUM : struct, Num<A>;

Arr<Lst<A>> ProductT<NUM, A>(this Arr<Lst<A>> x, Arr<Lst<A>> y) where NUM : struct, Num<A> =>
        ApplyT(default(NUM).Product, x, y);

Arr<Lst<A>> DivideT<NUM, A>(this Arr<Lst<A>> x, Arr<Lst<A>> y) where NUM : struct, Num<A>;

AppendT<SEMI, A>(this Arr<Lst<A>> x, Arr<Lst<A>> y) where SEMI : struct, Semigroup<A>;

int CompareT<ORD, A>(this Arr<Lst<A>> x, Arr<Lst<A>> y) where ORD : struct, Ord<A>;

bool EqualsT<EQ, A>(this Arr<Lst<A>> x, Arr<Lst<A>> y) where EQ : struct, Eq<A>;

Arr<Lst<A>> ApplyT<A, B>(this Func<A, B> fab, Arr<Lst<A>> fa);

Arr<Lst<C>> ApplyT<A, B, C>(this Func<A, B, C> fabc, Arr<Lst<A>> fa, Arr<Lst<A>> fb);
```
The number of functions has increased dramatically.  Some of the special ones are `Traverse` and `Sequence` which flips the inner and outer types.  So for example:
```c#
    Lst<Option<int>> x = List(Some(1), Some(2), Some(3), Some(4), Some(5));
    Option<Lst<int>> y = x.Sequence();

    Assert.True(y == Some(List(1, 2, 3, 4, 5)));
```
As you can see, the list is now inside the option.
```c#
    Lst<Option<int>> x = List(Some(1), Some(2), Some(3), None, Some(5));
    Option<Lst<int>> y = x.Sequence();

    Assert.True(y == None);
```
In this case there is a `None` in the `Lst` so when the `Lst<Option<>>` becomes a `Option<Lst<>>` the rules of the `Option` take over, and one `None` means all `None`.

This can be quite useful for `Either`:
```c#
    var x = List<Either<string, int>>(1, 2, 3, 4, "error");

    var y = x.Sequence();

    Assert.True(y.IsLeft && y == "error");
```
This collects the first error it finds, or returns `Right` if there is no error. 

`Traverse` is the same as `Sequence` except it applies a mapping function to each bound value as it's transforming the types.  Here's an example that runs 6 tasks in parallel, and collects their results:

```c#
    var start = DateTime.UtcNow;

    var f1 = Task.Run(() => { Thread.Sleep(3000); return 1; });
    var f2 = Task.Run(() => { Thread.Sleep(3000); return 2; });
    var f3 = Task.Run(() => { Thread.Sleep(3000); return 3; });
    var f4 = Task.Run(() => { Thread.Sleep(3000); return 4; });
    var f5 = Task.Run(() => { Thread.Sleep(3000); return 5; });
    var f6 = Task.Run(() => { Thread.Sleep(3000); return 6; });

    var res = await List(f1, f2, f3, f4, f5, f6).Traverse(x => x * 2);

    Assert.True(toSet(res) == Set(2, 4, 6, 8, 10, 12));

    var ms = (int)(DateTime.UtcNow - start).TotalMilliseconds;
    Assert.True(ms < 3500, $"Took {ms} ticks");
```
So there is a List of Tasks that becomes a single awaitable Task of List.

As well as the extensions, there are also static classes for the transformer types.  There is one for each type of monad.  So for example, `Option` has a `LanguageExt.OptionT` type.  Whenever you have a pair of nested monads, and `Option` is the inner monad, then you would use `OptionT`:
```c#
    var ma = List(Some(1),Some(2),Some(3),Some(4),Some(5));

    var total = OptionT.foldT(ma, 0, (s, x) => s + x); // 15
    var total = OptionT.sumT<TInt, int>(ma); // 15
    var mb    = OptionT.filterT(ma, x > 3); // List(Some(3), Some(4))
```

I could go on endlessly about the new types.  There are so many.  But for the release notes I think I should wrap it up.  It's worth taking a look at the API documentation for the [type-classes](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt.TypeClasses/index.htm) and the [instances](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt.ClassInstances/index.htm)

### IL.Ctor

Although this doesn't really fit with the core message of the library, it is something used internally in the project, and because it's super useful, I've decided to make it `public` rather than `private`.  There are 4 functions for building a `Func<...,R>` which invokes the constructor for a type.  
```c#
    var ticks = new DateTime(2017, 1, 1).Ticks;

    // Builds a delegate to call new DateTime(long);
    var ctor = IL.Ctor<long, DateTime>();

    DateTime res = ctor(ticks);

    Assert.True(res.Ticks == ticks);
```
The generated delegate doesn't use reflection, IL is emitted directly.  So invocation is as fast as calling `new DateTime(ticks)`.  The four `Ctor` functions are for up to four arguments.

The `NewType` system uses it to build a fast strongly-typed factory function:
```c#
    public abstract class NewType<NEWTYPE, A, PRED> :
        IEquatable<NEWTYPE>,
        IComparable<NEWTYPE>
        where PRED    : struct, Pred<A>
        where NEWTYPE : NewType<NEWTYPE, A, PRED>
    {
        protected readonly A Value;

        /// <summary>
        /// Constructor function
        /// </summary>
        public static readonly Func<A, NEWTYPE> New = IL.Ctor<A, NEWTYPE>();

        ...
    }
```
As you can see, it would be impossible to call `new NEWTYPE(x)` from the `NewType` base-class.

So for example:
```c#
   class Metres : NewType<Metres, float> 
   { 
       public Metres(float x) : base(x) {} 
   }

   var ms = Metres.New(100);
```

