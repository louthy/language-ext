![lang-ext](https://github.com/louthy/language-ext/blob/master/backers-images/banner.png?raw=true)

C# Functional Programming Language Extensions
=============================================

[![Join the chat at https://gitter.im/louthy/language-ext](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/louthy/language-ext?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) 

This library uses and abuses the features of C# to provide a functional-programming 'Base class library', that, if you squint, can look like 
extensions to the language itself.  The desire here is to make programming in C# much more reliable and to make the engineer's inertia flow 
in the direction of declarative and functional code rather than imperative.

__Author on twitter:__ 
https://twitter.com/paullouth

## Reference

#### [API Reference](https://louthy.github.io/language-ext)

#### [Issues that contain documentation and examples](https://github.com/louthy/language-ext/issues?utf8=%E2%9C%93&q=is%3Aissue%20label%3A%22examples%20%2F%20documentation%22%20)

## Contributing & Code of Conduct

If you would like to get involved with this project, please first read the [Contribution Guidelines](https://github.com/louthy/language-ext/blob/master/CONTRIBUTING.md) and the [Code of Conduct](https://github.com/louthy/language-ext/blob/master/CODE_OF_CONDUCT.md)

## Nu-get

Nu-get package | Description
---------------|-------------
[LanguageExt.Core](https://www.nuget.org/packages/LanguageExt.Core) | All of the core types and functional 'prelude'.  __This is all that's needed to get started__.
[LanguageExt.FSharp](https://www.nuget.org/packages/LanguageExt.FSharp) | F# to C# interop library.  Provides interop between the LanguageExt.Core types (like `Option`, `List` and `Map`) to the F# equivalents, as well as interop between core BCL types and F#
[LanguageExt.Parsec](https://www.nuget.org/packages/LanguageExt.Parsec) | Port of the [Haskell parsec library](https://hackage.haskell.org/package/parsec)
[LanguageExt.Rx](https://www.nuget.org/packages/LanguageExt.Rx) | Reactive Extensions support for various types within the Core
[LanguageExt.CodeGen](https://www.nuget.org/packages/LanguageExt.CodeGen) | Used to generate lenses and `With` functions automagically for record types. 

## Code-gen setup

To use the code-generation features of language-ext (which are totally optional by the way), then you must include the [LanguageExt.CodeGen](https://www.nuget.org/packages/LanguageExt.CodeGen) package into your project.  

To make the reference **build and design time only** (i.e. your project doesn't gain an additional dependencies because of the code-generator), open up your `csproj` and set the `PrivateAssets` attribute to `all`:
```c#
<DotNetCliToolReference Include="dotnet-codegen" Version="0.5.13" />
<PackageReference Include="LanguageExt.CodeGen" Version="3.1.24" PrivateAssets="all" />
```

> Obviously, update the `Version` attributes to the appropriate values.  Also note that you will probably need the latest VS2019+ for this to work.  Even early versions of VS2019 seem to have problems.

## Unity

This library seems compatible on the latest (at the time of writing) Unity 2018.2 with __incremental compiler__ (which enables C# 7).
So this library should work well once Unity has official support for C# 7 on upcoming 2018.3.
In the meanwhile, you can install incremental compiler instead. 
If you are concerned about writing functionally and the possible performance overheads then please take a look at [this wiki page](https://github.com/louthy/language-ext/wiki/Performance).

## Introduction
One of the great features of C#6+ is that it allows us to treat static classes like namespaces.  This means that we can use static 
methods without qualifying them first.  That instantly gives us access to single term method names that look exactly like functions 
in functional languages.  i.e.
```C#
    using static System.Console;
    
    WriteLine("Hello, World");
```
This library tries to bring some of the functional world into C#.  It won't always sit well with the seasoned C# OO programmer, 
especially the choice of camelCase names for a lot of functions and the seeming 'globalness' of a lot of the library.  

I can understand that much of this library is non-idiomatic; But when you think of the journey C# has been on, is idiomatic 
necessarily right?  A lot of C#'s idioms are inherited from Java and C# 1.0.  Since then we've had generics, closures, Func, LINQ, 
async...  C# as a language is becoming more and more like a  functional language on every release.  In fact the bulk of the new 
features are either inspired by or directly taken from features in functional languages.  So perhaps it's time to move the C# 
idioms closer to the functional world's idioms?

__A note about naming__

One of the areas that's likely to get seasoned C# heads worked up is my choice of naming style.  The intent is to try and make 
something that 'feels' like a functional language rather than follows the 'rule book' on naming conventions (mostly set out by 
the BCL).  

There is however a naming guide that will stand you in good stead whilst reading through this documentation:

* Type names are `PascalCase` in the normal way
* The types all have constructor functions rather than public constructors that you instantiate with `new`.  They will always 
be `PascalCase`:
```C#
    Option<int> x = Some(123);
    Option<int> y = None;
    List<int> items = List(1,2,3,4,5);
    Map<int, string> dict = Map((1, "Hello"), (2, "World"));
```
* Any (non-type constructor) static function that can be used on its own by `using static LanguageExt.Prelude` are `camelCase`.
```C#
    var x = map(opt, v => v * 2);
```
* Any extension methods, or anything 'fluent' are `PascalCase` in the normal way
```C#
    var x = opt.Map(v => v * 2);
```
Even if you don't agree with this non-idiomatic approach, all of the `camelCase` static functions have fluent variants, so actually 
you never have to see the 'non-standard' stuff. 

_If you're not using C# 6 yet, then you can still use this library.  Anywhere in the docs below where you see a camelCase function 
it can be accessed by prefixing with `Prelude.`_

### Getting started

To use this library, simply include `LanguageExt.Core.dll` in your project or grab it from NuGet. And then stick this at the top of each cs file that needs it:
```C#
using LanguageExt;
using static LanguageExt.Prelude;
```

The namespace `LanguageExt` contains the core types, and `LanguageExt.Prelude` contains the functions that you bring into scope `using static LanguageExt.Prelude`.  

### Features

Location | Feature | Description
---------|---------|------------
`Core` | `Arr<A>` | [Immutable array](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Arr_A.htm)
`Core` | `Lst<A>` | [Immutable list](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Lst_A.htm)
`Core` | `Map<K, V>` | [Immutable map](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Map_K_V.htm)
`Core` | `Map<OrdK, K, V>` | [Immutable map with Ord constraint on `K`](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Map_OrdK_K_V.htm)
`Core` | `HashMap<K, V>` | [Immutable hash-map](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/HashMap_K_V.htm)
`Core` | `HashMap<EqK, K, V>` | [Immutable hash-map with Eq constraint on `K`](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/HashMap_EqK_K_V.htm)
`Core` | `Set<A>` | [Immutable set](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Set_A.htm)
`Core` | `Set<OrdA, A>` | [Immutable set with Ord constraint on `A`](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Set_OrdA_A.htm)
`Core` | `HashSet<A>` | [Immutable hash-set](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/HashSet_A.htm)
`Core` | `HashSet<EqA, A>` | [Immutable hash-set with Eq constraint on `A`](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/HashSet_EqA_A.htm)
`Core` | `Que<A>` | [Immutable queue](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Que_T.htm)
`Core` | `Stck<A>` | [Immutable stack](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Stck_T.htm)
`Core` | `Option<A>` | [Option monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Option_A.htm) that can't be used with `null` values
`Core` | `OptionAsync<A>` | [OptionAsync monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/OptionAsync_A.htm) that can't be used with `null` values with all value realisation does asynchronously
`Core` | `OptionUnsafe<T>` | [Option monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/OptionUnsafe_A.htm) that can be used with `null` values
`Core` | `Either<L,R>` | [Right/Left choice monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Either_L_R.htm) that won't accept `null` values
`Core` | `EitherUnsafe<L, R>` | [Right/Left choice monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/EitherUnsafe_L_R.htm) that can be used with `null` values
`Core` | `EitherAsync<L, R>` | [EitherAsync monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/EitherAsync_L_R.htm) that can't be used with `null` values with all value realisation done asynchronously
`Core` | `Try<A>` | [Exception handling lazy monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Try_A.htm)
`Core` | `TryAsync<A>` | [Asynchronous exception handling lazy monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/TryAsync_A.htm)
`Core` | `TryOption<A>` | [Option monad with third state](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/TryOption_A.htm) 'Fail' that catches exceptions
`Core` | `TryOptionAsync<A>` | [Asynchronous Option monad with third state](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/TryOptionAsync_A.htm) 'Fail' that catches exceptions
`Core` | `Record<A>` | [Base type for creating record types](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Record_RECORDTYPE.htm)  with automatic structural equality, ordering, and hash code calculation.
`Core` | `Lens<A, B>` | [Well behaved bidirectional transformations](#transformation-of-nested-immutable-types-with-lenses) - i.e. the ability to easily generate new immutable values from existing ones, even when heavily nested.
`Core` | `Reader<E, A>` | [Reader monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Reader_Env_A.htm)
`Core` | `Writer<MonoidW, W, T>` | [Writer monad that logs to a `W` constrained to be a Monoid](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Writer_MonoidW_W_A.htm)
`Core` | `State<S, A>` | [State monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/State_S_A.htm)
`Core` | `Patch<EqA, A>` | Uses patch-theory to efficiently calculate the difference (`Patch.diff(list1, list2)`) between two collections of `A` and build a patch which can be applied (`Patch.apply(patch, list)`) to one to make the other (think git diff).
`Parsec` | `Parser<A>` | [String parser monad and full parser combinators library](https://louthy.github.io/language-ext/LanguageExt.Parsec/LanguageExt.Parsec/index.htm#Parser_T)
`Parsec` | `Parser<I, O>` | [Parser monad that can work with any input stream type](https://louthy.github.io/language-ext/LanguageExt.Parsec/LanguageExt.Parsec/index.htm#Parser_I_O)
`Core` | `NewType<SELF, A, PRED>` | [Haskell `newtype` equivalent](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/NewType_NEWTYPE_A_PRED.htm) i.e: `class Hours : NewType<Hours, double> { public Hours(double value) : base(value) { } }`.  The resulting type is: equatable, comparable, foldable, a functor, monadic, and iterable
`Core` | `NumType<SELF, NUM, A, PRED>` | [Haskell `newtype` equivalent but for numeric types](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/NumType_NUMTYPE_NUM_A_PRED.htm) i.e: `class Hours : NumType<Hours, TDouble, double> { public Hours(double value) : base(value) { } }`.  The resulting type is: equatable, comparable, foldable, a functor, a monoid, a semigroup, monadic, iterable, and can have basic artithmetic operations performed upon it.
`Core` | `FloatType<SELF, FLOATING, A, PRED>` | [Haskell `newtype` equivalent but for real numeric types](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/FloatType_SELF_FLOATING_A_PRED.htm) i.e: `class Hours : FloatType<Hours, TDouble, double> { public Hours(double value) : base(value) { } }`.  The resulting type is: equatable, comparable, foldable, a functor, a monoid, a semigroup, monadic, iterable, and can have complex artithmetic operations performed upon it.
`Core` | `Nullable<T>` extensions | [Extension methods for `Nullable<T>`](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/NullableExtensions_.htm) that make it into a functor, applicative, foldable, iterable and a monad
`Core` | `Task<T>` extensions | [Extension methods for `Task<T>`](https://louthy.github.io/language-ext/LanguageExt.Core/TaskExtensions_.htm) that make it into a functor, applicative, foldable, iterable and a monad
`Core` | `Validation<FAIL,SUCCESS>` | [Validation applicative and monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Validation_FAIL_SUCCESS.htm) for collecting multiple errors before aborting an operation
`Core` | `Validation<MonoidFail, FAIL, SUCCESS>` | [Validation applicative and monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Validation_FAIL_SUCCESS.htm) for collecting multiple errors before aborting an operation, uses the supplied monoid in the first generic argument to collect the failure values.
`Core` | Monad transformers | A higher kinded type (ish)
`Core` | Currying | [Translate the evaluation of a function that takes multiple arguments into a sequence of functions, each with a single argument](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Prelude_.htm#curry&lt;T1,%20T2,%20R&gt;)
`Core` | Partial application | [the process of fixing a number of arguments to a function, producing another function of smaller arity](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Prelude_.htm#par&lt;T1,%20T2,%20R&gt;)
`Core` | Memoization | [An optimization technique used primarily to speed up programs by storing the results of expensive function calls and returning the cached result when the same inputs occur again](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Prelude_.htm#memo&lt;T,%20R&gt;)
`Core` | [Improved lambda type inference](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Prelude_.htm#fun<R>) | `var add = fun( (int x, int y) => x + y)`
`Core` | [`IQueryable<T>` extensions](https://louthy.github.io/language-ext/LanguageExt.Core/QueryExtensions_.htm)  |
`Core` | [`IObservable<T>` extensions](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/ObservableExt_.htm)  |

It started out trying to deal with issues in C#, that after using Haskell and F# started to frustrate me:

* [Poor tuple support](#poor-tuple-support)
* [Null reference problem](#null-reference-problem)
    * [Option](#option)
* [Lack of lambda and expression inference](#lack-of-lambda-and-expression-inference)
* [Void isn't a real type](#void-isnt-a-real-type)
* [Mutable lists and dictionaries](#mutable-lists-and-dictionaries)
   * [Lists](#lists)
   * [List pattern matching](#list-pattern-matching)
   * [Maps](#maps)
* [Difficulty in creating immutable record types](#difficulty-in-creating-immutable-record-types)
   * [Transformation of immutable types](#transformation-of-immutable-types)
      * [`[With]`](#with)
   * [Transformation of nested immutable types with Lenses](#transformation-of-nested-immutable-types-with-lenses)
      * [`[WithLens]`](#withlens)
* [The awful 'out' parameter](#the-awful-out-parameter)
* [The lack of ad-hoc polymorphism](#ad-hoc-polymorphism)
   * [`Num<A>`](#num<A>)
   * [`Eq<A>`](#eq<A>)
   * [`Ord<A>`](#ord<A>)
   * [`Semigroup<A>`](#semigroup<A>)
   * [`Monoid<A>`](#monoid<A>)
   * [`Monad`](#monad)
   * [Transformer types](#transformer-types)

   

## Poor tuple support
I've been crying out for proper tuple support for ages.  When this library was created we were no closer (C# 6).  
The standard way of creating them is ugly `Tuple.Create(foo,bar)` compared to functional languages where the syntax is often 
`(foo,bar)` and to consume them you must work with the standard properties of `Item1`...`ItemN`.  Luckily now in C# 7
we can use: `(foo,bar)`.  But for those that can't:

```C#
    var ab = Tuple("a","b");
```

Now isn't that nice?  

Consuming the tuple is now handled using `Map`, which projects the `Item1`...`ItemN` onto a lambda function (or action):

```C#
    var name = Tuple("Paul","Louth");
    var res = name.Map( (first, last) => $"{first} {last}");
```
Or, you can use a more functional approach:
```C#
    var name = Tuple("Paul","Louth");
    var res = map( name, (first, last) => $"{first} {last}");
```
This allows the tuple properties to have names, and it also allows for fluent handling of functions that return tuples.

If you are using C#7 then you'll know that the new `Tuple` type is `ValueTuple`.  Just like with `Tuple`, language-ext 
adds many extensions to the standard BCL `ValueTuple`.  

For example:

```C#
    var abc = ('a', 'b').Add('c');                                           // ('a', 'b', 'c')
    var abcd = ('a', 'b').Add('c').Add('d');                                 // ('a', 'b', 'c', 'd')
    var abcd5 = ('a', 'b').Add('c').Add('d').Add(5);                         // ('a', 'b', 'c', 'd', 5)

    var sum = (1, 2, 3).Sum<TInt, int>();                                    // 6
    var product = (2, 4, 8).Product<TInt, int>();                            // 64
    var flag = ("one", "two", "three").Contains<TString, string>("one");     // true
    var str = ("Hello", " ", "World").Concat<TString, string>();             // "Hello World"
    var list = (List(1, 2, 3), List(4, 5, 6)).Concat<TLst<int>, Lst<int>>(); // [1,2,3,4,5,6]
```

## Null reference problem
`null` must be the biggest mistake in the whole of computer language history.  I realise the original designers 
of C# had to make pragmatic decisions, it's a shame this one slipped through though.  So, what to do about the 
'null problem'?

`null` is often used to indicate 'no value'.  i.e. the method called can't produce a value of the type it said 
it was going to produce, and therefore it gives you 'no value'.  The thing is that when 'no value' is passed to 
the consuming code, it gets assigned to a variable of type T, the same type that the function said it was going 
to return, except this variable now has a timebomb in it.  You must continually check if the value is `null`, if 
it's passed around it must be checked too.  

As we all know it's only a matter of time before a null reference bug crops up because the variable wasn't 
checked.  It puts C# in the realm of the dynamic languages, where you can't trust the value you're being given.

Functional languages use what's known as an 'option type'.  In F# it's called `Option` in Haskell it's called 
`Maybe`.  In the next section we'll see how it's used.

## Option
`Option<T>` works in a very similar way to `Nullable<T>` except it works with all types rather than just value 
types.  It's a `struct` and therefore can't be `null`.  An instance can be created by either calling `Some(value)`, 
which represents a positive 'I have a value' response;  Or `None`, which is the equivalent of returning `null`.

So why is it any better than returning `T` and using `null`?  It seems we can have a non-value response again 
right?  Yes, that's true, however you're forced to acknowledge that fact, and write code to handle both possible 
outcomes because you can't get to the underlying value without acknowledging the possibility of the two states 
that the value could be in.  This bulletproofs your code.  You're also explicitly telling any other programmers 
that: "This method might not return a value, make sure you deal with that".  This explicit declaration is very 
powerful.

This is how you create an `Option<int>`:

```C#
    var optional = Some(123);
```
To access the value you must check that it's valid first:

```C#
    int x = optional.Match( 
                Some: v  => v * 2,
                None: () => 0 
                );
```
An alternative (functional) way of matching is this:

```C#
    int x = match( optional, 
                   Some: v  => v * 2,
                   None: () => 0 );
```
Yet another alternative (fluent) matching method is this:
```C#
    int x = optional
               .Some( v  => v * 2 )
               .None( () => 0 );
```
So choose your preferred method and stick with it.  It's probably best not to mix styles.

There are also some helper functions to work with default `None` values,  You won't see a `.Value` or a 
`GetValueOrDefault()` anywhere in this library.  It is because `.Value` puts us right back to where we started, 
you may as well not use `Option<T>` in that case.  `GetValueOrDefault()` is as bad, because it can return `null` 
for reference types, and depending on how well defined the `struct` type is you're working with: a poorly 
defined value type.

However, clearly there will be times when you don't need to do anything with the `Some` case, because, well 
that's what you asked for.  Also, sometimes you just want some code to execute in the `Some` case and not the 
`None` case...

```C#
    // Returns the Some case 'as is' and 10 in the None case
    int x = optional.IfNone(10);        

    // As above, but invokes a Func<T> to return a valid value for x
    int x = optional.IfNone(() => GetAlternative());        
    
    // Invokes an Action<T> if in the Some state.
    optional.IfSome(x => Console.WriteLine(x));
```
Of course there are functional versions of the fluent version above:
```C#
    int x = ifNone(optional, 10);
    int x = ifNone(optional, () => GetAlternative());
    ifSome(optional, x => Console.WriteLine(x));
```
To smooth out the process of returning `Option<T>` types from methods there are some implicit conversion 
operators and constructors:

```C#
    // Implicitly converts the integer to a Some of int
    Option<int> GetValue()
    {
        return 1000;
    }

    // Implicitly converts to a None of int
    Option<int> GetValue()
    {
        return None;
    }
    
    // Will handle either a None or a Some returned
    Option<int> GetValue(bool select) =>
        select
            ? Some(1000)
            : None;
            
    // Explicitly converts a null value to None and a non-null value to Some(value)
    Option<string> GetValue()
    {
        string value = GetValueFromNonTrustedApi();
        return Optional(value);
    }
            
    // Implicitly converts a null value to None and a non-null value to Some(value)
    Option<string> GetValue()
    {
        string value = GetValueFromNonTrustedApi();
        return value;
    }
```

It's actually nearly impossible to get a `null` out of a function, even if the `T` in `Option<T>` is a 
reference type and you write `Some(null)`.  Firstly it won't compile, but you might think you can do this:

```C#
    private Option<string> GetStringNone()
    {
        string nullStr = null;
        return Some(nullStr);
    }
```
That will compile, but at runtime will throw a `ValueIsNullException`.  If you do either of these (below) 
you'll get a `None`.  

```C#
    private Option<string> GetStringNone()
    {
        string nullStr = null;
        return nullStr;
    }

    private Option<string> GetStringNone()
    {
        string nullStr = null;
        return Optional(nullStr);
    }

```
These are the coercion rules:

Converts from |  Converts to
--------------|-------------
`x` | `Some(x)`
`null` | `None`
`None` | `None`
`Some(x)` | `Some(x)`
`Some(null)` | `ValueIsNullException`
`Some(None)` | `Some(None)`
`Some(Some(x))` | `Some(Some(x))`
`Some(Nullable null)` | `ValueIsNullException`
`Some(Nullable x)` | `Some(x)`
`Optional(x)` | `Some(x)`
`Optional(null)` | `None`
`Optional(Nullable null)` | `None`
`Optional(Nullable x)` | `Some(x)`

As well as the protection of the internal value of `Option<T>`, there's protection for the return value 
of the `Some` and `None` handler functions.  You can't return `null` from those either, an exception will 
be thrown.

```C#
    // This will throw a ResultIsNullException exception
    string res = GetValue(true)
                     .Some(x => (string)null)
                     .None((string)null);
```

So `null` goes away if you use `Option<T>`.

However, there are times when you want your `Some` and `None` handlers to return `null`.  This is mostly 
when you need to use something in the BCL or from a third-party library, so momentarily you need to step 
out of your warm and cosy protected optional bubble, but you've got an `Option<T>` that will throw an 
exception if you try.  

So you can use `matchUnsafe` and `ifNoneUnsafe`:

```C#
    string x = matchUnsafe( optional,
                            Some: v => v,
                            None: () => null );

    string x = ifNoneUnsafe( optional, (string)null );
    string x = ifNoneUnsafe( optional, () => GetNull() );
```
And fluent versions:
```C#
    string x = optional.MatchUnsafe(
                   Some: v => v,
                   None: () => null 
                   );
    string x = optional.IfNoneUnsafe((string)null);
    string x = optional.IfNoneUnsafe(() => GetNull());
```
That is consistent throughout the library.  Anything that could return `null` has the `Unsafe` suffix.  That 
means that in those unavoidable circumstances where you need a `null`, it gives you and any other programmers 
working with your code the clearest possible sign that they should treat the result with care.

### Option monad - gasp!  Not the M word!

I know, it's that damn monad word again.  They're actually not scary at all, and damn useful.  But if you 
couldn't care less (or _could_ care less, for my American friends), it won't stop you taking advantage of the 
`Option<T>` type.  However, `Option<T>` type also implements `Select` and `SelectMany` and is therefore monadic.  
That means it can be used in LINQ expressions, but it means much more also.  

```C#
    Option<int> two = Some(2);
    Option<int> four = Some(4);
    Option<int> six = Some(6);
    Option<int> none = None;

    // This expression succeeds because all items to the right of 'in' are Some of int
    // and therefore it lands in the Some lambda.
    int r = match( from x in two
                   from y in four
                   from z in six
                   select x + y + z,
                   Some: v => v * 2,
                   None: () => 0 );     // r == 24

    // This expression bails out once it gets to the None, and therefore doesn't calculate x+y+z
    // and lands in the None lambda
    int r = match( from x in two
                   from y in four
                   from _ in none
                   from z in six
                   select x + y + z,
                   Some: v => v * 2,
                   None: () => 0 );     // r == 0
```
This can be great for avoiding the use of `if then else`, because the computation continues as long as the 
result is `Some` and bails otherwise.  It is also great for building blocks of computation that you can compose 
and reuse.  Yes, actually compose and reuse, not like OO where the promise of composability and modularity are 
essentially lies.  

To take this much further, all of the monads in this library implement a standard 'functional set' of functions:
```C#
    Sum                 // For Option<int> it's the wrapped value.
    Count               // For Option<T> is always 1 for Some and 0 for None.  
    Bind                // Part of the definition of anything monadic - SelectMany in LINQ
    Exists              // Any in LINQ - true if any element fits a predicate
    Filter              // Where in LINQ
    Fold                // Aggregate in LINQ
    ForAll              // All in LINQ - true if all element(s) fits a predicate
    Iter                // Passes the wrapped value(s) to an Action delegate
    Map                 // Part of the definition of any 'functor'.  Select in LINQ
    Lift / LiftUnsafe   // Different meaning to Haskell, this returns the wrapped value.  Dangerous, should be used sparingly.
    Select
    SeletMany
    Where
```
This makes them into what would be known in Haskell as a Type Class (although more of a catch-all type-class than a set of well-defined type-classes).  


* [Option reference](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Option_A.htm)
* [Option extensions reference](https://louthy.github.io/language-ext/LanguageExt.Core/OptionExtensions_.htm)

__Monad transformers__

Monad transformers allow for nested monadic types.  Imagine functionality for working with `Seq<Option<A>>` or a `Option<Task<A>>`, etc.

Now the problem with C# is it can't do higher order polymorphism  (imagine saying `Monad<M<T>>` where the `M` is polymorphic like the `T`).

There's a kind of cheat way to do it in C# through extension methods.  It still doesn't get you a single type called `Monad<M<T>>` 
(which is discussed later in the section on Ad-hoc Polymorphism), so it has limitations in that you can't write generic functions over higher-kinds.  
However it makes some of the problems of dealing with nested monadic types easier.

For example, below is a list of optional integers: `Lst<Option<int>>` (see lists later).  We want to double all of the `Some` values, leave the 
`None` alone and keep everything in the list:

```C#
    using LanguageExt;
    using static LanguageExt.Prelude;
    using LanguageExt.ClassInstances;    // Required for TInt on Sum (see ad-hoc polymorphism later)

    var list = List(Some(1), None, Some(2), None, Some(3));

    var presum = list.SumT<TInt, int>();                                // 6

    list = list.MapT(x => x * 2);

    var postsum = list.SumT<TInt, int>();
```
Notice the use of `MapT` instead of `Map` (and `SumT` instead of `Sum`).  If we used `Map` (equivalent to `Select` in `LINQ`), it would look like this:
```C#
    var list  = List(Some(1), None, Some(2), None, Some(3));
    
    var presum = list.Map(x => x.Sum()).Sum();
    
    list = list.Map( x => x.Map( v => v * 2 ) );
    
    var postsum = list.Map(x => x.Sum()).Sum();
```
As you can see the intention is much clearer in the first example.  And that's the point with functional programming most of the time.  It's about 
declaring intent rather than the mechanics of delivery.

To make this work we need extension methods for `List<Option<T>>` that define `MapT` and `SumT` [for the one  example above].  And we need one for 
every pair of monads in this library (for one level of nesting `A<B<T>>`), and for every function from the 'standard functional set' listed above.  
So that's 13 monads * 13 monads * 14 functions.  That's a lot of extension methods.  So there's T4 template that generates 'monad transformers' 
that allows for nested monads.

This is super powerful, and means that most of the time you can leave your `Option<T>` or any of the monads in this library wrapped.  You rarely 
need to extract the value.  Mostly you only need to extract the value to pass to the BCL or Third-party libraries.  Even then you could keep 
them wrapped and use `Iter` or `IterT`.


## if( arg == null ) throw new ArgumentNullException("arg")
Another horrible side-effect of `null` is having to bullet-proof every function that takes reference arguments.  This is truly tedious.  Instead use this:
```C#
    public void Foo( Some<string> arg )
    {
        string value = arg;
        ...
    }
```
By wrapping `string` as `Some<string>` we get free runtime `null` checking. Essentially it's impossible (well, almost) for `null` to propagate through.  As you can see (above) the `arg` variable casts automatically to `string value`.  It's also possible to get at the inner-value like so:
```C#
    public void Foo( Some<string> arg )
    {
        string value = arg.Value;
        ...
    }
```
If you're wondering how it works, well `Some<T>` is a `struct`, and has implicit conversion operators that convert a type of `T` to a type of `Some<T>`.  The constructor of `Some<T>` ensures that the value of `T` has a non-null value.

There is also an implicit cast operator from `Some<T>` to `Option<T>`.  The `Some<T>` will automatically put the `Option<T>` into a `Some` state.  It's not possible to go the other way and cast from `Option<T>` to `Some<T>`, because the `Option<T>` could be in a `None` state which would cause the `Some<T>` to throw `ValueIsNullException`.  We want to avoid exceptions being thrown, so you must explicitly `match` to extract the `Some` value.

There is one weakness to this approach, and that is that if you add a member property or field to a class which is a  `struct`, and if you don't initialise it, then C# is happy to go along with that.  This is the reason why you shouldn't normally include reference members inside structs (or if you do, have a strategy for dealing with it).

`Some<T>` unfortunately falls victim to this, it wraps a reference of type T.  Therefore it can't realistically create a useful default.  C# also doesn't call the default constructor for a `struct` in these circumstances.  So there's no way to catch the problem early.  For example:

```C#
    class SomeClass
    {
        public Some<string> SomeValue = "Hello";
        public Some<string> SomeOtherValue;
    }
    
    ...
    
    public void Greet(Some<string> arg)
    {
        Console.WriteLine(arg);
    }
    
    ...
    
    public void App()
    {
        var obj = new SomeClass();
        Greet(obj.SomeValue);
        Greet(obj.SomeOtherValue);
    }
```
In the example above `Greet(obj.SomeOtherValue);` will work until `arg` is used inside of the `Greet` function.  So that puts us back into the `null` realm.  There's nothing (that I'm aware of) that can be done about this.  `Some<T>` will throw a useful `SomeNotInitialisedException`, which should make life a little easier.
```
    "Unitialised Some<...>"
```
So what's the best plan of attack to mitigate this?

* Don't use `Some<T>` for class members.  That means the class logic might have to deal with `null` however.
* Or, always initialise `Some<T>` class members.  Mistakes do happen though.

There's no silver bullet here unfortunately.

_NOTE: Since writing this library I have come to the opinion that `Some<T>` isn't that useful.  It's much better to protect 'everything else' using `Option<T>` and immutable data structures.  It doesn't fix the argument null checks unfortunately.  But perhaps using a contracts library would be better._

* [Some reference](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Some_A.htm)

## Lack of lambda and expression inference 

One really annoying thing about the `var` type inference in C# is that it can't handle inline lambdas.  For example this won't compile, even though it's obvious it's a `Func<int,int,int>`.
```C#
    var add = (int x, int y) => x + y;
```
There are some good reasons for this, so best not to bitch too much.  Instead use the `fun` function from this library:
```C#
    var add = fun( (int x, int y) => x + y );
```
This will work for `Func<..>` and `Action<..>` types of up to seven generic arguments.  `Action<..>` will be converted to `Func<..,Unit>`.  To maintain an `Action` use the `act` function instead:
```C#
    var log = act( (int x) => Console.WriteLine(x) );
```
If you pass a `Func<..>` to `act` then its return value will be dropped.  So `Func<R>` becomes `Action`, and `Func<T,R>` will become `Action<T>`.

To do the same for `Expression<..>`, use the `expr` function:

```C#
    var add = expr( (int x, int y) => x + y );
```

Note, if you're creating a `Func` or `Action` that take parameters, you must provide the type:

```C#
    // Won't compile
    var add = fun( (x, y) => x + y );

    // Will compile
    var add = fun( (int x, int y) => x + y );
```

* [`fun` reference](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Prelude_.htm#fun)
* [`act` reference](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Prelude_.htm#act)
* [`expr` reference](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Prelude_.htm#expr)

## Void isn't a real type

Functional languages have a concept of a type that has one possible value, itself, called `Unit`.  As an example `bool` has two possible values: `true` and `false`.  `Unit` has one possible value, usually represented in functional languages as `()`.  You can imagine that methods that take no arguments, do in fact take one argument of `()`.  Anyway, we can't use the `()` representation in C#, so `LanguageExt` now provides `unit`.

```C#
    public Unit Empty()
    {
        return unit;
    }
```

`Unit` is the type and `unit` is the value.  It is used throughout the `LanguageExt` library instead of `void`.  The primary reason is that if you want to program functionally then all functions should return a value and `void` is a type with zero possible values - and that's the type-theory reason why `void` is a pain in the arse in C#.  This can help a lot with LINQ expressions.

* [Unit reference](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Unit_.htm)

## Mutable lists and dictionaries

With the new 'get only' property syntax with C# 6 it's now much easier to create immutable types.  Which everyone should do.  However there's still going to be a bias towards mutable collections.  There's a great library on NuGet called Immutable Collections.  Which sits in the `System.Collections.Immutable` namespace.  It brings performant immutable lists, dictionaries, etc. to C#.  However, this:

```C#
    var list = ImmutableList.Create<string>();
```
Compared to this:
```C#
    var list = new List<string>();
```
Is annoying.  There's clearly going to be a bias toward the shorter, easier to type, better known method of creating lists.  In functional languages collections are often baked in (because they're so fundamental), with lightweight and simple syntax for generating and modifying them.  So let's have some of that...

### Lists

There's support for `Cons`, which is the functional way of constructing lists:
```C#
    var test = Cons(1, Cons(2, Cons(3, Cons(4, Cons(5, empty<int>())))));

    var array = test.ToArray();

    Assert.IsTrue(array[0] == 1);
    Assert.IsTrue(array[1] == 2);
    Assert.IsTrue(array[2] == 3);
    Assert.IsTrue(array[3] == 4);
    Assert.IsTrue(array[4] == 5);
```

_Note, this isn't the strict definition of `Cons`, but it's a pragmatic implementation that returns an `IEnumerable<T>`, is lazy, and behaves the same.  Functional purists, please don't get too worked up!  I am yet to think of a way of implementing a proper type-safe `cons` (that can also represent trees, etc.) in C#._

Functional languages usually have a shortcut list constructor syntax that makes the `Cons` approach easier.  It usually looks something like this:

```F#
    let list = [1;2;3;4;5]
```

In C# it looks like this:

```C#
    var array = new int[] { 1, 2, 3, 4, 5 };
    var list = new List<int> { 1, 2, 3, 4, 5 };
```
Or worse:
```C#
    var list = new List<int>();
    list.Add(1);
    list.Add(2);
    list.Add(3);
    list.Add(4);
    list.Add(5);
```
So we provide the `List` function that takes any number of parameters and turns them into a list:

```C#
    // Creates a list of five items
     var test = List(1, 2, 3, 4, 5);
```

This is much closer to the 'functional way'.  It also returns a `Lst<T>` which is an immutable list implementation.  So it's now easier to use immutable-lists than the mutable ones.  And significantly less typing.

Also `Range`:

```C#
    // Creates a sequence of 1000 integers lazily (starting at 500).
    var list = Range(500,1000);
    
    // Produces: [0, 10, 20, 30, 40]
    var list = Range(0,50,10);
    
    // Produces: ['a,'b','c','d','e']
    var chars = Range('a','e');
```

Some of the standard set of list functions are available (in `LanguageExt.List`):

```C#
    using static LanguageExt.List;
    ...

    // Generates 10,20,30,40,50
    var input = List(1, 2, 3, 4, 5);
    var output1 = map(input, x => x * 10);

    // Generates 30,40,50
    var output2 = filter(output1, x => x > 20);

    // Generates 120
    var output3 = fold(output2, 0, (x, s) => s + x);

    Assert.IsTrue(output3 == 120);
```

The above can be written in a fluent style also:

```C#
    var res = List(1, 2, 3, 4, 5)
                .Map(x => x * 10)
                .Filter(x => x > 20)
                .Fold(0, (x, s) => s + x);

    Assert.IsTrue(res == 120);
```

### List pattern matching

Here we implement the standard functional pattern for matching on list elements.  In our version you must provide at least 2 handlers:

* One for an empty list
* One for a non-empty list

However, you can provide up to seven handlers, one for an empty list and six for deconstructing the first six items at the head of the list.

```C#
    public int Sum(IEnumerable<int> list) =>
        match( list,
               ()      => 0,
               (x, xs) => x + Sum(xs) );

    public int Product(IEnumerable<int> list) =>
        match( list,
               ()      => 0,
               x       => x,
               (x, xs) => x * Product(xs) );

    public void RecursiveMatchSumTest()
    {
        var list0 = List<int>();
        var list1 = List(10);
        var list5 = List(10,20,30,40,50);
        
        Assert.IsTrue(Sum(list0) == 0);
        Assert.IsTrue(Sum(list1) == 10);
        Assert.IsTrue(Sum(list5) == 150);
    }

    public void RecursiveMatchProductTest()
    {
        var list0 = List<int>();
        var list1 = List(10);
        var list5 = List(10, 20, 30, 40, 50);

        Assert.IsTrue(Product(list0) == 0);
        Assert.IsTrue(Product(list1) == 10);
        Assert.IsTrue(Product(list5) == 12000000);
    }
```
Those patterns should be very familiar to anyone who's ventured into the functional world.  For those that haven't, the `(x,xs)` convention might seem odd.  `x` is the item at the head of the list - `list.First()` in LINQ world.  `xs` (many X-es) is the tail of the list - `list.Skip(1)` in LINQ.  This recursive pattern of working on the head of the list until the list runs out is pretty much how loops are done in the functional world.  

Be wary of recursive processing however.  C# will happily blow up the stack after a few thousand iterations.  

Functional programming doesn't really _do_ design patterns, but if anything is a design pattern it's the use of `fold`.  If you put a bit of thought into it, you will realise that recursive processes all tend to follow a very similar pattern.  

The two recursive examples above for calculating the sum and product of a sequence of numbers can be written:

```C#
    // Sum
    var total = fold(list, 0, (s,x) => s + x);
    
    // Product
    var total = reduce(list, (s,x) => s * x);
```
`reduce` is `fold` but instead of providing an initial state value, it uses the first item in the sequence.  Therefore you don't get an initial multiply by zero (unless the first item is zero!).  Internally `fold`, `foldBack` and `reduce` use an iterative loop rather than a recursive one; so no stack blowing problems!

* [List module reference](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/List_.htm)
* [`Lst<T>` immutable list type reference](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Lst_A.htm)

### Maps

We also support dictionaries.  Again the word `Dictionary` is such a pain to type, especially when there's a perfectly valid alternative used in the functional world: `map`.

To create an immutable map, you no longer have to type:

```C#
    var dict = ImmutableDictionary.Create<string,int>();
```
Instead you can use:
```C#
    var dict = Map<string,int>();
```
_`Map<K,V>` is an implementation of an AVL Tree (self balancing binary tree).  This allows us to extend the standard `IDictionary` set of functions to include things like `findRange`._

Also you can pass in a list of tuples or key-value pairs:

```C#
    var people = Map((1, "Rod"),
                     (2, "Jane"),
                     (3, "Freddy"));
```
To read an item call:
```C#
    Option<string> result = find(people, 1);
```
This allows for branching based on whether the item is in the map or not:

```C#
    // Find the item, do some processing on it and return.
    var res = match( find(people, 100),
                     Some: v  => "Hello " + v,
                     None: () => "failed" );
                   
    // Find the item and return it.  If it's not there, return "failed"
    var res = find(people, 100).IfNone("failed");                   
    
    // Find the item and return it.  If it's not there, return "failed"
    var res = ifNone( find(people, 100), "failed" );
```
Because checking for the existence of something in a dictionary (`find`), and then matching on its result is very common, there is a more convenient `match` override:
```C#
    // Find the item, do some processing on it and return.
    var res = match( people, 1,
                     Some: v  => "Hello " + v,
                     None: () => "failed" );
```                   

To set an item call:
```C#
    var newThings = setItem(people, 1, "Zippy");
```

Obviously because it's an immutable structure, calling `add`, `tryAdd`, `addOrUpdate`, `addRange`, `tryAddRange`, `addOrUpdateRange`, `remove`, `setItem`, `trySetItem`, `setItems` or `trySetItems`... will generate a new `Map<K,V>`.  It's quite cunning though, and it only replaces the items that need to be replaced and returns a new map with the new items and shared old items.  This massively reduces the memory allocation burden

By holding onto a reference to the `Map` before and after calling `add` you essentially have a perfect timeline history of the changes.  But be wary that if what you're holding in the `Map` is *mutable* and you change your mutable items, then the old `Map` and the new `Map` will change.

So only store immutable items in a `Map`, or leave them alone if they're mutable.

* [Map module reference](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Map_.htm)
* [Map extensions reference](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/MapExtensions_.htm)
* [`Map<K, V>` immutable type reference](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Map_K_V.htm)

### Map transformers

There are additional transformer functions for dealing with 'wrapped' maps (i.e. `Map<int, Map<int, string>>`).  We only cover a limited set of the full set of `Map` functions at the moment.  You can wrap `Map` up to 4 levels deep and still call things like `Fold` and `Filter`.  There's  interesting variants of `Filter` and `Map` called `FilterRemoveT` and `MapRemoveT`, where if a filter or map operation leaves any keys at any level with an empty `Map` then it will auto-remove them.  

```C#
    Map<int,Map<int,Map<int, Map<int, string>>>> wrapped = Map.create<int,Map<int,Map<int,Map<int,string>>();
    
    wrapped = wrapped.AddOrUpdate(1,2,3,4,"Paul");
    wrapped = wrapped.SetItemT(1,2,3,4,"Louth");
    var name = wrapped.Find(1,2,3,4);               // "Louth"
```
The `Map` transformer functions:

_Note, there are only fluent versions of the transformer functions._

* `Find`
* `AddOrUpdate`
* `Remove`
* `MapRemoveT` - maps each level,  checks if the map is empty, in which case it removes it
* `MapT`
* `FilterT`
* `FilterRemoveT`` - filters each level, checks if the map is empty, in which case it removes it
* `Exists`
* `ForAll`
* `SetItemT`
* `TrySetItemT`
* `FoldT`
* more coming...

## Difficulty in creating immutable record types 

It's no secret that implementing immutable record types, with structural equality, structural ordering, and efficient hashing solutions is a real manual head-ache of implementing `Equals`, `GetHashCode`, deriving from `IEquatable<A>`, `IComparer<A>`, and implementing the operators: `==`, `!=`, `<`, `<=`, `>`, `>=`.  It is a constant maintenance headache of making sure they're kept up to date when new fields are added to the type - with no compilation errors if you forget to do it.

## `Record<A>`

This can now be achieved simply by deriving your type from `Record<A>` where `A` is the type you want to have structural equality and ordering.  i.e.
```c#
    public class TestClass : Record<TestClass>
    {
        public readonly int X;
        public readonly string Y;
        public readonly Guid Z;

        public TestClass(int x, string y, Guid z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
```
This gives you `Equals`, `IEquatable.Equals`, `IComparer.CompareTo`, `GetHashCode`, `operator==`, `operator!=`, `operator >`, `operator >=`, `operator <`, and `operator <=` implemented by default.  It also gives you a default `ToString()` implementation and `ISerializable.GetObjectData()` with a deserialisation constructor.

Note that only _fields_ or _field backed properties_ are used in the structural comparisons and hash-code building.  There are also `Attribute`s for opting fields out of the equality testing, ordering comparisons, hash-code generation, stringification (`ToString`),  and serialisation:

* `Equals()` - `NonEq`
* `CompareTo()` - `NonOrd`
* `GetHashCode()` - `NonHash`
* `ToString()` - `NonShow`
* Serialization - `NonSerializable`)

For example, here's a record type that opts out of various default behaviours:
```c#
    public class TestClass2 : Record<TestClass2>
    {
        [NonEq]
        public readonly int X;

        [NonHash]
        public readonly string Y;

        [NonShow]
        public readonly Guid Z;

        public TestClass2(int x, string y, Guid z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
```
If you want your type to serialise with Json.NET or other serialisers then you will need to add an extra serialisation constructor that calls the default base implementation:
```c#
    public class TestClass : Record<TestClass>
    {
        public readonly int X;
        public readonly string Y;
        public readonly Guid Z;

        public TestClass(int x, string y, Guid z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        
        TestClass(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
```
This will do full structural equality as the following examples demonstrate:
```c#
public class Cons<A> : Record<Cons<A>>
{
    public readonly A Head;
    public readonly Cons<A> Tail;

    public Cons(A head, Cons<A> tail)
    {
        Head = head;
        Tail = tail;
    }
}

public void ConsTests()
{
    var listA = new Cons<int>(1, new Cons<int>(2, new Cons<int>(3, new Cons<int>(4, null))));
    var listB = new Cons<int>(1, new Cons<int>(2, new Cons<int>(3, new Cons<int>(4, null))));
    var listC = new Cons<int>(1, new Cons<int>(2, new Cons<int>(3, null)));

    Assert.True(listA == listB);
    Assert.True(listB != listC);
    Assert.True(listA != listC);
}

public class Tree<A> : Record<Tree<A>>
{
    public readonly A Value;
    public readonly Tree<A> Left;
    public readonly Tree<A> Right;

    public Tree(A value, Tree<A> left, Tree<A> right)
    {
        Value = value;
        Left = left;
        Right = right;
    }
}

public void TreeTests()
{
    var treeA = new Tree<int>(5, new Tree<int>(3, null, null), new Tree<int>(7, null, new Tree<int>(9, null, null)));
    var treeB = new Tree<int>(5, new Tree<int>(3, null, null), new Tree<int>(7, null, new Tree<int>(9, null, null)));
    var treeC = new Tree<int>(5, new Tree<int>(3, null, null), new Tree<int>(7, null, null));

    Assert.True(treeA == treeB);
    Assert.True(treeB != treeC);
    Assert.True(treeA != treeC);
}
```
There are some [unit tests](https://github.com/louthy/language-ext/blob/master/LanguageExt.Tests/RecordTypesTest.cs) to see this in action.

> Inheritance is not supported in `Record` derived types.  So, if you derive a type from a type that derives from `Record` then you won't magically inherit any equality, ordering, hash-code, etc. behaviours.  This feature is explicitly here to implement record-like functionality, which - in other functional languages - do not support inheritance.  Equality of origin is explicitly checked for also.

## `RecordType<A>`

You can also use the 'toolkit' that `Record<A>` uses to build this functionality in your own bespoke types (perhaps if you want to use this for `struct` comparisons or if you can't derive directly from `Record<A>`, or maybe you just want some of the functionality for ad-hoc behaviour):  

The toolkit is composed of four functions:

```c#
    RecordType<A>.Hash(record);
```
This will provide the hash-code for the record of type `A` provided.  It can be used for your default `GetHashCode()` implementation.
```c#
    RecordType<A>.Equality(record, obj);
```
This provides structural equality with the record of type `A` and the record of type `object`.  The types must match for the equality to pass.  It can be used for your default `Equals(object)` implementation.
```c#
    RecordType<A>.EqualityTyped(record1, record2);
```
This provides structural equality with the record of type `A` and another record of type `A`.  It can be used for your default `Equals(a, b)` method for the `IEquatable<A>` implementation.
```c#
    RecordType<A>.Compare(this, other);
```
This provides a structural ordering comparison with the record of type `A` and another record the record of type `A`.  It can be used for your default `CompareTo(a, b)` method for the `IComparable<A>` implementation.

Below is the toolkit in use,  it's used to build a `struct` type that has structural equality, ordering, and hash-code implementation.
```c#
    public class TestStruct : IEquatable<TestStruct>, IComparable<TestStruct>
    {
        public readonly int X;
        public readonly string Y;
        public readonly Guid Z;

        public TestStruct(int x, string y, Guid z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override int GetHashCode() =>
            RecordType<TestStruct>.Hash(this);

        public override bool Equals(object obj) =>
            RecordType<TestStruct>.Equality(this, obj);

        public int CompareTo(TestStruct other) =>
            RecordType<TestStruct>.Compare(this, other);

        public bool Equals(TestStruct other) =>
            RecordType<TestStruct>.EqualityTyped(this, other);
    }
```
## Transformation of immutable types

If you're writing functional code you should treat your types as values.  Which means they should be immutable.  One common way to do this is to use `readonly` fields and provide a `With` function for mutation. i.e.

```c#
public class A
{
    public readonly X X;
    public readonly Y Y;

    public A(X x, Y y)
    {
        X = x;
        Y = y;
    }

    public A With(X X = null, Y Y = null) =>
        new A(
            X ?? this.X,
            Y ?? this.Y
        );
}
```
Then transformation can be achieved by using the named arguments feature of C# thus:

```c#
val = val.With(X: x);

val = val.With(Y: y);

val = val.With(X: x, Y: y);
```
### `[With]`
It can be quite tedious to write the `With` function however.  And so, if you include the `LanguageExt.CodeGen` nu-get package in your solution you gain the ability to use the `[With]` attribtue on a type.  This will build the `With` method for you.

> NOTE: The `LanguageExt.CodeGen` package and its dependencies will not be included in your final build - it is purely there to generate the code.

You must however:
* Make the `class` `partial`
* Have a constructor that takes the fields in the order they are in the type
* The names of the arguments should be the same as the field, but with the first character lower-case

i.e.

```c#
[With]
public partial class A
{
    public readonly X X;
    public readonly Y Y;

    public A(X x, Y y)
    {
        X = x;
        Y = y;
    }
}
```

## Transformation of nested immutable types with Lenses

One of the problems with immutable types is trying to transform something nested deep in several data structures.  This often requires a lot of nested `With` methods, which are not very pretty or easy to use.  

Enter the `Lens<A, B>` type.

Lenses encapsulate the getter and setter of a field in an immutable data structure and are composable:

```c#
[With]
public partial class Person
{
    public readonly string Name;
    public readonly string Surname;

    public Person(string name, string surname)
    {
        Name = name;
        Surname = surname;
    }

    public static Lens<Person, string> name =>
        Lens<Person, string>.New(
            Get: p => p.Name,
            Set: x => p => p.With(Name: x));

    public static Lens<Person, string> surname =>
        Lens<Person, string>.New(
            Get: p => p.Surname,
            Set: x => p => p.With(Surname: x));
}
```
This allows direct transformation of the value:
```c#
var person = new Person("Joe", "Bloggs");

var name = Person.name.Get(person);
var person2 = Person.name.Set(name + "l", person);  // Joel Bloggs
```
This can also be achieved using the `Update` function:
```c#
var person = new Person("Joe", "Bloggs");

var person2 = Person.name.Update(name => name + "l", person);  // Joel Bloggs
```
The power of lenses really becomes apparent when using nested immutable types, because lenses can be composed.  So, let's first create a `Role` type which will be used with the `Person` type to represent an employee's job title and salary:
```c#
[With]
public partial class Role
{
    public readonly string Title;
    public readonly int Salary;

    public Role(string title, int salary)
    {
        Title = title;
        Salary = salary;
    }

    public static Lens<Role, string> title =>
        Lens<Role, string>.New(
            Get: p => p.Title,
            Set: x => p => p.With(Title: x));

    public static Lens<Role, int> salary =>
        Lens<Role, int>.New(
            Get: p => p.Salary,
            Set: x => p => p.With(Salary: x));
}

[With]
public partial class Person
{
    public readonly string Name;
    public readonly string Surname;
    public readonly Role Role;

    public Person(string name, string surname, Role role)
    {
        Name = name;
        Surname = surname;
        Role = role;
    }

    public static Lens<Person, string> name =>
        Lens<Person, string>.New(
            Get: p => p.Name,
            Set: x => p => p.With(Name: x));

    public static Lens<Person, string> surname =>
        Lens<Person, string>.New(
            Get: p => p.Surname,
            Set: x => p => p.With(Surname: x));

    public static Lens<Person, Role> role =>
        Lens<Person, Role>.New(
            Get: p => p.Role,
            Set: x => p => p.With(Role: x));
}

```
We can now compose the lenses within the types to access the nested fields:
```c#
var cto = new Person("Joe", "Bloggs", new Role("CTO", 150000));

var personSalary = lens(Person.role, Role.salary);

var cto2 = personSalary.Set(170000, cto);
```
### `[WithLens]`

Typing the lens fields out every time is even more tedious than writing the `With` function, and so there is code generation for that too: using the `[WithLens]` attribute.  Next, we'll use some of the built-in lenses in the `Map` type to access and mutate a `Appt` type within a map:
```c#
[WithLens]
public partial class Person : Record<Person>
{
    public readonly string Name;
    public readonly string Surname;
    public readonly Map<int, Appt> Appts;

    public Person(string name, string surname, Map<int, Appt> appts)
    {
        Name = name;
        Surname = surname;
        Appts = appts;
    }
}

[WithLens]
public partial class Appt : Record<Appt>
{
    public readonly int Id;
    public readonly DateTime StartDate;
    public readonly ApptState State;

    public Appt(int id, DateTime startDate, ApptState state)
    {
        Id = id;
        StartDate = startDate;
        State = state;
    }
}

public enum ApptState
{
    NotArrived,
    Arrived,
    DNA,
    Cancelled
}
```
So, here we have a `Person` with a map of `Appt` types.  And we want to update an appointment state to be `Arrived`:
```c#
// Generate a Person with three Appts in a Map
var person = new Person("Paul", "Louth", Map(
    (1, new Appt(1, DateTime.Parse("1/1/2010"), ApptState.NotArrived)),
    (2, new Appt(2, DateTime.Parse("2/1/2010"), ApptState.NotArrived)),
    (3, new Appt(3, DateTime.Parse("3/1/2010"), ApptState.NotArrived))));

// Local function for composing a new lens from 3 other lenses
Lens<Person, ApptState> setState(int id) => 
    lens(Person.appts, Map<int, Appt>.item(id), Appt.state);

// Transform
var person2 = setState(2).Set(ApptState.Arrived, person);
```
Notice the local-function which takes an ID and uses that with the `item` lens in the `Map` type to mutate an `Appt`.  Very powerful stuff.

There are a number of useful lenses in the collection types that can do common things like mutate by index, head, tail, last, etc.

## The awful `out` parameter
This has to be one of the most awful patterns in C#:

```C#
    int result;
    if( Int32.TryParse(value, out result) )
    {
        ...
    }
    else
    {
        ...
    }
```
There's all kinds of wrong there.  Essentially the function needs to return two pieces of information:

* Whether the parse was a success or not
* The successfully parsed value

This is a common theme throughout the .NET framework.  For example on `IDictionary.TryGetValue`

```C#
    int value;
    if( dict.TryGetValue("thing", out value) )
    {
       ...
    }
    else
    {
       ...
    }
```       

So to solve it we now have methods that instead of returning `bool`, return `Option<T>`.  If the operation fails it returns `None`.  If it succeeds it returns `Some(the value)` which can then be matched.  Here's some usage examples:

```C#
    
    // Attempts to parse the value, uses 0 if it can't
    int res = parseInt("123").IfNone(0);

    // Attempts to parse the value, uses 0 if it can't
    int res = ifNone(parseInt("123"), 0);

    // Attempts to parse the value, doubles it if can, returns 0 otherwise
    int res = parseInt("123").Match(
                  Some: x => x * 2,
                 None: () => 0
              );

    // Attempts to parse the value, doubles it if can, returns 0 otherwise
    int res = match( parseInt("123"),
                     Some: x => x * 2,
                     None: () => 0 );
```

`out` method variants
* `IDictionary<K, V>.TryGetValue`
* `IReadOnlyDictionary<K, V>.TryGetValue`
* `int.TryParse` becomes `parseInt`
* `long.TryParse` becomes `parseLong`
* `short.TryParse` becomes `parseShort`
* `char.TryParse` becomes `parseChar`
* `sbyte.TryParse` becomes `parseSByte`
* `byte.TryParse` becomes `parseByte`
* `ulong.TryParse` becomes `parseULong`
* `uint.TryParse` becomes `parseUInt`
* `ushort.TryParse` becomes `parseUShort`
* `float.TryParse` becomes `parseFloat`
* `double.TryParse` becomes `parseDouble`
* `decimal.TryParse` becomes `parseDecimal`
* `bool.TryParse` becomes `parseBool`
* `Guid.TryParse` becomes `parseGuid`
* `DateTime.TryParse` becomes `parseDateTime`
* `DateTimeOffset.TryParse` becomes `parseDateTimeOffset`
* `Enum.TryParse` becomes `parseEnum`

_any others you think should be included, please get in touch_


## Ad-hoc polymorphism

> _This is where things get a little crazy!  This section is taking what's possible with C# to its limits.  None of what follows is essential for 99% of the use cases for this library.  However, the seasoned C# programmer will recognise some of the issues raised (like no common numeric base-type); and experienced functional programmers will recognise the category theory creeping in...  Just remember, this is all optional, but also pretty powerful if you want to go for it_.

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
        A Add(A x, A y);
        A Subtract(A x, A y);
        ...
    }
```
Notice how there are two arguments to `Add` and `Subtract`.  Normally if I was going to implement this `interface` the left-hand-side of the `Add` and `Subtract` would be `this`.  I will implement the _ad-hoc_ class-instance to demonstrate why that is:
```c#
    public struct TInt : Num<int>
    {
        public int Add(int x, int y) => x + y;
        public int Subtract(int x, int y) => x - y;
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
        public float Add(float x, float y) => x + y;
        public float Subtract(float x, float y) => x - y;
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
    int a   = DoubleIt<TInt, int>(5);        // 10
    float b = DoubleIt<TFloat, float>(5.25); // 10.5
```
By expanding the amount of operations that the `Num<A>` type-class can do, you can perform any numeric operation you like.  If you like you can add new numeric types (say for complex numbers, or whatever), where the rules of the type are kept in the _ad-hoc_ instance.

Luckily you don't need to do that, because I have created the `Num<A>` type (in the `LanguageExt.TypeClasses` namespace), as well as `Floating<A>` (with all of the operations from `Math`; like `Sin`, `Cos`, `Exp`, etc.).  `Num<A>` also has a base-type of `Arithmetic<A>` which supports `Plus`, `Subtract`, `Product`, `Negate`.  This is for types which don't need the full spec of the `Num<A>` type.  I have also mapped all of the core numeric types to instances: `TInt`, `TShort`, `TLong`, `TFloat`, `TDouble`, `TDecimal`, `TBigInt`, etc.  So it's possible to write truly generic numeric code once.

> There's no getting around the fact that providing the class-instance in the generic arguments list is annoying (and later you'll see how annoying).  The Roslyn team are looking into a type-classes like feature for a future version of C# (variously named: 'Concepts' or 'Shapes').  So this will I'm sure be rectified, and when it is, it will be implemented exactly as I am using them here.  
> 
> Until then the pain of providing the generic arguments must continue.  You do however get a _super-powered C#_ in the mean-time.  
> 
> The need to write this kind of super-generic code is rare; but when you need it, _you need it_ - and right now this is simply the most powerful way.

#### `Eq<A>`

Next up is `Eq<A>`.  Equality testing in C# is an absolute nightmare.  From the different semantics of `Equals` and `==`, to `IEqualityComparer`, and the enormous hack which is `EqualityComparer.Default` (which doesn't blow up at compile-time if your code is wrong).

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
    int x = OrdArray<TInt, int>.Inst.Compare(new [] {1,2}, new [] {1,2}); // 0
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
        MB Bind<MB, B>(MA ma, Func<A, MB> bind);
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
        MB Bind<MonadB, MB, B>(MA ma, Func<A, MB> bind) 
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
The eagle eyed reader will notice that this actually allows binding to any resulting monad (not just `Option<B>`).  I'm sure some may consider labelling this a monad as incorrect, but it works, it's type-safe, it's efficient, and performs the exact same function and so I am happy to use the term. 

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

Often you'll find yourself with nested monadic types `Option<Lst<A>>`, `Seq<Either<L, R>>`, `Try<Validation<Fail, Success>>`, ..., and you want to work with the bound value(s) of `A` without having to unwrap/match the values away.  And so there are around 100,000 lines of generated code for working with 'transformer types'. 

There is a new [`MonadTrans`](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt.TypeClasses/MonadTrans_OuterMonad_OuterType_InnerMonad_InnerType_A.htm) type-class and a default instance called [`Trans`](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Trans_OuterMonad_OuterType_InnerMonad_InnerType_A.htm).  It does all the heavy lifting, and it is what the generated code uses (it's also what you'd need to use if you create your own monadic types and you want to build transformers for the various pairs of monadic types).

For every pair of nested monads: `Lst<Option<A>>`, `Try<Either<L, A>>`, etc.  there are the following extension methods (this is for `Arr<Lst<A>>`):
```c#

// Sums all the bound value(s)
A SumT<NumA, A>(this Arr<Lst<A>> ma);

// Counds all the bound value(s)
int CountT<A>(this Arr<Lst<A>> ma);

// Monadic bind on the inner monad
Arr<Lst<B>> BindT<A, B>(this Arr<Lst<A>> ma, Func<A, Lst<B>> f);

// Flips the inner and outer monads (using the rules of the inner and outer 
// monads to compose the result) and performs a map operation on the bound values
Lst<Arr<B>> Traverse<A, B>(this Arr<Lst<A>> ma, Func<A, B> f);

// Flips the inner and outer monads (using the rules of the inner and outer 
// monads to compose the result) 
Lst<Arr<A>> Sequence<A>(this Arr<Lst<A>> ma);

// Maps the bound value(s)
Arr<Lst<B>> MapT<A, B>(this Arr<Lst<A>> ma, Func<A, B> f);

// Folds the bound value(s)
S FoldT<S, A>(this Arr<Lst<A>> ma, S state, Func<S, A, S> f);

// Reverse folds the bound value(s)
S FoldBackT<S, A>(this Arr<Lst<A>> ma, S state, Func<S, A, S> f);

// Returns true if f(x) returns true for any of the bound value(s)
bool ExistsT<A>(this Arr<Lst<A>> ma, Func<A, bool> f);

// Returns true if f(x) returns true for all of the bound value(s)
bool ForAllT<A>(this Arr<Lst<A>> ma, Func<A, bool> f);

// Iterates all of the bound values
Unit IterT<A>(this Arr<Lst<A>> ma, Action<A> f);

// Filters the bound value(s) with the predicate
Arr<Lst<A>> FilterT< A>(this Arr<Lst<A>> ma, Func<A, bool> pred);

// Filters the bound value(s) with the predicate
Arr<Lst<A>> Where<A>(this Arr<Lst<A>> ma, Func<A, bool> pred);

// Maps the bound value(s)
Arr<Lst<A>> Select<A, B>(this Arr<Lst<A>> ma, Func<A, B> f);

// LINQ monadic bind and project on the bound value(s)
Arr<Lst<C>> SelectMany<A, B, C>(
        this Arr<Lst<A>> ma,
        Func<A, Lst<B>> bind,
        Func<A, B, C> project);

// Plus operation on the bound value(s)
Arr<Lst<A>> PlusT<NUM, A>(this Arr<Lst<A>> x, Arr<Lst<A>> y) where NUM : struct, Num<A>;

// Subtraction operation on the bound value(s)
Arr<Lst<A>> SubtractT<NUM, A>(this Arr<Lst<A>> x, Arr<Lst<A>> y) where NUM : struct, Num<A>;

// Product operation on the bound value(s)
Arr<Lst<A>> ProductT<NUM, A>(this Arr<Lst<A>> x, Arr<Lst<A>> y) where NUM : struct, Num<A> =>
        ApplyT(default(NUM).Product, x, y);

// Divide operation on the bound value(s)
Arr<Lst<A>> DivideT<NUM, A>(this Arr<Lst<A>> x, Arr<Lst<A>> y) where NUM : struct, Num<A>;

// Semigroup append operation on the bound value(s)
AppendT<SEMI, A>(this Arr<Lst<A>> x, Arr<Lst<A>> y) where SEMI : struct, Semigroup<A>;

// Comparison operation on the bound value(s)
int CompareT<ORD, A>(this Arr<Lst<A>> x, Arr<Lst<A>> y) where ORD : struct, Ord<A>;

// Equality operation on the bound value(s)
bool EqualsT<EQ, A>(this Arr<Lst<A>> x, Arr<Lst<A>> y) where EQ : struct, Eq<A>;

// Applicative apply operation on the bound value(s)
Arr<Lst<A>> ApplyT<A, B>(this Func<A, B> fab, Arr<Lst<A>> fa);

// Application apply operation on the bound value(s)
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


### The rest

This README.md is a basic introduction to the library.  It is however full of many, many useful types, so do check the [API Reference](https://louthy.github.io/language-ext/index.htm) for more info.
