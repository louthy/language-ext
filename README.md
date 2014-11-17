C# Language Extensions
======================

Using and abusing the features of C# 6 to provide lots of helper methods and types

## Introduction

One of the great new features of C# 6 is that it allows us to treat static classes like namespaces.  This means that we can use static methods without qualifying them first.  This instantly gives us access to single term method names which look exactly like functions in functional languages.  I'm very much focussed on the functional paradigm at the moment, but I also have plenty of legacy C# to contend with.  So I created this library to bring some of the functional world into C#.

To use this library, simply include LanguageExt.Core.dll in your project.  And then stick `using LanguageExt;` at the top of each cs file that needs it.

What C# issues are we trying to fix?  Well, we can only paper over the cracks, but here's a glossary:

* Poor tuple support
* Null reference problem
* Lack of lambda and expression inference 
* Void isn't a real type
* List construction

## Poor tuple support
I've been crying out for proper tuple support for ages.  It looks like we're no closer with C# 6.  The standard way of creating them is ugly `Tuple.Create(foo,bar)` compared to functional languages where the syntax is often `(foo,bar)` and to consume them you must work with the standard properties of `Item1`..`ItemN`.  No more...

```C#
    var ab = tuple("a","b");
```

I chose the lower-case `tuple` to avoid conflicts between other types and existing code.  I think also tuples should be considered fundamental like `int`, and therefore deserves a lower-case name.  I do this with a number of other functions, I realise this might be painful for you stalwart OO guys, but I think this is a better approach.  Happy to discuss it however :)

So consuming the tuple is now handled using `With`, which projects the `Item1`..`ItemN` onto a lambda function (or action):

```C#
    var name = tuple("Paul","Louth");
    var res = name.With( (first,last) => "Hello "+first+" "+last );
```
This allows the tuple properties to have names, and it also allows for fluent handling of functions that return tuples.

## Null reference problem
`null` must be the biggest mistake in the whole of computer language history.  I realise the original designers of C# had to make pragmatic decisions, it's a shame this one slipped through though.  So, what to do about the 'null problem'?

`null` is often used to indicate 'no value'.  i.e. the method called can't produce a value of the type it said it was going to produce, and therefore it gives you 'no value'.  The thing is the when the 'no value' instruction is passed to the consuming code, it gets assigned to a variable of type T, the same type that the function said it was going to return, except this variable now has a timebomb in it.  You must continually check if the value is null, if it's passed around it must be checked too.  

As we all know it's only a matter of time before a null reference bug crops up because the variable wasn't checked.

Functional languages use what's known as an 'option type'.  In F# it's called `Option` in Haskell it's called `Maybe`...

## Optional
It works in a very similar way to `Nullable<T>` except it works with all types rather than just value types.  It's a struct and therefore can't be null.  An instance can be created by either calling `Some(value)`, which represents a positive 'I have a value' response;  Or `None`, which is the equivalent of returning `null`.

So why is it any better than returning `T` and using `null`.  It seems we can have a non-value response again right?  Yes, that's true, however you're forced to acknowledge that fact, and write code to handle both possible outcomes.  This bulletproofs your code.  

This is how you create an `Option<int>`:

```C#
var optional = Some(123);
```

To access the value you must check that it's valid first:

```C#
    optional.Match( 
        Some: v  => Assert.IsTrue(v == 123),
        None: () => Assert.Fail("Shouldn't get here")
        );
```
An alternative (more functional) way of matching is this:

```C#
    Match(optional, 
        Some: v  => Assert.IsTrue(v == 123),
        None: () => Assert.Fail("Shouldn't get here") 
        );
```

To smooth out the process of returning Option<T> types from methods there are some implicit conversion operators:

```C#
    // Automatically converts the integer to a Some of int
    Option<int> ImplicitSomeConversion() => 1000;

    // Automatically converts to a None of int
    Option<int> ImplicitNoneConversion() => None;
    
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

That will compile.  However the `Option<T>` will notice what you're up to and give you a `None` anyway.  This is pretty important to remember.  

For those of you who know what a monad is, then the `Option<T>` type implements `Select` and `SelectMany` and so can be use in LINQ expressions:

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
        None: () => Assert.Fail("Shouldn't get here")
    );

    // This expression bails out once it gets to the None, and therefore doesn't calculate x+y+z
    (from x in two
     from y in four
     from _ in Option<int>.None
     from z in six
     select x + y + z)
    .Match(
        Some: v => Assert.Fail("Shouldn't get here"),
        None: () => Assert.IsTrue(true)
    );
```

## Lack of lambda and expression inference 

One really annoying thing about the `var` type inference in C# is that it can't handle inline lamdas.  For example this won't compile, even though it's obvious it's a `Func<int>`.
```C#
    var fn = () => 123;
```
There are some good reasons for this, so best not to bitch too much.  Instead use the `fun` function from this library:
```C#
    var fn = fun( () => 123 );
```
This will work for `Func<..>` and `Action<..>` types of up to seven generic arguments.  To do the same for `Expression<..>`, use the `expr` function:

```C#
    var e = expr( () => 123 );
```

## Void isn't a real type

Functional languages have a concept of a type that has one possible value, itself, called Unit.  As an example `bool` has two values: `true` and `false`.  `Unit` has one value, usually represents in functional languages as `()`.  You can imagine that methods that take no arguments actually take one argument of `()`.  Anyway, we can't use the `()` representation in C#, so `LanguageExt` now provides `unit`.

```C#

    public Unit Empty()
    {
        return unit;
    }
```

`Unit` is the type and `unit` is the value.  It is used throughout the `LanguageExt` library instead of `void` so that it can be used with `Option` or `Either`.  

## List construction

Support for `cons`, which is the functional way of constructing lists:
```C#
    var test = cons(1, cons(2, cons(3, cons(4, cons(5)))));

    var array = test.ToArray();

    Assert.IsTrue(array[0] == 1);
    Assert.IsTrue(array[1] == 2);
    Assert.IsTrue(array[2] == 3);
    Assert.IsTrue(array[3] == 4);
    Assert.IsTrue(array[4] == 5);
```

There are a couple of additional `cons` functions which don't strictly follow in the spirit of the well known functional cons 'operator', but they're damn useful:

```C#
    // Creates a list of one int 
    var list = cons(1000);
```

```C#
    // Creates a list of two ints
    var list = cons(1000,2000);
```
Usually one would cons with an empty list.  But adding a function called empty seemed overkill, and would probably not do too well with C#'s type inference system.

### Future
There's more to come with this library.  Feel free to get in touch with any suggestions.
