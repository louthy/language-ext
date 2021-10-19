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
    /// <summary>
    /// Effects represent a 'fused' set of producer, pipes, and consumer into one type
    /// It neither yields nor awaits, but represents an entire effect system
    /// </summary>
    public class Effect<RT, A> : Proxy<RT, Void, Unit, Unit, Void, A> where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, Void, Unit, Unit, Void, A> Value;

        public Effect(Proxy<RT, Void, Unit, Unit, Void, A> value) =>
            Value = value;
        
        [Pure]
        public override Proxy<RT, Void, Unit, Unit, Void, A> ToProxy() =>
            Value.ToProxy();
        [Pure]

        public override Proxy<RT, Void, Unit, Unit, Void, S> Bind<S>(Func<A, Proxy<RT, Void, Unit, Unit, Void, S>> f) =>
            Value.Bind(f);
            
        [Pure]
        public override Proxy<RT, Void, Unit, Unit, Void, S> Map<S>(Func<A, S> f) =>
            Value.Map(f);

        [Pure]
        public override Proxy<RT, Void, Unit, C1, C, A> For<C1, C>(Func<Void, Proxy<RT, Void, Unit, C1, C, Unit>> f) =>
            Value.For(f);

        [Pure]
        public override Proxy<RT, Void, Unit, Unit, Void, S> Action<S>(Proxy<RT, Void, Unit, Unit, Void, S> r) =>
            Value.Action(r);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, Void, A> ComposeRight<UOutA, AUInA>(Func<Void, Proxy<RT, UOutA, AUInA, Void, Unit, A>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, Void, A> ComposeRight<UOutA, AUInA>(Func<Void, Proxy<RT, UOutA, AUInA, Unit, Void, Unit>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, Void, Unit, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<Void, Proxy<RT, Unit, Void, DInC, DOutC, A>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, Void, Unit, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<Void, Proxy<RT, Void, Unit, DInC, DOutC, Unit>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, Void, Unit, Unit, Void, A> Reflect() =>
            Value.Reflect();

        [Pure]
        public override Proxy<RT, Void, Unit, Unit, Void, A> Observe() =>
            Value.Observe();

        [Pure]
        public void Deconstruct(out Proxy<RT, Void, Unit, Unit, Void, A> value) =>
            value = Value;

        [Pure]
        public Aff<RT, C> SelectMany<B, C>(Func<A, Aff<RT, B>> bind, Func<A, B, C> project) =>
            this.RunEffect().Bind(a => bind(a).Map(b => project(a, b)));

        [Pure]
        public Aff<RT, C> SelectMany<B, C>(Func<A, Aff<B>> bind, Func<A, B, C> project) =>
            this.RunEffect().Bind(a => bind(a).Map(b => project(a, b)));

        [Pure]
        public Aff<RT, C> SelectMany<B, C>(Func<A, Eff<RT, B>> bind, Func<A, B, C> project) =>
            this.RunEffect().Bind(a => bind(a).Map(b => project(a, b)));

        [Pure]
        public Aff<RT, C> SelectMany<B, C>(Func<A, Eff<B>> bind, Func<A, B, C> project) =>
            this.RunEffect().Bind(a => bind(a).Map(b => project(a, b)));
    }
}
