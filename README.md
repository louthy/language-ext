![lang-ext](http://www.4four.org/images/lang-ext-logo.png)

C# Functional Language Extensions
=================================

Using and abusing the features of C# 6 to provide lots of helper functions and types, which, if you squint, can look like extensions to the language itself.

__Now on NuGet: https://www.nuget.org/packages/LanguageExt/__

## Introduction

One of the great new features of C# 6 is that it allows us to treat static classes like namespaces.  This means that we can use static methods without qualifying them first.  This instantly gives us access to single term method names which look exactly like functions in functional languages.  This library brings some of the functional world into C#.  It won't always sit well with the seasoned C# OO-only programmer, especially the choice of lowercase names for a lot of functions and the seeming 'globalness' of a lot of the library.  

I can understand that much of this library is non-idiomatic; But when you think of the journey C# has been on, is idiomatic necessarily right?  A lot of C#'s idioms are inherited from Java and C# 1.0.  Since then we've had generics, closures, Func, LINQ, async...  C# as a language is becoming more and more like a  functional language on every release.  In fact the bulk of the new features are either inspired by or directly taken from features in functional languages.  So perhaps it's time to move the C# idioms closer to the functional world's idioms?

Even if you don't agree, I guess you can pick 'n' choose what to work with.  There's still plenty here that will help day-to-day.

To use this library, simply include LanguageExt.Core.dll in your project.  And then stick this at the top of each cs file that needs it:
```C#
using LanguageExt;
using LanguageExt.Prelude;
```

`LanguageExt` contains the types, and `LanguageExt.Prelude` contains the helper functions.  There is also `LanguageExt.List` and `LanguageExt.Map`, more on those later.

What C# issues are we trying to fix?  Well, we can only paper over the cracks, but here's a summary:

* Poor tuple support
* Null reference problem
* Lack of lambda and expression inference 
* Void isn't a real type
* Mutable lists and dictionaries
* The awful 'out' parameter

## Poor tuple support
I've been crying out for proper tuple support for ages.  It looks like we're no closer with C# 6.  The standard way of creating them is ugly `Tuple.Create(foo,bar)` compared to functional languages where the syntax is often `(foo,bar)` and to consume them you must work with the standard properties of `Item1`...`ItemN`.  No more...

```C#
    var ab = tuple("a","b");
```

Now isn't that nice?  I chose the lower-case `tuple` to avoid conflicts between other types and existing code.  I think also tuples should be considered fundamental like `int`, and therefore deserves a lower-case name.  

Consuming the tuple is now handled using `With`, which projects the `Item1`...`ItemN` onto a lambda function (or action):

```C#
    var name = tuple("Paul","Louth");
    var res = name.With( (first,last) => "Hello \{first} \{last}" );
```
Or, you can use a more functional approach:
```C#
    var name = tuple("Paul","Louth");
    var res = with( name, (first,last) => "Hello \{first} \{last}" );
```
This allows the tuple properties to have names, and it also allows for fluent handling of functions that return tuples.

## Null reference problem
`null` must be the biggest mistake in the whole of computer language history.  I realise the original designers of C# had to make pragmatic decisions, it's a shame this one slipped through though.  So, what to do about the 'null problem'?

`null` is often used to indicate 'no value'.  i.e. the method called can't produce a value of the type it said it was going to produce, and therefore it gives you 'no value'.  The thing is that when 'no value' is passed to the consuming code, it gets assigned to a variable of type T, the same type that the function said it was going to return, except this variable now has a timebomb in it.  You must continually check if the value is `null`, if it's passed around it must be checked too.  

As we all know it's only a matter of time before a null reference bug crops up because the variable wasn't checked.  It puts C# in the realm of the dynamic languages, where you can't trust the value you're being given.

Functional languages use what's known as an 'option type'.  In F# it's called `Option` in Haskell it's called `Maybe`.  In the next section we'll see how it's used.

## Option
`Option<T>` works in a very similar way to `Nullable<T>` except it works with all types rather than just value types.  It's a `struct` and therefore can't be `null`.  An instance can be created by either calling `Some(value)`, which represents a positive 'I have a value' response;  Or `None`, which is the equivalent of returning `null`.

So why is it any better than returning `T` and using `null`?  It seems we can have a non-value response again right?  Yes, that's true, however you're forced to acknowledge that fact, and write code to handle both possible outcomes because you can't get to the underlying value without acknowledging the possibility of the two states that the value could be in.  This bulletproofs your code.  You're also explicitly telling any other programmers that: "This method might not return a value, make sure you deal with that".  This explicit declaration is very powerful.

This is how you create an `Option<int>`:

```C#
var optional = Some(123);
```
To access the value you must check that it's valid first:

```C#
    optional.Match( 
        Some: v  => Assert.IsTrue(v == 123),
        None: () => failwith<int>("Shouldn't get here")
        );
```
An alternative (functional) way of matching is this:

```C#
    match( optional, 
        Some: v  => Assert.IsTrue(v == 123),
        None: () => failwith<int>("Shouldn't get here") 
        );
```
Yet another alternative matching method is this:
```C#
    optional
        .Some( v  => Assert.IsTrue(v == 123) )
        .None( () => failwith<int>("Shouldn't get here") );
```
So choose your preferred method and stick with it.  It's probably best not to mix styles.

To smooth out the process of returning Option<T> types from methods there are some implicit conversion operators:

```C#
    // Automatically converts the integer to a Some of int
    Option<int> GetValue() => 1000;

    // Automatically converts to a None of int
    Option<int> GetValue() => None;
    
    // Will handle either a None or a Some returned
    Option<int> GetValue(bool select) =>
        select
            ? Some(1000)
            : None;
```

It's actually nearly impossible to get a `null` out of a function, even if the `T` in `Option<T>` is a reference type and you write `Some(null)`.  Firstly it won't compile, but you might think you can do this:

```C#
    private Option<string> GetStringNone()
    {
        string nullStr = null;
        return Some(nullStr);
    }
```

That will compile, but at runtime will throw a `ValueIsNullException`.  If you do this (below) you'll get a `None`.  

```C#
    private Option<string> GetStringNone()
    {
        string nullStr = null;
        return nullStr;
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


As well as the protection of the internal value of `Option<T>`, there's protection for the return value of the `Some` and `None` handler functions.  You can't return `null` from those either, an exception will be thrown.

```C#
    // This will throw a ResultIsNullException exception
    GetValue(true)
      .Some(x => (string)null)
      .None((string)null);
```

So `null` goes away if you use `Option<T>`.

Sometimes you just want to execute some specific behaviour when `None` is returned so that you can provide a decent default value, or raise an exception.  That is what the `failure` method is for:

```C#
    Option<int> optional = None;
        
    // Defaults to 999 if optional is None
    int value = optional.Failure(999);
```

You can also use a function to handle the failure:

```C#
    Option<int> optional = None;
        
    var backupInteger = fun( () => 999 );
        
    int value = optional.Failure(backupInteger);
```

There are also functional variants of `failure`:

```C#
    Option<int> optional = None;
        
    var backupInteger = fun( () => 999 );
        
    int value1 = failure(optional, 999);
    int value2 = failure(optional, backupInteger);
```

Essentially you can think of `Failure` as `Match` where the `Some` branch always returns the wrapped value 'as is'.  Therefore there's no need for a `Some` function handler.

If you know what a monad is, then the `Option<T>` type implements `Select` and `SelectMany` and is monadic.  Therefore it can be use in LINQ expressions.  

```C#
    var two = Some(2);
    var four = Some(4);
    var six = Some(6);

    // This exprssion succeeds because all items to the right of 'in' are Some of int.
    (from x in two
     from y in four
     from z in six
     select x + y + z)
    .Match(
        Some: v => Assert.IsTrue(v == 12),
        None: () => failwith<int>("Shouldn't get here")
    );

    // This expression bails out once it gets to the None, and therefore doesn't calculate x+y+z
    (from x in two
     from y in four
     from _ in Option<int>.None
     from z in six
     select x + y + z)
    .Match(
        Some: v => failwith<int>("Shouldn't get here"),
        None: () => Assert.IsTrue(true)
    );
```
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
If you're wondering how it works, well `Some<T>` is a `struct`, and has implicit conversation operators that convert a type of `T` to a type of `Some<T>`.  The constructor of `Some<T>` ensures that the value of `T` has a non-null value.

There is also an implicit cast operator from `Some<T>` to `Option<T>`.  The `Some<T>` will automatically put the `Option<T>` into a `Some` state.  It's not possible to go the other way and cast from `Option<T>` to `Some<T>`, because the `Option<T>` could be in a `None` state which wouid cause the `Some<T>` to throw `ValueIsNullException`.  We want to avoid exceptions being thrown, so you must explicitly `match` to extract the `Some` value.

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
    "Unitialised Some<T> in class member declaration."
```
So what's the best plan of attack to mitigate this?

* Don't use `Some<T>` for class members.  That means the class logic might have to deal with `null` however.
* Or, always initialise `Some<T>` class members.  Mistakes do happen though.

There's no silver bullet here unfortunately.

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

    // Wil compile
    var add = fun( (int x, int y) => x + y );
```

## Void isn't a real type

Functional languages have a concept of a type that has one possible value, itself, called `Unit`.  As an example `bool` has two values: `true` and `false`.  `Unit` has one value, usually represented in functional languages as `()`.  You can imagine that methods that take no arguments, do in fact take one argument of `()`.  Anyway, we can't use the `()` representation in C#, so `LanguageExt` now provides `unit`.

```C#
    public Unit Empty()
    {
        return unit;
    }
```

`Unit` is the type and `unit` is the value.  It is used throughout the `LanguageExt` library instead of `void`.  The primary reason is that if you want to program functionally then all functions should return a value and `void` isn't a first-class value.  This can help a lot with LINQ expressions for example.

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

There's support for `cons`, which is the functional way of constructing lists:
```C#
    var test = cons(1, cons(2, cons(3, cons(4, cons(5, empty<int>())))));

    var array = test.ToArray();

    Assert.IsTrue(array[0] == 1);
    Assert.IsTrue(array[1] == 2);
    Assert.IsTrue(array[2] == 3);
    Assert.IsTrue(array[3] == 4);
    Assert.IsTrue(array[4] == 5);
```

Note, this isn't the strict definition of `cons`, but it's a pragmatic implementation that returns an `IEnumerable<T>`, is lazy, and behaves the same.

Functional languages usually have additional list constructor syntax which makes the `cons` approach easier.  It usually looks something like this:

```F#
    let list = [1;2;3;4;5]
```

In C# it looks like this:

```C#
    var list = new int[] { 1, 2, 3, 4, 5 };
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
So we provide `list(...)` function which takes any number of parameters and turns them into a list:

```C#
    // Creates a list of five items
     var test = list(1, 2, 3, 4, 5);
```

This is much closer to the 'functional way'.  It also returns `IImmutableList<T>`.  So it's now easier to use immutable-lists than the mutable ones.  

Also `range`:

```C#
    // Creates a list of 1001 integers lazily.
    var list = range(1000,2000);
```

Some of the standard list functions are available.  These are obviously duplicates of what's in LINQ, therefore they've been put into their own `LanguageExt.List` namespace:

```C#
    // Generates 10,20,30,40,50
    var input = list(1, 2, 3, 4, 5);
    var output1 = map(input, x => x * 10);

    // Generates 30,40,50
    var output2 = filter(output1, x => x > 20);

    // Generates 120
    var output3 = fold(output2, 0, (x, s) => s + x);

    Assert.IsTrue(output3 == 120);
```

The above can be written in a fluent style also:

```C#
    var res = list(1, 2, 3, 4, 5)
                .map(x => x * 10)
                .filter(x => x > 20)
                .fold(0, (x, s) => s + x);

    Assert.IsTrue(res == 120);
```

Other list functions:
* `head`
* `headSafe` - returns `Option<T>`
* `tail`
* `foldr`
* `reduce`
* `each`
* `append`
* `rev`
* `sum`
* `zip`
* more coming...

We also support dictionaries.  Again the word Dictionary is such a pain to type, especially when they have a perfectly valid alternative name in the functional world: `map`.

To create an immutable map, you no longer have to type:

```C#
    var dict = ImmutableDictionary.Create<string,int>();
```
Instead you can use:
```C#
    var dict = map<string,int>();
```
Also you can pass in a list of tuples or key-value pairs, which will create a `ImmutableDictionary.Builder` before generating the immutable dictionary itself:
```C#
    var m = map<int, string>(
               tuple(1, "a"),
               tuple(2, "b"),
               tuple(3, "c")
            );
```
To read an item call:
```C#
    Option<string> result = find(m, 1);
```
This allows for branching based on whether the item is in the map or not:

```C#
    // Find the item, do some processing on it and return.
    var res = match( find(m, 100),
                        Some: v  => "Hello" + v,
                        None: () => "failed"
                   );
                   
    // Find the item and return it.  If it's not there, return "failed"
    var res = find(m, 100).Failure("failed");                   
    
    // Find the item and return it.  If it's not there, return "failed"
    var res = failure( find(m, 100), "failed" );
```

To set an item call:
```C#
    var m2 = setItem(m, 1, "x");
```

`map` functions (`using LanguageExt.Map`):
* `add`
* `set`
* `remove`
* `contains`
* `find`
* `each`
* `map`
* `filter`
* `length`
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
    int value1 = parseInt("123").Failure(0);

    // Attempts to parse the value, uses 0 if it can't
    int value2 = failure(parseInt("123"), 0);

    // Attempts to parse the value, dispatches it to the UseTheInteger
    // function if successful.  Throws an exception if not.
    parseInt("123").Match(
        Some: UseTheInteger,
        None: () => failwith<int>("Not an integer")
        );

    // Attempts to parse the value, dispatches it to the UseTheInteger
    // function if successful.  Throws an exception if not.
    match( parseInt("123"),
        Some: UseTheInteger,
        None: () => failwith<int>("Not an integer")
        );
```

### The rest
I haven't had time to document everything, so here's a quick list of what was missed:

Type or function | Description
-----------------|------------
`Either<Right,Left>` | Like `Option<T>`, however the `None` in `Option<T>` is called `Left` in `Either`, and `Some` is called `Right`.  Just remember: `Right` is right, `Left` is wrong.  Both `Right` and `Left` can hold values.  And they can be different types.  See the OptionEitherConfigSample for a demo.  Supports all the same functionality as `Option<T>`.
`SomeUnsafe()`, `RightUnsafe()`, `LeftUnsafe()` | These methods accept that sometimes `null` is a valid result, but you still want an option of saying `None`.  They allow `null` to propagate through, and it removes the `null` checks from the return value of `match`
`set<T>()` | ImmutableHashSet.Create<T>()
`stack<T>()` | ImmutableStack.Create<T>()
`array<T>()` | ImmutableArray.Create<T>()
`queue<T>()` | ImmutableQueue.Create<T>()
`freeze<T>()` | Converts an IEnumerable<T> to an IImmutableList<T>
`memo<T>(fn)` | Caches a function's result the first time it's called
`memo<T,R>(fn)` | Caches a result of a function once for each unique parameter passed to it
`ignore` | Takes one argument which it ignores and returns `unit` instead.
`Nullable<T>.ToOption()` | Converts a `Nullable<T>` to an `Option<T>`
`raise(exception)` | Throws the exception passed as an argument.  Useful in lambda's where a return value is needed.
`failwith(message)` | Throws an Exception with the message provided.  Useful in lambda's where a return value is needed.
`identity<T>()` | Identity function.  Returns the same value it was passed.
`IDictionary.TryGetValue()` and `IReadOnlyDictionary.TryGetValue()` | Variants that return `Option<T>`.

### Future
There's more to come with this library.  Feel free to get in touch with any suggestions.
