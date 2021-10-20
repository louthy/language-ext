__If you're new to this library or functional-programming this is almost certainly not the place to start browsing!__

Type-classes try to replicate the type-classes of Haskell in C#.  They pair with the class-instances using ad-hoc polymorphism.

Ad-hoc polymorphism has long been believed to not be possible in C#. However with some cunning _it is_. Ad-hoc polymorphism allows 
programmers to add traits to a type later. For example in C# it would be amazing if we had an interface called `INumeric` for numeric 
types like `int`, `long`, `double`, etc. The reason this doesn't exist is if you write a function like:

    INumeric Add(INumeric x, INumeric y) => x + y;

Then it would cause boxing. Which is slow (well, slower). I can only assume that's why it wasn't added by the BCL team. Anyway, it's 
possible to create a numeric type, very much like a type-class in Haskell, and ad-hoc instances of the numeric type-class that allow 
for generic numeric operations without boxing.  [See the wiki for a deeper dive into ad-hoc polymorphism](https://github.com/louthy/language-ext/wiki/Ad-hoc-polymorphism)