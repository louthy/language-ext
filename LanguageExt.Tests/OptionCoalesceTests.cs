using Xunit;

namespace LanguageExt.Tests;

public class OptionCoalesceTests
{
    [Fact]
    public void OptionCoalesceTest1()
    {
        var optional = Some(123);
        var value    = optional || 456;
        Assert.True(value == 123);
    }

    [Fact]
    public void OptionCoalesceTest2()
    {
        Option<int> optional = None;

        var value = optional || 456;
        Assert.True(value == 456);
    }

    [Fact]
    public void OptionCoalesceTest3()
    {
        Option<int> optional1 = None;
        Option<int> optional2 = None;
        var         value     = optional1 || optional2 || 456;
        Assert.True(value == 456);
    }

    [Fact]
    public void OptionUnsafeCoalesceTest1()
    {
        var optional = Some(123);
        var value    = optional || 456;
        Assert.True(value == 123);
    }
}
