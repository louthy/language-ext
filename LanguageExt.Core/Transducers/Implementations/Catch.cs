using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Transducers;

record CatchTransducer1<A>(Func<Error, bool> Match, Transducer<Error, A> Catch) : Transducer<A, A>
{
    public Transducer<A, A> Morphism => 
        this;

    public Reducer<A, S> Transform<S>(Reducer<A, S> reduce) =>
        new MatchReducer<S>(Match, Catch, reduce);

    record MatchReducer<S>(Func<Error, bool> Match, Transducer<Error, A> Catch, Reducer<A, S> Reducer) : Reducer<A, S>
    {
        public override TResult<S> Run(TState state, S stateValue, A value)
        {
            try
            {
                var tr = Reducer.Run(state, stateValue, value);
                while (true)
                {
                    switch (tr)
                    {
                        case TRecursive<S> r:
                            tr = r.Run();
                            break;

                        case TContinue<S> { Value: not null } r:
                            return tr;

                        case TComplete<S> { Value: not null } r:
                            return tr;

                        case TCancelled<S>:
                            return tr;

                        case TFail<S> r when Match(r.Error):
                            return Catch.Transform(Reducer).Run(state, stateValue, r.Error);

                        case TFail<S> r when Match(r.Error):
                            return tr;
 
                        default:
                            return TResult.None<S>();
                    }
                }
            }
            catch (Exception e)
            {
                return TResult.Fail<S>(e);
            }
        }
    }
}

record CatchSumTransducerRT<FromErr, X, A>(Func<X, bool> Match, Transducer<X, Sum<X, A>> Catch) : Transducer<Sum<X, A>, Sum<X, A>>
    where FromErr : struct, HasFromError<FromErr, X>
{
    public Transducer<Sum<X, A>, Sum<X, A>> Morphism => 
        this;

    public Reducer<Sum<X, A>, S> Transform<S>(Reducer<Sum<X, A>, S> reduce) =>
        new MatchReducer<S>(Match, Catch, reduce);

    record MatchReducer<S>(Func<X, bool> Match, Transducer<X, Sum<X, A>> Catch, Reducer<Sum<X, A>, S> Reducer)
        : Reducer<Sum<X, A>, S>
    {
        public override TResult<S> Run(TState state, S stateValue, Sum<X, A> value) => 
            value switch
            {
                SumRight<X, A> r =>
                    TryRun(state, stateValue, r),
                
                SumLeft<X, A> l when Match(l.Value) => 
                    Catch.Transform(Reducer)
                         .Run(state, stateValue, l.Value),

                SumLeft<X, A> l =>
                    TryRun(state, stateValue, l),

                _ => TResult.Complete(stateValue)
            };

        TResult<S> TryRun(TState state, S stateValue, Sum<X, A> value)
        {
            try
            {
                var tr = Reducer.Run(state, stateValue, value);
                while (true)
                {
                    switch (tr)
                    {
                        case TRecursive<S> r:
                            tr = r.Run();
                            break;

                        case TContinue<S> { Value: not null }:
                            return tr;

                        case TComplete<S> { Value: not null }:
                            return tr;

                        case TCancelled<S>:
                            return tr;

                        case TFail<S> r when Match(default(FromErr).FromError(r.Error)):
                            return Catch.Transform(Reducer)
                                        .Run(state, stateValue, default(FromErr).FromError(r.Error));

                        case TFail<S>:
                            return tr;
 
                        default:
                            return TResult.None<S>();
                    }
                }
            }
            catch (Exception e)
            {
                var left = default(FromErr).FromError(e);
                return Match(left)
                    ? Catch.Transform(Reducer).Run(state, stateValue, left)
                    : TResult.Fail<S>(e);
            }
        }
    }
}
