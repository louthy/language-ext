using System;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.DSL;

record IOAsyncActions<A, B>(IteratorAsync<K<IO, A>> Fas, Func<A, IO<B>> Next) : InvokeAsyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOAsyncActions<A, C>(Fas, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOAsyncActions<A, C>(Fas, x => Next(x).Bind(f));

    public override async ValueTask<IO<B>> Invoke(EnvIO envIO)
    {
        if (await Fas.IsEmpty)
        {
            return IO.fail<B>(Error.New("Actions is empty"));
        }
        else
        {
            ignore(await (await Fas.Head).RunAsync(envIO));
            return new IOAsyncActions<A, B>(await Fas.Tail, Next);
        }
    }
}
