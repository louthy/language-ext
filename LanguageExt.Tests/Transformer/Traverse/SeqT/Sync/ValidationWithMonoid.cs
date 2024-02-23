using System;
using LanguageExt.ClassInstances;
using LanguageExt.Common;
using Xunit;
using static LanguageExt.Prelude;

namespace LanguageExt.Tests.Transformer.Traverse.SeqT.Sync;

public class ValidationWithMonoidSeq
{
    [Fact]
    public void FailIsSingletonFail()
    {
        var ma = Fail<Seq<Error>, Seq<int>>([Error.New("alt")]);
        var mb = ma.Traverse(x => x).As();
        var mc = Seq.singleton(Fail<Seq<Error>, int>([Error.New("alt")]));

        Assert.True(mb == mc);
    }

    [Fact]
    public void SuccessEmptyIsEmpty()
    {
        var ma = Success<Seq<Error>, Seq<int>>(Empty);
        var mb = ma.Traverse(x => x).As();
        var mc = Seq.empty<Validation<Seq<Error>, int>>();

        Assert.True(mb == mc);
    }

    [Fact]
    public void SuccessNonEmptySeqIsSeqSuccesses()
    {
        var ma = Success<Seq<Error>, Seq<int>>(Seq(1, 2, 3, 4));
        var mb = ma.Traverse(x => x).As();
        var mc = Seq(
            Success<Seq<Error>, int>(1),
            Success<Seq<Error>, int>(2),
            Success<Seq<Error>, int>(3),
            Success<Seq<Error>, int>(4));

        Assert.True(mb == mc);
    }
}
