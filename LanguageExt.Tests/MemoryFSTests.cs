using Xunit;
using System.IO;
using LanguageExt.Sys.Test;
using System.Threading.Tasks;
using LanguageExt.Sys;
using LanguageExt.UnsafeValueAccess;
using File = LanguageExt.Sys.IO.File<LanguageExt.Eff<LanguageExt.Sys.Test.Runtime>, LanguageExt.Sys.Test.Runtime>;
using Dir  = LanguageExt.Sys.IO.Directory<LanguageExt.Eff<LanguageExt.Sys.Test.Runtime>, LanguageExt.Sys.Test.Runtime>;

namespace LanguageExt.Tests;

public class MemoryFSTests
{
    [Fact]
    public void Write_and_read_file()
    {
        var root = MemoryFS.IsUnix ? "/" : "C:\\";
        var rt   = Runtime.New();

        var comp = from _ in File.writeAllText($"{root}test.txt", "Hello, World")
                   from t in File.readAllText($"{root}test.txt")
                   select t;
            
        var r = comp.As().Run(rt, EnvIO.New());
        Assert.True(r == "Hello, World", FailMsg(r));
    }
        
    [Fact]
    public void Write_to_non_existent_folder()
    {
        var rt = Runtime.New();

        var comp = from _ in File.writeAllText("C:\\non-exist\\test.txt", "Hello, World")
                   from t in File.readAllText("C:\\non-exist\\test.txt")
                   select t;
            
        var r = comp.As().Run(rt, EnvIO.New());
        Assert.True(r.IsFail);
    }
        
    [Fact]
    public void Create_folder_write_and_read_file()
    {
        var root = MemoryFS.IsUnix ? "/non-exist/" : "C:\\non-exist\\"; 
        
        var rt = Runtime.New();

        var comp = from _1 in Dir.create(root)
                   from _2 in File.writeAllText($"{root}test.txt", "Hello, World")
                   from tx in File.readAllText($"{root}test.txt")
                   select tx;
            
        var r = comp.As().Run(rt, EnvIO.New());
        Assert.True(r == "Hello, World", FailMsg(r));
    }
        
    [Fact]
    public void Create_nested_folder_write_and_read_file()
    {
        var root = MemoryFS.IsUnix ? "/non-exist/also-non-exist" : "C:\\non-exist\\also-non-exist";
        var rt   = Runtime.New();

        var comp = from _1 in Dir.create(root)
                   from _2 in File.writeAllText(Path.Combine(root, "test.txt"), "Hello, World")
                   from tx in File.readAllText(Path.Combine(root, "test.txt"))
                   select tx;
            
        var r  = comp.As().Run(rt, EnvIO.New());
        var eq = r == "Hello, World";
        Assert.True(eq, FailMsg(r));
    }
        
    [Fact]
    public void Create_nested_folder_write_two_files_enumerate_one_of_them_using_star_pattern()
    {
        var fldr = MemoryFS.IsUnix ? "/non-exist/also-non-exist" : "C:\\non-exist\\also-non-exist";
        var root = MemoryFS.IsUnix ? "/" : "C:\\";
        
        var rt = Runtime.New();

        var comp = from _1 in Dir.create(fldr)
                   from _2 in File.writeAllText(Path.Combine(fldr, "test.txt"), "Hello, World")
                   from _3 in File.writeAllText(Path.Combine(fldr, "test.bat"), "Goodbye, World")
                   from en in Dir.enumerateFiles(root, "*.txt", SearchOption.AllDirectories)
                   from tx in File.readAllText(en.Head.ValueUnsafe()!)
                   select (tx, en.Count);
            
        var r = comp.As().Run(rt, EnvIO.New());
        Assert.True(r == ("Hello, World", 1), FailMsg(r));
    }
        
    [Fact]
    public void Create_nested_folder_write_two_files_enumerate_one_of_them_using_qm_pattern()
    {
        var fldr = MemoryFS.IsUnix ? "/non-exist/also-non-exist" : "C:\\non-exist\\also-non-exist";
        var root = MemoryFS.IsUnix ? "/" : "C:\\";
        
        var rt   = Runtime.New();

        var comp = from _1 in Dir.create(fldr)
                   from _2 in File.writeAllText(Path.Combine(fldr, "test.txt"), "Hello, World")
                   from _3 in File.writeAllText(Path.Combine(fldr, "test.bat"), "Goodbye, World")
                   from en in Dir.enumerateFiles(root, "????.txt", SearchOption.AllDirectories)
                   from tx in File.readAllText(en.Head.ValueUnsafe()!)
                   select (tx, en.Count);
            
        var r = comp.As().Run(rt, EnvIO.New());
        Assert.True(r == ("Hello, World", 1), FailMsg(r));
    }
        
    [Fact]
    public void Create_nested_folder_write_two_files_enumerate_fail_using_singular_qm_pattern()
    {
        var rt = Runtime.New();

        var comp = from _1 in Dir.create("C:\\non-exist\\also-non-exist")
                   from _2 in File.writeAllText("C:\\non-exist\\also-non-exist\\test.txt", "Hello, World")
                   from _3 in File.writeAllText("C:\\non-exist\\also-non-exist\\test.bat", "Goodbye, World")
                   from en in Dir.enumerateFiles("C:\\", "?.txt", SearchOption.AllDirectories)
                   from tx in File.readAllText(en.Head.ValueUnsafe()!)
                   select (tx, en.Count);
            
        var r = comp.As().Run(rt, EnvIO.New());
        Assert.True(r.IsFail);
    }
        
