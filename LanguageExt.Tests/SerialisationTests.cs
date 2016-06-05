using System;
using System.Collections.Generic;
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
            Assert.True(pid.Name.Value == "test");
            Assert.True(pid.Child("ing").Path == "/root/user/test/ing");

            var json = JsonConvert.SerializeObject(pid);

            pid = JsonConvert.DeserializeObject<ProcessId>(json);

            Assert.True(pid.Path == "/root/user/test");
            Assert.True(pid.Name.Value == "test");
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

        [Fact]
        public void MapTest()
        {
            var map = LanguageExt.Map.create(
                Tuple<ProcessName,ProcessId>("test5", "/root/user/test5"),
                Tuple<ProcessName, ProcessId>("test2", "/root/user/test2"),
                Tuple<ProcessName, ProcessId>("test1", "/root/user/test1"),
                Tuple<ProcessName, ProcessId>("test3", "/root/user/test3"),
                Tuple<ProcessName, ProcessId>("test4", "/root/user/test4")
                );

            var json = JsonConvert.SerializeObject(map.Tuples);

            map = LanguageExt.Map.createRange(JsonConvert.DeserializeObject<IEnumerable<Tuple<ProcessName, ProcessId>>>(json));

            Assert.True(map.Count == 5);
            Assert.True(map.ContainsKey("test1"));
            Assert.True(map.ContainsKey("test2"));
            Assert.True(map.ContainsKey("test3"));
            Assert.True(map.ContainsKey("test4"));
            Assert.True(map.ContainsKey("test5"));
            Assert.True(map["test1"] == "/root/user/test1");
            Assert.True(map["test2"] == "/root/user/test2");
            Assert.True(map["test3"] == "/root/user/test3");
            Assert.True(map["test4"] == "/root/user/test4");
            Assert.True(map["test5"] == "/root/user/test5");
        }
    }
}
