using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

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
public record Request<UOut, UIn, DIn, DOut, M, A>(UOut Value, Func<UIn, Proxy<UOut, UIn, DIn, DOut, M, A>> Next)
    : Proxy<UOut, UIn, DIn, DOut, M, A>
    where M : Monad<M> 
{
    [Pure]
    public override Proxy<UOut, UIn, DIn, DOut, M, A> ToProxy() => this;

    [Pure]
    public void Deconstruct(out UOut value, out Func<UIn, Proxy<UOut, UIn, DIn, DOut, M, A>> fun) =>
        (value, fun) = (Value, Next);

    [Pure]
    public override Proxy<UOut, UIn, C1, C, M, A> For<C1, C>(
        Func<DOut, Proxy<UOut, UIn, C1, C, M, DIn>> body) =>
        ReplaceRespond(body);

    [Pure]
    public override Proxy<UOut, UIn, DIn, DOut, M, S> Action<S>(
        Proxy<UOut, UIn, DIn, DOut, M, S> r) =>
        new Request<UOut, UIn, DIn, DOut, M, S>(Value, a => Next(a).Action(r));
        
    /*
        (f >\\ p) replaces each 'request' in `this` with `lhs`.

        Point-ful version of (\>\)
    */
    [Pure]
    public override Proxy<UOutA, AUInA, DIn, DOut, M, A> ReplaceRequest<UOutA, AUInA>(
        Func<UOut, Proxy<UOutA, AUInA, DIn, DOut, M, UIn>> lhs) =>
        lhs(Value).Bind(b => Next(b).ReplaceRequest(lhs));

    /*
        (p //> f) replaces each 'respond' in `this` with `rhs`

        Point-ful version of />/
    */
    [Pure]
    public override Proxy<UOut, UIn, DInC, DOutC, M, A> ReplaceRespond<DInC, DOutC>(
        Func<DOut, Proxy<UOut, UIn, DInC, DOutC, M, DIn>> rhs) =>
        new Request<UOut, UIn, DInC, DOutC, M, A>(Value, x => Next(x).ReplaceRespond(rhs));

    /// <remarks>
    /// (f +>> p) pairs each 'request' in `this` with a 'respond' in `lhs`.
    /// </remarks>
    [Pure]
    public override Proxy<UOutA, AUInA, DIn, DOut, M, A> PairEachRequestWithRespond<UOutA, AUInA>(
        Func<UOut, Proxy<UOutA, AUInA, UOut, UIn, M, A>> lhs) =>
        lhs(Value).PairEachRespondWithRequest(Next);

    /*
        (p >>~ f) pairs each 'respond' in `this` with a 'request' in `rhs`.

        Point-ful version of ('>~>')
    */
    [Pure]
    public override Proxy<UOut, UIn, DInC, DOutC, M, A> PairEachRespondWithRequest<DInC, DOutC>(
        Func<DOut, Proxy<DIn, DOut, DInC, DOutC, M, A>> rhs) =>
        new Request<UOut, UIn, DInC, DOutC, M, A>(Value, a => Next(a).PairEachRespondWithRequest(rhs));

    [Pure]
    public override Proxy<DOut, DIn, UIn, UOut, M, A> Reflect() =>
        new Respond<DOut, DIn, UIn, UOut, M, A>(Value, x => Next(x).Reflect());

    [Pure]
    public override Proxy<UOut, UIn, DIn, DOut, M, A> Observe() =>
        new ProxyM<UOut, UIn, DIn, DOut, M, A>(
            M.Pure(new Request<UOut, UIn, DIn, DOut, M, A>(
                       Value,
                       x => Next(x).Observe())));
 
    [Pure]
    public override Proxy<UOut, UIn, DIn, DOut, M, S> Bind<S>(Func<A, Proxy<UOut, UIn, DIn, DOut, M, S>> f) =>
        new Request<UOut, UIn, DIn, DOut, M, S>(Value, a => Next(a).Bind(f));

    [Pure]
    public override Proxy<UOut, UIn, DIn, DOut, M, S> Map<S>(Func<A, S> f) =>
        new Request<UOut, UIn, DIn, DOut, M, S>(Value, a => Next(a).Map(f));
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
public record Respond<UOut, UIn, DIn, DOut, M, A>(DOut Value, Func<DIn, Proxy<UOut, UIn, DIn, DOut, M, A>> Next)
    : Proxy<UOut, UIn, DIn, DOut, M, A>
    where M : Monad<M> 
{
    [Pure]
    public override Proxy<UOut, UIn, DIn, DOut, M, A> ToProxy() => this;

    [Pure]
    public override Proxy<UOut, UIn, DIn, DOut, M, S> Bind<S>(Func<A, Proxy<UOut, UIn, DIn, DOut, M, S>> f) =>
        new Respond<UOut, UIn, DIn, DOut, M, S>(Value, b1 => Next(b1).Bind(f));

    [Pure]
    public override Proxy<UOut, UIn, DIn, DOut, M, S> Map<S>(Func<A, S> f) =>
        new Respond<UOut, UIn, DIn, DOut, M, S>(Value, a => Next(a).Map(f));

    [Pure]
    public override Proxy<UOut, UIn, C1, C, M, A> For<C1, C>(Func<DOut, Proxy<UOut, UIn, C1, C, M, DIn>> body) =>
        ReplaceRespond(body);
            
    [Pure]
    public override Proxy<UOut, UIn, DIn, DOut, M, S> Action<S>(Proxy<UOut, UIn, DIn, DOut, M, S> r) =>
        new Respond<UOut, UIn, DIn, DOut, M, S>(Value, b1 => Next(b1).Action(r));

    /*
        (f >\\ p) replaces each 'request' in `this` with `lhs`.

        Point-ful version of (\>\)
    */
    [Pure]
    public override Proxy<UOutA, AUInA, DIn, DOut, M, A> ReplaceRequest<UOutA, AUInA>(Func<UOut, Proxy<UOutA, AUInA, DIn, DOut, M, UIn>> lhs) =>
        new Respond<UOutA, AUInA, DIn, DOut, M, A>(Value, x1 => Next(x1).ReplaceRequest(lhs));

    /*
        (p //> f) replaces each 'respond' in `this` with `rhs`

        Point-ful version of />/
    */
    [Pure]
    public override Proxy<UOut, UIn, DInC, DOutC, M, A> ReplaceRespond<DInC, DOutC>(
        Func<DOut, Proxy<UOut, UIn, DInC, DOutC, M, DIn>> rhs) =>
        rhs(Value).Bind(b1 => Next(b1).ReplaceRespond(rhs));

    /*
       (f +>> p) pairs each 'request' in `this` with a 'respond' in `lhs`.

       Point-ful version of ('>+>')
    */
    /// <remarks>
    /// (f +>> p) pairs each 'request' in `this` with a 'respond' in `lhs`.
    /// </remarks>
    [Pure]
    public override Proxy<UOutA, AUInA, DIn, DOut, M, A> PairEachRequestWithRespond<UOutA, AUInA>(
        Func<UOut, Proxy<UOutA, AUInA, UOut, UIn, M, A>> fb1) =>
        new Respond<UOutA, AUInA, DIn, DOut, M, A>(Value, c1 => Next(c1).PairEachRequestWithRespond(fb1));

    /*
        (p >>~ f) pairs each 'respond' in `this` with a 'request' in `rhs`.

        Point-ful version of ('>~>')
    */
    [Pure]
    public override Proxy<UOut, UIn, DInC, DOutC, M, A> PairEachRespondWithRequest<DInC, DOutC>(
        Func<DOut, Proxy<DIn, DOut, DInC, DOutC, M, A>> rhs) =>
        rhs(Value).PairEachRequestWithRespond(Next);

    [Pure]
    public override Proxy<DOut, DIn, UIn, UOut, M, A> Reflect() =>
        new Request<DOut, DIn, UIn, UOut, M, A>(Value, x => Next(x).Reflect());

    [Pure]
    public override Proxy<UOut, UIn, DIn, DOut, M, A> Observe() =>
        new ProxyM<UOut, UIn, DIn, DOut, M, A>(
            M.Pure(new Respond<UOut, UIn, DIn, DOut, M, A>(
                       Value,
                       x => Next(x).Observe())));

    [Pure]
    public void Deconstruct(out DOut value, out Func<DIn, Proxy<UOut, UIn, DIn, DOut, M, A>> fun) =>
        (value, fun) = (Value, Next);
}
