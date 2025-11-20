`Choice<F>` allows for propagation of 'failure' and 'choice' (in some appropriate sense, depending on the type).

`Choice` is a `SemigroupK`, but has a `Choose` method, rather than relying on the `SemigroupK.Combine` method, (which 
now has a default implementation of invoking `Choose`).  That creates a new semantic meaning for `Choose`, which is 
about choice propagation rather than the broader meaning of `Combine`.  It also allows for `Choose` and `Combine` to 
have separate implementations depending on the type.

The way to think about `Choose` and the inherited `SemigroupK.Combine` methods is:
* `Choose` is the failure/choice propagation operator: `|`
* `Combine` is the concatenation/combination/addition operator: `+`

Any type that supports the `Choice` trait should also implement the `|` operator, to enable easy choice/failure 
propagation.  If there is a different implementation of `Combine` (rather than accepting the default), then the type 
should also implement the `+` operator.

`ChoiceLaw` can help you test your implementation:

    choose(Pure(a),   Pure(b))  = Pure(a)
    choose(Fail,      Pure(b))  = Pure(b)
    choose(Pure(a),   Fail)     = Pure(a)
    choose(Fail [1],  Fail [2]) = Fail [2]

It also tests the `Applicative` and `Functor` laws.