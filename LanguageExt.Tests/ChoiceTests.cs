using System;
using static LanguageExt.Prelude;
using Xunit;
using System.Threading.Tasks;
using LanguageExt.Common;

namespace LanguageExt.Tests;

public class ChoiceTests
{
    [Fact]
    public async Task TaskFirstChoice()
    {
        var ma = 123.AsTask();
        var mb = new Exception().AsFailedTask<int>();
        var mc = new Exception().AsFailedTask<int>();

        var res = choice(ma, mb, mc);

        Assert.True(await res == 123);
    }

    [Fact]
    public async Task TaskMiddleChoice()
    {
        var ma = new Exception().AsFailedTask<int>();
        var mb = 123.AsTask();
        var mc = new Exception().AsFailedTask<int>();

        var res = choice(ma, mb, mc);

        Assert.True(await res == 123);
    }

    [Fact]
    public async Task TaskLastChoice()
    {
        var ma = new Exception().AsFailedTask<int>();
        var mb = new Exception().AsFailedTask<int>();
        var mc = 123.AsTask();

        var res = choice(ma, mb, mc);

        Assert.True(await res == 123);
    }

    [Fact]
    public async Task TaskNoChoice()
    {
        var ma = new Exception().AsFailedTask<int>();
        var mb = new Exception().AsFailedTask<int>();
        var mc = new Exception().AsFailedTask<int>();

        var res = choice(ma, mb, mc);

        await Assert.ThrowsAsync<BottomException>(async () => await res);
    }
}
