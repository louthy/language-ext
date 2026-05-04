using System;
using System.Collections.Generic;
using System.Text;
using LanguageExt.Traits.Domain;
using Xunit;

namespace LanguageExt.Tests;

public sealed class MichelinStart 
    : Maintainer<MichelinStart, uint>
{
    private readonly uint _value;

    public static MichelinStart One { get; } = new(1);

    public static MichelinStart Two { get; } = new(2);

    public static MichelinStart Three { get; } = new(3);

    public static MichelinStart Four { get; } = new(4);

    public static MichelinStart Five { get; } = new(5);

    private MichelinStart(uint value) => 
        _value = value;

    public uint To() => _value;

    public static Seq<MichelinStart> All { get; } =
    [
        One,
        Two,
        Three,
        Four,
        Five
    ];
}

public sealed class MaintainerTests
{
    [Fact]
    public void Get_ShouldReturnMaintainerValues()
    {
        var expAll = MichelinStart.All;
        var preludeAll = get<MichelinStart>();

        Assert.Equal(expAll, preludeAll);
    }

    [Fact]
    public void FindM_ShouldReturnSome()
    {
        const uint expValue = 3;
        var exp = MichelinStart.Three;
        var preludeSome = findM<MichelinStart>(s => s.To() == expValue);
        var extensionSome = MichelinStart.FindM(s => s.To() == expValue);


        Assert.True(preludeSome.IsSome);
        Assert.True(extensionSome.IsSome);
        Assert.Equal(exp, preludeSome.IfNone(() => throw new InvalidOperationException("Option was None")));
        Assert.Equal(exp, extensionSome.IfNone(() => throw new InvalidOperationException("Option was None")));
    }

    [Fact]
    public void FindM_ShouldReturnNone()
    {
        const uint expValue = 6;
        var preludeNone = findM<MichelinStart>(s => s.To() == expValue);
        var extensionNone = MichelinStart.FindM(s => s.To() == expValue);
        
        Assert.True(preludeNone.IsNone);
        Assert.True(extensionNone.IsNone);  
    }

    [Fact]
    public void Find_ShouldReturnValue()
    {
        const uint expValue = 4;
        var exp = MichelinStart.Four;
        var prelude = find<MichelinStart>(s => s.To() == expValue);
        var extension = MichelinStart.Find(s => s.To() == expValue);
        
        Assert.Equal(exp, prelude);
        Assert.Equal(exp, extension);
    }

    [Fact]
    public void Find_ShouldExplode()
    {
        const uint expValue = 7;

        Assert.Throws<InvalidOperationException>(() => find<MichelinStart>(s => s.To() == expValue));
        Assert.Throws<InvalidOperationException>(() => MichelinStart.Find(s => s.To() == expValue));

    }
}
