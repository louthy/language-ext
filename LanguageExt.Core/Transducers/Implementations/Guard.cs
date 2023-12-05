namespace LanguageExt;

record GuardTransducer<E, A> : Transducer<Guard<E, A>, Sum<E, Unit>>
{
    public static readonly Transducer<Guard<E, A>, Sum<E, Unit>> Default = new GuardTransducer<E, A>();
    
    public override Reducer<Guard<E, A>, S> Transform<S>(Reducer<Sum<E, Unit>, S> reduce) =>
        Reducer.from<Guard<E, A>, S>(
            (st, s, guard) =>
                guard.Flag
                    ? reduce.Run(st, s, Sum<E, Unit>.Right(default))
                    : reduce.Run(st, s, Sum<E, Unit>.Left(guard.OnFalse())));
}
