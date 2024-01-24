# Version 5.0.0 Major Release Notes

## New Features

- IO monad
- Transducers
- Recursive effects
- Streaming effects
- Auto-resource managment (Use / Release)
- Pure / Fail
- Lifting
- Improved guards

## Breaking changes

### `netstandard2.0` no longer supported

I held off for as long as I could, but there are lots of new C# features that this library can make use of, so it's time to lead .NET Framework behind and focus on .NET Core.  This version jumps straight to `net8.0` support.

### 'Trait' types now use static interface methods

Before static interface methods existed, the technique was to rely on the non-nullable nature of structs to get access to 'static' methods, by calling `default(TRAIT_TYPE).StaticMethod()`.

Language-ext has many of these 'trait types', like `Eq<A>`, `HasCancel<A>`, etc.  They have all been updated to use `static abstract` methods.

So, where before you might call: `default(EqA).Equals(x, y)` (where `EqA` is `struct, Eq<A>`) - you now need to call `EqA.Equals(x, y)`.  

This is obviously much more elegant and removes the need for the `struct` constraint.

If you have implemented any of these traits, as instances, then you'll need to implement these changes:

* Remove the `struct` from any constraints (`where X : struct`)
* Add `static` to trait-method implementations
* Any default `Inst` implementaions should be removed
* The types can still be implemented as structs, so that doesn't need to change, but they can be implemented with any instance type.

* Impact: Medium - your code will throw up lots of 'Cannot access static method' errors.  It is a fairly mechanical processes to fix them up.  


### Renamed `LanguageExt.ClassInstances.Sum<NUM, A>`

Renamed to `LanguageExt.ClassInstances.Addition<SUM, A>`

There's a new type called `Sum<L, R>` for use with transducers.  

* Impact: Low


###  `Guard<E>` has become `Guard<E, A>`

The `A` is never used, this just allows guards to work in LINQ by enabling the implementation of `SelectMany`.  The benefit is that a number of guards can be placed together in a LINQ statement, where only one could before.

* Impact: Zero unless you've written your own code to work with `Guard`.  If you only ever used the `guard` Prelude function this will have no impact.

### Existing uses of `HasCancel<RT>` should be replaced with `HasIO<RT, Error>`

The basic runtime traits needed by the effect monads (`IO`, `Eff`, and `Aff`) has been expanded to `HasCancel<RT>`, `HasFromError<RT, E>`, and `HasSyncContext<RT>`.  These can all be applied manually or you can use the `HasIO<RT, E>` trait which wraps all three traits into a single trait.

Simply search and replace: `HasCancel<RT>` for `HasIO<RT, Error>`.  This will work for all existing `Eff` and `Aff` code.  

* `HasSyncContext<RT>` allows all `IO` and `Eff` monads to use the `post(effect)` function.  This allows the running of an effect on original `SynchronizationContext` (needed for WPF and Windows Form applications).
* `HasFromErrror<RT, E>` allows conversion of `Error` to `E`.  The new `IO` monads have an parametric error type - and so this allows for exceptional errors and expecteded `Error` values to be converted to the parametric error type: `E`.

## Types made obsolete

* `Aff<RT, A>`, `Aff<A>`, `OptionAsync`, `EitherAsync`, `TryAsync`, `TryOptionAsync` have all been made obsolete, in line with this proposal: https://github.com/louthy/language-ext/discussions/1269.  See the _Type mapping_ table in the proposal of how to migrate.
	* `runtime<RT>()` now returns a `Transducer`, to continue to use it with `Aff` call `runtime<RT>().ToAff()`

