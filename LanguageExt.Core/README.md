## LanguageExt.Core

#### [API Reference](https://louthy.github.io/language-ext/LanguageExt.Core/index.htm)

### Features

This library is quickly becoming a 'Base Class Library' for functional programming in C#.  The features include:

Location | Feature | Description
---------|---------|------------
`Core` | `Arr<A>` | [Immutable array](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Arr_A.htm)
`Core` | `Lst<A>` | [Immutable list](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Lst_A.htm)
`Core` | `Map<K, V>` | [Immutable map](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Map_K_V.htm)
`Core` | `Map<OrdK, K, V>` | [Immutable map with Ord constraint on `K`](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Map_OrdK_K_V.htm)
`Core` | `HashMap<K, V>` | [Immutable hash-map](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/HashMap_K_V.htm)
`Core` | `HashMap<EqK, K, V>` | [Immutable hash-map with Eq constraint on `K`](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/HashMap_EqK_K_V.htm)
`Core` | `Set<A>` | [Immutable set](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Set_A.htm)
`Core` | `Set<OrdA, A>` | [Immutable set with Ord constraint on `A`](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Set_OrdA_A.htm)
`Core` | `HashSet<A>` | [Immutable hash-set](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/HashSet_A.htm)
`Core` | `HashSet<EqA, A>` | [Immutable hash-set with Eq constraint on `A`](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/HashSet_EqA_A.htm)
`Core` | `Que<A>` | [Immutable queue](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Que_T.htm)
`Core` | `Stck<A>` | [Immutable stack](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Stck_T.htm)
`Core` | `Option<A>` | [Option monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Option_A.htm) that can't be used with `null` values
`Core` | `OptionUnsafe<T>` | [Option monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/OptionUnsafe_A.htm) that can be used with `null` values
`Core` | `Either<L,R>` | [Right/Left choice monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Either_L_R.htm) that won't accept `null` values
`Core` | `EitherUnsafe<L,R>` | [Right/Left choice monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/EitherUnsafe_L_R.htm) that can be used with `null` values
`Core` | `Try<A>` | [Exception handling lazy monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Try_A.htm)
`Core` | `TryAsync<A>` | [Asynchronous exception handling lazy monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/TryAsync_A.htm)
`Core` | `TryOption<A>` | [Option monad with third state](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/TryOption_A.htm) 'Fail' that catches exceptions
`Core` | `TryOptionAsync<A>` | [Asynchronous Option monad with third state](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/TryOptionAsync_A.htm) 'Fail' that catches exceptions
`Core` | `Reader<E, A>` | [Reader monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Reader_Env_A.htm)
`Core` | `Writer<MonoidW, W, T>` | [Writer monad that logs to a `W` constrained to be a Monoid](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Writer_MonoidW_W_A.htm)
`Core` | `State<S, A>` | [State monad](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/State_S_A.htm)
`Core` | `NewType<SELF, A, PRED>` | [Haskell `newtype` equivalent](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/NewType_NEWTYPE_A_PRED.htm) i.e: `class Hours : NewType<Hours, double> { public Hours(double value) : base(value) { } }`.  The resulting type is: equatable, comparable, foldable, a functor, monadic, and iterable
`Core` | `NumType<SELF, NUM, A, PRED>` | [Haskell `newtype` equivalent but for numeric types](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/NumType_NUMTYPE_NUM_A_PRED.htm) i.e: `class Hours : NumType<Hours, TDouble, double> { public Hours(double value) : base(value) { } }`.  The resulting type is: equatable, comparable, foldable, a functor, a monoid, a semigroup, monadic, iterable, and can have basic artithmetic operations performed upon it.
`Core` | `FloatType<SELF, FLOATING, A, PRED>` | [Haskell `newtype` equivalent but for real numeric types](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/FloatType_SELF_FLOATING_A_PRED.htm) i.e: `class Hours : FloatType<Hours, TDouble, double> { public Hours(double value) : base(value) { } }`.  The resulting type is: equatable, comparable, foldable, a functor, a monoid, a semigroup, monadic, iterable, and can have complex artithmetic operations performed upon it.
`Core` | `Nullable<T>` extensions | [Extension methods for `Nullable<T>`](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/NullableExtensions_.htm) that make it into a functor, applicative, foldable, iterable and a monad
`Core` | `Task<T>` extensions | [Extension methods for `Task<T>`](https://louthy.github.io/language-ext/LanguageExt.Core/TaskExtensions_.htm) that make it into a functor, applicative, foldable, iterable and a monad
`Core` | Monad transformers | A higher kinded type (ish)
`Core` | Currying | Translate the evaluation of a function that takes multiple arguments into a sequence of functions, each with a single argument
`Core` | Partial application | the process of fixing a number of arguments to a function, producing another function of smaller arity
`Core` | Memoization | An optimization technique used primarily to speed up programs by storing the results of expensive function calls and returning the cached result when the same inputs occur again
`Core` | [Improved lambda type inference](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/Prelude_.htm#fun<R>) | `var add = fun( (int x, int y) => x + y)`
`Core` | [`IObservable<T>` extensions](https://louthy.github.io/language-ext/LanguageExt.Core/LanguageExt/ObservableExt_.htm)  |
