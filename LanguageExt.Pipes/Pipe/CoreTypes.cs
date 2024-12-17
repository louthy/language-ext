using System;
using System.Diagnostics.Contracts;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes;

/// <summary>
/// Pipes both can both be `await` and can `yield`
/// </summary>
/// <remarks>
///       Upstream | Downstream
///           +---------+
///           |         |
///     Unit〈==      〈== Unit
///           |         |
///      IN  ==〉      ==〉OUT
///           |    |    |
///           +----|----+
///                |
///                A
/// </remarks>
public record Pipe<IN, OUT, M, A> : Proxy<Unit, IN, Unit, OUT, M, A> 
    where M : Monad<M>
{
    public readonly Proxy<Unit, IN, Unit, OUT, M, A> Value;
        
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="value">Correctly shaped `Proxy` that represents a `Pipe`</param>
    public Pipe(Proxy<Unit, IN, Unit, OUT, M, A> value) =>
        Value = value;
        
    /// <summary>
    /// Calling this will effectively cast the sub-type to the base.
    /// </summary>
    /// <remarks>This type wraps up a `Proxy` for convenience, and so it's a `Proxy` proxy.  So calling this method
    /// isn't exactly the same as a cast operation, as it unwraps the `Proxy` from within.  It has the same effect
    /// however, and removes a level of indirection</remarks>
    /// <returns>A general `Proxy` type from a more specialised type</returns>
    [Pure]
    public override Proxy<Unit, IN, Unit, OUT, M, A> ToProxy() =>
        Value.ToProxy();

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public override Proxy<Unit, IN, Unit, OUT, M, S> Bind<S>(
        Func<A, Proxy<Unit, IN, Unit, OUT, M, S>> f) =>
        Value.Bind(f);

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Pipe<IN, OUT, M, B> Bind<B>(Func<A, K<M, B>> f) =>
        Value.Bind(x => Pipe.lift<IN, OUT, M, B>(f(x))).ToPipe();

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Pipe<IN, OUT, M, B> Bind<B>(Func<A, IO<B>> f) =>
        Value.Bind(x => Pipe.liftIO<IN, OUT, M, B>(f(x))).ToPipe();
            
    /// <summary>
    /// Lifts a pure function into the `Proxy` domain, causing it to map the bound value within
    /// </summary>
    /// <param name="f">The map function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the map operation</returns>
    [Pure]
    public override Proxy<Unit, IN, Unit, OUT, M, B> Map<B>(Func<A, B> f) =>
        Value.Map(f);

    /// <summary>
    /// Map the lifted monad
    /// </summary>
    /// <param name="f">The map function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the map operation</returns>
    [Pure]
    public override Proxy<Unit, IN, Unit, OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        Value.MapM(f);
        
    /// <summary>
    /// Extract the lifted IO monad (if there is one)
    /// </summary>
    /// <param name="f">The map function</param>
    /// <returns>A new `Proxy` that represents the innermost IO monad, if it exists.</returns>
    /// <exception cref="ExceptionalException">`Errors.UnliftIONotSupported` if there's no IO monad in the stack</exception>
    [Pure]
    public override Proxy<Unit, IN, Unit, OUT, M, IO<A>> ToIO() =>
        Value.ToIO();

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Pipe<IN, OUT, M, B> Bind<B>(Func<A, Pipe<IN, OUT, M, B>> f) => 
        Value.Bind(f).ToPipe();
        
    /// <summary>
    /// Lifts a pure function into the `Proxy` domain, causing it to map the bound value within
    /// </summary>
    /// <param name="f">The map function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the map operation</returns>
    [Pure]
    public new Pipe<IN, OUT, M, B> Select<B>(Func<A, B> f) => 
        Value.Map(f).ToPipe();        

    /// <summary>
    /// `For(body)` loops over the `Proxy p` replacing each `yield` with `body`
    /// </summary>
    /// <param name="body">Any `yield` found in the `Proxy` will be replaced with this function.  It will be composed so
    /// that the value yielded will be passed to the argument of the function.  That returns a `Proxy` to continue the
    /// processing of the computation</param>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the function provided</returns>
    [Pure]
    public override Proxy<Unit, IN, C1, C, M, A> For<C1, C>(
        Func<OUT, Proxy<Unit, IN, C1, C, M, Unit>> body) =>
        Value.For(body);

    /// <summary>
    /// Applicative action
    ///
    /// Invokes this `Proxy`, then the `Proxy r` 
    /// </summary>
    /// <param name="r">`Proxy` to run after this one</param>
    [Pure]
    public override Proxy<Unit, IN, Unit, OUT, M, S> Action<S>(Proxy<Unit, IN, Unit, OUT, M, S> r) =>
        Value.Action(r);


    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    /// <remarks>
    /// (f +>> p) pairs each 'request' in `this` with a 'respond' in `lhs`.
    /// </remarks>
    [Pure]
    public override Proxy<UOutA, AUInA, Unit, OUT, M, A> PairEachRequestWithRespond<UOutA, AUInA>(
        Func<Unit, Proxy<UOutA, AUInA, Unit, IN, M, A>> lhs) =>
        Value.PairEachRequestWithRespond(lhs);

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    [Pure]
    public override Proxy<UOutA, AUInA, Unit, OUT, M, A> ReplaceRequest<UOutA, AUInA>(
        Func<Unit, Proxy<UOutA, AUInA, Unit, OUT, M, IN>> lhs) =>
        Value.ReplaceRequest(lhs);

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    [Pure]
    public override Proxy<Unit, IN, DInC, DOutC, M, A> PairEachRespondWithRequest<DInC, DOutC>(
        Func<OUT, Proxy<Unit, OUT, DInC, DOutC, M, A>> rhs) =>
        Value.PairEachRespondWithRequest(rhs);

    /// <summary>
    /// Used by the various composition functions and when composing proxies with the `|` operator.  You usually
    /// wouldn't need to call this directly, instead either pipe them using `|` or call `Proxy.compose(lhs, rhs)` 
    /// </summary>
    [Pure]
    public override Proxy<Unit, IN, DInC, DOutC, M, A> ReplaceRespond<DInC, DOutC>(
        Func<OUT, Proxy<Unit, IN, DInC, DOutC, M, Unit>> rhs) =>
        Value.ReplaceRespond(rhs);

    /// <summary>
    /// Reverse the arrows of the `Proxy` to find its dual.  
    /// </summary>
    /// <returns>The dual of `this`</returns>
    [Pure]
    public override Proxy<OUT, Unit, IN, Unit, M, A> Reflect() =>
        Value.Reflect();

    /// <summary>
    /// 
    ///     Observe(lift (Pure(r))) = Observe(Pure(r))
    ///     Observe(lift (m.Bind(f))) = Observe(lift(m.Bind(x => lift(f(x)))))
    /// 
    /// This correctness comes at a small cost to performance, so use this function sparingly.
    /// This function is a convenience for low-level pipes implementers.  You do not need to
    /// use observe if you stick to the safe API.        
    /// </summary>
    [Pure]
    public override Proxy<Unit, IN, Unit, OUT, M, A> Observe() =>
        Value.Observe();

    [Pure]
    public void Deconstruct(out Proxy<Unit, IN, Unit, OUT, M, A> value) =>
        value = Value;
        
    /// <summary>
    /// Compose a `Producer` and a `Pipe` together into a `Producer`.  
    /// </summary>
    /// <param name="p1">`Producer`</param>
    /// <param name="p2">`Pipe`</param>
    /// <returns>`Producer`</returns>
    [Pure]
    public static Producer<OUT, M, A> operator |(Producer<IN, M, A> p1, Pipe<IN, OUT, M, A> p2) =>
        Proxy.compose(p1, p2);
        
    /// <summary>
    /// Compose a `Producer` and a `Pipe` together into a `Producer`.  
    /// </summary>
    /// <param name="p1">`Producer`</param>
    /// <param name="p2">`Pipe`</param>
    /// <returns>`Producer`</returns>
    [Pure]
    public static Producer<OUT, M, A> operator |(Producer<IN, A> p1, Pipe<IN, OUT, M, A> p2) =>
        Proxy.compose(p1, p2);
        
    /// <summary>
    /// Compose a `Producer` and a `Pipe` together into a `Producer`.  
    /// </summary>
    /// <param name="p1">`Producer`</param>
    /// <param name="p2">`Pipe`</param>
    /// <returns>`Producer`</returns>
    [Pure]
    public static Producer<OUT, M, A> operator |(Producer<OUT, IN> p1, Pipe<IN, OUT, M, A> p2) =>
        Proxy.compose(p1, p2);
        
    /// <summary>
    /// Compose a `Pipe` and a `Consumer` together into a `Consumer`.  
    /// </summary>
    /// <param name="p1">`Pipe`</param>
    /// <param name="p2">`Consumer`</param>
    /// <returns>`Consumer`</returns>
    [Pure]
    public static Consumer<IN, M, A> operator |(Pipe<IN, OUT, M, A> p1, Consumer<OUT, A> p2) =>
        Proxy.compose(p1, p2);
        
    /// <summary>
    /// Compose a `Pipe` and a `Consumer` together into a `Consumer`.  
    /// </summary>
    /// <param name="p1">`Pipe`</param>
    /// <param name="p2">`Consumer`</param>
    /// <returns>`Consumer`</returns>
    [Pure]
    public static Consumer<IN, M, A> operator |(Pipe<IN, OUT, M, A> p1, Consumer<OUT, M, A> p2) =>
        Proxy.compose(p1, p2);
        
    /// <summary>
    /// Composition chaining of two pipes.  
    /// </summary>
    /// <remarks>
    /// We can't provide the operator overloading to chain two pipes together, because operators don't have
    /// generics of their own.  And so we can use this method to chain to pipes together.
    /// </remarks>
    /// <param name="pipe">Right hand side `Pipe`</param>
    /// <typeparam name="C">Type of value that the right hand side pipe yields</typeparam>
    /// <returns>A composition of this pipe and the `Pipe` provided</returns>
    [Pure]
    public Pipe<IN, C, M, A> Then<C>(Pipe<OUT, C, M, A> pipe) =>
        Proxy.compose(this, pipe);

    /// <summary>
    /// Conversion operator from the _pure_ `Pipe` type, to the monad transformer version of the `Pipe`
    /// </summary>
    /// <param name="p">Pure `Pipe`</param>
    /// <returns>Monad transformer version of the `Pipe`</returns>
    [Pure]
    public static implicit operator Pipe<IN, OUT, M, A>(Pipe<IN, OUT, A> p) =>
        p.Interpret<M>();

    /// <summary>
    /// Conversion operator from the `Pure` type, to the monad transformer version of the `Pipe`
    /// </summary>
    /// <param name="p">`Pure` value</param>
    /// <returns>Monad transformer version of the `Pipe`</returns>
    [Pure]
    public static implicit operator Pipe<IN, OUT, M, A>(Pure<A> p) =>
        Pipe.Pure<IN, OUT, M, A>(p.Value);

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Pipe<IN, OUT, M, B> SelectMany<B>(Func<A, Pipe<IN, OUT, B>> bind) =>
        Value.Bind(a => bind(a).Interpret<M>()).ToPipe();

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Pipe<IN, OUT, M, C> SelectMany<B, C>(Func<A, Pipe<IN, OUT, B>> bind, Func<A, B, C> project) =>
        SelectMany(a => bind(a).Select(b => project(a, b)));
        
    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Pipe<IN, OUT, M, B> SelectMany<B>(Func<A, Pipe<IN, OUT, M, B>> f) => 
        Value.Bind(f).ToPipe();
        
    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Pipe<IN, OUT, M, C> SelectMany<B, C>(Func<A, Pipe<IN, OUT, M, B>> f, Func<A, B, C> project) => 
        Value.Bind(a => f(a).Map(b => project(a, b))).ToPipe();

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Pipe<IN, OUT, M, C> SelectMany<B, C>(Func<A, K<M, B>> bind, Func<A, B, C> project) =>
        Bind(x => M.Map(y => project(x, y), bind(x)));

    /// <summary>
    /// Monadic bind operation, for chaining `Proxy` computations together.
    /// </summary>
    /// <param name="f">The bind function</param>
    /// <typeparam name="B">The mapped bound value type</typeparam>
    /// <returns>A new `Proxy` that represents the composition of this `Proxy` and the result of the bind operation</returns>
    [Pure]
    public Pipe<IN, OUT, M, C> SelectMany<B, C>(Func<A, IO<B>> bind, Func<A, B, C> project) =>
        Bind(x => bind(x).Map(y => project(x, y)));

    /// <summary>
    /// Chain one pipe's set of awaits and yields after another
    /// </summary>
    [Pure]
    public static Pipe<IN, OUT, M, A> operator &(
        Pipe<IN, OUT, M, A> lhs,
        Pipe<IN, OUT, M, A> rhs) =>
        lhs.Bind(_ => rhs);
    
    [Pure]
    public override string ToString() => 
        "pipe";
}
