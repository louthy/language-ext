## `Error`

The `Error` type works like a discriminated-union, it is an `abstract record` type with many sub-cases
(which are listed below).  It is used extensively with various monadic types, like `Fin<A>`, the 
_Effect System_ monads of `Eff<A>`, `Eff<RT, A>`, `Aff<A>`, `Aff<RT, A>` and the compositional 
streaming Pipes features.  

> The reason they're buried in the `Common` namespace is because, `Error` is a common type name.  And so, this gives
the programmer a chance to not include it when `using LanguageExt;`

`Error` exists because `Exception` is really only meant for _exceptional_ errors. However, in C#-land we've been trained
to throw them even for *expected* errors.  

Instead we use `Error` to represent three key types of error:

* `Exceptional` - An unexpected error
* `Expected`    - An expected error
* `ManyErrors`  - Many errors (possibly zero)

These are the key base-types that indicate the *'flavour'* of the error.  For example, a 'user not found' error isn't
something exceptional, it's something we expect *might* happen.  An `OutOfMemoryException` however, *is*
exceptional - it should never happen, and we should treat it as such.

Most of the time we want sensible handling of expected errors, and bail out completely for something exceptional.  We also want 
to protect ourselves from information leakage.  Leaking exceptional errors via public APIs is a surefire way to open up more
information to hackers than you would like.  The `Error` derived types all try to protect against this kind of leakage without
losing the context of the type of error thrown.

Essentially an error is either created from an `Exception` or it isn't.  This allows for expected errors to be
represented without throwing exceptions, but also it allows for more principled error handling.  We can pattern-match on the
type, or use some of the built-in properties and methods to inspect the `Error`:

* `IsExceptional` - `true` for exceptional errors.  For `ManyErrors` this is `true` if _any_ of the errors are exceptional.
* `IsExpected` - `true` for non-exceptional/expected errors.  For `ManyErrors` this is `true` if _all_ of the errors are expected.
* `Is<E>(E exception)` - `true` if the `Error` is exceptional and any of the the internal `Exception` values are of type `E`.
* `Is(Error error)` - `true` if the `Error` matches the one provided.  i.e. `error.Is(Errors.TimedOut)`. 

The `Error` type can be constructed to be exceptional or expected.  For example, this is an expected error:

    Error.New("This error was expected")

When expected errors are used with codes then equality and matching is done via the code only:

    Error.New(404, "Page not found");

And this is an exceptional error:

    try
    {
        // This wraps up the exceptional error
    }
    catch(Exception e)
    {
        return Error.New(e);
    }

Finally, you can collect many errors:

    Error.Many(Error.New("error one"), Error.New("error two"));

Or more simply:

    Error.New("error one") + Error.New("error two")

You can extend the set of error types (perhaps for passing through extra data) by creating a new 
record inherits `Excpetional` or `Expected`:   

    public record BespokeError(bool MyData) 
        : Expected("Something bespoke", 100, None); 

By default the properties of the new error-type won't be serialised.  So, if you want to pass a 
payload over the wire, add the `[property: DataMember]` attribute to each member:

    public record BespokeError([property: DataMember] bool MyData) 
        : Expected("Something bespoke", 100, None); 

Using this technique it's trivial to create new error-types when additional data needs to be moved
around, but also there's a ton of built-in functionality for the most common use-cases.