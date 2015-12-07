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
            ProcessId test = "/disp/broadcast/[/root/user/c,/root/user/b,/root/user/c]";
        }
    }
}
