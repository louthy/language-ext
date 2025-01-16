using LanguageExt.Async.Linq;
using LanguageExt.Common;
using LanguageExt.Traits;

namespace LanguageExt.Pipes2;

public static class Prelude
{
}

/// <summary>
/// `ProducerT` streaming producer monad-transformer
/// </summary>
public static class ProducerT
{
    /// <summary>
    /// Create a producer that simply returns a bound value without yielding anything
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> pure<OUT, M, A>(A value) 
        where M : Monad<M> =>
        new ProducerTPure<OUT, M, A>(value);
    
    /// <summary>
    /// Create a producer that always fails
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="E">Failure type</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> fail<OUT, E, M, A>(E value) 
        where M : Monad<M>, Fallible<E, M> =>
        new ProducerTFail<OUT, E, M, A>(value);
    
    /// <summary>
    /// Create a producer that always fails
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> error<OUT, M, A>(Error value) 
        where M : Monad<M>, Fallible<M> =>
        new ProducerTFail<OUT, Error, M, A>(value);
    
    /// <summary>
    /// Create a producer that simply returns the bound value of the lifted monad without yielding anything
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> liftM<OUT, M, A>(K<M, A> value) 
        where M : Monad<M> =>
        new ProducerTLiftM<OUT, A, M, A>(value, pure<OUT, M, A>);
    
    /// <summary>
    /// Create a producer that yields nothing at all
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, A> empty<OUT, M, A>() 
        where M : Monad<M>, MonoidK<M> =>
        ProducerTEmpty<OUT, M, A>.Default;
    
    /// <summary>
    /// Yield a value downstream
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, Unit> yield<OUT, M>(OUT value) 
        where M : Monad<M> =>
        new ProducerTYield<OUT, M, Unit>(value, _ => pure<OUT, M, Unit>(default));
    
    /// <summary>
    /// Yield all values downstream
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, Unit> yieldAll<M, OUT>(IEnumerable<OUT> values) 
        where M : Monad<M>, Alternative<M> =>
        new ProducerTEnumerableYield<OUT, M, Unit>(values, _ => pure<OUT, M, Unit>(default));
    
    /// <summary>
    /// Yield all values downstream
    /// </summary>
    /// <typeparam name="OUT">Stream value to produce</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns></returns>
    public static ProducerT<OUT, M, Unit> yieldAll<OUT, M>(IAsyncEnumerable<OUT> values) 
        where M : Monad<M>, Alternative<M> =>
        new ProducerTAsyncEnumerableYield<OUT, M, Unit>(values, _ => pure<OUT, M, Unit>(default));    
}

/// <summary>
/// `ConsumerT` streaming consumer monad-transformer
/// </summary>
public static class ConsumerT
{
    /// <summary>
    /// Create a consumer that simply returns a bound value without awaiting anything
    /// </summary>
    /// <typeparam name="IN">Stream value to await</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ConsumerT<IN, M, A> pure<IN, M, A>(A value) 
        where M : Monad<M> =>
        new ConsumerTPure<IN, M, A>(value);
    
    /// <summary>
    /// Create a consumer that always fails
    /// </summary>
    /// <typeparam name="IN">Stream value to await</typeparam>
    /// <typeparam name="E">Failure type</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ConsumerT<IN, M, A> fail<IN, E, M, A>(E value) 
        where M : Monad<M>, Fallible<E, M> =>
        new ConsumerTFail<IN, E, M, A>(value);
    
    /// <summary>
    /// Create a consumer that always fails
    /// </summary>
    /// <typeparam name="IN">Stream value to await</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ConsumerT<IN, M, A> error<IN, M, A>(Error value) 
        where M : Monad<M>, Fallible<M> =>
        new ConsumerTFail<IN, Error, M, A>(value);
    
