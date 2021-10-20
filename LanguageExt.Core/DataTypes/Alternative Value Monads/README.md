_Alternative Value Monads_ are all monadic types that can have an alternative value!  What does this mean?  

Well, it's best to start with `Either<L, R>` (Left and Right).  If first we think about a tuple:

    (int, string)

This type can represent an `int` _AND_ a `string`.  `Either` is it's natural 'dual', it can be an `int` _OR_ a `string`:

    Either<int, string>

In the case of `Either` the _Right_ value is considered the _bound_ value of the monad, and the _Left_ value is considered 
the _alternative_ value.  All of the other _alternative value monads_ can be seen as derivatives of `Either`.

| Type | Bound Value Type | Alternative Value Type
| ---- | ---- | ---- |
| `Option<A>`                 | `A` | `None`
| `Fin<A>`                    | `A` | `Error`
| `Fin<A>`                    | `A` | `Error`
| `Validation<F, S>`          | `S` | `Seq<F>`
| `Validation<MonoidF, F, S>` | `S` | `F`
| `Nullable<A>`               | `A` | `null`
| `Some<A>`                   | `A` | `ValueIsNoneException`

_The alternative value is usually used to carry errors, but that doesn't have to be the case_