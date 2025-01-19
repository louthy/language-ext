using LanguageExt.Async.Linq;
using LanguageExt.Traits;

namespace LanguageExt.Pipes2;

record PipeTEmpty<IN, OUT, M, A> : PipeT<IN, OUT, M, A>
    where M : Monad<M>, MonoidK<M>
{
    public static readonly PipeT<IN, OUT, M, A> Default = new PipeTEmpty<IN, OUT, M, A>();
    
    public override PipeT<IN, OUT, M, B> Map<B>(Func<A, B> f) =>
        PipeTEmpty<IN, OUT, M, B>.Default;

    public override PipeT<IN, OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        PipeTEmpty<IN, OUT, M, B>.Default;
    
    public override PipeT<IN, OUT, M, B> ApplyBack<B>(PipeT<IN, OUT, M, Func<A, B>> ff) =>
        PipeTEmpty<IN, OUT, M, B>.Default;
    
    public override PipeT<IN, OUT, M, B> Action<B>(PipeT<IN, OUT, M, B> fb) =>
        PipeTEmpty<IN, OUT, M, B>.Default;

    public override PipeT<IN, OUT, M, B> Bind<B>(Func<A, PipeT<IN, OUT, M, B>> f) =>
        PipeTEmpty<IN, OUT, M, B>.Default;

    internal override PipeT<IN1, OUT, M, A> ReplaceAwait<IN1>(Func<PipeT<IN1, OUT, M, IN>> request) => 
        PipeT.empty<IN1, OUT, M, A>();

    internal override PipeT<IN, OUT1, M, A> ReplaceYield<OUT1>(Func<OUT, PipeT<IN, OUT1, M, Unit>> response) => 
        PipeT.empty<IN, OUT1, M, A>();
    
    internal override PipeT<IN1, OUT, M, A> PairEachAwaitWithYield<IN1>(Func<Unit, PipeT<IN1, IN, M, A>> request) => 
        PipeT.empty<IN1, OUT, M, A>();

    internal override PipeT<IN, OUT1, M, A> PairEachYieldWithAwait<OUT1>(Func<OUT, PipeT<OUT, OUT1, M, A>> response) =>
        PipeT.empty<IN, OUT1, M, A>();
    
    internal override K<M, A> Run() =>
        M.Empty<A>();
}

record PipeTPure<IN, OUT, M, A>(A Value) : PipeT<IN, OUT, M, A>
    where M : Monad<M>
{
    public override PipeT<IN, OUT, M, B> Map<B>(Func<A, B> f) =>
        PipeT.pure<IN, OUT, M, B>(f(Value));

    public override PipeT<IN, OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        PipeT.liftM<IN, OUT, M, B>(f(M.Pure(Value)));
    
    public override PipeT<IN, OUT, M, B> ApplyBack<B>(PipeT<IN, OUT, M, Func<A, B>> ff) =>
        ff.Map(f => f(Value));

    public override PipeT<IN, OUT, M, B> Action<B>(PipeT<IN, OUT, M, B> fb) =>
        fb;

    public override PipeT<IN, OUT, M, B> Bind<B>(Func<A, PipeT<IN, OUT, M, B>> f) =>
        f(Value);

    internal override PipeT<IN1, OUT, M, A> ReplaceAwait<IN1>(Func<PipeT<IN1, OUT, M, IN>> request) => 
        PipeT.pure<IN1, OUT, M, A>(Value);

    internal override PipeT<IN, OUT1, M, A> ReplaceYield<OUT1>(Func<OUT, PipeT<IN, OUT1, M, Unit>> response) => 
        PipeT.pure<IN, OUT1, M, A>(Value);
    
    internal override PipeT<IN1, OUT, M, A> PairEachAwaitWithYield<IN1>(Func<Unit, PipeT<IN1, IN, M, A>> request) => 
        PipeT.pure<IN1, OUT, M, A>(Value);

    internal override PipeT<IN, OUT1, M, A> PairEachYieldWithAwait<OUT1>(Func<OUT, PipeT<OUT, OUT1, M, A>> response) =>
        PipeT.pure<IN, OUT1, M, A>(Value);

    internal override K<M, A> Run() =>
        M.Pure(Value);
}

