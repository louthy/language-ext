`Option` monads support either an `A` (success) value, or a `None` (no-value) option.  You can think of this as an alternative to using
`null` to represent a lack of a value.  `null` unfortunately still allows you to `.` into the interface of the decalred type, which means
there's a ticking time-bomb in every reference type.  

C# does now have the nullable references feature, which goes some way to removing the need for an optional type, however there's still edge
cases that mean the reference types are problematic.  It's also useful to build generic types and say this is an `Option<A>` - I don't care
if it's a value-type or reference-type, it's optional.

And finally, there's the automatic checking of `None` values when using `Option<A>` in LINQ expressions, or if you call `Map`.  This makes
working with optional values, and the implications for all of the code that works with it, fully declarative. 

Here we have three flavours of `Option`:

1. `Option<A>` the default optional monad.  It does not allow `null` in its `Some` case.
2. `OptionUnsafe<A>` as above, but it does allow `null` in its `Some` case.
3. `OptionAsync<A>` is equivalent to `Task<Option<A>>`, but much more convenient to use, especially with LINQ expressions

You can construct a `Some` using the constructor functions in the `Prelude`:

    Option<int> ma = Some(123);
    Option<int> mb = None;
