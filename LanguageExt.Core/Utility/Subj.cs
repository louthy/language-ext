using System;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace LanguageExt
{
    /// <summary>
    /// Equivalent of Subject in RX.NET
    /// </summary>
    /// <remarks>Means we don't need the dependency on System.Reactive</remarks>
    /// <typeparam name="A"></typeparam>
    internal class Subj<A> : IObservable<A>, IDisposable
    {
        readonly Atom<Seq<IObserver<A>>> subs = Atom(Seq<IObserver<A>>());

        public IDisposable Subscribe(IObserver<A> observer)
        {
            observer = observer ?? throw new ArgumentNullException(nameof(observer));
            subs.Swap(s => s.Add(observer));
            return new Disp(observer, this);
        }

        public Unit OnNext(A value) =>
            subs.Value.Iter(s => Task.Run(() => s.OnNext(value)));

        public Unit OnError(Exception value) =>
            ignore(subs.Swap(ss => 
                             { 
                                 ss.Iter(s => Task.Run(() => s.OnError(value)));
                                 return Empty;
                             }));

        public Unit OnCompleted() =>
            ignore(subs.Swap(ss =>
                             {
                                 ss.Iter(s => Task.Run(s.OnCompleted));
                                 return Empty;
                             }));

        public void Dispose() =>
            OnCompleted();

        Unit Unsubscribe(IObserver<A> obs) =>
            ignore(subs.Swap(ss => ss.Filter(s => !ReferenceEquals(obs, s)).Strict()));

        class Disp : IDisposable
        {
            readonly IObserver<A> Obs;
            readonly Subj<A> Subj;
            
            public Disp(IObserver<A> obs, Subj<A> subj)
            {
                Obs  = obs;
                Subj = subj;
            }

            public void Dispose()
            {
                Obs.OnCompleted();
                Subj.Unsubscribe(Obs);
            }
        }
    }
}
