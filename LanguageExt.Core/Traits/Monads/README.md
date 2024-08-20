### Why do we even need monads?

* Monads _encapsulate impure side effects and other effects_, making them pure.
* Monads allow sequencing of operations without resorting to statements.
* Monads (with LINQ) allow us to write _pure_ code that looks a lot like statements in imperative code.
* Monads de-clutter your code making it more declarative. 

> **Quite simply**: _Monads are the 'statements' of pure functional programming and they encapsulate messy boilerplate and side-effects._

For a deeper dive into the 'why?' of monads, then check out [Paul Louth's Higher-Kinds series](https://paullouth.com/higher-kinds-in-csharp-with-language-ext-part-7-monads/).  

* [Monad](Monad) is the home of the main `Monad<M>` trait as well as its extension methods and `Monad` module type.
* [MonadIO](MonadIO) has the IO variant of the monad that allows lifting of the `IO<A>` monad into a monad-transformer stack.
* [MonadT](MonadT) has the `MonadT<T, M>` monad-transformer trait, it allows the lifting of a monad `M` into a monad-transformer `T`. 