# LanguageExt.Sys

`LanguageExt.Sys` is the a wrapper around the `System` namespace IO functions and is part of the [language-ext functional programming framework](https://github.com/louthy/language-ext).

The framework uses and abuses the features of C# to provide a pure functional-programming 
'Base Class Library' that, if you squint, can look like extensions to the language itself. 
The desire here is to make programming in C# much more robust by helping the engineer's 
inertia flow in the direction of declarative and pure functional code rather than imperative. 

Using these techniques for large code-bases can bring tangible benefits to long-term maintenance 
by removing hidden complexity and by easing the engineer's cognitive load.

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
| `Core`   | `Alternative<F>`                       | [A monoid on applicative functors](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Alternative/index.html)                                               |
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
| `Core`   | `SemiAlternative<F>`                   | [A semigroup on functors](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/SemiAlternative/index.html)                                                    |
| `Core`   | `Semigroup<A>`                         | [Provides an associative binary operation `+`](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Semigroup/index.html)                                     |
| `Core`   | `SemigroupK<M>`                        | [Equivalent of semigroups for working with higher-kinded types](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/SemigroupK/index.html)                   |
| `Core`   | `Stateful<M, S>`                       | [Generalised State monad abstraction](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Stateful/index.html)                                               |
| `Core`   | `Traversable<T>`                       | [Traversable structures support element-wise sequencing of Applicative effects](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Traversable/index.html)  |
| `Core`   | `Writable<M, W>`                       | [Generalised Writer monad abstraction](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Writable/index.html)                                              |

### [Value traits](https://louthy.github.io/language-ext/LanguageExt.Core/Traits/Domain/index.html)

These work a little like `NewType` but they impart semantic meaning and some common operators for the underlying value.

| Location | Feature                              | Description                                                                                                                                                                                                                                       |
|----------|--------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| `Core`   | `DomainType<SELF, REPR>`             | Provides a mapping from `SELF` to an underlying representation: `REPR`                                                                                                                                                                            |
| `Core`   | `Identifier <SELF>`                  | Identifiers (like IDs in databases: `PersonId` for example), they are equivalent to `DomaintType` with equality.                                                                                                                                  |
| `Core`   | `VectorSpace<SELF, SCALAR>`          | Scalable values; can add and subtract self, but can only multiply and divide by a scalar. Can also negate.                                                                                                                                        |
| `Core`   | `Amount <SELF, SCALAR>`              | Quantities, such as the amount of money in USD on a bank account or a file size in bytes. Derives `VectorSpace`, `IdentifierLike`, `DomainType`, and is orderable (comparable).                                                                   |
| `Core`   | `LocusLike <SELF, SCALAR, DISTANCE>` | Works with space-like structures. Spaces have absolute and relative distances. Has an origin/zero point and derives `DomainType`, `IdentifierLike`, `AmountLike` and `VectorSpace`.  `DISTANCE` must also be an `AmountLike<SELF, REPR, SCALAR>`. |
