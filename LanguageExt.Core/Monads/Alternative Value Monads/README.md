_Alternative Value Monads_ are monadic types that can have an alternative value!  What does this mean?  

If first we think about a tuple:

    (int, string)

This type can represent an `int` _AND_ a `string`.  Now consider the `Either<L, R>` monad, this means the value 
can be`Left` _OR_ `Right`  (`L` or `R`).  

So, this:

    Either<int, string>

Means either `int` _OR_ `string`. It is the natural _dual_ of tuple.

In the case of `Either` the _Right_ value is considered the _bound_ value of the monad, and the _Left_ value 
is considered the _alternative_ value.  All of the other _alternative value monads_ can be seen as derivatives 
or specialisations of `Either`.

| Type                     | Alternative Value Type | Bound Value Type |
|--------------------------|------------------------|------------------|
| `Either<L, R>`           | `L`                    | `R`              |
| `Option<A>`              | `None`                 | `A`              |
| `Fin<A>`                 | `Error`                | `A`              |
| `Validation<Fail, Succ>` | `Seq<Fail>`            | `Succ`           |
| `Nullable<A>`            | `null`                 | `A`              |

> _The alternative value is usually used to carry errors, but that doesn't have to be the case. It is 
> important to remember that the alternative-value can carry anything you want._