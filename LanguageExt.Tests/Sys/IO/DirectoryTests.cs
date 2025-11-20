using System.IO;
using System.Threading.Tasks;
using LanguageExt.Sys.IO;
using Xunit;
using Runtime = LanguageExt.Sys.Test.Runtime;

namespace LanguageExt.Tests.Sys.IO;

public class DirectoryTests
{
    [Fact]
    public async Task EnumerateFilesTest()
    {
        using var rt = Runtime.New();
        var path     = Path.Combine(Path.GetTempPath(), "subdir");
        var expected = Path.Combine(path, "file");
        
        var computation =
            from dir   in Directory<Runtime>.create(path)
            from file  in File<Runtime>.writeAllText(expected, string.Empty)
            from files in Directory<Runtime>.enumerateFiles(path)
            select files;

        var actual = (await computation.RunAsync(rt)).ThrowIfFail();

        Assert.True(actual == Seq(expected));
    }
}