    [Theory]
    [InlineData("C:\\non-exist", "/non-exist")]
    [InlineData("C:\\non-exist\\also-non-exist", "/non-exist/also-non-exist")]
    [InlineData("C:\\a\\b\\c\\d", "/a/b/c/d")]
    public void Create_and_delete_folder(string windowsPath, string linuxPath)
    {
        var path = MemoryFS.IsUnix ? linuxPath : windowsPath;
        
        var rt = Runtime.New();

        var comp = from p  in Pure(path)
                   from _1 in Dir.create(p)
                   from e1 in Dir.exists(p)
                   from _2 in Dir.delete(p)
                   from e2 in Dir.exists(p)
                   select (e1, e2);
            
        var r = comp.As().Run(rt, EnvIO.New());
        Assert.True(r == (true, false), FailMsg(r));
    }

    [Theory]
    [InlineData("C:\\non-exist")]
    [InlineData("C:\\non-exist\\also-non-exist")]
    [InlineData("D:\\a\\b\\c\\d")]
    public void Delete_non_existent_folder(string path)
    {
        var rt = Runtime.New();

        var comp = from p in SuccessEff(path)
                   from _ in Dir.delete(p)
                   select unit;
            
        var r = comp.As().Run(rt, EnvIO.New());
        Assert.True(r.IsFail);
    }
        
    [Theory]
    [InlineData(false, "C:\\non-exist", "C:\\non-exist\\test.txt")]
    [InlineData(false, "C:\\non-exist\\also-non-exist", "C:\\non-exist\\also-non-exist\\test.txt")]
    [InlineData(true, "/non-exist", "/non-exist/test.txt")]
    [InlineData(true, "/non-exist/also-non-exist", "/non-exist/also-non-exist/test.txt")]
    public void File_exists(bool isUnix, string folder, string file)
    {
        if (MemoryFS.IsUnix != isUnix) return;
        var rt = Runtime.New();

        var comp = from _1 in Dir.create(folder)
                   from _2 in File.writeAllText(file, "Hello, World")
                   from ex in File.exists(file)
                   select ex;
            
        var r = comp.As().Run(rt, EnvIO.New());
        Assert.True(r == true, FailMsg(r));
    }
        
    [Theory]
    [InlineData("C:\\non-exist\\test.txt")]
    [InlineData("C:\\non-exist\\also-non-exist\\test.txt")]
    [InlineData("X:\\non-exist\\also-non-exist\\test.txt")]
    [InlineData("X:\\test.txt")]
    public void File_doesnt_exist(string file)
    {
        var rt = Runtime.New();

        var comp = from _2 in File.writeAllText(file, "Hello, World")
                   from ex in File.exists(file)
                   select ex;
            
        var r = comp.As().Run(rt, EnvIO.New());
        Assert.True(r.IsFail);
    }

    [Theory]
    [InlineData(false, "C:\\", "C:\\a\\b\\c\\d", new[] {"C:\\a", "C:\\a\\b", "C:\\a\\b\\c", "C:\\a\\b\\c\\d" }, "*")]
    [InlineData(false, "C:\\", "C:\\a\\b\\c", new[] {"C:\\a", "C:\\a\\b", "C:\\a\\b\\c"}, "*")]
    [InlineData(false, "C:\\", "C:\\a\\b", new[] {"C:\\a", "C:\\a\\b"}, "*")]
    [InlineData(false, "C:\\", "C:\\a", new[] {"C:\\a"}, "*")]
    [InlineData(false, "C:\\", "C:\\and\\all\\the\\arrows", new[] {"C:\\and", "C:\\and\\all", "C:\\and\\all\\the\\arrows" }, "*a*")]
    [InlineData(true, "/", "/a/b/c/d", new[] {"/a", "/a/b", "/a/b/c", "/a/b/c/d" }, "*")]
    [InlineData(true, "/", "/a/b/c", new[] {"/a", "/a/b", "/a/b/c"}, "*")]
    [InlineData(true, "/", "/a/b", new[] {"/a", "/a/b"}, "*")]
    [InlineData(true, "/", "/a", new[] {"/a"}, "*")]
    [InlineData(true, "/", "/and/all/the/arrows", new[] {"/and", "/and/all", "/and/all/the/arrows" }, "*a*")]
    public void Enumerate_folders(bool isUnix, string root, string path, string[] folders, string pattern)
    {
        if (MemoryFS.IsUnix != isUnix) return;
        
        var rt = Runtime.New();

        var comp = from _1 in Dir.create(path)
                   from ds in Dir.enumerateDirectories(root, pattern, SearchOption.AllDirectories)
                   select ds.Strict();

        var r   = comp.As().Run(rt, EnvIO.New());
        Assert.True(r == toSeq(folders));
    }

    [Theory]
    [InlineData(false, "C:\\a\\b", "C:\\c\\d", "test.txt")]
    [InlineData(false, "C:\\", "C:\\c\\d", "test.txt")]
    [InlineData(true, "/a/b", "/c/d", "test.txt")]
    [InlineData(true, "/", "/c/d", "test.txt")]
    public void Move_file_from_one_folder_to_another(bool isUnix, string srcdir, string destdir, string file)
    {
        if (MemoryFS.IsUnix != isUnix) return;
        
        var rt = Runtime.New();

        var comp = from _1 in Dir.create(srcdir)
                   from _2 in Dir.create(destdir)
                   from sf in Pure(Path.Combine(srcdir, file))
                   from df in Pure(Path.Combine(destdir, file))
                   from _3 in File.writeAllText(sf, "Hello, World")
                   from _4 in Dir.move(sf, df)
                   from tx in File.readAllText(df)
                   from ex in File.exists(sf)
                   select (ex, tx);

        var r = comp.As().Run(rt, EnvIO.New());
        Assert.True(r == (false, "Hello, World"), FailMsg(r));
    }
        
    static string FailMsg<A>(Fin<A> ma) =>
        ma.Match(Succ: _ => "", Fail: e => e.Message);
}
