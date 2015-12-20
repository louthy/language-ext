## LanguageExt.Core

Feature | Description
--------|------------
`Lst<T>` | [Immutable list](https://github.com/louthy/language-ext/wiki/List-reference)
`Map<K,V>` | Immutable map
`Set<T>` | Immutable set
`Que<T>` | Immutable queue
`Stck<T>` | Immutable stack
`Option<T>` | Option monad that can't be used with `null` values
`OptionUnsafe<T>` | Option monad that can be used with `null` values
`Either<L,R>` | Right/Left choice monad that won't accept `null` values
`EitherUnsafe<L,R>` | Right/Left choice monad that can be used with `null` values
`Try<T>` | Exception catching monad
`TryOption<T>` | Option monad with third state 'Fail' that catches exceptions
`Reader<E,T>` | Reader monad
`Writer<O,T>` | Writer monad
`State<S,T>` | State monad
`Rws<E,O,S,T>` | Reader/Writer/State monad
Monad transformers | A higher kinded type (ish)
Currying | https://en.wikipedia.org/wiki/Currying
Partial application | https://en.wikipedia.org/wiki/Partial_application
Memoization | https://en.wikipedia.org/wiki/Memoization
Improved lambda type inference | `var add = fun( (int x, int y) => x + y)`
`IObservable<T>` extensions  | 
