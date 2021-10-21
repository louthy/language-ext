If you're new to this library, you may need a few pointers of where to look for features:

  * [`Prelude`](Prelude) is a 
    `static partial class`, this type is loaded with functions for constructing the key data types, as well 
    as many of the things you'd expect in a functional programming language's prelude.  Note, the `Prelude` type
    extends into many other parts of the source-tree.  It's the same type, but spread all over the code-base.
    And so, you may see `Prelude` in other areas of the documentation: it's the same type.
    
    Because it's so fundamental, you'll want to add this to the top of every code file:

        using static LanguageExt.Prelude;

    This makes all of the functions in the `Prelude` available as though they were local.         
  * [`Monads`](Monads) contains the common monads like `Option<A>` and `Either<L, R>`, as well as state-managing monads like `Reader`, `Writer`, and `State`.
  * [`Immutable Collections`](Immutable%20Collections) contains the high-performance functional collection types this library is famous for.
  * [`Effects`](Effects) is where the pure IO functionality of language-ext resides.  It is also where you'll find 
    the `Pipes` compositional streaming functionality.  To understand more about how to deal with side-effects, 
    [check the wiki](https://github.com/louthy/language-ext/wiki/How-to-deal-with-side-effects).
  * [`Concurrency`](Concurrency) is where you'll find lots of help in atomically managing shared data without locks. 
  
  