record PipeTFail<IN, OUT, E, M, A>(E Value) : PipeT<IN, OUT, M, A>
    where M : Monad<M>, Fallible<E, M>
{
    public override PipeT<IN, OUT, M, B> Map<B>(Func<A, B> f) =>
        PipeT.fail<IN, OUT, E, M, B>(Value);

    public override PipeT<IN, OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        PipeT.fail<IN, OUT, E, M, B>(Value);
    
    public override PipeT<IN, OUT, M, B> ApplyBack<B>(PipeT<IN, OUT, M, Func<A, B>> ff) =>
        PipeT.fail<IN, OUT, E, M, B>(Value);
    
    public override PipeT<IN, OUT, M, B> Action<B>(PipeT<IN, OUT, M, B> fb) =>
        PipeT.fail<IN, OUT, E, M, B>(Value);

    public override PipeT<IN, OUT, M, B> Bind<B>(Func<A, PipeT<IN, OUT, M, B>> f) =>
        PipeT.fail<IN, OUT, E, M, B>(Value);

    internal override PipeT<IN1, OUT, M, A> ReplaceAwait<IN1>(Func<PipeT<IN1, OUT, M, IN>> request) => 
        PipeT.fail<IN1, OUT, E, M, A>(Value);

    internal override PipeT<IN, OUT1, M, A> ReplaceYield<OUT1>(Func<OUT, PipeT<IN, OUT1, M, Unit>> response) => 
        PipeT.fail<IN, OUT1, E, M, A>(Value);
    
    internal override PipeT<IN1, OUT, M, A> PairEachAwaitWithYield<IN1>(Func<Unit, PipeT<IN1, IN, M, A>> request) => 
        PipeT.fail<IN1, OUT, E, M, A>(Value);

    internal override PipeT<IN, OUT1, M, A> PairEachYieldWithAwait<OUT1>(Func<OUT, PipeT<OUT, OUT1, M, A>> response) =>
        PipeT.fail<IN, OUT1, E, M, A>(Value);

    internal override K<M, A> Run() =>
        M.Fail<A>(Value);
}

record PipeTLazy<IN, OUT, M, A>(Func<PipeT<IN, OUT, M, A>> Acquire) : PipeT<IN, OUT, M, A>
    where M : Monad<M>
{
    public override PipeT<IN, OUT, M, B> Map<B>(Func<A, B> f) =>
        new PipeTLazy<IN, OUT, M, B>(() => Acquire().Map(f));

    public override PipeT<IN, OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new PipeTLazy<IN, OUT, M, B>(() => Acquire().MapM(f));
    
    public override PipeT<IN, OUT, M, B> ApplyBack<B>(PipeT<IN, OUT, M, Func<A, B>> ff) =>
        new PipeTLazy<IN, OUT, M, B>(() => Acquire().ApplyBack(ff));

    public override PipeT<IN, OUT, M, B> Action<B>(PipeT<IN, OUT, M, B> fb) =>
        new PipeTLazy<IN, OUT, M, B>(() => Acquire().Action(fb));

    public override PipeT<IN, OUT, M, B> Bind<B>(Func<A, PipeT<IN, OUT, M, B>> f) =>
        new PipeTLazy<IN, OUT, M, B>(() => Acquire().Bind(f));

    internal override PipeT<IN1, OUT, M, A> ReplaceAwait<IN1>(Func<PipeT<IN1, OUT, M, IN>> request) => 
        new PipeTLazy<IN1, OUT, M, A>(() => Acquire().ReplaceAwait(request));

    internal override PipeT<IN, OUT1, M, A> ReplaceYield<OUT1>(Func<OUT, PipeT<IN, OUT1, M, Unit>> response) => 
        new PipeTLazy<IN, OUT1, M, A>(() => Acquire().ReplaceYield(response));
    
    internal override PipeT<IN1, OUT, M, A> PairEachAwaitWithYield<IN1>(Func<Unit, PipeT<IN1, IN, M, A>> request) => 
        new PipeTLazy<IN1, OUT, M, A>(() => Acquire().PairEachAwaitWithYield(request));

    internal override PipeT<IN, OUT1, M, A> PairEachYieldWithAwait<OUT1>(Func<OUT, PipeT<OUT, OUT1, M, A>> response) =>
        new PipeTLazy<IN, OUT1, M, A>(() => Acquire().PairEachYieldWithAwait(response));

    internal override K<M, A> Run() =>
        Acquire().Run();
}

