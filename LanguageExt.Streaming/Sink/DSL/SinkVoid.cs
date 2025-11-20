using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt;

record SinkVoid<A> : Sink<A>
{
    public static readonly Sink<A> Default = new SinkVoid<A>();
    
    public override Sink<B> Comap<B>(Func<B, A> f) => 
        new SinkVoid<B>();

    public override IO<Unit> Post(A value) =>
        unitIO;

    public override IO<Unit> Complete() =>
        unitIO;

    public override IO<Unit> Fail(Error error) =>
        unitIO;
}
