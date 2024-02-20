# Version 5.0.0 Major Release Notes

**_WORK IN PROGRESS NOTES_**

## Motivations

### Empower the users of this library to create their own functional types

If you spend any time with Haskell or other languages with higher-kinds, you'll notice a heavy use of 'monad transformers'.  Monad Transformers allow you to _'stack'_ existing monadic types - and in the process compose their behaviours.  A naive way to think of this is a bit like multiple-inheritance in OO (it's much better than that though!)

But, because Monad Transformers need higher-kinds they can't be implemented in C#.  However, when you get down to basics, monads are just a particular 'flavour' of function compostion.  Monadic types just inject a few rules in-between the compositions to make them behave to their 'flavour' (like optional values, iterations, logging, reading/writing state, etc.)

> _It is then possible to consider that if one had access to the `Bind` function of `Option` and the `Bind` function of `State` then you could compose them into a new `Bind` function that had the behaviour of both_.  

But, how do we standardise the `Bind` function for any monadic type and make it available for composition?  This is where Transducers come in.  They are literally designed to be compositional pipeline components.  You could think of them as the LEGO bricks of functional programming (after functions of course! but transducers allow us to embelish our functions with addtional functionality, like resource tracking, streaming, optional behaviour, etc.)  

Therefore I have introduced Transducers as a new core capability in language-ext.  And all of the monadic types are now either implemented with Transducers or have the option to convert to a Transducer.


### Reduce the type and function explosion due to `async`

If I continued the way I was before then every monadic type would have an `*Async` variant, as would every method and function.  This was getting out of hand.  For something like an IO/effect monad where you could also have an optional error-type and optional runtime-type that meant 8 types.  Each with 1000s of lines of code to define them.  Then, when you think there's 20 or so 30 or so monadic types, it becomes a big maintenence problem.  There's also issues around consistency between each type (making sure everything has a `MapAsync`, `BindAsync`, etc.) - as well as making sure sync types can work with async types, etc.

So, as of now, this library stands against 'declarative async' - i.e. it's adopting a _'green threads mentality'_.  That is we will not be giving you `*Async` variants of anything.  All computation types (`IO`, `Eff`, `Reader`, `Proxy`, etc.) will support the _lifting_ of both synchronous and asynchronous functions, but you won't see evidence of asynchronicity in any type-signatures.

Many of the `*Async` functions like `MapAsync`, `BindAsync` are also gone.  There is now `Map`, `Bind`, etc. that accept `Transducer` types (as well as the regular `Func` version).  `Transducer` supports `liftAsync` to lift an asynchronous function into a `Transducer` - it can therefore do async operations without lots of extra boilerplate support for async elsewhere.

Of course, if you feel you miss these async variant functions, you can add your own extensions.

### Leverage modern C# features

The library has been held back by the need to support .NET Framework.  As of now this library is .NET (formally known as .NET Core) only.  Instantly jumping to .NET 8.0 (which has Long Term Support).

This opens up: static interface members (which allows the trait/ad-hoc polymorphism support to get a power-up) and collection initialisers for all of the immutable collections - amongst others.


## New Features

- IO monads
	- Two new IO effect monads that have parametric errors
- Transducers
	- All computation based monads rewritten to use transducers
- Infinite recursion in monads
- Streaming effects
	- By calling `many(stream)` in any monad or monad-transformer expression you instantly turn the monad into a stream.
	- Supported streams: `IAsyncEnumerable`, `IEnumerable`, `IObservable`
- Auto-resource managment (`use` / `release`)
- `Pure` / `Fail` monads
	- Allow easy lifting of pure and failure monadic values (a bit like `None`)
- Lifting
	- Allow easy lifting of synchronous functions and asynchronous into computation based monads.
- Improved guards
- Nullable annotations
	- Everywhere!
- Collection initialisers
	- `Seq<int> xs = [1, 2, 3]` FTW!.
- Monad transformers
	- Yes, for real: stackable, aliasable, monad-transformers!

## Breaking changes

### `netstandard2.0` no longer supported

Version 5 of language-ext jumps straight to `net8.0` support.

**Motivation**

I held off for as long as I could, but there are lots of new C# features that this library can make use of (primarily static interfaces, but others too like collection initialisers); so it's time to leave .NET Framework behind and focus on .NET [Core].  

**Impact**

