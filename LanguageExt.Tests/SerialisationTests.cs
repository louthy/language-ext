using System;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Process;
using Xunit;
using Newtonsoft.Json;

namespace LanguageExtTests
{
    
    public class SerialisationTests
    {
        [Fact]
        public void ProcessIdTest()
        {
            ProcessId pid = "/root/user/test";

            Assert.True(pid.Path == "/root/user/test");
            Assert.True(pid.GetName().Value == "test");
            Assert.True(pid.Child("ing").Path == "/root/user/test/ing");

            var json = JsonConvert.SerializeObject(pid);

            pid = JsonConvert.DeserializeObject<ProcessId>(json);

            Assert.True(pid.Path == "/root/user/test");
            Assert.True(pid.GetName().Value == "test");
            Assert.True(pid.Child("ing").Path == "/root/user/test/ing");
        }

        [Fact]
        public void ProcessNameTest()
        {
            ProcessName name = "test";

            Assert.True(name.Value == "test");

            var json = JsonConvert.SerializeObject(name);

            name = JsonConvert.DeserializeObject<ProcessName>(json);

            Assert.True(name.Value == "test");
        }
    }
}
