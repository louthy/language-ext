The `Error` type in this module is used extensively with various monadic types, like `Fin<A>`, the _Effect System_ monads 
of `Eff<A>`, `Eff<RT, A>`, `Aff<A>`, `Aff<RT, A>` and the compositional streaming Pipe's feature.  

The reason they're buried in the `Common` namespace, is because, well, `Error` is a common type name.  And so, this gives
the programmer a chance to not include it when they include `LanguageExt.Core`.  

`Error` exists because `Exceptions` are really only meant for _exceptional_ errors. However, in C#-land we've been trained
to throw them even for expected errors.  The `Error` type can be constructed to be exceptional or expected.  For example:

    Error.New("This error was expected")

    try
    {
        // This wraps up the exceptional error
    }
    catch(Exception e)
    {
        return Error.New(e);
    }

`Error` also supports error-codes, and giving an extra textual message for caught exceptions.  