High (if you're still on .NET Framework)

**Mitigation** 

Migrate your application to .NET Core


### `Seq1` made `[Obsolete]`

A [previous attempt](https://github.com/louthy/language-ext/releases/tag/v4.0.2) to remove `Seq1` was paused due to [potential migration issues](https://github.com/louthy/language-ext/discussions/931).  

**Motivation**

The plan to remove the `Seq1` singleton `Seq` constructor was announced a few years ago.  I've taken this opportunity to make it obsolete as we now have collection initialisers and the previous reasons for the delay in making `Seq1` obsolete have subsided (a 3 year window should be enough!).  

**Impact**

Low

**Mitigation** 

Use `[x]` or `Seq.singleton(x)`

### 'Trait' types now use static interface methods

Before static interface methods existed, the technique was to rely on the non-nullable nature of structs to get access to 'static' methods (via interface based constraints), by calling `default(TRAIT_TYPE).StaticMethod()`.

Language-ext has many of these 'trait types', like `Eq<A>`, `HasCancel<A>`, etc.  They have all been updated to use `static abstract` methods.

**Motivation**

So, where before you might call: `default(EqA).Equals(x, y)` (where `EqA` is `struct, Eq<A>`) - you now need to call `EqA.Equals(x, y)` (where `EqA` is `Eq<A>`) .  

This is obviously much more elegant and removes the need for the `struct` constraint.

**Impact**

Medium - your code will throw up lots of 'Cannot access static method' errors.  It is a fairly mechanical processes to fix them up.  

**Mitigation** 

If you have implemented any of these traits, as instances, then you'll need to implement these changes:

* Remove the `struct` from any constraints (`where X : struct`)
* Add `static` to trait method implementations
* Any default `Inst` usages should be removed
* The types can still be implemented as structs, so that doesn't need to change, but they can now be implemented with any instance type.

### The 'higher-kind' trait types have all been refactored

The following types have all bee rewritten: `Monad`, `Functor`, `Applicative`, `Alternative`, `Foldable`, etc.

**Motivation**

The new static interfaces have opened up a more effective approach to higher-kinds in C#.  Instead of doing as much as possible to retain the original types in methods like `Bind`, `Map`, `Apply`, etc. we now expect all types that need to leverage `Monad`, `Functor`, etc. to inherit `K<M, A>`.  

For example, `Option<A>` inherits `K<Option, A>`, `Seq<A>` inehrits `K<Seq, A>`, `Either<L, R>` inherits `K<Either<L>, R>`.  The `M` in `K<M, A>` is the trait implementation.  So, `Option` (no generic argument) would inherit `Monad<Option>`, `Traversable<Option>`, etc.  

Thise truly opens up higher-order generic programming in C#.  

*** TODO: Insert a link here to a tutorial on higher-kinds in C# ***

**Impact**

High, if you have built your own `Monad`, `Functor`, `Applicative` implementations; or you have been writing code that leverages the generic nature of the traits.  However, I doubt this impact will be large because the previous approach was cumbersome - hence the refactor.

**Mitigation** 

This is rewrite territory.  I would encourage you to look at the new traits and monad transformers - as they're much more effective.

### The `Semigroup<A>` and `Monoid<A>` types has been refactored

The `Append` in `Semigroup<A>` (which `Monoid<A>` inherits) is now an instance method.  Meaning you must derive your semigroup and monoidal types from `Monoid<YOUR_TYPE>` to leverage its capabilities.

The functions in the `TypeClass` static class have been moved to: `Monoid` and `Semigroup` static _module_ classes.  `TypeClass.mappend` is now `Semigroup.append`, `TypeClass.mconcat` is now `Monoid.concat`, etc.

Semigroup also defines `operator+` now.

**Motivation**

Monoids, like the other trait types, were set up work ad-hoc polymorphically.  That is, we could build a `Monoid` instance for a type that we don't own.  And, although we have now lost that capability, we have gained a much easier experience for working with monoidal types.

For example, `Validation<MonoidFail, Fail, Success>` is now just `Validation<Fail, Success>`.  Previously, you'd have to specify the `MonoidFail` trait all the time because there was no way for C# to infer it.  I suspect most people use the `Validation<Fail, Success>` variant with its built-in `Seq` of `Fail` results.  Now it's just as easy to use any monoid.

This obviously means that, with types that you don't own, they can't be monoidal directly.  

However, you can always wrap existing types with monoidal container:
```c#
public readonly record struct MEnumerable<A>(IEnumerable<A> Items) : 
    Monoid<MEnumerable<A>>
{
    public MEnumerable<A> Append(MEnumerable<A> rhs) =>
        new(Items.Concat(rhs.Items));

    public static MEnumerable<A> Empty =>
        new(Enumerable.Empty<A>());
}
```
This lifts an existing type into a monoid that you can then use with generic functions that expect a monoid.  I think that although this is a little bit awkward, it's the scenario that happens then least; most of the time we have control over the type we want to be monoidal and so we can just inherit `Monoid<YOUR_TYPE>`.

**Impact**

Medium - I'm not expecting mass adoption of the previous traits system, so it probably will have a low impact for most.  However, monoids were probably one of the easier traits to use.

**Mitigation**

Any implementations of `Monoid<YOUR_TYPE>` that you have, take the implementation and move the members into `YOUR_TYPE` and `Append` into a non-static method that takes a single argument rather than two (`this` is your first argument now).

### The static `TypeClass` class has been renamed `Trait`

`LanguageExt.TypeClass` is effectively a Prelude for the trait functions, this has been renamed to `LanguageExt.Trait`.


**Motivation**

The name type-class comes from Haskell, which has been a massive influence on this library, however, I think the word 'trait' is more descriptive than 'type class', which is potentially a bit confusing to the average C# developer.

**Impact**

Low

**Mitigation** 

Search and replace `TypeClass` for `Trait`.

### `Apply` extensions that use raw `Func` removed

**Motivation**

The applicative-functor `Apply` function is supposed to work on lifted functions (i.e. `M<Func<A, B>>` not the raw `Func<A, B>`).  I orignally provided variants that work with the raw `Func` for convenience, but really they're just `Map` by another name.

**Impact**

Medium

**Mitigation** 

The new `Functor` trait gives all functor types a new variant of `Map` which takes the `Func` as the first argument and the functor value as the second (this differs from the existing handwritten `Map` methods which take `this` as the first argument and a `Func` as the second argument).  That, along with new extension methods for `Func<A, ..., J>` have been added that curry the `Func` and then applies the map function.

So, instead of:

```c#
Func<string, string, string> surround = 
	(str, before, after) => $"{before} {str} {after}";

var mx = Some("Paul");
var my = Some("Mr.");
var mz = Some("Louth");

surround.Apply(mx).Apply(my).Apply(mz);
```

Change the first `Apply` to `Map`:

```c#
surround.Map(mx).Apply(my).Apply(mz);
```

Those of you that have used Haskell will recognise that pattern, as it's commonly used:

```haskell
surround <$> mx <*> my <*> mz
```

### Manually written `Sequence` extension methods have been removed (#1)

The `Sequence` methods that follow this pattern have been removed:

```c#
public static Fin<Lst<B>> Sequence<A, B>(this Lst<A> ta, Func<A, Fin<B>> f) => ...
```

**Motivation**

Before being able to formulate the higher-kinds I had to manually write six `Sequence` methods and `Traverse` for every pair of monadic types.  There are 450 of them in total.  As you can imagine that's a real pain to develop and maintain.  

The other issue is that `Sequence` and `Traverse` don't work for monadic types that you build.  You have to write those extension methods yourself for every pair.  This is hardly approachable.

There is a new trait type called `Traversable` that generalises traversals. The completely generic functions are available via:

```c#
    Traversable.traverse(...)
    Traversable.sequence(...)
    Traversable.sequenceA(...)
    Traversable.mapM(...)
```
They will work with *any* pair of traversble and applicatives.

I have also added a `Traverse` method to every traversable type (`Seq`, `Option`, etc.) and those will work with *any* applicative.  Which means if you build your own monads/applicative-functors that have implementations that derive from `Monad<M>` and/or `Applicative<F>` then they will automatically work with the language-ext traversable types.

I have named the new version `Traverse` because that's really what it should have been called in the first place.  So, as this is a big breaking changes release, I decided to bite the bullet and rename it.

**Impact**

High: `Sequence` is likely to be used a lot because it's so useful.  Renaming it to `Traverse` will instantly cause your build to fail.  The new version also returns a lifted value that you may need to convert.

**Mitigation**

Where your build fails due to `Sequence` you should change `Sequence` to `Traverse` and follow the call with `.As()` to lower the generic type:

Before:
```c#
	var results = Seq(1,2,3).Sequence(x => x % 2 == 0 : Some(x) : None);
```

After:
```c#
	var results = Seq(1,2,3).Traverse(x => x % 2 == 0 : Some(x) : None).As();
```

### Manually written `Sequence` extension methods have been removed (#2)

The `Sequence` methods that follow this pattern have been removed:

```c#
public static Option<Seq<A>> Sequence<A>(this Seq<Option<A>> ta) => ...
```

**Motivation**

The motivations are the same as for the other removal of `Sequence`.  By removing it we get to use the generalised version which allows others to build monadic types that be composed with the language-ext types and be traversed.

The difference with this removal and the last one is that there is no equivalent `Sequence` method added to the monadic types (like `Option`, `Seq`, etc.);  The reason for this is that it's not possible for C# to pick up the nested generics correctly, so it's a waste of time writing them.  

You can still call `Traversable.sequence` and `Traversable.sequenceA` to do this manually, however it's much easier to call `ma.Traverse(identity)` which is isomorphic.

**Mitigation**

Before:
```c#
var results = Seq(Some(1), Some(2), None).Sequence();
```

After:
```c#
var results = Seq(Some(1), Some(2), None).Traverse(identity).As();
```
It's not quite as concise obviously, but it is completely generic and extensible which the previous one wasn't.  You can also build your own extension methods for commonly used pairs of monads:

```c#
public static Option<Seq<A>> Sequence<A>(this Seq<Option<A>> mx) =>
    mx.Traverse(identity).As();
```
That will restore the previous functionality.

### Manually written `Traverse` extension methods have been removed (#3)

The `Traverse` methods that follow this pattern have been removed:

```c#
public static Seq<Fin<B>> Traverse<A, B>(this Fin<Seq<A>> ma, Func<A, B> f) => ...
```

**Motivation**

The motivations are the same as for the other removals of `Sequence`.  By removing it we get to use the generalised version which allows others to build monadic types that be composed with the language-ext types and be traversed.

Again, there will be no replacement for this as you can just use a combination of `Sequence` and `Map` to achieve the same goals.

**Mitigation**

Before:
```c#
var results = Seq(Some(1), Some(2), None).Traverse(x => x * 2);
```

After:
```c#
var results = Seq(Some(1), Some(2), None).Sequence().Map(x => x * 2).As();
```
Again, it's not quite as concise, but it is completely generic and extensible which the previous one wasn't.  You can also build your own extension methods for commonly used pairs of monads:

```c#
public static Option<Seq<B>> Traverse<A, B>(this Seq<Option<A>> mx, Func<A, B> f) =>
    mx.Sequence().Map(f).As();
```
That will restore the previous functionality.


### `ToComparer` doesn't exist on the `Ord<A>` trait any more

**Motivation**

Because the trait types now use `static` methods, we can't now have a `ToComparer()` extension for the `Ord<A>` type.  Instead there's a class called `OrdComparer` that contains a singleton `IComparer` property called `Default`.

**Impact**

Low

**Mitigation** 

Use `OrdComparer<OrdA, A>.Default` instead of `<OrdA>.ToComparer()`.


### Renamed `LanguageExt.ClassInstances.Sum<NUM, A>`

Renamed to `LanguageExt.ClassInstances.Addition<SUM, A>`

**Mitigation** 

There's a new type called `Sum<L, R>` for use with transducers.  

**Impact**

Low

**Mitigation** 

Rename uses of `Sum<NUM, A>` to `Addition<NUM, A>`


###  `Guard<E>` has become `Guard<E, A>`

The `A` is never used, this just allows guards to work in LINQ by enabling the implementation of `SelectMany`.  The benefit is that a number of guards can be placed together in a LINQ statement, where only one could before.

* Impact: Zero unless you've written your own code to work with `Guard`.  If you only ever used the `guard` Prelude function this will have no impact.

### Existing uses of `HasCancel<RT>` should be replaced with `HasIO<RT, Error>`

The basic runtime traits needed by the effect monads (`IO`, `Eff`, and `Aff`) has been expanded to `HasCancel<RT>`, `HasFromError<RT, E>`, and `HasSyncContext<RT>`.  These can all be applied manually or you can use the `HasIO<RT, E>` trait which wraps all three traits into a single trait.

Simply search and replace: `HasCancel<RT>` for `HasIO<RT, Error>`.  This will work for all existing `Eff` and `Aff` code.  

* `HasSyncContext<RT>` allows all `IO` and `Eff` monads to use the `post(effect)` function.  This allows the running of an effect on original `SynchronizationContext` (needed for WPF and Windows Form applications).
* `HasFromErrror<RT, E>` allows conversion of `Error` to `E`.  The new `IO` monads have an parametric error type - and so this allows for exceptional errors and expecteded `Error` values to be converted to the parametric error type: `E`.

### `UnitsOfMeasaure` namespace converted to a static class

The various utilty fields used for units (`seconds`, `cm`, `kg`, etc.) have been moved out of the `LanguageExt.Prelude` and into `LanguageExt.UnitsOfMeasure` static class.  The unit types (`Length`, `Mass`, etc.) are now all in the `LanguageExt` namespace.

* Impact: Low
* Mitigation: 
	* All usages of `using LanguageExt.UnitsOfMeasure` should be converted to `using static LanguageExt.UnitsOfMeasure`
	* Any uses of the unit-fields will also need `using static LanguageExt.UnitsOfMeasure` either globally or in each `cs` file.

### `Either` doesn't support `IEnumerable<EitherData>` any more

This is part of preparing the library for future serialisation improvements.

* Impact: Low
* Mitigation:
	* If you used the feature for serialisation, build your own handler for whatever serialisation library you use.
	* If you use it in LINQ expressions, write your own extension method to convert the `Either` to an `IEnumerable` that supports 

## `Either` 'bi' functions have their arguments flipped

There were lots of methods like `BiMap`, `BiBind`, `BiFold` etc. where `Left` and `Right` were in the wrong order in the method argument list.  So, I have flipped them.

* Impact: Low
* Mitigation: change the argument order of any usages

## Nullable (struct) extensions removed

These just keep causing problems with resolution of common functions like `Map`, `Bind`, etc.

The nullable Prelude remains.

## Support for `Tuple` and `KeyValuePair` removed

ValueTuple support only from now on.


## Types removed outright

Originally, I was going to just mark these as `[Obsolete]`.  And, for a while, they were.  But after over 1000 files changed with over 100,000 lines of code added or modified, I realised that maintaining these types (updating them to support nullable references and other fixups) was potentially doubling the effort I'd done up to that point.  So, I have just deleted them outright.  It is brutal, but it saved my sanity.

I am very, very, very sorry that this will mean you have to fixup everything before you can work with language-ext `v5`.  But unfortunately, I have to spread the load of this, as it was burning me to the ground!

This is my '.NET Framework to .NET Core' moment.  I realise that.  And I an truly sorry to those that have to do the migration.  Please make sure you have adequate time set aside for the migration.

* `Some<A>`
	* Mitigtation: use nullable references instead
* `OptionUnsafe<A>`
	* Mitigtation: use `Option<A?>` instead
* `OptionNone`:
	* Mitigtation: use `Fail<Unit>` instead
* `EitherUnsafe<L, R>`:
	* Mitigtation: use `Either<L?, R?>` instead
* `EitherLeft<L>`
	* Mitigtation: use `Fail<L>` instead
* `EitherRight<L>`:
	* Mitigtation: use `Pure<R>` instead
* `Try<A>`
	* Mitigtation: use `Eff<A>`
* `TryOption<A>`
	* Mitigtation: use `IO<Unit, A>` or `Eff<A>`
* `TryAsync<A>`
	* Mitigtation: use `Eff<A>`
* `TryOptionAsync<A>`
	* Mitigtation: use `IO<Unit, A>` or `Eff<A>`
* `Result<A>`
	* Mitigtation: use `Fin<A>`
* `OptionalResult<A>`
	* Mitigtation: use `Fin<A?>`
* Async extensions for `Option<A>` 
	* Mitigtation: use `ToAsync()` instead
* `ExceptionMatch`, `ExceptionMatchAsync`, `ExceptionMatchOptionalAsync`
	* Mitigtation: use effect monads with `@catch`


## Libraries removed outright


* `LanguageExt.SysX` - this was only needed to partition newer .NET Core code from .NET Standard.  This has now been merged into `LanguageExt.Sys`
* `LanguageExt.CodeGen` - is deprecated from now.  To be replaced later by `LanguageExt.SourceGen`.  Note, this library has always been standalone and can therefore continue to work without new versions being released.


# TODO

* Traverse / Sequence - generic system using Transducers
* Make TraverseParallel for Eff
* Find a way of resolving default implementations for classes now that we're using static interface methods.
* Test that resources are freed correctly in ResourceT when the result of Run is lazy
	* `bracket`
* `EitherT`, `TryT` (derives `EitherT<M, Error, A>)`, `Try` (derives `TryT<Identity, A>`)
* `yieldAll`, `many`, and `repeat` for Pipes needs tail recursion support
	* `yieldAll`, `many` have been temporarily removed