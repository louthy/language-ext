![lang-ext](http://www.4four.org/images/lang-ext-logo.png)

C# Functional Language Extensions
=================================

[![Join the chat at https://gitter.im/louthy/language-ext](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/louthy/language-ext?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) 

This library uses and abuses the features of C# 6+ to provide a functional 'Base class library', that, if you squint, can look like 
extensions to the language itself.

#### [API Reference](https://louthy.github.io/language-ext)

Nu-get package | Description
---------------|-------------
[LanguageExt.Core](https://www.nuget.org/packages/LanguageExt.Core) | All of the core types and functional 'prelude'.  __This is all that's needed to get started__.
[LanguageExt.FSharp](https://www.nuget.org/packages/LanguageExt.FSharp) | F# to C# interop library.  Provides interop between the LanguageExt.Core types (like `Option`, `List` and `Map`) to the F# equivalents, as well as interop between core BCL types and F#
[LanguageExt.Parsec](https://www.nuget.org/packages/LanguageExt.Parsec) | Port of the [Haskell parsec library](https://hackage.haskell.org/package/parsec)

__Twitter:__ 
https://twitter.com/paullouth


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

There is also:
* [`LanguageExt.List`](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/List_.htm)
* [`LanguageExt.Map`](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Map_.htm)
* [`LanguageExt.HashMap`](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/HashMap_.htm)
* [`LanguageExt.Queue`](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Queue_.htm)
* [`LanguageExt.Set`](https://louthy.github.io/language-ext/LanguageExt.CoreLanguageExt/Set_.htm)
* [`LanguageExt.HashSet`](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/HashSet_.htm)
* [`LanguageExt.Stack` ](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Stack_.htm)
* [`LanguageExt.TypeClass`](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/TypeClass_.htm)
* [`LanguageExt.Parsec`](https://louthy.github.io/language-ext/LanguageExt.Parsec/LanguageExt.Parsec/index.htm)
* [`LanguageExt.Parsec.Char`](https://louthy.github.io/language-ext/LanguageExt.Parsec/LanguageExt.Parsec/Char_.htm)
* [`LanguageExt.Parsec.Expr`](https://louthy.github.io/language-ext/LanguageExt.Parsec/LanguageExt.Parsec/Expr_.htm)
* [`LanguageExt.Parsec.Prim`](https://louthy.github.io/language-ext/LanguageExt.Parsec/LanguageExt.Parsec/Prim_.htm)
* `LanguageExt.Parsec.Token`
* [`LanguageExt.Parsec.Indent`](https://louthy.github.io/language-ext/LanguageExt.Parsec/LanguageExt.Parsec/Indent_.htm)
* `LanguageExt.UnitsOfMeasure`


_(more on those later)_

### Features

This library is quickly becoming a 'Base Class Library' for functional programming in C#.  The features include:

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
`Core` | `OptionUnsafe<T>` | [Option monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/OptionUnsafe_A.htm) that can be used with `null` values
`Core` | `Either<L,R>` | [Right/Left choice monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Either_L_R.htm) that won't accept `null` values
`Core` | `EitherUnsafe<L,R>` | [Right/Left choice monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/EitherUnsafe_L_R.htm) that can be used with `null` values
`Core` | `Try<A>` | [Exception handling lazy monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Try_A.htm)
`Core` | `TryAsync<A>` | [Asynchronous exception handling lazy monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/TryAsync_A.htm)
`Core` | `TryOption<A>` | [Option monad with third state](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/TryOption_A.htm) 'Fail' that catches exceptions
`Core` | `TryOptionAsync<A>` | [Asynchronous Option monad with third state](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/TryOptionAsync_A.htm) 'Fail' that catches exceptions
`Core` | `Reader<E, A>` | [Reader monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Reader_Env_A.htm)
`Core` | `Writer<MonoidW, W, T>` | [Writer monad that logs to a `W` constrained to be a Monoid](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Writer_MonoidW_W_A.htm)
`Core` | `State<S, A>` | [State monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/State_S_A.htm)
`Parsec` | `Parser<A>` | [String parser monad and full parser combinators library](https://louthy.github.io/language-ext/LanguageExt.Parsec/LanguageExt.Parsec/index.htm#Parser_T)
`Parsec` | `Parser<I, O>` | [Parser monad that can work with any input stream type](https://louthy.github.io/language-ext/LanguageExt.Parsec/LanguageExt.Parsec/index.htm#Parser_I_O)
`Core` | `NewType<SELF, A, PRED>` | [Haskell `newtype` equivalent](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/NewType_NEWTYPE_A_PRED.htm) i.e: `class Hours : NewType<Hours, double> { public Hours(double value) : base(value) { } }`.  The resulting type is: equatable, comparable, foldable, a functor, monadic, and iterable
`Core` | `NumType<SELF, NUM, A, PRED>` | [Haskell `newtype` equivalent but for numeric types](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/NumType_NUMTYPE_NUM_A_PRED.htm) i.e: `class Hours : NumType<Hours, TDouble, double> { public Hours(double value) : base(value) { } }`.  The resulting type is: equatable, comparable, foldable, a functor, a monoid, a semigroup, monadic, iterable, and can have basic artithmetic operations performed upon it.
`Core` | `FloatType<SELF, FLOATING, A, PRED>` | [Haskell `newtype` equivalent but for real numeric types](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/FloatType_SELF_FLOATING_A_PRED.htm) i.e: `class Hours : FloatType<Hours, TDouble, double> { public Hours(double value) : base(value) { } }`.  The resulting type is: equatable, comparable, foldable, a functor, a monoid, a semigroup, monadic, iterable, and can have complex artithmetic operations performed upon it.
`Core` | `Nullable<T>` extensions | [Extension methods for `Nullable<T>`](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/NullableExtensions_.htm) that make it into a functor, applicative, foldable, iterable and a monad
`Core` | `Task<T>` extensions | [Extension methods for `Task<T>`](https://louthy.github.io/language-ext/LanguageExt.Core/TaskExtensions_.htm) that make it into a functor, applicative, foldable, iterable and a monad
`Core` | Monad transformers | A higher kinded type (ish)
`Core` | Currying | [Translate the evaluation of a function that takes multiple arguments into a sequence of functions, each with a single argument](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Prelude_.htm#curry<T1, T2, R>)
`Core` | Partial application | [the process of fixing a number of arguments to a function, producing another function of smaller arity](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Prelude_.htm#par<T1, T2, R>)
`Core` | Memoization | [An optimization technique used primarily to speed up programs by storing the results of expensive function calls and returning the cached result when the same inputs occur again](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Prelude_.htm#memo<T, R>)
`Core` | [Improved lambda type inference](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Prelude_.htm#fun<R>) | `var add = fun( (int x, int y) => x + y)`
`Core` | [`IObservable<T>` extensions](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/ObservableExt_.htm)  |

It started out trying to deal with issues in C#, that after using Haskell and F# started to frustrate me:

* Poor tuple support
* Null reference problem
* Lack of lambda and expression inference 
* Void isn't a real type
* Mutable lists and dictionaries
* The awful 'out' parameter

## Poor tuple support
I've been crying out for proper tuple support for ages.  When this library was created we we're no closer (C# 6).  
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

    var sumA = (1, 2, 3).Sum<TInt, int>();                                   // 6
    var sumB = (2, 4, 8).Product<TInt, int>();                               // 64
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
    Option<int> GetValue() => 
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

Now the problem with C# is it can't do higher order polymorphism  (imagine saying `Monad<Option<T>>` instead of `Option<T>`, `Either<L,R>`, `Try<T>`, `IEnumerable<T>`.  And then the resulting type having all the features of the `Option` as well as the standard interface to `Monad`).

There's a kind of cheat way to do it in C# through extension methods.  It still doesn't get you a single type called `Monad<T>`, so it has limitations in terms of passing it around.  However it makes some of the problems of dealing with nested monadic types easier.

For example, below is a list of optional integers: `Lst<Option<int>>` (see lists later).  We want to double all of the `Some` values, leave the `None` alone and keep everything in the list:

```C#
    using LanguageExt.Trans;  // Required for all transformer extension methods

    var list = List(Some(1), None, Some(2), None, Some(3));
    
    var presum = list.SumT();                                // 6
    
    list  = list.MapT( x => x * 2 );
    
    var postsum = list.SumT();                               // 12
```
Notice the use of `MapT` instead of `Map` (and `SumT` instead of `Sum`).  If we used `Map` (equivalent to `Select` in `LINQ`), it would look like this:
```C#
    var list  = List(Some(1), None, Some(2), None, Some(3));
    
    var presum = list.Map(x => x.Sum()).Sum();
    
    list = list.Map( x => x.Map( v => v * 2 ) );
    
    var postsum = list.Map(x => x.Sum()).Sum();
```
As you can see the intention is much clearer in the first example.  And that's the point with functional programming most of the time.  It's about declaring intent rather than the mechanics of delivery.

To make this work we need extension methods for `List<Option<T>>` that define `MapT` and `SumT` [for the one  example above].  And we need one for every pair of monads in this library (for one level of nesting `A<B<T>>`), and for every function from the 'standard functional set' listed above.  So that's 13 monads * 13 monads * 14 functions.  That's a lot of extension methods.  So there's T4 template that generates 'monad transformers' that allows for nested monads.

This is super powerful, and means that most of the time you can leave your `Option<T>` or any of the monads in this library wrapped.  You rarely need to extract the value.  Mostly you only need to extract the value to pass to the BCL or Third-party libraries.  Even then you could keep them wrapped and use `Iter` or `IterT`.


## if( arg == null ) throw new ArgumentNullException("arg")
Another horrible side-effect of `null` is having to bullet-proof every function that take reference arguments.  This is truly tedious.  Instead use this:
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
    var people = Map( Tuple(1, "Rod"),
                      Tuple(2, "Jane"),
                      Tuple(3, "Freddy") );
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
* `IImmutableDictionary<K, V>.TryGetValue`
* `IImmutableSet<K, V>.TryGetValue`
* `Int32.TryParse` becomes `parseInt`
* `Int64.TryParse` becomes `parseLong`
* `Int16.TryParse` becomes `parseShort`
* `Char.TryParse` becomes `parseChar`
* `Byte.TryParse` becomes `parseByte`
* `UInt64.TryParse` becomes `parseULong`
* `UInt32.TryParse` becomes `parseUInt`
* `UInt16.TryParse` becomes `parseUShort`
* `float.TryParse` becomes `parseFloat`
* `double.TryParse` becomes `parseDouble`
* `decimal.TryParse` becomes `parseDecimal`
* `bool.TryParse` becomes `parseBool`
* `Guid.TryParse` becomes `parseGuid`
* `DateTime.TryParse` becomes `parseDateTime`

_any others you think should be included, please get in touch_



### The rest

This README.md is a basic introduction to the library.  It is however full of many, many useful types, so do check the [API Reference](https://louthy.github.io/language-ext/index.htm) for more info.

### Contributions
All contributors are welcome.  For anything other than bug fixes please get in touch via the issues page.  There are no fixed rules on what should and shouldn't be in this library, but some features are more valuable than others, and some require long-term maintenance that outweighs the value of the 
feature.  So please get sign-off from the project leader (Paul Louth) before putting in an excessive amount of work. 

If you would just like to get involved, but don't have any major feature work to submit, then the project will always benefit from more unit-tests, documentation, peer-review, etc.
