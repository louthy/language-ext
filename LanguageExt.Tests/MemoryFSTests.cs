using System.IO;
using Xunit;
using LanguageExt.Sys;
using LanguageExt.Sys.IO;
using LanguageExt.Sys.Test;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

using File = LanguageExt.Sys.IO.File<LanguageExt.Sys.Test.Runtime>;
using Dir  = LanguageExt.Sys.IO.Directory<LanguageExt.Sys.Test.Runtime>;

namespace LanguageExt.Tests
{
    public class MemoryFSTests
    {
        [Fact]
        public async Task Write_and_read_file()
        {
            var rt = Runtime.New();

            var comp = from _ in File.writeAllText("C:\\test.txt", "Hello, World")
                       from t in File.readAllText("C:\\test.txt")
                       select t;
            
            var r = await comp.Run(rt);
            Assert.True(r == "Hello, World", FailMsg(r));
        }
        
        [Fact]
        public async Task Write_to_non_existent_folder()
        {
            var rt = Runtime.New();

            var comp = from _ in File.writeAllText("C:\\non-exist\\test.txt", "Hello, World")
                       from t in File.readAllText("C:\\non-exist\\test.txt")
                       select t;
            
            var r = await comp.Run(rt);
            Assert.True(r.IsFail);
        }
        
        [Fact]
        public async Task Create_folder_write_and_read_file()
        {
            var rt = Runtime.New();

            var comp = from _1 in Dir.create("C:\\non-exist\\")
                       from _2 in File.writeAllText("C:\\non-exist\\test.txt", "Hello, World")
                       from tx in File.readAllText("C:\\non-exist\\test.txt")
                       select tx;
            
            var r = await comp.Run(rt);
            Assert.True(r == "Hello, World", FailMsg(r));
        }
        
        [Fact]
        public async Task Create_nested_folder_write_and_read_file()
        {
            var rt = Runtime.New();

            var comp = from _1 in Dir.create("C:\\non-exist\\also-non-exist")
                       from _2 in File.writeAllText("C:\\non-exist\\also-non-exist\\test.txt", "Hello, World")
                       from tx in File.readAllText("C:\\non-exist\\also-non-exist\\test.txt")
                       select tx;
            
            var r = await comp.Run(rt);
            Assert.True(r == "Hello, World", FailMsg(r));
        }
        
        [Fact]
        public async Task Create_nested_folder_write_two_files_enumerate_one_of_them_using_star_pattern()
        {
            var rt = Runtime.New();

            var comp = from _1 in Dir.create("C:\\non-exist\\also-non-exist")
                       from _2 in File.writeAllText("C:\\non-exist\\also-non-exist\\test.txt", "Hello, World")
                       from _3 in File.writeAllText("C:\\non-exist\\also-non-exist\\test.bat", "Goodbye, World")
                       from en in Dir.enumerateFiles("C:\\", "*.txt", SearchOption.AllDirectories)
                       from tx in File.readAllText(en.Head)
                       select (tx, en.Count);
            
            var r = await comp.Run(rt);
            Assert.True(r == ("Hello, World", 1), FailMsg(r));
        }
        
        [Fact]
        public async Task Create_nested_folder_write_two_files_enumerate_one_of_them_using_qm_pattern()
        {
            var rt = Runtime.New();

            var comp = from _1 in Dir.create("C:\\non-exist\\also-non-exist")
                       from _2 in File.writeAllText("C:\\non-exist\\also-non-exist\\test.txt", "Hello, World")
                       from _3 in File.writeAllText("C:\\non-exist\\also-non-exist\\test.bat", "Goodbye, World")
                       from en in Dir.enumerateFiles("C:\\", "????.txt", SearchOption.AllDirectories)
                       from tx in File.readAllText(en.Head)
                       select (tx, en.Count);
            
            var r = await comp.Run(rt);
            Assert.True(r == ("Hello, World", 1), FailMsg(r));
        }
        
        [Fact]
        public async Task Create_nested_folder_write_two_files_enumerate_fail_using_singular_qm_pattern()
        {
            var rt = Runtime.New();

            var comp = from _1 in Dir.create("C:\\non-exist\\also-non-exist")
                       from _2 in File.writeAllText("C:\\non-exist\\also-non-exist\\test.txt", "Hello, World")
                       from _3 in File.writeAllText("C:\\non-exist\\also-non-exist\\test.bat", "Goodbye, World")
                       from en in Dir.enumerateFiles("C:\\", "?.txt", SearchOption.AllDirectories)
                       from tx in File.readAllText(en.Head)
                       select (tx, en.Count);
            
            var r = await comp.Run(rt);
            Assert.True(r.IsFail);
        }

        static string FailMsg<A>(Fin<A> ma) =>
            ma.Match(Succ: _ => "", Fail: e => e.Message);
    }
}