    /// <summary>
    /// Create a consumer that simply returns the bound value of the lifted monad without awaiting anything
    /// </summary>
    /// <typeparam name="IN">Stream value to await</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static ConsumerT<IN, M, A> liftM<IN, M, A>(K<M, A> value) 
        where M : Monad<M> =>
        new ConsumerTLiftM<IN, A, M, A>(value, pure<IN, M, A>);
    
    /// <summary>
    /// Await a value from upstream
    /// </summary>
    /// <typeparam name="IN">Stream value to await</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <returns></returns>
    public static ConsumerT<IN, M, IN> awaiting<M, IN>() 
        where M : Monad<M>, MonoidK<M> =>
        new ConsumerTAwait<IN, M, IN>(pure<IN, M, IN>);
}


/// <summary>
/// `EffectT` streaming effect monad-transformer
/// </summary>
public static class EffectT
{
    /// <summary>
    /// Create an effect that simply returns a bound value 
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> pure<M, A>(A value) 
        where M : Monad<M> =>
        new EffectTPure<M, A>(value);
    
    /// <summary>
    /// Create an effect that always fails
    /// </summary>
    /// <typeparam name="E">Failure type</typeparam>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> fail<E, M, A>(E value) 
        where M : Monad<M>, Fallible<E, M> =>
        new EffectTFail<E, M, A>(value);
    
    /// <summary>
    /// Create an effect that always fails
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> error<M, A>(Error value) 
        where M : Monad<M>, Fallible<M> =>
        new EffectTFail<Error, M, A>(value);
        
    /// <summary>
    /// Create an effect that yields nothing at all
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> empty<M, A>() 
        where M : Monad<M>, MonoidK<M> =>
        EffectTEmpty<M, A>.Default;

    /// <summary>
    /// Create an effect that simply returns the bound value of the lifted monad
    /// </summary>
    /// <typeparam name="M">Lifted monad type</typeparam>
    /// <typeparam name="A">Bound value type</typeparam>
    /// <returns></returns>
    public static EffectT<M, A> liftM<M, A>(K<M, A> value) 
        where M : Monad<M> =>
        new EffectTLiftM<A, M, A>(value, pure<M, A>);
}

/// <summary>
/// `ProducerT` streaming producer monad-transformer instance
/// </summary>
public abstract record ProducerT<OUT, M, A>
    where M : Monad<M>
{
    public EffectT<M, A> Compose(ConsumerT<OUT, M, A> consumer) =>
        consumer.ReplaceAwait(this);
    
    public abstract ProducerT<OUT, M, B> Map<B>(Func<A, B> f);
    public abstract ProducerT<OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f);
    public abstract ProducerT<OUT, M, B> ApplyBack<B>(ProducerT<OUT, M, Func<A, B>> ff);
    public abstract ProducerT<OUT, M, B> Action<B>(ProducerT<OUT, M, B> fb);
    public abstract ProducerT<OUT, M, B> Bind<B>(Func<A, ProducerT<OUT, M, B>> f);
    public ProducerT<OUT, M, B> Bind<B>(Func<A, K<M, B>> f) => Bind(x => ProducerT.liftM<OUT, M, B>(f(x))); 

    internal abstract K<M, A> Run();
    internal abstract EffectT<M, A> ReplaceYield(Func<OUT, ConsumerT<OUT, M, A>> consumer);
     
    public ProducerT<OUT, M, B> Select<B>(Func<A, B> f) =>
        Map(f);
   
    public ProducerT<OUT, M, C> SelectMany<B, C>(Func<A, ProducerT<OUT, M, B>> f, Func<A, B, C> g) =>
        Bind(x => f(x).Map(y => g(x, y)));
   
    public ProducerT<OUT, M, C> SelectMany<B, C>(Func<A, K<M, B>> f, Func<A, B, C> g) =>
        Bind(x => f(x).Map(y => g(x, y)));
}

