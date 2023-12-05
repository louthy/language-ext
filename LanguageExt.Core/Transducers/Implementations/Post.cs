#nullable enable
using System;
using System.Threading;

namespace LanguageExt;

record PostTransducer<A, B>(Transducer<A, B> F) : Transducer<A, B>
{
    public override Reducer<A, S> Transform<S>(Reducer<B, S> reduce) =>
        new Reduce<S>(F, reduce);
    
    record Reduce<S>(Transducer<A, B> F, Reducer<B, S> Reducer) : Reducer<A, S>
    {
        public override TResult<S> Run(TState st, S s, A x)
        {
            var value = TResult.None<B>(); 
            using var wait = new AutoResetEvent(false);

            try
            {
                st.SynchronizationContext.Post(
                    _ =>
                    {
                        try
                        {
                            var r = F.Invoke(
                                x,
                                s,
                                new FReducer<B, S>((_, s1, v) =>
                                {
                                    value = TResult.Continue(v);
                                    return TResult.Continue(s1);
                                }),
                                st.Token,
                                st.SynchronizationContext);

                            if (r.Faulted)
                            {
                                // Bit of a hacky way to cast the result, but we know that the 
                                // result is faulted, so the map function never gets invoked.
                                value = r.Map<B>(_ => throw new NotSupportedException());
                            }
                        }
                        catch (Exception e)
                        {
                            value = TResult.Fail<B>(e);
                        }
                        finally
                        {
                            wait.Set();
                        }
                    },
                    null);
                
                wait.WaitOne();
            }
            catch (Exception e)
            {
                value = TResult.Fail<B>(e);
            }

            return value.Reduce(st, s, Reducer);
        }
    }
}
