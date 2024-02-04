# Version 5.0.0 Major Release Notes

## Motivations

### Empower the users of this library to create their own functional types

If you spend any time with Haskell or other languages with higher-kinds, you'll notice a heavy use of 'monad transformers'.  Monad Transformers allow you to _'stack'_ existing monadic types - and in the process compose their behaviours.  A naive way to think of this is a bit like multiple-inheritance in OO (it's much better than that though!)

But, because Monad Transformers need higher-kinds they can't be implemented in C#.  However, when you get down to basics, monads are just a particular 'flavour' of function compostion.  Monadic types just inject a few rules in-between the compositions to make them behave to their 'flavour' (like optional values, iterations, logging, reading/writing state, etc.)

> _It is then possible to consider that if one had access to the `Bind` function of `Option` and the `Bind` function of `State` then you could compose them into a new `Bind` function that had the behaviour of both_.  

But how do we standardise the `Bind` function for any monadic type and make it available for composition.  This is where Transducers come in.  They are literally designed to be compositional pipeline components.  You could think of them as the LEGO bricks of functional programming (after functions of course! but transducers allow us to embelish our functions with addtional functionality, like resource tracking, streaming, optional behaviour, etc.)  

Therefore I have introduced Transducers as a new core capability in language-ext.  And all of the monadic types are now either implemented with Transducers or have the option to convert to a Transducer.


### Reduce the type explosion due to `async`

If I continued the way I was before then every monadic type would have an `*Async` variant.  This was getting out of hand.  For something like an IO/effect monad where you could also have an optional error-type and optional runtime-type that meant 8 types.  Each with 1000s of lines of code to define them.  Then, when you think there's 20 or so 30 or so monadic types, it becomes a big maintenence problem.  There's also issues around consistency between each type (making sure everything has a `MapAsync`, `BindAsync`, etc.).

So, as of now, this library is against 'declarative async' - i.e. it's adopting a _'green threads mentality'_.  That is we will not be giving you `*Async` variants of anything.  All computation types (`IO`, `Eff`, `Reader`, `Proxy`, etc.) will support the _lifting_ of both synchronous and asynchronous functions.

Many of the `*Async` functions like `MapAsync`, `BindAsync` are also gone.  There is now `Map`, `Bind`, etc. that accept `Transducer` types (as well as the regular `Func` version).  `Transducer` supports `liftAsync` to lift an asynchronous function into a `Transducer` - it can therefore do async operations without lots of extra boilerplate support for async elsewhere.

### Leverage modern C# features

The library has been held back by the need to support .NET Framework.  As of now this is a .NET Core only library.  Instantly jumpimg to .NET 8.0 (which has Long Term Support).

This opens up: static interface members (which allows the trait/ad-hoc polymorphism support to get a power-up) and collection initialisers for all of the immutable collections - amongst others.


## New Features

- IO monad
- Transducers
- Recursive effects
- Streaming effects
- Auto-resource managment (`use` / `release`)
- `Pure` / `Fail`
- Lifting
- Improved guards
- Nullable annotations
- Collection initialisers

## Breaking changes

### `netstandard2.0` no longer supported

I held off for as long as I could, but there are lots of new C# features that this library can make use of (primarily static interfaces, but others too); so it's time to leave .NET Framework behind and focus on .NET Core.  This version jumps straight to `net8.0` support.

* Impact: High (if you're still on .NET Framework)
* Mitigation: Migrate your application to .NET Core

### 'Trait' types now use static interface methods

Before static interface methods existed, the technique was to rely on the non-nullable nature of structs to get access to 'static' methods (via interface based constraints), by calling `default(TRAIT_TYPE).StaticMethod()`.

Language-ext has many of these 'trait types', like `Eq<A>`, `HasCancel<A>`, etc.  They have all been updated to use `static abstract` methods.

So, where before you might call: `default(EqA).Equals(x, y)` (where `EqA` is `struct, Eq<A>`) - you now need to call `EqA.Equals(x, y)` (where `EqA` is `Eq<A>`) .  

This is obviously much more elegant and removes the need for the `struct` constraint.

If you have implemented any of these traits, as instances, then you'll need to implement these changes:

* Remove the `struct` from any constraints (`where X : struct`)
* Add `static` to trait method implementations
* Any default `Inst` usages should be removed
* The types can still be implemented as structs, so that doesn't need to change, but they can be implemented with any instance type.

* Impact: Medium - your code will throw up lots of 'Cannot access static method' errors.  It is a fairly mechanical processes to fix them up.  

### `ToComparer` doesn't exist on the `Ord<A>` trait any mroe

Because the trait types now use `static` methods, we can't now have a `ToComparer()` extension for the `Ord<A>` type.  Instead there's a class called `OrdComparer` that contains a singleton `IComparer` property called `Default`.

* Impact: Low
* Use `OrdComparer<OrdA, A>.Default` instead of `<OrdA>.ToComparer()`.

### Renamed `LanguageExt.ClassInstances.Sum<NUM, A>`

Renamed to `LanguageExt.ClassInstances.Addition<SUM, A>`

There's a new type called `Sum<L, R>` for use with transducers.  

* Impact: Low
* Mitigation: Rename uses of `Sum<NUM, A>` to `Addition<NUM, A>`


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

## Types made obsolete

* `Aff<RT, A>`, `Aff<A>`, `OptionAsync`, `EitherAsync`, `TryAsync`, `TryOptionAsync` have all been made obsolete, in line with this proposal: https://github.com/louthy/language-ext/discussions/1269.  See the _Type mapping_ table in the proposal of how to migrate.
	* `runtime<RT>()` now returns a `Transducer`, to continue to use it with `Aff` call `runtime<RT>().ToAff()`


## Types removed outright

* `Some<A>` - use nullable references instead
* `OptionUnsafe<A>` - use `Option<A?>` instead
* `OptionNone` - use `Fail<Unit>` instead
* `EitherUnsafe<L, R>` - use `Either<L?, R?>` instead
* `EitherLeft<L>` - use `Fail<L>` instead
* `EitherRight<L>` - use `Pure<R>` instead
* Async extensions for `Option<A>` - use `ToAsync()` instead
* `ExceptionMatch`, `ExceptionMatchAsync`, `ExceptionMatchOptionalAsync` - use effect monads with `@catch`

# TODO

* Traverse / Sequence - generic system using Transducers
* Make TraverseParallel for Eff