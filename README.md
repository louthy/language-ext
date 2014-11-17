C# Language Extensions
======================

Using and abusing the features of C# 6 to provide lots of helper methods and types

#Introduction

One of the great new features of C# 6 is that it allows us to treat static classes like namespaces.  This means that we can use static methods without qualifying them first.  This instantly gives us access to single term method names which look exactly like functions in functional languages.  I'm very much focussed on the functional paradigm at the moment, but I also have plenty of legacy C# to contend with.  So I created this library to bring some of the functional world into C#.

To use this library, simply include LanguageExt.Core.dll in your project.  And then stick `using LanguageExt;` at the top of each cs file that needs it.


#Tuple support
I've been crying out for proper tuple support for ages.  It looks like we're no closer with C# 6.  The standard way of creating them is ugly: `Tuple.Create(foo,bar)` and to consume them you must work with the standard properties of Item1..ItemN.  No more...

```
    var ab = tuple("a","b");
```

I chose the lower-case `tuple` to avoid conflicts between other types and existing code.  I think also tuple's should be considered fundamental like `int`, and therefore deserves a lower-case name.  I do this with a number of other functions.

So consuming the tuple is now handled using `With`, which projects the Item1..ItemN onto a lambda function (or action):

```
    var name = tuple("Paul","Louth");
    var res = name.With( (first,last) => "Hello "+first+" "+last );
```
   


