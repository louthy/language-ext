`Alternative<F>` inherits `SemiAlternative<F>`, `Applicative<F>`, `MonoidK<F>`.
The way to think of `Alternative<F>` is a monoid for applicative-functors.  

What that means is that any type that implements the `Alternative<F>` trait gains an `Empty()` (or, zero) value as well 
as the ability to `Combine` two structures together into one.

The `Alternative<F>` combines with `Applicative<F>` to provides the following default functionality:

* `filter` - you never need to implement once you have implemented `Alternative<F>`
* `choose` - Similar to `Filter` then `Map`.
* `oneOf` - takes a `Seq` of structures, returns the first one to succeed.
* `some` - runs the structure repeatedly until it succeeds.
* `many` - runs the structure repeatedly, collecting the results, until failure. Returns the collected results.
* `guard` - conditional failure 

Some of these you might recognise from the `Parsec` library.  This completely generalises the of alternative value 
coalescing. 