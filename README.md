![lang-ext](https://raw.githubusercontent.com/louthy/language-ext/main/Images/banner.png)

C# Functional Programming Language Extensions
=============================================

This library uses and abuses the features of C# to provide a pure functional-programming framework that, if you squint, can look like 
extensions to the language itself. The desire here is to make programming in C# much more robust by helping the engineer's inertia flow 
in the direction of declarative and pure functional code rather than imperative.  Using these techniques for large code-bases can bring 
tangible benefits to long-term maintenance by removing hidden complexity and by easing the engineer's cognitive load.

[![GitHub Discussions](https://raw.githubusercontent.com/louthy/language-ext/main/Images/discussions.svg)](https://github.com/louthy/language-ext/discussions)

__Author on...__
* __Blog__: [Notes from a Small Functional Island](https://paullouth.com/)
* __Bluesky__: [@paullouth.bsky.social](https://bsky.app/profile/paullouth.bsky.social)
* __Twitter:__ [@paullouth](https://twitter.com/paullouth)
* __Mastodon:__ [@louthy@4four.org](https://4four.org/@louthy)
* __Github ReadME project__: ['Functional programming is finally going mainstream'](https://github.com/readme/featured/functional-programming)

## Contents

* [Reference](#reference)
* [Nu-get package](#nu-get)
* [Getting started](#getting-started)
* [Prologue](#prologue)
* [**Features**](#features)
  * [Functional effects and IO](#functional-effects-and-io)
  * [Atomic concurrency, shared state, and collections](#atomic-concurrency-and-collections)
  * [Immutable collections](#immutable-collections)
  * [Optional and Alternative value monads](#optional-and-alternative-value-monads)
  * [State managing monads](#state-managing-monads)
  * [Parser combinators](#parser-combinators)
  * [Pretty: Produce nicely formatted text with smart layouts](#pretty)
  * [Differencing](#differencing)
  * [Traits](#traits)
  * [Value traits](#value-traits)
* [Contributing & Code of Conduct](#contributing--code-of-conduct)


## Reference

* [API Reference](https://louthy.github.io/language-ext/)
* [Issues that contain documentation and examples](https://github.com/louthy/language-ext/issues?utf8=%E2%9C%93&q=is%3Aissue%20label%3A%22examples%20%2F%20documentation%22%20)

## Nu-get

Nu-get package | Description
---------------|-------------
[LanguageExt.Core](https://www.nuget.org/packages/LanguageExt.Core) | All of the core types and functional 'prelude'. __This is all that's needed to get started__.
[LanguageExt.FSharp](https://www.nuget.org/packages/LanguageExt.FSharp) | F# to C# interop package. Provides interop between the LanguageExt.Core types (like `Option`, `List` and `Map`) to the F# equivalents, as well as interop between core BCL types and F#
[LanguageExt.Parsec](https://www.nuget.org/packages/LanguageExt.Parsec) | Port of the [Haskell parsec library](https://hackage.haskell.org/package/parsec)
[LanguageExt.Rx](https://www.nuget.org/packages/LanguageExt.Rx) | Reactive Extensions support for various types within the Core
[LanguageExt.Sys](https://www.nuget.org/packages/LanguageExt.Sys) | Provides an effects wrapper around the .NET System namespace making common IO operations pure and unit-testable

## Getting started

To use this library, simply include `LanguageExt.Core.dll` in your project or grab it from NuGet.  It is also worth setting up some `global using` for your project.  This is the full list that will cover the key functionality and bring it into scope:
```C#
global using LanguageExt;
global using LanguageExt.Common;
global using static LanguageExt.Prelude;
global using LanguageExt.Traits;
global using LanguageExt.Effects;
global using LanguageExt.Pipes;
global using LanguageExt.Pretty;
global using LanguageExt.Traits.Domain;
```
A minimum, might be:
```c#
global using LanguageExt;
global using static LanguageExt.Prelude;
```

The namespace `LanguageExt` contains most of the core types; `LanguageExt.Prelude` contains the functions that bring into scope the prelude functions that behave like standalone functions in ML style functional programming languages; `LanguageExt.Traits` brings in the higher-kinded trait-types and many extensions; `LanguageExt.Common` brings in the `Error` type and predefined `Errors`.

## Prologue
From C# 6 onwards we got the ability to treat static classes like namespaces. This means that we can use static 
methods without qualifying them first. That instantly gives us access to single term method names that look exactly like functions 
in ML-style functional languages. i.e.
```C#
    using static System.Console;
    
    WriteLine("Hello, World");
```
This library tries to bring some of the functional world into C#. It won't always sit well with the seasoned C# OO programmer, 
especially the choice of `camelCase` names for a lot of functions and the seeming 'globalness' of a lot of the library. 

I can understand that much of this library is non-idiomatic, but when you think of the journey C# has been on, is "idiomatic" 
necessarily right?  A lot of C#'s idioms are inherited from Java and C# 1.0. Since then we've had generics, closures, Func, LINQ, 
async... C# as a language is becoming more and more like a  functional language on every release. In fact, the bulk of the new 
features are either inspired by or directly taken from features in functional languages. So perhaps it's time to move the C# 
idioms closer to the functional world's idioms?

My goal with this library is very much to create a whole new community within the larger C# community.  This community is not 
constrained by the dogma of the past or by the norms of C#.  It understands that the OOP approach to programming has some problems
and tries to address them head-on.  

And for those that say "just use F#" or "just use Haskell", sure, go do that.  But it's important to remember that C# has a lot
going for it:

* Incredible investment into a state-of-the art compiler
* Incredible tooling (Visual Studio and Rider)
* A large ecosystem of open-source libraries
* A large community of developers already using it
  * This is also very important for companies that hire engineers
* It _is_ a functional programming language!  It has first-class functions, lambdas, etc.
  * And with this library it has a functional-first _Base Class Library_

### A note about naming

One of the areas that's likely to get seasoned C# heads worked up is my choice of naming style. The intent is to try and make 
something that _feels_ like a functional language rather than following rules of naming conventions (mostly set out by 
the BCL). 

There is, however, a naming guide that will keep you in good stead while reading through this documentation:

* Type names are `PascalCase` in the normal way
* The types all have constructor functions rather than public constructors that you instantiate with `new`. They will always 
be `PascalCase`:
```C#
    Option<int> x = Some(123);
    Option<int> y = None;
    Seq<int> items = Seq(1,2,3,4,5);
    List<int> items = List(1,2,3,4,5);
    HashMap<int, string> dict = HashMap((1, "Hello"), (2, "World"));
    Map<int, string> dict = Map((1, "Hello"), (2, "World"));
```
* Any (non-type constructor) static function that can be used on its own by `using static LanguageExt.Prelude` are `camelCase`.
```C#
    var x = map(opt, v => v * 2);
```
* Any extension methods, or anything "fluent" are `PascalCase` in the normal way
```C#
    var x = opt.Map(v => v * 2);
```
Even if you disagree with this non-idiomatic approach, all of the `camelCase` static functions have fluent variants, so you never actually have to see the non-standard stuff. 

## Features

### [Functional effects and IO](https://louthy.github.io/language-ext/LanguageExt.Core/Effects/index.html)

| Location | Feature      | Description                                                                                                                                                                                              |
|----------|--------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `Core`   | `IO<A>`      | [A synchronous and asynchronous side-effect: an IO monad](https://louthy.github.io/language-ext/LanguageExt.Core/Effects/IO/index.html)                                                                  |
| `Core`   | `Eff<A>`     | [A synchronous and asynchronous side-effect with error handling](https://louthy.github.io/language-ext/LanguageExt.Core/Effects/Eff/Eff%20no%20runtime/index.html)                                       |
| `Core`   | `Eff<RT, A>` | [Same as `Eff<A>` but with an injectable runtime for dependency-injection: a unit testable IO monad](https://louthy.github.io/language-ext/LanguageExt.Core/Effects/Eff/Eff%20with%20runtime/index.html) |
| `Core`   | Pipes        | [A clean and powerful stream processing system that lets you build and connect reusable streaming components](https://louthy.github.io/language-ext/LanguageExt.Core/Effects/Pipes/index.html)           |
| `Core`   | StreamT      | [less powerful (than Pipes), but easier to use streaming effects transformer](https://louthy.github.io/language-ext/LanguageExt.Core/Effects/StreamT/index.html)                                         |

### [Atomic concurrency and collections](https://louthy.github.io/language-ext/LanguageExt.Core/Concurrency/index.html)

| Location | Feature                            | Description                                                                                                                                            |
|----------|------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------|
| `Core`   | `Atom<A>`                          | [A lock-free atomically mutable reference for working with shared state](https://louthy.github.io/language-ext/LanguageExt.Core/Concurrency/Atom)      |
| `Core`   | `Ref<A>`                           | [An atomic reference to be used in the transactional memory system](https://louthy.github.io/language-ext/LanguageExt.Core/Concurrency/STM)            |
| `Core`   | `AtomHashMap<K, V>`                | [An immutable `HashMap` with a lock-free atomically mutable reference](https://louthy.github.io/language-ext/LanguageExt.Core/Concurrency/AtomHashMap) |
| `Core`   | `AtomSeq<A>`                       | [An immutable `Seq` with a lock-free atomically mutable reference](https://louthy.github.io/language-ext/LanguageExt.Core/Concurrency/AtomSeq)         |
| `Core`   | `VectorClock<A>`                   | [Understand distributed causality](https://louthy.github.io/language-ext/LanguageExt.Core/Concurrency/VectorClock)                                     |
| `Core`   | `VersionVector<A>`                 | [A vector clock with some versioned data](https://louthy.github.io/language-ext/LanguageExt.Core/Concurrency/VersionVector)                            |
| `Core`   | `VersionHashMap <ConflictV, K, V>` | [Distrubuted atomic versioning of keys in a hash-map](https://louthy.github.io/language-ext/LanguageExt.Core/Concurrency/VersionHashMap)               |

### [Immutable collections](https://louthy.github.io/language-ext/LanguageExt.Core/Immutable%20Collections/index.html)

| Location | Feature              | Description                                                                                                                                                                                                             |
|----------|----------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `Core`   | `Arr<A>`             | [Immutable array](https://louthy.github.io/language-ext/LanguageExt.Core/Immutable%20Collections/Arr/index.html)                                                                                                        |
| `Core`   | `Seq<A>`             | [Lazy immutable list, evaluate at-most-once](https://louthy.github.io/language-ext/LanguageExt.Core/Immutable%20Collections/Seq/index.html) - very, very fast!                                                          |
| `Core`   | `Iterable<A>`        | [Wrapper around `IEnumerable` with support for traits](https://louthy.github.io/language-ext/LanguageExt.Core/Immutable%20Collections/Iterable/index.html) - enables the higher-kinded traits to work with enumerables. |
| `Core`   | `Lst<A>`             | [Immutable list](https://louthy.github.io/language-ext/LanguageExt.Core/Immutable%20Collections/List/index.html) - use `Seq` over `Lst` unless you need `InsertAt`                                                      |
| `Core`   | `Map<K, V>`          | [Immutable map](https://louthy.github.io/language-ext/LanguageExt.Core/Immutable%20Collections/Map/index.html)                                                                                                          |
| `Core`   | `Map<OrdK, K, V>`    | [Immutable map with Ord constraint on `K`](https://louthy.github.io/language-ext/LanguageExt.Core/Immutable%20Collections/Map/index.html)                                                                               |
| `Core`   | `HashMap<K, V>`      | [Immutable hash-map](https://louthy.github.io/language-ext/LanguageExt.Core/Immutable%20Collections/HashMap/index.html)                                                                                                 |
| `Core`   | `HashMap<EqK, K, V>` | [Immutable hash-map with Eq constraint on `K`](https://louthy.github.io/language-ext/LanguageExt.Core/Immutable%20Collections/HashMap/index.html)                                                                       |
| `Core`   | `Set<A>`             | [Immutable set](https://louthy.github.io/language-ext/LanguageExt.Core/Immutable%20Collections/Set/index.html)                                                                                                          |
| `Core`   | `Set<OrdA, A>`       | [Immutable set with Ord constraint on `A`](https://louthy.github.io/language-ext/LanguageExt.Core/Immutable%20Collections/Set/index.html)                                                                               |
| `Core`   | `HashSet<A>`         | [Immutable hash-set](https://louthy.github.io/language-ext/LanguageExt.Core/Immutable%20Collections/HashSet/index.html)                                                                                                 |
| `Core`   | `HashSet<EqA, A>`    | [Immutable hash-set with Eq constraint on `A`](https://louthy.github.io/language-ext/LanguageExt.Core/Immutable%20Collections/HashSet/index.html)                                                                       |
| `Core`   | `Que<A>`             | [Immutable queue](https://louthy.github.io/language-ext/LanguageExt.Core/Immutable%20Collections/Queue/index.html)                                                                                                      |
| `Core`   | `Stck<A>`            | [Immutable stack](https://louthy.github.io/language-ext/LanguageExt.Core/Immutable%20Collections/Stack/index.html)                                                                                                      |

### [Optional and alternative value monads](https://louthy.github.io/language-ext/LanguageExt.Core/Monads/Alternative%20Monads/index.html)

| Location | Feature                         | Description                                                                                                                                                                                              |
|----------|---------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `Core`   | `Option<A>`                     | [Option monad](https://louthy.github.io/language-ext/LanguageExt.Core/Monads/Alternative%20Monads/Option/index.html)                                                                                     |
| `Core`   | `OptionT<M, A>`                 | [Option monad-transformer](https://louthy.github.io/language-ext/LanguageExt.Core/Monads/Alternative%20Monads/OptionT/index.html)                                                                        |
| `Core`   | `Either<L,R>`                   | [Right/Left choice monad](https://louthy.github.io/language-ext/LanguageExt.Core/Monads/Alternative%20Monads/Either/index.html)                                                                          |
| `Core`   | `EitherT<L, M, R>`              | [Right/Left choice monad-transformer](https://louthy.github.io/language-ext/LanguageExt.Core/Monads/Alternative%20Monads/EitherT/index.html)                                                             |
| `Core`   | `Fin<A>`                        | [`Error` handling monad, like `Either<Error, A>`](https://louthy.github.io/language-ext/LanguageExt.Core/Monads/Alternative%20Monads/Fin/index.html)                                                     |
| `Core`   | `FinT<M, A>`                    | [`Error` handling monad-transformer](https://louthy.github.io/language-ext/LanguageExt.Core/Monads/Alternative%20Monads/FinT/index.html)                                                                 |
| `Core`   | `Try<A>`                        | [Exception handling monad](https://louthy.github.io/language-ext/LanguageExt.Core/Monads/Alternative%20Monads/Try/index.html)                                                                            |
| `Core`   | `TryT<M, A>`                    | [Exception handling monad-transformer](https://louthy.github.io/language-ext/LanguageExt.Core/Monads/Alternative%20Monads/TryT/index.html)                                                               |
| `Core`   | `Validation<FAIL ,SUCCESS>`     | [Validation applicative and monad](https://louthy.github.io/language-ext/LanguageExt.Core/Monads/Alternative%20Monads/Validation/index.html) for collecting multiple errors before aborting an operation |
| `Core`   | `ValidationT<FAIL, M, SUCCESS>` | [Validation applicative and monad-transformer](https://louthy.github.io/language-ext/LanguageExt.Core/Monads/Alternative%20Monads/ValidationT/index.html)                                                |

### [State managing monads](https://louthy.github.io/language-ext/LanguageExt.Core/Monads/State%20and%20Environment%20Monads/index.html)

| Location | Feature            | Description                                                                                                                                                                             |
|----------|--------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `Core`   | `Reader<E, A>`     | [Reader monad](https://louthy.github.io/language-ext/LanguageExt.Core/Monads/State%20and%20Environment%20Monads/Reader/Reader/index.html)                                               |
| `Core`   | `ReaderT<E, M, A>` | [Reader monad-transformer](https://louthy.github.io/language-ext/LanguageExt.Core/Monads/State%20and%20Environment%20Monads/Reader/ReaderT/index.html)                                  |
| `Core`   | `Writer<W, A>`     | [Writer monad that logs to a `W` constrained to be a Monoid](https://louthy.github.io/language-ext/LanguageExt.Core/Monads/State%20and%20Environment%20Monads/Writer/Writer/index.html) |
| `Core`   | `WriterT<W, M, A>` | [Writer monad-transformer](https://louthy.github.io/language-ext/LanguageExt.Core/Monads/State%20and%20Environment%20Monads/Writer/WriterT/index.html)                                  |
| `Core`   | `State<S, A>`      | [State monad](https://louthy.github.io/language-ext/LanguageExt.Core/Monads/State%20and%20Environment%20Monads/State/State/index.html)                                                  |
| `Core`   | `StateT<S, M, A>`  | [State monad-transformer](https://louthy.github.io/language-ext/LanguageExt.Core/Monads/State%20and%20Environment%20Monads/State/StateT/index.html)                                     |

### [Parser combinators](https://louthy.github.io/language-ext/LanguageExt.Parsec/index.html)

| Location | Feature        | Description                                                                                                                    |
|----------|----------------|--------------------------------------------------------------------------------------------------------------------------------|
| `Parsec` | `Parser<A>`    | [String parser monad and full parser combinators library](https://louthy.github.io/language-ext/LanguageExt.Parsec/index.html) |
| `Parsec` | `Parser<I, O>` | [Parser monad that can work with any input stream type](https://louthy.github.io/language-ext/LanguageExt.Parsec/index.html)   |

### [Pretty](https://louthy.github.io/language-ext/LanguageExt.Core/Pretty/index.html)

| Location | Feature  | Description                                      |
|----------|----------|--------------------------------------------------|
| `Core`   | `Doc<A>` | Produce nicely formatted text with smart layouts |

### [Differencing](https://louthy.github.io/language-ext/LanguageExt.Core/DataTypes/Patch/index.html)

| Location | Feature         | Description                                                                                                                                                                                                                          |
|----------|-----------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `Core`   | `Patch<EqA, A>` | Uses patch-theory to efficiently calculate the difference (`Patch.diff(list1, list2)`) between two collections of `A` and build a patch which can be applied (`Patch.apply(patch, list)`) to one to make the other (think git diff). |

### [Traits](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/index.html)

The traits are major feature of `v5`+ language-ext that makes generic programming with higher-kinds a reality.  Check out Paul's [series on Higher Kinds](https://paullouth.com/higher-kinds-in-c-with-language-ext/) to get a deeper insight.

| Location | Feature                                | Description                                                                                                                                                            |
|----------|----------------------------------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `Core`   | `Applicative<F>`                       | [Applicative functor](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Applicative/index.html)                                                            |
| `Core`   | `Eq<A>`                                | [Ad-hoc equality trait](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Eq/index.html)                                                                   |
| `Core`   | `Fallible<F>`                          | [Trait that describes types that can fail](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Fallible/index.html)                                          |
| `Core`   | `Foldable<T>`                          | [Aggregation over a structure](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Foldable/index.html)                                                      |
| `Core`   | `Functor<F>`                           | [Functor `Map`](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Functor/index.html)                                                                      |
| `Core`   | `Has<M, TRAIT>`                        | [Used in runtimes to enable DI-like capabilities](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Has/index.html)                                        |
| `Core`   | `Hashable<A>`                          | [Ad-hoc has-a-hash-code trait](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Hashable/index.html)                                                      |
| `Core`   | `Local<M, E>`                          | [Creates a local environment to run a computation ](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Local/index.html)                                    |
| `Core`   | `Monad<M>`                             | [Monad trait](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Monads/Monad/index.html)                                                                   |
| `Core`   | `MonadT<M, N>`                         | [Monad transformer trait](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Monads/MonadT/index.html)                                                      |
| `Core`   | `Monoid<A>`                            | [A monoid is a type with an identity `Empty` and an associative binary operation `+`](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Monoid/index.html) |
| `Core`   | `MonoidK<M>`                           | [Equivalent of monoids for working on higher-kinded types](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/MonoidK/index.html)                           |
| `Core`   | `Mutates<M, OUTER_STATE, INNER_STATE>` | [Used in runtimes to enable stateful operations](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Mutates/index.html)                                     |
| `Core`   | `Ord<A>`                               | [Ad-hoc ordering / comparisons](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Ord/index.html)                                                          |
| `Core`   | `Range<SELF, NumOrdA, A>`              | [Abstraction of a range of values](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Range/index.html)                                                     |
| `Core`   | `Readable<M, Env>`                     | [Generalised Reader monad abstraction](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Readable/index.html)                                              |
| `Core`   | `Semigroup<A>`                         | [Provides an associative binary operation `+`](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Semigroup/index.html)                                     |
| `Core`   | `SemigroupK<M>`                        | [Equivalent of semigroups for working with higher-kinded types](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/SemigroupK/index.html)                   |
| `Core`   | `Stateful<M, S>`                       | [Generalised State monad abstraction](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Stateful/index.html)                                               |
| `Core`   | `Traversable<T>`                       | [Traversable structures support element-wise sequencing of Applicative effects](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Traversable/index.html)  |
| `Core`   | `Writable<M, W>`                       | [Generalised Writer monad abstraction](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Writable/index.html)                                              |

### [Value traits](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Domain/index.html)

These work a little like type-aliasing but they impart semantic meaning and some common operators for the underlying value.

| Location | Feature                              | Description                                                                                                                                                                                                                                       |
|----------|--------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `Core`   | `DomainType<SELF, REPR>`             | Provides a mapping from `SELF` to an underlying representation: `REPR`                                                                                                                                                                            |
| `Core`   | `Identifier <SELF>`                  | Identifiers (like IDs in databases: `PersonId` for example), they are equivalent to `DomaintType` with equality.                                                                                                                                  |
| `Core`   | `VectorSpace<SELF, SCALAR>`          | Scalable values; can add and subtract self, but can only multiply and divide by a scalar. Can also negate.                                                                                                                                        |
| `Core`   | `Amount <SELF, SCALAR>`              | Quantities, such as the amount of money in USD on a bank account or a file size in bytes. Derives `VectorSpace`, `IdentifierLike`, `DomainType`, and is orderable (comparable).                                                                   |
| `Core`   | `Locus <SELF, DISTANCE, SCALAR>`     | Works with space-like structures. Spaces have absolute and relative distances. Has an origin/zero point and derives `DomainType`, `IdentifierLike`, `AmountLike` and `VectorSpace`.  `DISTANCE` must also be an `AmountLike<SELF, REPR, SCALAR>`. |

_These features are still a little in-flux as of 17th Oct 2024 - they may evolve, be renamed, or removed - but I like the idea!_

## Further 

For some non-reference like documentation:

* Paul's blog: [Notes from a Small Functional Island](https://paullouth.com/) does deep dives into the philosophy of FP and the inner-workings of language-ext.
* [The wiki](https://github.com/louthy/language-ext/wiki) has some additional documentation, some might be a little out of date since the big `v5` refactor, but should give some good insights.

## Contributing & Code of Conduct

If you would like to get involved with this project, please first read the [Contribution Guidelines](https://github.com/louthy/language-ext/blob/main/CONTRIBUTING.md) and the [Code of Conduct](https://github.com/louthy/language-ext/blob/main/CODE_OF_CONDUCT.md).