record PipeTAsync<IN, OUT, M, A>(ValueTask<PipeT<IN, OUT, M, A>> Acquire) : PipeT<IN, OUT, M, A>
    where M : Monad<M>
{
    public override PipeT<IN, OUT, M, B> Map<B>(Func<A, B> f) =>
        new PipeTAsync<IN, OUT, M, B>(Acquire.Map(t => t.Map(f)));

    public override PipeT<IN, OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new PipeTAsync<IN, OUT, M, B>(Acquire.Map(t => t.MapM(f)));
    
    public override PipeT<IN, OUT, M, B> ApplyBack<B>(PipeT<IN, OUT, M, Func<A, B>> ff) =>
        new PipeTAsync<IN, OUT, M, B>(Acquire.Map(t => t.ApplyBack(ff)));

    public override PipeT<IN, OUT, M, B> Action<B>(PipeT<IN, OUT, M, B> fb) =>
        new PipeTAsync<IN, OUT, M, B>(Acquire.Map(t => t.Action(fb)));

    public override PipeT<IN, OUT, M, B> Bind<B>(Func<A, PipeT<IN, OUT, M, B>> f) =>
        new PipeTAsync<IN, OUT, M, B>(Acquire.Map(t => t.Bind(f)));

    internal override PipeT<IN1, OUT, M, A> ReplaceAwait<IN1>(Func<PipeT<IN1, OUT, M, IN>> request) => 
        new PipeTAsync<IN1, OUT, M, A>(Acquire.Map(t => t.ReplaceAwait(request)));

    internal override PipeT<IN, OUT1, M, A> ReplaceYield<OUT1>(Func<OUT, PipeT<IN, OUT1, M, Unit>> response) => 
        new PipeTAsync<IN, OUT1, M, A>(Acquire.Map(t => t.ReplaceYield(response)));
    
    internal override PipeT<IN1, OUT, M, A> PairEachAwaitWithYield<IN1>(Func<Unit, PipeT<IN1, IN, M, A>> request) => 
        new PipeTAsync<IN1, OUT, M, A>(Acquire.Map(t => t.PairEachAwaitWithYield(request)));

    internal override PipeT<IN, OUT1, M, A> PairEachYieldWithAwait<OUT1>(Func<OUT, PipeT<OUT, OUT1, M, A>> response) =>
        new PipeTAsync<IN, OUT1, M, A>(Acquire.Map(t => t.PairEachYieldWithAwait(response)));

    internal override K<M, A> Run() =>
        Acquire.GetAwaiter().GetResult().Run();

    internal override async ValueTask<K<M, A>> RunAsync()
    {
        var proxy = await Acquire;
        return await proxy.RunAsync();
    }
}

