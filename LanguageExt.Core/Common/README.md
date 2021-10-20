The `Error` type in this module is used extensively with various monadic types, like `Fin<A>`, the _Effect System_ monads 
of `Eff<A>`, `Eff<RT, A>`, `Aff<A>`, `Aff<RT, A>` and the compositional streaming Pipe's feature.  

The reason they're buried in the `Common` namespace, is because, well, `Error` is a common type name.  And so, this gives
the programmer a chance to not include it when they include `LanguageExt.Core`.  