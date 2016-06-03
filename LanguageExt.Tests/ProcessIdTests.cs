using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace LanguageExtTests
{
    public class ProcessIdTests
    {
        [Fact]
        public void TestSelectionIds()
        {
            lock (ProcessTests.sync)
            {
                Process.shutdownAll();
                ProcessConfig.initialise();

                ProcessId test = "/disp/broadcast/[/root/user/a,/root/user/b,/root/user/c]";

                Assert.True(test.Take(1).GetName() == "disp");
                Assert.True(test.Skip(1).Take(1).GetName() == "broadcast");
                Assert.True(test.Skip(2).Take(1).GetName() == "[/root/user/a,/root/user/b,/root/user/c]");
                Assert.True(test.Skip(2).Take(1).IsSelection);
                Assert.True(test.Skip(2).Take(1).GetSelection().Count() == 3);
                Assert.True(test.Skip(2).Take(1).GetSelection().First().Path == "/root/user/a");
                Assert.True(test.Skip(2).Take(1).GetSelection().Skip(1).First().Path == "/root/user/b");
                Assert.True(test.Skip(2).Take(1).GetSelection().Skip(2).First().Path == "/root/user/c");
            }
        }

        [Fact]
        public void TestRegistered()
        {
            lock (ProcessTests.sync)
            {
                Process.shutdownAll();
                ProcessConfig.initialise();

                ProcessId test1 = "@registered";
                Assert.True(test1.Path == "/disp/reg/local-registered");

                ProcessId test2 = "@another:registered";
                Assert.True(test2.Path == "/disp/reg/another-registered");
            }
        }
    }
}