record PipeTLazyAsync<IN, OUT, M, A>(Func<ValueTask<PipeT<IN, OUT, M, A>>> Acquire) : PipeT<IN, OUT, M, A>
    where M : Monad<M>
{
    public override PipeT<IN, OUT, M, B> Map<B>(Func<A, B> f) =>
        new PipeTLazyAsync<IN, OUT, M, B>(() => Acquire().Map(t => t.Map(f)));

    public override PipeT<IN, OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new PipeTLazyAsync<IN, OUT, M, B>(() => Acquire().Map(t => t.MapM(f)));
    
    public override PipeT<IN, OUT, M, B> ApplyBack<B>(PipeT<IN, OUT, M, Func<A, B>> ff) =>
        new PipeTLazyAsync<IN, OUT, M, B>(() => Acquire().Map(t => t.ApplyBack(ff)));

    public override PipeT<IN, OUT, M, B> Action<B>(PipeT<IN, OUT, M, B> fb) =>
        new PipeTLazyAsync<IN, OUT, M, B>(() => Acquire().Map(t => t.Action(fb)));

    public override PipeT<IN, OUT, M, B> Bind<B>(Func<A, PipeT<IN, OUT, M, B>> f) =>
        new PipeTLazyAsync<IN, OUT, M, B>(() => Acquire().Map(t => t.Bind(f)));

    internal override PipeT<IN1, OUT, M, A> ReplaceAwait<IN1>(Func<PipeT<IN1, OUT, M, IN>> request) => 
        new PipeTLazyAsync<IN1, OUT, M, A>(() => Acquire().Map(t => t.ReplaceAwait(request)));

    internal override PipeT<IN, OUT1, M, A> ReplaceYield<OUT1>(Func<OUT, PipeT<IN, OUT1, M, Unit>> response) => 
        new PipeTLazyAsync<IN, OUT1, M, A>(() => Acquire().Map(t => t.ReplaceYield(response)));
    
    internal override PipeT<IN1, OUT, M, A> PairEachAwaitWithYield<IN1>(Func<Unit, PipeT<IN1, IN, M, A>> request) => 
        new PipeTLazyAsync<IN1, OUT, M, A>(() => Acquire().Map(t => t.PairEachAwaitWithYield(request)));

    internal override PipeT<IN, OUT1, M, A> PairEachYieldWithAwait<OUT1>(Func<OUT, PipeT<OUT, OUT1, M, A>> response) =>
        new PipeTLazyAsync<IN, OUT1, M, A>(() => Acquire().Map(t => t.PairEachYieldWithAwait(response)));

    internal override K<M, A> Run() =>
        Acquire().GetAwaiter().GetResult().Run();

    internal override async ValueTask<K<M, A>> RunAsync()
    {
        var proxy = await Acquire();
        return await proxy.RunAsync();
    }
}

record PipeTLift<IN, OUT, X, M, A>(Func<X> Acquire, Func<X, PipeT<IN, OUT, M, A>> Next) : PipeT<IN, OUT, M, A>
    where M : Monad<M>
{
    public override PipeT<IN, OUT, M, B> Map<B>(Func<A, B> f) =>
        new PipeTLift<IN, OUT, X, M, B>(Acquire, x => Next(x).Map(f));

    public override PipeT<IN, OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new PipeTLift<IN, OUT, X, M, B>(Acquire, x => Next(x).MapM(f));
    
    public override PipeT<IN, OUT, M, B> ApplyBack<B>(PipeT<IN, OUT, M, Func<A, B>> ff) =>
        new PipeTLift<IN, OUT, X, M, B>(Acquire, x => Next(x).ApplyBack(ff));

    public override PipeT<IN, OUT, M, B> Action<B>(PipeT<IN, OUT, M, B> fb) =>
        new PipeTLift<IN, OUT, X, M, B>(Acquire, x => Next(x).Action(fb));

    public override PipeT<IN, OUT, M, B> Bind<B>(Func<A, PipeT<IN, OUT, M, B>> f) =>
        new PipeTLift<IN, OUT, X, M, B>(Acquire, x => Next(x).Bind(f));

    internal override PipeT<IN1, OUT, M, A> ReplaceAwait<IN1>(Func<PipeT<IN1, OUT, M, IN>> request) => 
        new PipeTLift<IN1, OUT, X, M, A>(Acquire, x => Next(x).ReplaceAwait(request));

    internal override PipeT<IN, OUT1, M, A> ReplaceYield<OUT1>(Func<OUT, PipeT<IN, OUT1, M, Unit>> response) => 
        new PipeTLift<IN, OUT1, X, M, A>(Acquire, x => Next(x).ReplaceYield(response));

    internal override PipeT<IN1, OUT, M, A> PairEachAwaitWithYield<IN1>(Func<Unit, PipeT<IN1, IN, M, A>> request) => 
        new PipeTLift<IN1, OUT, X, M, A>(Acquire, x => Next(x).PairEachAwaitWithYield(request));

    internal override PipeT<IN, OUT1, M, A> PairEachYieldWithAwait<OUT1>(Func<OUT, PipeT<OUT, OUT1, M, A>> response) =>
        new PipeTLift<IN, OUT1, X, M, A>(Acquire, x => Next(x).PairEachYieldWithAwait(response));
    
    internal override K<M, A> Run() =>
        Next(Acquire()).Run();
}

