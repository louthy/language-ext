using System.Threading.Tasks;
using LanguageExt.Pipes;
using LanguageExt.Sys.Test;
using Xunit;

using static LanguageExt.Pipes.Proxy;

namespace LanguageExt.Tests;

public class PipesTests
{
    [Fact]
    public Task MergeSynchronousProducersSucceeds() =>
        compose(
            Producer.merge<Runtime, int>(
                yield(1),
                yield(1)),
            awaiting<int>().Map(ignore))
        .RunEffect()
        .RunUnit(Runtime.New())
        .AsTask();
}
