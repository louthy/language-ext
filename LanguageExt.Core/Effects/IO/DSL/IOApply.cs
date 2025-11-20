using System;
using System.Threading.Tasks;
using LanguageExt.Traits;

namespace LanguageExt.DSL;

record IOApply<A, B, C>(K<IO, Func<A, B>> Ff, K<IO, A> Fa, Func<B, K<IO, C>> Next) : InvokeSyncIO<C>
{
    public override IO<D> Map<D>(Func<C, D> f) => 
        new IOApply<A, B, D>(Ff, Fa, x => Next(x).As().Map(f));

    public override IO<D> Bind<D>(Func<C, K<IO, D>> f) => 
        new IOApply<A, B, D>(Ff, Fa, x => Next(x).As().Bind(f));

    public override IO<D> BindAsync<D>(Func<C, ValueTask<K<IO, D>>> f) => 
        new IOApply<A, B, D>(Ff, Fa, x => Next(x).As().BindAsync(f));

    public override IO<C> Invoke(EnvIO envIO)
    {
        var tf = Ff.RunAsync(envIO);
        var ta = Fa.RunAsync(envIO);

        switch (tf.IsCompleted, ta.IsCompleted)
        {
            case (true, true):
                return Next(tf.Result(ta.Result)).As();
                
            default:
                return new IOApplyFunctionAsync<A, B, C>(tf, ta, Next);
        }        
    }
    
    public override string ToString() => 
        "IO apply";
}

record IOApplyFunctionAsync<A, B, C>(ValueTask<Func<A, B>> Ff, ValueTask<A> Fa, Func<B, K<IO, C>> Next) : InvokeAsyncIO<C>
{
    public override IO<D> Map<D>(Func<C, D> f) => 
        new IOApplyFunctionAsync<A, B, D>(Ff, Fa, x => Next(x).As().Map(f));

    public override IO<D> Bind<D>(Func<C, K<IO, D>> f) => 
        new IOApplyFunctionAsync<A, B, D>(Ff, Fa, x => Next(x).As().Bind(f));

    public override IO<D> BindAsync<D>(Func<C, ValueTask<K<IO, D>>> f) => 
        new IOApplyFunctionAsync<A, B, D>(Ff, Fa, x => Next(x).As().BindAsync(f));

    public override async ValueTask<IO<C>> Invoke(EnvIO envIO)
    {
        switch (Ff.IsCompleted, Fa.IsCompleted)
        {
            case (true, true):
                return Next(Ff.Result(Fa.Result)).As();
                
            case (false, true):
                return Next((await Ff)(Fa.Result)).As();
            
            case (true, false):
                return Next(Ff.Result(await Fa)).As();
            
            default:
                var tf = Ff.AsTask();
                var ta = Fa.AsTask();
                await Task.WhenAll(tf, ta);
                return Next(Ff.Result(Fa.Result)).As();
        }
    }
    
    public override string ToString() => 
        "IO apply";
}
