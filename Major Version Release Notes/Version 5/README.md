# Version 5.0.0 Major Release Notes

## New Features

- IO monad
- Transducers
- Recursive effects
- Streaming effects
- Auto-resource managment (Use / Release)
- Pure / Fail
- Improved guards


## Breaking changes

### Renamed `LanguageExt.ClassInstances.Sum<NUM, A>`

Renamed to `LanguageExt.ClassInstances.Addition<SUM, A>`

There's a new type called `Sum<L, R>` for use with transducers.  

* Impact: Low


###  `Guard<E>` has become `Guard<E, A>`

The `A` is never used, this just allows guards to work in LINQ by enabling the implementation of `SelectMany`.  The benefit is that a number of guards can be placed together in a LINQ statement, where only one could before.

* Impact: Zero unless you've written your own code to work with `Guard`.  If you only ever used the `guard` Prelude function this will have no impact.