record ProducerTEmpty<OUT, M, A> : ProducerT<OUT, M, A>
    where M : Monad<M>, MonoidK<M>
{
    public static readonly ProducerTEmpty<OUT, M, A> Default = new ();
    
    public override ProducerT<OUT, M, B> Map<B>(Func<A, B> f) =>
        ProducerTEmpty<OUT, M, B>.Default;

    public override ProducerT<OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        ProducerTEmpty<OUT, M, B>.Default;
    
    public override ProducerT<OUT, M, B> ApplyBack<B>(ProducerT<OUT, M, Func<A, B>> ff) =>
        ProducerTEmpty<OUT, M, B>.Default;
    
    public override ProducerT<OUT, M, B> Action<B>(ProducerT<OUT, M, B> fb) =>
        ProducerTEmpty<OUT, M, B>.Default;

    public override ProducerT<OUT, M, B> Bind<B>(Func<A, ProducerT<OUT, M, B>> f) =>
        ProducerTEmpty<OUT, M, B>.Default;

    internal override EffectT<M, A> ReplaceYield(Func<OUT, ConsumerT<OUT, M, A>> consumer) =>
        EffectT.empty<M, A>();

    internal override K<M, A> Run() =>
        MonoidK.empty<M, A>();
}

record ProducerTPure<OUT, M, A>(A Value) : ProducerT<OUT, M, A>
    where M : Monad<M>
{
    public override ProducerT<OUT, M, B> Map<B>(Func<A, B> f) =>
        new ProducerTPure<OUT, M, B>(f(Value));

    public override ProducerT<OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        ProducerT.liftM<OUT, M, A>(M.Pure(Value)).MapM(f);
    
    public override ProducerT<OUT, M, B> ApplyBack<B>(ProducerT<OUT, M, Func<A, B>> ff) =>
        ff.Map(f => f(Value));

    public override ProducerT<OUT, M, B> Action<B>(ProducerT<OUT, M, B> fb) =>
        fb;

    public override ProducerT<OUT, M, B> Bind<B>(Func<A, ProducerT<OUT, M, B>> f) =>
        f(Value);

    internal override EffectT<M, A> ReplaceYield(Func<OUT, ConsumerT<OUT, M, A>> consumer) =>
        EffectT.pure<M, A>(Value);

    internal override K<M, A> Run() =>
        M.Pure(Value);
}

record ProducerTFail<OUT, E, M, A>(E Value) : ProducerT<OUT, M, A>
    where M : Monad<M>, Fallible<E, M>
{
    public override ProducerT<OUT, M, B> Map<B>(Func<A, B> f) =>
        new ProducerTFail<OUT, E, M, B>(Value);

    public override ProducerT<OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new ProducerTFail<OUT, E, M, B>(Value);
    
    public override ProducerT<OUT, M, B> ApplyBack<B>(ProducerT<OUT, M, Func<A, B>> ff) =>
        new ProducerTFail<OUT, E, M, B>(Value);

    public override ProducerT<OUT, M, B> Action<B>(ProducerT<OUT, M, B> fb) =>
        new ProducerTFail<OUT, E, M, B>(Value);

    public override ProducerT<OUT, M, B> Bind<B>(Func<A, ProducerT<OUT, M, B>> f) =>
        new ProducerTFail<OUT, E, M, B>(Value);

    internal override EffectT<M, A> ReplaceYield(Func<OUT, ConsumerT<OUT, M, A>> consumer) =>
        EffectT.fail<E, M, A>(Value);

    internal override K<M, A> Run() =>
        M.Fail<A>(Value);
}

