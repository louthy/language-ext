using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using LanguageExt.Common;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Pipes
{
   
    internal abstract class Enumerate<RT, UOut, UIn, DIn, DOut, A> : Proxy<RT, UOut, UIn, DIn, DOut, A>
        where RT : struct, HasCancel<RT>
    {
        public readonly Func<Unit, Proxy<RT, UOut, UIn, DIn, DOut, A>> Next;
        
        internal abstract EnumerateDataType Type { get; }
        internal abstract IEnumerable<Proxy<RT, UOut, UIn, DIn, DOut, Unit>> MakeEffects();
        internal abstract IAsyncEnumerable<Proxy<RT, UOut, UIn, DIn, DOut, Unit>> MakeEffectsAsync();

        public Enumerate(Func<Unit, Proxy<RT, UOut, UIn, DIn, DOut, A>> next) =>
            Next = next;
        
        internal abstract IDisposable Subscribe(
            Action<Proxy<RT, UOut, UIn, DIn, DOut, Unit>> onNext,
            Action<Error> onError = null,
            Action onCompleted = null);
    }

    internal class Enumerate<RT, UOut, UIn, DIn, DOut, X, A> : Enumerate<RT, UOut, UIn, DIn, DOut, A>
        where RT : struct, HasCancel<RT>
    {
        internal readonly EnumerateData<X> Items;
        internal readonly Func<X, Proxy<RT, UOut, UIn, DIn, DOut, Unit>> Yield;

        internal Enumerate(
            EnumerateData<X> items, 
            Func<X, Proxy<RT, UOut, UIn, DIn, DOut, Unit>> yield,
            Func<Unit, Proxy<RT, UOut, UIn, DIn, DOut, A>> next) : base(next) =>
            (Items, Yield) = (items, yield);

        [Pure]
        internal override EnumerateDataType Type =>
            Items.Type;

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> ToProxy() => this;

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Bind<S>(Func<A, Proxy<RT, UOut, UIn, DIn, DOut, S>> f) =>
            new Enumerate<RT, UOut, UIn, DIn, DOut, X, S>(
                Items,
                Yield,
                x => Next(x).Bind(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Map<S>(Func<A, S> f) =>
            new Enumerate<RT, UOut, UIn, DIn, DOut, X, S>(
                Items,
                Yield,
                x => Next(x).Map(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, A> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> body) =>
            ReplaceRespond(body);

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Action<S>(Proxy<RT, UOut, UIn, DIn, DOut, S> r) =>
            new Enumerate<RT, UOut, UIn, DIn, DOut, X, S>(
                Items, 
                Yield,
                x => Next(x).Action(r));

        /// <remarks>
        /// (f +>> p) pairs each 'request' in `this` with a 'respond' in `lhs`.
        /// </remarks>
        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> PairEachRequestWithRespond<UOutA, AUInA>(
            Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, A>> fb1) =>
            new Enumerate<RT, UOutA, AUInA, DIn, DOut, X, A>(
                Items,
                c1 => Yield(c1).PairEachRequestWithRespond(x => fb1(x).Map(_ => Prelude.unit)),
                c1 => Next(c1).PairEachRequestWithRespond(fb1));

        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ReplaceRequest<UOutA, AUInA>(
            Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs) =>
            new Enumerate<RT, UOutA, AUInA, DIn, DOut, X, A>(
                Items,
                c1 => Yield(c1).ReplaceRequest(lhs),                
                c1 => Next(c1).ReplaceRequest(lhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> PairEachRespondWithRequest<DInC, DOutC>(
            Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, A>> rhs) =>
            new Enumerate<RT, UOut, UIn, DInC, DOutC, X, A>(
                Items,
                c1 => Yield(c1).PairEachRespondWithRequest(x => rhs(x).Map(_ => Prelude.unit)),
                c1 => Next(c1).PairEachRespondWithRequest(rhs));

        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ReplaceRespond<DInC, DOutC>(
            Func<DOut, Proxy<RT, UOut, UIn, DInC, DOutC, DIn>> rhs) =>
            new Enumerate<RT, UOut, UIn, DInC, DOutC, X, A>(
                Items,
                c1 => Yield(c1).ReplaceRespond(rhs),
                c1 => Next(c1).ReplaceRespond(rhs));

        [Pure]
        public override Proxy<RT, DOut, DIn, UIn, UOut, A> Reflect() =>
            new Enumerate<RT, DOut, DIn, UIn, UOut, X, A>(
                Items, 
                x => Yield(x).Reflect(),
                x => Next(x).Reflect());

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> Observe() =>
            new M<RT, UOut, UIn, DIn, DOut, A>(Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, A>>.Success(this));

        [Pure]
        internal void Deconstruct(
            out EnumerateData<X> items,
            out Func<X, Proxy<RT, UOut, UIn, DIn, DOut, Unit>> yield,
            out Func<Unit, Proxy<RT, UOut, UIn, DIn, DOut, A>> next) =>
            (items, yield, next) = (Items, Yield, Next);

        [Pure]
        internal override IEnumerable<Proxy<RT, UOut, UIn, DIn, DOut, Unit>> MakeEffects()
        {
            if (Type == EnumerateDataType.Enumerable)
            {
                foreach (var item in ((EnumerateEnumerable<X>)Items).Values)
                {
                    yield return Yield(item);
                }
            }
        }

        [Pure]
        internal override async IAsyncEnumerable<Proxy<RT, UOut, UIn, DIn, DOut, Unit>> MakeEffectsAsync()
        {
            if (Type == EnumerateDataType.AsyncEnumerable)
            {
                await foreach (var item in ((EnumerateAsyncEnumerable<X>)Items).Values)
                {
                    yield return Yield(item);
                }
            }
        }

        internal override IDisposable Subscribe(
            Action<Proxy<RT, UOut, UIn, DIn, DOut, Unit>> onNext,
            Action<Error> onError = null,
            Action onCompleted = null) =>
            Type == EnumerateDataType.Observable
                ? ((EnumerateObservable<X>)Items).Values.Subscribe(new Observerable(onCompleted, onError,
                    x => onNext(Yield(x))))
                : null;

        class Observerable : IObserver<X>
        {
            readonly Action onCompleted;
            readonly Action<Error> onError;
            readonly Action<X> onNext;

            public Observerable(Action onCompleted, Action<Error> onError, Action<X> onNext)
            {
                this.onCompleted = onCompleted;
                this.onError = onError;
                this.onNext = onNext;
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
