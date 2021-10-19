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
    /// Clients only request and never respond
    /// </summary>
    public class Client<RT, REQ, RES, A> : Proxy<RT, REQ, RES, Unit, Void, A> where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, REQ, RES, Unit, Void, A> Value;

        public Client(Proxy<RT, REQ, RES, Unit, Void, A> value) =>
            Value = value;
 
        [Pure]
        public override Proxy<RT, REQ, RES, Unit, Void, A> ToProxy() =>
            Value.ToProxy();

        [Pure]
        public override Proxy<RT, REQ, RES, Unit, Void, S> Bind<S>(Func<A, Proxy<RT, REQ, RES, Unit, Void, S>> f) =>
            Value.Bind(f);
            
        [Pure]
        public override Proxy<RT, REQ, RES, Unit, Void, S> Map<S>(Func<A, S> f) =>
            Value.Map(f);
        
        [Pure]
        public Client<RT, REQ, RES, B> Bind<B>(Func<A, Client<RT, REQ, RES, B>> f) => 
            Value.Bind(f).ToClient();
        
        [Pure]
        public Client<RT, REQ, RES, B> SelectMany<B>(Func<A, Client<RT, REQ, RES, B>> f) => 
            Value.Bind(f).ToClient();
        
        [Pure]
        public Client<RT, REQ, RES, C> SelectMany<B, C>(Func<A, Client<RT, REQ, RES, B>> f, Func<A, B, C> project) => 
            Value.Bind(a => f(a).Map(b => project(a, b))).ToClient();
        
        [Pure]
        public new Client<RT, REQ, RES, B> Select<B>(Func<A, B> f) => 
            Value.Map(f).ToClient();        
        
        [Pure]
        public override Proxy<RT, REQ, RES, C1, C, A> For<C1, C>(Func<Void, Proxy<RT, REQ, RES, C1, C, Unit>> f) =>
            Value.For(f);

        [Pure]
        public override Proxy<RT, REQ, RES, Unit, Void, S> Action<S>(Proxy<RT, REQ, RES, Unit, Void, S> r) =>
            Value.Action(r);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, Void, A> ComposeRight<UOutA, AUInA>(Func<REQ, Proxy<RT, UOutA, AUInA, REQ, RES, A>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, Unit, Void, A> ComposeRight<UOutA, AUInA>(Func<REQ, Proxy<RT, UOutA, AUInA, Unit, Void, RES>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, REQ, RES, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<Void, Proxy<RT, Unit, Void, DInC, DOutC, A>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, REQ, RES, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<Void, Proxy<RT, REQ, RES, DInC, DOutC, Unit>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, Void, Unit, RES, REQ, A> Reflect() =>
            Value.Reflect();

        [Pure]
        public override Proxy<RT, REQ, RES, Unit, Void, A> Observe() =>
            Value.Observe();

        [Pure]
        public void Deconstruct(out Proxy<RT, REQ, RES, Unit, Void, A> value) =>
            value = Value;

        [Pure]
        public static Effect<RT, A> operator |(Func<REQ, Server<RT, REQ, RES, A>> x, Client<RT, REQ, RES, A> y) =>
            y.ComposeRight(x).ToEffect();
        
        [Pure]
        public Client<RT, REQ, RES, B> SelectMany<B>(Func<A, Release<B>> bind) =>
            Value.Bind(a => bind(a).InterpretClient<RT, REQ, RES>()).ToClient();

        [Pure]
        public Client<RT, REQ, RES, C> SelectMany<B, C>(Func<A, Release<B>> bind, Func<A, B, C> project) =>
            SelectMany(a => bind(a).Select(b => project(a, b)));
    }
}