record ProducerTYield<OUT, M, A>(OUT Value, Func<Unit, ProducerT<OUT, M, A>> Next) : ProducerT<OUT, M, A>
    where M : Monad<M>
{
    public override ProducerT<OUT, M, B> Map<B>(Func<A, B> f) =>
        new ProducerTYield<OUT, M, B>(Value, x => Next(x).Map(f));
    
    public override ProducerT<OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new ProducerTYield<OUT, M, B>(Value, x => Next(x).MapM(f));
    
    public override ProducerT<OUT, M, B> ApplyBack<B>(ProducerT<OUT, M, Func<A, B>> ff) =>
        new ProducerTYield<OUT, M, B>(Value, x => Next(x).ApplyBack(ff));
    
    public override ProducerT<OUT, M, B> Action<B>(ProducerT<OUT, M, B> fb) =>
        new ProducerTYield<OUT, M, B>(Value, x => Next(x).Action(fb));

    public override ProducerT<OUT, M, B> Bind<B>(Func<A, ProducerT<OUT, M, B>> f) =>
        new ProducerTYield<OUT, M, B>(Value, x => Next(x).Bind(f));

    internal override EffectT<M, A> ReplaceYield(Func<OUT, ConsumerT<OUT, M, A>> consumer) =>
        consumer(Value).ReplaceAwait(Next(default));

    internal override K<M, A> Run() =>
        throw new InvalidOperationException("yield/await pairing not found");
}

record ProducerTLiftM<OUT, X, M, A>(K<M, X> Value, Func<X, ProducerT<OUT, M, A>> Next) : ProducerT<OUT, M, A>
    where M : Monad<M>
{
    public override ProducerT<OUT, M, B> Map<B>(Func<A, B> f) =>
        new ProducerTLiftM<OUT, X, M, B>(Value, x => Next(x).Map(f));
    
    public override ProducerT<OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new ProducerTLiftM<OUT, X, M, B>(Value, x => Next(x).MapM(f));
    
    public override ProducerT<OUT, M, B> ApplyBack<B>(ProducerT<OUT, M, Func<A, B>> ff) =>
        new ProducerTLiftM<OUT, X, M, B>(Value, x => Next(x).ApplyBack(ff));
    
    public override ProducerT<OUT, M, B> Action<B>(ProducerT<OUT, M, B> fb) =>
        new ProducerTLiftM<OUT, X, M, B>(Value, x => Next(x).Action(fb));

    public override ProducerT<OUT, M, B> Bind<B>(Func<A, ProducerT<OUT, M, B>> f) =>
        new ProducerTLiftM<OUT, X, M, B>(Value, x => Next(x).Bind(f));

    internal override EffectT<M, A> ReplaceYield(Func<OUT, ConsumerT<OUT, M, A>> consumer) =>
        new EffectTLiftM<X, M, A>(Value, x => Next(x).ReplaceYield(consumer));

    internal override K<M, A> Run() =>
        Value.Bind(v => Next(v).Run());
}

record ProducerTEnumerableYield<OUT, M, A>(IEnumerable<OUT> Values, Func<Unit, ProducerT<OUT, M, A>> Next) : 
    ProducerT<OUT, M, A>
    where M : Monad<M>, Alternative<M>
{
    public override ProducerT<OUT, M, B> Map<B>(Func<A, B> f) =>
        new ProducerTEnumerableYield<OUT, M, B>(Values, x => Next(x).Map(f));
    
    public override ProducerT<OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new ProducerTEnumerableYield<OUT, M, B>(Values, x => Next(x).MapM(f));
    
    public override ProducerT<OUT, M, B> ApplyBack<B>(ProducerT<OUT, M, Func<A, B>> ff) =>
        new ProducerTEnumerableYield<OUT, M, B>(Values, x => Next(x).ApplyBack(ff));
    
    public override ProducerT<OUT, M, B> Action<B>(ProducerT<OUT, M, B> fb) =>
        new ProducerTEnumerableYield<OUT, M, B>(Values, x => Next(x).Action(fb));

    public override ProducerT<OUT, M, B> Bind<B>(Func<A, ProducerT<OUT, M, B>> f) =>
        new ProducerTEnumerableYield<OUT, M, B>(Values, x => Next(x).Bind(f));

    internal override EffectT<M, A> ReplaceYield(Func<OUT, ConsumerT<OUT, M, A>> consumer) =>
        new EffectTManyEnumerable<A, M, A>(
            Values.Select(v => new ProducerTYield<OUT, M, A>(v, _ => ProducerT.pure<OUT, M, A>(default!)).ReplaceYield(consumer)),
            x => Next(x).ReplaceYield(consumer));

    internal override K<M, A> Run() =>
        throw new InvalidOperationException("yield/await pairing not found");
}