record PipeTLiftM<IN, OUT, M, A>(K<M, PipeT<IN, OUT, M, A>> Value) : PipeT<IN, OUT, M, A>
    where M : Monad<M>
{
    public override PipeT<IN, OUT, M, B> Map<B>(Func<A, B> f) =>
        new PipeTLiftM<IN, OUT, M, B>(Value.Map(px => px.Map(f)));

    public override PipeT<IN, OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) =>
        new PipeTLiftM<IN, OUT, M, B>(Value.Map(px => px.MapM(f)));
    
    public override PipeT<IN, OUT, M, B> ApplyBack<B>(PipeT<IN, OUT, M, Func<A, B>> ff) =>
        new PipeTLiftM<IN, OUT, M, B>(Value.Map(px => px.ApplyBack(ff)));

    public override PipeT<IN, OUT, M, B> Action<B>(PipeT<IN, OUT, M, B> fb) =>
        new PipeTLiftM<IN, OUT, M, B>(Value.Map(px => px.Action(fb)));

    public override PipeT<IN, OUT, M, B> Bind<B>(Func<A, PipeT<IN, OUT, M, B>> f) =>
        new PipeTLiftM<IN, OUT, M, B>(Value.Map(px => px.Bind(f)));

    internal override PipeT<IN1, OUT, M, A> ReplaceAwait<IN1>(Func<PipeT<IN1, OUT, M, IN>> request) => 
        new PipeTLiftM<IN1, OUT, M, A>(Value.Map(px => px.ReplaceAwait(request)));

    internal override PipeT<IN, OUT1, M, A> ReplaceYield<OUT1>(Func<OUT, PipeT<IN, OUT1, M, Unit>> response) => 
        new PipeTLiftM<IN, OUT1, M, A>(Value.Map(px => px.ReplaceYield(response)));
    
    internal override PipeT<IN1, OUT, M, A> PairEachAwaitWithYield<IN1>(Func<Unit, PipeT<IN1, IN, M, A>> request) => 
        new PipeTLiftM<IN1, OUT, M, A>(Value.Map(px => px.PairEachAwaitWithYield(request)));

    internal override PipeT<IN, OUT1, M, A> PairEachYieldWithAwait<OUT1>(Func<OUT, PipeT<OUT, OUT1, M, A>> response) =>
        new PipeTLiftM<IN, OUT1, M, A>(Value.Map(px => px.PairEachYieldWithAwait(response)));

    internal override K<M, A> Run() =>
        Value.Bind(p => p.Run());
}

record PipeTYield<IN, OUT, M, A>(OUT Value, Func<Unit, PipeT<IN, OUT, M, A>> Next) : PipeT<IN, OUT, M, A>
    where M : Monad<M>
{
    public override PipeT<IN, OUT, M, B> Map<B>(Func<A, B> f) => 
        new PipeTYield<IN, OUT, M, B>(Value, _ => Next(default).Map(f));

    public override PipeT<IN, OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) => 
        new PipeTYield<IN, OUT, M, B>(Value, _ => Next(default).MapM(f));

    public override PipeT<IN, OUT, M, B> ApplyBack<B>(PipeT<IN, OUT, M, Func<A, B>> ff) => 
        new PipeTYield<IN, OUT, M, B>(Value, _ => Next(default).ApplyBack(ff));

    public override PipeT<IN, OUT, M, B> Action<B>(PipeT<IN, OUT, M, B> fb) => 
        new PipeTYield<IN, OUT, M, B>(Value, _ => Next(default).Action(fb));

    public override PipeT<IN, OUT, M, B> Bind<B>(Func<A, PipeT<IN, OUT, M, B>> f) => 
        new PipeTYield<IN, OUT, M, B>(Value, _ => Next(default).Bind(f));

    internal override PipeT<IN1, OUT, M, A> ReplaceAwait<IN1>(Func<PipeT<IN1, OUT, M, IN>> request) =>
        new PipeTYield<IN1, OUT, M, A>(Value, _ => Next(default).ReplaceAwait(request));
    
    internal override PipeT<IN, OUT1, M, A> ReplaceYield<OUT1>(Func<OUT, PipeT<IN, OUT1, M, Unit>> response) =>
        response(Value).Bind(_ => Next(default).ReplaceYield(response));    
    
    internal override PipeT<IN1, OUT, M, A> PairEachAwaitWithYield<IN1>(Func<Unit, PipeT<IN1, IN, M, A>> request) => 
        new PipeTYield<IN1, OUT, M, A>(Value, _ => Next(default).PairEachAwaitWithYield(request));

    internal override PipeT<IN, OUT1, M, A> PairEachYieldWithAwait<OUT1>(Func<OUT, PipeT<OUT, OUT1, M, A>> response) =>
        response(Value).PairEachAwaitWithYield(Next);

    internal override K<M, A> Run() => 
        throw new InvalidOperationException("closed");
}

