using System;
using System.Diagnostics.Contracts;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

public record Queue<OUT, M, A> : Producer<OUT, M, A> 
    where M : Monad<M>
{
    /// <summary>
    /// Single queue channel
    /// </summary>
    readonly Channel<OUT> channel;

    /// <summary>
    /// Enqueue an item 
    /// </summary>
    public Unit Enqueue(OUT value) =>
        channel.Post(value);

    /// <summary>
    /// Enqueue an item 
    /// </summary>
    public K<M, Unit> EnqueueM(OUT value) =>
        M.Pure(Enqueue(value));

    /// <summary>
    /// Mark the Queue as done and cancel any Effect that it is in
    /// </summary>
    public Unit Done() =>
        channel.Stop();

    /// <summary>
    /// Mark the Queue as done and cancel any Effect that it is in
    /// </summary>
    public K<M, Unit> DoneM =>
        M.Pure(Done());
 
    internal Queue(Proxy<Void, Unit, Unit, OUT, M, A> value, Channel<OUT> channel) : base(value) =>
        this.channel = channel;
        
    [Pure]
    public override Proxy<Void, Unit, Unit, OUT, M, A> ToProxy() =>
        Value.ToProxy();

    [Pure]
    public override Proxy<Void, Unit, Unit, OUT, M, S> Bind<S>(Func<A, Proxy<Void, Unit, Unit, OUT, M, S>> f) =>
        Value.Bind(f);

    [Pure]
    public override Proxy<Void, Unit, Unit, OUT, M, B> Map<B>(Func<A, B> f) =>
        Value.Map(f);
        
    [Pure]
    public new Producer<OUT, M, B> Select<B>(Func<A, B> f) => 
        Value.Map(f).ToProducer();

    [Pure]
    public override Proxy<Void, Unit, C1, C, M, A> For<C1, C>(Func<OUT, Proxy<Void, Unit, C1, C, M, Unit>> body) =>
        Value.For(body);

    [Pure]
    public override Proxy<Void, Unit, Unit, OUT, M, B> Action<B>(Proxy<Void, Unit, Unit, OUT, M, B> r) =>
        Value.Action(r);

    /// <remarks>
    /// (f +>> p) pairs each 'request' in `this` with a 'respond' in `lhs`.
    /// </remarks>
    [Pure]
    public override Proxy<UOutA, AUInA, Unit, OUT, M, A> PairEachRequestWithRespond<UOutA, AUInA>(
        Func<Void, Proxy<UOutA, AUInA, Void, Unit, M, A>> lhs) =>
        Value.PairEachRequestWithRespond(lhs);

    [Pure]
    public override Proxy<UOutA, AUInA, Unit, OUT, M, A> ReplaceRequest<UOutA, AUInA>(
        Func<Void, Proxy<UOutA, AUInA, Unit, OUT, M, Unit>> lhs) =>
        Value.ReplaceRequest(lhs);

    [Pure]
    public override Proxy<Void, Unit, DInC, DOutC, M, A> PairEachRespondWithRequest<DInC, DOutC>(
        Func<OUT, Proxy<Unit, OUT, DInC, DOutC, M, A>> rhs) =>
        Value.PairEachRespondWithRequest(rhs);

    [Pure]
    public override Proxy<Void, Unit, DInC, DOutC, M, A> ReplaceRespond<DInC, DOutC>(
        Func<OUT, Proxy<Void, Unit, DInC, DOutC, M, Unit>> rhs) =>
        Value.ReplaceRespond(rhs);

    [Pure]
    public override Proxy<OUT, Unit, Unit, Void, M, A> Reflect() =>
        Value.Reflect();

    [Pure]
    public override Proxy<Void, Unit, Unit, OUT, M, A> Observe() =>
        Value.Observe();

    [Pure]
    public static Effect<M, A> operator |(Queue<OUT, M, A> p1, Consumer<OUT, M, A> p2) => 
        Proxy.compose(p1, p2);
        
    [Pure]
    public static Effect<M, A> operator |(Queue<OUT, M, A> p1, Consumer<OUT, A> p2) => 
        Proxy.compose(p1, p2);
    
    [Pure]
    public override string ToString() => 
        "queue";
}
