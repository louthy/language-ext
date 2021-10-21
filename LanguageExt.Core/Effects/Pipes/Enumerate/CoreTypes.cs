using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Pipes
{
    public abstract class Enumerate<RT, UOut, UIn, DIn, DOut, A> : Proxy<RT, UOut, UIn, DIn, DOut, A> where RT : struct, HasCancel<RT>
    {
        internal abstract EnumerateDataType Type { get; }
        internal abstract IEnumerable<Proxy<RT, UOut, UIn, DIn, DOut, A>> MakeEffects();
        internal abstract IAsyncEnumerable<Proxy<RT, UOut, UIn, DIn, DOut, A>> MakeEffectsAsync();
        internal abstract IDisposable Subscribe(
            Action<Proxy<RT, UOut, UIn, DIn, DOut, A>> onNext, 
            Action<Error> onError = null, 
            Action onCompleted = null);
    }

    public class Enumerate<RT, UOut, UIn, DIn, DOut, X, A> : Enumerate<RT, UOut, UIn, DIn, DOut, A> where RT : struct, HasCancel<RT>
    {
        internal readonly EnumerateData<X> Items;
        public readonly Func<X, Proxy<RT, UOut, UIn, DIn, DOut, A>> Next;

        internal Enumerate(EnumerateData<X> items, Func<X, Proxy<RT, UOut, UIn, DIn, DOut, A>> next) =>
            (Items, Next) = (items, next);

        public Enumerate(IEnumerable<X> items, Func<X, Proxy<RT, UOut, UIn, DIn, DOut, A>> next) =>
            (Items, Next) = (new EnumerateEnumerable<X>(items), next);

        public Enumerate(IAsyncEnumerable<X> items, Func<X, Proxy<RT, UOut, UIn, DIn, DOut, A>> next) =>
            (Items, Next) = (new EnumerateAsyncEnumerable<X>(items), next);

        public Enumerate(IObservable<X> items, Func<X, Proxy<RT, UOut, UIn, DIn, DOut, A>> next) =>
            (Items, Next) = (new EnumerateObservable<X>(items), next);

        [Pure]
        internal override EnumerateDataType Type =>
            Items.Type;
        
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> ToProxy() => this;

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Bind<S>(Func<A, Proxy<RT, UOut, UIn, DIn, DOut, S>> f) =>
            new Enumerate<RT, UOut, UIn, DIn, DOut, X, S>(Items, x => Next(x).Bind(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Map<S>(Func<A, S> f) =>
            new Enumerate<RT, UOut, UIn, DIn, DOut, X, S>(Items, x => Next(x).Map(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, A> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> body) =>
            new Enumerate<RT, UOut, UIn, C1, C, X, A>(Items, x => Next(x).For(body));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Action<S>(Proxy<RT, UOut, UIn, DIn, DOut, S> r) =>
            new Enumerate<RT, UOut, UIn, DIn, DOut, X, S>(Items, x => Next(x).Action(r));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, A>> fb1) =>
            new Enumerate<RT, UOutA, AUInA, DIn, DOut, X, A>(Items, c1 => Next(c1).ComposeRight(fb1));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ComposeRight<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs) =>
            new Enumerate<RT, UOutA, AUInA, DIn, DOut, X, A>(Items, c1 => Next(c1).ComposeRight(lhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, A>> rhs) =>
            new Enumerate<RT, UOut, UIn, DInC, DOutC, X, A>(Items, c1 => Next(c1).ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<DOut, Proxy<RT, UOut, UIn, DInC, DOutC, DIn>> rhs) =>
            new Enumerate<RT, UOut, UIn, DInC, DOutC, X, A>(Items, c1 => Next(c1).ComposeLeft(rhs));

        [Pure]
        public override Proxy<RT, DOut, DIn, UIn, UOut, A> Reflect() =>
            new Enumerate<RT, DOut, DIn, UIn, UOut, X, A>(Items, x => Next(x).Reflect());
        
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> Observe() =>
            new M<RT, UOut, UIn, DIn, DOut, A>(Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, A>>.Success(this));

        [Pure]
        internal void Deconstruct(out EnumerateData<X> items, out Func<X, Proxy<RT, UOut, UIn, DIn, DOut, A>> next) =>
            (items, next) = (Items, Next);

        [Pure]
        internal override IEnumerable<Proxy<RT, UOut, UIn, DIn, DOut, A>> MakeEffects()
        {
            if (Type == EnumerateDataType.Enumerable)
            {
                foreach (var item in ((EnumerateEnumerable<X>)Items).Values)
                {
                    yield return Next(item);
                }
            }
        }

        [Pure]
        internal override async IAsyncEnumerable<Proxy<RT, UOut, UIn, DIn, DOut, A>> MakeEffectsAsync()
        {
            if (Type == EnumerateDataType.AsyncEnumerable)
            {
                await foreach (var item in ((EnumerateAsyncEnumerable<X>)Items).Values)
                {
                    yield return Next(item);
                }
            }
        }

        internal override IDisposable Subscribe(
            Action<Proxy<RT, UOut, UIn, DIn, DOut, A>> onNext,
            Action<Error> onError = null,
            Action onCompleted = null) =>
            Type == EnumerateDataType.Observable
                ? ((EnumerateObservable<X>)Items).Values.Subscribe(new Observerable(onCompleted, onError, x => onNext(Next(x))))
                : null;

        class Observerable : IObserver<X>
        {
            readonly Action onCompleted;
            readonly Action<Error> onError;
            readonly Action<X> onNext;
            
            public Observerable(Action onCompleted, Action<Error> onError, Action<X> onNext)
            {
                this.onCompleted = onCompleted;
                this.onError     = onError;
                this.onNext      = onNext;
            }

            public void OnCompleted() =>
                onCompleted?.Invoke();

            public void OnError(Exception error) =>
                onError?.Invoke(error);

            public void OnNext(X value) =>
                onNext?.Invoke(value);
        }
    }
}