record PipeTAwait<IN, OUT, M, A>(Func<IN, PipeT<IN, OUT, M, A>> Next) : PipeT<IN, OUT, M, A>
    where M : Monad<M>
{
    public override PipeT<IN, OUT, M, B> Map<B>(Func<A, B> f) => 
        new PipeTAwait<IN, OUT, M, B>(x => Next(x).Map(f));

    public override PipeT<IN, OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) => 
        new PipeTAwait<IN, OUT, M, B>(x => Next(x).MapM(f));

    public override PipeT<IN, OUT, M, B> ApplyBack<B>(PipeT<IN, OUT, M, Func<A, B>> ff) => 
        new PipeTAwait<IN, OUT, M, B>(x => Next(x).ApplyBack(ff));

    public override PipeT<IN, OUT, M, B> Action<B>(PipeT<IN, OUT, M, B> fb) => 
        new PipeTAwait<IN, OUT, M, B>(x => Next(x).Action(fb));

    public override PipeT<IN, OUT, M, B> Bind<B>(Func<A, PipeT<IN, OUT, M, B>> f) => 
        new PipeTAwait<IN, OUT, M, B>(x => Next(x).Bind(f));

    internal override PipeT<IN1, OUT, M, A> ReplaceAwait<IN1>(Func<PipeT<IN1, OUT, M, IN>> request) =>
        request().Bind(x => Next(x).ReplaceAwait(request));
        
    internal override PipeT<IN, OUT1, M, A> ReplaceYield<OUT1>(Func<OUT, PipeT<IN, OUT1, M, Unit>> response) =>
        new PipeTAwait<IN, OUT1, M, A>(x => Next(x).ReplaceYield(response));

    internal override PipeT<IN1, OUT, M, A> PairEachAwaitWithYield<IN1>(Func<Unit, PipeT<IN1, IN, M, A>> request) => 
        request(default).PairEachYieldWithAwait(Next);

    internal override PipeT<IN, OUT1, M, A> PairEachYieldWithAwait<OUT1>(Func<OUT, PipeT<OUT, OUT1, M, A>> response) =>
        new PipeTAwait<IN, OUT1, M, A>(x => Next(x).PairEachYieldWithAwait(response));

    internal override K<M, A> Run() => 
        throw new InvalidOperationException("closed");
}

