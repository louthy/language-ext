The `Fin` monad supports either an `Error` or an `A` (success) value.  It is functionally exactly the same as `Either<Error, A>`, it is a 
convenience type to avoid the generics pain of `Either`.  

To construct a `Fin`:

    Fin<int> ma = Pure(123);
    Fin<int> mb = Fail(Error.New("Error!"));
