The `Try` monads are all exception catching monads.  Their alternative value is `Exeption`.  They're useful for wrapping up calls to
non-functional exception-throwing methods and making everything declarative.

**NOTE: These monads have been superseded by the `Eff` and `Aff` monads in the language-ext _Effects System_.  They will continue
to be supported, but no new functionality added**