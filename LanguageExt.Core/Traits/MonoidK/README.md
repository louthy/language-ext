`MonoidK<F>` inherits `SemigroupK<F>`.
The way to think of `MonoidK<F>` is a monoid for higher-kinds (`K<F, A>` types, rather than `A` types).  

What that means is that any type that implements the `MonoidK<F>` trait gains an `Empty()` (a 'zero'/identity element ) 
value as well as the ability to `Combine` two `K<F, A>` structures together into one.  Many implementations of 
`MonoidK<F>` use `Combine` to catch errors and propagate.  So, if the first `K<F, A>` argument to `Combine` fails, it 
simply returns the second argument.  If it succeeds, then the result of the first is returned.  This works a bit like 
`null` propagation with the `??` operator.  And while this isn't always how `MonoidK` is implemented, it's useful to know.

The `MonoidK<F>` trait combines with `Applicative<F>` and `Monad<F>` traits to provide the following default 
functionality:

* `Filter` | `filter` - if your type supports `Monad<F>` and `MonoidK<F>` you get free filtering and `Where` LINQ extension
* `Choose` | `choose` - if your type supports `Monad<F>` and `MonoidK<F>` then `Choose` does filtering and mapping 
* `OneOf` | `oneOf` - takes a collection of `K<F, A>` structures, returns the first one to succeed.
* `Some` | `some` - evaluates a `K<F, A>` structure repeatedly, collecting the `A` values, until it fails (at least one must succeed). Returns the `K<F, Seq<A>>`
* `Many` | `many` - evaluates a `K<F, A>` structure repeatedly, collecting the `A` values, until it fails. Returns the `K<F, Seq<A>>`
* `guard` - conditional failure 

Some of these you might recognise from the `Parsec` library.  This completely generalises the concept of alternative 
structure coalescing. 