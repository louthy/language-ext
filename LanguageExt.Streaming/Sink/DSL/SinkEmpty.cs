using System;
using LanguageExt.Common;
using static LanguageExt.Prelude;

namespace LanguageExt;

record SinkEmpty<A> : Sink<A>
{
    public static readonly Sink<A> Default = new SinkEmpty<A>();
    
    public override Sink<B> Comap<B>(Func<B, A> f) => 
        new SinkEmpty<B>();

    public override IO<Unit> Post(A value) =>
        IO.fail<Unit>(Errors.SinkFull);

    public override IO<Unit> Complete() =>
        unitIO;

    public override IO<Unit> Fail(Error error) =>
        unitIO;
}