record PipeTYieldAll<IN, OUT, M, A>(IEnumerable<PipeT<IN, OUT, M, A>> Yields) : PipeT<IN, OUT, M, A>
    where M : Monad<M>, Alternative<M>
{
    public override PipeT<IN, OUT, M, B> Map<B>(Func<A, B> f) => 
        new PipeTYieldAll<IN, OUT, M, B>(Yields.Select(p => p.Map(f)));

    public override PipeT<IN, OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) => 
        new PipeTYieldAll<IN, OUT, M, B>(Yields.Select(p => p.MapM(f)));

    public override PipeT<IN, OUT, M, B> ApplyBack<B>(PipeT<IN, OUT, M, Func<A, B>> ff) => 
        new PipeTYieldAll<IN, OUT, M, B>(Yields.Select(p => p.ApplyBack(ff)));

    public override PipeT<IN, OUT, M, B> Action<B>(PipeT<IN, OUT, M, B> fb) => 
        new PipeTYieldAll<IN, OUT, M, B>(Yields.Select(p => p.Action(fb)));

    public override PipeT<IN, OUT, M, B> Bind<B>(Func<A, PipeT<IN, OUT, M, B>> f) => 
        new PipeTYieldAll<IN, OUT, M, B>(Yields.Select(p => p.Bind(f)));

    internal override PipeT<IN1, OUT, M, A> ReplaceAwait<IN1>(Func<PipeT<IN1, OUT, M, IN>> request) =>
        new PipeTYieldAll<IN1, OUT, M, A>(Yields.Select(p => p.ReplaceAwait(request)));
    
    internal override PipeT<IN, OUT1, M, A> ReplaceYield<OUT1>(Func<OUT, PipeT<IN, OUT1, M, Unit>> response) =>
        new PipeTYieldAll<IN, OUT1, M, A>(Yields.Select(p => p.ReplaceYield(response)));
    
    internal override PipeT<IN1, OUT, M, A> PairEachAwaitWithYield<IN1>(Func<Unit, PipeT<IN1, IN, M, A>> request) => 
        new PipeTYieldAll<IN1, OUT, M, A>(Yields.Select(p => p.PairEachAwaitWithYield(request)));

    internal override PipeT<IN, OUT1, M, A> PairEachYieldWithAwait<OUT1>(Func<OUT, PipeT<OUT, OUT1, M, A>> response) =>
        new PipeTYieldAll<IN, OUT1, M, A>(Yields.Select(p => p.PairEachYieldWithAwait(response)));

    internal override K<M, A> Run() =>
        Yields.Select(p => p.Run()).Actions().Choose(M.Empty<A>());
}

record PipeTYieldAllAsync<IN, OUT, M, A>(IAsyncEnumerable<PipeT<IN, OUT, M, A>> Yields) : PipeT<IN, OUT, M, A>
    where M : Monad<M>, Alternative<M>
{
    public override PipeT<IN, OUT, M, B> Map<B>(Func<A, B> f) => 
        new PipeTYieldAllAsync<IN, OUT, M, B>(Yields.Select(p => p.Map(f)));

    public override PipeT<IN, OUT, M, B> MapM<B>(Func<K<M, A>, K<M, B>> f) => 
        new PipeTYieldAllAsync<IN, OUT, M, B>(Yields.Select(p => p.MapM(f)));

    public override PipeT<IN, OUT, M, B> ApplyBack<B>(PipeT<IN, OUT, M, Func<A, B>> ff) => 
        new PipeTYieldAllAsync<IN, OUT, M, B>(Yields.Select(p => p.ApplyBack(ff)));

    public override PipeT<IN, OUT, M, B> Action<B>(PipeT<IN, OUT, M, B> fb) => 
        new PipeTYieldAllAsync<IN, OUT, M, B>(Yields.Select(p => p.Action(fb)));

    public override PipeT<IN, OUT, M, B> Bind<B>(Func<A, PipeT<IN, OUT, M, B>> f) => 
        new PipeTYieldAllAsync<IN, OUT, M, B>(Yields.Select(p => p.Bind(f)));

    internal override PipeT<IN1, OUT, M, A> ReplaceAwait<IN1>(Func<PipeT<IN1, OUT, M, IN>> request) =>
        new PipeTYieldAllAsync<IN1, OUT, M, A>(Yields.Select(p => p.ReplaceAwait(request)));
    
    internal override PipeT<IN, OUT1, M, A> ReplaceYield<OUT1>(Func<OUT, PipeT<IN, OUT1, M, Unit>> response) =>
        new PipeTYieldAllAsync<IN, OUT1, M, A>(Yields.Select(p => p.ReplaceYield(response)));
    
    internal override PipeT<IN1, OUT, M, A> PairEachAwaitWithYield<IN1>(Func<Unit, PipeT<IN1, IN, M, A>> request) => 
        new PipeTYieldAllAsync<IN1, OUT, M, A>(Yields.Select(p => p.PairEachAwaitWithYield(request)));

    internal override PipeT<IN, OUT1, M, A> PairEachYieldWithAwait<OUT1>(Func<OUT, PipeT<OUT, OUT1, M, A>> response) =>
        new PipeTYieldAllAsync<IN, OUT1, M, A>(Yields.Select(p => p.PairEachYieldWithAwait(response)));

    internal override K<M, A> Run() =>
        Yields.Select(p => p.RunAsync()).Actions().Choose(M.Empty<A>());
}
