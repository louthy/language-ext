using System;
using System.Threading.Tasks;
using LanguageExt.Common;
using LanguageExt.Traits;
using static LanguageExt.Prelude;

namespace LanguageExt.DSL;

record IOAsyncActions<A, B>(IteratorIO<K<IO, A>> Fas, Func<A, IO<B>> Next) : InvokeSyncIO<B>
{
    public override IO<C> Map<C>(Func<B, C> f) => 
        new IOAsyncActions<A, C>(Fas, x => Next(x).Map(f));

    public override IO<C> Bind<C>(Func<B, K<IO, C>> f) => 
        new IOAsyncActions<A, C>(Fas, x => Next(x).Bind(f));

    public override IO<C> BindAsync<C>(Func<B, ValueTask<K<IO, C>>> f) => 
        new IOAsyncActions<A, C>(Fas, x => Next(x).BindAsync(f));

    public override IO<B> Invoke(EnvIO envIO) =>
        Fas.NextIO()
           .Bind(n => n switch
                      {
                          (Exist<K<IO, A>> (var head), Nil<K<IO, A>> tail) =>
                              head.Bind(Next),

                          (Exist<K<IO, A>> (var head), var tail) =>
                              head.Bind(_ => new IOAsyncActions<A, B>(tail, Next)),

                          _ => throw new NotSupportedException(
                                   "IterableNE can't be empty, so we will never get here")
                      });
}
