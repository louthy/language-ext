`Either` monads support either a `L` (left) or a `R` (right) value.  `L` is traditionally used as an error carrier, and any `Either` monad
carrying an `L` will short-cut any bind operations and return the `L` (kinda like an `Exception`).  However, they're not only for that, 
and can be used to carry an alternative value which could be mapped using `BiMap`, or `MapLeft`.

Here we have three flavours of `Either`:

1. `Either<L, R>` the default either monad.  It does not allow `null` in its `L` or `R` cases.
2. `EitherUnsafe<L, R>` as above, but it does allow `null` in its `L` and `R` cases.
3. `EitherAsync<L, R>` is equivalent to `Task<Either<L, R>>`, but much more convenient to use, especially with LINQ expressions

You can construct an `Either` using the constructor functions in the `Prelude`:

    Either<string, int> ma = Left<string, int>("this is an error");
    Either<string, int> mb = Right<string, int>(123);

There are also convenience types called `Left` and `Right`, that don't need both generics providing.  They can often make it a little 
easier to work with `Either`:

    Either<string, int> ma = Left("this is an error");
    Either<string, int> mb = Right(123);

It uses implicit casts to work out what the type should be.  Note, if you're having trouble getting the types resolved, just specify the 
type parameters.
