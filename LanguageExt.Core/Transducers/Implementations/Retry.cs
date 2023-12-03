#nullable enable
using System;
using System.Threading;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Transducers;

record RetryTransducer<A, B>(Transducer<A, B> F, Schedule Schedule, Func<Error, bool> Predicate) : Transducer<A, B>
{
    public override Reducer<A, S> Transform<S>(Reducer<B, S> reduce) => 
        new Reduce1<S>(F, Schedule, Predicate, reduce);
    
    record Reduce1<S>(Transducer<A, B> F, Schedule Schedule, Func<Error, bool> Predicate, Reducer<B, S> Reduce) 
        : Reducer<A, S>
    {
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
        
        public override TResult<S> Run(TState state, S stateValue, A value)
        {
            var tr = TryTransform(state, value);
            using var stepEnum = Schedule.Run().GetEnumerator();
            using var wait = new AutoResetEvent(false);

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
                        return Reduce.Run(state, stateValue, (B)r.Value);

                    case TComplete<Option<B>> r when r.Value.IsSome:
                        return TResult.Complete(stateValue);

                    case TCancelled<Option<B>>:
                        return TResult.Cancel<S>();

                    case TFail<Option<B>> r:
                        if (Predicate(r.Error))
                        {
                            return TResult.Fail<S>(r.Error);
                        }
                        else
                        {
                            if (stepEnum.MoveNext())
                            {
                                if (stepEnum.Current != 0)
                                {
                                    wait.WaitOne((TimeSpan)stepEnum.Current);
                                }
                                tr = TryTransform(state, value);
                            }
                            else
                            {
                                return TResult.Fail<S>(r.Error);
                            }
                        }
                        break;

                    default:
                        return TResult.None<S>();
                }
            }
            return TResult.Cancel<S>();
        }
    }
            
    public override string ToString() =>  
        "retry";
}

record RetrySumTransducer<RT, X, A>(Transducer<RT, Sum<X, A>> F, Schedule Schedule, Func<X, bool> Predicate) 
    : Transducer<RT, Sum<X, A>>
    where RT : struct, HasFromError<RT, X> 
{
    public override Reducer<RT, S> Transform<S>(Reducer<Sum<X, A>, S> reduce) => 
        new Reduce1<S>(F, Schedule, Predicate, reduce);
    
    record Reduce1<S>(Transducer<RT, Sum<X, A>> F, Schedule Schedule, Func<X, bool> Predicate, Reducer<Sum<X, A>, S> Reduce) 
        : Reducer<RT, S>
    {
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
        
        public override TResult<S> Run(TState state, S stateValue, RT value)
        {
            var tr = TryTransform(state, value);
            using var stepEnum = Schedule.Run().GetEnumerator();
            using var wait = new AutoResetEvent(false);

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
                            tr = TResult.Fail<Option<Sum<X, A>>>(e);
                        }
                        break;

                    case TContinue<Option<Sum<X, A>>> r when r.Value.IsSome:
                        switch((Sum<X, A>)r.Value)
                        {
                            case SumRight<X, A> sr:
                                return Reduce.Run(state, stateValue, sr);
                            
                            case SumLeft<X, A> sl:
                                if (Predicate(sl.Value))
                                {
                                    return Reduce.Run(state, stateValue, sl);
                                }
                                else
                                {
                                    if (stepEnum.MoveNext())
                                    {
                                        if (stepEnum.Current != 0)
                                        {
                                            wait.WaitOne((TimeSpan)stepEnum.Current);
                                        }
                                        tr = TryTransform(state, value);
                                    }
                                    else
                                    {
                                        return Reduce.Run(state, stateValue, sl);
                                    }
                                }
                                break;
                        }
                        break;

                    case TComplete<Option<Sum<X, A>>> r:
                        return TResult.Complete(stateValue);

                    case TCancelled<Option<Sum<X, A>>>:
                        return TResult.Cancel<S>();

                    case TFail<Option<Sum<X, A>>> r:
                        tr = TResult.Continue(Prelude.Some(Sum<X, A>.Left(default(RT).FromError(r.Error))));
                        break;

                    default:
                        return TResult.None<S>();
                }
            }
            return TResult.Cancel<S>();
        }
    }
            
    public override string ToString() =>  
        "retry";
}
