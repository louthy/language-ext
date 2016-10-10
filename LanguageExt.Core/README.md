## LanguageExt.Core

#### [API Reference](https://louthy.github.io)

Nu-get package | Description
---------------|-------------
[LanguageExt.Core](https://www.nuget.org/packages/LanguageExt.Core) | All of the core types and functional 'prelude'.  This is all that's needed to get started.

Feature | Description
---------|------------
`Lst<T>` | [Immutable list](https://louthy.github.io/languageext.core/LanguageExt/Lst_T.htm)
`Map<K,V>` | [Immutable map](https://louthy.github.io/languageext.core/LanguageExt/Map_K_V.htm)
`Set<T>` | [Immutable set](https://louthy.github.io/languageext.core/LanguageExt/Set_T.htm)
`Que<T>` | [Immutable queue](https://louthy.github.io/languageext.core/LanguageExt/Que_T.htm)
`Stck<T>` | [Immutable stack](https://louthy.github.io/languageext.core/LanguageExt/Stck_T.htm)
`Option<T>` | [Option monad](https://louthy.github.io/languageext.core/LanguageExt/Option_T.htm) that can't be used with `null` values
`OptionUnsafe<T>` | [Option monad](https://louthy.github.io/languageext.core/LanguageExt/OptionUnsafe_T.htm) that can be used with `null` values
`Either<L,R>` | [Right/Left choice monad](https://louthy.github.io/languageext.core/LanguageExt/Either_L_R.htm) that won't accept `null` values
`EitherUnsafe<L,R>` | [Right/Left choice monad](https://louthy.github.io/languageext.core/LanguageExt/EitherUnsafe_L_R.htm) that can be used with `null` values
`Try<T>` | [Exception handling lazy monad](https://louthy.github.io/languageext.core/LanguageExt/index.htm#Try_T)
`TryOption<T>` | [Option monad with third state](https://louthy.github.io/languageext.core/LanguageExt/index.htm#TryOption_T) 'Fail' that catches exceptions
`Reader<E,T>` | [Reader monad](https://louthy.github.io/languageext.core/LanguageExt/index.htm#Reader_Env_T)
`Writer<O,T>` | [Writer monad](https://louthy.github.io/languageext.core/LanguageExt/index.htm#Writer_Out_T)
`State<S,T>` | [State monad](https://louthy.github.io/languageext.core/LanguageExt/index.htm#State_S_T)
`Rws<E,O,S,T>` | [Reader/Writer/State monad](https://louthy.github.io/languageext.core/LanguageExt/index.htm#Rws_R_W_S_T)
`NewType<T>` | [Haskell `newtype` equivalent](https://louthy.github.io/languageext.core/LanguageExt/NewType_T.htm) i.e: `class Hours : NewType<double> { public Hours(double value) : base(value) { } }`.  The resulting type is: equatable, comparable, foldable, a functor, monadic, appendable, subtractable, divisible, multiplicable, and iterable
`Nullable<T>` extensions | [Extension methods for `Nullable<T>`](https://louthy.github.io/languageext.core/LanguageExt/NullableExtensions_.htm) that make it into a functor, applicative, foldable, iterable and a monad
`Task<T>` extensions | [Extension methods for `Task<T>`](https://louthy.github.io/languageext.core/LanguageExt/__TaskExt_.htm) that make it into a functor, applicative, foldable, iterable and a monad
Monad transformers | A higher kinded type (ish)
Currying | [Translate the evaluation of a function that takes multiple arguments into a sequence of functions, each with a single argument](https://louthy.github.io/languageext.core/LanguageExt/Prelude_.htm#curry<T1, T2, R>)
Partial application | [the process of fixing a number of arguments to a function, producing another function of smaller arity](https://louthy.github.io/languageext.core/LanguageExt/Prelude_.htm#par<T1, T2, R>)
Memoization | [An optimization technique used primarily to speed up programs by storing the results of expensive function calls and returning the cached result when the same inputs occur again](https://louthy.github.io/languageext.core/LanguageExt/Prelude_.htm#memo<T, R>)
[Improved lambda type inference](https://louthy.github.io/languageext.core/LanguageExt/Prelude_.htm#fun) | `var add = fun( (int x, int y) => x + y)`
`Core` | `IObservable<T>` extensions  |

