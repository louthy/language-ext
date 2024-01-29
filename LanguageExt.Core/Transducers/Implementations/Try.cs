using System;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt;

record TryTransducer<A, B>(
        Transducer<A, B> F, 
        Func<Error, bool> Match, 
        Transducer<Error, B> Catch) 
    : Transducer<A, B>
{
    public override Reducer<A, S> Transform<S>(Reducer<B, S> reduce) =>
        new MatchReducer<S>(F, Match, Catch, reduce);

    record MatchReducer<S>(
            Transducer<A, B> F,
            Func<Error, bool> Match, 
            Transducer<Error, B> Catch, 
            Reducer<B, S> Reducer)
        : Reducer<A, S>
    {
        public override TResult<S> Run(TState state, S stateValue, A value)
        {
            var tr = TryTransform(state, value);
            while(!state.Token.IsCancellationRequested)
            {
                switch (tr)
                {
                    case TRecursive<Option<B>> r:
                        try
                        {
                            tr = r.Run();
                        }
                        catch (Exception e)
                        {
                            tr = TResult.Fail<Option<B>>(e);
                        }
                        break;
                    
                    case TContinue<Option<B>> r when r.Value.IsSome:
                        return Reducer.Run(state, stateValue, (B)r.Value);

                    case TComplete<Option<B>>:
                        return TResult.Complete(stateValue);
                    
                    case TFail<Option<B>> r:
                        return Match(r.Error)
                            ? Catch.Transform(Reducer).Run(state, stateValue, r.Error)
                            : TResult.Fail<S>(r.Error);
                    
                    case TCancelled<Option<B>>:
                        return TResult.Cancel<S>();
                    
                    case TNone<Option<B>>:
                        return TResult.None<S>();
                }
            }
            return TResult.Cancel<S>();
        }
        
        TResult<Option<B>> TryTransform(TState state, A value)
        {
            try
            {
                var tf = F.Transform(Reducer<B>.optionIdentity);
                return tf.Run(state, Option<B>.None, value);
            }
            catch(Exception e)
            {
                return TResult.Fail<Option<B>>(e);
            }
        }
    }
                            
    public override string ToString() =>  
        $"try";
}

record TryTransducer<RT, X, A>(
        Transducer<RT, Sum<X, A>> F, 
        Func<X, bool> Match, 
        Transducer<X, Sum<X, A>> Catch) 
    : Transducer<RT, Sum<X, A>>
    where RT : HasFromError<RT, X>
{
    public override Reducer<RT, S> Transform<S>(Reducer<Sum<X, A>, S> reduce) =>
        new MatchReducer<S>(F, Match, Catch, reduce);

    record MatchReducer<S>(
            Transducer<RT, Sum<X, A>> F,
            Func<X, bool> Match, 
            Transducer<X, Sum<X, A>> Catch, 
            Reducer<Sum<X, A>, S> Reducer)
        : Reducer<RT, S>
    {
        public override TResult<S> Run(TState state, S stateValue, RT value)
        {
            var tr = TryTransform(state, value);
            while(!state.Token.IsCancellationRequested)
            {
                switch (tr)
                {
                    case TRecursive<Option<Sum<X, A>>> r:
                        try
                        {
                            tr = r.Run();
                        }
                        catch (Exception e)
                        {
                            tr = TResult.Continue(Prelude.Some(Sum<X, A>.Left(RT.FromError(e))));
                        }
                        break;
                    
                    case TContinue<Option<Sum<X, A>>> r when r.Value.IsSome:
                        return (Sum<X, A>)r.Value switch
                        {
                            SumLeft<X, A> sl when Match(sl.Value) =>
                                Catch.Transform(Reducer).Run(state, stateValue, sl.Value),
                            
                            var sum =>
                                Reducer.Run(state, stateValue, sum)
                        };

                    case TComplete<Option<Sum<X, A>>>:
                        return TResult.Complete(stateValue);
                    
                    case TFail<Option<Sum<X, A>>> r:
                        tr = TResult.Continue(Prelude.Some(Sum<X, A>.Left(RT.FromError(r.Error))));
                        break;
                    
                    case TCancelled<Option<Sum<X, A>>>:
                        return TResult.Cancel<S>();
                    
                    case TNone<Option<Sum<X, A>>>:
                        return TResult.None<S>();
                }
            }
            return TResult.Cancel<S>();
        }
        
        TResult<Option<Sum<X, A>>> TryTransform(TState state, RT value)
        {
            try
            {
                var tf = F.Transform(Reducer<Sum<X, A>>.optionIdentity);
                return tf.Run(state, Option<Sum<X, A>>.None, value);
            }
            catch(Exception e)
            {
                return TResult.Fail<Option<Sum<X, A>>>(e);
            }
        }
    }
                            
    public override string ToString() =>  
        $"try";
}