record ProducerTAsyncEnumerableYield<OUT, M, A>(IAsyncEnumerable<OUT> Values, Func<Unit, ProducerT<OUT, M, A>> Next) : 
    ProducerT<OUT, M, A>
    where M : Monad<M>, Alternative<M>
{
    public override ProducerT<OUT, M, B> Map<B>(Func<A, B> f) =>
        new ProducerTAsyncEnumerableYield<OUT, M, B>(Values, x => Next(x).Map(f));
    
    public override ProducerT<OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new ProducerTAsyncEnumerableYield<OUT, M, B>(Values, x => Next(x).MapM(f));
    
    public override ProducerT<OUT, M, B> ApplyBack<B>(ProducerT<OUT, M, Func<A, B>> ff) =>
        new ProducerTAsyncEnumerableYield<OUT, M, B>(Values, x => Next(x).ApplyBack(ff));
    
    public override ProducerT<OUT, M, B> Action<B>(ProducerT<OUT, M, B> fb) =>
        new ProducerTAsyncEnumerableYield<OUT, M, B>(Values, x => Next(x).Action(fb));

    public override ProducerT<OUT, M, B> Bind<B>(Func<A, ProducerT<OUT, M, B>> f) =>
        new ProducerTAsyncEnumerableYield<OUT, M, B>(Values, x => Next(x).Bind(f));

    internal override EffectT<M, A> ReplaceYield(Func<OUT, ConsumerT<OUT, M, A>> consumer) =>
        new EffectTManyAsyncEnumerable<A, M, A>(
            Values.Select(v => new ProducerTYield<OUT, M, A>(v, _ => ProducerT.pure<OUT, M, A>(default!)).ReplaceYield(consumer)),
            x => Next(x).ReplaceYield(consumer));

    internal override K<M, A> Run() =>
        throw new InvalidOperationException("yield/await pairing not found");
}

public abstract record ConsumerT<IN, M, A>
    where M : Monad<M>
{
    public abstract ConsumerT<IN, M, B> Map<B>(Func<A, B> f);
    public abstract ConsumerT<IN, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f);
    public abstract ConsumerT<IN, M, B> ApplyBack<B>(ConsumerT<IN, M, Func<A, B>> ff);
    public abstract ConsumerT<IN, M, B> Action<B>(ConsumerT<IN, M, B> fb);
    public abstract ConsumerT<IN, M, B> Bind<B>(Func<A, ConsumerT<IN, M, B>> f);
    public ConsumerT<IN, M, B> Bind<B>(Func<A, K<M, B>> f) => Bind(x => ConsumerT.liftM<IN, M, B>(f(x))); 

    internal abstract EffectT<M, A> ReplaceAwait(ProducerT<IN, M, A> produceBefore);
     
    public ConsumerT<IN, M, B> Select<B>(Func<A, B> f) =>
        Map(f);
   
    public ConsumerT<IN, M, C> SelectMany<B, C>(Func<A, ConsumerT<IN, M, B>> f, Func<A, B, C> g) =>
        Bind(x => f(x).Map(y => g(x, y)));
   
    public ConsumerT<IN, M, C> SelectMany<B, C>(Func<A, K<M, B>> f, Func<A, B, C> g) =>
        Bind(x => f(x).Map(y => g(x, y)));
}

