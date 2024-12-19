using System;
using LanguageExt.Common;

namespace LanguageExt.DSL;

record IOFail<A>(Error Value) : IODsl<A>
{
    public override IODsl<B> Map<B>(Func<A, B> f) =>
        new IOFail<B>(Value);
}
