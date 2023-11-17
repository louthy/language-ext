#nullable enable
using System.Threading;

namespace LanguageExt.Transducers;

record PostTransducer<A, B>(Transducer<A, B> F) : Transducer<A, B>
{
    public Transducer<A, B> Morphism =>
        this;

    public Reducer<A, S> Transform<S>(Reducer<B, S> reduce) =>
        Reducer.from<A, S>((st, s, x) =>
        {
            B? value = default; 
            using var wait = new AutoResetEvent(false);
            
            st.SynchronizationContext.Post(_ =>
                F.Invoke(
                    x, 
                    s, 
                    Reducer.from<B, S>((_, s1, v) =>
                    {
                        value = v;
                        return TResult.Complete(s1);
                    }),
                    () => wait.Set(),
                    st.Token, 
                    st.SynchronizationContext),
                null);

            wait.WaitOne();

            return value is not null 
                ? reduce.Run(st, s, value)
                : TResult.None<S>();
        });
}
