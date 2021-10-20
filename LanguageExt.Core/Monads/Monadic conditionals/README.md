`guard`, `when`, and `unless` allow for conditional operations and short-cutting in monadic expressions.

## Guards

Guards are used to stop the monadic expression continuing if a flag is `true` (for `guard`) or `false` (for `guardnot`).  

They only work with monads that have an _'alternative value'_ (which is usually used as the error condition: `Left` in 
`Either` for example).  An alternative value is provided when the guard triggers:  

    from x in ma
    from _ in guard(x == 100, Error.New("x should be 100"))
    select x;

Supported monads are:

    Either
    EitherUnsafe
    EitherAsync
    Fin
    Validation
    Aff
    Eff

## When and Unless

`when` and `unless` are similar to guards, but instead of providing _the_ alternative value, you provide an alternative monad 
to run.  This monad could be in a failed state, or it could run a successful _side effect_ (an `Aff` calling `Console<RT>.writeLine()` 
for example).

    from x in ma
    from _ in when(x == 100, Console.writeLine<RT>("x is 100, finally!"))
    select x;