record ConsumerTPure<IN, M, A>(A Value) : ConsumerT<IN, M, A>
    where M : Monad<M>
{
    public override ConsumerT<IN, M, B> Map<B>(Func<A, B> f) =>
        new ConsumerTPure<IN, M, B>(f(Value));

    public override ConsumerT<IN, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        ConsumerT.liftM<IN, M, A>(M.Pure(Value)).MapM(f);
    
    public override ConsumerT<IN, M, B> ApplyBack<B>(ConsumerT<IN, M, Func<A, B>> ff) =>
        ff.Map(f => f(Value));

    public override ConsumerT<IN, M, B> Action<B>(ConsumerT<IN, M, B> fb) =>
        fb;

    public override ConsumerT<IN, M, B> Bind<B>(Func<A, ConsumerT<IN, M, B>> f) =>
        f(Value);

    internal override EffectT<M, A> ReplaceAwait(ProducerT<IN, M, A> produceBefore) =>
        EffectT.liftM(produceBefore.Run().Map(_ => Value));
}

record ConsumerTFail<IN, E, M, A>(E Value) : ConsumerT<IN, M, A>
    where M : Monad<M>, Fallible<E, M>
{
    public override ConsumerT<IN, M, B> Map<B>(Func<A, B> f) =>
        new ConsumerTFail<IN, E, M, B>(Value);

    public override ConsumerT<IN, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new ConsumerTFail<IN, E, M, B>(Value);
    
    public override ConsumerT<IN, M, B> ApplyBack<B>(ConsumerT<IN, M, Func<A, B>> ff) =>
        new ConsumerTFail<IN, E, M, B>(Value);

    public override ConsumerT<IN, M, B> Action<B>(ConsumerT<IN, M, B> fb) =>
        new ConsumerTFail<IN, E, M, B>(Value);

    public override ConsumerT<IN, M, B> Bind<B>(Func<A, ConsumerT<IN, M, B>> f) =>
        new ConsumerTFail<IN, E, M, B>(Value);

    internal override EffectT<M, A> ReplaceAwait(ProducerT<IN, M, A> produceBefore) =>
        EffectT.liftM(produceBefore.Run().Bind(_ => M.Fail<A>(Value)));
}

record ConsumerTLiftM<IN, X, M, A>(K<M, X> Value, Func<X, ConsumerT<IN, M, A>> Next) : ConsumerT<IN, M, A>
    where M : Monad<M>
{
    public override ConsumerT<IN, M, B> Map<B>(Func<A, B> f) =>
        new ConsumerTLiftM<IN, X, M, B>(Value, x => Next(x).Map(f));

    public override ConsumerT<IN, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new ConsumerTLiftM<IN, X, M, B>(Value, x => Next(x).MapM(f));
    
    public override ConsumerT<IN, M, B> ApplyBack<B>(ConsumerT<IN, M, Func<A, B>> ff) =>
        new ConsumerTLiftM<IN, X, M, B>(Value, x => Next(x).ApplyBack(ff));
    
    public override ConsumerT<IN, M, B> Action<B>(ConsumerT<IN, M, B> fb) =>
        new ConsumerTLiftM<IN, X, M, B>(Value, x => Next(x).Action(fb));

    public override ConsumerT<IN, M, B> Bind<B>(Func<A, ConsumerT<IN, M, B>> f) =>
        new ConsumerTLiftM<IN, X, M, B>(Value, x => Next(x).Bind(f));

    internal override EffectT<M, A> ReplaceAwait(ProducerT<IN, M, A> produceBefore) =>
        new EffectTLiftM<X, M, A>(Value, x => Next(x).ReplaceAwait(produceBefore));
}

record ConsumerTAwait<IN, M, A>(Func<IN, ConsumerT<IN, M, A>> Next) : ConsumerT<IN, M, A>
    where M : Monad<M>
{
    public override ConsumerT<IN, M, B> Map<B>(Func<A, B> f) =>
        new ConsumerTAwait<IN, M, B>(x => Next(x).Map(f));

    public override ConsumerT<IN, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new ConsumerTAwait<IN, M, B>(x => Next(x).MapM(f));
    
    public override ConsumerT<IN, M, B> ApplyBack<B>(ConsumerT<IN, M, Func<A, B>> ff) =>
        new ConsumerTAwait<IN, M, B>(x => Next(x).ApplyBack(ff));
    
    public override ConsumerT<IN, M, B> Action<B>(ConsumerT<IN, M, B> fb) =>
        new ConsumerTAwait<IN, M, B>(x => Next(x).Action(fb));

    public override ConsumerT<IN, M, B> Bind<B>(Func<A, ConsumerT<IN, M, B>> f) =>
        new ConsumerTAwait<IN, M, B>(x => Next(x).Bind(f));

    internal override EffectT<M, A> ReplaceAwait(ProducerT<IN, M, A> produceBefore) =>
        produceBefore.ReplaceYield(Next);
}

