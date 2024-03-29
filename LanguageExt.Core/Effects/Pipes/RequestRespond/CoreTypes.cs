using System;
using System.Diagnostics.Contracts;
using LanguageExt.Effects.Traits;

namespace LanguageExt.Pipes
{
    /// <summary>
    /// One of the algebraic cases of the `Proxy` type.  This type represents a request.
    /// </summary>
    /// <typeparam name="RT">Aff system runtime</typeparam>
    /// <typeparam name="UOut">Upstream out type</typeparam>
    /// <typeparam name="UIn">Upstream in type</typeparam>
    /// <typeparam name="DIn">Downstream in type</typeparam>
    /// <typeparam name="DOut">Downstream uut type</typeparam>
    /// <typeparam name="A">The monadic bound variable - it doesn't flow up or down stream, it works just like any bound
    /// monadic variable.  If the effect represented by the `Proxy` ends, then this will be the result value.
    ///
    /// When composing `Proxy` sub-types (like `Producer`, `Pipe`, `Consumer`, etc.)  </typeparam>
    public class Request<RT, UOut, UIn, DIn, DOut, A> : Proxy<RT, UOut, UIn, DIn, DOut, A> where RT : struct, HasCancel<RT>
    {
        public readonly UOut Value;
        public readonly Func<UIn, Proxy<RT, UOut, UIn, DIn, DOut, A>> Next;
        
        public Request(UOut value, Func<UIn, Proxy<RT, UOut, UIn, DIn, DOut, A>> next) =>
            (Value, Next) = (value, next);

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> ToProxy() => this;

        [Pure]
        public void Deconstruct(out UOut value, out Func<UIn, Proxy<RT, UOut, UIn, DIn, DOut, A>> fun) =>
            (value, fun) = (Value, Next);

        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, A> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> body) =>
            ReplaceRespond(body);

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Action<S>(Proxy<RT, UOut, UIn, DIn, DOut, S> r) =>
            new Request<RT, UOut, UIn, DIn, DOut, S>(Value, a => Next(a).Action(r));
        
        /*
            (f >\\ p) replaces each 'request' in `this` with `lhs`.

            Point-ful version of (\>\)
        */
        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ReplaceRequest<UOutA, AUInA>(
            Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs) =>
            lhs(Value).Bind(b => Next(b).ReplaceRequest(lhs));

        /*
            (p //> f) replaces each 'respond' in `this` with `rhs`

            Point-ful version of />/
        */
        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ReplaceRespond<DInC, DOutC>(
            Func<DOut, Proxy<RT, UOut, UIn, DInC, DOutC, DIn>> rhs) =>
            new Request<RT, UOut, UIn, DInC, DOutC, A>(Value, x => Next(x).ReplaceRespond(rhs));

        /// <remarks>
        /// (f +>> p) pairs each 'request' in `this` with a 'respond' in `lhs`.
        /// </remarks>
        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> PairEachRequestWithRespond<UOutA, AUInA>(
            Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, A>> lhs) =>
            lhs(Value).PairEachRespondWithRequest(Next);

        /*
            (p >>~ f) pairs each 'respond' in `this` with a 'request' in `rhs`.

            Point-ful version of ('>~>')
        */
        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> PairEachRespondWithRequest<DInC, DOutC>(
            Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, A>> rhs) =>
            new Request<RT, UOut, UIn, DInC, DOutC, A>(Value, a => Next(a).PairEachRespondWithRequest(rhs));

