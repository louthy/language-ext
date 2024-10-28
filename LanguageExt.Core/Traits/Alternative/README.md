`Alternative<F>` allows for propagation of 'failure' and 'choice' (in some appropriate sense, depending on the type),
as well as provision of a unit/identity value (`Empty`).

`Alternative` is a `Choice` and `MonoidK`, which means it has a `Choose` method, a `Combine` method (which defaults to
calling the `Choose` method), and an `Empty` method.  That creates a semantic meaning for `Choose`, which is about 
choice propagation rather than the broader meaning of `SemigroupK.Combine`.  It also allows for `Choose` and `Combine` 
to have separate implementations depending on the type.

The way to think about `Choose` and the inherited `SemigroupK.Combine` methods is:
* `Choose` is the failure/choice propagation operator: `|`
* `Combine` is the concatenation/combination/addition operator: `+`

Any type that supports the `Alternative` trait should also implement the `|` operator, to enable easy choice/failure 
propagation.  If there is a different implementation of `Combine` (rather than accepting the default), then the type 
should also implement the `+` operator.

`AlternativeLaw` can help you test your implementation:

    choose(Pure(a), Pure(b)) = Pure(a)
    choose(Empty(), Pure(b)) = Pure(b)
    choose(Pure(a), Empty()) = Pure(a)
    choose(Empty(), Empty()) = Empty()

It also tests the `Applicative` and `Functor` laws.