public abstract record EffectT<M, A>
    where M : Monad<M>
{
    public abstract EffectT<M, B> Map<B>(Func<A, B> f);
    public abstract EffectT<M, B> MapM<B>(Func<K<M, A>, K<M, B>> f);
    public abstract EffectT<M, B> ApplyBack<B>(EffectT<M, Func<A, B>> ff);
    public abstract EffectT<M, B> Action<B>(EffectT<M, B> fb);
    public abstract EffectT<M, B> Bind<B>(Func<A, EffectT<M, B>> f);
    public EffectT<M, B> Bind<B>(Func<A, K<M, B>> f) => Bind(x => EffectT.liftM(f(x))); 
    public abstract K<M, A> Run();

    public EffectT<M, B> Select<B>(Func<A, B> f) =>
        Map(f);
   
    public EffectT<M, C> SelectMany<B, C>(Func<A, EffectT<M, B>> f, Func<A, B, C> g) =>
        Bind(x => f(x).Map(y => g(x, y)));
   
    public EffectT<M, C> SelectMany<B, C>(Func<A, K<M, B>> f, Func<A, B, C> g) =>
        Bind(x => f(x).Map(y => g(x, y)));
}

record EffectTEmpty<M, A> : EffectT<M, A>
    where M : Monad<M>, MonoidK<M>
{
    public static readonly EffectT<M, A> Default = new EffectTEmpty<M, A>();
    
    public override EffectT<M, B> Map<B>(Func<A, B> f) =>
        EffectTEmpty<M, B>.Default;
    
    public override EffectT<M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        EffectTEmpty<M, B>.Default;
    
    public override EffectT<M, B> ApplyBack<B>(EffectT<M, Func<A, B>> ff) =>
        EffectTEmpty<M, B>.Default;

    public override EffectT<M, B> Action<B>(EffectT<M, B> fb) =>
        EffectTEmpty<M, B>.Default;

    public override EffectT<M, B> Bind<B>(Func<A, EffectT<M, B>> f) =>
        EffectTEmpty<M, B>.Default;

    public override K<M, A> Run() =>
        M.Empty<A>();
}

record EffectTPure<M, A>(A Value) : EffectT<M, A>
    where M : Monad<M>
{
    public override EffectT<M, B> Map<B>(Func<A, B> f) =>
        new EffectTPure<M, B>(f(Value));
    
    public override EffectT<M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        EffectT.liftM(M.Pure(Value)).MapM(f);
    
    public override EffectT<M, B> ApplyBack<B>(EffectT<M, Func<A, B>> ff) =>
        ff.Map(f => f(Value));

    public override EffectT<M, B> Action<B>(EffectT<M, B> fb) =>
        fb;

    public override EffectT<M, B> Bind<B>(Func<A, EffectT<M, B>> f) =>
        f(Value);

    public override K<M, A> Run() =>
        M.Pure(Value);
}

record EffectTFail<E, M, A>(E Value) : EffectT<M, A>
    where M : Monad<M>, Fallible<E, M>
{
    public override EffectT<M, B> Map<B>(Func<A, B> f) =>
        new EffectTFail<E, M, B>(Value);
    
    public override EffectT<M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new EffectTFail<E, M, B>(Value);
    
    public override EffectT<M, B> ApplyBack<B>(EffectT<M, Func<A, B>> ff) =>
        new EffectTFail<E, M, B>(Value);

    public override EffectT<M, B> Action<B>(EffectT<M, B> fb) =>
        new EffectTFail<E, M, B>(Value);

    public override EffectT<M, B> Bind<B>(Func<A, EffectT<M, B>> f) =>
        new EffectTFail<E, M, B>(Value);

    public override K<M, A> Run() =>
        M.Fail<A>(Value);
}