        [Pure]
        public override Proxy<RT, DOut, DIn, UIn, UOut, A> Reflect() =>
            new Respond<RT, DOut, DIn, UIn, UOut, A>(Value, x => Next(x).Reflect());
 
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> Observe() =>
            new M<RT, UOut, UIn, DIn, DOut, A>(
                Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, A>>.Success(new Request<RT, UOut, UIn, DIn, DOut, A>(
                                                                        Value,
                                                                        x => Next(x).Observe()))
                );
 
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Bind<S>(Func<A, Proxy<RT, UOut, UIn, DIn, DOut, S>> f) =>
            new Request<RT, UOut, UIn, DIn, DOut, S>(Value, a => Next(a).Bind(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Map<S>(Func<A, S> f) =>
            new Request<RT, UOut, UIn, DIn, DOut, S>(Value, a => Next(a).Map(f));
    }

    /// <summary>
    /// One of the algebraic cases of the `Proxy` type.  This type represents a response.
    /// </summary>
    /// <typeparam name="RT">Aff system runtime</typeparam>
    /// <typeparam name="UOut">Upstream out type</typeparam>
    /// <typeparam name="UIn">Upstream in type</typeparam>
    /// <typeparam name="DIn">Downstream in type</typeparam>
    /// <typeparam name="DOut">Downstream uut type</typeparam>
    /// <typeparam name="A">The monadic bound variable - it doesn't flow up or down stream, it works just like any bound
    /// monadic variable.  If the effect represented by the `Proxy` ends, then this will be the result value.
    ///
    /// When composing `Proxy` sub-types (like `Producer`, `Pipe`, `Consumer`, etc.)  </typeparam>
    public class Respond<RT, UOut, UIn, DIn, DOut, A> : Proxy<RT, UOut, UIn, DIn, DOut, A> 
        where RT : struct, HasCancel<RT>
    {
        public readonly DOut Value;
        public readonly Func<DIn, Proxy<RT, UOut, UIn, DIn, DOut, A>> Next;
        
        public Respond(DOut value, Func<DIn, Proxy<RT, UOut, UIn, DIn, DOut, A>> next) =>
            (Value, Next) = (value, next);

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> ToProxy() => this;

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Bind<S>(Func<A, Proxy<RT, UOut, UIn, DIn, DOut, S>> f) =>
            new Respond<RT, UOut, UIn, DIn, DOut, S>(Value, b1 => Next(b1).Bind(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Map<S>(Func<A, S> f) =>
            new Respond<RT, UOut, UIn, DIn, DOut, S>(Value, a => Next(a).Map(f));

        [Pure]
        public override Proxy<RT, UOut, UIn, C1, C, A> For<C1, C>(Func<DOut, Proxy<RT, UOut, UIn, C1, C, DIn>> body) =>
            ReplaceRespond(body);
            
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, S> Action<S>(Proxy<RT, UOut, UIn, DIn, DOut, S> r) =>
            new Respond<RT, UOut, UIn, DIn, DOut, S>(Value, b1 => Next(b1).Action(r));

        /*
            (f >\\ p) replaces each 'request' in `this` with `lhs`.

            Point-ful version of (\>\)
        */
        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> ReplaceRequest<UOutA, AUInA>(Func<UOut, Proxy<RT, UOutA, AUInA, DIn, DOut, UIn>> lhs) =>
            new Respond<RT, UOutA, AUInA, DIn, DOut, A>(Value, x1 => Next(x1).ReplaceRequest(lhs));

        /*
            (p //> f) replaces each 'respond' in `this` with `rhs`

            Point-ful version of />/
        */
        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> ReplaceRespond<DInC, DOutC>(
            Func<DOut, Proxy<RT, UOut, UIn, DInC, DOutC, DIn>> rhs) =>
            rhs(Value).Bind(b1 => Next(b1).ReplaceRespond(rhs));

        /*
           (f +>> p) pairs each 'request' in `this` with a 'respond' in `lhs`.

           Point-ful version of ('>+>')
        */
        /// <remarks>
        /// (f +>> p) pairs each 'request' in `this` with a 'respond' in `lhs`.
        /// </remarks>
        [Pure]
        public override Proxy<RT, UOutA, AUInA, DIn, DOut, A> PairEachRequestWithRespond<UOutA, AUInA>(
            Func<UOut, Proxy<RT, UOutA, AUInA, UOut, UIn, A>> fb1) =>
            new Respond<RT, UOutA, AUInA, DIn, DOut, A>(Value, c1 => Next(c1).PairEachRequestWithRespond(fb1));

        /*
            (p >>~ f) pairs each 'respond' in `this` with a 'request' in `rhs`.

            Point-ful version of ('>~>')
        */
        [Pure]
        public override Proxy<RT, UOut, UIn, DInC, DOutC, A> PairEachRespondWithRequest<DInC, DOutC>(
            Func<DOut, Proxy<RT, DIn, DOut, DInC, DOutC, A>> rhs) =>
            rhs(Value).PairEachRequestWithRespond(Next);

        [Pure]
        public override Proxy<RT, DOut, DIn, UIn, UOut, A> Reflect() =>
            new Request<RT, DOut, DIn, UIn, UOut, A>(Value, x => Next(x).Reflect());
 
        [Pure]
        public override Proxy<RT, UOut, UIn, DIn, DOut, A> Observe() =>
            new M<RT, UOut, UIn, DIn, DOut, A>(
                Aff<RT, Proxy<RT, UOut, UIn, DIn, DOut, A>>.Success(new Respond<RT, UOut, UIn, DIn, DOut, A>(
                                                                        Value,
                                                                        x => Next(x).Observe()))
            );

        [Pure]
        public void Deconstruct(out DOut value, out Func<DIn, Proxy<RT, UOut, UIn, DIn, DOut, A>> fun) =>
            (value, fun) = (Value, Next);
    }
}
