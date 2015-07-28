using System;
using LanguageExt;
using static LanguageExt.Prelude;
using static LanguageExt.Process;
using NUnit.Framework;
using Newtonsoft.Json;

namespace LanguageExtTests
{
    [TestFixture]
    public class SerialisationTests
    {
        [Test]
        public void ProcessIdTest()
        {
            ProcessId pid = "/root/user/test";

            Assert.IsTrue(pid.Path == "/root/user/test");
            Assert.IsTrue(pid.GetName().Value == "test");
            Assert.IsTrue(pid.MakeChildId("ing").Path == "/root/user/test/ing");

            var json = JsonConvert.SerializeObject(pid);

            pid = JsonConvert.DeserializeObject<ProcessId>(json);

            Assert.IsTrue(pid.Path == "/root/user/test");
            Assert.IsTrue(pid.GetName().Value == "test");
            Assert.IsTrue(pid.MakeChildId("ing").Path == "/root/user/test/ing");
        }

        [Test]
        public void ProcessNameTest()
        {
            ProcessName name = "test";

            Assert.IsTrue(name.Value == "test");

            var json = JsonConvert.SerializeObject(name);

            name = JsonConvert.DeserializeObject<ProcessName>(json);

            Assert.IsTrue(name.Value == "test");
        }
    }
}
