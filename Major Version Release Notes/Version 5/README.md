# Version 5.0.0 Major Release Notes

**_WORK IN PROGRESS NOTES_**

Version 5 of language-ext is huge.  It is both massive in terms of new capabilities delivered but also in _breaking changes_ that will require you to spend some time fixing up compilation failures and making judgements about your code.  An upgrade to `v5` should be considered a significant undertaking for a large code-base - please make sure you have the time before upgrading.  
I have made copious notes below to help you understand the changes and to ease the migration.  If, during the migration you encounter issues that are not in these notes, please let me know so I can update them for everybody else.  

Further contextual information about the updates can be [found on my blog](https://paullouth.com/higher-kinds-in-c-with-language-ext/) -- it digs a little deeper into the new capabilities and hopefully explains why these changes are necessary.

Language-ext is 10 years old this year, so please consider this my _once per decade refresh_.  It is very much my .NET Framework to .NET Core moment.


# Contents

* [Motivations](#motivations)
	* [Empower the users](#empower-the-users)
	* [Wage war on `async` (green threads)](#wage-war-on-async-green-threads)
	* [Leverage modern C# features](#leverage-modern-c-features)
* [New Features](#new-features)
	* [Higher-kinded polymorphism](#higher-kinded-polymorphism)
		* [Introduction](#introduction)
		* [Traits](#traits)
		* [Functor]()
		* [Applicative]()
		* [Monad]()
		* [Foldable]()
        * [Fallible]()
		* [Traversable]()
        * [SemigroupK]()
		* [MonoidK]()
		* [SemiAlternative]()
		* [Alternative]()
		* [Free monads]()
 	* [IO monad]()
       * [Auto-resource managment]() (`use` / `release`)
	* [Streaming effects]()
    * [Monad transformers (MonadT)]()
      * [OptionT]()
	  * [EitherT]()
	  * [FinT]()
	  * [ValidationT]()
      * [Reader and ReaderT]()
      * [Writer and WriterT]()
      * [State and StateT]()
      * [StreamT]()
	* [All computation based monads rewritten to use transformers]()
	* [Infinite recursion in monads]()
	* [`Pure` / `Fail` monads]()
	* [Lifting]()
	* [Improved guards]()
	* [Nullable annotations]()
	* [Collection initialisers]()
* [Breaking changes](#breaking-changes)
	* [netstandard2.0 no longer supported](#netstandard20-no-longer-supported)
	* [`Seq1` made `[Obsolete]`](#seq1-made-obsolete)
	* ['Trait' types now use static interface methods](#trait-types-now-use-static-interface-methods)
	* [The 'higher-kind' trait types have all been refactored](#the-higher-kind-trait-types-have-all-been-refactored)
	* [The `Semigroup<A>` and `Monoid<A>` types have been refactored](#the-semigroupa-and-monoida-types-have-been-refactored)
	* [The static `TypeClass` class has been renamed `Trait`](#the-static-typeclass-class-has-been-renamed-trait)
	* [`Apply` extensions that use raw `Func` removed](#apply-extensions-that-use-raw-func-removed)
	* [Manually written `Sequence` extension methods have been removed (#1)](#manually-written-sequence-extension-methods-have-been-removed-1)
	* [Manually written `Sequence` extension methods have been removed (#2)](#manually-written-sequence-extension-methods-have-been-removed-2)
	* [Manually written `Sequence` extension methods have been removed (#3)](#manually-written-traverse-extension-methods-have-been-removed-3)
	* [`ToComparer` doesn't exist on the `Ord<A>` trait any more](#tocomparer-doesnt-exist-on-the-orda-trait-any-more)
	* [Renamed `LanguageExt.ClassInstances.Sum`](#renamed-languageextclassinstancessum)
	* [`Guard<E>` has become `Guard<E, A>`](#guarde-has-become-guarde-a)
	* [Existing uses of `HasCancel<RT>` should be replaced with `HasIO<RT>`](#existing-uses-of-hascancelrt-should-be-replaced-with-hasiort)
	* [`UnitsOfMeasaure` namespace converted to a static class](#unitsofmeasaure-namespace-converted-to-a-static-class)
	* [`Either` doesn't support `IEnumerable<EitherData>` any more](#either-doesnt-support-ienumerableeitherdata-any-more)
	* [`Either` 'bi' functions have their arguments flipped](#either-bi-functions-have-their-arguments-flipped)
	* [Nullable (struct) extensions removed](#nullable-struct-extensions-removed)
	* [Support for `Tuple` and `KeyValuePair` removed](#support-for-tuple-and-keyvaluepair-removed)
	* [Types removed outright](#types-removed-outright)
		* [`Some<A>`](#somea)
		* [`OptionNone`](#optionnone)
		* [`EitherUnsafe<L, R>`](#eitherunsafel-r)
		* [`EitherLeft<L>`](#eitherleftl)
		* [`EitherRight<L>`](#eitherrightl)
		* [`Try<A>`](#trya)
		* [`TryOption<A>`](#tryoptiona)
		* [`TryAsync<A>`](#tryasynca)
		* [`TryOptionAsync<A>`](#tryoptionasynca)
		* [`Result<A>`](#resulta)
		* [`OptionalResult<A>`](#optionalresulta)
		* [Async extensions for `Option<A>`](#async-extensions-for-optiona)
		* [`ExceptionMatch`, `ExceptionMatchAsync`, `ExceptionMatchOptionalAsync`](#exceptionmatch-exceptionmatchasync-exceptionmatchoptionalasync)
        * [`NewType`, `NumType`, `FloatType`]()
	* [Libraries removed outright](#libraries-removed-outright)
		* [`LanguageExt.SysX`](#languageextsysx)
		* [`LanguageExt.CodeGen`](#languageextcodegen)	
		* [`LanguageExt.Transformers`](#languageexttransformers)	

# Motivations

* Empower the users
* Wage war on async (green threads)
* Support for transducers
* Once per decade refresh

## Empower the users

If you spend any time with Haskell or other languages with higher-kinds, you'll notice a heavy use of higher-kinded polymorphism.  From functors, applicatives, monads, foldables, traversables, monad transformers, etc.  This flavour of polymorphism allows for compositional superpowers that we can only dream of in C#.

_Composition of pure functional components_ always leads to new, more capable components, that are also **automatically** pure.  This is the true power of pure functional composition; unlike OO composition that just collects potential complexity, pure functional composition abstracts away from it.  You can trust the composition to be sound.

Much of this library up until this point has been trying to give you the capability to build these compositions.  It's been achieved so far by lots of typing by me and lots of code-gen.  For example, the `LanguageExt.Transformers` library was a code-genned mass of source-code that combined every monadic type with every other monadic type so that you can work on them nested.  It was 200,000 lines of generated code - and was getting exponentially worse.

I've also written 100s (maybe 1000s) of `Select` and `SelectMany` implementations that give you the impression that C# has generalised monads.  But it doesn't, it's just a lot of typing.  If you create your own monadic type it won't magically work with any existing language-ext types and you can't write general functions that accept any monad and have it just work.

> And that is very much because C# doesn't support higher-kinded polymorphism.

That means that you pretty much only get to use what I provide.  That's not acceptable to me.  I want to empower everybody to be able to leverage pure functional composition in the same way that can be done in Haskell (and other languages that have higher-kinds).  A series of articles on higher-kinds [features on on my blog](https://paullouth.com/higher-kinds-in-c-with-language-ext/).

## Wage war on `async` (green threads)

If I continued the way I was before then every monadic type would have an `*Async` variant, as would every method and function.  This was getting out of hand.  For something like an IO/effect monad where you could also have an optional error-type and optional runtime-type that meant 8 types.  Each with 1000s of lines of code to define them.  Then, when you think there's 20 or so 30 or so monadic types, it becomes a big maintenence problem.  There's also issues around consistency between each type (making sure everything has a `MapAsync`, `BindAsync`, etc.) - as well as making sure sync types can work with async types, etc.

So, as of now, this library stands against 'declarative async' - i.e. we are  adopting a _'green threads mentality'_.  That is we will not be giving you `*Async` variants of anything.  All IO computation types (`IO` and `Eff`) will support the _lifting_ of both synchronous and asynchronous functions, but you won't see evidence of asynchronicity in any type-signatures.

Those types each have a `Run()` function which _appear_ to run synchronously, i.e. they don't return a `Task<A>`.  In fact, they don't run synchronously, they run _concurrently_.  Internally, they use similar mechanics to `Task` to yield time to your current thread whilst waiting for their own IO operations to complete.  So, calling `operation.Run()` is the same as calling `await operation.RunAsync()` - you just don't need the rest of your code infected by `async`.

When you want an operation not to run concurrently, but in parallel instead (i.e. queue the work to be run on the next available `ThreadPool` thread), you can call `operation.Fork()`.  It supports fire-and-forget, so `operation.Fork().Run()` returns immediately, or you can await the result:
```c#
var forkedOperation = from f in operation.Fork() // runs in parallel
                      from r in f.Await		     // use the ForkIO to await the result
                      select r;

// This will yield the current thread to allow concurrency, whilst the forked
// operation runs on another thread.
var result = forkedOperation.Run();
```
To lift an existing `Task` based function into these types you can just call:

* `liftIO(Func<EnvIO, Task<A>> function)`
* `liftIO(Func<Task<A>> function)`

Which are both in the `Prelude` and allow an `IO` operation to be lifted into any monad that supports IO.  `EnvIO` gives you access to the `CancellationToken`, `CancellationTokenSource`, and `SynchronizationConrext`.  

For example:

```c#
var operation = from text in liftIO(env => File.ReadAllTextAsync(path, env.Token))
                let lines = text.Split("\r\n").ToSeq()
                select lines;
```
We can then run that operation:
```c#
Seq<string> result = operation.Run();
```
And we get a concrete `Seq<string>` - not a `Task<Seq<string>>`, but the operation ran concurrently.


## Leverage modern C# features

The library has been held back by the need to support .NET Framework.  As of now this library is .NET (formerly known as .NET Core) only.  Instantly jumping to .NET 8.0 (which has Long Term Support).

This opens up: static interface members (which allows the trait/ad-hoc polymorphism support to get a power-up) and collection initialisers for all of the immutable collections - amongst others.


# New Features

## Higher-kinded polymorphism

### Introduction
So, what is higher-kinded polymorphism?  Think of a function like this:

```c#
static Option<int> AddOne(Option<int> mx) =>
	mx.Map(x => x + 1);
```
That's a function that takes an `Option` and leverages its `Map` function to add one to the value inside.  The `Map` function makes it a 'Functor'.  Functors map.  

But, if functors map, and all functors have a `Map` method, why can't I write:
```c#
static F<int> AddOne<F>(F<int> mx) where F : Functor =>
	mx.Map(x => x + 1);
```
It's because we can only make the *lower-kind* polymorphic.  For example, I can write a function like this:
```c#
static Option<string> Show(Option<A> mx) =>
	mx.Map(x => x.ToString());
```
Where the lower-kind, the `A`, is parametric - but not the _higher-kind_ (the `F` in the previous example).

You might think we could just do this with interfaces:

```c#

interface Functor<A>
{
    Functor<B> Map<B>(Func<A, B> f);
}

public class Option<A> : Functor<A>
{
    // ...    
}
public class Seq<A> : Functor<A>
{
    // ...    
}

```
On the surface, that looks like we can then just accept `Functor<A>` and call map on it.  The problem is that we really shouldn't mix and match different types of functor (same with monads, applicatives, etc).  We've lost the information on what's inside the functor.  Every time we call `Map` on `Option` it stops being an `Option`, so we can't then call `Bind`, or any other useful functions, we're stuck doing `Map` forever.

## Traits

C# has recently [introduced static interface members](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/static-virtual-interface-members).  It allows us to create ['trait types'](https://en.wikipedia.org/wiki/Trait_%28computer_programming%29).  Users of language-ext know this approach from the TypeClasses and ClassInstances technique.  But, with static interface methods the approach has become much more elegant and usable.  

> language-ext is now .NET 8 only - mostly to leverage static interface methods

So, now what we can do is define a really simple type:

```c#
public interface K<F, A>
```
This one type will change your life!

Remember, we could't create a higher-kinded type like `F<A>`, but we can create this: `K<F, A>`.  It has no members, it's a completely empty interface, we simply use this as a 'marker' for our types so that they can leverage higher-kinded polymorphism.  If you look at any of the major types in language-ext `v5` you'll see the `K` type being used (`K` is short for 'Kind'):

This is `Option<A>`:
```c#
public readonly struct Option<A> :
    K<Option, A>
{
    ...
}
```
It locks its `F` type to be `Option` (notice the lack of an `A`, it's just `Option`).

If you then go and look at `Option`, you'll notice it inherits some really interesting interfaces!

```c#
public class Option : 
	Monad<Option>, 
	Traversable<Option>, 
	Alternative<Option>
{
	// ...
}
```
* `Monad` inherits `Applicative` and `Functor`
* `Traversable` inherits `Functor` and `Foldable`

And, if you and look at those interfaces you'll see the static interface methods that all leverage the `K` kind-type:

```c#
public interface Functor<F>  
    where F : Functor<F>
{
    public static abstract K<F, B> Map<A, B>(Func<A, B> f, K<F, A> ma);
}
```

Now, let's look at the Haskell defintion of `Functor`:

```haskell
class Functor f where
    fmap :: (a -> b) -> f a -> f b
```

Notice how the type is parameterised by `f` (just like `Functor<F>`), and how it takes a higher-kinded value of `f a`, which is the same as our `K<F, A>`, and it returns a higher-kinded value of `f b`, which is our `K<F, B>`.

> We have the same defintion as Haskell.  Exactly the same!

What does this mean?  Well, we can now write generic functions that work with functors, applicatives, monads, monad-transformers, traversables, alternatives, foldables, state monads, reader monads, writer monads...

Here's the example from before:

```c#
public static K<F, int> AddOne<F>(K<F, int> mx) where F : Functor<F> =>
	F.Map(x => x + 1, mx);
```
It calls the **static** `Map` function on the `F` trait, which is implemented by each type.  Here's the implementation for `Option`

```c#
public class Option :
	Monad<Option>, 
	Traversable<Option>, 
	Alternative<Option>
{
	// ...

	public static K<Option, B> Map<A, B>(Func<A, B> f, K<Option, A> ma) => 
	    ma.As().Map(f);

	// ...

}
```
Remember, `Option<A>` inherits from `K<Option, A>`, so we can just downcast it and call the `Option<A>` implementation of `Map`.  The `As()` method does the downcast from `K<Option, A>` to `Option<A>`.

> You may think downcasting is a bit risky here, but really nothing else should inherit from `K<Option, A>`.  Doing so only makes sense for `Option<A>`.  I think the risk of a casting issue is close to zero.

So, just to be clear.  Every type, like `Option<A>`, `Seq<A>`, `Eff<A>`, etc. has a sibling type of the same name with the last generic parameter removed. So, `Option<A>` has a sibling type of `Option`, `Seq<A>` has `Seq`, etc.  Those sibling types implement the traits, like `Monad<M>`, `Functor<F>`, etc.  And, because `Option<A>`, `Seq<A>`, etc. all inherit from `K<TRAIT, A>` - where `TRAIT` is `Option`, `Seq`, `Eff`; this allows generic functions that have constriants like `where F : Functor<F>` to 'find' the bespoke implementation.

> Types like `Either<L, R>`, that have multiple generic arguments, again just lose the last argument for their sibling type: `Either<L>`.

Invoking the trait functions directly isn't that elegant - although perfectly usable - so there's extension methods that work with all of the abstract traits.  Here's the above `AddOne` method rewritten to use the `Map` extension instead:

```c#
public static K<F, int> AddOne<F>(K<F, int> mx) where F : Functor<F> =>
	mx.Map(x => x + 1);
```

So, let's try it out by calling it with a number of functors:

```c#
K<Option, int> mx = AddOne(Option<int>.Some(10));
K<Seq, int>    my = AddOne(Seq<int>(1, 2, 3, 4));
K<Fin, int>    mz = AddOne(Fin<int>.Succ(123));
```
Note, the return types have the 'trait' baked in.  So you know it's still an option, seq, fin, etc.  

> Without full support for higher-kinds (from the C# language team) we can't do better than that.  

However, there are extensions to help get back to the original type.  Just call: `As()`.

```c#
Option<int> mx = AddOne(Option<int>.Some(10)).As();
Seq<int>    my = AddOne(Seq<int>(1, 2, 3, 4)).As();
Fin<int>    mz = AddOne(Fin<int>.Succ(123)).As();
```
You only need to do that when you 'realise the concrete type'.  Because the trait (`Option`, `Seq`, `Fin`, etc.) is the type that inherits `Monad`, `Applicative`, `Traversable`, etc. (not `Option<A>`, `Seq<A>`, `Fin<A>`, ) - you can just call their capabilities directly off the `K` value:

```c#
Option<int> mx = AddOne(Option<int>.Some(10))
                     .Bind(x => Option<int>.Some(x + 10))
                     .Map(x => x + 20)
                     .As();

Seq<int> mx = AddOne(Seq<int>(1, 2, 3, 4))
                  .Bind(x => Seq(x + 10))
                  .Map(x => x + 20)
                  .As();

```


Just this capability alone has alowed me [to delete nearly 200,000 lines of generated code](https://github.com/louthy/language-ext/commit/c4c9df3b3b2fd9f0eaf0850742ce309948eea0d7).  That is incredible!


************ CONTINUE HERE ***************





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

### `Error` no longer implicitly convertable from `String`

It seemed like a good idea at the time, but can easily cause problems with `Error` carrying types like `Fin` (`Fin<string>` in particular).  To avoid the confusion I have made it an `explicit` conversion operation.

**Impact**

Low

**Mitigation** 

Manually cast to `Error` where needed - or call `Error.New(string)`

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

### Renaming of methods in Arithmetic trait

Arithmetic trait: `Plus` renamed `Add`, `Product` renamed `Multiply`.

### The `Semigroup<A>` and `Monoid<A>` types have been refactored

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


### Renamed `LanguageExt.ClassInstances.Sum`

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

### Existing uses of `HasCancel<RT>` should be replaced with `HasIO<RT>`

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

### `Some<A>`
	* Mitigtation: use nullable references instead
### `OptionUnsafe<A>`
	* Mitigtation: use `Option<A?>` instead
### `OptionAsync<A>`
	* Mitigtation: use `OptionT<IO, A>` instead
### `OptionNone`:
	* Mitigtation: use `Fail<Unit>` instead
### `EitherUnsafe<L, R>`:
	* Mitigtation: use `Either<L?, R?>` instead
### `EitherLeft<L>`
	* Mitigtation: use `Fail<L>` instead
### `EitherRight<L>`:
	* Mitigtation: use `Pure<R>` instead
### `EitherAsync<L, R>`:
	* Mitigtation: use `EitherT<L, IO, R>` instead
### `Try<A>`
	* Mitigtation: use `Eff<A>`
### `TryOption<A>`
	* Mitigtation: use `OptionT<IO, A>`
### `TryAsync<A>`
	* Mitigtation: use `Eff<A>`
### `TryOptionAsync<A>`
	* Mitigtation: use `OptionT<IO, A>`
### `Result<A>`
	* Mitigtation: use `Fin<A>`
### `OptionalResult<A>`
	* Mitigtation: use `Fin<A?>`
### Async extensions for `Option<A>` and `Either<L, R>`
	* Mitigtation: use `ToIO()` to convert to the transformer variants with embedded `IO`
### `ExceptionMatch`, `ExceptionMatchAsync`, `ExceptionMatchOptionalAsync`
	* Mitigtations: 
		* use effect monads with `@catch`
		* use `switch` expressions


## Libraries removed outright

### `LanguageExt.SysX`

this was only needed to partition newer .NET Core code from .NET Standard.  This has now been merged into `LanguageExt.Sys`

### `LanguageExt.CodeGen` 

Deprecated from now.  To be replaced later by `LanguageExt.SourceGen`.  Note, this library has always been standalone and can therefore continue to work without new versions being released.

### `LanguageExt.Transformers` 

No need for them now we have proper higher-kind support.

# TODO

* Traverse / Sequence - generic system using Transducers
* Make TraverseParallel for Eff
* ~~Find a way of resolving default implementations for classes now that we're using static interface methods (IL.cs).~~
* Test that resources are freed correctly in ResourceT when the result of Run is lazy
	* `bracket`
* `EitherT`, `TryT` (derives `EitherT<M, Error, A>)`, `Try` (derives `TryT<Identity, A>`)
* `yieldAll`, `many`, and `repeat` for Pipes needs tail recursion support
	* `yieldAll`, `many` have been temporarily removed
* `yieldAll` in Pipes has a temporary solution - need proper recursion strategy
* ~~Use alpha and beta versioning like like: `5.0.0-alpha.1`, `5.0.0-alpha.2`, etc. So we can release `5.0.0` when done~~
* Make ForkIO.Await respect two cancellation tokens, the original and the one from the runner of Await 
* ~~Make Eff a ReaderM, ResourceM, etc. -- so we don't have to do so much manual lifting~~
* Overrides of Foldable for the sequence types (`Count()`, `Head()`, `Last()` etc.)
* Make sure Index is used properly in collections this[] implementations (namely, from end!)
* Make sure correct trait is used between MonoidK and Alternative -- the collections in particular should be MonoidK as the Combine usually concats the collections (rather than provides an Alternative).
* Review `Prelude_Collections`
* Write unit tests (generally!)
* Write unit tests for index operator on lists and foldables [^1]
* Add IComparisonOperators / IAdditionOperators / etc. to types
* Make NewTypes support appropriate traits
* Add Applicatice.Actions to every applicative that has an error state
* Exception catching in Producer.merge
* Implement `TraverseM` for each data-type (like the `Traverse` implementations)
