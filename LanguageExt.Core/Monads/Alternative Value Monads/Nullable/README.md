The `Nullable` extensions turns the `Nullable<T>` type from .NET into a monad.  This means you can use them in LINQ expressions, just like
the other monadic types in this library.  There are natural transformation functions to help convert from a nullable into other types, i.e.

    int? x = ...
    Option<int> mx = x.ToOption();