record EffectTLiftM<X, M, A>(K<M, X> Value, Func<X, EffectT<M, A>> Next) : EffectT<M, A>
    where M : Monad<M>
{
    public override EffectT<M, B> Map<B>(Func<A, B> f) =>
        new EffectTLiftM<X, M, B>(Value, x => Next(x).Map(f));

    public override EffectT<M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new EffectTLiftM<X, M, B>(Value, x => Next(x).MapM(f));

    public override EffectT<M, B> ApplyBack<B>(EffectT<M, Func<A, B>> ff) =>
        new EffectTLiftM<X, M, B>(Value, x => Next(x).ApplyBack(ff));

    public override EffectT<M, B> Action<B>(EffectT<M, B> fb) =>
        new EffectTLiftM<X, M, B>(Value, x => Next(x).Action(fb));

    public override EffectT<M, B> Bind<B>(Func<A, EffectT<M, B>> f) =>
        new EffectTLiftM<X, M, B>(Value, x => Next(x).Bind(f));

    public override K<M, A> Run() =>
        Value.Bind(x => Next(x).Run());
}

record EffectTManyEnumerable<X, M, A>(IEnumerable<EffectT<M, X>> Values, Func<Unit, EffectT<M, A>> Next) : EffectT<M, A>
    where M : Monad<M>, Alternative<M>
{
    public override EffectT<M, B> Map<B>(Func<A, B> f) =>
        new EffectTManyEnumerable<X, M, B>(Values, x => Next(x).Map(f));

    public override EffectT<M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new EffectTManyEnumerable<X, M, B>(Values, x => Next(x).MapM(f));

    public override EffectT<M, B> ApplyBack<B>(EffectT<M, Func<A, B>> ff) =>
        new EffectTManyEnumerable<X, M, B>(Values, x => Next(x).ApplyBack(ff));

    public override EffectT<M, B> Action<B>(EffectT<M, B> fb) =>
        new EffectTManyEnumerable<X, M, B>(Values, x => Next(x).Action(fb));

    public override EffectT<M, B> Bind<B>(Func<A, EffectT<M, B>> f) =>
        new EffectTManyEnumerable<X, M, B>(Values, x => Next(x).Bind(f));

    public override K<M, A> Run() =>
        M.Action(M.Actions(Values.Select(mv => mv.Run())), Next(default).Run());
}

record EffectTManyAsyncEnumerable<X, M, A>(IAsyncEnumerable<EffectT<M, X>> Values, Func<Unit, EffectT<M, A>> Next) : EffectT<M, A>
    where M : Monad<M>, Alternative<M>
{
    public override EffectT<M, B> Map<B>(Func<A, B> f) =>
        new EffectTManyAsyncEnumerable<X, M, B>(Values, x => Next(x).Map(f));

    public override EffectT<M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new EffectTManyAsyncEnumerable<X, M, B>(Values, x => Next(x).MapM(f));

    public override EffectT<M, B> ApplyBack<B>(EffectT<M, Func<A, B>> ff) =>
        new EffectTManyAsyncEnumerable<X, M, B>(Values, x => Next(x).ApplyBack(ff));

    public override EffectT<M, B> Action<B>(EffectT<M, B> fb) =>
        new EffectTManyAsyncEnumerable<X, M, B>(Values, x => Next(x).Action(fb));

    public override EffectT<M, B> Bind<B>(Func<A, EffectT<M, B>> f) =>
        new EffectTManyAsyncEnumerable<X, M, B>(Values, x => Next(x).Bind(f));

    public override K<M, A> Run() =>
        M.Action(M.Actions(Values.Select(mv => mv.Run())), Next(default).Run());
}
