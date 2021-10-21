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
    /// `Server` receives requests of type `REQ` and sends responses of type `RES`.
    ///
    /// `Servers` only `respond` and never `request`.
    /// </summary>
    /// <remarks> 
    ///       Upstream | Downstream
    ///           +---------+
    ///           |         |
    ///     Void <==       <== RES
    ///           |         |
    ///     Unit ==>       ==> REQ
    ///           |    |    |
    ///           +----|----+
    ///                |
    ///                A
    /// </remarks>
    public class Server<RT, REQ, RES, A> : Proxy<RT, Void, Unit, REQ, RES, A>  where RT : struct, HasCancel<RT>
    {
        public readonly Proxy<RT, Void, Unit, REQ, RES, A> Value;
    
        public Server(Proxy<RT, Void, Unit, REQ, RES, A> value) =>
            Value = value;
        
        [Pure]
        public override Proxy<RT, Void, Unit, REQ, RES, A> ToProxy() =>
            Value.ToProxy();

        [Pure]
        public override Proxy<RT, Void, Unit, REQ, RES, S> Bind<S>(Func<A, Proxy<RT, Void, Unit, REQ, RES, S>> f) =>
            Value.Bind(f);
            
        [Pure]
        public override Proxy<RT, Void, Unit, REQ, RES, S> Map<S>(Func<A, S> f) =>
            Value.Map(f);
        
        [Pure]
        public Server<RT, REQ, RES, B> Bind<B>(Func<A, Server<RT, REQ, RES, B>> f) => 
            Value.Bind(f).ToServer();
        
        [Pure]
        public Server<RT, REQ, RES, B> SelectMany<B>(Func<A, Server<RT, REQ, RES, B>> f) => 
            Value.Bind(f).ToServer();
        
        [Pure]
        public Server<RT, REQ, RES, C> SelectMany<B, C>(Func<A, Server<RT, REQ, RES, B>> f, Func<A, B, C> project) => 
            Value.Bind(a => f(a).Map(b => project(a, b))).ToServer();
        
        [Pure]
        public new Server<RT, REQ, RES, B> Select<B>(Func<A, B> f) => 
            Value.Map(f).ToServer();
        
        [Pure]
        public override Proxy<RT, Void, Unit, C1, C, A> For<C1, C>(Func<RES, Proxy<RT, Void, Unit, C1, C, REQ>> f) =>
            Value.For(f);

        [Pure]
        public override Proxy<RT, Void, Unit, REQ, RES, S> Action<S>(Proxy<RT, Void, Unit, REQ, RES, S> r) =>
            Value.Action(r);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, REQ, RES, A> ComposeRight<UOutA, AUInA>(Func<Void, Proxy<RT, UOutA, AUInA, Void, Unit, A>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, UOutA, AUInA, REQ, RES, A> ComposeRight<UOutA, AUInA>(Func<Void, Proxy<RT, UOutA, AUInA, REQ, RES, Unit>> lhs) =>
            Value.ComposeRight(lhs);

        [Pure]
        public override Proxy<RT, Void, Unit, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<RES, Proxy<RT, REQ, RES, DInC, DOutC, A>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, Void, Unit, DInC, DOutC, A> ComposeLeft<DInC, DOutC>(Func<RES, Proxy<RT, Void, Unit, DInC, DOutC, REQ>> rhs) =>
            Value.ComposeLeft(rhs);

        [Pure]
        public override Proxy<RT, RES, REQ, Unit, Void, A> Reflect() =>
            Value.Reflect();

        [Pure]
        public override Proxy<RT, Void, Unit, REQ, RES, A> Observe() =>
            Value.Observe();

        [Pure]
        public void Deconstruct(out Proxy<RT, Void, Unit, REQ, RES, A> value) =>
            value = Value;
        
        public Server<RT, REQ, RES, B> SelectMany<B>(Func<A, Release<B>> bind) =>
            Value.Bind(a => bind(a).InterpretServer<RT, REQ, RES>()).ToServer();

        public Server<RT, REQ, RES, C> SelectMany<B, C>(Func<A, Release<B>> bind, Func<A, B, C> project) =>
            SelectMany(a => bind(a).Select(b => project(a, b)));
    }
}
